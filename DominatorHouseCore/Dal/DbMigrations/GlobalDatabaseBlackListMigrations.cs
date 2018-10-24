namespace DominatorHouseCore.Dal.DbMigrations
{
    public interface IGlobalDatabaseBlackListMigrations : IDbMigration
    {

    }

    public class GlobalDatabaseBlackListMigrations : BaseDbMigrations, IGlobalDatabaseBlackListMigrations
    {
        public GlobalDatabaseBlackListMigrations()
        {
            AddMigrations(1, conn =>
            {
                conn.CreateTable<DatabaseHandler.DHTables.BlackListUser>();
                return "Initialization";
            });
        }
    }
}
