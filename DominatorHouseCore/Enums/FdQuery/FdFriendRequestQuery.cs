using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdFriendRequestQuery
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyLocation")]
        Location = 2,
        //[Description("FdlangCustomUsersList")]
        //CustomUserList = 3,
        ////[Description("FdlangSuggestedFriends")]
        ////SuggsetedFriends = 4,
        [Description("LangKeyGroupMembers")]
        GroupMembers = 5,
        [Description("LangKeyPageMembers")]
        PageMembers = 6,
        [Description("LangKeyGraphSearchUrl")]
        GraphSearchUrl = 7,
        [Description("LangKeyFriendOfFriend")]
        FriendofFriend = 8,
        [Description("LangKeyFanpagePostLikers")]
        PagePostLikers = 9,
        [Description("LangKeyGroupPostLikers")]
        GroupPostLikers = 10
    }
}