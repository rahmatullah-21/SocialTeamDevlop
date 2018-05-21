using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Interfaces
{
    public interface INetworkCoreFactory
    {
        /// <summary>
        ///     Specify the network of the dominator
        /// </summary>
        SocialNetworks Network { get; set; }

        /// <summary>
        ///     creates job process based on social network and module
        /// </summary>
        IJobProcessFactory JobProcessFactory { get; set; }

        /// <summary>
        ///     Scraps data from social network feed based on query (queries)
        /// </summary>
       // IScraperFactory QueryScraperFactory { get; set; }

        IQueryScraperFactory QueryScraperFactory { get; set; }

        ITabHandlerFactory TabHandlerFactory { get; set; }

        IAccountUpdateFactory AccountUpdateFactory { get; set; }

        IAccountCountFactory AccountCountFactory { get; set; }

        IAccountToolsFactory AccountUserControlTools { get; set; }


        IDestinationSelectors AccountDetailsSelectors { get; set; }

        IDatabaseConnection AccountDatabase { get; set; }

        IDatabaseConnection CampaignDatabase { get; set; }

        IReportFactory ReportFactory { get; set; }

        IViewCampaignsFactory ViewCampaigns { get; set; }


    }
}