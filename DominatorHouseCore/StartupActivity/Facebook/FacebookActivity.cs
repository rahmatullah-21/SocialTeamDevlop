using DominatorHouseCore.Interfaces.StartUp;
using DominatorHouseCore.StartupActivity.Quora;

namespace DominatorHouseCore.StartupActivity.Facebook
{
    public class FacebookActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
            switch (activity)
            {
                case "Follow":
                    return new QuoraFollowActivity();
                default:
                    return null;
            }
        }

    }
}
