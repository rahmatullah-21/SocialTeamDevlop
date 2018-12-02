using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces
{
    public interface ICampaignInteractionDetails
    {
        void InitializeInteraction(SocialNetworks networks);

        void UpdateInteractedData(SocialNetworks networks);

        void ReadInteractedData(SocialNetworks networks);

        Dictionary<string, CampaignInteractionDataModel> CampaignInteractedCollections { get; }

        CampaignInteractedUtility CampaignInteractedUtility { get; }
    }
}
