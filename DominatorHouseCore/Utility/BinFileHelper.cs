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
        private static readonly object _proxyFileLocker = new object();
        private static readonly object _postFileLocker = new object();
        private static readonly object _configFileLocker = new object();
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
                return ProtoBuffBase.DeserializeList<DominatorAccountModel>(ConstantVariable.GetIndexAccountFile());
        }

        
        // TODO: back compatibility for account models of PD, TWD etc.
        // Modify index account path. Uses only for testing purposes of PD, TWD and others.
        public static List<T> GetAccountDetailsFor<T>() where T : class
        {
            lock (_accountDetailsFileLocker)
                return ProtoBuffBase.DeserializeList<T>(ConstantVariable.GetIndexAccountFile());
        }


        // Get all campigns 
        public static List<CampaignDetails> GetCampaignDetail()
        {
            lock (_campaignsFileLocker)
                return ProtoBuffBase.DeserializeList<CampaignDetails>(ConstantVariable.GetIndexCampaignFile());
        }
        
        
        // Get all templates 
        public static List<TemplateModel> GetTemplateDetails()
        {
            lock (_templatesFileLocker)
                return ProtoBuffBase.DeserializeList<TemplateModel>(ConstantVariable.GetTemplatesFile());
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
                    int indexOfAccountToUpdate = 0;
                    var accountDetailsList = GetAccountDetails();
                   
                    if (accountDetailsList != null)
                    {
                        indexOfAccountToUpdate = FindAccountIndex(accountDetailsList, accountModel.AccountBaseModel.AccountId);
                        accountDetailsList[indexOfAccountToUpdate] = accountModel;
                    }
                    else
                    {
                        accountDetailsList = new List<DominatorAccountModel>();
                        accountDetailsList.Add(accountModel);
                    }
                    bool result = ProtoBuffBase.SerializeList(accountDetailsList, ConstantVariable.GetIndexAccountFile());

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
                    List<T> accountDetailsList = GetAccountDetailsFor<T>();
                    int indexOfAccountToUpdate = FindAccountIndex(accountDetailsList,(accountModel as dynamic).AccountId);
                        
                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeList(accountDetailsList,
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


        public static bool UpdateAllAccounts(List<DominatorAccountModel> accountDetailsList)
        {
            return UpdateAllAccounts<DominatorAccountModel>(accountDetailsList);
        }


        // TODO: back compatibility to save old AccountModel. Have to be replaced with IList<DominatorAccountModel>
        public static bool UpdateAllAccounts<T>(List<T> accountDetailsList) where T : class
        {
            lock (_accountDetailsFileLocker)
            {
                try
                {
                    bool result = ProtoBuffBase.SerializeList(accountDetailsList,
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
        

        public static void UpdateCampaigns(List<CampaignDetails> campaignList)
        {
            lock (_campaignsFileLocker)
            {
                try
                {
                    ProtoBuffBase.SerializeList(campaignList, ConstantVariable.GetIndexCampaignFile());
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
                    ProtoBuffBase.SerializeList(templatesList, ConstantVariable.GetTemplatesFile());
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update campaigns error - " + ex.Message);
                }
            }
        }
        public static void SaveProxy<T>(T ProxyManagerModel) where T : class
        {
            ProtoBuffBase.AppendObject(ProxyManagerModel, ConstantVariable.GetOtherProxyFile());
        }
        public static List<T> GetProxyDetails<T>() where T : class
        {
            lock (_proxyFileLocker)
                return ProtoBuffBase.DeserializeList<T>(ConstantVariable.GetOtherProxyFile());
        }
        public static int FindProxyIndex<T>(List<T> proxy, string ProxyName)
        {
            return typeof(T) == typeof(ProxyManagerModel) ?
                proxy.FindIndex(a => (a as ProxyManagerModel).AccountProxy.ProxyName == ProxyName) :
                proxy.FindIndex(a => (a as dynamic).AccountProxy.ProxyName == ProxyName);
        }
        internal static bool UpdateProxy<T>(T proxy) where T : class
        {
            try
            {
                lock (_proxyFileLocker)
                {
                    List<T> proxyDetailsList = GetProxyDetails<T>();
                    int indexOfProxyToUpdate = FindProxyIndex(proxyDetailsList, (proxy as dynamic).AccountProxy.ProxyName);

                    if (indexOfProxyToUpdate == -1)
                        return false;

                    proxyDetailsList[indexOfProxyToUpdate] = proxy;

                    bool result = ProtoBuffBase.SerializeList(proxyDetailsList,
                                                                ConstantVariable.GetOtherProxyFile());

                    GlobusLogHelper.log.Trace($"Update Proxy - [{result}]");
                    return result;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update proxy details error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }
        public static bool UpdateAllProxy<T>(List<T> proxyDetailsList) where T : class
        {
            lock (_proxyFileLocker)
            {
                try
                {
                    bool result = ProtoBuffBase.SerializeList(proxyDetailsList, ConstantVariable.GetOtherProxyFile());

                    GlobusLogHelper.log.Debug("Proxy succesfully saved");

                    return result;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update All Proxy error - " + ex.Message);
                    ex.DebugLog();
                    return false;
                }
            }
        }
        public static void SavePosts<T>(T PostModel) where T : class
        {
            ProtoBuffBase.AppendObject(PostModel, ConstantVariable.GetOtherPostsFile());
        }
        public static List<T> GetPostDetails<T>() where T : class
        {
            lock (_postFileLocker)
                return ProtoBuffBase.DeserializeList<T>(ConstantVariable.GetOtherPostsFile());
        }
        internal static bool UpdatePost<T>(T post) where T : class
        {
            try
            {
                lock (_postFileLocker)
                {
                    List<T> postDetailsList = GetPostDetails<T>();
                    int indexOfPostToUpdate = FindPostIndex(postDetailsList, (post as dynamic).CampaignDetails.CampaignName);

                    if (indexOfPostToUpdate == -1)
                        return false;

                    postDetailsList[indexOfPostToUpdate] = post;

                    bool result = ProtoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());

                    GlobusLogHelper.log.Trace($"Update Posts - [{result}]");
                    return result;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error("Update Posts details error - " + ex.Message);
                ex.DebugLog();
                return false;
            }
        }
        public static bool UpdateAllPosts<T>(List<T> postDetailsList) where T : class
        {
            lock (_proxyFileLocker)
            {
                try
                {
                    bool result = ProtoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());

                    GlobusLogHelper.log.Debug("Posts succesfully saved");

                    return result;
                }
                catch (Exception ex)
                {
                    GlobusLogHelper.log.Error("Update All Posts error - " + ex.Message);
                    ex.DebugLog();
                    return false;
                }
            }
        }
        public static int FindPostIndex<T>(List<T> posts, string CampaignName)
        {
            return typeof(T) == typeof(AddPostModel) ?
                posts.FindIndex(a => (a as AddPostModel).CampaignDetails.CampaignName == CampaignName) :
                posts.FindIndex(a => (a as dynamic).AccountProxy.ProxyName == CampaignName);
        }
        public static void SaveConfig<T>(T Config) where T : class
        {
            ProtoBuffBase.AppendObject(Config, ConstantVariable.GetOtherConfigFile());
        }
        public static List<T> GetConfigDetails<T>() where T : class
        {
            try
            {
                lock (_configFileLocker)
                    return ProtoBuffBase.DeserializeList<T>(ConstantVariable.GetOtherConfigFile());
            }
            catch (Exception ex)
            {


            }
            return null;
        }
    }
}
