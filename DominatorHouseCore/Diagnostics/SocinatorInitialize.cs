using System;
using System.Linq;
using System.Collections.Generic;
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
using DominatorHouseCore.Request;
using ProtectedCommon;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {
        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, INetworkCollectionFactory> RegisteredNetworks { get; } =
            new Dictionary<SocialNetworks, INetworkCollectionFactory>();

        public static HashSet<SocialNetworks> AvailableNetworks { get; set; } = new HashSet<SocialNetworks>();

        public static async Task<HashSet<SocialNetworks>> GetAvailableSocialNetworks(string license)
        {
            try
            {

                #region Error

                // Get all available networks from license  
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

                FeatureFlags.Instance = new FeatureFlags() {
                    {"SocinatorInitializer", true },
                    {"Twitter", true },
                    {"Social", true},
                    {"Instagram",true },
                    {"Gplus",true },
                    {"LinkedIn",true },
                    {"Quora",true },
                    {"Facebook",true },
                    {"Youtube",true },
                    {"Reddit",true },
                    {"Tumblr",true },
                    {"Pinterest",true}
                };

                return AvailableNetworks;

                #endregion

                if (string.IsNullOrEmpty(license))
                {                   
                    FeatureFlags.Instance = new FeatureFlags()
                    {
                        {"SocinatorInitializer", true}
                    };
                    return AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }
                    
                var macId = GetMacId();

                var url =
                    $"https://socinator.com/amember/softsale/api/activate?key={license}&request[hardware-id]={macId}";

                string finalResponse;
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                var licenseresponse = (HttpWebResponse)await request.GetResponseAsync();

                var responseStream = licenseresponse.GetResponseStream();

                if (responseStream == null)
                {
                    FeatureFlags.Instance = new FeatureFlags()
                    {
                        {"SocinatorInitializer", true}
                    };
                    return AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }
                  
                using (var streamReader = new StreamReader(responseStream))
                {
                    finalResponse = streamReader.ReadToEnd();
                }

                if (finalResponse.Contains("License Key not found"))
                {
                    FeatureFlags.Instance = new FeatureFlags()
                    {
                        {"SocinatorInitializer", true}
                    };
                    return AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }

                if (finalResponse.Contains("Ok"))
                {
                    FeatureFlags.Instance = new FeatureFlags()
                    {
                        {"SocinatorInitializer", true}
                    };
                    return AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }

                if (finalResponse.Contains("Sorry"))
                {
                    // Get all available networks from license  
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

                    FeatureFlags.Instance = new FeatureFlags() {
                        {"SocinatorInitializer", true },
                        {"Twitter", true },
                        {"Social", true},
                        {"Instagram",true },
                        {"Gplus",true },
                        {"LinkedIn",true },
                        {"Quora",true },
                        {"Facebook",true },
                        {"Youtube",true },
                        {"Reddit",true },
                        {"Tumblr",true },
                        {"Pinterest",true}
                    };
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return AvailableNetworks;
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
            {SocialNetworks.Social,"DominatorHouse" },
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
    }
}