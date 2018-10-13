using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using SQLite;

namespace DominatorHouseCore.DatabaseHandler.Utility
{
    public class GlobalDatabaseConnection : IGlobalDatabaseConnection
    {

        public SQLiteConnection GetSqlConnection()
        {
            var directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
            var connectionString = directoryName + $"\\Global.db";
            DirectoryUtilities.CreateDirectory(directoryName);
            var dbConnection = new SQLite.SQLiteConnection(connectionString);
            dbConnection.CreateTable<DHTables.AccountDetails>();
            dbConnection.CreateTable<DHTables.BlackWhiteListUser>();
            return dbConnection;
        }

        public SQLiteConnection GetSqlConnection(SocialNetworks networks, UserType userType)
        {
            var directoryName = ConstantVariable.GetPlatformBaseDirectory() + $"\\Index\\Global\\DB\\{userType}";
            var connectionString = directoryName + $"\\{networks}.db";
            DirectoryUtilities.CreateDirectory(directoryName);
            var dbConnection = new SQLiteConnection(@"data source=" + connectionString);
            if (userType == UserType.BlackListedUser)
            {
                dbConnection.Table<DHTables.BlackListUser>();
            }
            else
            {
                dbConnection.Table<DHTables.WhiteListUser>();
            }

            return dbConnection;
        }
    }
}