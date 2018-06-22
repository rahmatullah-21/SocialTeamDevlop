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
                    .GetPublisherCampaignFile()).FirstOrDefault(x=> x.CampaignId == CampaignId);

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

        protected abstract void StartPublish();

        public void StartPublishing(bool isRunSingleAccount)
        {
            lock (SyncJobProcess)
            {
                StartPublish();

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

        public PublisherPostlistModel GetPostModel()
        {
            var pendingPostList = PostlistFileManager.GetAll(CampaignId)
                .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending);

            if (!pendingPostList.Any())
            {
                return null;
            }

            return null;
        }

        #endregion

    }
}