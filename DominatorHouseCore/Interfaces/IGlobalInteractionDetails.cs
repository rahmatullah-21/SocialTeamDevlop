using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IGlobalInteractionDetails
    {
        GlobalInteractionDataModel this[SocialNetworks networks, ActivityType activityType] { get; }

        void AddInteractedData(SocialNetworks networks, ActivityType activityType, string interactedData);

        void RemoveIfExist(SocialNetworks networks, ActivityType activityType, string interactedData);
    }
}