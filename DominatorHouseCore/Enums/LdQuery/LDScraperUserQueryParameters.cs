using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDScraperUserQueryParameters
    {
        [Description("LangKeyKeyword")]
        Keyword,
        [Description("LangKeyProfileUrl")]
        ProfileUrl,
        [Description("LangKeySearchUrl")]
        SearchUrl,
        [Description("LangKeyInput")]
        Input,
        [Description("LangKeyOnly1stConnection")]
        Only1stConnection,
        [Description("LangKeyJoinedGroupUrl")]
        JoinedGroupUrl,
    }
}