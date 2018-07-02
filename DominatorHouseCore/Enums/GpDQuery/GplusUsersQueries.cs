using System.ComponentModel;

namespace DominatorHouseCore.Enums.GpDQuery
{
    public enum GplusUsersQueries
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomUsersList")]
        CustomUsers = 2,
        [Description("LangKeyUsersWhoLikedPosts")]
        UsersWhoLikedPost = 3,
        [Description("LangKeyUsersWhoCommentedOnPosts")]
        UsersWhoCommentedOnPost = 4,
        [Description("LangKeyFromSomeonesCircle")]
        FromSomeonesCircle = 5,
        [Description("LangKeyFromCircleOfFollowers")]
        FromCircleOfFollowers = 6,
        [Description("LangKeyFromCircleOfFollowings")]
        FromCircleOfFollowings = 7
    }
}
