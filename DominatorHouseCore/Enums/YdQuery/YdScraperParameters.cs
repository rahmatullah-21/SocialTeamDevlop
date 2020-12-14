#region

using System.ComponentModel;

#endregion

namespace DominatorHouseCore.Enums.YdQuery
{
    public enum YdScraperParameters
    {
        [Description("LangKeyKeywords")] Keywords = 1,
        [Description("LangKeyCustomURLS")] CustomUrls = 2,
        [Description("LangKeyCustomChannel")] CustomChannel = 3,

        [Description("LangKeyYTVideoCommenters")]
        YTVideoCommenters = 4
    }

    public enum YdPostScraperParameters
    {
        [Description("LangKeyKeywords")]
        Keywords = 1
    }

    public enum YdChannelScraperParameters
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyYTVideoCommenters")]
        YTVideoCommenters = 4
    }
}