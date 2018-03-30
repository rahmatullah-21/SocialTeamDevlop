using DominatorHouse.DominatorCores;
using DominatorHouseCore.Interfaces;

namespace DominatorHouse.Factories
{
    internal class SocialNetworkCollectionFactory : INetworkCollectionFactory
    {
        public INetworkCoreFactory GetNetworkCoreFactory()
        {
            var dominatorNetworkCoreFactory = new DominatorNetworkCoreFactory();
            var dominatorCoreBuilder = DominatorCoreBuilder.Instance(dominatorNetworkCoreFactory);
            return dominatorCoreBuilder.GetDominatorCoreObjects();
        }
    }
}