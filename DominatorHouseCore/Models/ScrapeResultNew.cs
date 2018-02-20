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
        public IUser InstagramUser { get; set; }
        public IPost InstagramPost { get; set; }
        public ActivityType ActivityType { get; set; }
        public QueryInfo QueryInfo { get; set; }
        public bool IsAccountLocked { get; set; }
    }
}
