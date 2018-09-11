using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.IoC
{
    public interface ISocialNetworkModule
    {
        SocialNetworks Network { get; }

        INetworkCollectionFactory GetNetworkCollectionFactory(
            DominatorAccountViewModel.AccessorStrategies strategies);

        IPublisherCollectionFactory GetPublisherCollectionFactory();
    }
}
