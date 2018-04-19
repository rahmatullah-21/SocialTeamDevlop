using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class GlobalDbInitializer : SqliteDropCreateDatabaseWhenModelChanges<GlobalDbContext>
    {
        public GlobalDbInitializer(DbModelBuilder modelBuilder, Action<GlobalDbContext> SeedDataBase)
            : base(modelBuilder, typeof(CustomHistory))
        {
           
        }
        
        protected override void Seed(GlobalDbContext context)
        {
        }
    }
}
