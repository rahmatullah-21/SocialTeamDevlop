using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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

        }

        #endregion

        #region Properties

        public string CampaignId { get; set; }

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

        private CancellationTokenSource CampaignCancellationToken { get; set; }

        private CancellationTokenSource CurrentJobCancellationToken { get; set; }

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

        public void StartPublishing(bool isRunSingleAccount)
        {
            lock (SyncJobProcess)
            {
                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    GroupDestinationList.ForEach(groupUrl =>
                    {
                        var post = GetPostModel("Group", groupUrl);
                        var ispublished = PublishOnGroups(AccountModel.AccountId, groupUrl, post);
                        if (!ispublished) return;
                        UpdatePostWithSuccessful(groupUrl, post);
                        return;
                    });

                    PageDestinationList.ForEach(pageUrl =>
                    {
                        var post = GetPostModel("Page", pageUrl);
                        var ispublished = PublishOnPages(AccountModel.AccountId, pageUrl, post);
                        if (!ispublished) return;
                        UpdatePostWithSuccessful(pageUrl, post);
                        return;
                    });

                    CustomDestinationList.ForEach(customList =>
                    {
                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);
                        var ispublished = PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                        if (!ispublished) return;
                        UpdatePostWithSuccessful(customList.DestinationValue, post);
                        return;
                    });

                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountId);
                    var isOwnWallPostpublished = PublishOnOwnWall(AccountModel.AccountId, ownWallpost);
                    if (!isOwnWallPostpublished)
                        return;
                    UpdatePostWithSuccessful(AccountModel.AccountId, ownWallpost);
                    return;
                }
                else
                {
                    var allGroupsPages = new Dictionary<string, string>();
                    GroupDestinationList.Shuffle();
                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });
                    PageDestinationList.Shuffle();
                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    allGroupsPages.ForEach(x =>
                    {
                        if (x.Value == "Group")
                        {
                            var post = GetPostModel("Group", x.Key);
                            var ispublished = PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            if (!ispublished) return;
                            UpdatePostWithSuccessful(x.Key, post);
                            return;
                        }
                        else
                        {
                            var post = GetPostModel("Page", x.Key);
                            var ispublished = PublishOnPages(AccountModel.AccountId, x.Key, post);
                            if (!ispublished) return;
                            UpdatePostWithSuccessful(x.Key, post);
                            return;
                        }
                    });

                    CustomDestinationList.Shuffle();
                    CustomDestinationList.ForEach(customList =>
                    {
                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);
                        var ispublished = PublishOnCustomDestination(AccountModel.AccountId, customList, post);
                        if (!ispublished) return;
                        UpdatePostWithSuccessful(customList.DestinationValue, post);
                        return;
                    });


                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountId);
                    var isOwnWallPostpublished = PublishOnOwnWall(AccountModel.AccountId, ownWallpost);
                    if (!isOwnWallPostpublished) return;
                    UpdatePostWithSuccessful(AccountModel.AccountId, ownWallpost);
                    return;
                }
            }
        }


        public void StartPublishing(bool isRunSingleAccount, PublisherPostlistModel post)
        {
            lock (SyncJobProcess)
            {
                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    GroupDestinationList.ForEach(groupUrl =>
                    {
                        if (!ValidateNetworkAdvancedSettings(post, "Group", groupUrl))
                            return;
                        var ispublished = PublishOnGroups(AccountModel.AccountId, groupUrl, post);


                        if (!ispublished) return;
                        UpdatePostWithSuccessful(groupUrl, post);
                        return;
                    });

                    PageDestinationList.ForEach(pageUrl =>
                    {                      
                        if (!ValidateNetworkAdvancedSettings(post, "Page", pageUrl))
                            return;
                        var ispublished = PublishOnPages(AccountModel.AccountId, pageUrl, post);
                        if (!ispublished) return;
                        UpdatePostWithSuccessful(pageUrl, post);
                        return;
                    });

                    if (!ValidateNetworkAdvancedSettings(post, "OwnWall", AccountModel.AccountId))
                        return;                 
                    var isOwnWallPostpublished = PublishOnOwnWall(AccountModel.AccountId, post);
                    if (!isOwnWallPostpublished)
                        return;
                    UpdatePostWithSuccessful(AccountModel.AccountId, post);
                    return;
                }
                else
                {
                    var allGroupsPages = new Dictionary<string, string>();
                    GroupDestinationList.Shuffle();
                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });
                    PageDestinationList.Shuffle();
                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    allGroupsPages.ForEach(x =>
                    {
                        if (x.Value == "Group")
                        {                          
                            if (!ValidateNetworkAdvancedSettings(post, "Group", x.Key))
                                return;
                            var ispublished = PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            if (!ispublished) return;
                            UpdatePostWithSuccessful(x.Key, post);
                            return;
                        }
                        else
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Page", x.Key))
                                return;                          
                            var ispublished = PublishOnPages(AccountModel.AccountId, x.Key, post);
                            if (!ispublished) return;
                            UpdatePostWithSuccessful(x.Key, post);
                            return;
                        }
                    });
                  
                    if (!ValidateNetworkAdvancedSettings(post, "OwnWall", AccountModel.AccountId))
                        return;
                    var isOwnWallPostpublished = PublishOnOwnWall(AccountModel.AccountId, post);
                    if (!isOwnWallPostpublished) return;
                    UpdatePostWithSuccessful(AccountModel.AccountId, post);
                    return;
                }
            }
        }

        private void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel post)
        {
            if (OtherConfiguration.IsEnableSignatureChecked)
            {
                post.PostDescription = post.PostDescription.Replace("\r\n", string.Empty)
                    .Replace(OtherConfiguration.SignatureText, string.Empty);
            }

            var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));
            post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
            PostlistFileManager.UpdatePost(CampaignId, post);

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
            // Todo : Delays
        }

        public PublisherPostlistModel GetPostModel(string destination, string destinationUrl)
        {
            var pendingPostList = PostlistFileManager.GetAll(CampaignId)
                .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

            if (!pendingPostList.Any())
            {
                var campaignName = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                      .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == CampaignId);
                if (campaignName == null)
                    return null;
                GlobusLogHelper.log.Info($"No more post are available for campaign {campaignName.CampaignName}!");
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
                if (loopCount >= pendingPostList.Count)
                    break;

                var filterPostModel = GeneralSettingsModel.IsChooseRandomPostsChecked ?
                    pendingPostList[RandomUtilties.GetRandomNumber(0, pendingPostList.Count - 1)] :
                    pendingPostList.FirstOrDefault(x => x.PostId != null);

                if (ValidateNetworkAdvancedSettings(filterPostModel, destination, destinationUrl))
                    return filterPostModel;

                loopCount++;
            }

            return null;
        }


        public bool ValidateNetworkAdvancedSettings(PublisherPostlistModel filterPostModel, string destination, string destinationUrl)
        {
            var allDestinations = filterPostModel.LstPublishedPostDetailsModels.Select(x => x.DestinationUrl).ToList();
            if (allDestinations.Contains(destinationUrl))
            {
                GlobusLogHelper.log.Info($"Post has already posted with destintion : {destination}-{destinationUrl} !");
                return false;
            }
                
            if (ValidateNetworksSettings(CampaignId))
            {
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

                return true;
            }
            GlobusLogHelper.log.Info($"Post has failed with {Network} network - advanced settings!");
            return false;
        }

        #endregion
    }

}