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
        [Description("langNearMyLocation")]
        NearMyLocation = 4,
        [Description("langCustomTweetsList")]
        CustomTweetsList = 5,
        [Description("langSomeonesFollowersTweets")]
        SomeonesFollowers = 6,
        [Description("langSomeonesFollowingsTweets")]
        SomeonesFollowings = 7,
        [Description("langFollowersOfSomeonesFollowingsTweets")]
        FollowersOfFollowings = 8,
        [Description("langFollowersOfSomeonesFollowersTweets")]
        FollowersOfFollowers = 9,
        [Description("langLikedUsersTweets")]
        UsersWhoLikedOnTweet = 10,
        [Description("langCommentedUsersTweets")]
        UsersWhoCommentedOnTweet = 11,
        [Description("langRetweetedUsersTweets")]
        UsersWhoRetweetedTweet = 12,
        [Description("langCommentedTweets")]
        CommentedTweet = 13,
    }
}