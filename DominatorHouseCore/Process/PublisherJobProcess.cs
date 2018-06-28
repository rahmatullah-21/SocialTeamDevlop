using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;
using ProtoBuf;

namespace DominatorHouseCore.Process
{
    public abstract class PublisherJobProcess
    {
        #region Constructor

        public PublisherJobProcess(string campaignId,
            string accountId,
            SocialNetworks network,
            List<string> groupDestinationLists,
            List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource camapignCancellationToken)
        {
            CampaignId = campaignId;

            Network = network;

            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                       (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            AccountModel = AccountsFileManager.GetAccountById(accountId);

            PageDestinationList = pageDestinationList;

            GroupDestinationList = groupDestinationLists;

            CustomDestinationList = customDestinationModels;

            IsPublishOnOwnWall = isPublishOnOwnWall;

            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = camapignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token, CurrentJobCancellationToken.Token);

            var campaign = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == CampaignId);

            CampaignName = campaign?.CampaignName;

        }

        #endregion

        #region Properties

        public string CampaignId { get; set; }

        public string CampaignName { get; set; }

        public SocialNetworks Network { get; set; }

        public JobConfigurationModel JobConfigurations { get; set; }

        public OtherConfigurationModel OtherConfiguration { get; set; }

        public GeneralModel GeneralSettingsModel { get; set; }

        private static readonly object SyncJobProcess = new object();

        public DominatorAccountModel AccountModel { get; set; }

        public List<string> GroupDestinationList { get; set; }

        public List<string> PageDestinationList { get; set; }

        public List<PublisherCustomDestinationModel> CustomDestinationList { get; set; }

        public bool IsPublishOnOwnWall { get; set; }

        public CancellationTokenSource CampaignCancellationToken { get; set; }

        public CancellationTokenSource CurrentJobCancellationToken { get; set; }

        public CancellationTokenSource CombinedCancellationToken { get; set; }

        public virtual bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails) => false;

        public virtual bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails) => false;

        public virtual bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails) => false;

        public virtual bool PublishOnCustomDestination(string accountId,
            PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails) => false;

        #endregion

        #region Methods

        protected abstract bool ValidateNetworksSettings(string campaign);

        public virtual bool DeletePost(string postId) => true;

        public void StartPublishing()
        {
            lock (SyncJobProcess)
            {
                try
                {
                    GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,CampaignName);

                    if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                    {
                        #region Publish on Groups

                        GroupDestinationList.ForEach(groupUrl =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            var post = GetPostModel("Group", groupUrl);
                            if (post == null)
                                return;
                            PublishOnGroups(AccountModel.AccountId, groupUrl, post);                            
                        });

                        #endregion

                        #region Publish on Pages

                        PageDestinationList.ForEach(pageUrl =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            var post = GetPostModel("Page", pageUrl);
                            if (post == null)
                                return;
                            PublishOnPages(AccountModel.AccountId, pageUrl, post);
                            
                        });

                        #endregion

                        #region Custom Destination

                        CustomDestinationList.ForEach(customList =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            var post = GetPostModel(customList.DestinationType, customList.DestinationValue);
                            if (post == null)
                                return;
                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                            
                        });

                        #endregion
                    }
                    else
                    {
                        #region Shuffle the groups and pages

                        var allGroupsPages = new Dictionary<string, string>();

                        GroupDestinationList.Shuffle();
                        PageDestinationList.Shuffle();

                        GroupDestinationList.ForEach(x =>
                        {
                            allGroupsPages.Add(x, "Group");
                        });

                        PageDestinationList.ForEach(x =>
                        {
                            allGroupsPages.Add(x, "Page");
                        });

                        allGroupsPages.ForEach(x =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            if (x.Value == "Group")
                            {
                                var post = GetPostModel("Group", x.Key);
                                if (post == null)
                                    return;
                                PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            }
                            else
                            {
                                var post = GetPostModel("Page", x.Key);
                                if (post == null)
                                    return;
                                PublishOnPages(AccountModel.AccountId, x.Key, post);
                            }
                            
                        });



                        #endregion

                        #region Shuffle custom destination

                        CustomDestinationList.Shuffle();
                        CustomDestinationList.ForEach(customList =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                            if (post == null)
                                return;

                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                            
                        });

                        #endregion
                    }

                    #region Own Wall

                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountBaseModel.UserName);
                    if (ownWallpost == null)
                        return;
                    PublishOnOwnWall(AccountModel.AccountId, ownWallpost);

                    #endregion
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            e.DebugLog("Cancellation requested before task completion!");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public void StartPublishing(PublisherPostlistModel post)
        {
            lock (SyncJobProcess)
            {
                GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);

                try
                {
                    if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                    {
                        #region Publish on Groups

                        GroupDestinationList.ForEach(groupUrl =>
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Group", groupUrl))
                                return;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            PublishOnGroups(AccountModel.AccountId, groupUrl, post);

                            
                        });

                        #endregion

                        #region Publish on Pages

                        PageDestinationList.ForEach(pageUrl =>
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Page", pageUrl))
                                return;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            PublishOnPages(AccountModel.AccountId, pageUrl, post);

                            
                        });

                        #endregion

                        #region Publish on Custom destination

                        CustomDestinationList.ForEach(customList =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                            
                        });

                        #endregion                 
                    }
                    else
                    {
                        #region Shuffle Groups and pages

                        var allGroupsPages = new Dictionary<string, string>();

                        GroupDestinationList.Shuffle();
                        PageDestinationList.Shuffle();

                        GroupDestinationList.ForEach(x =>
                        {
                            allGroupsPages.Add(x, "Group");
                        });

                        PageDestinationList.ForEach(x =>
                        {
                            allGroupsPages.Add(x, "Page");
                        });

                        allGroupsPages.ForEach(x =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            if (x.Value == "Group")
                            {
                                if (!ValidateNetworkAdvancedSettings(post, "Group", x.Key))
                                    return;
                                PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            }
                            else
                            {
                                if (!ValidateNetworkAdvancedSettings(post, "Page", x.Key))
                                    return;
                                PublishOnPages(AccountModel.AccountId, x.Key, post);
                            }
                            
                        });


                        #endregion

                        #region Custom Destination

                        CustomDestinationList.Shuffle();
                        CustomDestinationList.ForEach(customList =>
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();
                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);                            
                        });

                        #endregion
                    }

                    #region OwnWall

                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    if (!ValidateNetworkAdvancedSettings(post, "OwnWall", AccountModel.AccountId))
                        return;

                    PublishOnOwnWall(AccountModel.AccountId, post);

                    #endregion

                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            e.DebugLog("Cancellation requested before task completion!");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel post)
        {
            if (OtherConfiguration.IsEnableSignatureChecked)
            {
                post.PostDescription = post.PostDescription.Replace("\r\n", string.Empty)
                    .Replace(OtherConfiguration.SignatureText, string.Empty);
            }

            var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));
            post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
            PostlistFileManager.UpdatePost(CampaignId, post);
            PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
        }

        public void Stop()
        {
            try
            {
                CurrentJobCancellationToken.Cancel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DelayBeforeNextPublish()
        {

            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenPost.StartValue,
                  JobConfigurations.DelayBetweenPost.EndValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, delay);

            Thread.Sleep(delay * 1000);
        }

        public PublisherPostlistModel GetPostModel(string destination, string destinationUrl)
        {
            try
            {
                CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                var pendingPostList = PostlistFileManager.GetAll(CampaignId)
                        .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

                if (!pendingPostList.Any())
                {                  
                    GlobusLogHelper.log.Info($"No more post are available for campaign {CampaignName}!");
                    return null;
                }

                if (GeneralSettingsModel.IsWhenPublishingSendOnePostChecked)
                    pendingPostList = pendingPostList.Where(x => x.LstPublishedPostDetailsModels.Count == 0).ToList();
                else
                    pendingPostList = (from postlist in pendingPostList
                                       let allDestinations = postlist.LstPublishedPostDetailsModels.Select(x => x.DestinationUrl).ToList()
                                       where !allDestinations.Contains(destinationUrl)
                                       select postlist).ToList();

                var loopCount = 0;

                while (true)
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    if (loopCount >= pendingPostList.Count)
                        break;

                    var filterPostModel = GeneralSettingsModel.IsChooseRandomPostsChecked ?
                        pendingPostList[RandomUtilties.GetRandomNumber(0, pendingPostList.Count - 1)] :
                        pendingPostList.FirstOrDefault(x => x.PostId != null);

                    if (ValidateNetworkAdvancedSettings(filterPostModel, destination, destinationUrl))
                    {
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
                        return filterPostModel;
                    }

                    loopCount++;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        throw new AggregateException("Cancellation requested before task completion!");
                    else
                        throw new AggregateException(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        public bool ValidateNetworkAdvancedSettings(PublisherPostlistModel filterPostModel, string destination, string destinationUrl)
        {
            try
            {
                var allDestinations = filterPostModel.LstPublishedPostDetailsModels.Select(x => x.DestinationUrl).ToList();

                if (allDestinations.Contains(destinationUrl))
                {
                   
                    GlobusLogHelper.log.Info(destination == "OwnWall"
                        ? string.Format(Log.AlreadyPublishedOnOwnWall, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName)
                        : string.Format(Log.AlreadyPublishedOnDestination, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, destination, destinationUrl));
                    return false;
                }

                filterPostModel?.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                {
                    AccountName = AccountModel.AccountBaseModel.UserName,
                    Destination = destination,
                    DestinationUrl = destinationUrl,
                    Description = filterPostModel.PostDescription,
                    IsPublished = ConstantVariable.Yes,
                    Successful = ConstantVariable.No,
                    PublishedDate = DateTime.Now.ToString("dd/mm/yy"),
                    Link = ConstantVariable.NotPublished
                });

                filterPostModel.PostQueuedStatus = PostQueuedStatus.Published;
                PostlistFileManager.UpdatePost(CampaignId, filterPostModel);

                if (OtherConfiguration.IsEnableSignatureChecked)
                {
                    filterPostModel.PostDescription = filterPostModel.PostDescription + "\r\n" +
                                                      OtherConfiguration.SignatureText;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        #endregion
    }

}