using DominatorHouseCore.Interfaces.StartUp;

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "DownloadScraper":
                case "UserScraper":
                    return new InstagramUserActivity();
                default:
                    return new InstagramPostActivity();
            }
        }

    }
}
