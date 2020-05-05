namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseMigrations : IDbMigration
    {

    }

    public class GlobalDatabaseMigrations : BaseDbMigrations, IGlobalDatabaseMigrations
    {
        public GlobalDatabaseMigrations()
        {
            AddMigrations(7, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.AccountDetails>();
                conn.CreateTable<DatabaseHandler.DHTables.BlackWhiteListUser>();
                conn.CreateTable<DatabaseHandler.DHTables.LocationList>();
                conn.CreateTable<DatabaseHandler.DHTables.InstaAccountBackup>();
                return "Initialization";
            });
        }
    }
}
