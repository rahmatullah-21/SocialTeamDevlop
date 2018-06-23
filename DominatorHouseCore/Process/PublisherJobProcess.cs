using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Process
{
    public abstract class PublisherJobProcess
    {
        #region Constructor

        public PublisherJobProcess(string campaignId, string accountId, SocialNetworks network, List<string> groupDestinationLists, List<string> pageDestinationList, bool isPublishOnOwnWall)
        {

            CampaignId = campaignId;

            Network = network;

            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                       (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            AccountModel = AccountsFileManager.GetAccountById(accountId);

            PageDestinationList = pageDestinationList;

            GroupDestinationList = groupDestinationLists;

            IsPublishOnOwnWall = isPublishOnOwnWall;

            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;
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

        public bool IsPublishOnOwnWall { get; set; }

        #endregion

        #region Methods

        protected abstract bool ValidateNetworksSettings(string campaign);

        public void StartPublishing(bool isRunSingleAccount)
        {
            lock (SyncJobProcess)
            {
                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {

                    GroupDestinationList.ForEach(groupUrl =>
                    {
                        var post = GetPostModel("Group", groupUrl);
                        var ispublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                             .GetPublishingPostLibrary().PublishOnGroups(AccountModel.AccountId, groupUrl, post);
                        if (ispublished)
                        {
                            // update post list
                            return;
                        }
                    });

                    PageDestinationList.ForEach(pageUrl =>
                    {
                        var post = GetPostModel("Page", pageUrl);
                        var ispublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                            .GetPublishingPostLibrary().PublishOnPages(AccountModel.AccountId, pageUrl, post);
                        if (ispublished)
                        {
                            // update post list
                            return;
                        }
                    });

                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountId);
                    var isOwnWallPostpublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                        .GetPublishingPostLibrary().PublishOnOwnWall(AccountModel.AccountId, ownWallpost);
                    if (isOwnWallPostpublished)
                    {
                        // update post list
                        return;
                    }

                }
                else
                {

                    var allGroupsPages = new Dictionary<string,string>();
                    GroupDestinationList.Shuffle();
                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x,"Group");
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
                            var ispublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                                .GetPublishingPostLibrary().PublishOnGroups(AccountModel.AccountId, x.Key, post);
                            if (ispublished)
                            {
                                // update post list
                                return;
                            }
                        }
                        else
                        {
                            var post = GetPostModel("Page", x.Key);
                            var ispublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                                .GetPublishingPostLibrary().PublishOnPages(AccountModel.AccountId, x.Key, post);
                            if (ispublished)
                            {
                                // update post list
                                return;
                            }
                        }                    
                    });
                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountId);
                    var isOwnWallPostpublished = PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost
                        .GetPublishingPostLibrary().PublishOnOwnWall(AccountModel.AccountId, ownWallpost);
                    if (isOwnWallPostpublished)
                    {
                        // update post list
                        return;
                    }
                }
               
                //PublisherInitialize.GetPublisherLibrary(Network).GetPublisherCoreFactory().PublishingPost.GetPublishingPostLibrary()
            }
        }

        public static void Stop()
        {
            // Todo : Stop publish with cancellation token
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
                return null;

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
                if(loopCount >= pendingPostList.Count)
                    break;

                var filterPostModel = GeneralSettingsModel.IsChooseRandomPostsChecked ?
                    pendingPostList[RandomUtilties.GetRandomNumber(0, pendingPostList.Count - 1)] :
                    pendingPostList.FirstOrDefault(x => x.PostId != null);

                if (ValidateNetworksSettings(CampaignId))
                {
                    filterPostModel?.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel()
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

                    if (filterPostModel == null)
                        return null;                    
                    filterPostModel.PostQueuedStatus = PostQueuedStatus.Published;
                    PostlistFileManager.UpdatePost(CampaignId, filterPostModel);
                    return filterPostModel;
                }

                loopCount++;
            }

            return null;
        }

        #endregion
    }
}