using System.ComponentModel;

namespace DominatorHouseCore.Enums.TdQuery
{
    public enum TdTweetInteractionQueryEnum
    {
        [Description("langKeywords")]
        Keywords = 1,
        [Description("langHashtags")]
        Hashtags = 2,
        [Description("langLocationTweets")]
        LocationTweets = 3,
        [Description("langCustomTweetsList")]
        CustomTweetsList = 4,
        [Description("langSomeonesFollowersTweets")]
        SomeonesFollowers = 5,
        [Description("langSomeonesFollowingsTweets")]
        SomeonesFollowings = 6,
        [Description("langFollowersOfSomeonesFollowingsTweets")]
        FollowersOfFollowings = 7,
        [Description("langFollowersOfSomeonesFollowersTweets")]
        FollowersOfFollowers = 8,
        [Description("langLikedUsersTweets")]
        UsersWhoLikedOnTweet = 9,
        [Description("langCommentedUsersTweets")]
        UsersWhoCommentedOnTweet = 10,
        [Description("langRetweetedUsersTweets")]
        UsersWhoRetweetedTweet = 11,
        [Description("langCommentedTweets")]
        CommentedTweet = 12,
    }
}