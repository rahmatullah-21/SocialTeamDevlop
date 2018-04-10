using DominatorHouseCore.LogHelper;
using NLog;
using NLog.Common;
using NLog.Config;
using NLog.Targets;
using System;
using System.Windows;
using System.Windows.Controls;

namespace DominatorHouseCore.LogHelper
{
    public class GlobusLogHelper
    {
        static GlobusLogHelper()
        {
#if DEBUG
            // Setup the logging view for Sentinel - http://sentinel.codeplex.com
            var sentinalTarget = new NLogViewerTarget()
            {
                Name = "sentinal",
                Address = "udp://127.0.0.1:9999",
                IncludeNLogData = false
            };
            var sentinalRule = new LoggingRule("*", LogLevel.Trace, sentinalTarget);
            LogManager.Configuration.AddTarget("sentinal", sentinalTarget);
            LogManager.Configuration.LoggingRules.Add(sentinalRule);

            // Setup the logging view for Harvester - http://harvester.codeplex.com
            var harvesterTarget = new OutputDebugStringTarget()
            {
                Name = "harvester",
                Layout = "${log4jxmlevent:includeNLogData=false}"
            };
            var harvesterRule = new LoggingRule("*", LogLevel.Trace, harvesterTarget);
            LogManager.Configuration.AddTarget("harvester", harvesterTarget);
            LogManager.Configuration.LoggingRules.Add(harvesterRule);
#endif

            LogManager.ReconfigExistingLoggers();
            log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.ToString());
        }

        /// <summary>
        /// Singleton Instance
        /// </summary>
        public static ILogger log { get; }



        /// <summary>
        /// Private Constructer for Singleton Implementation
        /// </summary>
        private GlobusLogHelper()
        {
        }

        internal static void InitializeLoggerUI(ILoggableWindow window)
        {
            ((Window)window).Loaded += (s, e) => {
                var target = new NlogUiTarget("Activity Log", LogLevel.Info);
                target.Log += (log, error) => window.LogText(log, error);                
            };
        }

        public static void LogTextToList(ListBox list, string message)
        {
            list.Dispatcher.BeginInvoke(new Action(() =>
            {
                try
                {
                    if (list.Items.Count > 1000)
                        list.Items.RemoveAt(list.Items.Count - 1);

                   // list.Items.Insert(0, $"{DateTime.Now.ToString()}\tGram Dominator 3.0\t{message.Replace("\t", " ")}");

                    list.Items.Insert(0, $"{DateTime.Now.ToString()}\tSocinator\t{message.Replace("\t", " ")}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Error : " + ex.Message);
                }
            }));
        }

    }
}
