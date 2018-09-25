using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.Converters;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.SocioPublisher;
using EmbeddedBrowser;
//using EmbeddedBrowser;
using FluentScheduler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using Socinator.Social.AutoActivity.Views;
using DominatorHouse.Model;
using DominatorHouseCore.Command;
using Dragablz;

namespace DominatorHouse.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {

        public MainWindowViewModel()
        {
            try
            {

                Application.Current.Dispatcher.Invoke(() =>
                {
                    FatalErrorDiagnosis();
                    MainWindowModel.Languages.Add("English");

                    MainWindowModel.LoggerCollection =
                        CollectionViewSource.GetDefaultView(MainWindowModel.LstLoggerModels);
                });
                ResizeLoggerCommand = new BaseCommand<object>((sender) => true, ResizeLogger);
                NetworkChangeCommand = new BaseCommand<object>((sender) => true, NetworkSelectionChange);
                TabItemSelectCommand = new BaseCommand<object>((sender) => true, OnSelectTabItem);
                WinClosingCommand = new BaseCommand<object>((sender) => true, OnClosing);
                LoggerNetworkChangeCommand = new BaseCommand<object>((sender) => true, LoggerNetworkChange);
                LoggerActivityChangeCommand = new BaseCommand<object>((sender) => true, LoggerActivityChange);
                LoggerSelectionChangeCommand = new BaseCommand<object>((sender) => true, LoggerSelectionChange);
                LanguageChangeCommand = new BaseCommand<object>((sender) => true, LanguageChange);
                
                BindingOperations.EnableCollectionSynchronization(MainWindowModel.LstLoggerModels, _lock);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #region Command

        public ICommand ResizeLoggerCommand { get; set; }
        public ICommand NetworkChangeCommand { get; set; }
        public ICommand TabItemSelectCommand { get; set; }
        public ICommand WinClosingCommand { get; set; }
        public ICommand LoggerNetworkChangeCommand { get; set; }
        public ICommand LoggerActivityChangeCommand { get; set; }
        public ICommand LoggerSelectionChangeCommand { get; set; }
        public ICommand LanguageChangeCommand { get; set; }
        #endregion

        #region Property

        private static object _lock = new object();
        public Grid MainGrid { get; set; }
        private MainWindowModel mainWindowModel = new MainWindowModel();

        public MainWindowModel MainWindowModel
        {

            get
            {
                return mainWindowModel;
            }
            set
            {
                if (mainWindowModel == value)
                    return;
                SetProperty(ref mainWindowModel, value);
            }
        }

        private static PerformanceCounter PerformanceCounter { get; }
          = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");
        static readonly string RamSize = GetRamsize();
        string _fatalError;

        #endregion

        #region System Details  

        private async void StartbindMemory()
        {
            while (true)
            {
                var availablememory = GetMemoryUsage().ToString(CultureInfo.InvariantCulture);

                var cpuUsage = GetCpuUsage();

                try
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MainWindowModel.Datetime = " : " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        MainWindowModel.RamSize = " " + RamSize ;
                        MainWindowModel.Availablememory = " " + availablememory + " MB";
                        MainWindowModel.CpuUsage = " " + cpuUsage + "%";
                    });

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                await Task.Delay(100);
            }
        }

        private static string GetRamsize()
        {
            var objManagementClass = new ManagementClass("Win32_ComputerSystem");
            var objManagementObjectCollection = objManagementClass.GetInstances();
            foreach (var item in objManagementObjectCollection)
                return Convert.ToString(
                           Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0),
                           CultureInfo.InvariantCulture) + " MB";

            return " 0 MB";
        }

        private static string GetCpuUsage()
        {
            try
            {
                Processor.Get();
                return Processor.Properties["PercentProcessorTime"].Value.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }


        private static double GetMemoryUsage()
        {
            var memAvailable = (double)PerformanceCounter.NextValue();
            return memAvailable;
        }

        #endregion

        #region Filteration

        public bool FilterByActivityType(object sender)
        {
            try
            {
                var selectedTab = MainWindowModel.SelectedLogTab?.Header.ToString();
                var type = (selectedTab == "Info") ? "Info" : "Error";
                var logger = sender as LoggerModel;

                return logger?.Network.IndexOf(MainWindowModel.SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0
                    && logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return true;
            }

        }
        public bool FilterByNetwork(object sender)
        {
            try
            {
                var selectedTab = MainWindowModel.SelectedLogTab?.Header.ToString();
                var type = (selectedTab == "Info") ? "Info" : "Error";
                var logger = sender as LoggerModel;
                if (MainWindowModel.SelectedNetwork == SocialNetworks.Social.ToString())
                    return true;
                return logger?.Network.IndexOf(MainWindowModel.SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0
                    && logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

        }
        public bool FilterByNetworkActivity(LoggerModel logger, string type)
        {
            if (!string.IsNullOrEmpty(MainWindowModel.SelectedActivity) && !string.IsNullOrEmpty(MainWindowModel.SelectedNetwork))
            {
                return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0 && logger?.Network.IndexOf(MainWindowModel.SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0
                && logger?.Network.IndexOf(MainWindowModel.SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0;

            }
            else if (!string.IsNullOrEmpty(MainWindowModel.SelectedActivity))
                return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0
                && logger?.Network.IndexOf(MainWindowModel.SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0;

            else if (!string.IsNullOrEmpty(MainWindowModel.SelectedNetwork))
                return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0 && logger?.Network.IndexOf(MainWindowModel.SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0;

            return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;

        }
        public bool FilterByInfo(object sender)
        {
            try
            {
                var logger = sender as LoggerModel;
                return FilterByNetworkActivity(logger, "Info");
                //  return logger?.LogType.IndexOf("Info", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }

        }

        public bool FilterByError(object sender)
        {
            try
            {
                var logger = sender as LoggerModel;
                return FilterByNetworkActivity(logger, "Error");
                //  return logger?.LogType.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0 || logger?.LogType.IndexOf("Warn", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        #endregion

        #region Command Methods

        private void LanguageChange(object sender)
        {

        }

        private void LoggerSelectionChange(object sender)
        {
            try
            {
                if (MainWindowModel.SelectedLogTab == null)
                {
                    try
                    {
                        var selectedTab = (((TabablzControl)sender).SelectedItem as TabItem)?.Header.ToString();

                        if (selectedTab == MainWindowModel.LastTab)
                            return;

                        if (selectedTab?.IndexOf("Info", StringComparison.InvariantCultureIgnoreCase) == 0)
                        {
                            MainWindowModel.LastTab = "Info";
                            MainWindowModel.LoggerCollection.Filter += FilterByInfo;
                        }
                        else
                        {
                            MainWindowModel.LastTab = "Error";
                            MainWindowModel.LoggerCollection.Filter += FilterByError;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                    MainWindowModel.LoggerCollection.Filter += FilterByNetwork;
            }
            catch (Exception ex)
            {
                MainWindowModel.LoggerCollection.Filter += FilterByNetwork;
                ex.DebugLog();
            }
        }

        private void LoggerActivityChange(object sender)
        {
            try
            {
                MainWindowModel.LoggerCollection.Filter += FilterByActivityType;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                MainWindowModel.LoggerCollection.Filter = null;
            }
        }

        private void LoggerNetworkChange(object sender)
        {
            try
            {
                MainWindowModel.ActivityType.Clear();
                if (MainWindowModel.SelectedNetwork == SocialNetworks.Social.ToString())
                {
                    MainWindowModel.ActivityType = new ObservableCollection<string>(Enum.GetNames(typeof(ActivityType)));
                    MainWindowModel.LoggerCollection.Filter += FilterByNetwork;
                    return;
                }

                foreach (var name in Enum.GetNames(typeof(ActivityType)))
                {
                    if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(MainWindowModel.SelectedNetwork))
                        MainWindowModel.ActivityType.Add(name);
                }
                MainWindowModel.LoggerCollection.Filter += FilterByNetwork;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
                MainWindowModel.LoggerCollection.Filter = null;
            }
        }

        private void OnClosing(object sender)
        {
            try
            {
                var e = (CancelEventArgs)sender;
                e.Cancel = true;
                bool isClose = Dialog.ShowCustomDialog("Confirmation", "Are you sure to close Socinator?", "Yes", "No") == MessageDialogResult.Affirmative;
                if (isClose)
                {
                    Application.Current.Shutdown();
                    Process.GetCurrentProcess().Kill();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void OnSelectTabItem(object sender)
        {
            try
            {
                var textBlockDetails = (FrameworkElement)sender as TextBlock;

                if (textBlockDetails == null)
                    return;

                if (textBlockDetails.Text == Application.Current.FindResource("LangKeyAccountsActivity").ToString())
                {
                    DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);

                    // var accountUi = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Social).GetNetworkCoreFactory()
                    //    .AccountUserControlTools;
                    //accountUi.GetStartupToolsView();
                }
                if (textBlockDetails.Text == Application.Current.FindResource("LangKeyPublisher").ToString())
                {
                    PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
                }
                if (textBlockDetails.Text == Application.Current.FindResource("LangKeyAccountsManager").ToString())
                {
                    AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
                }
                if (textBlockDetails.Text == Application.Current.FindResource("LangKeySociopublisher").ToString())
                {
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherDefaultPage.Instance();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void NetworkSelectionChange(object sender)
        {
            try
            {
                var network = (SocialNetworks)sender;

                MainWindowModel.TabDock = Dock.Top;

                if (network == SocialNetworks.Social)
                    MainWindowModel.TabDock = Dock.Left;
                TabInitialize(network);

            }
            catch (Exception ex)
            {

            }
        }


        private void ResizeLogger(object sender)
        {
            try
            {
                MainGrid = sender as Grid;
                if (MainGrid.RowDefinitions[2].Height.Value <= 200 && MainGrid.RowDefinitions[2].Height.Value > 45)
                    MainGrid.RowDefinitions[2].Height = new GridLength(45);
                else
                    MainGrid.RowDefinitions[2].Height = new GridLength(200);
            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }
        }

        #endregion

        #region Methods

        private async Task IsCheck()
        {
            try
            {
                var key = SocinatorKeyHelper.GetKey();

                var networks = await UtilityManager.LogIndividualNetworksExceptions(key.FatalErrorMessage);

                if (networks.Count <= 1)
                {


                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            Application.Current.Shutdown();
                            Process.GetCurrentProcess().Kill();
                        });
                    }
                    else
                    {
                        Application.Current.Shutdown();
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }
            catch (Exception)
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        Application.Current.Shutdown();
                        Process.GetCurrentProcess().Kill();
                    });
                }
                else
                {
                    Application.Current.Shutdown();
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        private async Task FatalErrorDiagnosis()
        {
            try
            {
                string fatalError;
                var key = SocinatorKeyHelper.GetKey();
                if (key != null)
                {
                    var settings = new MetroDialogSettings()
                    {
                        DefaultText = string.IsNullOrEmpty(key.FatalErrorMessage) ? "" : key.FatalErrorMessage,
                        AffirmativeButtonText = "Validate"
                    };
                    while (true)
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "License", settings);
                        if (await IsProcessFatalError(fatalError))
                            continue;
                        else break;
                    }
                }
                else
                    while (true)
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "License");
                        if (await IsProcessFatalError(fatalError))
                            continue;
                        else break;
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task<bool> DiagnoseFatalError(string fatalError)
        {
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "Hang On! Checking your License status",
                "this will take few moments...");
            controller.SetIndeterminate();
            _fatalError = fatalError;
            var networks = await UtilityManager.LogIndividualNetworksExceptions(_fatalError);

            if (networks == null)
            {
                await controller.CloseAsync();
                return await DiagnoseFatalError(fatalError);
            }
            if (networks.Count <= 1)
            {
                // Close();
                await controller.CloseAsync();
                await FatalErrorDiagnosis();
                return true;
            }


            MainWindowModel._strategies = new DominatorAccountViewModel.AccessorStrategies
            {
                ActionCheckAccount = AccountStatusChecker,
                AccountBrowserLogin = AccountBrowserLogin,
                _determine_available = (SocialNetworks s) => MainWindowModel._availableNetworks.Contains(s),
                _inform_warnings = GlobusLogHelper.log.Warn,
                action_UpdateFollower = AccountUpdate,
                EditProfile = EditProfile,
                RemovePhoneVerification = RemovePhoneVerification
            };
            Socinator.DominatorCores.DominatorCoreBuilder.Strategies = MainWindowModel._strategies;

            var fatalErrorHandler = new DominatorHouseCore.Models.FatalErrorHandler
            {
                FatalErrorMessage = fatalError,
                FatalErrorAddedDate = DateTime.Now,
                ErrorNetworks = networks
            };
            SocinatorKeyHelper.SaveKey(fatalErrorHandler);
            FeatureFlags.Check("SocinatorInitializer", SocinatorInitializer);
            await controller.CloseAsync();
            return true;
        }

        private void RemovePhoneVerification(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.RemovePhoneVerification(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditProfile(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.EditProfile(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task<bool> IsProcessFatalError(string fatalError)
        {
            if (!string.IsNullOrEmpty(fatalError) && await DiagnoseFatalError(fatalError))
                return false;
            else if (fatalError == null)
                Application.Current.MainWindow.Close();
            else
            {
                if (DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "License", "Please validate Socinator !!", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    return true;
                else
                    Application.Current.MainWindow.Close();
            }

            return false;
        }

        private void SocinatorInitializer()
        {
            try
            {
                var streamq = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt");
                var accountCustomControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, MainWindowModel._strategies);

                ThreadFactory.Instance.Start(() =>
                {
                    JobManager.AddJob(() => InitializeJobCores(_fatalError), x => x.ToRunNow());
                });

                //Init UI delegates            
                CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg) ==
                    MessageDialogResult.Affirmative;

                TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;

                ConfigFileManager.ApplyTheme();

                var performanceTask = new Task(StartbindMemory,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
                performanceTask.Start();

                TabSwitcher.ChangeTabIndex = (mainTabIndex, subTabIndex) =>
                {
                    MainWindowModel.SelectedViewIndex = mainTabIndex;

                    if (subTabIndex == null)
                        return;

                    var selectedTabObject = mainWindowModel.SelectedContent?.Content.Value;

                    ((dynamic)selectedTabObject)?.setIndex((int)subTabIndex);
                };

                // Go to campaign from respective module after campaign saved
                TabSwitcher.GoToCampaign = ()
                    => MainWindowModel.SelectedViewIndex =
                        MainWindowModel.TabItems.FindIndex(x => x.Title == Application.Current.FindResource("LangKeyCampaigns")?.ToString());

                //Closed += (o, e) => Process.GetCurrentProcess().Kill();
            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                MainWindowModel.SelectedViewIndex = index;
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);

            }
            else
            {
                MainWindowModel.SelectedViewIndex = index;

                DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);

            }
        }

        public void NetworkSelectionChanges(string network)
        {
            try
            {
                //if (mainWindowModel.TabItems == null)
                //    return;
                MainWindowModel.TabDock = Dock.Top;
                var selectedSocialNetwork =
                    (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                if (selectedSocialNetwork == SocialNetworks.Social)
                    MainWindowModel.TabDock = Dock.Left;
                TabInitialize(selectedSocialNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void TabInitialize(SocialNetworks network)
        {
            try
            {
                var tabHandler = SocinatorInitialize.GetSocialLibrary(network).GetNetworkCoreFactory().TabHandlerFactory;
                if (tabHandler == null)
                    return;
                MainWindowModel.TabItems = new ObservableCollection<TabItemTemplates>(tabHandler.NetworkTabs);
                Application.Current.MainWindow.Title = tabHandler.NetworkName;
                MainWindowModel.SelectedViewIndex = 0;
                tabHandler.UpdateAccountCustomControl(network);
                SocinatorInitialize.SetAsActiveNetwork(network);
            }
            catch (Exception ex)
            {
                MainWindowModel.TabDock = Dock.Left;

                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Fatal Error",
                    $"Please purchase access of {network} automation features!");
                ex.DebugLog();
                MainWindowModel.SelectedNetworkIndex = 0;
            }
        }

        public void InitializeJobCores(string license)
        {

            try
            {
                var streamq = Assembly.GetExecutingAssembly()
                    .GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt");
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt"))
                {
                    TextReader tr = new StreamReader(stream);
                    ConstantVariable.Revision = tr.ReadToEnd();
                }
                ThreadFactory.Instance.Start(() =>
                {
                    var nextDayTime = DateTime.Now.AddDays(1);

                    JobManager.AddJob(() => InitializeJobCores(license),
                        x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                            .AndEvery(1).Days());
                });

                MainWindowModel.AvailableNetworks = SocinatorInitialize.AvailableNetworks;
                var to_remove = new List<SocialNetworks>();
                FeatureFlags.UpdateFeatures();
                foreach (var network in MainWindowModel.AvailableNetworks)
                {
                    FeatureFlags.Check(network.ToString(), () =>
                    {
                        try
                        {
                            var networkNamespace = SocinatorInitialize.GetNetworksNamespace(network);
                            var networkAssembly = Assembly.Load(networkNamespace);

                            #region Network Functionality

                            var networkFullNameSpace = $"{networkNamespace}.Factories.{network}NetworkCollectionFactory";
                            var networkType = networkAssembly.GetType(networkFullNameSpace);
                            // is this a correct type?
                            if (typeof(INetworkCollectionFactory).IsAssignableFrom(networkType))
                            {
                                INetworkCollectionFactory networkCoreFactory;
                                var constructors = networkType.GetConstructors();
                                // do we have a constructor taking a strategy object?
                                var selectedConstructor = constructors.FirstOrDefault(ci =>
                                {
                                    var pars = ci.GetParameters();
                                    return pars.Length == 1 && pars[0].ParameterType ==
                                       typeof(DominatorAccountViewModel.AccessorStrategies);
                                });
                                if (selectedConstructor != default(ConstructorInfo))
                                {
                                    networkCoreFactory =
                                        (INetworkCollectionFactory)selectedConstructor.Invoke(new object[] { MainWindowModel._strategies });
                                }
                                else
                                {
                                    // if not, do we have a constructor with no parameters?
                                    selectedConstructor = constructors.First(ci => ci.GetParameters().Length == 0);
                                    networkCoreFactory = (INetworkCollectionFactory)selectedConstructor.Invoke(null);
                                }
                                SocinatorInitialize.SocialNetworkRegister(networkCoreFactory, network);
                            }

                            #endregion

                            #region Publisher Functionality

                            try
                            {
                                var publisherFullNameSpace = $"{networkNamespace}.Factories.{network}PublisherCollectionFactory";
                                var publisherType = networkAssembly.GetType(publisherFullNameSpace);

                                if (!typeof(IPublisherCollectionFactory).IsAssignableFrom(publisherType))
                                    return;

                                var constructors = publisherType.GetConstructors();
                                var selectedConstructor = constructors.First(ci => ci.GetParameters().Length == 0);
                                var publisherCoreFactory = (IPublisherCollectionFactory)selectedConstructor.Invoke(null);
                                PublisherInitialize.SaveNetworkPublisher(publisherCoreFactory, network);
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }

                            #endregion

                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            to_remove.Add(network);
                            ex.DebugLog();
                        }
                    });
                }

                MainWindowModel.AvailableNetworks.ExceptWith(to_remove);

                FeatureFlags.UpdateFeatures();

                var softWareSettings = new DominatorHouse.Utilities.SoftwareSettings();
                var softWareSetting = new DominatorHouseCore.Settings.SoftwareSettings();
                ThreadFactory.Instance.Start(() =>
                {

                    softWareSettings.InitializeOnLoadConfigurations(MainWindowModel._strategies);

                    // For Every day backup
                    softWareSetting.InitializeOnLoadConfigurations(); DirectoryUtilities.DeleteOldLogsFile();
                    //DirectoryUtilities.Compress();


                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                    PublishScheduler.ScheduleTodaysPublisher();
                    PublishScheduler.UpdateNewGroupList();
                    var publisherPostFetcher = new PublisherPostFetcher();
                    publisherPostFetcher.StartFetchingPostData();

                    var deletionPostlist =
                    GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                        .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                });





                #region Publisher

                //ThreadFactory.Instance.Start(() =>
                //{
                //    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                //    PublishScheduler.ScheduleTodaysPublisher();
                //    PublishScheduler.UpdateNewGroupList();
                //    var publisherPostFetcher = new PublisherPostFetcher();
                //    publisherPostFetcher.StartFetchingPostData();

                //    var deletionPostlist =
                //    GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                //        .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                //    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                //});

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    JobManager.AddJob(async () => await IsCheck(),
                        x => x.ToRunOnceAt(DateTime.Now.AddHours(1))
                            .AndEvery(1).Hours());
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog();
            }
            catch (AggregateException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.CheckStatus(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountUpdate(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.UpdateDetails(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                var browserWindow = new BrowserWindow(dominatorAccountModel);
                browserWindow.Show();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                //MessageBox.Show(ex.Message);
            }
        }


        #endregion

    }
}
