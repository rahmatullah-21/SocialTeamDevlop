using System.Collections.Generic;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IGlobalInteractionDetails
    {
        void InitializeInteraction();

        void UpdateInteractedData();

        void ReadInteractedData();

        Dictionary<string, GlobalInteractionDataModel> CampaignInteractedCollections { get; set; }
    }
}