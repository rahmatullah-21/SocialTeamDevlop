using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdProfileScraperQuery
    {
        [Description("LangKeyEvents")]
        Events = 1,
        [Description("LangKeyFanpageLikers")]
        FanpageLikers = 2,
        [Description("LangKeyFriendOfFriend")]
        FriendofFriend = 3,
        [Description("LangKeyGroupMembers")]
        GroupMembers = 4,
        [Description("LangKeyPostLikers")]
        PostLikers = 5,
        [Description("LangKeyPostSharer")]
        PostSharer = 6,
        [Description("LangKeyPostCommentors")]
        PostCommentor = 7,
        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 8,
        [Description("LangKeyCustomUsersList")]
        CustomUserList = 9
    }
}