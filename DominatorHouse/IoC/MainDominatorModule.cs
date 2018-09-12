using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel;
using Unity;
using Unity.Resolution;

namespace DominatorHouse
{
    public class MainDominatorModule : ISocialNetworkModule
    {
        public SocialNetworks Network => SocialNetworks.Social;

        private readonly IUnityContainer _unityContainer;

        public MainDominatorModule(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;
        }


        public INetworkCollectionFactory GetNetworkCollectionFactory(
            DominatorAccountViewModel.AccessorStrategies strategies)
        {
            return _unityContainer.Resolve<INetworkCollectionFactory>(Network.ToString(),
                new ParameterOverrides { { "strategies", strategies } });
        }


        public IPublisherCollectionFactory GetPublisherCollectionFactory()
        {
            return _unityContainer.Resolve<IPublisherCollectionFactory>(Network.ToString());
        }
    }
}
