using DominatorHouse.Helpers;
using DominatorHouseCore.BusinessLogic;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using System;
using System.Collections.Generic;
using System.Windows;

namespace DominatorHouseCore.Diagnostics
{    
    /// <summary>
    /// Class wraps whole initialization of common Dominator objects and classes:
    /// Exception handling, logging, etc.
    /// 
    /// Use it in every particular application in main window
    /// </summary>
    public static class DominatorHouseInitializer
    {
        public class LibraryCoreObjects
        {
            /// <summary>
            /// Network ID
            /// </summary>
            public SocialNetworks Network { get;  set; }

            /// <summary>
            /// reates job process based on social network and module
            /// </summary>
            public IJobProcessFactory JobProcessFactory { get;  set; }       

            /// <summary>
            /// Scraps data from social network feed based on query (queries)
            /// </summary>
            public IScraperFactory QueryScraperFactory { get;  set; }

            /// <summary>
            /// Library main window
            /// </summary>
            public Window MainWindow { get; set; }
        }


        static bool _isInitialized = false;

        private static Dictionary<SocialNetworks, LibraryCoreObjects> _registeredLibraries = new Dictionary<SocialNetworks, LibraryCoreObjects>();

        public static LibraryCoreObjects ActiveLibrary { get; private set; }

        public static SocialNetworks ActiveSocialNetwork => ActiveLibrary.Network;

        // Platform sets only once on first initialization. May be DominatorHouseSocial for DH, or GramDominator for standalone exe.
        public static string PlatformName { get; private set; }
        

        /// <summary>
        /// Call this method in ctor of particular main window of library
        /// </summary>
        /// <param name="mainWindow"></param>
        public static void Init(Window mainWindow, 
                IJobProcessFactory jobProcessFactory,
                IScraperFactory queryScrapperFactory,
                Enums.SocialNetworks network)
        {
            if (_registeredLibraries.ContainsKey(network))
            {
                ActiveLibrary = _registeredLibraries[network];
                return;
            }

            // Save data of active library
            ActiveLibrary = new LibraryCoreObjects()
            {
                Network = network,
                MainWindow = mainWindow,
                JobProcessFactory = jobProcessFactory,
                QueryScraperFactory = queryScrapperFactory,
            };
            _registeredLibraries.Add(network, ActiveLibrary);


            // Do this initialization only once
            if (!_isInitialized)
            {
                // Save platform for paths
                PlatformName = PlatformNameFromEnum(network);

                // initialize global exception handler
                GlobusExceptionHandler.SetupGlobalExceptionHandlers();
                GlobusExceptionHandler.DisableErrorDialog();

                _isInitialized = true;
            }            

            // initialize logging to UI
            if (mainWindow is ILoggableWindow)
                GlobusLogHelper.InitializeLoggerUI((ILoggableWindow)mainWindow);
            
#if DEBUG && ATTACH_CONSOLE
            ConsoleManager.Show();
#endif            
        }
        

        public static string PlatformNameFromEnum(SocialNetworks network)
        {
            switch (network)
            {
                case SocialNetworks.Facebook:
                    return "FaceDominator";

                case SocialNetworks.Instagram:
                    return "GramDominator";

                case SocialNetworks.Twitter:
                    return "TwtDominator";

                case SocialNetworks.Pinterest:
                    return "PinDominator";

                case SocialNetworks.LinkedIn:
                    return "LinkedDominator";

                case SocialNetworks.Reddit:
                    return "RedditDominator";

                //case SocialNetworks.Craglist:
                //    return "CraglistDominator";

                //case SocialNetworks.Backpage:
                //    return "BackpageDominator";

                case SocialNetworks.Social:
                    return "DominatorHouseSocial";

                default:
                    throw new ArgumentException($"{nameof(network)} - unknown network");
            }
        }
    }
}
