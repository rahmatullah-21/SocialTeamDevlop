using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Linq;
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
                string directoryName, connectionString;
                GetDbPath(DBName, databaseType, out directoryName, out connectionString);

                DirectoryUtilities.CreateDirectory(directoryName);

                if (databaseType == DatabaseType.AccountType)
                    return new DataBaseConnection(connectionString, networks, ModelConfiguration.ConfigureAccountdataBaseEntity);

                return new DataBaseConnection(connectionString, networks, ModelConfiguration.ConfigureCampaignDataBaseEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static void GetDbPath(string DBName, DatabaseType? databaseType, out string directoryName, out string connectionString)
        {
            directoryName = GetDirectory(databaseType);
            connectionString = GetDbPath(DBName, directoryName);
        }

        private static string GetDbPath(string DBName, string directoryName)
        {
            return directoryName + $"\\{DBName}.db";
        }

        private static string GetDirectory(DatabaseType? databaseType)
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

            return directoryName;
        }

        public static void DeleteDatabase(IEnumerable<string> DBNames, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            var directory = GetDirectory(databaseType);
            if (Directory.Exists(directory))
            {
                DBNames
                    .Select(name => GetDbPath(name, directory))
                    .Where(File.Exists)
                    .ForEach(File.Delete);
            }
            // if directories are now empty, remove them
            DirectoryInfo parent = null;
            for (var dir = new DirectoryInfo(directory); dir.EnumerateDirectories().FirstOrDefault() == null && dir.EnumerateFiles().FirstOrDefault() == null; dir = parent)
            {
                parent = dir.Parent;
                dir.Delete();
            }
        }

        #endregion


    }
}
