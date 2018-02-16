#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Management;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using GramDominatorCore.GDLibrary;
using GramDominatorCore.GDUtility;
using GramDominatorCore.GDViewModel.Accounts;

using GramDominatorUI.TabManager;
using MahApps.Metro.Controls;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorUIUtility.CustomControl;
using NLog;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic;
using DominatorUIUtility.Views.Publisher;
using GramDominatorUI.GDViews.SocialProfiles;

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
            DominatorHouseInitializer.Init(this, DominatorJobProcessFactory.Instance, SocialNetworks.Social);

            InitializeComponent();

            AccountGrowthModeTab.ItemsSource = InitializeAllTabs();

            GlobusLogHelper.log.Info("Welcome to Dominator social");
            Loaded += (o, e) => GlobusLogHelper.log.Info("Welcome to Dominator social");

            Task performanceTask = new Task(() => StartbindMemory(),
                TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            performanceTask.Start();

            Closed += (o, e) => Process.GetCurrentProcess().Kill();
        }


        public void LogText(string message, bool error)
        {
            if (!error)
                GlobusLogHelper.LogTextToList(InfoLogger, message);
            else
                GlobusLogHelper.LogTextToList(ErrorLogger, message);
        }


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





        private void ActivityLog_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            //if(Logger.Visibility == Visibility.Collapsed)
            //Logger.Visibility = Visibility.Visible;
            //else
            //{
            //    Logger.Visibility = Visibility.Collapsed;

            //}

        }


        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.Title = "Dominator - All in One";

            if (AccountGrowthModeTab == null)
                return;

            SocialNetworks socialNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), (cmbSocialNetwork.SelectedItem as ComboBoxItem).Content.ToString());
            AccountGrowthModeTab.TabStripPlacement = Dock.Top;

            switch (socialNetwork)
            {
                case SocialNetworks.Instagram:
                  
                    GramDominatorUI.MainWindow gramDominator = new GramDominatorUI.MainWindow();
                    TabItems = gramDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Instagram.ToString() + " Dominator";
                    break;
                case SocialNetworks.Twitter:
                    #warning UNCOMMENT LINES BELLOW WHEN COMPILED
                    //TwtDominatorUI.MainWindow twtDominator = new TwtDominatorUI.MainWindow();
                    //TabItems = twtDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.Twitter.ToString() + " Dominator";
                    break;
                case SocialNetworks.PinInterest:
                    //PinDominator.MainWindow pinDominator = new PinDominator.MainWindow();
                    //TabItems = pinDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.PinInterest.ToString() + " Dominator";
                    break;
                case SocialNetworks.Social:
                    AccountGrowthModeTab.TabStripPlacement = Dock.Left;
                    TabItems = InitializeAllTabs();
                    this.Title = "Dominator - All in One";
                    break;

                default:
                    this.Title = "Dominator - All in One";
                    break;
            }

            AccountGrowthModeTab.ItemsSource = TabItems;
            AccountGrowthModeTab.SelectedIndex = 0;

        }


        public List<TabItemTemplates> InitializeAllTabs()
        {
            return new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title =FindResource("langAccountsManager").ToString(),
                    Content = new Lazy<UserControl>(() =>  AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social))
                },
                new TabItemTemplates
                {
                Title=FindResource("langDashBoard").ToString(),
                //   Content=new Lazy<UserControl>(()=>new DashBoard())
                },
                new TabItemTemplates
                {
                Title=FindResource("langModuleConfiguration").ToString(),
                Content=new Lazy<UserControl>(()=>new ToolTabs())
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
                //Content=new Lazy<UserControl>(()=>new ToolTabs())
                },
                new TabItemTemplates
                {
                Title=FindResource("langOtherConfigurations").ToString(),
                //  Content=new Lazy<UserControl>(()=>new OtherConfiguration())
                }


        };
        }

    }


}
