using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net;
using System.Windows;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProtectedCommon;
using DominatorHouseCore.FileManagers;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {
        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, INetworkCollectionFactory> RegisteredNetworks { get; } =
            new Dictionary<SocialNetworks, INetworkCollectionFactory>();

        public static HashSet<SocialNetworks> AvailableNetworks { get; set; } = new HashSet<SocialNetworks>();

        public static async Task<HashSet<SocialNetworks>> SetAvailableSocialNetworks(string license)
        {
            try
            {
                if (string.IsNullOrEmpty(license))
                {
                    FeatureFlags.Instance = new FeatureFlags()
                    {
                        {"SocinatorInitializer", true}
                    };
                    return AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }

                var macId = GetMacId();

                string finalResponse;
                var responseStream = await CheckLicenseActivation(license, macId);
                using (var streamReader = new StreamReader(responseStream))
                {
                    finalResponse = streamReader.ReadToEnd();
                }

                await SetAllLicensedSocialNetworks(JObject.Parse(finalResponse)["code"].ToString(), license, macId);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                if (ex.Message == "The remote name could not be resolved: 'socinator.com'")
                {
                    var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                          "Network Error",
                          "Please check your Internet connection and try again !!"
                     , MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Try Again", "Cancel"));
                    if (dialogResult == MessageDialogResult.Affirmative)
                    {
                        return null;
                    }
                }

            }
            return AvailableNetworks;
        }

        public static HashSet<SocialNetworks> SetLicensedSocialNetworks()
        {
            FeatureFlags.Instance = new FeatureFlags()
            {
                {"SocinatorInitializer", true}
            };
            AvailableNetworks.Add(SocialNetworks.Social);
            AvailableNetworks.Add(SocialNetworks.Twitter);
            AvailableNetworks.Add(SocialNetworks.Facebook);
            AvailableNetworks.Add(SocialNetworks.Gplus);
            AvailableNetworks.Add(SocialNetworks.Instagram);
            AvailableNetworks.Add(SocialNetworks.LinkedIn);
            AvailableNetworks.Add(SocialNetworks.Quora);
            AvailableNetworks.Add(SocialNetworks.Pinterest);
            AvailableNetworks.Add(SocialNetworks.Tumblr);
            AvailableNetworks.Add(SocialNetworks.Youtube);
            AvailableNetworks.Add(SocialNetworks.Reddit);

            FeatureFlags.Instance = new FeatureFlags()
            {
                {"SocinatorInitializer", true},
                {"Twitter", true},
                {"Social", true},
                {"Instagram", true},
                {"Gplus", true},
                {"LinkedIn", true},
                {"Quora", true},
                {"Facebook", true},
                {"Youtube", true},
                {"Reddit", true},
                {"Tumblr", true},
                {"Pinterest", true}
            };
            return AvailableNetworks;
        }

        private static async Task<HashSet<SocialNetworks>> SetAllLicensedSocialNetworks(string code, string license, string macId)
        {
            string message = "Oops something went wrong";

            try
            {
                if (license == "Beta_BHWLaunch")
                {
                    return SetLicensedSocialNetworks();
                }
                else
                {
                    switch (code)
                    {
                        case "no_activation_found":
                            string finalResponse;
                            using (var streamReader = new StreamReader(await ActivateLicense(license, macId)))
                            {
                                finalResponse = streamReader.ReadToEnd();
                            }
                            return await SetAllLicensedSocialNetworks(JObject.Parse(finalResponse)["code"].ToString(), license, macId);

                        case "license_empty":
                            message = "Empty or invalid license key submitted, Please check your license key and enter again.";
                            break;
                        case "license_not_found":
                            message =
                                "Oops, we are unable to find key you have entered, please recheck once at your end or contact support.";
                            break;
                        case "license_disabled":
                            message = "Your License key has been disabled, please contact support for more information.";
                            break;
                        case "license_expired":
                            message = "Your license key has got expired, please renew your subscription or contact support";
                            break;
                        case "invalid_input":
                            message = "Your entered license key is invalid, please check your license key and enter again.";
                            break;
                        case "no_spare_activations":
                            message =
                                "You have already reached the maximum allowed activations for this license key, please buy more license or deactivate your previous activation.";
                            //var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                            //      "License Error",
                            //      "You have already reached the maximum allowed activations for this license key, please buy more license or deactivate your previous activation."
                            // , MessageDialogStyle.AffirmativeAndNegative, new MetroDialogSettings()
                            // {
                            //     AffirmativeButtonText = "Deactivate Previous Activation"
                            // });
                            //if (dialogResult == MessageDialogResult.Affirmative)
                            //{
                            //    using (var streamReader = new StreamReader( await DeActivateLicense(license, macId)))
                            //    {
                            //        finalResponse = streamReader.ReadToEnd();
                            //    }
                            //    if(JObject.Parse(finalResponse)["code"].ToString()=="ok")
                            //    {
                            //        DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                            //            "License", "Sucessfully Deactivated.");
                            //        var licence =
                            //            DialogCoordinator.Instance.ShowModalInputExternal(Application.Current.MainWindow,
                            //                "Socinator", "License");
                            //        return await SetAvailableSocialNetworks(license);
                            //    }
                            //}
                            break;
                        case "no_reactivation_allowed":
                            message = "Sorry, but we are unable to reactivate your license key, please contact support.";
                            break;
                        case "ok":
                            return SetLicensedSocialNetworks();
                        case "other_error":
                            message = "Sorry, some unknown error occured, please contact support.";
                            break;
                    }
                    Dialog.ShowDialog("License Error", message);
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            return new HashSet<SocialNetworks>();
        }
        private static async Task<Stream> CheckLicenseActivation(string license, string macId)
        {
            var url = $"https://socinator.com/amember/softsale/api/check-activation?key={license}&request[hardware-id]={macId}";
            return await HttpHelper.GetResponseStreamAsync(url);
        }

        private static async Task<Stream> ActivateLicense(string license, string macId)
        {
            var url = $"https://socinator.com/amember/softsale/api/activate?key={license}&request[hardware-id]={macId}";
            return await HttpHelper.GetResponseStreamAsync(url);
        }
        private static async Task<Stream> DeActivateLicense(string license, string macId)
        {
            var url = $"https://socinator.com/amember/softsale/api/deactivate?key={license}&request[hardware-id]={macId}";
            return await HttpHelper.GetResponseStreamAsync(url);
        }
        public static string GetMacId()
        {
            var macAddresses = string.Empty;
            try
            {
                foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (nic.OperationalStatus != OperationalStatus.Up)
                        continue;

                    if (!string.IsNullOrEmpty(macAddresses))
                        break;

                    macAddresses += nic.GetPhysicalAddress().ToString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return macAddresses;
        }

        private static Dictionary<SocialNetworks, string> NetworksNamespace { get; set; } = new Dictionary<SocialNetworks, string>()
        {
            {SocialNetworks.Social,"Socinator" },
            {SocialNetworks.Facebook,"FaceDominatorUI"},
            {SocialNetworks.Twitter,"TwtDominatorUI"},
            { SocialNetworks.Gplus,"GplusDominatorUI"},
            {SocialNetworks.Instagram,"GramDominatorUI"},
            {SocialNetworks.LinkedIn,"LinkedDominatorUI"},
            {SocialNetworks.Quora,"QuoraDominatorUI"},
            {SocialNetworks.Pinterest,"PinDominator"},
            {SocialNetworks.Tumblr,"TumblrDominatorUI"},
            {SocialNetworks.Youtube,"YoutubeDominatorUI"},
            {SocialNetworks.Reddit,"RedditDominatorUI"},
        };

        public static string GetNetworksNamespace(SocialNetworks networks)
        {
            var networksNamespace = NetworksNamespace[networks];

            return networksNamespace;
        }

        public static INetworkCollectionFactory ActiveNetwork { get; private set; }

        public static SocialNetworks ActiveSocialNetwork => GetActiveSocialNetwork();

        private static SocialNetworks GetActiveSocialNetwork()
        {
            try
            {
                return ActiveNetwork.GetNetworkCoreFactory().Network;
            }
            catch (Exception)
            {
                return SocialNetworks.Social;
            }
        }

        public static INetworkCollectionFactory GetSocialLibrary(SocialNetworks networks)
        {
            return RegisteredNetworks.ContainsKey(networks) ? RegisteredNetworks[networks] : null;
        }

        public static void LogInitializer(Window mainWindow)
        {
            GlobalExceptionInitializer();

            var window = mainWindow as ILoggableWindow;

            if (window != null)
                GlobusLogHelper.InitializeLoggerUI(window);
        }

        public static void SetAsActiveNetwork(SocialNetworks networks)
        {
            var activenetwork = RegisteredNetworks[networks];

            if (activenetwork != null)
            {
                ActiveNetwork = activenetwork;
            }
        }

        public static void SocialNetworkRegister(INetworkCollectionFactory networkCollectionFactory, SocialNetworks networks)
        {
            if (RegisteredNetworks.ContainsKey(networks))
                return;

            RegisteredNetworks.Add(networks, networkCollectionFactory);
        }


        private static void GlobalExceptionInitializer()
        {
            if (_isInitialized)
                return;

            GlobusExceptionHandler.SetupGlobalExceptionHandlers();
            GlobusExceptionHandler.DisableErrorDialog();
            _isInitialized = true;
        }

        public static IGlobalDatabaseConnection GetGlobalDatabase()
        {
            return new GlobalDatabaseConnection();
        }

    }



    public class SocinatorAccountBuilder
    {
        private DominatorAccountModel DominatorAccountModel { get; set; }

        public SocinatorAccountBuilder(string accountId)
        {
            var account = AccountsFileManager.GetAccountById(accountId);
            DominatorAccountModel = account;
        }

        public SocinatorAccountBuilder AddOrUpdateModuleSettings(ActivityType activityType,
            ModuleConfiguration moduleConfiguration)
        {
            var moduleSettings = DominatorAccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);

            if (moduleSettings == null)
                DominatorAccountModel.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
            else
            {
                try
                {
                    if(!moduleConfiguration.IsEnabled)
                    DominatorScheduler.StopActivity(DominatorAccountModel.AccountBaseModel.AccountId,
                               activityType.ToString(), moduleSettings.TemplateId);

                    DominatorAccountModel.ActivityManager.LstModuleConfiguration.Remove(moduleSettings);
                    DominatorAccountModel.ActivityManager.LstModuleConfiguration.Add(moduleConfiguration);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            return this;
        }


        public SocinatorAccountBuilder RemoveModuleSettings(ActivityType activityType)
        {
            var moduleSettings = DominatorAccountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);

            if (moduleSettings != null)
                DominatorAccountModel.ActivityManager.LstModuleConfiguration.Remove(moduleSettings);
            
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateCookies(CookieCollection cookies)
        {
            DominatorAccountModel.Cookies = cookies;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDominatorAccountBase(DominatorAccountBaseModel accountBaseModel)
        {
            DominatorAccountModel.AccountBaseModel = accountBaseModel;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateLoginStatus(bool status)
        {
            DominatorAccountModel.IsUserLoggedIn = status;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateUserAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentWeb = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMobileAgentWeb(string webAgent)
        {
            DominatorAccountModel.UserAgentMobile = webAgent;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateMobileRequests(bool isUseOnlyMobileRequest)
        {
            DominatorAccountModel.UseMobileRequestOnly = isUseOnlyMobileRequest;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(Dictionary<string, string> extraProperity)
        {
            DominatorAccountModel.ExtraParameters = extraProperity;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateExtraParameter(string key, string value)
        {
            DominatorAccountModel.ExtraParameters.Add(key, value);
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn1(int? value)
        {
            DominatorAccountModel.DisplayColumnValue1 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn2(int? value)
        {
            DominatorAccountModel.DisplayColumnValue2 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn3(int? value)
        {
            DominatorAccountModel.DisplayColumnValue3 = value;
            return this;
        }

        public SocinatorAccountBuilder AddOrUpdateDisplayColumn4(int? value)
        {
            DominatorAccountModel.DisplayColumnValue4 = value;
            return this;
        }

        public bool SaveToBinFile()
         => AccountsFileManager.Edit(DominatorAccountModel);
    }


    public class NetworkCoreLibraryBuilder
    {
        public NetworkCoreLibraryBuilder()
        {
        }
        public NetworkCoreLibraryBuilder(INetworkCoreFactory networkCoreFactory)
        {
            NetworkCoreFactory = networkCoreFactory;
        }

        public INetworkCoreFactory NetworkCoreFactory { get; set; }

        public NetworkCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            NetworkCoreFactory.Network = networks;
            return this;
        }

        public NetworkCoreLibraryBuilder AddJobFactory(IJobProcessFactory jobProcessFactory)
        {
            NetworkCoreFactory.JobProcessFactory = jobProcessFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddScraperFactory(IQueryScraperFactory scraperFactory)
        {
            NetworkCoreFactory.QueryScraperFactory = scraperFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddTabFactory(ITabHandlerFactory tabFactory)
        {
            NetworkCoreFactory.TabHandlerFactory = tabFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountFactory(IAccountUpdateFactory accountUpdate)
        {
            NetworkCoreFactory.AccountUpdateFactory = accountUpdate;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountCounts(IAccountCountFactory accountCount)
        {
            NetworkCoreFactory.AccountCountFactory = accountCount;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountUiTools(IAccountToolsFactory accountUserControl)
        {
            NetworkCoreFactory.AccountUserControlTools = accountUserControl;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountDbConnection(IDatabaseConnection accountDbConnection)
        {
            NetworkCoreFactory.AccountDatabase = accountDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddCampaignDbConnection(IDatabaseConnection campaignDbConnection)
        {
            NetworkCoreFactory.CampaignDatabase = campaignDbConnection;
            return this;
        }

        public NetworkCoreLibraryBuilder AddReportFactory(IReportFactory reportFactory)
        {
            NetworkCoreFactory.ReportFactory = reportFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddViewCampaignFactory(IViewCampaignsFactory viewCampaigns)
        {
            NetworkCoreFactory.ViewCampaigns = viewCampaigns;
            return this;
        }
        public NetworkCoreLibraryBuilder AddCampaignInteractedDetailsFactory(ICampaignInteractionDetails campaignInteractionDetails)
        {
            NetworkCoreFactory.CampaignInteractionDetails = campaignInteractionDetails;
            return this;
        }


    }
}