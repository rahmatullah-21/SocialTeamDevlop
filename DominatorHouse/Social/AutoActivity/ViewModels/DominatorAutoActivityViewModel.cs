using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public interface IDominatorAutoActivityViewModel
    {
        void CallRespectiveView(SocialNetworks networks);
        void InitializeAccounts();
    }

    public class DominatorAutoActivityViewModel : BindableBase, IDominatorAutoActivityViewModel
    {
        private readonly object _syncObject = new object();
        public DelegateCommand<AccountsActivityDetailModel> GoToToolsCmd { get; }
        public DelegateCommand<ActivityDetailsModel> ChangeActivityStatusCmd { get; }

        /// <summary>
        /// To hold all accounts important activities enable status
        /// </summary>
        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; }

        private DominatorAutoActivityViewModel()
        {
            _selectedUserControl = new UserControl();
            AccountsCollection = new ObservableCollection<AccountsActivityDetailModel>();
            BindingOperations.EnableCollectionSynchronization(AccountsCollection, _syncObject);
            GoToToolsCmd = new DelegateCommand<AccountsActivityDetailModel>(GoToTools);
            ChangeActivityStatusCmd = new DelegateCommand<ActivityDetailsModel>(ChangeActivityStatus);
        }

        private void ChangeActivityStatus(ActivityDetailsModel currentDataContext)
        {
            try
            {
                var currentAccountActivity = AccountsFileManager.GetAccountById(currentDataContext.AccountId).ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == currentDataContext.Title);
                var account = AccountsFileManager.GetAccountById(currentDataContext.AccountId);
                var campaignStatus = CampaignsFileManager.Get()?.FirstOrDefault(x => x.TemplateId == currentAccountActivity.TemplateId)?.Status;
                if (campaignStatus == "Paused" && currentDataContext.Status)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", $"This account belongs to campaign configuration, which is paused state. Please make the campaign active before changing activity status for this account.");
                    currentDataContext.Status = false;
                    return;
                }
                if (currentDataContext == null)
                    return;
            }
            catch (Exception ex)
            {


            }

            var accountDetails = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social).DominatorAccountViewModel
                .LstDominatorAccountModel
                .FirstOrDefault(x => x.AccountBaseModel.AccountId == currentDataContext.AccountId);

            // accountDetails?.NotifyCancelled();

            var status = DominatorScheduler.ChangeAccountsRunningStatus(currentDataContext.Status, currentDataContext.AccountId,
                currentDataContext.Title);

            if (!status)
            {
                try
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(this, "Error", $"Please configure your {currentDataContext.Title} settings, before starting the activity. Make sure you have added enough queries and have clicked on SAVE button");
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
                if (Equals(_selectedUserControl, value))
                    return;
                SetProperty(ref _selectedUserControl, value);
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
                    ThreadFactory.Instance.Start(InitializeAccounts);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        /// <summary>
        /// To Initialize the account details with enable status 
        /// </summary>
        public void InitializeAccounts()
        {
            try
            {
                // read from bin file for getting all accounts
                var accounts = AccountsFileManager.GetAll();



                // if accounts count more than one means generate the activities
                lock (_syncObject)
                {
                    AccountsCollection.Clear();

                    if (accounts != null)
                        foreach (var account in accounts)
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
                                var activities = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                    .GetNetworkCoreFactory()
                                    .AccountUserControlTools
                                    .GetImportantActivityTypes();
                                try
                                {
                                    foreach (var x in activities)
                                    {
                                        try
                                        {
                                            // get the activity details                    
                                            var activityData = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(y =>
                                                y.ActivityType == x);

                                            // if activity present then add to list with status
                                            if (activityData != null)
                                            {
                                                accountsActivityDetailModel.ActivityDetailsCollections
                                                    .Add(new ActivityDetailsModel
                                                    {
                                                        Status = activityData.IsEnabled,
                                                        Title = x,
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
                                    Application.Current.Dispatcher.Invoke(() =>
                                    {
                                        if (AccountsCollection.All(x => x.AccountId != account.AccountBaseModel.AccountId))
                                            AccountsCollection.Add(accountsActivityDetailModel);
                                    });
                                }
                                else
                                {
                                    // add the item to account collection
                                    if (AccountsCollection.All(x => x.AccountId != account.AccountBaseModel.AccountId))
                                        AccountsCollection.Add(accountsActivityDetailModel);
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        }

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

    }
}
