using CommonServiceLocator;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.Common;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DominatorHouse.ViewModels
{
    public class MainViewModel : BindableBase, IMainViewModel
    {
        private readonly IApplicationResourceProvider _applicationResourceProvider;
        private readonly ISchedulerProxy _schedulerProxy;
        private Dock _tabDock;
        public ILogViewModel LogViewModel { get; }
        public IPerfCounterViewModel PerfCounterViewModel { get; }

        public SelectableViewModel<string> Languages { get; }

        public ISelectedNetworkViewModel AvailableNetworks { get; }

        public SelectableViewModel<TabItemTemplates> TabItems { get; }

        public AccessorStrategies Strategies { get; set; }

        public ICommand SelectionChangedCommand { get; set; }
        string _fatalError { get; set; }
        private bool IsCancelFromLicenceValidationState { get; set; }
        public Dock TabDock
        {
            get
            {
                return _tabDock;
            }
            set
            {
                SetProperty(ref _tabDock, value, nameof(TabDock));
            }
        }
        public MainViewModel(ILogViewModel logViewModel, IApplicationResourceProvider applicationResourceProvider, IPerfCounterViewModel perfCounterViewModel, ISelectedNetworkViewModel availableNetworks, ISchedulerProxy schedulerProxy)
        {
            SocinatorKeyHelper.InitilizeKey();
            FatalErrorDiagnosis();

            Application.Current.MainWindow.Closing += (s, e) => OnClosing(e);
            LogViewModel = logViewModel;
            _applicationResourceProvider = applicationResourceProvider;
            PerfCounterViewModel = perfCounterViewModel;
            AvailableNetworks = availableNetworks;
            _schedulerProxy = schedulerProxy;
            //SelectionChangedCommand = new BaseCommand<object>((sender) => true, SelectionChangedExecute);
            Languages = new SelectableViewModel<string>(ServiceLocator.Current.GetInstance<IBinFileHelper>().LanguagesList());
            AvailableNetworks.ItemSelected += OnAvailableNetworks_ItemSelected;
            TabItems = new SelectableViewModel<TabItemTemplates>(new List<TabItemTemplates>());
            TabItems.ItemSelected += OnTabItems_ItemSelected;
            TabSwitcher.ChangeTabIndex = (mainTabIndex, subTabIndex) =>
            {
                TabItems.SelectByIndex(mainTabIndex);

                if (subTabIndex == null)
                    return;

                //TODO : it's awful! (use dymanic for it) change it later!
                var selectedTabObject = TabItems.Selected?.Content.Value;

                ((dynamic)selectedTabObject)?.SetIndex((int)subTabIndex);
            };

            // Go to campaign from respective module after campaign saved
            TabSwitcher.GoToCampaign = ()
                => TabItems.Selected =
                    TabItems.Items.FirstOrDefault(x => x.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyCampaigns));

            TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;

            Strategies = new AccessorStrategies
            {
                _determine_available = a => AvailableNetworks.Contains(a),
                _inform_warnings = GlobusLogHelper.log.Warn,
            };

            Socinator.DominatorCores.DominatorCoreBuilder.Strategies = Strategies;
        }

        private void SelectionChangedExecute(object sender)
        {
            try
            {
                var selected = (sender as ComboBox).SelectedItem as string;
                bool delFlag = false;
                var binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
                switch (selected)
                {
                    case "English":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Resources/Languages/Chinese.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Resources/Languages/English.xaml", UriKind.RelativeOrAbsolute) });
                            binFileHelper.SetLanguages("English\r\nChinese");
                        }
                        break;

                    case "Chinese":
                        {
                            List<ResourceDictionary> res = Application.Current.Resources.MergedDictionaries.ToList();
                            ResourceDictionary obj = res.Where(x => x.Source.OriginalString.ToString() == "/DominatorUIUtility;component/Resources/Languages/English.xaml").FirstOrDefault();
                            delFlag = Application.Current.Resources.MergedDictionaries.Remove(obj);
                            if (delFlag)
                                Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary() { Source = new Uri("/DominatorUIUtility;component/Resources/Languages/Chinese.xaml", UriKind.RelativeOrAbsolute) });
                            binFileHelper.SetLanguages("Chinese\r\nEnglish");
                        }
                        break;
                }
            }
            catch (Exception ex) { }
        }

        private void OnClosing(CancelEventArgs e)
        {
            try
            {
                e.Cancel = true;
                bool isClose = Dialog.ShowCustomDialog("LangKeyConfirmation".FromResourceDictionary(), "LangKeyConfirmationToCloseSocinator".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MessageDialogResult.Affirmative;
                if (isClose)
                {
                    DominatorHouseCore.Utility.Utilities.KillGecko();
                    Application.Current.Shutdown();
                    Process.GetCurrentProcess().Kill();
                }
                else if (IsCancelFromLicenceValidationState)
                    FatalErrorDiagnosis();

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
                var key = SocinatorKeyHelper.Key;

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
            catch (Exception ex)
            {
                ex.DebugLog();
                //if (!Application.Current.Dispatcher.CheckAccess())
                //{
                //    Application.Current.Dispatcher.Invoke(() =>
                //    {
                //        Application.Current.Shutdown();
                //        Process.GetCurrentProcess().Kill();
                //    });
                //}
                //else
                //{
                //    Application.Current.Shutdown();
                //    Process.GetCurrentProcess().Kill();
                //}
            }
        }

        private bool _isStartedfirstTime;
        private async Task FatalErrorDiagnosis()
        {
            string fatalError;
            var key = SocinatorKeyHelper.Key;
            if (key != null)
            {
                _isStartedfirstTime = true;
                if (await DiagnoseFatalError(key.FatalErrorMessage))
                {
                    if (!_isStartedfirstTime)
                        return;
                    var settings = new MetroDialogSettings()
                    {
                        DefaultText = string.IsNullOrEmpty(key.FatalErrorMessage) ? "" : key.FatalErrorMessage,
                        AffirmativeButtonText = "Validate"
                    };
                    while (true)
                    {
                        try
                        {
                            fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "LangKeyLicense".FromResourceDictionary(), settings);
                            if (string.IsNullOrEmpty(fatalError))
                            {
                                Application.Current.MainWindow.Close();
                                continue;
                            }
                            if (await IsProcessFatalError(fatalError))
                                // ReSharper disable once RedundantJumpStatement
                                continue;
                            // ReSharper disable once RedundantIfElseBlock
                            else if (_isStartedfirstTime)
                                continue;
                            else
                                break;
                        }
                        catch (Exception ex)
                        {
                        }
                    }
                }
            }
            else
                while (true)
                {
                    try
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "LangKeyLicense".FromResourceDictionary());
                        if (await IsProcessFatalError(fatalError))
                            // ReSharper disable once RedundantJumpStatement
                            continue;
                        else break;
                    }
                    catch (Exception ex)
                    {
                    }
                }

        }

        private async Task<bool> DiagnoseFatalError(string fatalError)
        {
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "LangKeyCheckingLicense".FromResourceDictionary(),
                "LangKeyTakeFewMoments".FromResourceDictionary());
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

                await controller.CloseAsync();
                if (!_isStartedfirstTime)
                    await FatalErrorDiagnosis();
                return true;
            }
            _isStartedfirstTime = false;
            IsCancelFromLicenceValidationState = false;
            var fatalErrorHandler = new FatalErrorHandler
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
            {
                IsCancelFromLicenceValidationState = true;
                Application.Current.MainWindow.Close();
            }
            else
            {
                if (DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "LangKeyLicense".FromResourceDictionary(), "LangKeyValidateSocinator".FromResourceDictionary(), MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
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
                Task.Factory.StartNew(() =>
                 {
                     FeatureFlags.UpdateFeatures();
                     var modules = ServiceLocator.Current.GetAllInstances<ISocialNetworkModule>();
                     foreach (var socialNetworkModule in modules.Where(a => SocinatorInitialize.IsNetworkAvailable(a.Network)))
                     {
                         var module = socialNetworkModule;
                         if (FeatureFlags.Instance.ContainsKey(module.Network.ToString()))
                         {
                             try
                             {
                                 SocinatorInitialize.SocialNetworkRegister(
                                     module.GetNetworkCollectionFactory(Strategies), module.Network);
                                 PublisherInitialize.SaveNetworkPublisher(module.GetPublisherCollectionFactory(),
                                     module.Network);
                                 AddNetwork(socialNetworkModule.Network);
                             }
                             catch (AggregateException ex)
                             {
                                 Console.WriteLine(ex.Message);
                             }
                             catch (Exception ex)
                             {
                                 ex.DebugLog();
                             }
                         }
                        //FeatureFlags.Check(module.Network.ToString(), () =>
                        //{
                        //    try
                        //    {
                        //        SocinatorInitialize.SocialNetworkRegister(module.GetNetworkCollectionFactory(Strategies), module.Network);
                        //        PublisherInitialize.SaveNetworkPublisher(module.GetPublisherCollectionFactory(), module.Network);
                        //        AddNetwork(socialNetworkModule.Network);
                        //    }
                        //    catch (AggregateException ex)
                        //    {
                        //        Console.WriteLine(ex.Message);
                        //    }
                        //    catch (Exception ex)
                        //    {
                        //        ex.DebugLog();
                        //    }
                        //});
                        Task.Delay(5);
                     }

                     SetActiveNetwork(SocialNetworks.Social);
                 });
                ThreadFactory.Instance.Start(() =>
                {
                    _schedulerProxy.AddJob(InitializeJobCores, x => x.ToRunNow());
                });

                ConfigFileManager.ApplyTheme();

                Application.Current.MainWindow.Closed += (o, e) => Process.GetCurrentProcess().Kill();

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

        private void InitializeJobCores()
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var nextDayTime = DateTime.Now.AddDays(1);

                    _schedulerProxy.AddJob(InitializeJobCores,
                        x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                            .AndEvery(1).Days());
                });

                FeatureFlags.UpdateFeatures();

                Task.Factory.StartNew(() =>
                {
                    #region log deletion and backup Account

                    DirectoryUtilities.DeleteOldLogsFile();
                    DirectoryUtilities.CompressAccountDetails();

                    #endregion

                    #region SoftwareSettings

                    var softwareSetting = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
                    softwareSetting.InitializeOnLoadConfigurations();

                    //  softwareSetting.ActivityManagerInitializer();

                    //softwareSetting.ScheduleAutoUpdation();
                    //if (SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook) != null)
                    //    softwareSetting.ScheduleAdsScraping();

                    #endregion

                });
                Task.Factory.StartNew(() =>
                {
                    #region Publisher

                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                    //PublishScheduler.ScheduleTodaysPublisher();
                    PublishScheduler.UpdateNewGroupList();
                    var publisherPostFetcher = new PublisherPostFetcher();
                    publisherPostFetcher.StartFetchingPostData();

                    #endregion

                    var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                    var deletionPostlist =
                        genericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                            .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                });

                #region Commented
                //Parallel.Invoke(() =>
                //                 {
                //                     DirectoryUtilities.DeleteOldLogsFile();
                //                     DirectoryUtilities.CompressAccountDetails();
                //                 },
                //                 () =>
                //                  {
                //                    var softwareSetting = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
                //                    softwareSetting.InitializeOnLoadConfigurations();
                //                    softwareSetting.ActivityManagerInitializer();
                //                    softwareSetting.ScheduleAutoUpdation();
                //                    if (SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook) != null)
                //                        softwareSetting.ScheduleAdsScraping();
                //                },

                //                () =>
                //                {
                //                    PublisherInitialize.GetInstance.PublishCampaignInitializer();
                //                    PublishScheduler.ScheduleTodaysPublisher();
                //                    PublishScheduler.UpdateNewGroupList();
                //                },
                //               () =>
                //                {
                //                    var publisherPostFetcher = new PublisherPostFetcher();
                //                    publisherPostFetcher.StartFetchingPostData();
                //                },
                //                () =>
                //                {
                //                    var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
                //                    var deletionPostlist =
                //                        genericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                //                            .GetDeletePublisherPostModel).Where(x => x.IsDeletedAlready == false).ToList();
                //                    deletionPostlist.ForEach(PublishScheduler.DeletePublishedPost);
                //                }); 
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
                    _schedulerProxy.AddJob(async () => await IsCheck(),
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

        private void OnTabItems_ItemSelected(object sender, TabItemTemplates itemTemplate)
        {
            if (itemTemplate != null)
            {
                if (itemTemplate.Title ==
                    _applicationResourceProvider.GetStringResource(ApplicationResourceProvider
                        .LangKeyAccountsActivity))
                {
                    ServiceLocator.Current.GetInstance<IDominatorAutoActivityViewModel>().CallRespectiveView(SocialNetworks.Social);
                }

                if (itemTemplate.Title ==
                    _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyPublisher))
                {
                    PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl =
                        Home.GetSingletonHome();
                }

                if (itemTemplate.Title ==
                    _applicationResourceProvider.GetStringResource(ApplicationResourceProvider
                        .LangKeyAccountsManager))
                {
                    var lastOne = AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType;
                    /* LastControlType will be have value "AccountManager" if last opened UserControl was "Account Manager" itselt, it won't let to change UserControl if "Account Details" was opened. */
                    if (AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType == "AccountDetail")
                        return;
                    
                    AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                            AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, Strategies);
                }

                if (itemTemplate.Title ==
                    _applicationResourceProvider.GetStringResource(
                        ApplicationResourceProvider.LangKeySociopublisher))
                {
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                        PublisherDefaultPage.Instance();
                }
            }
        }

        private void OnAvailableNetworks_ItemSelected(object sender, SocialNetworks? network)
        {
            if (!network.HasValue)
                return;

            TabDock = Dock.Top;
            if (network == SocialNetworks.Social)
                TabDock = Dock.Left;

            // if "Account details" was opened in account manager, then discard all account details changes while switching network 
            var isAccountDetailsOpened = AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType;
            if(isAccountDetailsOpened == "AccountDetail")
            {
                ((AccountDetail)(AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl)).AccountDetailsViewModel.UpdateCurrentDominatorAccountModel();
                 AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType = "AccountManager";
            }

            TabInitialize(network.Value);
        }

        public void AddNetwork(SocialNetworks socialNetwork)
        {
            AvailableNetworks.Add(socialNetwork);
        }

        public void SetActiveNetwork(SocialNetworks social)
        {
            AvailableNetworks.Selected = social;
        }

        public void TabInitialize(SocialNetworks network)
        {
            try
            {
                var tabHandler = SocinatorInitialize.GetSocialLibrary(network).GetNetworkCoreFactory().TabHandlerFactory;
                if (tabHandler == null)
                    return;
                TabItems.Renew(tabHandler.NetworkTabs);
                TabItems.SelectByIndex(0);
                tabHandler.UpdateAccountCustomControl(network);
                SocinatorInitialize.SetAsActiveNetwork(network);
            }
            catch (Exception ex)
            {
                TabDock = Dock.Left;

                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Fatal Error",
                    $"Please purchase access of {network} automation features!");
                ex.DebugLog();
            }
        }

        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            var AutoActivityViewModel = ServiceLocator.Current.GetInstance<IDominatorAutoActivityViewModel>();
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                TabItems.SelectByIndex(index);
                AutoActivityViewModel.NewAutoActivityObject(network, selectedAccount);
            }
            else
            {
                TabItems.SelectByIndex(index);
                AutoActivityViewModel.CallRespectiveView(SocialNetworks.Social);
                AutoActivityViewModel.NewAutoActivityObject(network, selectedAccount);
            }
        }

        public void Dispose()
        {
            PerfCounterViewModel?.Dispose();
            AvailableNetworks.ItemSelected -= OnAvailableNetworks_ItemSelected;
            TabItems.ItemSelected -= OnTabItems_ItemSelected;
        }
    }
}
