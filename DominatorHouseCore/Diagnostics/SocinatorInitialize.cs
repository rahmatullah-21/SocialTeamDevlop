using System;
using System.Linq;
using System.Collections.Generic;
using System.Windows;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using System.Net.Http;
using ProtectedCommon;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {
        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, INetworkCollectionFactory> RegisteredNetworks { get; } =
            new Dictionary<SocialNetworks, INetworkCollectionFactory>();

        public static HashSet<SocialNetworks> AvailableNetworks { get; set; } = new HashSet<SocialNetworks>();

        public static HashSet<SocialNetworks> GetAvailableSocialNetworks(string license)
        {
            try
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
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return AvailableNetworks;
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
            catch (Exception e)
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