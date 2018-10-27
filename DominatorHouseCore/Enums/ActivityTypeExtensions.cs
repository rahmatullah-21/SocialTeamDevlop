using DominatorHouseCore.Converters;

namespace DominatorHouseCore.Enums
{
    public static class ActivityTypeExtensions
    {
        public static bool IsSupportedByNetwork(this ActivityType activityType, SocialNetworks network)
        {
            return EnumDescriptionConverter.GetDescription(activityType).Contains(network.ToString());
        }
    }
}
