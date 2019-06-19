using DominatorHouseCore.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TumblrDominatorCore.ViewModels.GrowFollower;
using TumblrDominatorCore.ViewModels.Blog;
using TumblrDominatorCore.ViewModels.Engage;
using TumblrDominatorCore.ViewModels.Message;
using TumblrDominatorCore.ViewModels.Scraper;

namespace DominatorHouse.ViewModels.Startup.ModuleConfig
{
    public class TumblrJobConfig : INetworkJobConfig
    {
        public Dictionary<string, object> RegisterJobConfigurations { get; set; } = new Dictionary<string, object>();

        public void RegisterJobConfiguration()
        {
            try
            {
                RegisterJobConfigurations.Add(ActivityType.Follow.ToString(), new FollowerViewModel());
                RegisterJobConfigurations.Add(ActivityType.Unfollow.ToString(), new UnfollowerViewModel());
                RegisterJobConfigurations.Add(ActivityType.Reblog.ToString(), new ReblogViewModel());
                RegisterJobConfigurations.Add(ActivityType.Comment.ToString(), new CommentViewModel());
                RegisterJobConfigurations.Add(ActivityType.Like.ToString(), new LikeViewModel());
                RegisterJobConfigurations.Add(ActivityType.BroadcastMessages.ToString(), new BroadcastMessagesViewModel());
                RegisterJobConfigurations.Add(ActivityType.CommentScraper.ToString(), new CommentScraperViewModel());
                RegisterJobConfigurations.Add(ActivityType.PostScraper.ToString(), new PostScraperViewModel());
                RegisterJobConfigurations.Add(ActivityType.UserScraper.ToString(), new UserScraperViewModel());


            }
            catch (System.Exception ex)
            {

            }
        }
    }
}
