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
                    break;
                case SocialNetworks.Twitter:
                    modelBuilder.Entity<TdTables.Accounts.FeedInfoes>();
                    modelBuilder.Entity<TdTables.Accounts.Friendships>();
                    modelBuilder.Entity<TdTables.Accounts.DailyStatitics>();
                    modelBuilder.Entity<TdTables.Accounts.InteractedPosts>();
                    modelBuilder.Entity<TdTables.Accounts.InteractedUsers>();
                    modelBuilder.Entity<TdTables.Accounts.UnfollowedUsers>();
                    //modelBuilder.Entity<TdTables.Campaign.InteractedPosts>();
                    //modelBuilder.Entity<TdTables.Campaign.InteractedUsers>();
                    //modelBuilder.Entity<TdTables.Campaign.UnfollowedUsers>();
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
                    break;
                case SocialNetworks.Facebook:
                    break;
                case SocialNetworks.LinkedIn:
                    break;
                case SocialNetworks.Quora:
                    modelBuilder.Entity<FeedInfoes>();
                    modelBuilder.Entity<Friendships>();
                    modelBuilder.Entity<DailyStatitics>();
                    modelBuilder.Entity<InteractedPosts>();
                    modelBuilder.Entity<InteractedUsers>();
                    modelBuilder.Entity<UnfollowedUsers>();
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Youtube:
                    break;


            }

          
        }

    }
}
