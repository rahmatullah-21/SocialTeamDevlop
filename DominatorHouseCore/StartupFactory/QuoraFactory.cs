using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.GdQuery;
using DominatorHouseCore.Enums.QdQuery;
using DominatorHouseCore.Interfaces.StartUp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupFactory
{
    public class QuoraFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class FacebookFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class InstagramFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "Reposter":
                case "DownloadScraper":
                case "UserScraper":
                    return new InstagramFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class YoutubeFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class PinterestFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class LinkedinFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
     public class TwitterFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class TumblrFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public class RedditFactory : INetworkFactory
    {
        public BaseActivityFactory GetActivityFactory(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivityFactory();
                default:
                    return null;
            }
        }

    }
    public abstract class BaseActivityFactory
    {
        public abstract List<string> GetQueryType();
    }
    public class QuoraFollowActivityFactory : BaseActivityFactory
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(FollowerQuery)).ToList();
        }
    }
    public class InstagramFollowActivityFactory : BaseActivityFactory
    {
        public override List<string> GetQueryType()
        {
         return Enum.GetNames(typeof(GdUserQuery)).ToList();
        }
    }
}
