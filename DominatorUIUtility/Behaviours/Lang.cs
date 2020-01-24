using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace LegionUIUtility.Behaviours
{
    public class Lang
    {
        static string NumberOf = "LangKeyNumberOf".FromResourceDictionary();
        static string PerDay = "LangKeyPerDay".FromResourceDictionary();
        public static string GetPerJob(ActivityType activityType)
        {
            string PerJob = "LangKeyPerJob".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerJob;
        }

        public static string GetPerHour(ActivityType activityType)
        {
            string PerHour = "LangKeyPerHour".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerHour;
        }
        public static string GetPerDay(ActivityType activityType)
        {
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerDay;
        }
        public static string GetPerWeek(ActivityType activityType)
        {
            string PerWeek = "LangKeyPerWeek".FromResourceDictionary();
            return NumberOf + " " + GetlangActivity(activityType) + " " + PerWeek;

        }
        public static string GetMaxActivityDay(ActivityType activityType)
        {
            string maxDay = "LangKeyMax".FromResourceDictionary();
            return maxDay + " " + GetlangActivity(activityType) + " " + PerDay;
        }
        public static string GetlangActivity(ActivityType activityType)
        {
            return $"LangKey{activityType}".FromResourceDictionary();
        }
    }
}
