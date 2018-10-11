using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Interfaces
{
    public interface IGlobalInteractionDetails
    {
        void InitializeInteraction();

        void UpdateInteractedData();

        void ReadInteractedData();

        Dictionary<ActivityType, GlobalInteractionDataModel> GlobalInteractedCollections { get; set; }
    }
}