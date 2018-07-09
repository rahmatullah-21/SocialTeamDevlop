using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Interfaces
{
    public interface IPublisherCoreFactory
    {
        /// <summary>
        /// Specify the network of the dominator
        /// </summary>
        SocialNetworks Network { get; set; }

        IPublisherJobProcessFactory PublisherJobFactory { get; set; }

        IPublisherPostScraper PostScraper { get; set; }

    }
}