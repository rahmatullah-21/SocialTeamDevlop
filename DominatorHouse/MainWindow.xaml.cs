#region Namespaces
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
using DominatorHouse.ViewModel;

#endregion

namespace Socinator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILoggableWindow, INotifyPropertyChanged
    {

        private MainWindowViewModel _mainWindowViewModel;

        public MainWindowViewModel MainWindowViewModel
        {
            get { return _mainWindowViewModel; }
            set
            {
                _mainWindowViewModel = value;
                OnPropertyChanged(nameof(MainWindowViewModel));
            }
        }

        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);
               
                InitializeComponent();
                MainWindowViewModel = new MainWindowViewModel();
                SocinatorInitialize.LogInitializer(this);
                SocinatorWindow.DataContext = MainWindowViewModel;
                MainGrid.DataContext = MainWindowViewModel;
                Loaded += (o, e) =>
                {
                    GlobusLogHelper.log.Info($"Welcome to {ConstantVariable.ApplicationName}!");

                };

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void CopyCanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void CopyExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                ListView lb = (ListView)(sender);
                var message = (lb?.SelectedItem as LoggerModel).Message;
                if (!string.IsNullOrEmpty(message))
                {
                    Clipboard.SetText(message);
                    ToasterNotification.ShowSuccess("Message copied");
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void LogText(string message, LogLevel logLevel)
        {

            ThreadFactory.Instance.Start(() =>
            {
                GlobusLogHelper.LogTextToList(MainWindowViewModel.MainWindowModel.LstLoggerModels, message, logLevel);

                #region Filter Commented

                //try
                //{
                //    Application.Current.Dispatcher.Invoke(() =>
                //    {
                //        if (LastTab == "Info")
                //            LoggerCollection.Filter += FilterByInfo;

                //        else
                //            LoggerCollection.Filter += FilterByError;

                //    });

                //}
                //catch (Exception ex)
                //{
                //    ex.DebugLog();
                //} 

                #endregion

            });

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MainWindowViewModel.MainWindowModel.IsClickedFromMainWindow)
            {
                var dialog = new Dialog();

                var activityLogWindow = dialog.GetMetroWindow(Logger, "Activity Log");

                MainWindowViewModel.MainWindowModel.IsClickedFromMainWindow = false;
                activityLogWindow.Closing += (senders, events) =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                activityLogWindow.Content = null;
                                Grid.SetRow(Logger, 2);
                                MainGrid.Children.Add(Logger);

                                Logger.Children.Remove(RootLayout);
                                Logger.Children.Add(RootLayout);
                                MainGrid.RowDefinitions[2].Height = new GridLength(200);
                                MainWindowViewModel.MainWindowModel.IsClickedFromMainWindow = true;
                                ActivityLog.IsEnabled = true;
                                InitialTabablzControl.SelectedIndex = 0;
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        });
                    });
                };

                MainGrid.RowDefinitions[2].Height = new GridLength(0);
                MainGrid.Children.Remove(Logger);
                activityLogWindow.Show();
                ActivityLog.IsEnabled = false;

            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }



        #region Old Code [Commented]
        //private static readonly string RamSize = GetRamsize();

        //private HashSet<SocialNetworks> _availableNetworks;
        //private Dock _tabDock = Dock.Left;
        //private ObservableCollection<TabItemTemplates> _tabItems;
        //private ObservableCollection<string> _languages;

        //private Dictionary<string, CancellationToken> _accountUpdater = new Dictionary<string, CancellationToken>();

        //private bool IsClickedFromMainWindow { get; set; } = true;

        //private DominatorAccountViewModel.AccessorStrategies _strategies;

        //private string _fatalError;
        //public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            var accountUpdateFactory = SocinatorInitialize
        //                .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
        //                .GetNetworkCoreFactory().AccountUpdateFactory;
        //            accountUpdateFactory.CheckStatus(dominatorAccountModel);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void AccountUpdate(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            var accountUpdateFactory = SocinatorInitialize
        //                .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
        //                .GetNetworkCoreFactory().AccountUpdateFactory;
        //            accountUpdateFactory.UpdateDetails(dominatorAccountModel);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        var browserWindow = new BrowserWindow(dominatorAccountModel);
        //        browserWindow.Show();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        //MessageBox.Show(ex.Message);
        //    }
        //}

        #region System Details  

        //private async void StartbindMemory()
        //{
        //    while (true)
        //    {
        //        var availablememory = GetMemoryUsage().ToString(CultureInfo.InvariantCulture);

        //        var cpuUsage = GetCpuUsage();

        //        try
        //        {
        //            Dispatcher.Invoke(() =>
        //            {
        //                lbl_Datetime.Text = " : " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
        //                lbl_LoadedMemory.Text = " " + RamSize;
        //                lbl_Availablememory.Text = " " + availablememory + "  MB";
        //                lbl_CPUUsage.Text = " " + cpuUsage + " % ";
        //            });

        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }

        //        await Task.Delay(100);
        //    }
        //}


        //private static string GetRamsize()
        //{
        //    var objManagementClass = new ManagementClass("Win32_ComputerSystem");
        //    var objManagementObjectCollection = objManagementClass.GetInstances();
        //    foreach (var item in objManagementObjectCollection)
        //        return Convert.ToString(
        //                   Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0),
        //                   CultureInfo.InvariantCulture) + " MB";

        //    return "0 MB";
        //}


        //private static string GetCpuUsage()
        //{
        //    try
        //    {
        //        Processor.Get();
        //        return Processor.Properties["PercentProcessorTime"].Value.ToString();
        //    }
        //    catch (Exception)
        //    {
        //        return string.Empty;
        //    }
        //}


        //private static double GetMemoryUsage()
        //{
        //    var memAvailable = (double)PerformanceCounter.NextValue();
        //    return memAvailable;
        //}

        #endregion

        //private void SocinatorWindow_OnClosing(object sender, CancelEventArgs e)
        //{
        //    e.Cancel = true;

        //    bool isClose = this.ShowModalMessageExternal("Confirmation", "Are you sure to close Socinator?", MessageDialogStyle.AffirmativeAndNegative,
        //                         Dialog.SetMetroDialogButton("Yes", "No")) == MessageDialogResult.Affirmative;
        //    if (isClose)
        //    {
        //        Application.Current.Shutdown();
        //        Process.GetCurrentProcess().Kill();
        //    }

        //}

        //private void CmbAvailableNetworks_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{

        //    try
        //    {
        //        MainWindowViewModel.MainWindowModel.ActivityType.Clear();
        //        if (CmbAvailableNetworks.SelectedItem.ToString() == SocialNetworks.Social.ToString())
        //        {
        //            MainWindowViewModel.MainWindowModel.ActivityType = new ObservableCollection<string>(Enum.GetNames(typeof(ActivityType)));
        //            MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByNetwork;
        //            return;
        //        }

        //        foreach (var name in Enum.GetNames(typeof(ActivityType)))
        //        {
        //            if (EnumDescriptionConverter.GetDescription((ActivityType)Enum.Parse(typeof(ActivityType), name)).Contains(CmbAvailableNetworks.SelectedItem.ToString()))
        //                MainWindowViewModel.MainWindowModel.ActivityType.Add(name);
        //        }
        //        MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByNetwork;

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        MainWindowViewModel.MainWindowModel.LoggerCollection.Filter = null;
        //    }
        //    // GlobusLogHelper.log.Error("this is error");
        //}


        //private void CmbActivityType_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByActivityType;
        //}

        #region Filteration

        //private bool FilterByActivityType(object sender)
        //{
        //    try
        //    {
        //        var selectedTab = (InitialTabablzControl.SelectedItem as TabItem)?.Header.ToString();
        //        var type = (selectedTab == "Info") ? "Info" : "Error";
        //        var logger = sender as LoggerModel;

        //        return logger?.Network.IndexOf(SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            && logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;


        //        //var logger = sender as LoggerModel;
        //        //return logger?.ActivityType?.IndexOf(CmbActivityType.SelectedItem.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return false;
        //    }

        //}
        //private bool FilterByNetwork(object sender)
        //{
        //    try
        //    {
        //        var selectedTab = (InitialTabablzControl.SelectedItem as TabItem)?.Header.ToString();
        //        var type = (selectedTab == "Info") ? "Info" : "Error";
        //        var logger = sender as LoggerModel;
        //        if (SelectedNetwork == SocialNetworks.Social.ToString())
        //            return true;
        //        return logger?.Network.IndexOf(SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0
        //            && logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return false;
        //    }

        //}
        //private bool FilterByNetworkActivity(LoggerModel logger, string type)
        //{
        //    if (!string.IsNullOrEmpty(SelectedActivity) && !string.IsNullOrEmpty(SelectedNetwork))
        //    {
        //        return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0 && logger?.Network.IndexOf(SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0
        //        && logger?.Network.IndexOf(SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0;

        //    }
        //    else if (!string.IsNullOrEmpty(SelectedActivity))
        //        return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0
        //        && logger?.Network.IndexOf(SelectedActivity, StringComparison.InvariantCultureIgnoreCase) >= 0;

        //    else if (!string.IsNullOrEmpty(SelectedNetwork))
        //        return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0 && logger?.Network.IndexOf(SelectedNetwork, StringComparison.InvariantCultureIgnoreCase) >= 0;

        //    return logger?.LogType.IndexOf(type, StringComparison.InvariantCultureIgnoreCase) >= 0;

        //}
        //private bool FilterByInfo(object sender)
        //{
        //    try
        //    {
        //        var logger = sender as LoggerModel;
        //        return FilterByNetworkActivity(logger, "Info");
        //        //  return logger?.LogType.IndexOf("Info", StringComparison.InvariantCultureIgnoreCase) >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return false;
        //    }

        //}

        //private bool FilterByError(object sender)
        //{
        //    try
        //    {
        //        var logger = sender as LoggerModel;
        //        return FilterByNetworkActivity(logger, "Error");
        //        //  return logger?.LogType.IndexOf("Error", StringComparison.InvariantCultureIgnoreCase) >= 0 || logger?.LogType.IndexOf("Warn", StringComparison.InvariantCultureIgnoreCase) >= 0;
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //        return false;
        //    }
        //}
        #endregion

        #region Properties

        //private ObservableCollection<LoggerModel> _lstLoggerModels = new ObservableCollection<LoggerModel>();
        //public ObservableCollection<LoggerModel> LstLoggerModels
        //{
        //    get
        //    {
        //        return _lstLoggerModels;
        //    }
        //    set
        //    {
        //        _lstLoggerModels = value;
        //        OnPropertyChanged(nameof(LstLoggerModels));
        //    }
        //}
        //private ObservableCollection<string> _activityType = new ObservableCollection<string>();
        //public ObservableCollection<string> ActivityType
        //{
        //    get
        //    {
        //        return _activityType;
        //    }
        //    set
        //    {
        //        _activityType = value;
        //        OnPropertyChanged(nameof(ActivityType));
        //    }
        //}
        //private ICollectionView _loggerCollection;

        //public ICollectionView LoggerCollection
        //{
        //    get
        //    {
        //        return _loggerCollection;
        //    }
        //    set
        //    {
        //        _loggerCollection = value;
        //        OnPropertyChanged(nameof(LoggerCollection));
        //    }
        //}
        //private string _selectedNetwork = string.Empty;

        //public string SelectedNetwork
        //{
        //    get
        //    {
        //        return _selectedNetwork;
        //    }
        //    set
        //    {
        //        _selectedNetwork = value;
        //        OnPropertyChanged(nameof(SelectedNetwork));
        //    }
        //}
        //private string _selectedActivity = string.Empty;

        //public string SelectedActivity
        //{
        //    get
        //    {
        //        return _selectedActivity;
        //    }
        //    set
        //    {
        //        _selectedActivity = value;
        //        OnPropertyChanged(nameof(SelectedActivity));
        //    }
        //}

        #endregion

        //  public string LastTab { get; set; } = "Info";

        //private void InitialTabablzControl_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    try
        //    {
        //        if (CmbAvailableNetworks.SelectedItem == null)
        //        {
        //            try
        //            {
        //                var selectedTab = (InitialTabablzControl.SelectedItem as TabItem)?.Header.ToString();

        //                if (selectedTab == MainWindowViewModel.MainWindowModel.LastTab)
        //                    return;

        //                if (selectedTab?.IndexOf("Info", StringComparison.InvariantCultureIgnoreCase) == 0)
        //                {
        //                    MainWindowViewModel.MainWindowModel.LastTab = "Info";
        //                    MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByInfo;
        //                }
        //                else
        //                {
        //                    MainWindowViewModel.MainWindowModel.LastTab = "Error";
        //                    MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByError;
        //                }
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //        }
        //        else
        //            MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByNetwork;
        //    }
        //    catch (Exception ex)
        //    {
        //        MainWindowViewModel.MainWindowModel.LoggerCollection.Filter += MainWindowViewModel.FilterByNetwork;
        //        ex.DebugLog();
        //    }
        //}


        //private async Task IsCheck()
        //{
        //    try
        //    {
        //        var key = SocinatorKeyHelper.GetKey();

        //        var networks = await UtilityManager.LogIndividualNetworksExceptions(key.FatalErrorMessage);

        //        if (networks.Count <= 1)
        //        {


        //            if (!Application.Current.Dispatcher.CheckAccess())
        //            {
        //                Application.Current.Dispatcher.Invoke(() =>
        //                {
        //                    Application.Current.Shutdown();
        //                    Process.GetCurrentProcess().Kill();
        //                });
        //            }
        //            else
        //            {
        //                Application.Current.Shutdown();
        //                Process.GetCurrentProcess().Kill();
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!Application.Current.Dispatcher.CheckAccess())
        //        {
        //            Application.Current.Dispatcher.Invoke(() =>
        //            {
        //                Application.Current.Shutdown();
        //                Process.GetCurrentProcess().Kill();
        //            });
        //        }
        //        else
        //        {
        //            Application.Current.Shutdown();
        //            Process.GetCurrentProcess().Kill();
        //        }
        //    }
        //}

        //private async Task FatalErrorDiagnosis()
        //{
        //    try
        //    {
        //        string fatalError;
        //        var key = SocinatorKeyHelper.GetKey();
        //        if (key != null)
        //        {
        //            var settings = new MetroDialogSettings()
        //            {
        //                DefaultText = string.IsNullOrEmpty(key.FatalErrorMessage) ? "" : key.FatalErrorMessage,
        //                AffirmativeButtonText = "Validate"
        //            };
        //            while (true)
        //            {
        //                fatalError = await this.ShowInputAsync("Socinator", "License", settings);
        //                if (await IsProcessFatalError(fatalError))
        //                    continue;
        //                else break;
        //            }
        //        }
        //        else
        //            while (true)
        //            {
        //                fatalError = await this.ShowInputAsync("Socinator", "License");
        //                if (await IsProcessFatalError(fatalError))
        //                    continue;
        //                else break;
        //            }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //private async Task<bool> DiagnoseFatalError(string fatalError)
        //{
        //    var controller = await DialogCoordinator.Instance.ShowProgressAsync(this, "Hang On! Checking your License status",
        //        "this will take few moments...");
        //    controller.SetIndeterminate();
        //    _fatalError = fatalError;
        //    var networks = await UtilityManager.LogIndividualNetworksExceptions(_fatalError);

        //    if (networks == null)
        //    {
        //        await controller.CloseAsync();
        //        return await DiagnoseFatalError(fatalError);
        //    }
        //    if (networks.Count <= 1)
        //    {
        //        Close();
        //        await controller.CloseAsync();
        //        await FatalErrorDiagnosis();
        //        return true;
        //    }


        //    _strategies = new DominatorAccountViewModel.AccessorStrategies
        //    {
        //        ActionCheckAccount = AccountStatusChecker,
        //        AccountBrowserLogin = AccountBrowserLogin,
        //        _determine_available = (SocialNetworks s) => _availableNetworks.Contains(s),
        //        _inform_warnings = GlobusLogHelper.log.Warn,
        //        action_UpdateFollower = AccountUpdate,
        //        EditProfile = EditProfile,
        //        RemovePhoneVerification = RemovePhoneVerification
        //    };
        //    DominatorCores.DominatorCoreBuilder.Strategies = _strategies;

        //    var fatalErrorHandler = new DominatorHouseCore.Models.FatalErrorHandler
        //    {
        //        FatalErrorMessage = fatalError,
        //        FatalErrorAddedDate = DateTime.Now,
        //        ErrorNetworks = networks
        //    };
        //    SocinatorKeyHelper.SaveKey(fatalErrorHandler);
        //    FeatureFlags.Check("SocinatorInitializer", SocinatorInitializer);
        //    await controller.CloseAsync();
        //    return true;
        //}

        //private void RemovePhoneVerification(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            var profileFactory = SocinatorInitialize
        //                .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
        //                .GetNetworkCoreFactory().ProfileFactory;
        //            profileFactory.RemovePhoneVerification(dominatorAccountModel);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //private void EditProfile(DominatorAccountModel dominatorAccountModel)
        //{
        //    try
        //    {
        //        Task.Factory.StartNew(() =>
        //        {
        //            var profileFactory = SocinatorInitialize
        //                .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
        //                .GetNetworkCoreFactory().ProfileFactory;
        //            profileFactory.EditProfile(dominatorAccountModel);
        //        });
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //private async Task<bool> IsProcessFatalError(string fatalError)
        //{
        //    if (!string.IsNullOrEmpty(fatalError) && await DiagnoseFatalError(fatalError))
        //        return false;
        //    else if (fatalError == null)
        //        Close();
        //    else
        //    {
        //        if (this.ShowModalMessageExternal("License", "Please validate Socinator !!", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
        //            return true;
        //        else
        //            Close();
        //    }

        //    return false;
        //}


        //public ObservableCollection<string> Languages
        //{
        //    get
        //    {
        //        return _languages;
        //    }
        //    set
        //    {
        //        _languages = value;
        //        OnPropertyChanged(nameof(Languages));
        //    }

        //}
        //public ObservableCollection<TabItemTemplates> TabItems
        //{
        //    get
        //    {
        //        return _tabItems;
        //    }
        //    set
        //    {
        //        _tabItems = value;
        //        OnPropertyChanged(nameof(TabItems));
        //    }

        //}

        //private int _selectedViewIndex;

        //public int SelectedViewIndex
        //{
        //    get
        //    {
        //        return _selectedViewIndex;
        //    }
        //    set
        //    {
        //        _selectedViewIndex = value;
        //        OnPropertyChanged(nameof(SelectedViewIndex));
        //    }
        //}

        //private int _selectedNetworkIndex;

        //public int SelectedNetworkIndex
        //{
        //    get
        //    {
        //        return _selectedNetworkIndex;
        //    }
        //    set
        //    {
        //        _selectedNetworkIndex = value;
        //        OnPropertyChanged(nameof(SelectedNetworkIndex));
        //    }
        //}

        //private static PerformanceCounter PerformanceCounter { get; }
        //    = new PerformanceCounter("Memory", "Available MBytes");

        //private static ManagementObject Processor { get; }
        //    = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        //public HashSet<SocialNetworks> AvailableNetworks
        //{
        //    get
        //    {
        //        return _availableNetworks;
        //    }
        //    set
        //    {
        //        _availableNetworks = value;
        //        OnPropertyChanged(nameof(AvailableNetworks));
        //    }
        //}

        //public Dock TabDock
        //{
        //    get
        //    {
        //        return _tabDock;
        //    }
        //    set
        //    {
        //        _tabDock = value;
        //        OnPropertyChanged(nameof(TabDock));
        //    }
        //}
        //private void SocinatorInitializer()
        //{
        //    try
        //    {
        //        var streamq = Assembly.GetExecutingAssembly()
        //            .GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt");
        //        var accountCustomControl =
        //            AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, _strategies);

        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            JobManager.AddJob(() => InitializeJobCores(_fatalError), x => x.ToRunNow());
        //        });

        //        //Init UI delegates            
        //        CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
        //            DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg) ==
        //            MessageDialogResult.Affirmative;

        //        TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;

        //        ConfigFileManager.ApplyTheme();

        //        var performanceTask = new Task(StartbindMemory,
        //            TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
        //        performanceTask.Start();

        //        TabSwitcher.ChangeTabIndex = (mainTabIndex, subTabIndex) =>
        //        {
        //            SelectedViewIndex = mainTabIndex;

        //            if (subTabIndex == null)
        //                return;

        //            var selectedTabObject = (MainTabControl.SelectedContent as TabItemTemplates)?.Content.Value;

        //            ((dynamic)selectedTabObject)?.setIndex((int)subTabIndex);
        //        };

        //        // Go to campaign from respective module after campaign saved
        //        TabSwitcher.GoToCampaign = ()
        //            => SelectedViewIndex =
        //                TabItems.FindIndex(x => x.Title == FindResource("LangKeyCampaigns").ToString());

        //        Closed += (o, e) => Process.GetCurrentProcess().Kill();
        //    }
        //    catch (AggregateException ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}


        //private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        //{
        //    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
        //    {
        //        SelectedViewIndex = index;
        //        SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);

        //    }
        //    else
        //    {
        //        SelectedViewIndex = index;

        //        DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
        //        SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);


        //        //GlobusLogHelper.log.Info("Goto Tools options only for social mode !");
        //        //NetworkSelectionChanges("Social");
        //        //SelectedViewIndex = index;
        //        //SocialAutoActivity.NewAutoActivityObject(network, selectedAccount);
        //    }
        //}

        //private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    MainWindowViewModel.NetworkSelectionChanges(cmbSocialNetwork.SelectedItem.ToString());
        //}

        //private void NetworkSelectionChanges(string network)
        //{
        //    try
        //    {
        //        if (MainTabControl == null)
        //            return;
        //        TabDock = Dock.Top;
        //        var selectedSocialNetwork =
        //            (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
        //        if (selectedSocialNetwork == SocialNetworks.Social)
        //            TabDock = Dock.Left;
        //        TabInitialize(selectedSocialNetwork);
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}

        //public void TabInitialize(SocialNetworks network)
        //{
        //    try
        //    {
        //        var tabHandler = SocinatorInitialize.GetSocialLibrary(network).GetNetworkCoreFactory().TabHandlerFactory;
        //        if (tabHandler == null)
        //            return;
        //        TabItems = new ObservableCollection<TabItemTemplates>(tabHandler.NetworkTabs);
        //        Title = tabHandler.NetworkName;
        //        SelectedViewIndex = 0;
        //        tabHandler.UpdateAccountCustomControl(network);
        //        SocinatorInitialize.SetAsActiveNetwork(network);
        //    }
        //    catch (Exception ex)
        //    {
        //        TabDock = Dock.Left;

        //        DialogCoordinator.Instance.ShowModalMessageExternal(this, "Fatal Error",
        //            $"Please purchase access of {network} automation features!");
        //        ex.DebugLog();
        //        SelectedNetworkIndex = 0;
        //    }
        //}

        //private void TabItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    try
        //    {
        //        var textBlockDetails = (FrameworkElement)sender as TextBlock;

        //        if (textBlockDetails == null)
        //            return;

        //        if (textBlockDetails.Text == FindResource("LangKeyAccountsActivity").ToString())
        //        {
        //            DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);

        //            // var accountUi = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Social).GetNetworkCoreFactory()
        //            //    .AccountUserControlTools;
        //            //accountUi.GetStartupToolsView();
        //        }
        //        if (textBlockDetails.Text == FindResource("LangKeyPublisher").ToString())
        //        {
        //            PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
        //        }
        //        if (textBlockDetails.Text == FindResource("LangKeyAccountsManager").ToString())
        //        {
        //            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
        //        }
        //        if (textBlockDetails.Text == FindResource("LangKeySociopublisher").ToString())
        //        {
        //            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherDefaultPage.Instance();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }


        //}

        //private void ActivityLog_OnMouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    if (MainGrid.RowDefinitions[2].Height.Value <= 200 && MainGrid.RowDefinitions[2].Height.Value > 45)
        //        MainGrid.RowDefinitions[2].Height = new GridLength(45);
        //    else
        //        MainGrid.RowDefinitions[2].Height = new GridLength(200);
        //}


        //public void InitializeJobCores(string license)
        //{

        //    try
        //    {
        //        var streamq = Assembly.GetExecutingAssembly()
        //            .GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt");
        //        using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("DominatorHouse.RevisionHistory.revisionhistory.txt"))
        //        {
        //            TextReader tr = new StreamReader(stream);
        //            ConstantVariable.Revision = tr.ReadToEnd();
        //        }
        //        ThreadFactory.Instance.Start(() =>
        //            {
        //                var nextDayTime = DateTime.Now.AddDays(1);

        //                JobManager.AddJob(() => InitializeJobCores(license),
        //                    x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
        //                        .AndEvery(1).Days());
        //            });

        //        AvailableNetworks = SocinatorInitialize.AvailableNetworks;
        //        var to_remove = new List<SocialNetworks>();
        //        FeatureFlags.UpdateFeatures();
        //        foreach (var network in AvailableNetworks)
        //        {
        //            FeatureFlags.Check(network.ToString(), () =>
        //            {
        //                try
        //                {
        //                    var networkNamespace = SocinatorInitialize.GetNetworksNamespace(network);
        //                    var networkAssembly = Assembly.Load(networkNamespace);

        //                    #region Network Functionality

        //                    var networkFullNameSpace = $"{networkNamespace}.Factories.{network}NetworkCollectionFactory";
        //                    var networkType = networkAssembly.GetType(networkFullNameSpace);
        //                    // is this a correct type?
        //                    if (typeof(INetworkCollectionFactory).IsAssignableFrom(networkType))
        //                    {
        //                        INetworkCollectionFactory networkCoreFactory;
        //                        var constructors = networkType.GetConstructors();
        //                        // do we have a constructor taking a strategy object?
        //                        var selectedConstructor = constructors.FirstOrDefault(ci =>
        //                        {
        //                            var pars = ci.GetParameters();
        //                            return pars.Length == 1 && pars[0].ParameterType ==
        //                               typeof(DominatorAccountViewModel.AccessorStrategies);
        //                        });
        //                        if (selectedConstructor != default(ConstructorInfo))
        //                        {
        //                            networkCoreFactory =
        //                                (INetworkCollectionFactory)selectedConstructor.Invoke(new object[] { _strategies });
        //                        }
        //                        else
        //                        {
        //                            // if not, do we have a constructor with no parameters?
        //                            selectedConstructor = constructors.First(ci => ci.GetParameters().Length == 0);
        //                            networkCoreFactory = (INetworkCollectionFactory)selectedConstructor.Invoke(null);
        //                        }
        //                        SocinatorInitialize.SocialNetworkRegister(networkCoreFactory, network);
        //                    }

        //                    #endregion

        //                    #region Publisher Functionality

        //                    try
        //                    {
        //                        var publisherFullNameSpace = $"{networkNamespace}.Factories.{network}PublisherCollectionFactory";
        //                        var publisherType = networkAssembly.GetType(publisherFullNameSpace);

        //                        if (!typeof(IPublisherCollectionFactory).IsAssignableFrom(publisherType))
        //                            return;

        //                        var constructors = publisherType.GetConstructors();
        //                        var selectedConstructor = constructors.First(ci => ci.GetParameters().Length == 0);
        //                        var publisherCoreFactory = (IPublisherCollectionFactory)selectedConstructor.Invoke(null);
        //                        PublisherInitialize.SaveNetworkPublisher(publisherCoreFactory, network);
        //                    }
        //                    catch (Exception ex)
        //                    {
        //                        ex.DebugLog();
        //                    }

        //                    #endregion

        //                }
        //                catch (AggregateException ex)
        //                {
        //                    Console.WriteLine(ex.Message);
        //                }
        //                catch (Exception ex)
        //                {
        //                    to_remove.Add(network);
        //                    ex.DebugLog();
        //                }
        //            });
        //        }

        //        AvailableNetworks.ExceptWith(to_remove);

        //        FeatureFlags.UpdateFeatures();

        //        var softWareSettings = new DominatorHouse.Utilities.SoftwareSettings();
        //        ThreadFactory.Instance.Start(() => { softWareSettings.InitializeOnLoadConfigurations(_strategies); });

        //        var softWareSetting = new DominatorHouseCore.Settings.SoftwareSettings();
        //        ThreadFactory.Instance.Start(() => { softWareSetting.InitializeOnLoadConfigurations(); });

        //        // For Every day backup
        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            DirectoryUtilities.DeleteOldLogsFile();
        //            //DirectoryUtilities.Compress();
        //        });

        //        #region Publisher

        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            PublisherInitialize.GetInstance.PublishCampaignInitializer();
        //            PublishScheduler.ScheduleTodaysPublisher();
        //            PublishScheduler.UpdateNewGroupList();
        //        });

        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            var publisherPostFetcher = new PublisherPostFetcher();
        //            publisherPostFetcher.StartFetchingPostData();
        //        });

        //        ThreadFactory.Instance.Start(() =>
        //            {
        //                var deletionPostlist =
        //                GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
        //                    .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
        //                deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
        //            });

        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }

        //    try
        //    {
        //        ThreadFactory.Instance.Start(() =>
        //        {
        //            JobManager.AddJob(async () => await IsCheck(),
        //                x => x.ToRunOnceAt(DateTime.Now.AddHours(1))
        //                    .AndEvery(1).Hours());
        //        });
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    catch (AggregateException ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //} 
        #endregion

    }
}