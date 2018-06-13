using System.ComponentModel;

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrQuery
    {
        [Description("TumlangKeyword")]
        Keyword,
        [Description("TumlangUserFollowing")]
        UserFollowing,
        [Description("TumlangUserFollower")]
        UserFollower,
        [Description("TumlangHashTag")]
        HashtagUsers,
        [Description("TumlangUserCommentedOnPost")]
        UserCommentedOnPost,
        [Description("TumlangUserLikedThePost")]
        UserLikedThePost,
        [Description("TumlangUserReblogedThePost")]
        UserReblogedThePost,
        [Description("TumlangUserLikedCommentedReblogedThePost")]
        UserLikedCommentedReblogedThePost
    }
    public enum TumblrPostQuery
    {
        [Description("TumlangKeyword")]
        Keyword,
        [Description("TumlangHashTag")]
        HashtagUsers,
        //TODO
        //[Description("TumlangNewsFeed")]
        //Dashboard,
        //[Description("TumlangUsername")]
        //Username,



    }
}
