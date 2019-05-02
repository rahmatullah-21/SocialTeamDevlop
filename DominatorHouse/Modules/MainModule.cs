using DominatorHouse.Startup;
using DominatorHouse.Support.Perf.Views;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System.Windows;

namespace DominatorHouse.Modules
{
    public class MainModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectUserType>();
            containerRegistry.RegisterForNavigation<SelectNetwork>();
            containerRegistry.RegisterForNavigation<SaveSetting>();
            containerRegistry.RegisterForNavigation<SelectActivity>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("PerfCounterRegion", typeof(PerfCounterView));
            regionManager.RegisterViewWithRegion("StartupRegion", typeof(SelectUserType));
            
        }
    }
}
