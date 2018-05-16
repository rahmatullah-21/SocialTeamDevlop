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

        public void CallRespectiveView(SocialNetworks networks)
        {
            try
            {
                var accountToolsView = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().AccountUserControlTools;
                SelectedUserControl = accountToolsView.GetStartupToolsView();

                if (networks == SocialNetworks.Social)
                    InitializeAccounts();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void InitializeAccounts()
        {
            var accounts = AccountsFileManager.GetAll();

            AccountsCollection = new ObservableCollection<AccountsActivityDetailModel>();

            if (accounts != null)
                foreach (var account in accounts)
                {
                    var accountsActivityDetailModel = new AccountsActivityDetailModel
                    {
                        AccountName = account.AccountBaseModel.UserName,
                        AccountId = account.AccountBaseModel.AccountId,
                        AccountNetwork = account.AccountBaseModel.AccountNetwork,
                        ActivityDetailsCollections = new ObservableCollection<ActivityDetailsModel>()
                    };

                    account.ActivityManager.LstModuleConfiguration.ForEach(x =>
                    {
                        var activityDetailsModel = new ActivityDetailsModel()
                        {
                            Title = x.ActivityType.ToString(),
                            Status = x.IsEnabled,
                        };

                        if (accountsActivityDetailModel.ActivityDetailsCollections.Count <= 5)
                            accountsActivityDetailModel.ActivityDetailsCollections.Add(activityDetailsModel);
                    });

                    var accountModuleList = EnumUtility.GetEnums(account.AccountBaseModel.AccountNetwork.ToString());

                    if (accountModuleList.Count >
                        accountsActivityDetailModel.ActivityDetailsCollections.Count)
                    {
                        var alreadyAddedActivity = account.ActivityManager.LstModuleConfiguration
                            .Select(x => x.ActivityType).ToList();

                        var notAddedList = accountModuleList.Except(alreadyAddedActivity);

                        notAddedList.ForEach(x =>
                        {
                            var activityDetailsModel = new ActivityDetailsModel
                            {
                                Title = x.ToString(),
                                Status = false,
                            };
                            if (accountsActivityDetailModel.ActivityDetailsCollections.Count <= 5)
                                accountsActivityDetailModel.ActivityDetailsCollections.Add(activityDetailsModel);
                        });
                    }

                    AccountsCollection.Add(accountsActivityDetailModel);
                }
            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }
    }
}
