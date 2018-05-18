using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Interfaces
{
    interface IReportFactory
    {
        string GetHeaders();

        ObservableCollection<QueryInfo> GetSavedQuery(string subModuleName, string activitySettings);

      
        void ExportReports(string subModule, string fileName);
    }
}