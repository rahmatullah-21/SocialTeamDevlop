using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDScraperUserQueryParameters
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langProfileUrls")]
        ProfileUrls,
        [Description("langSearchUrl")]
        SearchUrl,
        [Description("langInputs")]
        Inputs,
        [Description("langOnly1stConnection")]
        Only1stConnection,
        [Description("langJoinedGroupUrl")]
        JoinedGroupUrl,
    }
}