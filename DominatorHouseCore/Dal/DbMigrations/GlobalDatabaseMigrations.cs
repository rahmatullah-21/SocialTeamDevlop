namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseMigrations : IDbMigration
    {

    }

    public class GlobalDatabaseMigrations : BaseDbMigrations, IGlobalDatabaseMigrations
    {
        public GlobalDatabaseMigrations()
        {
            AddMigrations(2, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.AccountDetails>();
                conn.CreateTable<DatabaseHandler.DHTables.BlackWhiteListUser>();
                return "Initialization";
            });
        }
    }
}
