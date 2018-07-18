using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.ViewModel
{
    public class TwitterDailyStatisticsViewModel : BindableBase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Followers { get; set; }
        public int Followings { get; set; }
        public int Tweets { get; set; }
        public int Likes { get; set; }

    }

   
}
