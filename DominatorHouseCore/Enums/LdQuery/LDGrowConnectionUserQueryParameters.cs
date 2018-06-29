using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDGrowConnectionUserQueryParameters
    {
        [Description("LangKeyKeyword")]
        Keyword,
        [Description("LangKeyEmail")]
        Email,
        [Description("LangKeyProfileUrl")]
        ProfileUrl,
        [Description("LangKeySearchUrl")]
        SearchUrl,
    }
}