using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Threading;
using System.Linq;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DataBaseHandler
    {

        #region database Helper Methodtext,

        public static Dictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; set; } = new Dictionary<SocialNetworks, Action< DbOperations>>
        {
            {SocialNetworks.Gplus,(operation) => {operation.Count<GplusTables.Accounts.Friendships>();}},
            {SocialNetworks.Twitter,(operation) =>{operation.Count<TdTables.Accounts.Friendships>();}},
            {SocialNetworks.Facebook,(operation)=>{operation.Count<FdTables.Accounts.Friends>();} },
            {SocialNetworks.Instagram,(operation)=>{operation.Count<GdTables.Accounts.Friendships>();}},
            {SocialNetworks.Pinterest,(operation) =>{operation.Count<PdTables.Accounts.Friendships>();} },
            {SocialNetworks.Quora,(operation) =>{operation.Count<QdTables.Accounts.Friendships>(); } },
            {SocialNetworks.LinkedIn,(operation) => {operation.Count<LdTables.Account.Connections>();} },
            {SocialNetworks.Youtube,(operation)=>{operation.Count<YdTables.Accounts.Friendships>(); }},
            {SocialNetworks.Reddit,(operation) => {operation.Count<RdTables.Accounts.InteractedUsers>();} },
            {SocialNetworks.Tumblr,(operation)=>{operation.Count<TumblrTables.Account.InteractedUser>(); }}
        };

        static Dictionary<SocialNetworks, Action<DbOperations>> _dbCampaignInitialCounters = new Dictionary<SocialNetworks, Action<DbOperations>>
        {
            {SocialNetworks.Gplus,operation=>{ operation.Count<GplusTables.Campaigns.InteractedUsersReport>();}},
            {SocialNetworks.Twitter,operation=>{operation.Count<TdTables.Campaign.InteractedUsers>();}},
            {SocialNetworks.Facebook,operation=>{operation.Count<FdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Instagram,operation=>{operation.Count<GdTables.Campaigns.InteractedUsers>(); } },
            {SocialNetworks.Pinterest,operation =>{operation.Count<PdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Quora,operation =>{ operation.Count<QdTables.Campaigns.InteractedUsers>(); } },
            {SocialNetworks.LinkedIn,operation=>{operation.Count<LdTables.Campaign.InteractedUsers>();} },
            {SocialNetworks.Youtube,operation=>{operation.Count<YdTables.Campaign.InteractedUsers>(); }},
            {SocialNetworks.Reddit,(operation) => {operation.Count<RdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Tumblr,(operation)=>{operation.Count<TumblrTables.Campaign.InteractedUser>(); }}
        };






        private static Dictionary<SocialNetworks, Action<DataBaseConnection>> _dbCounters { get; set; } = new Dictionary<SocialNetworks, Action<DataBaseConnection>>
        {
            {SocialNetworks.Gplus,db =>{db.Count<GplusTables.Accounts.Friendships>();}},
            {SocialNetworks.Twitter,db =>{db.Count<TdTables.Accounts.Friendships>();}},
            {SocialNetworks.Facebook,db=>{db.Count<FdTables.Accounts.Friends>();} },
            {SocialNetworks.Instagram,db=>{db.Count<GdTables.Accounts.Friendships>();}},
            {SocialNetworks.Pinterest,db =>{db.Count<PdTables.Accounts.Friendships>();} },
            {SocialNetworks.Quora,db =>{db.Count<QdTables.Accounts.Friendships>(); } },
            {SocialNetworks.LinkedIn,db => {db.Count<LdTables.Account.Connections>();} },
            {SocialNetworks.Youtube,db=>{db.Count<YdTables.Accounts.Friendships>(); }}
        };


        private static Dictionary<SocialNetworks, Action<DataBaseConnectionCampaign>> _dbCampaignCounters { get; set; } = new Dictionary<SocialNetworks, Action<DataBaseConnectionCampaign>>
        {
            {SocialNetworks.Gplus,db=>{ db.Count<GplusTables.Campaigns.InteractedUsersReport>();}},
            {SocialNetworks.Twitter,db=>{db.Count<TdTables.Campaign.InteractedUsers>();}},
            {SocialNetworks.Facebook,db=>{db.Count<FdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Instagram,db=>{db.Count<GdTables.Campaigns.InteractedUsers>(); } },
            {SocialNetworks.Pinterest,db =>{db.Count<PdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Quora,db =>{ db.Count<QdTables.Campaigns.InteractedUsers>(); } },
            {SocialNetworks.LinkedIn,db=>{db.Count<LdTables.Campaign.InteractedUsers>();} },
            {SocialNetworks.Youtube,db=>{db.Count<YdTables.Campaign.InteractedUsers>(); }}
        };

        private static Action<DataBaseConnectionGlobal> _dbGlobalCounters { get; set; } =InitilizeGlobalDb;
      

        public static void InitilizeGlobalDb(DataBaseConnectionGlobal db)
        {
            db.Count<DHTables.AccountDetails>();
            db.Count<DHTables.BlackWhiteListUser>();
        }



        public static void CreateDataBase(string dbName, SocialNetworks networks, DatabaseType? databaseType = DatabaseType.AccountType)
        {
            try
            {
                if (databaseType == DatabaseType.AccountType)
                {
                   // var databaseConnection = GetDataBaseConnectionInstance(dbName, networks);
                   // _dbCounters[networks](databaseConnection);
                }
                else if (databaseType  == DatabaseType.CampaignType)
                {
                 //   var databaseConnection = GetDataBaseConnectionCampaignInstance(dbName, networks);
                 //   _dbCampaignCounters[networks](databaseConnection);
                }
                else
                {
                    var databaseConnection = GetDataBaseConnectionGlobalInstance();
                    _dbGlobalCounters(databaseConnection);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public DataBaseConnection GetDataBaseConnection(string dbName, SocialNetworks networks)
        {
            try
            {

                string directoryName, connectionString;
                GetDbPath(dbName, DatabaseType.AccountType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();

                //var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
                //DirectoryUtilities.CreateDirectory(directoryName);
                //var connectionString = directoryName + $"\\{dbName}.db";
                return new DataBaseConnection(connectionString, networks, objModelConfiguration.ConfigureAccountdataBaseEntity);

            }
            catch (Exception)
            {
                return null;
            }
        }



        public static DataBaseConnection GetDataBaseConnectionInstance(string dbName, SocialNetworks networks)
        {
            try
            {

                string directoryName, connectionString;
                GetDbPath(dbName, DatabaseType.AccountType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();

                //var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
                //DirectoryUtilities.CreateDirectory(directoryName);
                //var connectionString = directoryName + $"\\{dbName}.db";
                return new DataBaseConnection(connectionString, networks, objModelConfiguration.ConfigureAccountdataBaseEntity);

            }
            catch (Exception)
            {
                return null;
            }
        }


        public DataBaseConnectionCampaign GetDataBaseConnectionCampaign(string dbName, SocialNetworks networks)
        {
            try
            {
                string directoryName, connectionString;
                GetDbPath(dbName, DatabaseType.CampaignType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();
                return new DataBaseConnectionCampaign(connectionString, networks, objModelConfiguration.ConfigureCampaignDataBaseEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static DataBaseConnectionCampaign GetDataBaseConnectionCampaignInstance(string dbName, SocialNetworks networks)
        {
            try
            {
                string directoryName, connectionString;
                GetDbPath(dbName, DatabaseType.CampaignType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();
                return new DataBaseConnectionCampaign(connectionString, networks, objModelConfiguration.ConfigureCampaignDataBaseEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        [Obsolete("Don't use GetDataBaseConnectionGlobalInstance with parameter instead use GetDataBaseConnectionGlobalInstance without parameter ")]
        public static DataBaseConnectionGlobal GetDataBaseConnectionGlobalInstance(string DBName)
        {
            try
            {
                string directoryName, connectionString;
                GetDbPath(DBName, DatabaseType.GlobalType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();
                return new DataBaseConnectionGlobal(connectionString, objModelConfiguration.ConfigureGlobalDataBaseEntity);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


       

        public static DataBaseConnectionGlobal GetDataBaseConnectionGlobalInstance()
        {
            try
            {
                string directoryName, connectionString;
                GetDbPath("Global", DatabaseType.GlobalType, out directoryName, out connectionString);
                DirectoryUtilities.CreateDirectory(directoryName);
                var objModelConfiguration = new ModelConfiguration();
                return new DataBaseConnectionGlobal(connectionString,  objModelConfiguration.ConfigureGlobalDataBaseEntity);
            }
            catch (Exception ex)
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
                    directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
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
