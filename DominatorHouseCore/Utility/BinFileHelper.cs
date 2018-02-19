using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.IO;
using System.Diagnostics;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;

namespace DominatorHouseCore.Utility
{
    internal class BinFileHelper
    {
        private static readonly object _accountDetailsFileLocker = new object();
        private static readonly object _campaignsFileLocker = new object();
        private static readonly object _templatesFileLocker = new object();

        public static ObservableCollectionBase<string> GetUsers()
            => new ObservableCollectionBase<string>(GetAccountDetails().Select(x => x.AccountBaseModel.UserName).ToList());

        public static ObservableCollectionBase<string> GetUsers<T>() where T : class
            => new ObservableCollectionBase<string>(GetAccountDetailsFor<T>().Select(x => (x as dynamic).UserName as string).ToList());


        public static bool Append<T>(T obj)
        {

            object locker = _accountDetailsFileLocker;
            string filePath = ConstantVariable.GetIndexAccountFile();

            if (typeof(T) == typeof(CampaignDetails))
            {
                locker = _campaignsFileLocker;
                filePath = ConstantVariable.GetIndexCampaignFile();
            }

            else if (typeof(T) == typeof(TemplateModel))
            {
                locker = _templatesFileLocker;
                filePath = ConstantVariable.GetTemplatesFile();
            }

            try
            {               
                lock (locker)
                    ProtoBuffBase.AppendObject<T>(obj, filePath);
                return true;
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error($"Error caught while adding the account "+ex.StackTrace);
                return false;
            }
        }

        public static List<DominatorAccountModel> GetAccountDetails()
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<DominatorAccountModel>(ConstantVariable.GetIndexAccountFile());
        }

        
        // TODO: back compatibility for account models of PD, TWD etc.
        // Modify index account path. Uses only for testing purposes of PD, TWD and others.
        public static List<T> GetAccountDetailsFor<T>() where T : class
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<T>(ConstantVariable.GetIndexAccountFile());
        }


        // Get all campigns 
        public static List<CampaignDetails> GetCampaignDetail()
        {
            lock (_campaignsFileLocker)
                return ProtoBuffBase.DeserializeObjects<CampaignDetails>(ConstantVariable.GetIndexCampaignFile());
        }
        
        
        // Get all templates 
        public static List<TemplateModel> GetTemplateDetails()
        {
            lock (_templatesFileLocker)
                return ProtoBuffBase.DeserializeObjects<TemplateModel>(ConstantVariable.GetTemplatesFile());
        }


        public static int FindAccountIndex<T>(List<T> accounts, string id)
        {
            return typeof(T) == typeof(DominatorAccountModel)                                            ?
                accounts.FindIndex(a => (a as DominatorAccountModel).AccountBaseModel.AccountId == id)   :
                accounts.FindIndex(a => (a as dynamic).AccountId == id);            
        }

        /// <summary>
        /// Overwrites AccountDetails.bin with updated account
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        public static bool UpdateAccount(DominatorAccountModel accountModel)
        {
            try
            {
                lock (_accountDetailsFileLocker)
                {
                    var accountDetailsList = GetAccountDetails();
                    int indexOfAccountToUpdate = FindAccountIndex(accountDetailsList, accountModel.AccountBaseModel.AccountId);

                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList, ConstantVariable.GetIndexAccountFile());

                    GlobusLogHelper.log.Trace($"Update Accounts - [{result}]");
                    return result;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update account details error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }

        // TODO: backward compatibility
        internal static bool UpdateAccount<T>(T accountModel) where T : class
        {
            try
            {
                lock (_accountDetailsFileLocker)
                {
                    var accountDetailsList = GetAccountDetailsFor<T>();
                    int indexOfAccountToUpdate = FindAccountIndex(accountDetailsList,(accountModel as dynamic).AccountId);
                        
                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList,
                                                                 ConstantVariable.GetIndexAccountFile());

                    GlobusLogHelper.log.Trace($"Update Accounts - [{result}]");
                    return result;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update account details error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }


        public static bool UpdateAllAccounts(IList<DominatorAccountModel> accountDetailsList)
        {
            return UpdateAllAccounts<DominatorAccountModel>(accountDetailsList);
        }


        // TODO: back compatibility to save old AccountModel. Have to be replaced with IList<DominatorAccountModel>
        public static bool UpdateAllAccounts<T>(IList<T> accountDetailsList)
        {
            lock (_accountDetailsFileLocker)
            {
                try
                {
                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList,
                                                                 ConstantVariable.GetIndexAccountFile());

                    GlobusLogHelper.log.Debug("Accounts succesfully saved");

                    return result;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update All Accounts error - " + ex.Message);
                    ex.DebugLog();
                    return false;
                }
            }
        }
        

        public static void UpdateCampaigns(IList<CampaignDetails> campaignList)
        {
            lock (_campaignsFileLocker)
            {
                try
                {
                    ProtoBuffBase.SerializeObjects(campaignList, ConstantVariable.GetIndexCampaignFile());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update campaigns error - " + ex.Message);
                }
            }
        }

        public static void UpdateTemplates(List<TemplateModel> templatesList)
        {
            lock (_templatesFileLocker)
            {
                try
                {
                    ProtoBuffBase.SerializeObjects(templatesList, ConstantVariable.GetTemplatesFile());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update campaigns error - " + ex.Message);
                }
            }
        }
    }
}
