using DominatorHouseCore.Enums;
using DominatorHouseCore.StartupActivity;
using DominatorHouseCore.StartupActivity.Facebook;
using DominatorHouseCore.StartupActivity.Instagram;
using DominatorHouseCore.StartupActivity.Linkedin;
using DominatorHouseCore.StartupActivity.Pinterest;
using DominatorHouseCore.StartupActivity.Quora;
using DominatorHouseCore.StartupActivity.Reddit;
using DominatorHouseCore.StartupActivity.Tumblr;
using DominatorHouseCore.StartupActivity.Twitter;
using DominatorHouseCore.StartupActivity.Youtube;
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
        private static Dictionary<string, INetworkActivity> Networks { get; } = new Dictionary<string, INetworkActivity>();
        public static INetworkActivity GetNetworkfactory(string networks)
        {
            return Networks.ContainsKey(networks) ? Networks[networks] : null;
        }
     
        public static void RegisterNetwork()
        {

            try
            {
                Networks.Add(SocialNetworks.Quora.ToString(), new QuoraActivity());
                Networks.Add(SocialNetworks.Facebook.ToString(), new FacebookActivity());
                Networks.Add(SocialNetworks.Instagram.ToString(), new InstagramActivity());
                Networks.Add(SocialNetworks.LinkedIn.ToString(), new LinkedinActivity());
                Networks.Add(SocialNetworks.Pinterest.ToString(), new PinterestActivity());
                Networks.Add(SocialNetworks.Reddit.ToString(), new RedditActivity());
                Networks.Add(SocialNetworks.Tumblr.ToString(), new TumblrActivity());
                Networks.Add(SocialNetworks.Twitter.ToString(), new TwitterActivity());
                Networks.Add(SocialNetworks.Youtube.ToString(), new YoutubeActivity());
            }
            catch {}
        }
    }
    #endregion

    public interface INetworkActivity
    {
        BaseActivity GetActivity(string activity);
    }

}
