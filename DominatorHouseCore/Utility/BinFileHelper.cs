using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;


namespace DominatorHouseCore.Utility
{
    public interface IBinFileHelper
    {
        List<CampaignDetails> GetCampaignDetail();
        void UpdateCampaigns(List<CampaignDetails> campaignList);
        bool Append<T>(T obj);
        bool AddDestination(PublisherCreateDestinationModel publisherCreateDestination);
        bool UpdateDestination(PublisherCreateDestinationModel publisherCreateDestination);
        List<PublisherCreateDestinationModel> GetDestination(string destinationId);
        List<DominatorAccountModel> GetAccountDetails();
        bool UpdateAllAccounts<T>(List<T> accountDetailsList) where T : class;
        void SaveConfig(Configuration config);
        List<Configuration> GetConfigDetails<T>();
        void SavePosts<T>(T PostModel) where T : class;
        List<AddPostModel> GetPostDetails();
        bool UpdatePost(AddPostModel post);
        bool UpdateAllPosts(List<AddPostModel> postDetailsList);
        void SaveProxy(ProxyManagerModel model);
        List<ProxyManagerModel> GetProxyDetails();
        bool UpdateProxy(ProxyManagerModel proxy);
        bool UpdateAllProxy(List<ProxyManagerModel> proxyDetailsList);
        List<PublisherManageDestinationModel> GetPublisherManageDestinationModels();
        bool UpdateAllManageDestination(List<PublisherManageDestinationModel> publisherDestinationList);
        bool UpdateAllPostlists(string campaignId, List<PublisherPostlistModel> publisherPostlist);
        List<PublisherPostlistModel> GetPublisherPostListModels(string campaignId);
        bool UpdateAllPostListSettings(List<PublisherPostlistSettingsModel> publisherDestinationList);
        List<PublisherPostlistSettingsModel> GetPublisherPostListSettingsModels();
        PublisherCreateDestinationModel GetSingleDestination(string destinationId);
        List<TemplateModel> GetTemplateDetails();
        List<CampaignInteractionViewModel> GetCampaignInteractedDetails(SocialNetworks network);

        void UpdateCampaignInteractedDetails(List<CampaignInteractionViewModel> campaignInteractedDatas,
            SocialNetworks network);

        void UpdateGlobalInteractedDetails(List<GlobalInteractionViewModel> globalInteractedDatas,
            SocialNetworks network);

        List<GlobalInteractionViewModel> GetGlobalInteractedDetails(SocialNetworks network);
    }

    public class BinFileHelper : IBinFileHelper
    {
        private readonly ILockFileConfigProvider _lockFileConfigProvider;
        private readonly IProtoBuffBase _protoBuffBase;

        public BinFileHelper(ILockFileConfigProvider lockFileConfigProvider, IProtoBuffBase protoBuffBase)
        {
            _lockFileConfigProvider = lockFileConfigProvider;
            _protoBuffBase = protoBuffBase;
        }

        public ObservableCollection<string> GetUsers<T>() where T : class
            => new ObservableCollection<string>(GetAccountDetailsFor<T>().Select(x => (x as dynamic).UserName as string)
                .ToList());



        public bool Append<T>(T obj)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<T, bool>(filePath =>
                {
                    _protoBuffBase.AppendObject<T>(obj, filePath);
                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        public List<DominatorAccountModel> GetAccountDetails()
        {
            return _lockFileConfigProvider.WithFile<DominatorAccountModel, List<DominatorAccountModel>>(indexAccountPath =>
                File.Exists(indexAccountPath)
                    ? _protoBuffBase.DeserializeList<DominatorAccountModel>(indexAccountPath)
                    : new List<DominatorAccountModel>());
        }




        // TODO: back compatibility for account models of PD, TWD etc.
        // Modify index account path. Uses only for testing purposes of PD, TWD and others.
        public List<T> GetAccountDetailsFor<T>() where T : class
        {
            return _lockFileConfigProvider.WithFile<T, List<T>>(file => _protoBuffBase.DeserializeList<T>(file));
        }


        // Get all campigns 
        public List<CampaignDetails> GetCampaignDetail()
        {
            return _lockFileConfigProvider.WithFile<CampaignDetails, List<CampaignDetails>>(file =>
                _protoBuffBase.DeserializeList<CampaignDetails>(file));
        }


        // Get all templates 
        public List<TemplateModel> GetTemplateDetails()
        {
            return _lockFileConfigProvider.WithFile<TemplateModel, List<TemplateModel>>(file =>
                _protoBuffBase.DeserializeList<TemplateModel>(file));
        }

        public int FindAccountIndex(List<DominatorAccountModel> accounts, string id)
        {
            return accounts.FindIndex(candidate => candidate.AccountId == id);
        }

        [Obsolete("This method is not safe")]
        public int FindAccountIndex<T>(List<T> accounts, string id)
        {
            return typeof(T) == typeof(DominatorAccountModel)
                ? accounts.FindIndex(a => (a as DominatorAccountModel).AccountBaseModel.AccountId == id)
                : accounts.FindIndex(a => (a as dynamic).AccountId == id);
        }

        /// <summary>
        /// Overwrites AccountDetails.bin with updated account
        /// </summary>
        /// <param name="accountModel"></param>
        /// <returns></returns>
        public bool UpdateAccount(DominatorAccountModel accountModel)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<DominatorAccountModel, bool>(file =>
                {
                    int indexOfAccountToUpdate = 0;
                    var accountDetailsList = GetAccountDetails();

                    if (accountDetailsList != null)
                    {
                        indexOfAccountToUpdate =
                            FindAccountIndex(accountDetailsList, accountModel.AccountBaseModel.AccountId);
                        accountDetailsList[indexOfAccountToUpdate] = accountModel;
                    }
                    else
                    {
                        accountDetailsList = new List<DominatorAccountModel>();
                        accountDetailsList.Add(accountModel);
                    }
                    bool result = _protoBuffBase.SerializeList(accountDetailsList, file);

                    GlobusLogHelper.log.Trace($"Update Accounts - [{result}]");
                    return result;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        // TODO: backward compatibility
        [Obsolete("This code is unsafe")]
        internal bool UpdateAccount<T>(T accountModel) where T : class
        {
            try
            {
                return _lockFileConfigProvider.WithFile<T, bool>(file =>
                {
                    List<T> accountDetailsList = GetAccountDetailsFor<T>();
                    int indexOfAccountToUpdate =
                        FindAccountIndex(accountDetailsList, (accountModel as dynamic).AccountId);

                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = _protoBuffBase.SerializeList(accountDetailsList, file);

                    GlobusLogHelper.log.Trace($"Update Accounts - [{result}]");
                    return result;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        // TODO: back compatibility to save old AccountModel. Have to be replaced with IList<DominatorAccountModel>
        public bool UpdateAllAccounts<T>(List<T> accountDetailsList) where T : class
        {
            try
            {
                return _lockFileConfigProvider.WithFile<T, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(accountDetailsList, file);
                    GlobusLogHelper.log.Debug("Accounts succesfully saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }


        public void UpdateCampaigns(List<CampaignDetails> campaignList)
        {
            try
            {
                _lockFileConfigProvider.WithFile<CampaignDetails, bool>(file =>
                    _protoBuffBase.SerializeList(campaignList, file));
                GlobusLogHelper.log.Debug("Campaigns succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateTemplates(List<TemplateModel> templatesList)
        {
            try
            {
                _lockFileConfigProvider.WithFile<TemplateModel, bool>(file =>
                    _protoBuffBase.SerializeList(templatesList, file));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SaveProxy(ProxyManagerModel model)
        {
            _lockFileConfigProvider.WithFile<ProxyManagerModel, bool>(file =>
            {
                _protoBuffBase.AppendObject(model, file);
                return true;
            });
        }

        public List<ProxyManagerModel> GetProxyDetails()
        {
            return _lockFileConfigProvider.WithFile<ProxyManagerModel, List<ProxyManagerModel>>(file =>
                _protoBuffBase.DeserializeList<ProxyManagerModel>(file));
        }

        public int FindProxyIndex<T>(List<T> proxy, string ProxyId)
        {
            return typeof(T) == typeof(ProxyManagerModel)
                ? proxy.FindIndex(a => (a as ProxyManagerModel).AccountProxy.ProxyId == ProxyId)
                : proxy.FindIndex(a => (a as dynamic).AccountProxy.ProxyId == ProxyId);
        }

        public bool UpdateProxy(ProxyManagerModel proxy)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<ProxyManagerModel, bool>(file =>
                {
                    var proxyDetailsList = GetProxyDetails();
                    var indexOfProxyToUpdate = FindProxyIndex(proxyDetailsList, proxy.AccountProxy.ProxyId);

                    if (indexOfProxyToUpdate == -1)
                        return false;

                    proxyDetailsList[indexOfProxyToUpdate] = proxy;

                    bool result = _protoBuffBase.SerializeList(proxyDetailsList, file);

                    GlobusLogHelper.log.Trace($"Update Proxy - [{result}]");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public bool UpdateAllProxy(List<ProxyManagerModel> proxyDetailsList)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<ProxyManagerModel, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(proxyDetailsList, file);
                    GlobusLogHelper.log.Debug("Proxy succesfully saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public void SavePosts<T>(T PostModel) where T : class
        {
            _protoBuffBase.AppendObject(PostModel, ConstantVariable.GetOtherPostsFile());
        }

        public List<AddPostModel> GetPostDetails()
        {
            return _lockFileConfigProvider.WithFile<AddPostModel, List<AddPostModel>>(file =>
                _protoBuffBase.DeserializeList<AddPostModel>(file));
        }

        public bool UpdatePost(AddPostModel post)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<AddPostModel, bool>(file =>
                {
                    var postDetailsList = GetPostDetails();
                    int indexOfPostToUpdate = FindPostIndex(postDetailsList, post.CampaignDetails.CampaignName);

                    if (indexOfPostToUpdate == -1)
                        return false;

                    postDetailsList[indexOfPostToUpdate] = post;

                    bool result = _protoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());

                    GlobusLogHelper.log.Trace($"Update Posts - [{result}]");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public bool UpdateAllPosts(List<AddPostModel> postDetailsList)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<AddPostModel, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());
                    GlobusLogHelper.log.Debug("Posts succesfully saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public int FindPostIndex<T>(List<T> posts, string CampaignName)
        {
            return typeof(T) == typeof(AddPostModel)
                ? posts.FindIndex(a => (a as AddPostModel).CampaignDetails.CampaignName == CampaignName)
                : posts.FindIndex(a => (a as dynamic).AccountProxy.ProxyName == CampaignName);
        }

        public void SaveConfig(Configuration config)
        {
            _lockFileConfigProvider.WithFile<Configuration, bool>(file =>
            {
                _protoBuffBase.AppendObject(config, file);
                return true;
            });
        }

        public List<Configuration> GetConfigDetails<T>()
        {
            try
            {
                return _lockFileConfigProvider.WithFile<Configuration, List<Configuration>>(file =>
                {
                    if (File.Exists(file))
                    {
                        return _protoBuffBase.DeserializeList<Configuration>(file);
                    }
                    return new List<Configuration>();
                });
            }
            catch (Exception)
            {

            }
            return null;
        }


        #region Publisher


        public bool AddDestination(PublisherCreateDestinationModel publisherCreateDestination)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<PublisherCreateDestinationModel, bool>(filePath =>
                {
                    DirectoryUtilities.CreateDirectory(filePath);
                    _protoBuffBase.AppendObject(publisherCreateDestination,
                        filePath + $"{publisherCreateDestination.DestinationId}.bin");
                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        public bool UpdateDestination(PublisherCreateDestinationModel publisherCreateDestination)
        {
            try
            {
                var data = _protoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                    $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{publisherCreateDestination.DestinationId}.bin");

                if (data != null)
                {
                    data[0] = publisherCreateDestination;

                    bool result = _protoBuffBase.SerializeList(data,
                        $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{publisherCreateDestination.DestinationId}.bin");

                    GlobusLogHelper.log.Trace($"Update Destination - [{result}]");

                    return result;
                }
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
            return false;
        }


        public List<PublisherCreateDestinationModel> GetDestination(string destinationId)
            => _protoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{destinationId}.bin");


        public PublisherCreateDestinationModel GetSingleDestination(string destinationId)
        {
            var lists = _protoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{destinationId}.bin");

            if (lists.Count > 0)
                return lists[0];

            return null;
        }

        public void DeleteDestinationFile(string destinationId)
        {

        }


        public List<PublisherManageDestinationModel> GetPublisherManageDestinationModels()
        {
            return _lockFileConfigProvider.WithFile<PublisherManageDestinationModel, List<PublisherManageDestinationModel>>(
                publisherDestinationPath => File.Exists(publisherDestinationPath)
                    ? _protoBuffBase.DeserializeList<PublisherManageDestinationModel>(publisherDestinationPath)
                    : new List<PublisherManageDestinationModel>());
        }

        public List<PublisherPostlistSettingsModel> GetPublisherPostListSettingsModels()
        {
            return _lockFileConfigProvider.WithFile<PublisherPostlistSettingsModel, List<PublisherPostlistSettingsModel>>(
                publisherPostListPath => File.Exists(publisherPostListPath)
                    ? _protoBuffBase.DeserializeList<PublisherPostlistSettingsModel>(publisherPostListPath)
                    : new List<PublisherPostlistSettingsModel>());
        }


        public List<PublisherPostlistModel> GetPublisherPostListModels(string campaignId)
        {
            return _lockFileConfigProvider.WithFile<PublisherPostlistModel, List<PublisherPostlistModel>>(
                publisherPostListPath => File.Exists($"{publisherPostListPath}\\{campaignId}.bin")
                    ? _protoBuffBase.DeserializeList<PublisherPostlistModel>($"{publisherPostListPath}\\{campaignId}.bin")
                    : new List<PublisherPostlistModel>());
        }

        public bool UpdateAllPostlists(string campaignId, List<PublisherPostlistModel> publisherPostlist)
        {
            try
            {
                return _lockFileConfigProvider.WithFile<PublisherPostlistModel, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(publisherPostlist, $"{file}\\{campaignId}.bin");
                    GlobusLogHelper.log.Debug("Publisher Post list saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public bool UpdateAllManageDestination(List<PublisherManageDestinationModel> publisherDestinationList)
        {
            return UpdateAllPublisherDestination(publisherDestinationList);
        }

        public bool UpdateAllPublisherDestination<T>(List<T> publishDestinations) where T : class
        {
            try
            {
                return _lockFileConfigProvider.WithFile<T, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(publishDestinations, file);
                    GlobusLogHelper.log.Debug("Publisher destination saved");
                    return result;
                });
            }
            catch (Exception ex)
            {

                ex.DebugLog();
                return false;
            }
        }

        public bool UpdateAllPostListSettings(List<PublisherPostlistSettingsModel> publisherDestinationList)
        {
            return Updates(publisherDestinationList);
        }

        public bool Updates<T>(List<T> itemColletion) where T : class
        {
            try
            {
                return _lockFileConfigProvider.WithFile<T, bool>(file =>
                {
                    bool result = _protoBuffBase.SerializeList(itemColletion, file);
                    return result;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        #endregion

        #region CampaignInteractedData

        public List<CampaignInteractionViewModel> GetCampaignInteractedDetails(SocialNetworks network)
        {
            return _lockFileConfigProvider.WithFile<CampaignInteractionViewModel, List<CampaignInteractionViewModel>>(file =>
                _protoBuffBase.DeserializeList<CampaignInteractionViewModel>(file + $"\\{network}CampaignInteractedData.bin"));
        }


        public void UpdateCampaignInteractedDetails(List<CampaignInteractionViewModel> campaignInteractedDatas,
            SocialNetworks network)
        {
            try
            {
                _lockFileConfigProvider.WithFile<CampaignInteractionViewModel, bool>(file =>
                    _protoBuffBase.SerializeList(campaignInteractedDatas, file + $"\\{network}CampaignInteractedData.bin"));
                GlobusLogHelper.log.Debug("Campaigns interacted data's succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error, While update the datas to campaign interacted bin file");
            }
        }

        #endregion

        #region GlobalInteractedData

        public List<GlobalInteractionViewModel> GetGlobalInteractedDetails(SocialNetworks network)
        {
            return _lockFileConfigProvider.WithFile<GlobalInteractionViewModel, List<GlobalInteractionViewModel>>(file =>
                _protoBuffBase.DeserializeList<GlobalInteractionViewModel>(file + $"\\{network}InteractedData.bin"));
        }

        public void UpdateGlobalInteractedDetails(List<GlobalInteractionViewModel> globalInteractedDatas,
            SocialNetworks network)
        {
            try
            {
                _lockFileConfigProvider.WithFile<GlobalInteractionViewModel, bool>(file =>
                    _protoBuffBase.SerializeList(globalInteractedDatas, file + $"\\{network}InteractedData.bin"));
                GlobusLogHelper.log.Debug("Global interacted data's succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error, While update the datas to global interacted bin file");
            }
        }

        #endregion

        public void SaveFacebookEntity<T>(List<T> friendsModelList, string filePath) where T : class
        {
            _protoBuffBase.SerializeList(friendsModelList, filePath);
        }

        public List<T> GetFacebookEntity<T>() where T : class
        {
            return _protoBuffBase.DeserializeList<T>(ConstantVariable.GetFacebookDetailsConfigFile());
        }
    }

}
