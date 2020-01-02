namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseMigrations : IDbMigration
    {

    }

    public class GlobalDatabaseMigrations : BaseDbMigrations, IGlobalDatabaseMigrations
    {
        public GlobalDatabaseMigrations()
        {
            AddMigrations(3, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.AccountDetails>();
                conn.CreateTable<DatabaseHandler.DHTables.BlackWhiteListUser>();
                conn.CreateTable <DatabaseHandler.DHTables.LoggerData>();
                return "Initialization";
            });
        }
    }
}
