using System.Data.Entity;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class ModelConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder, SocialNetworks network )
        {
            ConfigureAccountdataBaseEntity(modelBuilder, network);
        }
    
        public static void ConfigureAccountdataBaseEntity(DbModelBuilder modelBuilder, SocialNetworks networks)
        {
            switch (networks)
            {
                case SocialNetworks.Instagram:
                    modelBuilder.Entity<FeedInfoes>();
                    modelBuilder.Entity<Friendships>();
                    modelBuilder.Entity<DailyStatitics>();
                    modelBuilder.Entity<InteractedPosts>();
                    modelBuilder.Entity<InteractedUsers>();
                    modelBuilder.Entity<UnfollowedUsers>();
                    modelBuilder.Entity<HashtagScrape>();
                    break;
                case SocialNetworks.Twitter:
                    modelBuilder.Entity<TdTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<TdTables.Accounts.Friendships>();
                    modelBuilder.Entity<TdTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<TdTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<TdTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<TdTables.Accounts.UnfollowedUsers>();
                    break;
                case SocialNetworks.Pinterest:
                    modelBuilder.Entity<PdTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<PdTables.Accounts.Friendships>();
                    modelBuilder.Entity<PdTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<PdTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<PdTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<PdTables.Accounts.UnfollowedUsers>();
                    modelBuilder.Entity<PdTables.Accounts.InteractedBoards>();
                    break;
                case SocialNetworks.Gplus:
                    modelBuilder.Entity<GplusTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<GplusTables.Accounts.Friendships>();
                    modelBuilder.Entity<GplusTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<GplusTables.Accounts.UnfollowedUsers>();
                    modelBuilder.Entity<GplusTables.Accounts.Communities>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedCommunities>();
                    break;
                case SocialNetworks.Facebook:
                    break;
                case SocialNetworks.LinkedIn:
                    modelBuilder.Entity<LdTables.Account.Connections>();
                    modelBuilder.Entity<LdTables.Account.DailyStatitics>();
                    modelBuilder.Entity<LdTables.Account.FeedInfo>();
                    modelBuilder.Entity<LdTables.Account.InteractedCompanies>();
                    modelBuilder.Entity<LdTables.Account.InteractedGroups>();
                    modelBuilder.Entity<LdTables.Account.InteractedJobs>();
                    modelBuilder.Entity<LdTables.Account.InteractedPosts>();
                    modelBuilder.Entity<LdTables.Account.InteractedUsers>();
                    break;
                case SocialNetworks.Quora:
                    modelBuilder.Entity<QdTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<QdTables.Accounts.Friendships>();
                    modelBuilder.Entity<QdTables.Accounts.DailyStatitics>();                    
                    modelBuilder.Entity<QdTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<QdTables.Accounts.UnfollowedUsers>();
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Youtube:
                    modelBuilder.Entity<YdTables.Accounts.FeedInfos>();
                    modelBuilder.Entity<YdTables.Accounts.Friendships>();
                    modelBuilder.Entity<YdTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<YdTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<YdTables.Accounts.InteractedChannels>();
                    modelBuilder.Entity<YdTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<YdTables.Accounts.UnfollowedUsers>();
                    break;
            }       
        }


        public static void ConfigureCampaignDataBaseEntity(DbModelBuilder modelBuilder, SocialNetworks networks)
        {
            switch (networks)
            {
                case SocialNetworks.Instagram:
                    modelBuilder.Entity<FeedInfoes>();
                    modelBuilder.Entity<Friendships>();
                    modelBuilder.Entity<DailyStatitics>();
                    modelBuilder.Entity<InteractedPosts>();
                    modelBuilder.Entity<InteractedUsers>();
                    modelBuilder.Entity<UnfollowedUsers>();
                    modelBuilder.Entity<HashtagScrape>();
                    break;
                case SocialNetworks.Twitter:
                    modelBuilder.Entity<TdTables.Campaign.InteractedPosts>();
                    modelBuilder.Entity<TdTables.Campaign.InteractedUsers>();
                    modelBuilder.Entity<TdTables.Campaign.UnfollowedUsers>();
                    break;
                case SocialNetworks.Pinterest:
                    modelBuilder.Entity<PdTables.Campaigns.InteractedPosts>();
                    modelBuilder.Entity<PdTables.Campaigns.InteractedUsers>();
                    modelBuilder.Entity<PdTables.Campaigns.UnfollowedUsers>();
                    break;
                case SocialNetworks.Gplus:
                    // still not added campaign DB
                    modelBuilder.Entity<GplusTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<GplusTables.Accounts.Friendships>();
                    modelBuilder.Entity<GplusTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<GplusTables.Accounts.UnfollowedUsers>();
                    modelBuilder.Entity<GplusTables.Accounts.Communities>();
                    modelBuilder.Entity<GplusTables.Accounts.InteractedCommunities>();
                    break;
                case SocialNetworks.Facebook:
                    break;
                case SocialNetworks.LinkedIn:
                    modelBuilder.Entity<LdTables.Campaign.InteractedCompanies>();
                    modelBuilder.Entity<LdTables.Campaign.InteractedGroups>();
                    modelBuilder.Entity<LdTables.Campaign.InteractedJobs>();
                    modelBuilder.Entity<LdTables.Campaign.InteractedPosts>();
                    modelBuilder.Entity<LdTables.Campaign.InteractedUsers>();
                    break;
                case SocialNetworks.Quora:                
                    modelBuilder.Entity<QdTables.Campaigns.InteractedPosts>();
                    modelBuilder.Entity<QdTables.Campaigns.InteractedUsers>();
                    modelBuilder.Entity<QdTables.Campaigns.InteracteractedAnswers>();
                    modelBuilder.Entity<QdTables.Campaigns.InteracteractedQuestion>();
                    modelBuilder.Entity<QdTables.Campaigns.UnfollowedUsers>();
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Youtube:
                    modelBuilder.Entity<YdTables.Campaign.InteractedUsers>();
                    modelBuilder.Entity<YdTables.Campaign.UnfollowedUsers>();
                    modelBuilder.Entity<YdTables.Campaign.InteractedPosts>();
                    modelBuilder.Entity<YdTables.Campaign.InteractedChannels>();
                    break;
            }


        }

    }
}
