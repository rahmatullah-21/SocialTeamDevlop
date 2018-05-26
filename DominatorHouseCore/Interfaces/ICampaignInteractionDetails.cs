using System.Collections.Generic;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface ICampaignInteractionDetails
    {
        void InitializeInteraction();

        void UpdateInteractedData();

        void ReadInteractedData();

        Dictionary<string, CampaignInteractionDataModel> CampaignInteractedCollections { get; set; }
    }
}
