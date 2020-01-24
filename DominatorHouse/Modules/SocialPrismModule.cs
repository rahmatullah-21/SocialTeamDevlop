using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Legion.Modules
{
    public class SocialPrismModule : IModule
    {
        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            // ReSharper disable once UnusedVariable
            var regionManager = containerProvider.Resolve<IRegionManager>();
        }
    }
}
