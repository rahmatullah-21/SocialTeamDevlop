using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDMessengerQueryParameters
    {
        [Description("LDlangKeyword")]
        Keyword,
        [Description("LDNotification")]
        Notification,
        [Description("LDlangJoinedGroupUrl")]
        JoinedGroupUrl,
    }
}