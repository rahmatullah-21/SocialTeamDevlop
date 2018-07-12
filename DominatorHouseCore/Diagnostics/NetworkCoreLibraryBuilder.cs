using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;

namespace DominatorHouseCore.Diagnostics
{
    public class NetworkCoreLibraryBuilder
    {
        public NetworkCoreLibraryBuilder()
        {
        }
        public NetworkCoreLibraryBuilder(INetworkCoreFactory networkCoreFactory)
        {
            NetworkCoreFactory = networkCoreFactory;
        }

        public INetworkCoreFactory NetworkCoreFactory { get; set; }

        public NetworkCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            NetworkCoreFactory.Network = networks;
            return this;
        }

        public NetworkCoreLibraryBuilder AddJobFactory(IJobProcessFactory jobProcessFactory)
        {
            NetworkCoreFactory.JobProcessFactory = jobProcessFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddScraperFactory(IQueryScraperFactory scraperFactory)
        {
            NetworkCoreFactory.QueryScraperFactory = scraperFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddTabFactory(ITabHandlerFactory tabFactory)
        {
            NetworkCoreFactory.TabHandlerFactory = tabFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountFactory(IAccountUpdateFactory accountUpdate)
        {
            NetworkCoreFactory.AccountUpdateFactory = accountUpdate;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountCounts(IAccountCountFactory accountCount)
        {
            NetworkCoreFactory.AccountCountFactory = accountCount;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountUiTools(IAccountToolsFactory accountUserControl)
        {
            NetworkCoreFactory.AccountUserControlTools = accountUserControl;
            return this;
        }


        public NetworkCoreLibraryBuilder AddAccountSelectors(IDestinationSelectors destinationSelectors)
        {
            NetworkCoreFactory.AccountDetailsSelectors = destinationSelectors;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountDbConnection(IDatabaseConnection accountDbConnection)
        {
            NetworkCoreFactory.AccountDatabase = accountDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddCampaignDbConnection(IDatabaseConnection campaignDbConnection)
        {
            NetworkCoreFactory.CampaignDatabase = campaignDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddReportFactory(IReportFactory reportFactory)
        {
            NetworkCoreFactory.ReportFactory = reportFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddViewCampaignFactory(IViewCampaignsFactory viewCampaigns)
        {
            NetworkCoreFactory.ViewCampaigns = viewCampaigns;
            return this;
        }

        public NetworkCoreLibraryBuilder AddCampaignInteractedDetailsFactory(ICampaignInteractionDetails campaignInteractionDetails)
        {
            NetworkCoreFactory.CampaignInteractionDetails = campaignInteractionDetails;
            return this;
        }
        public NetworkCoreLibraryBuilder AddAccountVerificationFactory(IAccountVerificationFactory accountVerification)
        {
            NetworkCoreFactory.AccountVerificationFactory = accountVerification;
            return this;
        }
    }
}