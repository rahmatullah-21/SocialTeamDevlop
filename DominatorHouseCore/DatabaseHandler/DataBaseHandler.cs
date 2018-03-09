using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using System.Data.Entity;
using System.Threading;
using DominatorHouseCore.DatabaseHandler.AccountDB;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.Utility;
using SQLite.CodeFirst;

namespace DominatorHouseCore.DatabaseHandler
{
    public class DataBaseHandler
    {

        #region database Helper Method

        public static void CreateDataBase(string DBName, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            try
            {
                DataBaseConnectionCodeFirst.DataBaseConnection databaseConnection =
                    GetDataBaseConnectionInstance(DBName, databaseType);

                var initiaThread = new Thread(() => databaseConnection.Count<Friendships>()) { IsBackground = true };
                initiaThread.Start();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static DataBaseConnectionCodeFirst.DataBaseConnection GetDataBaseConnectionInstance(string DBName, DatabaseType? databaseType = DatabaseType.AccountType)
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
                return new DataBaseConnectionCodeFirst.DataBaseConnection(connectionString, ModelConfiguration.ConfigureAccountdataBaseEntity);
            }
            catch (Exception Ex)
            {
                return null;
            }
        }

        #endregion


    }
}
