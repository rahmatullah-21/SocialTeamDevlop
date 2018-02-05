using System;
using System.Globalization;

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

    }
}
