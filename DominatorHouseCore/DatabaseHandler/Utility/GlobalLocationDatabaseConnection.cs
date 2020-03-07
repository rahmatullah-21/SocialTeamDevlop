using DominatorHouseCore.Dal;
using DominatorHouseCore.Dal.DbMigrations;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class GlobalLocationDatabaseConnection : VersionedDbConnection, IGlobalLocationDatabaseConnection
    {
        
        
        public GlobalLocationDatabaseConnection(IGlobalLocationDatabaseMigrations dbMigration) : base(dbMigration)
        {
            
        }


        public SQLiteConnection GetSqlConnection()
        {
            var directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
            DirectoryUtilities.CreateDirectory(directoryName);
            var connectionString = directoryName + "\\GlobalLocation.db";
            return GetSqlConnectionAndRunMigration(connectionString);
        }

        
    }
}
