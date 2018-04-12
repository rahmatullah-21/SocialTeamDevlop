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

            {SocialNetworks.Gplus,
                db =>
                {
                    db.Count<GplusTables.Accounts.Friendships>();
                    db.Count<GplusTables.Campaigns.InteractedUsersReport>();
                }
            },

            {SocialNetworks.Twitter,
                db =>
                {
                    db.Count<TdTables.Accounts.Friendships>();
                    db.Count<TdTables.Campaign.InteractedUsers>();
                }
            },
            {SocialNetworks.Facebook,
                db =>
                {
                    db.Count<FdTables.Accounts.Friends>();
                    db.Count<FdTables.Campaigns.InteractedUsers>();
                }
            },
            {SocialNetworks.Instagram,
                db =>
                {
                    db.Count<Friendships>();
                    db.Count<InteractedUsers>();
                }
            },
            {SocialNetworks.Pinterest,
                db =>
                {
                    db.Count<PdTables.Accounts.Friendships>();
                    db.Count<PdTables.Campaigns.InteractedUsers>();
                }
            },
            {SocialNetworks.Quora,
                db =>
                {
                    db.Count<QdTables.Accounts.Friendships>();
                    db.Count<QdTables.Campaigns.InteractedUsers>();
                }
            },
            {SocialNetworks.LinkedIn,
                db =>
                {
                    db.Count<LdTables.Account.Connections>();
                    db.Count<LdTables.Campaign.InteractedUsers>();
                }
            },
            {SocialNetworks.Youtube,
            db =>
            {
                db.Count<YdTables.Accounts.Friendships>();
                db.Count<YdTables.Campaign.InteractedUsers>();
            }
            }
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
                    GetDataBaseConnectionInstance(DBName, networks, databaseType);
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

                if (databaseType == DatabaseType.AccountType)
                    return new DataBaseConnection(connectionString, networks, ModelConfiguration.ConfigureAccountdataBaseEntity);

                return new DataBaseConnection(connectionString, networks, ModelConfiguration.ConfigureCampaignDataBaseEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


    }
}
