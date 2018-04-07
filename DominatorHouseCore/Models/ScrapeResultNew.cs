using DominatorHouseCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}
