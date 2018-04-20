using System;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class CampaignDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<CampaignDbContext>
    {
        public CampaignDbInitializer(DbModelBuilder modelBuilder, Action<CampaignDbContext> SeedDataBase)
            : base(modelBuilder, typeof(CustomHistory))
        {
            // SeedDataBase
        }

        protected override void Seed(CampaignDbContext context)
        {
        }
    }
}