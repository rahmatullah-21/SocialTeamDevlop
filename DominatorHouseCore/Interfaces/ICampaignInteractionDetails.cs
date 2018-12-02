using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces
{
    public interface ICampaignInteractionDetails
    {
        void InitializeInteraction();

        void UpdateInteractedData();

        void ReadInteractedData();

        Dictionary<string, CampaignInteractionDataModel> CampaignInteractedCollections { get; set; }

        CampaignInteractedUtility CampaignInteractedUtility { get; }
    }
}
