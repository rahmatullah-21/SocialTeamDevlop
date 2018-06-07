namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IFdPostSettings
    {
        bool SellPostSelected { get; set; }

        string ProductName { get; set; }

        int ProductPrice { get; set; }

        string ProductAvailableLocation { get; set; }

        bool SellPostTurnOffComment { get; set; }

        bool IsReplaceDescriptionSelected { get; set; }

        string PostReplaceDescription { get; set; }
    }
}