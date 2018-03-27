#region Namespaces

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.DominatorCores;
using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.Publisher;
using EmbeddedBrowser;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FaceDominatorUI.FdCoreLibrary;
using FluentScheduler;
using GramDominatorCore.Factories;
using GramDominatorCore.GDViewModel.Accounts;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TwtDominatorCore.Factories;

#endregion

namespace DominatorHouse
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILoggableWindow , INotifyPropertyChanged
    {
        private static readonly string RamSize = GetRamsize();

        private bool IsClickedFromMainWindow = true;

        public MainWindow()
        {
            DominatorHouseCore.Diagnostics.SocinatorInitialize.LogInitializer(this);
            InitializeComponent();
            SocinatorWindow.DataContext = this;
            SocinatorInitializer();
        }

        private void SocinatorInitializer()
        {           
            var accountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
            accountCustomControl.DominatorAccountViewModel.ActionCheckAccount = AccountStatusChecker;
            accountCustomControl.DominatorAccountViewModel.AccountBrowserLogin = AccountBrowserLogin;

            Task.Factory.StartNew(() => { JobManager.AddJob(() => InitializeJobCores("License"), x => x.ToRunNow()); });
            
            //Init UI delegates            
            CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg,
                    MessageDialogStyle.Affirmative) == MessageDialogResult.Affirmative;

            TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;
            AccountAddUpdate.UpdateGDAccount = AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount;
            //AccountAddUpdate.UpdateQDAccount = QuoraDominatorCore.ViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount;

            ConfigFileManager.ApplyTheme();

            var performanceTask = new Task(StartbindMemory,
                TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            performanceTask.Start();

            TabSwitcher.ChangeTabIndex = (mainTabIndex, subTabIndex) =>
            {
                MainTabControl.SelectedIndex = mainTabIndex;

                if (subTabIndex == null)
                    return;

                var selectedTabObject = (MainTabControl.SelectedContent as TabItemTemplates)?.Content.Value;

                ((dynamic)selectedTabObject)?.setIndex((int)subTabIndex);
            };

            TabSwitcher.GoToCampaign = ()
                => MainTabControl.SelectedIndex =
                    TabItems.FindIndex(x => x.Title == FindResource("langCampaigns").ToString());

           

            DialogParticipation.SetRegister(this, this);

            Closed += (o, e) => Process.GetCurrentProcess().Kill();
        }

        public List<TabItemTemplates> TabItems { get; set; }

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");


        public void LogText(string message, bool error)
        {
            GlobusLogHelper.LogTextToList(!error ? InfoLogger : ErrorLogger, message);
        }


        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            MainTabControl.SelectedIndex = index;
            SocialAutoActivity.NewAutoActivityObject(network, selectedAccount);
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




        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MainTabControl == null)
                return;

            MainTabControl.TabStripPlacement = Dock.Top;

            var selectedSocialNetwork =
                (SocialNetworks) Enum.Parse(typeof(SocialNetworks), cmbSocialNetwork.SelectedItem.ToString());

            if (selectedSocialNetwork == SocialNetworks.Social)
                MainTabControl.TabStripPlacement = Dock.Left;

            TabInitialize(selectedSocialNetwork);
        }

        private void TabItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = (FrameworkElement)sender as TextBlock;

            if (textBlockDetails == null)
                return;

            if (textBlockDetails.Text == FindResource("langAutoActivity").ToString())
                DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
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
                var ActivityLogWindow = dialog.GetMetroWindow(sender, "Activity Log");
                ActivityLogWindow.Topmost = false;
                IsClickedFromMainWindow = false;
                ActivityLogWindow.Closing += (senders, events) =>
                {
                    Logger.Children.Remove(RootLayout);
                    Logger.Children.Add(RootLayout);
                    MainGrid.RowDefinitions[2].Height = new GridLength(200);
                    IsClickedFromMainWindow = true;
                };
                MainGrid.RowDefinitions[2].Height = new GridLength(25);
                ActivityLogWindow.ShowDialog();
            }
        }

        private List<SocialNetworks> _availableNetworks;

        public List<SocialNetworks> AvailableNetworks
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

        public void InitializeJobCores(string license)
        {
            Task.Factory.StartNew(() =>
            {
                var nextDayTime = DateTime.Now.AddDays(1);

                JobManager.AddJob(() => InitializeJobCores("License"),
                    x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                        .AndEvery(1).Days());
            });

            // get all available networks from license          
            AvailableNetworks = new List<SocialNetworks>
            {
                SocialNetworks.Social,
                SocialNetworks.Facebook,
                SocialNetworks.Instagram,
                SocialNetworks.Twitter,
                SocialNetworks.Gplus
            };

            var socialNetworkObject = new List<NetworkCoreLibrary>();

            foreach (var network in AvailableNetworks)
                switch (network)
                {
                    case SocialNetworks.Facebook:
                        socialNetworkObject.Add(FdCoreBuilder.Instance.GetFdCoreObjects());
                        break;
                    case SocialNetworks.Instagram:                        
                        break;
                    case SocialNetworks.Twitter:                       
                        break;
                    case SocialNetworks.Pinterest:
                        break;
                    case SocialNetworks.LinkedIn:
                        break;
                    case SocialNetworks.Reddit:
                        break;
                    case SocialNetworks.Social:
                        socialNetworkObject.Add(DominatorCoreBuilder.Instance.GetDominatorCoreObjects());
                        break;
                    case SocialNetworks.Quora:
                        break;
                    case SocialNetworks.Gplus:
                        break;
                    case SocialNetworks.Youtube:
                        break;
                    case SocialNetworks.Tumblr:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

            SocinatorInitialize.SocialNetworkRegister(socialNetworkObject);

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

        public void TabInitialize(SocialNetworks network)
        {
            var tabHandler = SocinatorInitialize.GetSocialLibrary(network).TabHandlerFactory;
            MainTabControl.ItemsSource = TabItems = tabHandler.NetworkTabs;
            Title = tabHandler.NetworkName;
            MainTabControl.SelectedIndex = 0;
            SocinatorInitialize.SetAsActiveNetwork(network);
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            var accountUpdateFactory = SocinatorInitialize.GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                .AccountUpdateFactory;
            accountUpdateFactory.CheckStatus(dominatorAccountModel);
        }

        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            var browserWindow = new BrowserWindow(dominatorAccountModel);
            browserWindow.Show();
        }
    }
}