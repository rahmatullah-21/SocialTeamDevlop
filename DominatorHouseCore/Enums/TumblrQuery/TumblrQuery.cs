using System.ComponentModel;

namespace DominatorHouseCore.Enums.TumblrQuery
{
    public enum TumblrQuery
    {
        [Description("LangKeyKeywords")]
        Keyword,
        [Description("LangKeySomeonesFollowings")]
        UserFollowing,
        [Description("LangKeyHashtagUserS")]
        HashtagUsers,
        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UserCommentedOnPost,
        [Description("LangKeyUsersWhoLikedPosts")]
        UserLikedThePost,
        [Description("LangKeyReblogPost")]
        UserReblogedThePost,
        [Description("LangKeyReblogLikerCommenter")]
        UserLikedCommentedReblogedThePost,
        [Description("LangKeyPostOwner")]
        PostOwner
    }
    public enum TumblrPostQuery
    {
        [Description("LangKeyKeywords")]
        Keyword,
        [Description("LangKeyHashtagUserS")]
        HashtagUsers,
        //TODO
        //[Description("TumlangNewsFeed")]
        //Dashboard,
        //[Description("TumlangUsername")]
        //Username,



    }
}
