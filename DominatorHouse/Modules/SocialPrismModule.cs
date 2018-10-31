using DominatorHouseCore.Enums;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Socinator.Social.AutoActivity.Views;

namespace DominatorHouse.Modules
{
    public class SocialPrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<SocialAutoActivity>($"{SocialNetworks.Social}AutoActivity");
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            //regionManager.RegisterViewWithRegion($"AutoActivity", typeof(SocialAutoActivity));
        }
    }
}
