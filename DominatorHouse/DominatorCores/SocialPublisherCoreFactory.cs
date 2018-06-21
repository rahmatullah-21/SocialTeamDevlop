using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using FaceDominatorCore.FDViewModel.Publisher;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherCoreFactory : IPublisherCoreFactory
    {
        public SocialNetworks Network { get; set; }

        public IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        public IPublisherPostScraper PostScraper { get; set; }

        public IPublishingPost PublishingPost { get; set; }
    }
}