using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "Reposter":
                case "DownloadScraper":
                case "UserScraper":
                    return new InstagramUserActivity();
                default:
                    return new InstagramPostActivity();
            }
        }
    }
}
