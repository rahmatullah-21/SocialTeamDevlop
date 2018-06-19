using DominatorHouseCore.BusinessLogic.Scheduler;

namespace DominatorHouseCore.Interfaces
{
    public interface IPublisherCoreFactory
    {
        IPublisherJobProcessFactory PublisherFactory { get; set; }

        PostScraper PostScraper { get; set; }

        PublishingPost PublishingPost { get; set; }
    }
}