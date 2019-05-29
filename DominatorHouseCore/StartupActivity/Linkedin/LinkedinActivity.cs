using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.StartUp;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    public class LinkedinActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "GroupInviter":
                case "GroupJoiner":
                    return new LinkedinGroupActivity();
                case "ConnectionRequest":
                    return new LinkedinConnectionActivity();
                case "BroadcastMessages":
                    return new LinkedinMessageActivity();
                case "CompanyScraper":
                case "JobScraper":
                case "UserScraper":
                    return new LinkedinScraperActivity();
                default:
                    return new LinkedinEngageActivity();
            }
        }

    }
}
