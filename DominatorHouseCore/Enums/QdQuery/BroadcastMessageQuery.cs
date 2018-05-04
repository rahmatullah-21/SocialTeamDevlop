using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum BroadcastMessageQuery
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers,
        [Description("langCustomUser")]
        CustomUsers,
        [Description("langCustomUrl")]
        CustomUrl
    }
}