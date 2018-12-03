using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface ICampaignInteractionDetails
    {
        CampaignInteractionDataModel this[SocialNetworks networks, string key] { get; }
        void AddInteractedData(SocialNetworks networks, string campaignId, string interactedData);
        void RemoveIfExist(SocialNetworks networks, string campaignId, string interactedData);
    }
}
