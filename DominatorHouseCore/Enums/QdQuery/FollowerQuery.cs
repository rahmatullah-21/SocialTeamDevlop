using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum FollowerQuery
    {
        [Description("LangKeyKeywords")]
        Keywords,
        [Description("LangKeySomeonesFollowers")]
        SomeonesFollowers,
        [Description("LangKeySomeonesFollowings")]
        SomeonesFollowings,
        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowings ,
        [Description("LangKeyFollowersOfSomeonesFollowings")]
        FollowersOfFollowers ,
        [Description("LangKeyCustomUsersList")]
        CustomUsers,
    }
}