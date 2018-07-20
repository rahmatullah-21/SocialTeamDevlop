using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.ViewModel
{
    public class DailyStatisticsViewModel : BindableBase
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int GrowthColumnValue1 { get; set; }
        public int GrowthColumnValue2 { get; set; }
        public int GrowthColumnValue3 { get; set; }
        public int GrowthColumnValue4 { get; set; }

    }

   
}
