using System.ComponentModel;

namespace DominatorHouseCore.Enums.GpDQuery
{
    
    public enum GplusPostsQueries
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomUsersList")]
        FromUser = 2,
        [Description("LangKeyCustomPostsList")]
        CustomPost = 3,
        [Description("LangKeyCustomCommunity")]
        FromCommunity = 4
    }
}
