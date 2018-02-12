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
using GramDominatorUI.AccountGrowthMode;
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

            //XmlConfigurator.Configure();
            var account = RandomUtilties.GetRandomTexts(10);
            InitializeComponent();
            
            objMainWindowRef = this;
            InitializeTabs();
            GlobusLogHelper.log.Info("Welcome to Dominator social" );        
            Loaded += (o, e) => GlobusLogHelper.log.Info("Welcome to Dominator social");
            
            //AccountManagerViewModel accountManagerViewModel = AccountManagerViewModel.GetAccountManagerViewModel();

            NormalModeTab.ItemsSource = TabItems;
            DominatorScheduler.ChangeTabIndex += ChangeIndex;

            Task performanceTask = new Task(() => StartbindMemory(),
                TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
            performanceTask.Start();

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

            Closed += (o, e) => Process.GetCurrentProcess().Kill();
        }


        public void LogText(string message, bool error)
        {
            if (!error)
                GlobusLogHelper.LogTextToList(InfoLogger, message);
            else
                GlobusLogHelper.LogTextToList(ErrorLogger, message);
        }


        public static MainWindow objMainWindowRef = null;

        private void InitializeTabs()
        {
            SocialNetworks socialNetwork = SocialNetworks.Social;
            switch (socialNetwork)
            {
                case SocialNetworks.Instagram:
                    GramDominatorUI.MainWindow gramDominator = new GramDominatorUI.MainWindow();
                    TabItems = gramDominator.InitializeAllTabs();
                    break;
                case SocialNetworks.Twitter:
                    TwtDominatorUI.MainWindow twtDominator = new TwtDominatorUI.MainWindow();
                    TabItems = twtDominator.InitializeAllTabs();
                    break;
                case SocialNetworks.Social:
                    TabItems = InitializeAllTabs();
                    this.Title = "Dominator - All in One";
                    break;
                default:

                    break;
            }

            NormalModeTab.ItemsSource = TabItems;
            var vv = NormalModeTab.SelectedContent as UserControl;
        }

        public void ChangeIndex(int TabControlIndex, int TabIndex)
        {
            NormalModeTab.SelectedIndex = TabControlIndex;
            //string item = (NormalModeTab.SelectedItem as TabItemViewModel).Title;
            switch (TabControlIndex)
            {
                case 1:
                    GrowFollowersTab objGrowFollowersTab = GrowFollowersTab.GetSingeltonObjectGrowFollowersTab();
                    objGrowFollowersTab.setIndex(TabIndex);
                    break;
                case 2:
                    InstaPosterTab objInstaPosterTab = InstaPosterTab.GetSingeltonObjectInstaPosterTab();
                    objInstaPosterTab.setIndex(TabIndex);
                    break;

                case 3:
                    InstachatTab.GetSingeltonObjectInstachatTab();
                    break;
                case 4:
                    var objInstaLikerInstaCommenterTab = InstaLikerInstaCommenterTab.GetSingeltonObjectInstaLikerInstaCommenterTab();
                    objInstaLikerInstaCommenterTab.setIndex(TabIndex);
                    break;
                case 5:
                    InstaScrapeTab objInstaScrapeTab = InstaScrapeTab.GetSingeltonObjectInstaScrapeTab();
                    objInstaScrapeTab.setIndex(TabIndex);
                    break;
                case 6:
                    //campaign
                    break;
            }
        }

        public void SelectTab(int mainTabindex)
        {
            NormalModeTab.SelectedIndex = mainTabindex;
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


        private void btnAccountGrowthMode_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (btnAccountGrowthMode.Name == "btnAccountGrowthMode")
            {

                AccountGrowthMode();
            }
            else 
            {
                NormalMode();
            }
        }

        public void AccountGrowthMode()
        {
            AccountGrowthModeTab.Children.Clear();
            AccountGrowthModeTab.Children.Add(AccountGrowth.GetSingletonAccountGrowth());
            NormalModeTab.Visibility = System.Windows.Visibility.Collapsed;

            AccountGrowthModeTab.Visibility = System.Windows.Visibility.Visible;

            btnAccountGrowthMode.Content = "Switch to Normal Mode";
            btnAccountGrowthMode.Name = "btnNormalMode";
        }

        public void NormalMode()
        {
            NormalModeTab.Visibility = System.Windows.Visibility.Visible;
            AccountGrowthModeTab.Visibility = System.Windows.Visibility.Collapsed;


            btnAccountGrowthMode.Content = "Switch to Account Growth Mode";
            btnAccountGrowthMode.Name = "btnAccountGrowthMode";
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


        private ICollectionView _infoLoggerCollection;
        public ICollectionView InfoLoggerCollection
        {
            get
            {
                return _infoLoggerCollection;
            }
            set
            {
                if (_infoLoggerCollection != null && value == _infoLoggerCollection)
                    return;

            }
        }

        private ICollectionView _errorLoggerCollection;
        public ICollectionView ErrorLoggerCollection
        {
            get
            {
                return _errorLoggerCollection;
            }
            set
            {
                if (_errorLoggerCollection != null && value == _errorLoggerCollection)
                    return;

            }
        }



        private void cmbSocialNetwork_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            this.Title = "Dominator - All in One";

            if (NormalModeTab == null)
                return;

            SocialNetworks socialNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), (cmbSocialNetwork.SelectedItem as ComboBoxItem).Content.ToString());

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
                case SocialNetworks.PinInterest:
                    //PinDominator.MainWindow pinDominator = new PinDominator.MainWindow();
                    //TabItems = pinDominator.InitializeAllTabs();
                    this.Title = SocialNetworks.PinInterest.ToString() + " Dominator";
                    break;
                case SocialNetworks.Social:
                    TabItems = InitializeAllTabs();
                    this.Title = "Dominator - All in One";
                    break;

                default:
                    this.Title = "Dominator - All in One";
                    break;
            }
            NormalModeTab.ItemsSource = TabItems;
            NormalModeTab.SelectedIndex = 0;
            var vv = NormalModeTab.SelectedContent as UserControl;
        }


        public List<TabItemTemplates> InitializeAllTabs()
        {
            return new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title=FindResource("langAccounts").ToString(),
                    Content=new Lazy<UserControl>(()=>new AccountTabCustomControl())
                }
                //new TabItemTemplates
                //{
                //    Title=FindResource("langGrowFollowers﻿").ToString(),
                //    Content=new Lazy<UserControl>(GrowFollowersTab.GetSingeltonObjectGrowFollowersTab)
                //},
                //new TabItemTemplates
                //{
                //    Title=FindResource("langInstaPoster﻿").ToString(),
                //    Content=new Lazy<UserControl>(InstaPosterTab.GetSingeltonObjectInstaPosterTab)
                //},
                
            };
        }
    }

    #region LogFornetclass
    //public class GlobusLogAppender : log4net.Appender.AppenderSkeleton
    //{

    //    private static readonly object lockerLog4Append = new object();

    //    /// <summary>
    //    /// 
    //    /// </summary>
    //    /// <param name="loggingEvent"></param>
    //    protected override void Append(log4net.Core.LoggingEvent loggingEvent)
    //    {
    //        try
    //        {
    //            string loggerName = loggingEvent.Level.Name;
    //            MainWindow mainWindow = MainWindow.objMainWindowRef;


    //            lock (lockerLog4Append)
    //            {
    //                switch (loggingEvent.Level.Name)
    //                {
    //                    case "DEBUG":
    //                        try
    //                        {

    //                            {
    //                                if (!mainWindow.InfoLogger.Dispatcher.CheckAccess())
    //                                {
    //                                    mainWindow.InfoLogger.Dispatcher.Invoke(new Action(delegate
    //                                    {
    //                                        try
    //                                        {
    //                                            if (mainWindow.InfoLogger.Items.Count > 1000)
    //                                            {
    //                                                mainWindow.InfoLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                            }

    //                                            mainWindow.InfoLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0" + "\r\t" + loggingEvent.RenderedMessage.Replace("\t"," "));
    //                                        }
    //                                        catch (Exception ex)
    //                                        {
                                               

    //                                            GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                                        }

    //                                    }));

    //                                }
    //                                else
    //                                {
    //                                    try
    //                                    {
    //                                        if (mainWindow.InfoLogger.Items.Count > 1000)
    //                                        {
    //                                            mainWindow.InfoLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                        }

    //                                        mainWindow.InfoLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0 " + "\r\t" + loggingEvent.RenderedMessage.Replace("\t", " "));
    //                                    }
    //                                    catch (Exception ex)
    //                                    {
    //                                       GlobusLogHelper.log.Error("Error : 74" + ex.Message);
    //                                    }
    //                                }
    //                            }
    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            Console.WriteLine("Error Case Debug : " + ex.StackTrace);
    //                            Console.WriteLine("Error Case Debug : " + ex.Message);

    //                            GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                        }
    //                        break;
    //                    case "INFO":
    //                        try
    //                        {
    //                            if (loggingEvent.RenderedMessage.Contains("error"))
    //                            {
    //                                GlobusLogHelper.log.Error(loggingEvent.RenderedMessage);
    //                                return;
    //                            }
    //                            if (!mainWindow.InfoLogger.Dispatcher.CheckAccess())
    //                            {
    //                                mainWindow.InfoLogger.Dispatcher.Invoke(new Action(delegate
    //                                {
    //                                    try
    //                                    {
    //                                        if (mainWindow.InfoLogger.Items.Count > 1000)
    //                                        {
    //                                            mainWindow.InfoLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                        }

    //                                        mainWindow.InfoLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage.Replace("\t", " "));
    //                                    }
    //                                    catch (Exception ex)
    //                                    {
    //                                         GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                                    }

    //                                }));

    //                            }
    //                            else
    //                            {
    //                                try
    //                                {
    //                                    if (mainWindow.InfoLogger.Items.Count > 1000)
    //                                    {
    //                                        mainWindow.InfoLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                    }

    //                                    mainWindow.InfoLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0 " + "\t\t" + loggingEvent.RenderedMessage.Replace("\t", " "));
    //                                }
    //                                catch (Exception ex)
    //                                {
    //                                   GlobusLogHelper.log.Error("Error : 75" + ex.Message);
    //                                }
    //                            }

    //                        }
    //                        catch (Exception ex)
    //                        {
                                
    //                           GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                        }
    //                        break;

    //                    case "ERROR":

    //                        #region ERROR
    //                        try
    //                        {
    //                            var messege = loggingEvent.RenderedMessage.Split(new string[] { " at" }, StringSplitOptions.None);
    //                            if (!mainWindow.ErrorLogger.Dispatcher.CheckAccess())
    //                            {
    //                                mainWindow.ErrorLogger.Dispatcher.Invoke(new Action(delegate
    //                                {
    //                                    try
    //                                    {
    //                                        if (mainWindow.ErrorLogger.Items.Count > 1000)
    //                                        {
    //                                            mainWindow.ErrorLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                        }

    //                                        if ((!String.IsNullOrEmpty(messege[0]) && messege[0] != "  ") && !messege[0].Contains("<"))
    //                                            mainWindow.ErrorLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0 " + "\t\t" + "Error : " + "\t\t" + loggingEvent.RenderedMessage.Replace("\t", " "));
    //                                    }
    //                                    catch (Exception ex)
    //                                    {
    //                                        GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                                    }

    //                                }));

    //                            }
    //                            else
    //                            {
    //                                try
    //                                {
    //                                    if (mainWindow.ErrorLogger.Items.Count > 1000)
    //                                    {
    //                                        mainWindow.ErrorLogger.Items.RemoveAt(mainWindow.InfoLogger.Items.Count - 1);
    //                                    }
    //                                    if (!(String.IsNullOrEmpty(messege[0]) && messege[0] == "  ") && !messege[0].Contains("<"))
    //                                        mainWindow.ErrorLogger.Items.Insert(0, loggingEvent.TimeStamp + "\t" + "Gram Dominator 3.0 " + "\t\t" + "Error : " + "\t\t" + loggingEvent.RenderedMessage.Replace("\t", " "));

    //                                }
    //                                catch (Exception ex)
    //                                {
    //                                    GlobusLogHelper.log.Error("Error : 75" + ex.Message);
    //                                }
    //                            }

    //                        }
    //                        catch (Exception ex)
    //                        {
    //                            GlobusLogHelper.log.Error(" Error : " + ex.Message);
    //                        }

    //                        #endregion

    //                        break;
    //                }
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //            // GlobusLogHelper.log.Error("Error : 76" + ex.Message);
    //        }

    //    }


    //}
    #endregion

}
