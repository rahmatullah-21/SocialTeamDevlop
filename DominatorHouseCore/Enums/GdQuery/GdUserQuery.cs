using System.ComponentModel;

namespace DominatorHouseCore.Enums.GdQuery
{
    public enum GdUserQuery
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langSuggestedUsers")]
        SuggestedUsers,
        [Description("langHashtagusers")]
        HashtagUsers,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings,
        [Description("langLocationUsers")]
        LocationUsers,
        [Description("langCustomUser")]
        CustomUsers,
        [Description("langUsersWhoLiked")]
        UsersWhoLikedPost,
        [Description("langUsersWhoCommented")]
        UsersWhoCommentedOnPost,
    }
}