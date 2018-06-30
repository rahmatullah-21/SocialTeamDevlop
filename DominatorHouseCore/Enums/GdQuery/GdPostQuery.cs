using System.ComponentModel;

namespace DominatorHouseCore.Enums.GdQuery
{
    public enum GdPostQuery
    {
        [Description("LangKeySuggestedUsersPosts")]
        SuggestedUsersPosts,
        [Description("LangKeyHashtagPostS")]
        HashtagPost,
        [Description("LangKeyHashtagUsersPostS")]
        HashtagUsersPost,
        [Description("LangKeySomeonesFollowersPostS")]
        SomeonesFollowersPost,
        [Description("LangKeySomeonesFollowingsPostS")]
        SomeonesFollowingsPost,
        [Description("LangKeyFollowersOfSomeonesFollowersPostS")]
        FollowersOfFollowersPost,
        [Description("LangKeyFollowersOfSomeonesFollowingsPostS")]
        FollowersOfFollowingsPost,
        [Description("LangKeyLocationPosts")]
        LocationPosts,
        [Description("LangKeyLocationUsersPosts")]
        LocationUsersPost,
        [Description("LangKeyCustomPhotos")]
        CustomPhotos,
        [Description("LangKeyPostsOfUsersWhoLikedPost")]
        PostOfUsersWhoLikedPost,
        [Description("LangKeyPostsOfUsersWhoCommentedOnPost")]
        PostOfUsersWhoCommentedOnPost,
        [Description("LangKeySpecificUsersPosts")]
        SpecificUsersPosts
    }
}