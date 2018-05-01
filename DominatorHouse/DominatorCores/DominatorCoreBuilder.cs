using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.ViewModel;



namespace DominatorHouse.DominatorCores
{
    public class DominatorCoreBuilder : NetworkCoreLibraryBuilder
    {
        private static DominatorCoreBuilder _instance;
        private static DominatorAccountViewModel.AccessorStrategies _strategies;
        public static DominatorAccountViewModel.AccessorStrategies Strategies
        {
            set
            {
                _strategies = value;
            }
        }

        public static DominatorCoreBuilder Instance(INetworkCoreFactory networkCoreFactory)
            => _instance ?? (_instance = new DominatorCoreBuilder(networkCoreFactory));

        private DominatorCoreBuilder(INetworkCoreFactory networkCoreFactory)
            : base(networkCoreFactory)
        {
            AddNetwork(SocialNetworks.Social)
                .AddTabFactory(DominatorTabHandlerFactory.GetInstance(_strategies))
                .AddJobFactory(DominatorJobProcessFactory.Instance)
                .AddScraperFactory(DominatorScraperFactory.Instance)
                .AddAccountCounts(DominatorAccountCountFactory.Instance)
                .AddAccountUiTools(DominatorAccountToolsFactory.Instance);
        }

        public INetworkCoreFactory GetDominatorCoreObjects() => NetworkCoreFactory;
    }
}