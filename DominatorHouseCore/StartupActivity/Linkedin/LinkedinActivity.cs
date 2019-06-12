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
                case "UserScraper":
                case "SalesNavigatorUserScraper":
                    return new LinkedinScraperActivity();
                case "JobScraper":
                case "CompanyScraper":
                case "SalesNavigatorCompanyScraper":
                    return new LinkedinCompanyScraperActivity();
                default:
                    return new LinkedinEngageActivity();
            }
        }

    }
}
