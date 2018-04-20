using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdFriendRequestQuery
    {
        [Description("FdlangKeywords")]
        Keywords = 1,
        [Description("FdlangLocation")]
        Location = 2,
        //[Description("FdlangCustomUsersList")]
        //CustomUserList = 3,
        ////[Description("FdlangSuggestedFriends")]
        ////SuggsetedFriends = 4,
        [Description("FdlangGroupMembers")]
        GroupMembers = 5,
        [Description("FdlangPageMembers")]
        PageMembers = 6,
        [Description("FdlangGraphSearchUrl")]
        GraphSearchUrl = 7,
        [Description("FdlangFriendofFriend")]
        FriendofFriend = 8,
        [Description("FdlangPagePostLikers")]
        PagePostLikers = 9,
        [Description("FdlangGroupPostLikers")]
        GroupPostLikers = 10
    }
}