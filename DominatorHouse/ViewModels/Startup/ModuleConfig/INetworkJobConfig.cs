using DominatorHouseCore.Enums;
using System.Collections.Generic;

namespace DominatorHouse.ViewModels.Startup.ModuleConfig
{
    interface INetworkJobConfig
    {
        void RegisterJobConfiguration();
        Dictionary<string, object> RegisterJobConfigurations { get; set; }
    }
    public class NetworkReg
    {
        public static Dictionary<string, object> RegisterNetworkJobConfig { get; set; } = new Dictionary<string, object>();
        static NetworkReg()
        {
            //RegisterNetworkJobConfig.Add(SocialNetworks.Facebook.ToString(), new FollowerModel());
            //RegisterNetworkJobConfig.Add(SocialNetworks.Instagram.ToString(), new UnfollowerModel());
            //RegisterNetworkJobConfig.Add(SocialNetworks.LinkedIn.ToString(), new ReportAnswerModel());
            //RegisterNetworkJobConfig.Add(SocialNetworks.Pinterest.ToString(), new ReportUserModel());
            RegisterNetworkJobConfig.Add(SocialNetworks.Quora.ToString(), new QuoraJobConfig());
            //RegisterNetworkJobConfig.Add(SocialNetworks.Reddit.ToString(), new SendMessageToFollowerModel());
            RegisterNetworkJobConfig.Add(SocialNetworks.Tumblr.ToString(), new TumblrJobConfig());
            //RegisterNetworkJobConfig.Add(SocialNetworks.Twitter.ToString(), new UserScraperModel());
            //RegisterNetworkJobConfig.Add(SocialNetworks.Youtube.ToString(), new AnswersScraperModel());

        }
    }
}
