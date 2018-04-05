using System.ComponentModel;

namespace DominatorHouseCore.Enums.GpDQuery
{
    
    public enum GplusPostsQueries
    {
        [Description("langKeywords")]
        Keywords = 1,
        [Description("langFromUser")]
        FromUser = 2,
        [Description("langCustomPost")]
        CustomPost = 3,
        [Description("langFromCommunity")]
        FromCommunity = 4
    }
}
