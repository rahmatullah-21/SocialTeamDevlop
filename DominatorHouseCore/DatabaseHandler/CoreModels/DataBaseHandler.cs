using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public class DataBaseHandler
    {

        #region database Helper Methodtext,

        public static Dictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; set; } = new Dictionary<SocialNetworks, Action<DbOperations>>
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

        public static Dictionary<SocialNetworks, Action<DbOperations>> DbCampaignInitialCounters { get; set; } = new Dictionary<SocialNetworks, Action<DbOperations>>
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
            try
            {
                DirectoryInfo parent = null;
                for (var dir = new DirectoryInfo(directory); dir.EnumerateDirectories().FirstOrDefault() == null && dir.EnumerateFiles().FirstOrDefault() == null; dir = parent)
                {
                    parent = dir.Parent;
                    dir.Delete();
                }
            }
            catch (IOException)
            {
            }
        }

        #endregion


    }
}
