using AutoMapper;
using Legion.AutoMapping;
using Legion.Social;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.DashboardVms;
using LegionUIUtility.IoC;
using LegionUIUtility.ViewModel.OtherConfigurations;
using LegionUIUtility.ViewModel.OtherConfigurations.ThridPartyServices;
using LegionUIUtility.ViewModel.OtherTools;
using Socinator.Factories;
using Unity;
using Unity.Extension;

namespace Legion.IoC
{
    public class SocialContainerExtension : UnityContainerExtension
    {
        private const SocialNetworks CurrentyNetwork = SocialNetworks.Social;
        protected override void Initialize()
        {
            // specify name of the module (CurrentyNetwork.ToString()) when you think it will be many registration upon your interface
            Container.RegisterType<ISocialNetworkModule, MainDominatorModule>(CurrentyNetwork.ToString());
            Container.RegisterType<INetworkCollectionFactory, SocialNetworkCollectionFactory>(CurrentyNetwork.ToString());
            Container.RegisterType<IPublisherCollectionFactory, SocialPublisherCollectionFactory>(CurrentyNetwork.ToString());
            Container.RegisterType<Profile, MainMapperProfile>(CurrentyNetwork.ToString());

            // views
            // TODO: reg rid of it later
            Container.RegisterSingleton<TablifiedContentControl, TablifiedContentControl<IDashboardViewModel>>(
                "Dashboard");
            Container.RegisterSingleton<TablifiedContentControl, TablifiedContentControl<IThridPartyServicesViewModel>>(
                "ThirdPartyServices");
            Container.RegisterSingleton<TablifiedContentControl, TablifiedContentControl<IOtherConfigurationViewModel>>(
                "OtherConfiguration");
            Container.RegisterSingleton<TablifiedContentControl, TablifiedContentControl<IOtherToolsViewModel>>(
                "OtherTools");

            // viewmodels
            Container.RegisterSingleton<ITablifiedContentControlViewModel<IDashboardViewModel>, TablifiedContentControlViewModel<IDashboardViewModel>>();
            Container.RegisterSingleton<ITablifiedContentControlViewModel<IThridPartyServicesViewModel>, TablifiedContentControlViewModel<IThridPartyServicesViewModel>>();
            Container.RegisterSingleton<ITablifiedContentControlViewModel<IOtherConfigurationViewModel>, TablifiedContentControlViewModel<IOtherConfigurationViewModel>>();
            Container.RegisterSingleton<ITablifiedContentControlViewModel<IOtherToolsViewModel>, TablifiedContentControlViewModel<IOtherToolsViewModel>>();
        }
    }
}
