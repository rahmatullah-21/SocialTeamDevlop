using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;

namespace DominatorUIUtility.Behaviours
{
    public class ReportManager
    {
        public static Func<string, string, ObservableCollectionBase<QueryInfo>> GetSavedQuery { get; set; }

        public static
            Func<Reports, Dictionary<string, string>, DominatorHouseCore.DatabaseHandler.CoreModels.DataBaseConnection, CampaignDetails, int>
            GetReportDetail { get; set; }

        public static Func<string> GetHeader { get; set; }
        public static Action<string, string> ExportReports { get; set; }
        public static Func<string, ReportModel, bool> FilterByQueryType { get; set; }
        public static Func<string, ReportModel, bool> FilterByAccount { get; set; }
    }
}