using System;
using System.Globalization;

namespace DominatorHouseCore.Utility
{
    [Obsolete("This class will no longer be present, instead use DateTimeUtilities class",true)]
    public static class DateTimeHelper
    {

        //    static DateTimeHelper()
        //    {

        //    }

        //    public static int ConvertToEpoch(this DateTime date)
        //    {
        //        DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        //        return Convert.ToInt32(Math.Floor((date.ToUniversalTime() - dateTime).TotalSeconds));
        //    }

        //    public static DayOfWeek CurrentDay()
        //    {
        //        return DateTime.Now.DayOfWeek;
        //    }

        //    public static TimeSpan CurrentHourMinute()
        //    {
        //        DateTime now = DateTime.Now;
        //        return new TimeSpan(now.Hour, now.Minute, 0);
        //    }

        //    public static DateTime EpochToDateTimeUtc(this int epoch)
        //    {
        //        return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds((double)epoch);
        //    }

        //    public static TimeSpan EpochToTimeSpan(this int epoch)
        //    {
        //        return new TimeSpan(0, 0, epoch);
        //    }

        //    public static int GetEpochTime()
        //    {
        //        return (int)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        //    }

        //    public static double GetEpochTimeMicroSecs()
        //    {
        //        return (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds;
        //    }

        //    public static int GetTimezoneOffset()
        //    {
        //        return (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalSeconds;
        //    }

        //    public static string ReadableDateTime(this DateTime time)
        //    {
        //        return time.ToString("MM/dd/yyyy HH:mm:ss", (IFormatProvider)CultureInfo.InvariantCulture);
        //    }

        //    public static DateTime StartOfWeek(this DateTime date, DayOfWeek day)
        //    {
        //        int num = date.DayOfWeek - day;
        //        if (num < 0)
        //            num += 7;
        //        return date.AddDays((double)(-1 * num)).Date;
        //    }
        //}

    }


}
