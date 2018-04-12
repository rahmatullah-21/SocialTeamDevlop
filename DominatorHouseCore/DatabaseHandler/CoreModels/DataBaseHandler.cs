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

        private static Dictionary<SocialNetworks, Action<DataBaseConnection>> _dbCounters = new Dictionary<SocialNetworks, Action<DataBaseConnection>>
        {

            {SocialNetworks.Gplus,  db => {  db.Count<GplusTables.Accounts.Friendships>();}},
            {SocialNetworks.Twitter,   db =>  {  db.Count<TdTables.Accounts.Friendships>();}},
            {SocialNetworks.Facebook,  db =>  {  db.Count<FdTables.Accounts.Friends>();} },
            {SocialNetworks.Instagram, db => {  db.Count<Friendships>();  } },
            {SocialNetworks.Pinterest, db => {db.Count<PdTables.Accounts.Friendships>();} },
            {SocialNetworks.Quora,db => { db.Count<QdTables.Accounts.Friendships>(); } },
            {SocialNetworks.LinkedIn,  db => {  db.Count<LdTables.Account.Connections>();} },
            {SocialNetworks.Youtube,  db =>{    db.Count<YdTables.Accounts.Friendships>(); }}
        };


        static Dictionary<SocialNetworks, Action<DataBaseConnectionCampaign>> _dbCampaignCounters = new Dictionary<SocialNetworks, Action<DataBaseConnectionCampaign>>
        {

            {SocialNetworks.Gplus,  db => { db.Count<GplusTables.Campaigns.InteractedUsersReport>();}},
            {SocialNetworks.Twitter,   db =>  {  db.Count<TdTables.Campaign.InteractedUsers>();}},
            {SocialNetworks.Facebook,  db =>  { db.Count<FdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Instagram, db => {  db.Count<Friendships>();  db.Count<InteractedUsers>(); } },
            {SocialNetworks.Pinterest, db => { db.Count<PdTables.Campaigns.InteractedUsers>();} },
            {SocialNetworks.Quora,db => { db.Count<QdTables.Campaigns.InteractedUsers>(); } },
            {SocialNetworks.LinkedIn,  db => {  db.Count<LdTables.Campaign.InteractedUsers>();} },
            {SocialNetworks.Youtube,  db =>{   db.Count<YdTables.Campaign.InteractedUsers>(); }}
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
                if (databaseType == DatabaseType.AccountType)
                {
                    DataBaseConnection databaseConnection = GetDataBaseConnectionInstance(DBName, networks);
                    executionStrategy(() => _dbCounters[networks](databaseConnection));
                }
                else if (databaseType  == DatabaseType.CampaignType)
                {
                    DataBaseConnectionCampaign databaseConnection = GetDataBaseConnectionCampaignInstance(DBName, networks);
                    _dbCampaignCounters[networks](databaseConnection);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static DataBaseConnection GetDataBaseConnectionInstance(string DBName, SocialNetworks networks)
        {
            try
            {
                var objModelConfiguration = new ModelConfiguration();
                var directoryName = ConstantVariable.GetIndexAccountDir() + $"\\DB";
                DirectoryUtilities.CreateDirectory(directoryName);
                var connectionString = directoryName + $"\\{DBName}.db";
                return new DataBaseConnection(connectionString, networks, objModelConfiguration.ConfigureAccountdataBaseEntity);

            }
            catch (Exception)
            {
                return null;
            }
        }

        public static DataBaseConnectionCampaign GetDataBaseConnectionCampaignInstance(string DBName, SocialNetworks networks)
        {
            try
            {
                var objModelConfiguration = new ModelConfiguration();

                var directoryName = ConstantVariable.GetIndexCampaignDir() + $"\\DB";
                DirectoryUtilities.CreateDirectory(directoryName);
                var connectionString = directoryName + $"\\{DBName}.db";
                return new DataBaseConnectionCampaign(connectionString, networks, objModelConfiguration.ConfigureCampaignDataBaseEntity);
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion


    }
}
