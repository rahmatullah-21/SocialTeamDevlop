using System.ComponentModel;

namespace DominatorHouseCore.Enums.GdQuery
{
    public enum GdPostQuery
    {
        [Description("GDlangSuggestedUsersPosts")]
        SuggestedUsersPosts,
        [Description("langHashTagPost")]
        HashtagPost,
        [Description("GDlangHashTagUsersPost")]
        HashtagUsersPost,
        [Description("GDlangSomeonesFollowersPost")]
        SomeonesFollowersPost,
        [Description("GDlangSomeonesFollowingsPost")]
        SomeonesFollowingsPost,
        [Description("GDlangFollowersOfSomeonesFollowersPosts")]
        FollowersOfFollowersPost,
        [Description("GDlangFollowersOfSomeonesFollowingsPosts")]
        FollowersOfFollowingsPost,
        [Description("langLocationPosts")]
        LocationPosts,
        [Description("GDlangLocationUsersPosts")]
        LocationUsersPost,
        [Description("langCustomPhotos")]
        CustomPhotos,
        [Description("GDlangPostsOfUsersWhoLikedPost")]
        PostOfUsersWhoLikedPost,
        [Description("GDlangPostsOfUsersWhoCommentedOnPost")]
        PostOfUsersWhoCommentedOnPost
    }
}