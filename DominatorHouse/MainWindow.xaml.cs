#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.Publisher;
using GramDominatorUI.TabManager;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using DominatorHouseCore.Utility;
using GramDominatorUI.TabManager;

#endregion

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILoggableWindow
    {

        public List<TabItemTemplates> TabItems { get; set; }

        // Bring all the performance bottlenecks to here. Actually MainWindow class should 
        // not be bothered about performance counter or management object. 
        // TODO: Fix to conform to SRP.
        private static string s_RamSizeOfCurrentComputer = getRAMsize();
        private static PerformanceCounter objPerformanceCounter = new PerformanceCounter("Memory", "Available MBytes");
        private static ManagementObject processor = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        public MainWindow()
        {
            DominatorHouseInitializer.Init(this,
                DominatorJobProcessFactory.Instance,
                DominatorScraperFactory.Instance,
                SocialNetworks.Social);

            InitializeComponent();

            MainTabControl.ItemsSource = InitializeAllTabs();
            TabSwitcher.ChangeTabIndex = ChangeTabIndex;

            GlobusLogHelper.log.Info("Welcome to Dominator social");
            Loaded += (o, e) => GlobusLogHelper.log.Info("Welcome to Dominator social");
             TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;
            Task performanceTask = new Task(() => StartbindMemory(),
            TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            performanceTask.Start();

#if SKIP_DELAYS
            cmbSocialNetwork.SelectedIndex = 1;     // Go to instagram
#endif
            #region commeted
            //Task.Factory.StartNew(() =>
            //{
            //    DateTime NextDayTime = DateTime.Now.AddDays(1);
            //    accountManagerViewModel.InitialAccountDetails();

            //    JobManager.AddJob(() =>
            //        accountManagerViewModel.InitialAccountDetails(),
            //        x => x.ToRunOnceAt(new DateTime(NextDayTime.Year, NextDayTime.Month, NextDayTime.Day,
            //            0, 0, 1)
            //        ).AndEvery(1).Days());
            //}); 
            #endregion
            DialogParticipation.SetRegister(this, this);
            Closed += (o, e) => Process.GetCurrentProcess().Kill();
        }


        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            MainTabControl.SelectedIndex = index;
            SocialAutoActivity.NewAutoActivityObject(network, selectedAccount);
        }

        private void ChangeTabIndex(int mainTabIndex, int? subTabIndex = null)
        {
            MainTabControl.SelectedIndex = mainTabIndex;

            if (subTabIndex == null) return;
            

            // NOTE: Works for instagram tabs
            switch (mainTabIndex)
            {
                case 1:
                    GrowFollowersTab.GetSingeltonObjectGrowFollowersTab().setIndex((int)subTabIndex);
                    break;
                case 2:
                    InstaPosterTab.GetSingeltonObjectInstaPosterTab().setIndex((int)subTabIndex);
                    break;

                case 3:
                    InstachatTab.GetSingeltonObjectInstachatTab().setIndex((int)subTabIndex);
                    break;
                case 4:
                    InstaLikerInstaCommenterTab.GetSingeltonObjectInstaLikerInstaCommenterTab().setIndex((int)subTabIndex);
                    break;

                case 5:
                    InstaScrapeTab.GetSingeltonObjectInstaScrapeTab().setIndex((int)subTabIndex);
                    break;
            }

        }

        public void LogText(string message, bool error)
        {
            if (!error)
                GlobusLogHelper.LogTextToList(InfoLogger, message);
            else
                GlobusLogHelper.LogTextToList(ErrorLogger, message);
        }

        public static MainWindow objMainWindowRef = null;

        #region commented for now
        //private void InitializeTabs()
        //{
        //    SocialNetworks socialNetwork = SocialNetworks.Social;
        //    switch (socialNetwork)
        //    {
        //        case SocialNetworks.Instagram:
        //            GramDominatorUI.MainWindow gramDominator = new GramDominatorUI.MainWindow();
        //            TabItems = gramDominator.InitializeAllTabs();
        //            break;
        //        case SocialNetworks.Twitter:

        //            //TwtDominatorUI.MainWindow twtDominator = new TwtDominatorUI.MainWindow();
        //            //TabItems = twtDominator.InitializeAllTabs();
        //            break;
        //        case SocialNetworks.Social:
        //            TabItems = InitializeAllTabs();
        //            this.Title = "Dominator - All in One";
        //            break;
        //        default:

        //            break;
        //    }

        //    NormalModeTab.ItemsSource = TabItems;
        //    var vv = NormalModeTab.SelectedContent as UserControl;
        //}

        //public void ChangeIndex(int TabControlIndex, int TabIndex)
        //{
        //    NormalModeTab.SelectedIndex = TabControlIndex;
        //    //string item = (NormalModeTab.SelectedItem as TabItemViewModel).Title;
        //    switch (TabControlIndex)
        //    {
        //        case 1:
        //            GrowFollowersTab objGrowFollowersTab = GrowFollowersTab.GetSingeltonObjectGrowFollowersTab();
        //            objGrowFollowersTab.setIndex(TabIndex);
        //            break;
        //        case 2:
        //            InstaPosterTab objInstaPosterTab = InstaPosterTab.GetSingeltonObjectInstaPosterTab();
        //            objInstaPosterTab.setIndex(TabIndex);
        //            break;

        //        case 3:
        //            InstachatTab.GetSingeltonObjectInstachatTab();
        //            break;
        //        case 4:
        //            var objInstaLikerInstaCommenterTab = InstaLikerInstaCommenterTab.GetSingeltonObjectInstaLikerInstaCommenterTab();
        //            objInstaLikerInstaCommenterTab.setIndex(TabIndex);
        //            break;
        //        case 5:
        //            InstaScrapeTab objInstaScrapeTab = InstaScrapeTab.GetSingeltonObjectInstaScrapeTab();
        //            objInstaScrapeTab.setIndex(TabIndex);
        //            break;
        //        case 6:
        //            //campaign
        //            break;
        //    }
        //}

        //public void SelectTab(int mainTabindex)
        //{
        //    NormalModeTab.SelectedIndex = mainTabindex;
        //} 
        #endregion

        async private void StartbindMemory()
        {
            while (true)
            {
                string Availablememory = getMemoryUsage().ToString();
                string CPUUsage = getCPUUsage();

                // WPF items can only be modified in the UI thread.
                try
                {
                    Dispatcher.Invoke(() =>
                           {
                               lbl_Datetime.Text = " : " + DateTime.Now.ToString();
                               lbl_LoadedMemory.Text = " " + s_RamSizeOfCurrentComputer;
                               lbl_Availablememory.Text = " " + Availablememory + "  MB";
                               lbl_CPUUsage.Text = " " + CPUUsage + " % ";
                           });
                }
                catch (Exception ex)
                {

                    Console.WriteLine();
                }

                await Task.Delay(100);
            }
        }





        /// <summary>
        /// Getting Ram size
        /// </summary>
        /// <returns></returns>
        private static string getRAMsize()
        {
            ManagementClass objManagementClass = new ManagementClass("Win32_ComputerSystem");
            ManagementObjectCollection objManagementObjectCollection = objManagementClass.GetInstances();
            foreach (ManagementObject item in objManagementObjectCollection)
            {
                return Convert.ToString(Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0)) + " MB";
            }

            return "0 MB";
        }

        /// <summary>
        /// Getting CPU Usages
        /// </summary>
        /// <returns></returns>
        private string getCPUUsage()
        {
            try
            {
                processor.Get();
                return processor.Properties["PercentProcessorTime"].Value.ToString();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Getting Memory Usages
        /// </summary>
        /// <returns></returns>
        private double getMemoryUsage()
        {
            double memAvailable = (double)objPerformanceCounter.NextValue();
            return memAvailable;
        }


        //public void NormalMode()
        //{
        //    NormalModeTab.Visibility = System.Windows.Visibility.Visible;
        //    MainTabControl.Visibility = System.Windows.Visibility.Collapsed;
        //    btnAccountGrowthMode.Content = "Switch to Account Growth Mode";
        //    btnAccountGrowthMode.Name = "btnAccountGrowthMode";
        //}

        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.Title = "Dominator - All in One";

            if (MainTabControl == null)
                return;

            SocialNetworks socialNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), (cmbSocialNetwork.SelectedItem as ComboBoxItem).Content.ToString());
            MainTabControl.TabStripPlacement = Dock.Top;

            switch (socialNetwork)
            {
                case SocialNetworks.Instagram:
                    GramDominatorUI.MainWindow gramDominator = new GramDominatorUI.MainWindow();
                    TabItems = gramDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Instagram.ToString() + " Dominator";

                    break;

                case SocialNetworks.Twitter:
                    TwtDominatorUI.MainWindow twtDominator = new TwtDominatorUI.MainWindow();
                    TabItems = twtDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Twitter.ToString() + " Dominator";
                    break;
                case SocialNetworks.Pinterest:
                    PinDominator.MainWindow pinDominator = new PinDominator.MainWindow();
                    TabItems = pinDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Pinterest.ToString() + " Dominator";
                    break;
                case SocialNetworks.Social:
                    MainTabControl.TabStripPlacement = Dock.Left;
                    TabItems = InitializeAllTabs();
                    this.Title = "Dominator - All in One";
                    break;
                case SocialNetworks.Quora:
#warning    UNCOMMENT LINES BELOW WHEN COMPILED
                    //QuoraDominator.MainWindow quoraDominator = new QuoraDominator.MainWindow();
                    //TabItems = quoraDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Quora.ToString() + " Dominator";
                    break;

                    this.Title = "Dominator - All in One";
                    break;
            }

            MainTabControl.ItemsSource = TabItems;
            MainTabControl.SelectedIndex = 0;

        }

        public List<TabItemTemplates> InitializeAllTabs()
        {
            AccountCustomControl AccountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);

            AccountCustomControl.DominatorAccountViewModel.action_CheckAccount = action_CheckAccount;

            return new List<TabItemTemplates>
            {

                new TabItemTemplates
                {
                    Title =FindResource("langAccountsManager").ToString(),

                    Content = new Lazy<UserControl>(() => AccountCustomControl),

                },
                new TabItemTemplates
                {
                Title=FindResource("langDashBoard").ToString(),
                //   Content=new Lazy<UserControl>(()=>new DashBoard())
                },
                //new TabItemTemplates
                //{
                //Title=FindResource("langAutoActivity").ToString(),
                //Content=new Lazy<UserControl>(()=>new ToolTabs())
                //},
                new TabItemTemplates
                {
                    Title=FindResource("langAutoActivity").ToString(),
                    Content=new Lazy<UserControl>(()=>DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social))
                    //Content=new Lazy<UserControl>(()=> new HomeAutoActivity())
                },
                new TabItemTemplates
                {
                Title=FindResource("langPublisher").ToString(),
                Content=new Lazy<UserControl>(Home.GetSingletonHome)
                },
                new TabItemTemplates
                {
                Title=FindResource("langProxyManager").ToString(),
                Content=new Lazy<UserControl>(()=>new ProxyManager())
                },
                new TabItemTemplates
                {
                Title=FindResource("langSettings").ToString(),
                Content=new Lazy<UserControl>(()=>new DominatorHouse.Social.Settings.View.Home())
                },
                new TabItemTemplates
                {
                Title=FindResource("langOtherConfigurations").ToString(),
                //  Content=new Lazy<UserControl>(()=>new OtherConfiguration())
                }

                //HomeAutoActivity
            };

        }

        private void TabItem_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var textBlockDetails = ((FrameworkElement)sender) as TextBlock;

            if (textBlockDetails == null)
                return;

            if (textBlockDetails.Text == FindResource("langAutoActivity").ToString())
            {
                DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
            }
        }


        private void ActivityLog_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.RowDefinitions[2].Height.Value <= 200 && MainGrid.RowDefinitions[2].Height.Value > 25)
                MainGrid.RowDefinitions[2].Height = new GridLength(25);
            else
                MainGrid.RowDefinitions[2].Height = new GridLength(200);
        }

        bool IsClickedFromMainWindow = true;

        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickedFromMainWindow)
            {
                Dialog dialog = new Dialog();
                Window ActivityLogWindow = dialog.GetMetroWindow(sender, "Activity Log");
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

        public void action_CheckAccount(DominatorAccountModel dominatorAccountModel)
        {
            switch (dominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Pinterest:
                    //PinDominatorCore.PDViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.Instagram:
                    GramDominatorCore.GDViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.Twitter:
                    //TwtDominatorCore.TDViewModel.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
            }

        }


      

    }
}
