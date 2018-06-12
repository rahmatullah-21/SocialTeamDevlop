using System.ComponentModel;

namespace DominatorHouseCore.Enums.GpDQuery
{
    public enum GplusUsersQueries
    {
        [Description("langKeywords")]
        Keywords = 1,
        [Description("langCustomUser")]
        CustomUsers = 2,
        [Description("langUsersWhoLiked")]
        UsersWhoLikedPost = 3,
        [Description("langUsersWhoCommented")]
        UsersWhoCommentedOnPost = 4,
        [Description("langFromSomeonesCircle")]
        FromSomeonesCircle = 5,
        [Description("langFromCircleOfFollowers")]
        FromCircleOfFollowers = 6,
        [Description("langFromCircleOfFollowings")]
        FromCircleOfFollowings = 7
    }
}
