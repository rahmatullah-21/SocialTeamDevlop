using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Models
{
    public class ScrapeResultNew
    {
        public IUser ResultUser { get; set; }
        public IPost ResultPost { get; set; }
        public IGroup ResultGroup { get; set; }
        public ActivityType ActivityType { get; set; }
        public QueryInfo QueryInfo { get; set; }
        public bool IsAccountLocked { get; set; }
        public ICommunity ResultCommunity { get; set; }
        public IJob ResultJob { get; set; }
        public ICompany ResultCompany { get; set; }

        public IChannel ResultChannel { get; set; }

        public IPage ResultPage { get; set; }

        public IComments ResultComment { get; set; }
    }
}
