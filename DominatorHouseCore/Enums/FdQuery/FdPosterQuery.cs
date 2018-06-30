using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdPosterQuery
    {
        [Description("LangKeyTimeline")]
        Timeline = 1,
        [Description("LangKeyNewsfeed")]
        NewsFeed = 2,
        [Description("LangKeyFriendTimeline")]
        FriendTimeline = 3,
        [Description("LangKeyGroups")]
        Groups = 4,
        [Description("LangKeyPages")]
        Pages = 5,
        [Description("LangKeyCustomPostsList")]
        CustomPostUrl = 6,
        [Description("LangKeyFacedominatorCampaign")]
        FaceDominatorCampaign = 7
    }
}