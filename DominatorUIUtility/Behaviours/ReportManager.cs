using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorUIUtility.Behaviours
{
    public class ReportManager
    {
        public static Func<string, string, ObservableCollectionBase<QueryInfo>> GetSavedQuery { get; set; }

        public static Func<Reports, Dictionary<string, string>, DataBaseConnectionCodeFirst.DataBaseConnection, CampaignDetails, ObservableCollection<object>> GetReportDetail { get; set; }
       public static Func<string> GetHeader { get; set; }
        public static Action<string,string> ExportReports { get; set; }
       
    }
}
