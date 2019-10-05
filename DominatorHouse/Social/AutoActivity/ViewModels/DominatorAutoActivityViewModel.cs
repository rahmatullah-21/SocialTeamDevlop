using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public interface IDominatorAutoActivityViewModel
    {
        void CallRespectiveView(SocialNetworks networks);
        bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts);
    }

    public class DominatorAutoActivityViewModel : BindableBase, IDominatorAutoActivityViewModel
    {
        private readonly object _syncObject = new object();
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountCollectionViewModel _accountCollectionViewModel;


        private UserControl _selectedUserControl;
        /// <summary>
        /// To bind the initial view for each dominator
        /// </summary>
        public UserControl SelectedUserControl
        {
            get
            {
                return _selectedUserControl;
            }
            set
            {
                SetProperty(ref _selectedUserControl, value);
                OnPropertyChanged(nameof(ShowContent));
                OnPropertyChanged(nameof(ShowSocial));
            }
        }

        public DelegateCommand<AccountsActivityDetailModel> GoToToolsCmd { get; }
        public DelegateCommand<ActivityDetailsModel> ChangeActivityStatusCmd { get; }

        /// <summary>
        /// To hold all accounts important activities enable status
        /// </summary>
        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; }

        public bool ShowContent => SelectedUserControl != null;
        public bool ShowSocial => SelectedUserControl == null;

        public DominatorAutoActivityViewModel(IAccountsFileManager accountsFileManager, IAccountCollectionViewModel accountCollectionViewModel)
        {
            _accountsFileManager = accountsFileManager;
            _accountCollectionViewModel = accountCollectionViewModel;
            _selectedUserControl = new UserControl();
            AccountsCollection = new ObservableCollection<AccountsActivityDetailModel>();

            BindingOperations.EnableCollectionSynchronization(AccountsCollection, _syncObject);
            GoToToolsCmd = new DelegateCommand<AccountsActivityDetailModel>(GoToTools);
            ChangeActivityStatusCmd = new DelegateCommand<ActivityDetailsModel>(ChangeActivityStatus);
        }

        public bool NewAutoActivityObject(SocialNetworks soicalNetworks, string selectedAccounts)
        {
            try
            {
                CallRespectiveView(soicalNetworks);

                SocinatorInitialize.GetSocialLibrary(soicalNetworks)
                    .GetNetworkCoreFactory().AccountUserControlTools.RecentlySelectedAccount = selectedAccounts;

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        /// <summary>
        /// To bind the respective network view for auto activity
        /// </summary>
        /// <param name="networks">pass the social network for which UI gets bind </param>
        public void CallRespectiveView(SocialNetworks networks)
        {
            try
            {
                // collect the UI
                var accountToolsView = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().AccountUserControlTools;

                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                        SelectedUserControl = networks == SocialNetworks.Social ? null : accountToolsView.GetStartupToolsView());
                }
                else
                {
                    SelectedUserControl = networks == SocialNetworks.Social ? null : accountToolsView.GetStartupToolsView();
                }

                SocinatorInitialize.AccountModeActiveSocialNetwork = networks;
                // If passed network is social then initialize the account details
                if (networks == SocialNetworks.Social)
                    InitializeAccounts();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ChangeActivityStatus(ActivityDetailsModel currentDataContext)
        {

            var account = _accountsFileManager.GetAccountById(currentDataContext.AccountId);
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            var currentAccountActivity =
                jobActivityConfigurationManager[account.AccountId, currentDataContext.Title];
            var campaignStatus = campaignFileManager.FirstOrDefault(x => x.TemplateId == currentAccountActivity?.TemplateId)?.Status;
            if (campaignStatus == "Paused" && currentDataContext.Status)
            {
                Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), "LangKeyErrorCampaignConfigurationIsPaused".FromResourceDictionary());
                currentDataContext.Status = false;
                return;
            }

            var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            var status = dominatorScheduler.ChangeAccountsRunningStatus(currentDataContext.Status, currentDataContext.AccountId,
                currentDataContext.Title);

            if (!status)
            {
                try
                {
                    Dialog.ShowDialog("LangKeyError".FromResourceDictionary(), String.Format("LangKeyConfigureYourSettings".FromResourceDictionary(), currentDataContext.Title));
                    currentDataContext.Status = false;
                }
                catch (Exception ex)
                {

                    ex.DebugLog();
                }
            }
        }


        private void GoToTools(AccountsActivityDetailModel accountsActivityDetailModel)
        {
            if (accountsActivityDetailModel == null)
                return;

            SocinatorInitialize.GetSocialLibrary(accountsActivityDetailModel.AccountNetwork).GetNetworkCoreFactory()
                    .AccountUserControlTools.RecentlySelectedAccount =
                accountsActivityDetailModel.AccountName;

            CallRespectiveView(accountsActivityDetailModel.AccountNetwork);
        }


        /// <summary>
        /// To Initialize the account details with enable status 
        /// </summary>
        private void InitializeAccounts()
        {
            // read from bin file for getting all accounts


            // if accounts count more than one means generate the activities
            AccountsCollection.Clear();

            Task.Factory.StartNew(() =>
            {
                var accountCollection = _accountCollectionViewModel.GetCopySync()
                    .Where(x => x.AccountBaseModel.Status == AccountStatus.Success);
                foreach (var account in accountCollection)
                {
                    try
                    {
                        // initialize the activity details
                        var accountsActivityDetailModel = new AccountsActivityDetailModel
                        {
                            AccountName = account.AccountBaseModel.UserName,
                            AccountId = account.AccountBaseModel.AccountId,
                            AccountNetwork = account.AccountBaseModel.AccountNetwork,
                            ActivityDetailsCollections = new ObservableCollection<ActivityDetailsModel>()
                        };

                        // get the respective network details
                        var activities = SocinatorInitialize
                            .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                            .GetNetworkCoreFactory()
                            .AccountUserControlTools
                            .GetImportantActivityTypes();
                        try
                        {
                            var jobActivityConfigurationManager = ServiceLocator.Current
                                .GetInstance<IJobActivityConfigurationManager>();
                            foreach (var x in activities)
                            {
                                try
                                {
                                    var titleData = x.GetDescriptionAttr().Split(',');

                                    var activityTitle = titleData.LastOrDefault().Contains("LangKey") ?
                                           titleData.LastOrDefault().FromResourceDictionary() : x.ToString();
                                    
                                    // get the activity details                    
                                    var activityData =
                                        jobActivityConfigurationManager[account.AccountId, x];

                                    // if activity present then add to list with status
                                    if (activityData != null)
                                    {
                                        accountsActivityDetailModel.ActivityDetailsCollections
                                            .Add(new ActivityDetailsModel
                                            {
                                                Status = activityData.IsEnabled,
                                                Title = x,
                                                ActivityTitle = activityTitle,
                                                AccountId = account.AccountId
                                            });
                                    }
                                    // if activity not present then add to list with default status
                                    else
                                    {
                                        accountsActivityDetailModel.ActivityDetailsCollections
                                            .Add(new ActivityDetailsModel
                                            {
                                                Status = false,
                                                Title = x,
                                                ActivityTitle = activityTitle,
                                                AccountId = account.AccountId
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        if (!Application.Current.Dispatcher.CheckAccess())
                        {
                            // add the item to account collection
                            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                lock (_syncObject)
                                {
                                    if (AccountsCollection.All(x =>
                                        x.AccountId != account.AccountBaseModel.AccountId))
                                        AccountsCollection.Add(accountsActivityDetailModel);
                                }
                            }), DispatcherPriority.Render);
                        }
                        else
                        {
                            lock (_syncObject)
                            {
                                // add the item to account collection
                                if (AccountsCollection.All(x => x.AccountId != account.AccountBaseModel.AccountId))
                                    AccountsCollection.Add(accountsActivityDetailModel);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            });

        }
    }
}
