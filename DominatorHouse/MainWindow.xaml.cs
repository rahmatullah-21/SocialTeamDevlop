#region Namespaces
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using EmbeddedBrowser;
// using EmbeddedBrowser;
using FluentScheduler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using Socinator.Social.AutoActivity.Views;

#endregion

namespace Socinator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILoggableWindow, INotifyPropertyChanged
    {
        private static readonly string RamSize = GetRamsize();

        private HashSet<SocialNetworks> _availableNetworks;
        private Dock _tabDock = Dock.Left;
        private ObservableCollection<TabItemTemplates> _tabItems;
        private ObservableCollection<string> _languages;
        private Dictionary<string, CancellationToken> _accountUpdater = new Dictionary<string, CancellationToken>();

        private bool IsClickedFromMainWindow { get; set; } = true;

        private DominatorAccountViewModel.AccessorStrategies _strategies;

        private string _licenseKey;

        public MainWindow()
        {
            try
            {
               

                DialogParticipation.SetRegister(this, this);
                Dispatcher.Invoke(async () => { await LicenseCheck(); });
                _languages = new ObservableCollection<string>();
                _languages.Add("English");
                InitializeComponent();
                SocinatorInitialize.LogInitializer(this);
                SocinatorWindow.DataContext = this;
                Loaded += (o, e) => GlobusLogHelper.log.Info($"Welcome to {ConstantVariable.ApplicationName}!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task LicenseCheck()
        {
            try
            {
                string license;
                var key = SocinatorKeyHelper.GetKey();
                if (key != null)
                {
                    var settings = new MetroDialogSettings()
                    {
                        DefaultText = string.IsNullOrEmpty(key.LicenseKey) ? "" : key.LicenseKey,
                        AffirmativeButtonText = "Validate"
                    };
                    while (true)
                    {
                        license = await this.ShowInputAsync("Socinator", "License", settings);
                        if (await IsValidateAgain(license))
                            continue;
                        else break;
                    }
                }
                else
                    while (true)
                    {
                        license = await this.ShowInputAsync("Socinator", "License");
                        if (await IsValidateAgain(license))
                            continue;
                        else break;
                    }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private async Task<bool> ValidateLicense(string license)
        {
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(this, "Hang On! Checking your license status",
                "this will take few moments...");
            controller.SetIndeterminate();
            _licenseKey = license;
            var networks = await SocinatorInitialize.SetAvailableSocialNetworks(_licenseKey);
            if (networks == null)
            {
                await controller.CloseAsync();
                return await ValidateLicense(license);
            }
            if (networks.Count <= 1)
            {
                Close();
                await controller.CloseAsync();
                await LicenseCheck();
                return true;
            }


            _strategies = new DominatorAccountViewModel.AccessorStrategies
            {
                ActionCheckAccount = AccountStatusChecker,
                AccountBrowserLogin = AccountBrowserLogin,
                _determine_available = (SocialNetworks s) => _availableNetworks.Contains(s),
                _inform_warnings = GlobusLogHelper.log.Warn,
                action_UpdateFollower = AccountUpdate
            };
            DominatorCores.DominatorCoreBuilder.Strategies = _strategies;

            var licenseManager = new DominatorHouseCore.Models.LicenseManager
            {
                LicenseKey = license,
                LicenseAddedDate = DateTime.Now,
                LicensedNetworks = networks
            };
            SocinatorKeyHelper.SaveKey(licenseManager);
            FeatureFlags.Check("SocinatorInitializer", SocinatorInitializer);
            await controller.CloseAsync();
            return true;
        }

        private async Task<bool> IsValidateAgain(string license)
        {
            if (!string.IsNullOrEmpty(license) && await ValidateLicense(license))
                return false;
            else if (license == null)
                Close();
            else
            {
                if (this.ShowModalMessageExternal("License", "Please validate Socinator !!", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
                    return true;
                else
                    Close();
            }

            return false;
        }
        public ObservableCollection<string> Languages
        {
            get
            {
                return _languages;
            }
            set
            {
                _languages = value;
                OnPropertyChanged(nameof(Languages));
            }

        }
        public ObservableCollection<TabItemTemplates> TabItems
        {
            get
            {
                return _tabItems;
            }
            set
            {
                _tabItems = value;
                OnPropertyChanged(nameof(TabItems));
            }

        }

        private int _selectedViewIndex;

        public int SelectedViewIndex
        {
            get
            {
                return _selectedViewIndex;
            }
            set
            {
                _selectedViewIndex = value;
                OnPropertyChanged(nameof(SelectedViewIndex));
            }
        }

        private int _selectedNetworkIndex;

        public int SelectedNetworkIndex
        {
            get
            {
                return _selectedNetworkIndex;
            }
            set
            {
                _selectedNetworkIndex = value;
                OnPropertyChanged(nameof(SelectedNetworkIndex));
            }
        }

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        public HashSet<SocialNetworks> AvailableNetworks
        {
            get
            {
                return _availableNetworks;
            }
            set
            {
                _availableNetworks = value;
                OnPropertyChanged(nameof(AvailableNetworks));
            }
        }

        public Dock TabDock
        {
            get
            {
                return _tabDock;
            }
            set
            {
                _tabDock = value;
                OnPropertyChanged(nameof(TabDock));
            }
        }

        public void LogText(string message, bool error)
        {
            GlobusLogHelper.LogTextToList(!error ? InfoLogger : ErrorLogger, message);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void SocinatorInitializer()
        {

            try
            {
                var accountCustomControl =
                    AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, _strategies);

                Task.Factory.StartNew(() =>
                {
                    JobManager.AddJob(() => InitializeJobCores(_licenseKey), x => x.ToRunNow());
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
                    SelectedViewIndex = mainTabIndex;

                    if (subTabIndex == null)
                        return;

                    var selectedTabObject = (MainTabControl.SelectedContent as TabItemTemplates)?.Content.Value;

                    ((dynamic)selectedTabObject)?.setIndex((int)subTabIndex);
                };

                // Go to campaign from respective module after campaign saved
                TabSwitcher.GoToCampaign = ()
                    => SelectedViewIndex =
                        TabItems.FindIndex(x => x.Title == FindResource("DHlangCampaigns").ToString());

                Closed += (o, e) => Process.GetCurrentProcess().Kill();
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
                SelectedViewIndex = index;
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);

            }
            else
            {
                SelectedViewIndex = index;

                DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);


                //GlobusLogHelper.log.Info("Goto Tools options only for social mode !");
                //NetworkSelectionChanges("Social");
                //SelectedViewIndex = index;
                //SocialAutoActivity.NewAutoActivityObject(network, selectedAccount);
            }
        }

        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            NetworkSelectionChanges(cmbSocialNetwork.SelectedItem.ToString());
        }

        private void NetworkSelectionChanges(string network)
        {
            try
            {
                if (MainTabControl == null)
                    return;
                TabDock = Dock.Top;
                var selectedSocialNetwork =
                    (SocialNetworks)Enum.Parse(typeof(SocialNetworks), network);
                if (selectedSocialNetwork == SocialNetworks.Social)
                    TabDock = Dock.Left;
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
                TabItems = new ObservableCollection<TabItemTemplates>(tabHandler.NetworkTabs);
                Title = tabHandler.NetworkName;
                SelectedViewIndex = 0;
                tabHandler.UpdateAccountCustomControl(network);
                SocinatorInitialize.SetAsActiveNetwork(network);
            }
            catch (Exception ex)
            {
                TabDock = Dock.Left;

                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Fatal Error",
                    $"Please purchase access of {network} automation features!");

                SelectedNetworkIndex = 0;
            }
        }

        private void TabItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                var textBlockDetails = (FrameworkElement)sender as TextBlock;

                if (textBlockDetails == null)
                    return;

                if (textBlockDetails.Text == FindResource("langAutoActivity").ToString())
                {
                    DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);

                    // var accountUi = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Social).GetNetworkCoreFactory()
                    //    .AccountUserControlTools;
                    //accountUi.GetStartupToolsView();
                }
                if (textBlockDetails.Text == FindResource("langPublisher").ToString())
                {
                    PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


        }

        private void ActivityLog_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.RowDefinitions[2].Height.Value <= 200 && MainGrid.RowDefinitions[2].Height.Value > 25)
                MainGrid.RowDefinitions[2].Height = new GridLength(25);
            else
                MainGrid.RowDefinitions[2].Height = new GridLength(200);
        }

        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickedFromMainWindow)
            {
                var dialog = new Dialog();
                var activityLogWindow = dialog.GetMetroWindow(sender, "Activity Log");
                activityLogWindow.Topmost = false;
                IsClickedFromMainWindow = false;
                activityLogWindow.Closing += (senders, events) =>
                {
                    Logger.Children.Remove(RootLayout);
                    Logger.Children.Add(RootLayout);
                    MainGrid.RowDefinitions[2].Height = new GridLength(200);
                    IsClickedFromMainWindow = true;
                };
                MainGrid.RowDefinitions[2].Height = new GridLength(0);
                activityLogWindow.ShowDialog();
            }
        }

        public void InitializeJobCores(string license)
        {
            try
            {
                Task.Factory.StartNew(() =>
                    {
                        var nextDayTime = DateTime.Now.AddDays(1);

                        JobManager.AddJob(() => InitializeJobCores(license),
                            x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                                .AndEvery(1).Days());
                    });

                AvailableNetworks = SocinatorInitialize.AvailableNetworks;
                var to_remove = new List<SocialNetworks>();
                foreach (var network in AvailableNetworks)
                {
                    FeatureFlags.Check(network.ToString(), () =>
                    {
                        try
                        {
                            var networkNamespace = SocinatorInitialize.GetNetworksNamespace(network);
                            var networkAssembly = Assembly.Load(networkNamespace);
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
                                        (INetworkCollectionFactory)selectedConstructor.Invoke(new object[] { _strategies });
                                }
                                else
                                {
                                    // if not, do we have a constructor with no parameters?
                                    selectedConstructor = constructors.First(ci => ci.GetParameters().Length == 0);
                                    networkCoreFactory = (INetworkCollectionFactory)selectedConstructor.Invoke(null);
                                }
                                SocinatorInitialize.SocialNetworkRegister(networkCoreFactory, network);
                            }
                        }
                        catch (AggregateException ex)
                        {

                        }
                        catch (Exception ex)
                        {
                            to_remove.Add(network);
                            ex.DebugLog();
                        }
                    });
                }

                AvailableNetworks.ExceptWith(to_remove);

                var accountDetails = AccountsFileManager.GetAll();

                RunningActivityManager.Initialize(accountDetails);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
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
                Task.Factory.StartNew(() =>
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
                GlobusLogHelper.log.Error(ex.Message);
                //MessageBox.Show(ex.Message);
            }
        }

        #region System Details  

        private async void StartbindMemory()
        {
            while (true)
            {
                var availablememory = GetMemoryUsage().ToString(CultureInfo.InvariantCulture);

                var cpuUsage = GetCpuUsage();

                try
                {
                    Dispatcher.Invoke(() =>
                    {
                        lbl_Datetime.Text = " : " + DateTime.Now.ToString(CultureInfo.InvariantCulture);
                        lbl_LoadedMemory.Text = " " + RamSize;
                        lbl_Availablememory.Text = " " + availablememory + "  MB";
                        lbl_CPUUsage.Text = " " + cpuUsage + " % ";
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

            return "0 MB";
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

        private void SocinatorWindow_OnClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;

            bool isClose = this.ShowModalMessageExternal("Confirmation", "Are you sure to close Socinator?", MessageDialogStyle.AffirmativeAndNegative,
                                 Dialog.SetMetroDialogButton("Yes", "No")) == MessageDialogResult.Affirmative;
            if (isClose)
            {
                Application.Current.Shutdown();
                Process.GetCurrentProcess().Kill();
            }

        }


    }
}