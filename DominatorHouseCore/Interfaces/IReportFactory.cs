using System;
using System.Collections.Generic;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    public interface IReportFactory
    {        
        /// <summary>
        /// To get the saved query details via moduleName and activity settings
        /// </summary>
        /// <param name="subModuleName">pass the submodule name like follower, send friend request, and so on</param>
        /// <param name="activitySettings">pass the activity settings as the string which was saved already in bin file</param>
        /// <returns>returns all saved query details for the respective submodule and activity settings</returns>
        IEnumerable<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings);

        int GetReportDetail(ReportModel reportModel, List<KeyValuePair<string, string>> queryDetails, CampaignDetails campaignDetails);

        void ExportReports(string subModule, string fileName);

        int GetAccountWiseReportDetail(AccountWiseReportModel accountWiseReportModel, string accountId);

        bool FilterByQueryValue(string queryType, string queryValue, ReportModel reportModel);

        bool FilterByAccount(string accountId, ReportModel reportModel);

        bool FitlerByReportDate(DateTime startDate, DateTime enDate, ReportModel reportModel);

    }
}