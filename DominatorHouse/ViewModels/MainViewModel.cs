using CommonServiceLocator;
using DominatorHouse.Social.AutoActivity.ViewModels;
using DominatorHouseCore;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.BusinessLogic.GlobalRoutines;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.Common;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.IoC;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.SocioPublisher;
using EmbeddedBrowser;
using FluentScheduler;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
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
        private Dock _tabDock;
        public ILogViewModel LogViewModel { get; }
        public IPerfCounterViewModel PerfCounterViewModel { get; }

        public SelectableViewModel<string> Languages { get; }

        public ISelectedNetworkViewModel AvailableNetworks { get; }

        public SelectableViewModel<TabItemTemplates> TabItems { get; }

        public AccessorStrategies Strategies { get; set; }
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
        public ICommand WinActivateCommand { get; set; }
        public ICommand WinClosingCommand { get; set; }
        public MainViewModel(ILogViewModel logViewModel, IApplicationResourceProvider applicationResourceProvider, IPerfCounterViewModel perfCounterViewModel, ISelectedNetworkViewModel availableNetworks)
        {
            FatalErrorDiagnosis();

            Application.Current.MainWindow.Closing += (s, e) => OnClosing(e);

            LogViewModel = logViewModel;
            _applicationResourceProvider = applicationResourceProvider;
            PerfCounterViewModel = perfCounterViewModel;
            AvailableNetworks = availableNetworks;
            Languages = new SelectableViewModel<string>(new[] { "English" });
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

                ((dynamic)selectedTabObject)?.setIndex((int)subTabIndex);
            };

            // Go to campaign from respective module after campaign saved
            TabSwitcher.GoToCampaign = ()
                => TabItems.Selected =
                    TabItems.Items.FirstOrDefault(x => x.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyCampaigns));

            TabSwitcher.ChangeTabWithNetwork = ChangeTabWithNetwork;

            Strategies = new AccessorStrategies
            {
                ActionCheckAccount = AccountStatusChecker,
                AccountBrowserLogin = AccountBrowserLogin,
                _determine_available = a => AvailableNetworks.Contains(a),
                _inform_warnings = GlobusLogHelper.log.Warn,
                action_UpdateFollower = AccountUpdate,
                EditProfile = EditProfile,
                RemovePhoneVerification = RemovePhoneVerification
            };

            Socinator.DominatorCores.DominatorCoreBuilder.Strategies = Strategies;
        }

        private void OnClosing(CancelEventArgs e)
        {
            try
            {

                e.Cancel = true;
                bool isClose = Dialog.ShowCustomDialog("Confirmation", "Are you sure to close Socinator?", "Yes", "No") == MessageDialogResult.Affirmative;
                if (isClose)
                {
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
                    try
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "License", settings);
                        if (await IsProcessFatalError(fatalError))
                            continue;
                        else break;
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            else
                while (true)
                {
                    try
                    {
                        fatalError = await DialogCoordinator.Instance.ShowInputAsync(Application.Current.MainWindow, "Socinator", "License");
                        if (await IsProcessFatalError(fatalError))
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
            var controller = await DialogCoordinator.Instance.ShowProgressAsync(Application.Current.MainWindow, "Hang On! Checking your License status",
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
                Application.Current.MainWindow.Close();
                await controller.CloseAsync();
                await FatalErrorDiagnosis();
                return true;
            }
            IsCancelFromLicenceValidationState = false;
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
            {
                IsCancelFromLicenceValidationState = true;
                Application.Current.MainWindow.Close();
            }
            else
            {
                if (DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "License", "Please validate Socinator !!", MessageDialogStyle.AffirmativeAndNegative) == MessageDialogResult.Affirmative)
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
                FeatureFlags.UpdateFeatures();
                var modules = ServiceLocator.Current.GetAllInstances<ISocialNetworkModule>();
                foreach (var socialNetworkModule in modules.Where(a => SocinatorInitialize.IsNetworkAvailable(a.Network)))
                {
                    var module = socialNetworkModule;
                    FeatureFlags.Check(module.Network.ToString(), () =>
                    {
                        try
                        {
                            SocinatorInitialize.SocialNetworkRegister(module.GetNetworkCollectionFactory(Strategies), module.Network);
                            PublisherInitialize.SaveNetworkPublisher(module.GetPublisherCollectionFactory(), module.Network);
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
                    });
                }

                SetActiveNetwork(SocialNetworks.Social);
                ThreadFactory.Instance.Start(() =>
                {
                    JobManager.AddJob(() => InitializeJobCores(_fatalError), x => x.ToRunNow());
                });

                //Init UI delegates            
                CampaignGlobalRoutines.Instance.ConfirmDialog = msg =>
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirm", msg) ==
                    MessageDialogResult.Affirmative;


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



        public void InitializeJobCores(string license)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var nextDayTime = DateTime.Now.AddDays(1);

                    JobManager.AddJob(() => InitializeJobCores(license),
                        x => x.ToRunOnceAt(new DateTime(nextDayTime.Year, nextDayTime.Month, nextDayTime.Day, 0, 0, 1))
                            .AndEvery(1).Days());
                });

                FeatureFlags.UpdateFeatures();

                var softWareSettings = new DominatorHouse.Utilities.SoftwareSettings();
                ThreadFactory.Instance.Start(() => { softWareSettings.InitializeOnLoadConfigurations(Strategies); });

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


        private void RemovePhoneVerification(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.RemovePhoneVerification(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void EditProfile(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.EditProfile(dominatorAccountModel);
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
            }
        }
        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
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
                ThreadFactory.Instance.Start(() =>
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
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                TabItems.SelectByIndex(index);
                ServiceLocator.Current.GetInstance<IDominatorAutoActivityViewModel>().NewAutoActivityObject(network, selectedAccount);
            }
            else
            {
                TabItems.SelectByIndex(index);
                ServiceLocator.Current.GetInstance<IDominatorAutoActivityViewModel>().CallRespectiveView(SocialNetworks.Social);
                ServiceLocator.Current.GetInstance<IDominatorAutoActivityViewModel>().NewAutoActivityObject(network, selectedAccount);
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
