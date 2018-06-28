using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DominatorHouseCore.Patterns;
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

        public int PublishedCount { get; set; }


        #endregion

        #region Methods

        protected abstract bool ValidateNetworksSettings(string campaign);

        public virtual bool DeletePost(string postId) => true;

        public void StartPublishing(int maximumPostCount)
        {
            lock (SyncJobProcess)
            {
                PublishedCount = 0;
                var isReachedMaximumCount = false;
                var isNoPostAvailable = false;

                try
                {
                    GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);

                    if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                    {
                        #region Publish on Groups

                        foreach (var groupUrl in GroupDestinationList)
                        {
                            if (PublishedCount > maximumPostCount)
                                isReachedMaximumCount = true;

                            if (isReachedMaximumCount)
                                break;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            var post = GetPostModel("Group", groupUrl);

                            if (post == null)
                            {
                                isNoPostAvailable = true;
                                break;
                            }

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{groupUrl}]");

                            PublishOnGroups(AccountModel.AccountId, groupUrl, post);
                            PublishedCount++;
                        }

                        #endregion

                        #region Publish on Pages

                        if (isReachedMaximumCount || isNoPostAvailable)
                            return;

                        foreach (var pageUrl in PageDestinationList)
                        {
                            if (PublishedCount > maximumPostCount)
                                isReachedMaximumCount = true;

                            if (isReachedMaximumCount)
                                break;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            var post = GetPostModel("Page", pageUrl);

                            if (post == null)
                            {
                                isNoPostAvailable = true;
                                break;
                            }

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{pageUrl}]");

                            PublishOnPages(AccountModel.AccountId, pageUrl, post);

                            PublishedCount++;
                        }

                        #endregion

                        #region Custom Destination

                        if (isReachedMaximumCount || isNoPostAvailable)
                            return;

                        foreach (var customList in CustomDestinationList)
                        {
                            if (PublishedCount > maximumPostCount)
                                isReachedMaximumCount = true;

                            if (isReachedMaximumCount)
                                break;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType} [{customList.DestinationValue}]");

                            var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                            if (post == null)
                            {
                                isNoPostAvailable = true;
                                break;
                            }

                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);

                            PublishedCount++;
                        }

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

                        foreach (var destination in allGroupsPages)
                        {
                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            if (isReachedMaximumCount)
                                break;

                            if (destination.Value == "Group")
                            {
                                var post = GetPostModel("Group", destination.Key);

                                if (post == null)
                                {
                                    isNoPostAvailable = true;
                                    break;
                                }

                                GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{destination.Key}]");

                                PublishOnGroups(AccountModel.AccountId, destination.Key, post);
                            }
                            else
                            {
                                var post = GetPostModel("Page", destination.Key);

                                if (post == null)
                                {
                                    isNoPostAvailable = true;
                                    break;
                                }

                                GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{destination.Key}]");

                                PublishOnPages(AccountModel.AccountId, destination.Key, post);
                            }
                            PublishedCount++;
                        }

                        #endregion

                        #region Shuffle custom destination

                        if (isReachedMaximumCount || isNoPostAvailable)
                            return;

                        CustomDestinationList.Shuffle();

                        foreach (var customList in CustomDestinationList)
                        {
                            if (PublishedCount > maximumPostCount)
                                isReachedMaximumCount = true;

                            if (isReachedMaximumCount)
                                break;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                            if (post == null)
                            {
                                isNoPostAvailable = true;
                                break;
                            }

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType} [{customList.DestinationValue}]");

                            PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                            PublishedCount++;
                        }

                        #endregion
                    }

                    #region Own Wall

                    if (PublishedCount > maximumPostCount)
                        isReachedMaximumCount = true;

                    if (isReachedMaximumCount || isNoPostAvailable)
                        return;

                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountBaseModel.UserName);

                    if (ownWallpost == null)
                        return;

                    GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "Own wall");

                    PublishOnOwnWall(AccountModel.AccountId, ownWallpost);

                    PublishedCount++;

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

        public void StartPublishing(PublisherPostlistModel post, int count)
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
                            if (!ValidateNetworkAdvancedSettings(post, "Group", groupUrl, true))
                                return;

                            CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                            PublishOnGroups(AccountModel.AccountId, groupUrl, post);


                        });

                        #endregion

                        #region Publish on Pages

                        PageDestinationList.ForEach(pageUrl =>
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Page", pageUrl, true))
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
                                if (!ValidateNetworkAdvancedSettings(post, "Group", x.Key, true))
                                    return;
                                PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            }
                            else
                            {
                                if (!ValidateNetworkAdvancedSettings(post, "Page", x.Key, true))
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

                    if (!ValidateNetworkAdvancedSettings(post, "OwnWall", AccountModel.AccountBaseModel.UserName, true))
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

        public void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel post, string publishedUrl)
        {

            var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));
            post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
            post.LstPublishedPostDetailsModels[postIndex].Link = publishedUrl;
            PostlistFileManager.UpdatePost(CampaignId, post);
            PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
        }


        public PublisherPostlistModel PerformGeneralSettings(PublisherPostlistModel givenPostModel)
        {

            var postModelWithGeneralSettings = givenPostModel.DeepClone();

            #region Fetch Post media list

            if (GeneralSettingsModel.IsChooseSingleRandomImageChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                {
                    var randomNumber = RandomUtilties.GetRandomNumber(0, postModelWithGeneralSettings.MediaList.Count);
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[randomNumber] };
                }
            }
            else if (GeneralSettingsModel.IsChooseOnlyFirstImageChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[0] };
            }
            else if (GeneralSettingsModel.IsChooseBetweenChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                {
                    var randomNumber = RandomUtilties.GetRandomNumber(GeneralSettingsModel.ChooseBetween.StartValue, GeneralSettingsModel.ChooseBetween.EndValue);
                    if (randomNumber < givenPostModel.MediaList.Count)
                    {
                        givenPostModel.MediaList.Shuffle();
                        postModelWithGeneralSettings.MediaList = new ObservableCollection<string>(givenPostModel.MediaList.Take(randomNumber));
                    }                   
                }
            }

            var removeVideoExtension = new ObservableCollection<string>();

            foreach (var media in postModelWithGeneralSettings.MediaList)
            {                
                removeVideoExtension.Add(media.Replace(ConstantVariable.VideoToImageConvertFileName,string.Empty));
            }

            postModelWithGeneralSettings.MediaList = removeVideoExtension;

            #endregion

            #region Remove link

            if (GeneralSettingsModel.IsRemoveLinkFromPostsChecked)
            {
                postModelWithGeneralSettings.PostDescription = Utilities.RemoveUrls(givenPostModel.PostDescription);
            }

            #endregion

            #region Shorten Url

            if (OtherConfiguration.IsShortenURLsChecked)
            {
                postModelWithGeneralSettings.PostDescription = Utilities.ReplaceWithShortenUrl(postModelWithGeneralSettings.PostDescription);
            }

            #endregion

            #region Adding Signature

            if (OtherConfiguration.IsEnableSignatureChecked)
            {
                postModelWithGeneralSettings.PostDescription = postModelWithGeneralSettings.PostDescription + "\r\n" +
                                                  OtherConfiguration.SignatureText;
            }

            #endregion
          
            return postModelWithGeneralSettings;

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

        public bool ValidateNetworkAdvancedSettings(PublisherPostlistModel filterPostModel, string destination, string destinationUrl, bool isDirectPost = false)
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
                    DestinationUrl = destination == "OwnWall" ? AccountModel.AccountBaseModel.UserName : destinationUrl,
                    Description = filterPostModel.PostDescription,
                    IsPublished = ConstantVariable.Yes,
                    Successful = ConstantVariable.No,
                    PublishedDate = DateTime.Now.ToString("dd/mm/yy"),
                    Link = ConstantVariable.NotPublished
                });

                filterPostModel.PostQueuedStatus = PostQueuedStatus.Published;

                var triedCount =
                    filterPostModel.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                var successCount =
                    filterPostModel.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                filterPostModel.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                filterPostModel.PostRunningStatus = DateTime.Now > filterPostModel.ExpiredTime
                    ? PostRunningStatus.Completed
                    : PostRunningStatus.Active;

                PostlistFileManager.UpdatePost(CampaignId, filterPostModel);

                if (filterPostModel.PostRunningStatus == PostRunningStatus.Completed)
                {
                    if (isDirectPost)
                        GlobusLogHelper.log.Info(Log.PostExpired, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName);
                    return false;
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