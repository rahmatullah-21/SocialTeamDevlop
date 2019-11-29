using DominatorHouseCore.Enums;
using DominatorHouseCore.StartupActivity;
using DominatorHouseCore.StartupActivity.Instagram;
using System.Collections.Generic;

namespace DominatorHouseCore.Interfaces.StartUp
{
    
    class SocialNetworkActivity
    {
        private static Dictionary<string, INetworkActivity> Networks { get; } = new Dictionary<string, INetworkActivity>();
        public static INetworkActivity GetNetworkActivity(string networks)
        {
            return Networks.ContainsKey(networks) ? Networks[networks] : null;
        }
     
        public static void RegisterNetwork()
        {

            try
            {
                Networks.Add(SocialNetworks.Instagram.ToString(), new InstagramActivity());
            }
            catch {}
        }
    }
   
    public interface INetworkActivity
    {
        BaseActivity GetActivity(string activity);
    }

}
