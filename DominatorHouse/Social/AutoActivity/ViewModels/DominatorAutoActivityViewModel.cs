using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using Socinator.Social.AutoActivity.Views;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;


namespace Socinator.Social.AutoActivity.ViewModels
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
        {
            return ObjDominatorAutoActivityViewModel ?? (ObjDominatorAutoActivityViewModel = new DominatorAutoActivityViewModel());
        }

        public ICollectionView AccountsCollectionView { get; set; }

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

        public void CallRespectiveView(SocialNetworks networks)
        {
            var accountToolsView = SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().AccountUserControlTools;
            SelectedUserControl = accountToolsView.GetStartupToolsView();

            if (networks == SocialNetworks.Social)
                InitializeAccounts();
        }

        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; set; }


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
                        AutoActivityModuleDetailsCollections = new ObservableCollection<AutoActivityModuleDetails>()
                    };

                    account.ActivityManager.LstModuleConfiguration.ForEach(x =>
                    {
                        var activityDetailsModel = new ActivityDetailsModel()
                        {
                            Title = x.ActivityType.ToString(),
                            Status = x.IsEnabled ? "Active" : "InActive",
                        };
                        var autoActivityModuleDetails = new AutoActivityModuleDetails
                        {
                            ActivityDetailsModel = activityDetailsModel
                        };
                        accountsActivityDetailModel.AutoActivityModuleDetailsCollections.Add(autoActivityModuleDetails);
                    });

                    var accountModuleList = EnumUtility.GetEnums(account.AccountBaseModel.AccountNetwork.ToString());

                    if (accountModuleList.Count >
                        accountsActivityDetailModel.AutoActivityModuleDetailsCollections.Count)
                    {
                        var alreadyAddedActivity = account.ActivityManager.LstModuleConfiguration
                            .Select(x => x.ActivityType).ToList();

                        var notAddedList = accountModuleList.Except(alreadyAddedActivity);

                        notAddedList.ForEach(x =>
                        {
                            var activityDetailsModel = new ActivityDetailsModel()
                            {
                                Title = x.ToString(),
                                Status = "InActive",
                            };

                            var autoActivityModuleDetails = new AutoActivityModuleDetails
                            {
                                ActivityDetailsModel = activityDetailsModel
                            };

                            accountsActivityDetailModel.AutoActivityModuleDetailsCollections.Add(
                                autoActivityModuleDetails);
                        });
                    }
                    AccountsCollection.Add(accountsActivityDetailModel);
                }
            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }


    }
}
