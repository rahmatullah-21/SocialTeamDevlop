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

        public static ObservableCollectionBase<string> GetUsers(SocialNetworks socialNetwork)
            => new ObservableCollectionBase<string>(GetAccountDetails(socialNetwork).Select(x => x.AccountBaseModel.UserName).ToList());

        public static ObservableCollectionBase<string> GetUsers<T>() where T : class
            => new ObservableCollectionBase<string>(GetAccountDetailsFor<T>().Select(x => (x as dynamic).UserName as string).ToList());


        public static bool Append<T>(T obj)
        {

            object locker = _accountDetailsFileLocker;
            string filePath = ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}";

            if (typeof(T) == typeof(CampaignDetails))
            {
                locker = _campaignsFileLocker;
                filePath = $"{ConstantVariable.socialNetworkPath(DominatorHouseInitializer.ActiveSocialNetwork)}\\{ConstantVariable.CampaignDetails}";
            }

            else if (typeof(T) == typeof(TemplateModel))
            {
                locker = _templatesFileLocker;
                filePath = $"{ConstantVariable.socialNetworkPath(DominatorHouseInitializer.ActiveSocialNetwork)}\\{ConstantVariable.TemplateBinName}";
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

        public static List<DominatorAccountModel> GetAccountDetails(SocialNetworks network)
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<DominatorAccountModel>(
                       ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");
        }

        public static List<DominatorAccountModel> GetAccountDetails()
                => GetAccountDetails(DominatorHouseInitializer.ActiveSocialNetwork);

        // TODO: back compatibility for account models of PD, TWD etc.
        public static List<T> GetAccountDetailsFor<T>() where T : class
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<T>(
                           ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");
        }


        // Get campigns for certain social network
        public static List<CampaignDetails> GetCampaignDetail(SocialNetworks network)
        {
            lock (_campaignsFileLocker)
                return ProtoBuffBase.DeserializeObjects<CampaignDetails>(
                    $"{ConstantVariable.socialNetworkPath(network)}\\{ConstantVariable.CampaignDetails}");
        }

        public static List<CampaignDetails> GetCampaignDetail()
            => GetCampaignDetail(DominatorHouseInitializer.ActiveSocialNetwork);

        
        // Get templates for certain social network
        public static List<TemplateModel> GetTemplateDetails(SocialNetworks network)
        {
            lock (_templatesFileLocker)
                return ProtoBuffBase.DeserializeObjects<TemplateModel>(
                    $"{ConstantVariable.socialConfigurationPath(network)}\\{ConstantVariable.TemplateBinName}");
        }

        public static List<TemplateModel> GetTemplateDetails()
            => GetTemplateDetails(DominatorHouseInitializer.ActiveSocialNetwork);


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
                    int indexOfAccountToUpdate =
                        accountDetailsList.FindIndex(x => x.AccountBaseModel.AccountId == accountModel.AccountBaseModel.AccountId);

                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList,
                            ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");

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
                    int indexOfAccountToUpdate =
                        accountDetailsList.FindIndex(x => (x as dynamic).AccountId == (accountModel as dynamic).AccountId);

                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeObjects(accountDetailsList,
                            ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");

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
                                ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");

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
                    ProtoBuffBase.SerializeObjects(campaignList, ConstantVariable.GetIndexCampaignPath() + $@"\{ConstantVariable.CampaignDetails}");
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
                    ProtoBuffBase.SerializeObjects(templatesList, ConstantVariable.GetTemplatesPath());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update campaigns error - " + ex.Message);
                }
            }
        }
    }
}
