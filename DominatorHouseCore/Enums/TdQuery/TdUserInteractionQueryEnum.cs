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
        [Description("langNearMyLocation")]
        NearMyLocation = 4,
        [Description("langCustomUsersList")]
        CustomUsersList = 5,
        [Description("langSomeonesFollowers")]
        SomeonesFollowers = 6,
        [Description("langSomeonesFollowings")]
        SomeonesFollowings = 7,
        [Description("langFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 8,
        [Description("langFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 9,
        [Description("langUsersWhoLikedTweet")]
        UsersWhoLikedOnTweet = 10,
        [Description("langUsersWhoCommentedOnTweet")]
        UsersWhoCommentedOnTweet = 11,
        [Description("langUsersWhoRetweetedTweet")]
        UsersWhoRetweetedTweet = 12,
    }
}