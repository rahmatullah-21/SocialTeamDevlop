using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdProfileScraperQuery
    {
        [Description("FdlangEvents")]
        Events = 1,
        [Description("FdlangFanpageLikers")]
        FanpageLikers = 2,
        [Description("FdlangFriendofFriend")]
        FriendofFriend = 3,
        [Description("FdlangGroupMembers")]
        GroupMembers = 4,
        [Description("FdlangPostLikers")]
        PostLikers = 5,
        [Description("FdlangPostSharer")]
        PostSharer = 6,
        [Description("FdlangPostCommentor")]
        PostCommentor = 7,
        [Description("FdlangGraphSearchUrl")]
        GraphSearchUrl = 8,
        [Description("FdlangCustomUsersList")]
        CustomUserList = 9
    }
}