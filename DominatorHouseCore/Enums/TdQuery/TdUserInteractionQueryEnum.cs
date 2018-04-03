using System.ComponentModel;

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdUserInteractionQueryEnum
    {
        [Description("langKeywords")]
        Keywords = 1,
        [Description("langHashtags")]
        Hashtags = 2,
        [Description("langLocationUsers")]
        LocationUsers = 3,
        [Description("langCustomUsersList")]
        CustomUsersList = 4,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers = 5,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings = 6,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 7,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 8,
        [Description("langUsersWhoLikedTweet")]
        UsersWhoLikedOnTweet = 9,
        [Description("langUsersWhoCommentedOnTweet")]
        UsersWhoCommentedOnTweet = 10,
        [Description("langUsersWhoRetweetedTweet")]
        UsersWhoRetweetedTweet = 11,
    }
}