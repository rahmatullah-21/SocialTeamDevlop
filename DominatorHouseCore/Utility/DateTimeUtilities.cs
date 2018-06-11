using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.Utility
{
    public static class DateTimeUtilities
    {

        /// <summary>
        /// GetCurrentEpochTime is used to get the epoch value for given date time
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static int GetCurrentEpochTime(this DateTime date)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32(Math.Floor((date.ToUniversalTime() - dateTime).TotalSeconds));
        }

        public static int ConvertToEpoch(this DateTime date)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt32(Math.Floor((date.ToUniversalTime() - dateTime).TotalSeconds));
        }

        /// <summary>
        /// GetDayOfWeek is used to return the current day
        /// </summary>
        /// <returns></returns>
        public static DayOfWeek GetDayOfWeek()
        {
            return DateTime.Now.DayOfWeek;
        }

        public static TimeSpan GetTimeSpanCurrentHourMinute()
        {
            DateTime now = DateTime.Now;
            return new TimeSpan(now.Hour, now.Minute, 0);
        }

        public static TimeSpan GetTimeSpanForGivenTime(DateTime dateTime)
        {
            return new TimeSpan(dateTime.Hour, dateTime.Minute, 0);
        }


        public static DateTime EpochToDateTimeUtc(this int epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)epoch);
        }

        public static DateTime EpochToDateTimeUtc(this Int64 epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds((double)epoch);
        }
        public static DateTime EpochToDateTimeUtc(this double epoch)
        {
            return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(
                (double)epoch);
        }

        public static TimeSpan EpochToTimeSpan(this int epoch)
        {
            return new TimeSpan(0, 0, epoch);
        }

        public static int GetEpochTime()
        {
            return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }




        public static double GetEpochTimeMicroSecs()
        {
            return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        }

        public static int GetTimezoneOffset()
        {
            return (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalSeconds;
        }

        public static string ReadableDateTime(this DateTime time)
        {
            return time.ToString("MM/dd/yyyy HH:mm:ss", (IFormatProvider)CultureInfo.InvariantCulture);
        }

        public static DateTime StartOfWeekOld(this DateTime date, DayOfWeek day)
        {
            int num = date.DayOfWeek - day;
            if (num < 0)
                num += 7;
            return date.AddDays((double)(-1 * num)).Date;
        }

        public static DateTime GetStartOfWeek(this DateTime date)
        {
            int num = date.DayOfWeek - DayOfWeek.Sunday;
            if (num < 0)
                num += 7;
            return date.AddDays((double)(-1 * num)).Date;
        }

        public static DateTime GetNextStartTime(this DateTime date, ModuleConfiguration moduleConfiguration, int dayCount)
        {
            int num = date.DayOfWeek - DayOfWeek.Sunday;
            if (num < 0)
                num += 7;
            var nextWeekStartDate = (date.AddDays((double)(-1 * num)).Date).AddDays(dayCount);
            if (dayCount == 1)
            {
                var NextWeekStartDate = date.AddDays(1).Date;
            }

            foreach (var runningTime in moduleConfiguration.LstRunningTimes)
            {
                if (!runningTime.IsEnabled)
                    continue;

                if (runningTime.Timings.Count <= 0)
                    continue;

                var startTime = nextWeekStartDate.GetDateOfDateTime(runningTime.DayOfWeek);

                var timings = runningTime.Timings.ToList();

                timings.Sort(new RunningTimeComparer());

                if (dayCount == 0)
                {
                    var currentDateTime = DateTime.Now.AddSeconds(30);
                    var availableTimingRanges = timings.Where(x => DateTime.Today.Date.Add(x.StartTime) > currentDateTime).ToList();
                    availableTimingRanges.Sort(new RunningTimeComparer());
                    nextWeekStartDate = startTime.Add(availableTimingRanges[0].StartTime);
                }
                else
                {
                    nextWeekStartDate = startTime.Add(timings[0].StartTime);
                }

                return nextWeekStartDate;
            }

            return nextWeekStartDate;
        }

        public static DateTime GetStartTimeOfNextWeek(ModuleConfiguration moduleConfiguration)
        {
            int num = DateTime.Today.DayOfWeek - DayOfWeek.Sunday;
            if (num < 0)
                num += 7;
            var nextWeekStartDate = (DateTime.Today.AddDays((double)(-1 * num)).Date).AddDays(7);
            foreach (var runningTime in moduleConfiguration.LstRunningTimes)
            {
                if (!runningTime.IsEnabled)
                    continue;

                if (runningTime.Timings.Count <= 0)
                    continue;

                var startTime = nextWeekStartDate.GetDateOfDateTime(runningTime.DayOfWeek);
                var timings = runningTime.Timings.ToList();
                timings.Sort(new RunningTimeComparer());
                nextWeekStartDate = startTime.Add(timings[0].StartTime);
                return nextWeekStartDate;
            }
            return nextWeekStartDate;
        }

        public static DateTime GetStartTimeOfTomorrow(ModuleConfiguration moduleConfiguration)
        {
            var startTimeOfTomorrow = DateTime.Today.AddDays(1);
            //Get date for tomorrow with default time
            int num = DayOfWeek.Sunday - DateTime.Today.DayOfWeek;

            for (int i = 1; i < 8; i++)
            {
                var Day = DateTime.Today.AddDays(i).DayOfWeek;
                var runningTimes = moduleConfiguration.LstRunningTimes[(int)Day];
                if (!runningTimes.IsEnabled)
                    continue;

                if (runningTimes.Timings.Count <= 0)
                    continue;
                //var startTime = startTimeOfTomorrow.GetDateOfDateTime(runningTimes.DayOfWeek);
                var timings = runningTimes.Timings.ToList();
                timings.Sort(new RunningTimeComparer());
                startTimeOfTomorrow = DateTime.Today.AddDays(i);
                startTimeOfTomorrow = startTimeOfTomorrow.Add(timings[0].StartTime);
                return startTimeOfTomorrow;
            }
            return startTimeOfTomorrow;
        }

        public static DateTime GetStartTimeOfNextJob(ModuleConfiguration moduleConfiguration, int? delayBetweenJob = null)
        {
            try
            {
                var delay = 0;
                if (delayBetweenJob != null)
                    delay = (int)delayBetweenJob;

                var startTimeOfNextJob = DateTime.Now.AddMinutes(delay);
                var today = DateTime.Today.DayOfWeek;
                var todayIndex = (int)today;
                var runningTimes = moduleConfiguration.LstRunningTimes[todayIndex];
                var timings = runningTimes.Timings.ToList();
                timings.Sort(new RunningTimeComparer());
                var availableTimingRanges = timings.Where(x => DateTime.Today.Date.Add(x.EndTime) > startTimeOfNextJob || DateTime.Today.Date.Add(x.StartTime) > startTimeOfNextJob).ToList();
                if (availableTimingRanges.Count > 0)
                {
                    availableTimingRanges.Sort(new RunningTimeComparer());
                    var calculatedStartTime = DateTime.Today.Add(availableTimingRanges[0].StartTime);
                    if (calculatedStartTime > startTimeOfNextJob) startTimeOfNextJob = calculatedStartTime;
                    //if (moduleConfiguration.NextRun > startTimeOfNextJob) startTimeOfNextJob = moduleConfiguration.NextRun;
                }
                else
                {
                    return GetStartTimeOfTomorrow(moduleConfiguration);
                }
                return startTimeOfNextJob;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
                return DateTime.MinValue;
            }
        }

        public static DateTime GetStartTimeForHourly(ModuleConfiguration moduleConfiguration, int? delayBetweenJob = null)
        {
           return GetStartTimeOfNextJob(moduleConfiguration,15);
        }

        public static DateTime GetDateOfDateTime(this DateTime startDate, DayOfWeek requiredDay)
        {
            for (int countIndex = 0; countIndex < 7; countIndex++)
            {
                var currentDate = startDate.AddDays(countIndex);
                if (currentDate.DayOfWeek == requiredDay)
                    return currentDate;
            }
            return startDate;
        }


        public static DateTime GetStartOfWeek(this DateTime date, DayOfWeek day)
        {
            int num = date.DayOfWeek - day;
            if (num < 0)
                num += 7;
            return date.AddDays((double)(-1 * num)).Date;
        }
        public static Int64 GetCurrentEpochTimeMilliSeconds(this DateTime date)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64(Math.Floor((date.ToUniversalTime() - dateTime).TotalMilliseconds));
        }

        public static DateTime GetNextStartTime(ModuleConfiguration moduleConfiguration,
            ReachedLimitType reachedLimitType, int? delayBetweenJob = null)
        {
            var nextStartTime = DateTime.Now;
            switch (reachedLimitType)
            {
                case ReachedLimitType.Weekly:
                    return GetStartTimeOfNextWeek(moduleConfiguration);
                case ReachedLimitType.Daily:
                    return GetStartTimeOfTomorrow(moduleConfiguration);
                case ReachedLimitType.Hourly:
                    return GetStartTimeForHourly(moduleConfiguration);
                case ReachedLimitType.Job:
                    return GetStartTimeOfNextJob(moduleConfiguration, delayBetweenJob);

            }
            
            return nextStartTime;
        }
    }


    public enum ReachedLimitType
    {
        Daily,
        Weekly,
        Job,
        Hourly,
        NoLimit
    }
}
