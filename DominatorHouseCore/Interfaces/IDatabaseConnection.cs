using System;
using System.Data.Entity;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;

namespace DominatorHouseCore.Interfaces
{
    public interface IDatabaseConnection
    {
        string ConnectionString { get; set; }

        DbContext GetContext(string connectionString);

    }

    public interface IGlobalDatabaseConnection
    {
        string ConnectionString { get; set; }

        DbContext GetDbContext();
        DbContext GetDbContext(SocialNetworks networks,UserType userType);
       
    }
}