using DominatorHouseCore.Utility;
using System;
using System.ComponentModel;
using System.Linq;

namespace DominatorHouseCore.Enums
{
    public enum QueryType
    {

        //[Description("Facebook, Instagram,Twitter, Pinterest, LinkedIn, Reddit, Quora,Youtube,Tumblr")]
        [Facebook]
        [Instagram]
        [Twitter]
        [Pinterest]
        [LinkedIn]
        [Reddit]
        [Youtube]
        [Tumblr]
        [Quora]
        [Description("Follow")]
        Keyword,

        [Quora]
        [Description("Follow")]
        SomeOneFollower
    }
    class FacebookAttribute : Attribute { }
    class InstagramAttribute : Attribute { }
    class TwitterAttribute : Attribute { }
    class QuoraAttribute : Attribute { }
    class PinterestAttribute : Attribute { }
    class LinkedInAttribute : Attribute { }
    class RedditAttribute : Attribute { }
    class YoutubeAttribute : Attribute { }
    class TumblrAttribute : Attribute { }

    static class QueryTypeHelper
    {
        public static bool HasNetworkAndActivity<T>(this Enum value, string activityType)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            if (member != null && Attribute.IsDefined(member, typeof(T)))
                return value.HaveActivity(activityType);
            return false;
        }
        public static bool IsFacebookActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<FacebookAttribute>(activityType);
        }
        public static bool IsInstagramActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<InstagramAttribute>(activityType);
        }
        public static bool IsTwitterActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<TwitterAttribute>(activityType);
        }
        public static bool IsPinterestActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<PinterestAttribute>(activityType);
        }
        public static bool IsLinkedInActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<LinkedInAttribute>(activityType);
        }
        public static bool IsRedditActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<RedditAttribute>(activityType);
        }
        public static bool IsYoutubeActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<YoutubeAttribute>(activityType);
        }
        public static bool IsTumblrActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<TumblrAttribute>(activityType);
        }
        public static bool IsQuoraActivity(this Enum value, string activityType)
        {
            return value.HasNetworkAndActivity<QuoraAttribute>(activityType);
        }

        public static bool HaveActivity(this Enum value, string activityType)
        {
            return value.GetDescriptionAttr().Contains(activityType);
        }

    }
}
