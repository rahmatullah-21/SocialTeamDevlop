using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Dal.DbMigrations
{
  
    public interface IGlobalLocationDatabaseMigrations : IDbMigration
    {

    }

    public class GlobalLocationDatabaseMigrations : BaseDbMigrations, IGlobalLocationDatabaseMigrations
    {
        public GlobalLocationDatabaseMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.LocationList>();
                return "Initialization";
            });
        }
    }
}
