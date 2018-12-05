using System;
using DominatorHouseCore.Models;

namespace DominatorUIUtility.Behaviours
{
   
    
    public class ReportManager
    {
        //public static Func<string, string, ObservableCollection<QueryInfo>> GetSavedQuery { get; set; }

        //public static Func<Reports, List<KeyValuePair<string, string>>,DbOperations, CampaignDetails, int> GetReportDetail
        //{ get; set; }
        ////public static Func<CampaignAccountWiseReport, DataBaseConnection, string, int> GetAccountWiseReportDetail{ get; set; }
        //public static Func<CampaignAccountWiseReport, DbOperations, int> GetAccountWiseReportDetail { get; set; }


        //// public static Func<Reports, List<KeyValuePair<string, string>>, CampaignDetails, int>  GetReportDetail { get; set; }

        //// public static Func<CampaignAccountWiseReport, DataBaseConnection, string, int> GetAccountWiseReportDetail{ get; set; }

        public static Func<string> GetHeader { get; set; }
        //public static Action<string, string> ExportReports { get; set; }
        public static Func<string, ReportModel, bool> FilterByQueryType { get; set; }
        //public static Func<string, ReportModel, bool> FilterByAccount { get; set; }
    }

}