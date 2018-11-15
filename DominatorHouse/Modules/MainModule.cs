using DominatorHouse.Support.Perf.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DominatorHouse.Modules
{
    public class MainModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("PerfCounterRegion", typeof(PerfCounterView));
        }
    }
}
