using DominatorHouse.Helpers;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.LogHelper;
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
        static bool _isInitialized = false;

        /// <summary>
        /// Call this method in ctor of particular main window 
        /// </summary>
        /// <param name="mainWindow"></param>
        public static void Init(Window mainWindow)
        {
            if (_isInitialized) return;
            _isInitialized = true;

            // initialize global exception handler
            GlobusExceptionHandler.SetupGlobalExceptionHandlers();
            GlobusExceptionHandler.DisableErrorDialog();

            // initialize logging to UI
            if (mainWindow is ILoggableWindow)
                GlobusLogHelper.InitializeLoggerUI((ILoggableWindow)mainWindow);

            // init Job Process Factory for this library


#if DEBUG && ATTACH_CONSOLE
            ConsoleManager.Show();
#endif            
        }
    }
}
