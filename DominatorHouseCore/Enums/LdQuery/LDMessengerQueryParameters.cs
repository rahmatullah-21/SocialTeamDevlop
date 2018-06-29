using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDMessengerQueryParameters
    {
        [Description("LangKeyKeyword")]
        Keyword,
        [Description("LangKeyNotification")]
        Notification,
        [Description("LangKeyJoinedGroupUrl")]
        JoinedGroupUrl,
    }
}