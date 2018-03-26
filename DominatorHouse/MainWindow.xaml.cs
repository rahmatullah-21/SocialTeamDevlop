#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Management;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.Publisher;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using DominatorHouseCore.Utility;
using DominatorHouseCore.BusinessLogic;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorUIUtility;
using EmbeddedBrowser;
using FaceDominatorCore.FDFactories;
using FaceDominatorCore.FDLibrary;
using FaceDominatorCore.FDLibrary.FdProcesses;
using FluentScheduler;
using GplusDominatorCore.GpDFactories;
using GramDominatorCore.Factories;
using GramDominatorUI.TabManager;
using PinDominatorCore.Factories;
using TwtDominatorCore.Factories;

#endregion

namespace DominatorHouse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow, ILoggableWindow
    {

        public List<TabItemTemplates> TabItems { get; set; }

        private static readonly string RamSize = GetRaMsize();

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        public MainWindow()
        {           
            SocinatorInitialize.LogInitializer(this);

            InitializeComponent();

            MainTabControl.ItemsSource = InitializeAllTabs();

            // Init UI delegates            
            CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>

                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg, MessageDialogStyle.Affirmative) == MessageDialogResult.Affirmative;


           // TabSwitcher.ChangeTabIndex = ChangeTabIndex;
            TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;
            TabSwitcher.SelectMainTab = SelectMainIndex;
            AccountAddUpdate.UpdateGDAccount = GramDominatorCore.GDViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount;
           // AccountAddUpdate.UpdateQDAccount = QuoraDominatorCore.ViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount;
                    
            ConfigFileManager.ApplyTheme();


            var performanceTask = new Task(StartbindMemory,TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            performanceTask.Start();

            Task.Factory.StartNew(() =>
            {             
                JobManager.AddJob(() => InitializeJobCores("License"),x=>x.ToRunNow());                                              
            });


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

            //if (subTabIndex == null) return;


            //// NOTE: Works for instagram tabs
            //switch (mainTabIndex)
            //{
            //    case 1:
            //        GrowFollowersTab.GetSingeltonObjectGrowFollowersTab().setIndex((int)subTabIndex);
            //        break;
            //    case 2:
            //        InstaPosterTab.GetSingeltonObjectInstaPosterTab().setIndex((int)subTabIndex);
            //        break;

            //    case 3:
            //        InstachatTab.GetSingeltonObjectInstachatTab().setIndex((int)subTabIndex);
            //        break;
            //    case 4:
            //        InstaLikerInstaCommenterTab.GetSingeltonObjectInstaLikerInstaCommenterTab().setIndex((int)subTabIndex);
            //        break;

            //    case 5:
            //        InstaScrapeTab.GetSingeltonObjectInstaScrapeTab().setIndex((int)subTabIndex);
            //        break;
            //}

        }

        private void SelectMainIndex(int mainTabIndex)
        {
            MainTabControl.SelectedIndex = mainTabIndex;
        }

        public void LogText(string message, bool error)
        {
            GlobusLogHelper.LogTextToList(!error ? InfoLogger : ErrorLogger, message);
        }


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


        private static string GetRaMsize()
        {
            var objManagementClass = new ManagementClass("Win32_ComputerSystem");
            var objManagementObjectCollection = objManagementClass.GetInstances();
            foreach (var item in objManagementObjectCollection)
            {
                return Convert.ToString(Math.Round(Convert.ToDouble(item.Properties["TotalPhysicalMemory"].Value) / 1048576, 0), CultureInfo.InvariantCulture) + " MB";
            }

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
                case SocialNetworks.Facebook:
                    FaceDominatorUI.MainWindow faceDominator = new FaceDominatorUI.MainWindow();
                    TabItems = faceDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Facebook.ToString() + " Dominator";
                    break;
                case SocialNetworks.LinkedIn:
                    //LinkedDominatorUI.MainWindow linkedInDominator = new LinkedDominatorUI.MainWindow();
                    //TabItems = linkedInDominator.InitializeAllTabs();
                    //this.Title = SocialNetworks.LinkedIn.ToString() + " Dominator";
                    break;
                case SocialNetworks.Quora:
                    //QuoraDominator.MainWindow quoraDominator = new QuoraDominator.MainWindow();
                    //TabItems = quoraDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Quora.ToString() + " Dominator";
                    break;
                case SocialNetworks.Reddit:
                    this.Title = SocialNetworks.Reddit.ToString() + " Dominator";
                    break;
                case SocialNetworks.Gplus:
                    GplusDominatorUI.MainWindow gplusDominator = new GplusDominatorUI.MainWindow();
                    TabItems = gplusDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Gplus.ToString() + " Dominator";
                    break;
                case SocialNetworks.Youtube:
                    this.Title = SocialNetworks.Youtube.ToString() + " Dominator";
                    break;
                case SocialNetworks.Social:
                    MainTabControl.TabStripPlacement = Dock.Left;
                    TabItems = InitializeAllTabs();
                    this.Title = "Dominator - All in One";
                    break;
                default:
                    this.Title = "Dominator House";
                    break;
            }

            MainTabControl.ItemsSource = TabItems;
            MainTabControl.SelectedIndex = 0;

        }

        public List<TabItemTemplates> InitializeAllTabs()
     {
            var accountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);

            accountCustomControl.DominatorAccountViewModel.action_CheckAccount = action_CheckAccount;

            accountCustomControl.DominatorAccountViewModel.AccountBrowserLogin = AccountBrowserLogin;

            return new List<TabItemTemplates>
            {

                new TabItemTemplates
                {
                    Title =FindResource("langAccountsManager").ToString(),

                    Content = new Lazy<UserControl>(() => accountCustomControl),

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
                    PinDominatorCore.PDViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.Instagram:
                    GramDominatorCore.GDViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.Twitter:
                    TwtDominatorCore.TDViewModel.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.LinkedIn:
                    // Call your methods to login
                    break;
                case SocialNetworks.Gplus:
                    GplusDominatorCore.GplusViewModel.Accounts.AccountManagerViewModel.GetAccountManagerViewModel().UpdateAccount(dominatorAccountModel);
                    break;
                case SocialNetworks.Facebook:
                    var fdLoginProcess = new FdLoginProcess();
                    fdLoginProcess.CheckLogin(dominatorAccountModel);
                    break;
                case SocialNetworks.Youtube:
                    // Call your methods to login
                    break;
                case SocialNetworks.Quora:
                    // Call your methods to login
                    break;
                case SocialNetworks.Reddit:
                    // Call your methods to login
                    break;
            }

        }




        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            BrowserWindow browserWindow = new BrowserWindow(dominatorAccountModel);
            browserWindow.Show();          
        }



        public void InitializeJobCores(string license)
        {
            Task.Factory.StartNew(() =>
            {
                var nextDayTime = DateTime.Now.AddDays(1);

               JobManager.AddJob(() => InitializeJobCores("License"),
                    x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1)).AndEvery(1).Days());
            });

            // get all available networks from license          
            var availablNetworks = new List<SocialNetworks>
            {
                SocialNetworks.Social,
                SocialNetworks.Facebook,
                SocialNetworks.Instagram,
                SocialNetworks.Twitter
            };

            var socialNetworkObject = new List<SocialNetworkObjects>();
           
            foreach (var network in availablNetworks)
            {
                switch (network)
                {
                    case SocialNetworks.Facebook:
                        socialNetworkObject.Add(new SocialNetworkObjects()
                        {
                            JobProcessFactory = FdJobProcessFactory.Instance,
                            QueryScraperFactory = FdScraperFactory.Instance,
                            Network = SocialNetworks.Facebook
                        });
                        break;
                    case SocialNetworks.Instagram:
                        socialNetworkObject.Add(new SocialNetworkObjects()
                        {
                            JobProcessFactory = GdJobProcessFactory.Instance,
                            QueryScraperFactory = GdScraperFactory.Instance,
                            Network = SocialNetworks.Instagram
                        });
                        break;
                    case SocialNetworks.Twitter:
                        socialNetworkObject.Add(new SocialNetworkObjects()
                        {
                            JobProcessFactory= TdJobProcessFactory.Instance,
                            QueryScraperFactory= TdScraperFactory.Instance,
                            Network = SocialNetworks.Twitter
                        });
                        break;
                    case SocialNetworks.Pinterest:
                        break;
                    case SocialNetworks.LinkedIn:
                        break;
                    case SocialNetworks.Reddit:
                        break;
                    case SocialNetworks.Social:
                        socialNetworkObject.Add(new SocialNetworkObjects()
                        {
                            JobProcessFactory = DominatorJobProcessFactory.Instance,
                            QueryScraperFactory = DominatorScraperFactory.Instance,
                            Network = SocialNetworks.Social
                        });
                        break;
                    case SocialNetworks.Quora:
                        break;
                    case SocialNetworks.Gplus:
                        break;
                    case SocialNetworks.Youtube:

                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            SocinatorInitialize.SocialNetworkRegister(socialNetworkObject);

            var accountDetails = AccountsFileManager.GetAll();

            foreach (var account in accountDetails)
            {
                foreach (var modulesConfiguration in account.ActivityManager.LstModuleConfiguration)
                {                   
                        DominatorScheduler.ScheduleTodayJobs(account, account.AccountBaseModel.AccountNetwork, modulesConfiguration.ActivityType);
                        DominatorScheduler.ScheduleForEachModule(moduleToIgnore: modulesConfiguration.ActivityType, account: account, network: account.AccountBaseModel.AccountNetwork);
                }
            }
        }
    }
}
