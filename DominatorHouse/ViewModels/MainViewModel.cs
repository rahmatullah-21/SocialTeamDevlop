using DominatorHouseCore;
using DominatorHouseCore.AppResources;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorHouseCore.ViewModel.Common;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views.Publisher;
using DominatorUIUtility.Views.SocioPublisher;
using EmbeddedBrowser;
using MahApps.Metro.Controls.Dialogs;
using Socinator.Social.AutoActivity.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace DominatorHouse.ViewModels
{
    public interface IMainViewModel : IDisposable
    {
        void AddNetwork(SocialNetworks socialNetwork);

        void SetActiveNetwork(SocialNetworks social);
        DominatorAccountViewModel.AccessorStrategies Strategies { get; }
    }

    public class MainViewModel : BindableBase, IMainViewModel
    {
        private readonly IApplicationResourceProvider _applicationResourceProvider;
        private Dock _tabDock;
        private string _title;
        public ILogViewModel LogViewModel { get; }
        public IPerfCounterViewModel PerfCounterViewModel { get; }

        public SelectableViewModel<string> Languages { get; }

        public SelectableViewModel<SocialNetworks> AvailableNetworks { get; }

        public SelectableViewModel<TabItemTemplates> TabItems { get; }

        public DominatorAccountViewModel.AccessorStrategies Strategies { get; }

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

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                SetProperty(ref _title, value, nameof(Title));
            }
        }

        public MainViewModel(ILogViewModel logViewModel, IApplicationResourceProvider applicationResourceProvider, IPerfCounterViewModel perfCounterViewModel)
        {
            LogViewModel = logViewModel;
            _applicationResourceProvider = applicationResourceProvider;
            PerfCounterViewModel = perfCounterViewModel;
            Languages = new SelectableViewModel<string>(new[] { "English" });
            AvailableNetworks = new SelectableViewModel<SocialNetworks>(new List<SocialNetworks>());
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

            Strategies = new DominatorAccountViewModel.AccessorStrategies
            {
                ActionCheckAccount = AccountStatusChecker,
                AccountBrowserLogin = AccountBrowserLogin,
                _determine_available = AvailableNetworks.Contains,
                _inform_warnings = GlobusLogHelper.log.Warn,
                action_UpdateFollower = AccountUpdate,
                EditProfile = EditProfile,
                RemovePhoneVerification = RemovePhoneVerification
            };

            Socinator.DominatorCores.DominatorCoreBuilder.Strategies = Strategies;
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
                //MessageBox.Show(ex.Message);
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
                if (itemTemplate.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyAccountsActivity))
                {
                    DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
                }
                if (itemTemplate.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyPublisher))
                {
                    PublisherIndexPage.Instance.PublisherIndexPageViewModel.SelectedUserControl = Home.GetSingletonHome();
                }
                if (itemTemplate.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeyAccountsManager))
                {
                    AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl =
                        AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, Strategies);
                }
                if (itemTemplate.Title == _applicationResourceProvider.GetStringResource(ApplicationResourceProvider.LangKeySociopublisher))
                {
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherDefaultPage.Instance();
                }
            }
        }

        private void OnAvailableNetworks_ItemSelected(object sender, SocialNetworks network)
        {
            TabDock = Dock.Top;
            if (network == SocialNetworks.Social)
                TabDock = Dock.Left;
            TabInitialize(network);
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
                Title = tabHandler.NetworkName;
                tabHandler.UpdateAccountCustomControl(network);
                SocinatorInitialize.SetAsActiveNetwork(network);
            }
            catch (Exception ex)
            {
                TabDock = Dock.Left;

                DialogCoordinator.Instance.ShowModalMessageExternal(this, "Fatal Error",
                    $"Please purchase access of {network} automation features!");
                ex.DebugLog();
            }
        }


        private void ChangeTabWithNetwork(int index, SocialNetworks network, string selectedAccount)
        {
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                TabItems.SelectByIndex(index);
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);
            }
            else
            {
                TabItems.SelectByIndex(index);
                DominatorAutoActivity.GetSingletonDominatorAutoActivity(SocialNetworks.Social);
                SocialAutoActivity.GetSingletonSocialAutoActivity().NewAutoActivityObject(network, selectedAccount);
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
