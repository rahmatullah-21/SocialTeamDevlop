using System;
using System.Data.Common;
using System.Data.Entity;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class CommonDbContext : DbContext
    {

        Action<DbModelBuilder,SocialNetworks> ConfigureDbModelBuilder;

        Action<CommonDbContext> SeedDataBase;

        private SocialNetworks Networks { get; set; }

        public CommonDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        public CommonDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        public CommonDbContext(DbConnection connection, bool contextOwnsConnection, SocialNetworks networks ,Action<DbModelBuilder, SocialNetworks> ConfigureDbModelBuilder = null , Action<CommonDbContext> SeedDataBase = null)
          : base(connection, contextOwnsConnection)
        {
            Configure();
            this.Networks = networks;
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
                this.ConfigureDbModelBuilder(modelBuilder, Networks);
                var initializer = new CommonDbInitializer(modelBuilder, this.SeedDataBase);
                Database.SetInitializer(initializer);
            }
        }
    }
}