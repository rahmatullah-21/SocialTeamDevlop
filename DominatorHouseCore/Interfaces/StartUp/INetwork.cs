using DominatorHouseCore.Enums;
using DominatorHouseCore.StartupFactory;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces.StartUp
{
    interface ISocialNetwork
    {
        List<string> GetQueryType(string activityType);
    }
    #region Commented
    class NetworkFactory
    {
        private static Dictionary<string, INetworkFactory> Networks { get; } = new Dictionary<string, INetworkFactory>();
        public static INetworkFactory GetNetworkfactory(string networks)
        {
            return Networks.ContainsKey(networks) ? Networks[networks] : null;
        }
        public static void RegisterNetwork(string networks, INetworkFactory networkFactory)
        {
            if (Networks.ContainsKey(networks))
                return;

            Networks.Add(networks, networkFactory);
        }
        public static void RegisterNetwork()
        {

            try
            {
                Networks.Add(SocialNetworks.Quora.ToString(), new QuoraFactory());
                Networks.Add(SocialNetworks.Facebook.ToString(), new FacebookFactory());
                Networks.Add(SocialNetworks.Instagram.ToString(), new InstagramFactory());
                Networks.Add(SocialNetworks.LinkedIn.ToString(), new LinkedinFactory());
                Networks.Add(SocialNetworks.Pinterest.ToString(), new PinterestFactory());
                Networks.Add(SocialNetworks.Reddit.ToString(), new RedditFactory());
                Networks.Add(SocialNetworks.Tumblr.ToString(), new TumblrFactory());
                Networks.Add(SocialNetworks.Twitter.ToString(), new TwitterFactory());
                Networks.Add(SocialNetworks.Youtube.ToString(), new YoutubeFactory());
            }
            catch (System.Exception ex)
            {

              
            }
        }
    }
    //public class ActivityFactory
    //{
    //    private static Dictionary<string, List<string>> Networks { get; } = new Dictionary<string, List<string>>();

    //    public static void RegisterActivity(string activity, List<string> queryType)
    //    {
    //        if (Networks.ContainsKey(activity))
    //            return;

    //        Networks.Add(activity, queryType);
    //    }

    //    public List<string> GetQueryType(string activityType)
    //    {
    //        return Networks.ContainsKey(activityType) ? Networks[activityType] : null;
    //    }
    //} 
    #endregion

    public interface INetworkFactory
    {
        BaseActivityFactory GetActivityFactory(string activity);
    }

}
