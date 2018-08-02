using NLog;
using NLog.Config;
using NLog.Targets;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System.Linq;
using DominatorHouseCore.Utility;
using System.Collections.Concurrent;

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
            ((Window)window).Loaded += (s, e) =>
            {
                //var target = new NlogUiTarget("Activity Log", LogLevel.Info);
                //target.Log += (log, error) => window.LogText(log, error);

                //var warning = new NlogUiTarget("Activity Warning", LogLevel.Warn);
                //warning.Log += (log, warns) => window.LogText(log, warns);

                var target = new NlogUiTarget("Activity Log", LogLevel.Info);
                target.Log += (log, loglevel) => window.LogText(log, loglevel);
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

                    list.Items.Insert(0, $"{DateTime.Now.ToString()}\t{message}");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(" Error : " + ex.Message);
                }
            }));
        }


        //public static void LogTextToList(ObservableCollection<LoggerModel> lstLoggerModels, string message, LogLevel logLevel)
        //{
        //    try
        //    {
        //        //if (lstLoggerModels.Count > 1000)
        //        //    lstLoggerModels.RemoveAt(lstLoggerModels.Count - 1);


        //        var messages = message.Split('\t');

        //        var log = new LoggerModel
        //        {
        //            DateTime = DateTime.Now,
        //            Network = messages[0].Trim(),
        //            AccountCampaign = messages[1].Trim(),
        //            ActivityType = messages[2].Trim(),
        //            Message = messages[3].Trim(),
        //            MessageCode = messages[4].Trim(),
        //            LogType = logLevel.ToString().Trim()

        //        };
        //        if(!Application.Current.Dispatcher.CheckAccess())
        //        {
        //            Application.Current.Dispatcher.Invoke( () => lstLoggerModels.Add(log));
        //        }
        //        else
        //        {
        //            lstLoggerModels.Add(log);
        //        }


        //        //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));

        //        //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));
        //        ////Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
        //    }
        //    catch (Exception ex)
        //    {
        //        if (!string.IsNullOrEmpty(message))
        //        {
        //            var log = new LoggerModel
        //            {
        //                Network = SocialNetworks.Social.ToString(),
        //                DateTime = DateTime.Now,
        //                Message = message,
        //                LogType = logLevel.ToString()
        //            };
        //            if (!Application.Current.Dispatcher.CheckAccess())
        //            {
        //                Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Add(log));
        //            }
        //            else
        //            {
        //                lstLoggerModels.Add(log);
        //            }
        //            //Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
        //            //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));
        //        }
        //        ex.DebugLog();
        //    }

        //}

        public static ConcurrentDictionary<LogLevel, object> LoggerListsDeletionlock { get; set; } = new ConcurrentDictionary<LogLevel, object>();

        public static void LogTextToList(ObservableCollection<LoggerModel> lstLoggerModels, string message, LogLevel logLevel)
        {
            try
            {
                if (lstLoggerModels.Count > 1000)
                {
                    var updatelock = LoggerListsDeletionlock.GetOrAdd(LogLevel.Info, _lock => new object());
                    lock (updatelock)
                    {
                        if (lstLoggerModels.Count > 1000)
                        {
                            var removeItems = lstLoggerModels.OrderBy(x => x.DateTime).Take(300);
                            removeItems.ForEach(item =>
                            {
                                lstLoggerModels.Remove(item);
                            });
                        }
                    }
                }

                var messages = message.Split('\t');

                var log = new LoggerModel
                {
                    DateTime = DateTime.Now,
                    Network = messages[0].Trim(),
                    AccountCampaign = messages[1].Trim(),
                    ActivityType = messages[2].Trim(),
                    Message = messages[3].Trim(),
                    MessageCode = messages[4].Trim(),
                    LogType = logLevel.ToString().Trim()

                };
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
                }
                else
                {
                    lstLoggerModels.Insert(0, log);
                }


                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));

                //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));
                ////Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(message))
                {
                    var log = new LoggerModel
                    {
                        Network = SocialNetworks.Social.ToString(),
                        DateTime = DateTime.Now,
                        Message = message,
                        LogType = logLevel.ToString()
                    };
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
                    }
                    else
                    {
                        lstLoggerModels.Insert(0, log);
                    }
                    //Application.Current.Dispatcher.Invoke(() => lstLoggerModels.Insert(0, log));
                    //Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() => lstLoggerModels.Insert(0, log)));
                }
                ex.DebugLog();
            }

        }
    }
}
