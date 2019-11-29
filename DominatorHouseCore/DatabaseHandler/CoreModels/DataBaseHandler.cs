using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DominatorHouseCore.DatabaseHandler.CoreModels
{
    public interface IDataBaseHandler
    {
        IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; }
        IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbCampaignInitialCounters { get; }
        void DeleteDatabase(IEnumerable<string> DBNames, DatabaseType? databaseType = DatabaseType.AccountType);
    }


    public class DataBaseHandler : IDataBaseHandler
    {

        #region database Helper Methodtext,

        public IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbInitialCounters { get; } = new Dictionary<SocialNetworks, Action<DbOperations>>
        {
            {SocialNetworks.Instagram,(operation)=>{operation.Count<GdTables.Accounts.Friendships>();}},
        };

        public IReadOnlyDictionary<SocialNetworks, Action<DbOperations>> DbCampaignInitialCounters { get; } = new Dictionary<SocialNetworks, Action<DbOperations>>
        {
            {SocialNetworks.Instagram,operation=>{operation.Count<GdTables.Campaigns.InteractedUsers>(); } },
        };


        private static string GetDbPath(string DBName, string directoryName)
        {
            return directoryName + $"\\{DBName}.db";
        }

        private static string GetDirectory(DatabaseType? databaseType)
        {
            string directoryName;

            switch (databaseType)
            {
                case DatabaseType.CampaignType:
                    directoryName = ConstantVariable.GetIndexCampaignDir() + "\\DB";
                    break;
                case DatabaseType.AccountType:
                    directoryName = ConstantVariable.GetIndexAccountDir() + "\\DB";
                    break;
                default:
                    directoryName = ConstantVariable.GetPlatformBaseDirectory() + @"\Index\Global\DB";
                    break;
            }
            return directoryName;
        }

        public void DeleteDatabase(IEnumerable<string> DBNames, DatabaseType? databaseType = DatabaseType.AccountType)
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
                DirectoryInfo parent;
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
