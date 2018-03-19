using System;
using System.Data.Entity;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class CommonDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<CommonDbContext>
    {
        public CommonDbInitializer(DbModelBuilder modelBuilder,Action<CommonDbContext> SeedDataBase )
            : base(modelBuilder, typeof(CustomHistory))
        {
           // SeedDataBase
        }

        protected override void Seed(CommonDbContext context)
        {
        }
    }
}