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
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.Social.AutoActivity.Views;
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
using EmbeddedBrowser;
// using EmbeddedBrowser;
using FluentScheduler;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

#endregion

namespace DominatorHouse
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

        private bool IsClickedFromMainWindow { get; set; } = true;

        private DominatorAccountViewModel.AccessorStrategies _strategies;

        public MainWindow()
        {

            _strategies = new DominatorAccountViewModel.AccessorStrategies
            {
                ActionCheckAccount = AccountStatusChecker,
                AccountBrowserLogin = AccountBrowserLogin,
                _determine_available = (SocialNetworks s) => _availableNetworks.Contains(s),
                _inform_warnings = GlobusLogHelper.log.Warn
            };

            DominatorCores.DominatorCoreBuilder.Strategies = _strategies;

            SocinatorInitialize.LogInitializer(this);
            Loaded += (o, e) => GlobusLogHelper.log.Info("Welcome to Socinator!");
            InitializeComponent();
            SocinatorWindow.DataContext = this;
           // FeatureFlags.Check("Instagram", SocinatorInitializer);
            FeatureFlags.Check("SocinatorInitializer", SocinatorInitializer);


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
            var accountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, _strategies);

            Task.Factory.StartNew(() => { JobManager.AddJob(() => InitializeJobCores("License"), x => x.ToRunNow()); });

            //Init UI delegates            
            CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg) == MessageDialogResult.Affirmative;

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
                    TabItems.FindIndex(x => x.Title == FindResource("langCampaigns").ToString());

            DialogParticipation.SetRegister(this, this);

            Closed += (o, e) => Process.GetCurrentProcess().Kill();
        }


        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            SelectedViewIndex = index;
            SocialAutoActivity.NewAutoActivityObject(network, selectedAccount);
        }

        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl == null)
                return;
            TabDock = Dock.Top;
            var selectedSocialNetwork =
                (SocialNetworks)Enum.Parse(typeof(SocialNetworks), cmbSocialNetwork.SelectedItem.ToString());
            if (selectedSocialNetwork == SocialNetworks.Social)
                TabDock = Dock.Left;
            TabInitialize(selectedSocialNetwork);
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
                
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Fetal Error",
                    $"Please purchase access of {network} automation features!");

                SelectedNetworkIndex = 0;
            }            
        }

        private void TabItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement)sender as TextBlock;

            if (textBlockDetails == null)
                return;

            if (textBlockDetails.Text == FindResource("langAutoActivity").ToString())
            {
                var accountUi = SocinatorInitialize.GetSocialLibrary(SocialNetworks.Social).GetNetworkCoreFactory()
                    .AccountUserControlTools;
                accountUi.GetStartupToolsView();
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
                MainGrid.RowDefinitions[2].Height = new GridLength(25);
                activityLogWindow.ShowDialog();
            }
        }

        public void InitializeJobCores(string license)
        {
            Task.Factory.StartNew(() =>
            {
                var nextDayTime = DateTime.Now.AddDays(1);

                JobManager.AddJob(() => InitializeJobCores("License"),
                    x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                        .AndEvery(1).Days());
            });

            AvailableNetworks = SocinatorInitialize.GetAvailableSocialNetworks(license);
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
                                return pars.Length == 1 && pars[0].ParameterType == typeof(DominatorAccountViewModel.AccessorStrategies);
                            });
                            if (selectedConstructor != default(ConstructorInfo))
                            {
                                networkCoreFactory = (INetworkCollectionFactory)selectedConstructor.Invoke(new object[] { _strategies });
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
                    catch (Exception ex)
                    {
                        to_remove.Add(network);
                        ex.DebugLog();
                    }
                });
            }

            AvailableNetworks.ExceptWith(to_remove);

            var accountDetails = AccountsFileManager.GetAll();

            foreach (var account in accountDetails)
                foreach (var modulesConfiguration in account.ActivityManager.LstModuleConfiguration)
                {
                    DominatorScheduler.ScheduleTodayJobs(account, account.AccountBaseModel.AccountNetwork,
                        modulesConfiguration.ActivityType);
                    DominatorScheduler.ScheduleForEachModule(modulesConfiguration.ActivityType, account,
                        account.AccountBaseModel.AccountNetwork);
                }
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            var accountUpdateFactory = SocinatorInitialize
                .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory().AccountUpdateFactory;
            accountUpdateFactory.CheckStatus(dominatorAccountModel);
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
    }
}