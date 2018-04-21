using System.ComponentModel;

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdUserInteractionQueryEnum
    {
        [Description("TdLangKeywords")]
        Keywords = 1,
        [Description("TdLangHashtags")]
        Hashtags = 2,
        [Description("TdLangLocationUsers")]
        LocationUsers = 3,
        [Description("TdLangNearMyLocation")]
        NearMyLocation = 4,
        [Description("TdLangCustomUsersList")]
        CustomUsersList = 5,
        [Description("TdLangSomeonesFollowers")]
        SomeonesFollowers = 6,
        [Description("TdLangSomeonesFollowings")]
        SomeonesFollowings = 7,
        [Description("TdLangFollowersOfSomeonesFollowings")]
        FollowersOfFollowings = 8,
        [Description("TdLangFollowersOfSomeonesFollowers")]
        FollowersOfFollowers = 9,
        [Description("TdLangUsersWhoLikedTweet")]
        UsersWhoLikedOnTweet = 10,
        [Description("TdLangUsersWhoCommentedOnTweet")]
        UsersWhoCommentedOnTweet = 11,
        [Description("TdLangUsersWhoRetweetedTweet")]
        UsersWhoRetweetedTweet = 12,
    }
}