using System.ComponentModel;

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdTweetInteractionQueryEnum
    {
        [Description("TdLangOwnFollwers")]
        OwnFollowers = 1,
        [Description("TdLangOwnFollowings")]
        OwnFollowings = 2,
        [Description("TdLangKeywords")]
        Keywords = 3,
        [Description("TdLangHashtags")]
        Hashtags = 4,
        [Description("TdLangLocationTweets")]
        LocationTweets = 5,
        [Description("TdLangNearMyLocation")]
        NearMyLocation = 6,
        [Description("TdLangCustomTweetsList")]
        CustomTweetsList = 7,
        [Description("TdLangSomeonesFollowersTweets")]
        SomeonesFollowers = 8,
        [Description("TdLangSomeonesFollowingsTweets")]
        SomeonesFollowings = 9,
        [Description("TdLangFollowersOfSomeonesFollowingsTweets")]
        FollowersOfFollowings = 10,
        [Description("TdLangFollowersOfSomeonesFollowersTweets")]
        FollowersOfFollowers = 11,
        [Description("TdLangLikedUsersTweets")]
        UsersWhoLikedOnTweet = 12,
        [Description("TdLangCommentedUsersTweets")]
        UsersWhoCommentedOnTweet = 13,
        [Description("TdLangRetweetedUsersTweets")]
        UsersWhoRetweetedTweet = 14,
        [Description("TdLangCommentedTweets")]
        CommentedTweet = 15,
    }
}