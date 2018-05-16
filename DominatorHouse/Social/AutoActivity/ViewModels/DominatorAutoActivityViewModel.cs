using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;


namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public class DominatorAutoActivityViewModel : BindableBase
    {
        private DominatorAutoActivityViewModel()
        {
            _selectedUserControl = new UserControl();
            AccountsCollection = new ObservableCollection<AccountsActivityDetailModel>();
        }

        private static DominatorAutoActivityViewModel ObjDominatorAutoActivityViewModel { get; set; } = null;

        public static DominatorAutoActivityViewModel GetSingletonDominatorAutoActivityViewModel()
          => ObjDominatorAutoActivityViewModel ?? (ObjDominatorAutoActivityViewModel = new DominatorAutoActivityViewModel());



        private ICollectionView _accountsCollectionView;
        /// <summary>
        /// To give the itemsource for account details
        /// </summary>
        public ICollectionView AccountsCollectionView
        {
            get
            {
                return _accountsCollectionView;
            }
            set
            {
                if (Equals(_accountsCollectionView, value))
                    return;
                SetProperty(ref _accountsCollectionView, value);
            }
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


        private ObservableCollection<AccountsActivityDetailModel> _accountsCollection;
        /// <summary>
        /// To hold all accounts important activities enable status
        /// </summary>
        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection
        {
            get
            {
                return _accountsCollection;
            }
            set
            {
                if (Equals(_accountsCollection, value))
                    return;
                SetProperty(ref _accountsCollection, value);
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
                SelectedUserControl = accountToolsView.GetStartupToolsView();

                // If passed network is social then initialize the account details
                if (networks == SocialNetworks.Social)
                    InitializeAccounts();
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
            // read from bin file for getting all accounts
            var accounts = AccountsFileManager.GetAll();

            // clear saved account details
            AccountsCollection.Clear();

            // if accounts count more than one means generate the activities
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
                            .GetImportantActivityTypes().ToList();

                        activities.ForEach(x =>
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
                                            Title = x.ToString()
                                        });
                                }
                                // if activity not present then add to list with default status
                                else
                                {
                                    accountsActivityDetailModel.ActivityDetailsCollections
                                        .Add(new ActivityDetailsModel
                                        {
                                            Status = false,
                                            Title = x.ToString()
                                        });
                                }
                            }
                            catch (Exception ex)
                            {
                                ex.DebugLog();
                            }
                        });
                        // add the item to account collection
                        AccountsCollection.Add(accountsActivityDetailModel);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
            // Initialize to collection viewS
            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }
    }
}
