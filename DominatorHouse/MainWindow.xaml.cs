#region Namespaces
using DominatorHouse.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
//using EmbeddedBrowser;
using FluentScheduler;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

#endregion

namespace Socinator
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IMainViewModel _mainViewModel;
        private static readonly string RamSize = GetRamsize();

        private bool IsClickedFromMainWindow { get; set; } = true;

        private string _fatalError;

        public MainWindow()
        {
            try
            {
                DialogParticipation.SetRegister(this, this);
                Dispatcher.Invoke(async () => { await FatalErrorDiagnosis(); });
                InitializeComponent();
                SocinatorInitialize.LogInitializer(this);
                _mainViewModel = IoC.Container.Resolve<IMainViewModel>();
                SocinatorWindow.DataContext = _mainViewModel;
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
                        fatalError = await this.ShowInputAsync("Socinator", "License", settings);
                        if (await IsProcessFatalError(fatalError))
                            continue;
                        else break;
                    }
                }
                else
                    while (true)
                    {
                        fatalError = await this.ShowInputAsync("Socinator", "License");
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
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(this, "Hang On! Checking your License status",
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
                Close();
                await controller.CloseAsync();
                await FatalErrorDiagnosis();
                return true;
            }

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

        private async Task<bool> IsProcessFatalError(string fatalError)
        {
            if (!string.IsNullOrEmpty(fatalError) && await DiagnoseFatalError(fatalError))
                return false;
            else if (fatalError == null)
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

        private static PerformanceCounter PerformanceCounter { get; }
            = new PerformanceCounter("Memory", "Available MBytes");

        private static ManagementObject Processor { get; }
            = new ManagementObject("Win32_PerfFormattedData_PerfOS_Processor.Name='_Total'");

        private void SocinatorInitializer()
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    JobManager.AddJob(() => InitializeJobCores(_fatalError), x => x.ToRunNow());
                });

                //Init UI delegates            
                CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Confirm", msg) ==
                    MessageDialogResult.Affirmative;


                ConfigFileManager.ApplyTheme();

                var performanceTask = new Task(StartbindMemory,
                    TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent);
                performanceTask.Start();


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

        private void ActivityLog_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (MainGrid.RowDefinitions[2].Height.Value <= 200 && MainGrid.RowDefinitions[2].Height.Value > 45)
                MainGrid.RowDefinitions[2].Height = new GridLength(45);
            else
                MainGrid.RowDefinitions[2].Height = new GridLength(200);
        }

        private void InitialTabablzControl_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (IsClickedFromMainWindow)
            {
                var dialog = new Dialog();

                var activityLogWindow = dialog.GetMetroWindow(Logger, "Activity Log");

                IsClickedFromMainWindow = false;
                activityLogWindow.Closing += (senders, events) =>
                {
                    activityLogWindow.Content = null;
                    Grid.SetRow(Logger, 2);
                    MainGrid.Children.Add(Logger);
                    MainGrid.RowDefinitions[2].Height = new GridLength(200);
                    IsClickedFromMainWindow = true;
                };

                MainGrid.RowDefinitions[2].Height = new GridLength(0);
                MainGrid.Children.Remove(Logger);
                activityLogWindow.Show();
            }
        }

        public void InitializeJobCores(string license)
        {

            try
            {
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

                FeatureFlags.UpdateFeatures();
                foreach (var network in SocinatorInitialize.AvailableNetworks)
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
                                        (INetworkCollectionFactory)selectedConstructor.Invoke(new object[] { _mainViewModel.Strategies });
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

                            _mainViewModel.AddNetwork(network);
                        }
                        catch (AggregateException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                }

                _mainViewModel.SetActiveNetwork(SocialNetworks.Social);
                FeatureFlags.UpdateFeatures();

                var softWareSettings = new DominatorHouse.Utilities.SoftwareSettings();
                ThreadFactory.Instance.Start(() => { softWareSettings.InitializeOnLoadConfigurations(_mainViewModel.Strategies); });

                var softWareSetting = new DominatorHouseCore.Settings.SoftwareSettings();
                ThreadFactory.Instance.Start(() => { softWareSetting.InitializeOnLoadConfigurations(); });

                // For Every day backup
                ThreadFactory.Instance.Start(() =>
                {
                    DirectoryUtilities.DeleteOldLogsFile();
                    //DirectoryUtilities.Compress();
                });

                #region Publisher

                ThreadFactory.Instance.Start(() =>
                {
                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                    PublishScheduler.ScheduleTodaysPublisher();
                    PublishScheduler.UpdateNewGroupList();
                });

                ThreadFactory.Instance.Start(() =>
                {
                    var publisherPostFetcher = new PublisherPostFetcher();
                    publisherPostFetcher.StartFetchingPostData();
                });

                ThreadFactory.Instance.Start(() =>
                    {
                        var deletionPostlist =
                        GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                            .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                        deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                    });

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