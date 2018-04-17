using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class GlobalDbContext:DbContext
    {
        Action<DbModelBuilder> ConfigureDbModelBuilder;

        Action<GlobalDbContext> SeedDataBase;

     

        public GlobalDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        public GlobalDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        public GlobalDbContext(DbConnection connection, bool contextOwnsConnection,
           Action<DbModelBuilder> ConfigureDbModelBuilder = null,
            Action<GlobalDbContext> SeedDataBase = null)
            : base(connection, contextOwnsConnection)
        {
            Configure();
            this.ConfigureDbModelBuilder = ConfigureDbModelBuilder;
            this.SeedDataBase = SeedDataBase;
        }

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (this.ConfigureDbModelBuilder != null)
            {
                this.ConfigureDbModelBuilder(modelBuilder);
                var initializer = new GlobalDbInitializer(modelBuilder, this.SeedDataBase);
                Database.SetInitializer(initializer);
            }
        }
    }
}
