namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseWhiteListMigrations : IDbMigration
    {

    }

    public class GlobalDatabaseWhiteListMigrations : BaseDbMigrations, IGlobalDatabaseWhiteListMigrations
    {
        public GlobalDatabaseWhiteListMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.WhiteListUser>();
                return "Initialization";
            });
        }
    }
}
