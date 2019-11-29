using DominatorHouseCore.Utility;
using System;
using System.ComponentModel;
using System.Linq;

namespace DominatorHouseCore.Enums
{
    public enum QueryType
    {

        //[Description("Facebook, Instagram,Twitter, Pinterest, LinkedIn, Reddit, Quora,Youtube,Tumblr")]
        [Instagram]
        [Description("Follow")]
        Keyword,
        [Description("Follow")]
        SomeOneFollower
    }
    class InstagramAttribute : Attribute { }
  
    static class QueryTypeHelper
    {
        public static bool HasNetworkAndActivity<T>(this Enum value, string activityType)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (member != null && Attribute.IsDefined(member, typeof(T)))
                return value.HaveActivity(activityType);
            return false;
        }
      
        public static bool IsInstagramActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<InstagramAttribute>(activityType);
        }     

        public static bool HaveActivity(this Enum value, string activityType)
        {
            return value.GetDescriptionAttr().Contains(activityType);
        }

    }
}
