using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDGrowConnectionUserQueryParameters
    {
        [Description("langKeywords")]
        Keywords,
        [Description("langEmails")]
        Emails,
        [Description("langProfileUrls")]
        ProfileUrls,
        [Description("langSearchUrl")]
        SearchUrl,
    }
}