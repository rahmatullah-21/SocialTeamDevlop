using System.Data.Entity;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.AccountDB
{
    public class ModelConfiguration
    {
        public static void Configure(DbModelBuilder modelBuilder, SocialNetworks network )
        {
            ConfigureAccountdataBaseEntity(modelBuilder, network);
        }

        #region Commented
        //private static void ConfigureTeamEntity(DbModelBuilder modelBuilder)
        //{
        //    modelBuilder.Entity<Team>().ToTable("Base.MyTable")
        //        .HasRequired(t => t.Coach)
        //        .WithMany()
        //        .WillCascadeOnDelete(false);

        //    modelBuilder.Entity<Team>()
        //        .HasRequired(t => t.Stadion)
        //        .WithRequiredPrincipal()
        //        .WillCascadeOnDelete(true);
        //} 
        #endregion

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

                    break;
                case SocialNetworks.Gplus:
                    modelBuilder.Entity<FeedInfoes>();
                    modelBuilder.Entity<Friendships>();
                    modelBuilder.Entity<DailyStatitics>();
                    modelBuilder.Entity<InteractedPosts>();
                    modelBuilder.Entity<InteractedUsers>();
                    modelBuilder.Entity<UnfollowedUsers>();
                    break;
                case SocialNetworks.Facebook:
                    break;
                case SocialNetworks.LinkedIn:
                    break;
                case SocialNetworks.Quora:
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Youtube:
                    break;


            }

          
        }

    }
}
