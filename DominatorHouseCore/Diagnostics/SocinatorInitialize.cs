using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Management;
using System.Windows;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using System.Net.NetworkInformation;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media.Animation;
using ControlzEx.Standard;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Request;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json.Linq;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using ConfigurationManager = System.Configuration.ConfigurationManager;

namespace DominatorHouseCore.Diagnostics
{
    public class SocinatorInitialize
    {
        private static bool _isInitialized;

        private static Dictionary<SocialNetworks, INetworkCollectionFactory> RegisteredNetworks { get; } =
            new Dictionary<SocialNetworks, INetworkCollectionFactory>();


        public static int MaximumAccountCount { get; set; } = 10000;

        public static HashSet<SocialNetworks> AvailableNetworks { get; set; } = new HashSet<SocialNetworks>();

        public static bool IsNetworkAvailable(SocialNetworks network)
            => AvailableNetworks.Contains(network);

        public static HashSet<SocinatorIntellisenseModel> Macros { get; set; } = new HashSet<SocinatorIntellisenseModel>();

        public static void InitializeMacros()
        {
            try
            {
                var macros = GenericFileManager.GetModuleDetails<SocinatorIntellisenseModel>(ConstantVariable.GetMacroDetails);
                Macros.Clear();
                macros?.ForEach(macro =>
                {
                    Macros.Add(new SocinatorIntellisenseModel { Key = @"{" + macro.Key + @"}", Value = macro.Value });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static Dictionary<SocialNetworks, string> NetworksNamespace { get; set; } = new Dictionary<SocialNetworks, string>()
        {
            {SocialNetworks.Social,ConfigurationManager.AppSettings["Social"] },
            {SocialNetworks.Facebook,ConfigurationManager.AppSettings["Facebook"]},
            {SocialNetworks.Twitter,ConfigurationManager.AppSettings["Twitter"]},
            {SocialNetworks.Gplus,ConfigurationManager.AppSettings["Gplus"]},
            {SocialNetworks.Instagram,ConfigurationManager.AppSettings["Instagram"]},
            {SocialNetworks.LinkedIn,ConfigurationManager.AppSettings["LinkedIn"]},
            {SocialNetworks.Quora,ConfigurationManager.AppSettings["Quora"]},
            {SocialNetworks.Pinterest,ConfigurationManager.AppSettings["Pinterest"]},
            {SocialNetworks.Tumblr,ConfigurationManager.AppSettings["Tumblr"]},
            {SocialNetworks.Youtube,ConfigurationManager.AppSettings["Youtube"]},
            {SocialNetworks.Reddit,ConfigurationManager.AppSettings["Reddit"]},
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


    public class UtilityManager
    {
        public static async Task<HashSet<SocialNetworks>> ResolveExceptions(string inputString, string exemption, string fixtures)
        {
            var message = "Oops something went wrong";

            try
            {
                if (inputString == ConfigurationManager.AppSettings["Unavailable"])
                {
                    string finalResponse;
                    using (var streamReader = new StreamReader(await LogExemptions(exemption, fixtures)))
                    {
                        finalResponse = streamReader.ReadToEnd();
                    }
                    return await ResolveExceptions(JObject.Parse(finalResponse)["code"].ToString(), exemption,
                        fixtures);
                }

                if (inputString == ConfigurationManager.AppSettings["EmptyExemption"])
                    message = ConfigurationManager.AppSettings["EmptyExemptionErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["ExemptionNotFound"])
                    message = ConfigurationManager.AppSettings["ExemptionNotFoundErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["ExemptionDisabled"])
                    message = ConfigurationManager.AppSettings["ExemptionDisabledErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["ExemptionExpired"])
                    message = ConfigurationManager.AppSettings["ExemptionExpiredErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["InvalidInput"])
                    message = ConfigurationManager.AppSettings["InvalidInputErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["MoreLimits"])
                    message = ConfigurationManager.AppSettings["MoreLimitsErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["NoMoreAllowed"])
                    message = ConfigurationManager.AppSettings["NoMoreAllowedErrorMessage"];

                else if (inputString == ConfigurationManager.AppSettings["Matched"])
                {
                    var availableExemption = await FindExemptions(exemption);

                    string details;
                    using (var streamReader = new StreamReader(availableExemption))
                    {
                        string decryptedString;
                        var availbleNetworkResponse = streamReader.ReadToEnd();
                        var exemptionNumber = JObject.Parse(availbleNetworkResponse)[ConfigurationManager.AppSettings["ExemptionId"]].ToString();
                        var exemptionErrorDetails = await GetExemptionInnerException(exemptionNumber);
                        using (var stream = new StreamReader(exemptionErrorDetails))
                        {
                            decryptedString = stream.ReadToEnd();
                        }
                        details = AesDecryption.DecryptAes(decryptedString);
                    }
                    return LogExceptionForEachNetwork(details);
                }

                else if (inputString == ConfigurationManager.AppSettings["Unknown"])
                    message = ConfigurationManager.AppSettings["UnknownErrorMessage"];

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    //var customDialog = new CustomDialog()
                    //{
                    //    HorizontalAlignment = HorizontalAlignment.Center,
                    //    Content = message
                    //};
                    //var objDialog = new Dialog();
                    //var dialogWindow = objDialog.GetMetroWindowWithOutClose(customDialog, ConfigurationManager.AppSettings["Title"]);
                   

                    //var sleep = 1;
                    //while (true)
                    //{
                    //    if (sleep < 10)
                    //    {
                            
                    //    }
                    //}
                    
                    //dialogWindow.ShowDialog();
                    //Thread.Sleep(10 * 1000);
                    //dialogWindow.Close();
                    Dialog.ShowDialog(ConfigurationManager.AppSettings["Title"], message);
                    return new HashSet<SocialNetworks>();
                });
            }
            else
            {
                //var customDialog = new CustomDialog()
                //{
                //    HorizontalAlignment = HorizontalAlignment.Center,
                //    Content = message
                //};
                //var objDialog = new Dialog();
                //var dialogWindow = objDialog.GetMetroWindowWithOutClose(customDialog, ConfigurationManager.AppSettings["Title"]);
                //dialogWindow.ShowDialog();

                //Thread.Sleep(10 * 1000);
                //dialogWindow.Close();
                //return new HashSet<SocialNetworks>();
                Dialog.ShowDialog(ConfigurationManager.AppSettings["Title"], message);
                return new HashSet<SocialNetworks>();
            }

            return new HashSet<SocialNetworks>();
        }

      

        public static async Task<Stream> ProcessInputString(string exemption, string fixtures)
            => await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.ProcessingInput, exemption, fixtures));

        private static async Task<Stream> FindExemptions(string exemption)
            => await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.FindExemptions, exemption));

        private static async Task<Stream> GetExemptionInnerException(string innerException)
        {
            var key = ConfigurationManager.AppSettings["ExceptionKey"];
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.ExemptionInnerException, innerException, key));
        }

        private static async Task<Stream> LogExemptions(string exemption, string fixtures)
            => await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.LogExemptions, exemption, fixtures));

        private static async Task<Stream> LogDebugExemption(string exemption, string fixtures)
        {
            return await HttpHelper.GetResponseStreamAsync(string.Format(ConstantVariable.LogDebugExemption, exemption, fixtures));
        }

        public static string GetFixtures()
        {
            var uuid = string.Empty;
            try
            {
                var ComputerName = "localhost";
                var scope = new ManagementScope($"\\\\{ComputerName}\\root\\CIMV2", null);
                scope.Connect();
                var query = new ObjectQuery("SELECT UUID FROM Win32_ComputerSystemProduct");
                var searcher = new ManagementObjectSearcher(scope, query);
                foreach (var wmiObject in searcher.Get())
                {
                    uuid = wmiObject["UUID"].ToString();
                }

                var split = Regex.Split(uuid, "-");
                return split.Last();
            }
            catch (Exception e)
            {
                e.DebugLog($"Exception {e.Message} Trace {e.StackTrace}");
            }
            return null;

            //var fixtures = string.Empty;
            //try
            //{
            //    foreach (var nic in NetworkInterface.GetAllNetworkInterfaces())
            //    {
            //        if (nic.OperationalStatus != OperationalStatus.Up)
            //            continue;

            //        if (!string.IsNullOrEmpty(fixtures))
            //            break;

            //        fixtures += nic.GetPhysicalAddress().ToString();
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex.Message);
            //}
            //return fixtures;
        }

        public static async Task<HashSet<SocialNetworks>> LogIndividualNetworksExceptions(string exemption)
        {
            try
            {
                if (string.IsNullOrEmpty(exemption))
                {
                    FeatureFlags.Instance = new FeatureFlags
                    {
                        {"SocinatorInitializer", true}
                    };
                    return SocinatorInitialize.AvailableNetworks = new[] { SocialNetworks.Social }.ToHashSet();
                }

                var fixture = GetFixtures();

                string finalResponse;
                var responseStream = await ProcessInputString(exemption, fixture);
                using (var streamReader = new StreamReader(responseStream))
                {
                    finalResponse = streamReader.ReadToEnd();
                }

                return await ResolveExceptions(JObject.Parse(finalResponse)["code"].ToString(), exemption, fixture);
            }
            catch (Exception ex)
            {
                ex.DebugLog();

                if (!ex.Message.Contains("The remote name could not be resolved"))
                    return SocinatorInitialize.AvailableNetworks;

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Network Error",
                    "Please check your Internet connection and try again !!"
                    , MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Try Again", "Cancel"));
                if (dialogResult == MessageDialogResult.Affirmative)
                {
                    return null;
                }
            }
            return SocinatorInitialize.AvailableNetworks;
        }




        public static HashSet<SocialNetworks> LogExceptionForEachNetwork(string details)
        {
            try
            {
                FeatureFlags.Instance = new FeatureFlags { { "SocinatorInitializer", true } };

                SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);

                if (string.IsNullOrEmpty(details))
                    return SocinatorInitialize.AvailableNetworks;

                #region All networks with unlimited Networks

                try
                {
                    var jsonArray = JArray.Parse(details);

                    #region Getting Exemption Title

                    var exemptionItems = jsonArray.Children()["nested"].First()[ConfigurationManager.AppSettings["ExemptionItem"]].ToString();
                    var arrayExemptionItems = JArray.Parse(exemptionItems)[0];
                    var exemptionTitle = arrayExemptionItems[ConfigurationManager.AppSettings["ExemptionTitle"]].ToString();

                    #endregion

                    #region Full Exemption

                    if (exemptionTitle == ConfigurationManager.AppSettings["FullExemption"])
                    {
                        try
                        {
                            var options = jsonArray.Children()["nested"].First()[ConfigurationManager.AppSettings["ExemptionItem"]].First()["options"]
                                .ToString();
                            var packageCount =
                                JObject.Parse(options)[ConfigurationManager.AppSettings["SelectPackage"]]["value"].ToString();

                            SocinatorInitialize.MaximumAccountCount = int.Parse(Utilities.GetIntegerOnlyString(packageCount));
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Twitter);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Facebook);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Gplus);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Instagram);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.LinkedIn);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Quora);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Pinterest);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Tumblr);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Youtube);
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Reddit);
                        }
                        catch (Exception ex)
                        {
                            SocinatorInitialize.MaximumAccountCount = 0;
                            ex.DebugLog();
                        }
                    }
                    #endregion

                    #region Custom Exemption

                    else if (exemptionTitle == ConfigurationManager.AppSettings["CustomExemption"])
                    {
                        try
                        {
                            var arrInvoiceItems = JArray.Parse(exemptionItems);

                            SocinatorInitialize.AvailableNetworks.Clear();

                            FeatureFlags.Instance = new FeatureFlags { { "SocinatorInitializer", true }, { "Social", true } };

                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);

                            foreach (var token in arrInvoiceItems)
                            {
                                try
                                {
                                    var tokenString = token["options"].ToString();

                                    var networkSplit = Regex.Split(tokenString, "},");

                                    SocinatorInitialize.MaximumAccountCount = 10000;

                                    foreach (var networkValues in networkSplit)
                                    {
                                        try
                                        {
                                            var isSelected = Utilities.GetBetween(networkValues, "value\":[\"", "\"],");
                                            if (isSelected != "1")
                                                continue;
                                            var network = Utilities.FirstMatchExtractor(networkValues, "optionLabel\":\"(.*?)\"");
                                            var networks = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                                            SocinatorInitialize.AvailableNetworks.Add(networks);
                                        }
                                        catch (Exception ex)
                                        {
                                            ex.DebugLog();
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    SocinatorInitialize.MaximumAccountCount = 0;
                                    ex.DebugLog();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            SocinatorInitialize.MaximumAccountCount = 0;
                        }
                    }
                    #endregion

                    #region Single Exemption

                    else
                    {
                        try
                        {
                            SocinatorInitialize.AvailableNetworks.Clear();
                            FeatureFlags.Instance = new FeatureFlags { { "SocinatorInitializer", true }, { "Social", true } };
                            SocinatorInitialize.AvailableNetworks.Add(SocialNetworks.Social);
                            var exemptionDescription = arrayExemptionItems[ConfigurationManager.AppSettings["ExemptionDescription"]].ToString();
                            exemptionDescription = exemptionDescription.Replace(ConstantVariable.MarketingSoftware, string.Empty).Trim();
                            var networks = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), exemptionDescription);
                            SocinatorInitialize.AvailableNetworks.Add(networks);
                            SocinatorInitialize.MaximumAccountCount = 10000;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                            SocinatorInitialize.MaximumAccountCount = 0;
                        }
                    }

                    #endregion

                    return SocinatorInitialize.AvailableNetworks;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return SocinatorInitialize.AvailableNetworks;
        }
    }
}