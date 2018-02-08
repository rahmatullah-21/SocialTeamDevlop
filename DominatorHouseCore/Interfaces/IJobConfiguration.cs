using System.Collections.Generic;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public interface IJobConfiguration
    {
        RangeUtilities DelayBetweenActivity { get; set; }

        RangeUtilities DelayBetweenJobs { get; set; }

        RangeUtilities ActivitiesPerJob { get; set; }

        RangeUtilities ActivitiesPerHour { get; set; }

        RangeUtilities ActivitiesPerDay { get; set; }

        RangeUtilities ActivitiesPerWeek { get; set; }

        IncreaseActivityRange IncreaseActivitiesEachDay { get; set; }

        List<RunningTimes> RunningTime { get; set; }

        string ActivitiesPerJobDisplayName { get; set; }

        string ActivitiesPerHourDisplayName { get; set; }

        string ActivitiesPerDayDisplayName { get; set; }

        string ActivitiesPerWeekDisplayName { get; set; }

        string IncreaseActivityDisplayName { get; set; }

    }
}