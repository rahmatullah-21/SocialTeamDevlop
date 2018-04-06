using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDGrowConnectionUserQueryParameters
    {
        [Description("LDlangKeyword")]
        Keyword,
        [Description("LDlangEmail")]
        Email,
        [Description("LDlangProfileUrl")]
        ProfileUrl,
        [Description("LDlangSearchUrl")]
        SearchUrl,
    }
}