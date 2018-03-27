using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using MahApps.Metro.Converters;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {

        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, NetworkCoreLibrary> RegisteredNetworks { get; } = new Dictionary<SocialNetworks, NetworkCoreLibrary>();

        public static NetworkCoreLibrary ActiveNetwork { get; private set; }

        public static SocialNetworks ActiveSocialNetwork
            => ActiveNetwork.Network;

        public static NetworkCoreLibrary GetSocialLibrary(SocialNetworks networks)
        {
            return RegisteredNetworks.Count != 0 ? RegisteredNetworks[networks] : null;
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
                ActiveNetwork = activenetwork;
            else
            {
                // TODO : update the license and recheck register dominator
                // TODO : Auto restart feature
            }
        }

        public static void SocialNetworkRegister(NetworkCoreLibrary activeNetwork)
        {
            if (RegisteredNetworks.ContainsKey(activeNetwork.Network))
                return;

            RegisteredNetworks.Add(activeNetwork.Network, activeNetwork);
        }

        public static void SocialNetworkRegister(List<NetworkCoreLibrary> networkObjects)
        {
            foreach (var socialNetworkObjects in networkObjects)
            {
                try
                {
                    if (RegisteredNetworks.ContainsKey(socialNetworkObjects.Network))
                        continue;

                    RegisteredNetworks.Add(socialNetworkObjects.Network, socialNetworkObjects);
                }
                catch (Exception e)
                {
                    e.DebugLog();
                }
            }
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

    public class NetworkCoreLibrary
    {
        /// <summary>
        /// Specify the network of the dominator
        /// </summary>
        public SocialNetworks Network { get; set; }

        /// <summary>
        /// creates job process based on social network and module
        /// </summary>
        public IJobProcessFactory JobProcessFactory { get; set; }

        /// <summary>
        /// Scraps data from social network feed based on query (queries)
        /// </summary>
        public IScraperFactory QueryScraperFactory { get; set; }

        public ITabHandlerFactory TabHandlerFactory { get; set; }

        public IAccountUpdateFactory AccountUpdateFactory { get; set; }

        public IAccountCountFactory AccountCountFactory { get; set; }

        public IAccountToolsFactory AccountUserControlTools { get; set; }
       
    }


    public class NetworkCoreLibraryBuilder
    {

        public NetworkCoreLibrary NetworkCoreLibrary { get; set; }

        public NetworkCoreLibraryBuilder()
        {
            NetworkCoreLibrary = new NetworkCoreLibrary();
        }

        public NetworkCoreLibraryBuilder AddNetwork(SocialNetworks networks)
        {
            NetworkCoreLibrary.Network = networks;
            return this;
        }

        public NetworkCoreLibraryBuilder AddJobFactory(IJobProcessFactory jobProcessFactory)
        {
            NetworkCoreLibrary.JobProcessFactory = jobProcessFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddScraperFactory(IScraperFactory scraperFactory)
        {
            NetworkCoreLibrary.QueryScraperFactory = scraperFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddTabFactory(ITabHandlerFactory tabFactory)
        {
            NetworkCoreLibrary.TabHandlerFactory = tabFactory;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountFactory(IAccountUpdateFactory accountUpdate)
        {
            NetworkCoreLibrary.AccountUpdateFactory = accountUpdate;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountCounts(IAccountCountFactory accountCount)
        {
            NetworkCoreLibrary.AccountCountFactory = accountCount;
            return this;
        }

        public NetworkCoreLibraryBuilder AddAccountUiTools(IAccountToolsFactory accountUserControl)
        {
            NetworkCoreLibrary.AccountUserControlTools = accountUserControl;
            return this;
        }

    }
}
