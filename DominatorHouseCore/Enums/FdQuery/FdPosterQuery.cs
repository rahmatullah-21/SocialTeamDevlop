using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum FdPosterQuery
    {
        [Description("FdlangTimeline")]
        Timeline = 1,
        [Description("FdlangNewsFeed")]
        NewsFeed = 2,
        [Description("FdlangFriendTimeline")]
        FriendTimeline = 3,
        [Description("FdlangGroups")]
        Groups = 4,
        [Description("FdlangPages")]
        Pages = 5,
        [Description("FdlangCustomPostList")]
        CustomPostUrl = 6,
        [Description("FdlangFdCampaign")]
        FaceDominatorCampaign = 7
    }
}