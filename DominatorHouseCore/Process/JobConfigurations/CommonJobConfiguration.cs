using DominatorHouseCore.Models;
using System.Collections.Generic;

namespace DominatorHouseCore.Process.JobConfigurations
{

    public class CommonJobConfiguration
    {
        public JobConfiguration JobConfiguration { get; }
        public List<QueryInfo> SavedQueries { get; }
        public bool IsNeedToSchedule { get; }

        public CommonJobConfiguration(JobConfiguration jobConfiguration, List<QueryInfo> savedQueries, bool isNeedToSchedule)
        {
            JobConfiguration = jobConfiguration;
            SavedQueries = savedQueries;
            IsNeedToSchedule = isNeedToSchedule;
        }
    }
}
