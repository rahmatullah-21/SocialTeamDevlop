using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.IO;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.ViewModel;


namespace DominatorHouseCore.Utility
{
    public class BinFileHelper
    {
        //private static readonly object _accountDetailsFileLocker = new object();
        //private static readonly object _campaignsFileLocker = new object();
        //private static readonly object _templatesFileLocker = new object();
        //private static readonly object _proxyFileLocker = new object();
        //private static readonly object _postFileLocker = new object();
        //private static readonly object _configFileLocker = new object();

        public static ObservableCollection<string> GetUsers()
            => new ObservableCollection<string>(GetAccountDetails().Select(x => x.AccountBaseModel.UserName).ToList());

        public static ObservableCollection<string> GetUsers(SocialNetworks networks)
            => new ObservableCollection<string>(GetAccountDetails()
                .Where(x => x.AccountBaseModel.AccountNetwork == networks).Select(x => x.AccountBaseModel.UserName)
                .ToList());

        public static ObservableCollection<string> GetUsers<T>() where T : class
            => new ObservableCollection<string>(GetAccountDetailsFor<T>().Select(x => (x as dynamic).UserName as string)
                .ToList());

        private static Dictionary<Type, Tuple<object, Func<string>>> __lockAndFileByType =
            new Dictionary<Type, Tuple<object, Func<string>>>
            {
                {
                    typeof(CampaignDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexCampaignFile)
                },
                {typeof(TemplateModel), Tuple.Create(new object(), (Func<string>) ConstantVariable.GetTemplatesFile)},
                {
                    typeof(ProxyManagerModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherProxyFile)
                },
                {typeof(AddPostModel), Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherPostsFile)},
                {typeof(Configuration), Tuple.Create(new object(), (Func<string>) ConstantVariable.GetOtherConfigFile)},

                //Todo: Following line need to delete
                {
                    typeof(PublisherAccountDetails),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherFile)
                },

                {
                    typeof(PublisherPostlistModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreatePostlistFolder)
                },
                
                {
                    typeof(PublisherManageDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherDestinationsFile)
                },
                {
                    typeof(PublisherCreateDestinationModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherCreateDestinationsFolder)
                },
                {
                    typeof(PublisherPostlistSettingsModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetPublisherPostlistSettingsFile)
                },
                {
                    typeof(CampaignInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {
                    typeof(GlobalInteractionViewModel),
                    Tuple.Create(new object(), (Func<string>) ConstantVariable.GetConfigurationDir)
                },
                {typeof(object), Tuple.Create(new object(), (Func<string>) ConstantVariable.GetIndexAccountFile)}
            };

        /// <summary>
        /// Do something while locking the file that is the repository for the corresponding class
        /// </summary>
        /// <typeparam name="T">subject</typeparam>
        /// <typeparam name="R">return type</typeparam>
        /// <param name="act">action to perform</param>
        /// <returns>repeats the action returned value</returns>
        static R WithFile<T, R>(Func<string, R> act)
        {
            Tuple<object, Func<string>> typeConfig;
            // first, try the actual type
            if (!__lockAndFileByType.TryGetValue(typeof(T), out typeConfig))
            {
                // second, try to see if it's an assignable type
                var presentBaseClass = __lockAndFileByType.Keys.Except(new Type[] {typeof(object)}).FirstOrDefault(
                    candidateBase => candidateBase.IsAssignableFrom(typeof(T)));
                if (presentBaseClass == default(Type))
                {
                    presentBaseClass = typeof(object);
                }
                typeConfig = __lockAndFileByType[presentBaseClass];
            }
            lock (typeConfig.Item1)
            {
                return act(typeConfig.Item2());
            }
        }

        public static bool Append<T>(T obj)
        {
            try
            {
                return WithFile<T, bool>(filePath =>
                {
                    ProtoBuffBase.AppendObject<T>(obj, filePath);
                    return true;
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }


        public static List<DominatorAccountModel> GetAccountDetails()
        {
            return WithFile<DominatorAccountModel, List<DominatorAccountModel>>(indexAccountPath =>
                File.Exists(indexAccountPath)
                    ? ProtoBuffBase.DeserializeList<DominatorAccountModel>(indexAccountPath)
                    : new List<DominatorAccountModel>());
        }




        // TODO: back compatibility for account models of PD, TWD etc.
        // Modify index account path. Uses only for testing purposes of PD, TWD and others.
        public static List<T> GetAccountDetailsFor<T>() where T : class
        {
            return WithFile<T, List<T>>(file => ProtoBuffBase.DeserializeList<T>(file));
        }


        // Get all campigns 
        public static List<CampaignDetails> GetCampaignDetail()
        {
            return WithFile<CampaignDetails, List<CampaignDetails>>(file =>
                ProtoBuffBase.DeserializeList<CampaignDetails>(file));
        }


        // Get all templates 
        public static List<TemplateModel> GetTemplateDetails()
        {
            return WithFile<TemplateModel, List<TemplateModel>>(file =>
                ProtoBuffBase.DeserializeList<TemplateModel>(file));
        }

        public static int FindAccountIndex(List<DominatorAccountModel> accounts, string id)
        {
            return accounts.FindIndex(candidate => candidate.AccountId == id);
        }

        [Obsolete("This method is not safe")]
        public static int FindAccountIndex<T>(List<T> accounts, string id)
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
        public static bool UpdateAccount(DominatorAccountModel accountModel)
        {
            try
            {
                return WithFile<DominatorAccountModel, bool>(file =>
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
                    bool result = ProtoBuffBase.SerializeList(accountDetailsList, file);

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
        internal static bool UpdateAccount<T>(T accountModel) where T : class
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    List<T> accountDetailsList = GetAccountDetailsFor<T>();
                    int indexOfAccountToUpdate =
                        FindAccountIndex(accountDetailsList, (accountModel as dynamic).AccountId);

                    if (indexOfAccountToUpdate == -1)
                        return false;

                    accountDetailsList[indexOfAccountToUpdate] = accountModel;

                    bool result = ProtoBuffBase.SerializeList(accountDetailsList, file);

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


        public static bool UpdateAllAccounts(List<DominatorAccountModel> accountDetailsList)
        {
            return UpdateAllAccounts<DominatorAccountModel>(accountDetailsList);
        }


        // TODO: back compatibility to save old AccountModel. Have to be replaced with IList<DominatorAccountModel>
        public static bool UpdateAllAccounts<T>(List<T> accountDetailsList) where T : class
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(accountDetailsList, file);
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


        public static void UpdateCampaigns(List<CampaignDetails> campaignList)
        {
            try
            {
                WithFile<CampaignDetails, bool>(file =>
                    ProtoBuffBase.SerializeList(campaignList, file));
                GlobusLogHelper.log.Debug("Campaigns succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void UpdateTemplates(List<TemplateModel> templatesList)
        {
            try
            {
                WithFile<TemplateModel, bool>(file =>
                    ProtoBuffBase.SerializeList(templatesList, file));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void SaveProxy(ProxyManagerModel model)
        {
            WithFile<ProxyManagerModel, bool>(file =>
            {
                ProtoBuffBase.AppendObject(model, file);
                return true;
            });
        }

        public static List<ProxyManagerModel> GetProxyDetails()
        {
            return WithFile<ProxyManagerModel, List<ProxyManagerModel>>(file =>
                ProtoBuffBase.DeserializeList<ProxyManagerModel>(file));
        }

        public static int FindProxyIndex<T>(List<T> proxy, string ProxyId)
        {
            return typeof(T) == typeof(ProxyManagerModel)
                ? proxy.FindIndex(a => (a as ProxyManagerModel).AccountProxy.ProxyId == ProxyId)
                : proxy.FindIndex(a => (a as dynamic).AccountProxy.ProxyId == ProxyId);
        }

        internal static bool UpdateProxy(ProxyManagerModel proxy)
        {
            try
            {
                return WithFile<ProxyManagerModel, bool>(file =>
                {
                    var proxyDetailsList = GetProxyDetails();
                    var indexOfProxyToUpdate = FindProxyIndex(proxyDetailsList, proxy.AccountProxy.ProxyId);

                    if (indexOfProxyToUpdate == -1)
                        return false;

                    proxyDetailsList[indexOfProxyToUpdate] = proxy;

                    bool result = ProtoBuffBase.SerializeList(proxyDetailsList, file);

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

        public static bool UpdateAllProxy(List<ProxyManagerModel> proxyDetailsList)
        {
            try
            {
                return WithFile<ProxyManagerModel, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(proxyDetailsList, file);
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

        public static void SavePosts<T>(T PostModel) where T : class
        {
            ProtoBuffBase.AppendObject(PostModel, ConstantVariable.GetOtherPostsFile());
        }

        public static List<AddPostModel> GetPostDetails()
        {
            return WithFile<AddPostModel, List<AddPostModel>>(file =>
                ProtoBuffBase.DeserializeList<AddPostModel>(file));
        }

        internal static bool UpdatePost(AddPostModel post)
        {
            try
            {
                return WithFile<AddPostModel, bool>(file =>
                {
                    var postDetailsList = GetPostDetails();
                    int indexOfPostToUpdate = FindPostIndex(postDetailsList, post.CampaignDetails.CampaignName);

                    if (indexOfPostToUpdate == -1)
                        return false;

                    postDetailsList[indexOfPostToUpdate] = post;

                    bool result = ProtoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());

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

        public static bool UpdateAllPosts(List<AddPostModel> postDetailsList)
        {
            try
            {
                return WithFile<AddPostModel, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(postDetailsList, ConstantVariable.GetOtherPostsFile());
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

        public static int FindPostIndex<T>(List<T> posts, string CampaignName)
        {
            return typeof(T) == typeof(AddPostModel)
                ? posts.FindIndex(a => (a as AddPostModel).CampaignDetails.CampaignName == CampaignName)
                : posts.FindIndex(a => (a as dynamic).AccountProxy.ProxyName == CampaignName);
        }

        public static void SaveConfig(Configuration config)
        {
            WithFile<Configuration, bool>(file =>
            {
                ProtoBuffBase.AppendObject(config, file);
                return true;
            });
        }

        public static List<Configuration> GetConfigDetails<T>()
        {
            try
            {
                return WithFile<Configuration, List<Configuration>>(file =>
                {
                    if (File.Exists(file))
                    {
                        return ProtoBuffBase.DeserializeList<Configuration>(file);
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


        public static bool AddDestination(PublisherCreateDestinationModel publisherCreateDestination)
        {
            try
            {
                return WithFile<PublisherCreateDestinationModel, bool>(filePath =>
                {
                    DirectoryUtilities.CreateDirectory(filePath);
                    ProtoBuffBase.AppendObject(publisherCreateDestination,
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


        public static bool UpdateDestination(PublisherCreateDestinationModel publisherCreateDestination)
        {
            try
            {
                var data = ProtoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                    $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{publisherCreateDestination.DestinationId}.bin");

                if (data != null)
                {
                    data[0] = publisherCreateDestination;

                    bool result = ProtoBuffBase.SerializeList(data,
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


        public static List<PublisherCreateDestinationModel> GetDestination(string destinationId)
            => ProtoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{destinationId}.bin");


        public static PublisherCreateDestinationModel GetSingleDestination(string destinationId)
        {
            var lists= ProtoBuffBase.DeserializeList<PublisherCreateDestinationModel>(
                $"{ConstantVariable.GetPublisherCreateDestinationsFolder()}\\{destinationId}.bin");

            if (lists.Count > 0)
                return lists[0];

            return null;
        }

        public static void DeleteDestinationFile(string destinationId)
        {
            
        }


        public static List<PublisherManageDestinationModel> GetPublisherManageDestinationModels()
        {
            return WithFile<PublisherManageDestinationModel, List<PublisherManageDestinationModel>>(
                publisherDestinationPath => File.Exists(publisherDestinationPath)
                    ? ProtoBuffBase.DeserializeList<PublisherManageDestinationModel>(publisherDestinationPath)
                    : new List<PublisherManageDestinationModel>());
        }

        public static List<PublisherPostlistSettingsModel> GetPublisherPostListSettingsModels()
        {
            return WithFile<PublisherPostlistSettingsModel, List<PublisherPostlistSettingsModel>>(
                publisherPostListPath => File.Exists(publisherPostListPath)
                    ? ProtoBuffBase.DeserializeList<PublisherPostlistSettingsModel>(publisherPostListPath)
                    : new List<PublisherPostlistSettingsModel>());
        }


        public static List<PublisherPostlistModel> GetPublisherPostListModels(string campaignId)
        {
            return WithFile<PublisherPostlistModel, List<PublisherPostlistModel>>(
                publisherPostListPath => File.Exists($"{publisherPostListPath}\\{campaignId}.bin")
                    ? ProtoBuffBase.DeserializeList<PublisherPostlistModel>($"{publisherPostListPath}\\{campaignId}.bin")
                    : new List<PublisherPostlistModel>());
        }

        public static bool UpdateAllPostlists(string campaignId ,List<PublisherPostlistModel> publisherPostlist)
        {
            try
            {
                return WithFile<PublisherPostlistModel, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(publisherPostlist, $"{file}\\{campaignId}.bin");
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

        public static bool UpdateAllManageDestination(List<PublisherManageDestinationModel> publisherDestinationList)
        {
            return UpdateAllPublisherDestination(publisherDestinationList);
        }

        public static bool UpdateAllPublisherDestination<T>(List<T> publishDestinations) where T : class
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(publishDestinations, file);
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

        public static bool UpdateAllPostListSettings(List<PublisherPostlistSettingsModel> publisherDestinationList)
        {
            return Updates(publisherDestinationList);
        }

        public static bool Updates<T>(List<T> itemColletion) where T : class
        {
            try
            {
                return WithFile<T, bool>(file =>
                {
                    bool result = ProtoBuffBase.SerializeList(itemColletion, file);
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

        public static List<CampaignInteractionViewModel> GetCampaignInteractedDetails(SocialNetworks network)
        {
            return WithFile<CampaignInteractionViewModel, List<CampaignInteractionViewModel>>(file =>
                ProtoBuffBase.DeserializeList<CampaignInteractionViewModel>(file + $"\\{network}InteractedData.bin"));
        }


        public static void UpdateCampaignInteractedDetails(List<CampaignInteractionViewModel> campaignInteractedDatas,
            SocialNetworks network)
        {
            try
            {
                WithFile<CampaignInteractionViewModel, bool>(file =>
                    ProtoBuffBase.SerializeList(campaignInteractedDatas, file + $"\\{network}InteractedData.bin"));
                GlobusLogHelper.log.Debug("Campaigns interacted data's succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error, While update the datas to campaign interacted bin file");
            }
        }

        #endregion

        #region GlobalInteractedData

        public static List<GlobalInteractionViewModel> GetGlobalInteractedDetails(SocialNetworks network)
        {
            return WithFile<GlobalInteractionViewModel, List<GlobalInteractionViewModel>>(file =>
                ProtoBuffBase.DeserializeList<GlobalInteractionViewModel>(file + $"\\{network}InteractedData.bin"));
        }

        public static void UpdateGlobalInteractedDetails(List<GlobalInteractionViewModel> globalInteractedDatas,
            SocialNetworks network)
        {
            try
            {
                WithFile<GlobalInteractionViewModel, bool>(file =>
                    ProtoBuffBase.SerializeList(globalInteractedDatas, file + $"\\{network}InteractedData.bin"));
                GlobusLogHelper.log.Debug("Global interacted data's succesfully saved");
            }
            catch (Exception ex)
            {
                ex.DebugLog("Error, While update the datas to global interacted bin file");
            }
        } 

        #endregion

    }

}
