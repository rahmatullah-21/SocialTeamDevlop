using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface IJobConfiguration
    {
        RangeUtilities ActivitiesDelay { get; set; }

        RangeUtilities JobsDelay { get; set; }

        RangeUtilities JobsActivityCount { get; set; }

        RangeUtilities HoursActivityCount { get; set; }

        RangeUtilities DaysActivityCount { get; set; }

        RangeUtilities WeeksActivityCount { get; set; }

        IncreaseActivityRange IncreaseActivity { get; set; }

        List<RunningTimes> RunningTime { get; set; }

        string JobsActivityDisplayName { get; set; }

        string HoursActivityDisplayName { get; set; }

        string DaysActivityDisplayName { get; set; }

        string WeeksActivityDisplayName { get; set; }

        string IncreaseActivityDisplayName { get; set; }

    }
}