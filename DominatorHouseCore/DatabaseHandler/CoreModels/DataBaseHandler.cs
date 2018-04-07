using System;
using System.Collections.Generic;
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


        static Dictionary<SocialNetworks, Action<DataBaseConnection>> _dbCounters = new Dictionary<SocialNetworks, Action<DataBaseConnection>>
        {
            {SocialNetworks.Twitter, db => db.Count<DominatorHouseCore.DatabaseHandler.TdTables.Accounts.Friendships>() },
            {SocialNetworks.Gplus, db => db.Count<Friendships>() },
            {SocialNetworks.Facebook, db => db.Count<Friendships>() },
            {SocialNetworks.Instagram, db => db.Count<Friendships>() },
            {SocialNetworks.Quora, db => db.Count<Friendships>() },
            {SocialNetworks.Pinterest, db => db.Count<PdTables.Accounts.Friendships>() },
        };

        public static void NewThread(Action act)
        {
            new Thread(() => act()) { IsBackground = true }.Start();
        }

        public static void CreateDataBase(string DBName, SocialNetworks networks, DatabaseType? databaseType = DatabaseType.AccountType, Action<Action> executionStrategy = null)
        {
            if (executionStrategy == null) executionStrategy = NewThread;
            try
            {
                DataBaseConnection databaseConnection =
                    GetDataBaseConnectionInstance(DBName, networks,databaseType);
                executionStrategy(() => _dbCounters[networks](databaseConnection));
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
