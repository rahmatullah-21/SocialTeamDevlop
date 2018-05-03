using System.ComponentModel;

namespace DominatorHouseCore.Enums.QdQuery
{
    public enum FollowerQuery
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers,
        [Description("QDlangSomeonesFollowings")]
        SomeonesFollowings,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings ,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers ,
        [Description("langCustomUser")]
        CustomUsers,
    }
}