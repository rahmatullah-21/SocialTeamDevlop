using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using FaceDominatorCore.FDFactories;
using FaceDominatorUI.FdCoreLibrary;

namespace DominatorHouse.DominatorCores
{
    public class DominatorCoreBuilder : NetworkCoreLibraryBuilder
    {    
        private static DominatorCoreBuilder _instance;

        public static DominatorCoreBuilder Instance => _instance ?? (_instance = new DominatorCoreBuilder());

        private DominatorCoreBuilder()
        {
            AddNetwork(SocialNetworks.Social)
                .AddTabFactory(DominatorTabHandlerFactory.Instance)
                .AddJobFactory(DominatorJobProcessFactory.Instance)
                .AddScraperFactory(DominatorScraperFactory.Instance);          
        }

        public NetworkCoreLibrary GetDominatorCoreObjects() => NetworkCoreLibrary;
    }
}