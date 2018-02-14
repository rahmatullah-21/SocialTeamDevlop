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
    public class BinFileHelper
    {

        #region Read Accounts

        private static readonly object _accountDetailsFileLocker = new object();
        private static readonly object _campaignsFileLocker = new object();
        private static readonly object _templatesFileLocker = new object();

        #endregion

        public static ObservableCollectionBase<string> GetUsers(SocialNetworks socialNetwork)
            => new ObservableCollectionBase<string>(GetAccountDetails(socialNetwork).Select(x => x.AccountBaseModel.UserName).ToList());


        public static IEnumerable<DominatorAccountModel> GetAccountDetails(SocialNetworks network)
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<DominatorAccountModel>(
                       ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");
        }

        public static IEnumerable<DominatorAccountModel> GetAccountDetails()
                => GetAccountDetails(DominatorHouseInitializer.ActiveSocialNetwork);

        public static IEnumerable<T> GetAccountDetailsFor<T>(SocialNetworks network) where T : class
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<T>(
                    ConstantVariable.GetIndexAccountPath(network) + $@"\{ConstantVariable.AccountDetails}");
        }        

        public static IEnumerable<CampaignDetails> GetCampaignDetail(SocialNetworks network)
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<CampaignDetails>(
                    $"{ConstantVariable.socialNetworkPath(network)}\\{ConstantVariable.CampaignDetails}");
        }

        public static IEnumerable<CampaignDetails> GetCampaignDetail()
            => GetCampaignDetail(DominatorHouseInitializer.ActiveSocialNetwork);

        public static IEnumerable<TemplateModel> GetTemplateDetails(SocialNetworks network)
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeObjects<TemplateModel>(
                    $"{ConstantVariable.socialConfigurationPath(network)}\\{ConstantVariable.TemplateBinName}");
        }

        public static IEnumerable<TemplateModel> GetTemplateDetails()
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
                    var accountDetailsList = GetAccountDetails().ToList();      
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

        public static void UpdateAllAccounts(IList<DominatorAccountModel> accountDetailsList)
        {
            UpdateAllAccounts<DominatorAccountModel>(accountDetailsList);
        }


        // TODO: back compatibility to save old AccountModel. Have to be replaced with IList<DominatorAccountModel>
        public static void UpdateAllAccounts<T>(IList<T> accountDetailsList)
        {
            lock (_accountDetailsFileLocker)
            {
                try
                {
                    ProtoBuffBase.SerializeObjects(accountDetailsList,
                                ConstantVariable.GetIndexAccountPath() + $@"\{ConstantVariable.AccountDetails}");
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update All Accounts error - " + ex.Message);
                    ex.DebugLog();
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

        public static void UpdateTemplates(IEnumerable<TemplateModel> templatesList)
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
