using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.Utility
{
    public class DataBaseHandler
    {

        #region database Helper Method

        public static void CreateDataBase(string DBName , DatabaseType ? databaseType = DatabaseType.AccountType)
        {
            try
            {
                DataBaseConnectionCodeFirst.DataBaseConnection databaseConnection =
                    GetDataBaseConnectionInstance(DBName, databaseType);
                databaseConnection.Add<DataBaseConnection.CommonDatabaseConnection.Tables.Account.Friendships>(new DataBaseConnection.CommonDatabaseConnection.Tables.Account.Friendships()
                {
                    FullName = "Vikas Singh"
                });
                List<DataBaseConnection.CommonDatabaseConnection.Tables.Account.Friendships> lstFriendships = databaseConnection.Get<DataBaseConnection.CommonDatabaseConnection.Tables.Account.Friendships>();
            }
            catch (Exception Ex)
            {

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
                        directoryName = ConstantVariable.GetIndexCampaignPath(SocialNetworks.Instagram) + $"\\DB";//GetIndexCampaignPath(SocialNetworks.Instagram)
                        break;
                    case DatabaseType.AccountType:
                        directoryName = ConstantVariable.GetIndexAccountPath() + $"\\DB";//GetIndexAccountPath()
                        break;
                    default:
                        directoryName = ConstantVariable.GetIndexAccountPath() + $"\\DB";
                        break;
                }
         
                DirectoryUtilities.CreateDirectory(directoryName);  
                string connectionString = directoryName + $"\\{DBName}.db";
                return new DataBaseConnectionCodeFirst.DataBaseConnection(connectionString, SQLite.CodeFirst.ModelConfiguration.ConfigureAccountdataBaseEntity);
            }
            catch (Exception Ex)
            {
                return null;
            }

        }


      

        #endregion


    }
}
