using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDScraperUserQueryParameters
    {
        [Description("LDlangKeyword")]
        Keyword,
        [Description("LDlangProfileUrl")]
        ProfileUrl,
        [Description("LDlangSearchUrl")]
        SearchUrl,
        [Description("LDlangInput")]
        Input,
        [Description("LDlangOnly1stConnection")]
        Only1stConnection,
        [Description("LDlangJoinedGroupUrl")]
        JoinedGroupUrl,
    }
}