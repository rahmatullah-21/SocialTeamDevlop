using DominatorHouse.Support.Perf.Views;
using DominatorUIUtility.Views.AccountSetting;
using DominatorUIUtility.Views.AccountSetting.Activity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DominatorHouse.Modules
{
    public class MainModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            //containerRegistry.RegisterForNavigation<SelectActivity>();
            //containerRegistry.RegisterForNavigation<Follow>();
            //containerRegistry.RegisterForNavigation<JobConfig>();
            //containerRegistry.RegisterForNavigation<QueryControl>();
           
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("PerfCounterRegion", typeof(PerfCounterView));
           // regionManager.RegisterViewWithRegion("StartupRegion", typeof(SelectActivity));

        }
    }
}
