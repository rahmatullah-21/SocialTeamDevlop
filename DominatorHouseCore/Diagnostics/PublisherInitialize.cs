using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using ConstantVariable = DominatorHouseCore.Utility.ConstantVariable;

namespace DominatorHouseCore.Diagnostics
{

    public class PublisherInitialize
    {
        private PublisherInitialize()
        {  }

        #region Properties


        private static Dictionary<SocialNetworks, IPublisherCollectionFactory> NetworkWisePublishers { get; } =
            new Dictionary<SocialNetworks, IPublisherCollectionFactory>();


        private static PublisherInitialize _publisherInitialize = null;

        public static PublisherInitialize GetInstance => _publisherInitialize ?? (_publisherInitialize = new PublisherInitialize());

        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels { get; set; } = new ObservableCollection<PublisherCampaignStatusModel>();

        #endregion

        public static void SaveNetworkPublisher(IPublisherCollectionFactory publisherCollectionFactory, SocialNetworks networks)
        {
            if (NetworkWisePublishers.ContainsKey(networks))
                return;

            NetworkWisePublishers.Add(networks, publisherCollectionFactory);
        }

        public static IPublisherCollectionFactory GetPublisherLibrary(SocialNetworks networks)
        {
            return NetworkWisePublishers.ContainsKey(networks) ? NetworkWisePublishers[networks] : null;
        }
 
        public ObservableCollection<PublisherCampaignStatusModel> GetSavedCampaigns() 
            => ListPublisherCampaignStatusModels;

        public void PublishCampaignInitializer()
        {
            var allCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            allCampaign.ForEach(campaigns =>
            {
                var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                {
                    CampaignName = campaigns.CampaignName,
                    CampaignId = campaigns.CampaignId,
                    StartDate = campaigns.JobConfigurations.CampaignStartDate,
                    EndDate = campaigns.JobConfigurations.CampaignEndDate,
                    CreatedDate = campaigns.CreatedDate,
                    Status = campaigns.CampaignStatus,
                    DestinationCount = campaigns.LstDestinationId.Count,
                    IsRotateDayChecked = campaigns.JobConfigurations.IsRotateDayChecked,
                    TimeRange = campaigns.JobConfigurations.TimeRange,
                    SpecificRunningTime = campaigns.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                    ScheduledWeekday = campaigns.JobConfigurations.Weekday,
                    IsRunSingleAccountPerCampaign = campaigns.IsRunSingleAccountPerCampaign
                };

                ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
               
            });

        }

    }

    public class PublisherCoreLibraryBuilder
    {
        public PublisherCoreLibraryBuilder()
        {
        }
        public PublisherCoreLibraryBuilder(IPublisherCoreFactory publisherCoreFactory)
        {
            PublisherCoreFactory = publisherCoreFactory;
        }

        public IPublisherCoreFactory PublisherCoreFactory { get; set; }

        public PublisherCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            PublisherCoreFactory.Network = networks;
            return this;
        }
  
        public PublisherCoreLibraryBuilder AddPublisherJobFactory(IPublisherJobProcessFactory jobFactory)
        {
            PublisherCoreFactory.PublisherJobFactory = jobFactory;
            return this;
        }

        public PublisherCoreLibraryBuilder AddPostScraper(IPublisherPostScraper postScraper)
        {
            PublisherCoreFactory.PostScraper = postScraper;
            return this;
        }     
    }

}