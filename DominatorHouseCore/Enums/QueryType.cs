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
        [Description("ModuleName")]
        Keyword,

        [Quora]
        [Description("ModuleName")]
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
        public static bool HasAttribute<T>(this Enum value)
        {
            var member = value.GetType().GetMember(value.ToString()).FirstOrDefault();
            return member != null && Attribute.IsDefined(member, typeof(T));
        }
        public static bool IsFacebook(this Enum value) { return value.HasAttribute<FacebookAttribute>(); }
        public static bool IsInstagram(this Enum value) { return value.HasAttribute<InstagramAttribute>(); }
        public static bool IsTwitter(this Enum value) { return value.HasAttribute<TwitterAttribute>(); }
        public static bool IsPinterest(this Enum value) { return value.HasAttribute<PinterestAttribute>(); }
        public static bool IsLinkedIn(this Enum value) { return value.HasAttribute<LinkedInAttribute>(); }
        public static bool IsReddit(this Enum value) { return value.HasAttribute<RedditAttribute>(); }
        public static bool IsYoutube(this Enum value) { return value.HasAttribute<YoutubeAttribute>(); }
        public static bool IsTumblr(this Enum value) { return value.HasAttribute<TumblrAttribute>(); }
        public static bool IsQuora(this Enum value) { return value.HasAttribute<QuoraAttribute>(); }

    }
}
