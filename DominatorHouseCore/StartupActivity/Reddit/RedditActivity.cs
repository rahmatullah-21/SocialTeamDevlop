using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces.StartUp;

namespace DominatorHouseCore.StartupActivity.Reddit
{
    public class RedditActivity : INetworkActivity
    {
        public BaseActivity GetActivity(string activity)
        {
           
            switch (activity)
            {
                case "Follow":
                case "BroadcastMessages":
                case "UnSubscribe":
                case "UserScraper":
                    return new RedditUserActivity();
                case "ChannelScraper":
                case "Subscribe":
                    return new RedditCommunityActivity();
                case "RemoveVote":
                case "RemoveVoteComment":
                    return new RedditRemoveVoteActivity();
                default:
                    return new RedditPostActivity();
            }
        }
    }
}
