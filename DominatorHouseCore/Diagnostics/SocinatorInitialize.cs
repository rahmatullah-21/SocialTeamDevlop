using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using MahApps.Metro.Converters;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {

        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, SocialNetworkObjects> RegisteredNetworks { get;  } = new Dictionary<SocialNetworks, SocialNetworkObjects>();

        public static SocialNetworkObjects ActiveNetwork { get; private set; }

        public static SocialNetworks ActiveSocialNetwork 
            => ActiveNetwork.Network;

        public static SocialNetworkObjects GetSocialLibrary(SocialNetworks networks)  
            => RegisteredNetworks[networks];
     
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

        public static void SocialNetworkRegister(SocialNetworkObjects activeNetwork, SocialNetworks network)
        {
            if (RegisteredNetworks.ContainsKey(network))
                return;

            RegisteredNetworks.Add(network, activeNetwork);
        }

        public static void SocialNetworkRegister(List<SocialNetworkObjects> networkObjects)
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

    public class SocialNetworkObjects
    {
        /// <summary>
        /// Specify the network of the dominator
        /// </summary>
        public SocialNetworks Network { get;  set; }

        /// <summary>
        /// creates job process based on social network and module
        /// </summary>
        public IJobProcessFactory JobProcessFactory { get;  set; }

        /// <summary>
        /// Scraps data from social network feed based on query (queries)
        /// </summary>
        public IScraperFactory QueryScraperFactory { get;  set; }

    }

}
