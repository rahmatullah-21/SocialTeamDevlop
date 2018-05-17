namespace DominatorHouseCore.Interfaces
{
    public interface INetworkCollectionFactory
    {
        INetworkCoreFactory GetNetworkCoreFactory();
        void InitiliazeCampaignReport();
        void InitializeEditDuplicateCampaign();
    }
}