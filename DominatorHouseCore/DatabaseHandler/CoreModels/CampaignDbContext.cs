using System;
using System.Data.Common;
using System.Data.Entity;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class CampaignDbContext : DbContext
    {
        Action<DbModelBuilder, SocialNetworks> ConfigureDbModelBuilder;

        Action<CampaignDbContext> SeedDataBase;

        private SocialNetworks Networks { get; set; }

        public CampaignDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        public CampaignDbContext(DbConnection connection, bool contextOwnsConnection)
            : base(connection, contextOwnsConnection)
        {
            Configure();
        }

        public CampaignDbContext(DbConnection connection, bool contextOwnsConnection, SocialNetworks networks, Action<DbModelBuilder, SocialNetworks> ConfigureDbModelBuilder = null, Action<CampaignDbContext> SeedDataBase = null)
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
                var initializer = new CampaignDbInitializer(modelBuilder, this.SeedDataBase);
              //  Database.SetInitializer(initializer);
            }
        }
    }
}