using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace DominatorHouse.DominatorCores
{
    internal class DominatorNetworkCoreFactory : INetworkCoreFactory
    {

        /// <summary>
        ///     Specify the network of the dominator
        /// </summary>
        public SocialNetworks Network { get; set; }

        /// <summary>
        ///     creates job process based on social network and module
        /// </summary>
        public IJobProcessFactory JobProcessFactory { get; set; }

        /// <summary>
        ///     Scraps data from social network feed based on query (queries)
        /// </summary>
        public IQueryScraperFactory QueryScraperFactory { get; set; }

        public ITabHandlerFactory TabHandlerFactory { get; set; }

        public IAccountUpdateFactory AccountUpdateFactory { get; set; }

        public IAccountCountFactory AccountCountFactory { get; set; }

        public IAccountToolsFactory AccountUserControlTools { get; set; }

        public IDatabaseConnection AccountDatabase { get; set; }

        public IDatabaseConnection CampaignDatabase { get; set; }

    }
}