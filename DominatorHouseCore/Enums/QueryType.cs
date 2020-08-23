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
}
