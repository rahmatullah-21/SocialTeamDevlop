using System.ComponentModel;

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdTweetInteractionQueryEnum
    {
        [Description("TdLangKeywords")]
        Keywords = 1,
        [Description("TdLangHashtags")]
        Hashtags = 2,
        [Description("TdLangLocationTweets")]
        LocationTweets = 3,
        [Description("TdLangNearMyLocation")]
        NearMyLocation = 4,
        [Description("TdLangCustomTweetsList")]
        CustomTweetsList = 5,
        [Description("TdLangSomeonesFollowersTweets")]
        SomeonesFollowers = 6,
        [Description("TdLangSomeonesFollowingsTweets")]
        SomeonesFollowings = 7,
        [Description("TdLangFollowersOfSomeonesFollowingsTweets")]
        FollowersOfFollowings = 8,
        [Description("TdLangFollowersOfSomeonesFollowersTweets")]
        FollowersOfFollowers = 9,
        [Description("TdLangLikedUsersTweets")]
        UsersWhoLikedOnTweet = 10,
        [Description("TdLangCommentedUsersTweets")]
        UsersWhoCommentedOnTweet = 11,
        [Description("TdLangRetweetedUsersTweets")]
        UsersWhoRetweetedTweet = 12,
        [Description("TdLangCommentedTweets")]
        CommentedTweet = 13,
    }
}