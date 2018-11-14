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
        public int GrowthColumnValue5 { get; set; }


        public int this[int i]
        {
            get
            {
                switch (i)
                {
                    case 1:
                        return GrowthColumnValue1;
                    case 2:
                        return GrowthColumnValue2;
                    case 3:
                        return GrowthColumnValue3;
                    case 4:
                        return GrowthColumnValue4;
                    case 5:
                        return GrowthColumnValue5;
                    default:
                        throw new ArgumentOutOfRangeException("GrowthColumnValue X");
                }
            }
        }

    }


}
