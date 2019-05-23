using DominatorUIUtility.Views.AccountSetting;
using DominatorUIUtility.Views.AccountSetting.Activity;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DominatorUIUtility.Module
{
    public class UiModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SelectActivity>();
            containerRegistry.RegisterForNavigation<Follow>();
            containerRegistry.RegisterForNavigation<Unfollow>();
            containerRegistry.RegisterForNavigation<Like>();
            containerRegistry.RegisterForNavigation<Comment>();
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("StartupRegion", typeof(SelectActivity));
        }
    }
}
