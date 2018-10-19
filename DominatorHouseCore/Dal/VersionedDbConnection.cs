using DominatorHouseCore.Dal.DbMigrations;
using SQLite;

namespace DominatorHouseCore.Dal
{
    public abstract class VersionedDbConnection : DbConnection
    {


        private readonly object _syncObject = new object();
        protected readonly IDbMigration DbMigration;

        protected VersionedDbConnection(IDbMigration dbMigration)
        {
            DbMigration = dbMigration;
        }

        protected SQLiteConnection GetSqlConnectionAndRunMigration(string path)
        {
            lock (_syncObject)
            {
                var conn = base.GetConnection(path);
                DbMigration.RunMigration(conn);

                return conn;
            }
        }
    }
}
