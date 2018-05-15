using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.SQLite;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler.Utility
{      
    public class GlobalDatabaseConnection : IGlobalDatabaseConnection
    {
        public string ConnectionString { get; set; }

        public DbContext GetDbContext()
        {
            try
            {
                var directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
                ConnectionString = directoryName + $"\\Global.db";
                DirectoryUtilities.CreateDirectory(directoryName);
                var dbConnection = new SQLiteConnection(@"data source=" + ConnectionString);
                var context = new GlobalDbContext(dbConnection, false);
                return context;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }
    }

    public class GlobalDbContext : DbContext
    {
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

        private void Configure()
        {
            Configuration.ProxyCreationEnabled = true;
            Configuration.LazyLoadingEnabled = true;
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            var tdModuleConfiguration = new GlobalModuleConfiguration();
            tdModuleConfiguration.Configuration(modelBuilder);
            var initializer = new GlobalDatabaseInitializer(modelBuilder);
            Database.SetInitializer(initializer);
        }
    }

    public class GlobalCustomHistory : IHistory
    {
        public int Id { get; set; }
        public string Hash { get; set; }
        public string Context { get; set; }
        public DateTime CreateDate { get; set; }
    }

    public class GlobalDatabaseInitializer : SqliteDropCreateDatabaseWhenModelChanges<GlobalDbContext>
    {
        public GlobalDatabaseInitializer(DbModelBuilder modelBuilder)
            : base(modelBuilder, typeof(GlobalCustomHistory))
        {
            // SeedDataBase
        }

        protected override void Seed(GlobalDbContext context)
        {

        }
    }

    public class GlobalModuleConfiguration : IModuleConfiguration
    {
        public void Configuration(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DHTables.AccountDetails>();
            modelBuilder.Entity<DHTables.BlackWhiteListUser>();
        }
    }


}