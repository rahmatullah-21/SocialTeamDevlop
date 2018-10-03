using AutoMapper;
using DominatorHouse.AutoMapping;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.IoC;
using Socinator.Factories;
using Unity;
using Unity.Extension;

namespace DominatorHouse.IoC
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
        }
    }
}
