using System;
using System.Threading;
using DominatorHouseCore.DatabaseHandler.AccountDB;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DataBaseHandler
    {

        #region database Helper Method

        public static void CreateDataBase(string DBName,SocialNetworks networks, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            try
            {
                DataBaseConnection databaseConnection =
                    GetDataBaseConnectionInstance(DBName, networks,databaseType);

                switch (networks)
                {
                    case SocialNetworks.Twitter:
                        var initiaThread = new Thread(() => databaseConnection.Count<DominatorHouseCore.DatabaseHandler.TdTables.Accounts.Friendships>()) { IsBackground = true };
                        initiaThread.Start();
                        break;
                    case SocialNetworks.Gplus:
                        var initialGplusThread = new Thread(() => databaseConnection.Count<Friendships>()) { IsBackground = true };
                        initialGplusThread.Start();
                        break;

                }
               
               
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static DataBaseConnection GetDataBaseConnectionInstance(string DBName, SocialNetworks networks, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            try
            {
                string directoryName = string.Empty;
                switch (databaseType)
                {
                    case DatabaseType.CampaignType:
                        directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
                        break;
                    case DatabaseType.AccountType:
                        directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
                        break;
                    default:
                        directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
                        break;
                }

                DirectoryUtilities.CreateDirectory(directoryName);
                string connectionString = directoryName + $"\\{DBName}.db";
                return new DataBaseConnection(connectionString, networks, ModelConfiguration.ConfigureAccountdataBaseEntity);
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        #endregion


    }
}
