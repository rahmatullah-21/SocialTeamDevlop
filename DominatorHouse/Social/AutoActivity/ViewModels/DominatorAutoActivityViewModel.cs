using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouse.Social.AutoActivity.Views;
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
            switch (networks)
            {
                case SocialNetworks.Social:
                    SelectedUserControl = SocialAutoActivity.GetSingletonSocialAutoActivity();
                    InitializeAccounts();
                    break;
                case SocialNetworks.Instagram:
                    SelectedUserControl = GramDominatorUI.TabManager.ToolTabs.GetSingletonToolTabs();
                    break;

                case SocialNetworks.Twitter:
                    //SelectedUserControl= TwtDominatorUI.TabManager.ToolsTab.GetSingletonToolTabs();
                    break;
            }
        }

        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; set; }

        private void InitializeAccounts()
        {
            var accounts = AccountsFileManager.GetAll();

            AccountsCollection =
                new ObservableCollection<AccountsActivityDetailModel>();

            foreach (var account in accounts)
            {
                var accountsActivityDetailModel = new AccountsActivityDetailModel
                {
                    DominatorAccountModel = account,
                    AutoActivityModuleDetailsCollections = new ObservableCollection<AutoActivityModuleDetails>()
                };

                account.ActivityManager.LstModuleConfiguration.ForEach(x =>
                {
                    var activityDetailsModel = new ActivityDetailsModel()
                    {
                        Title = x.ActivityType.ToString(),
                        Status = x.IsEnabled ? "Active" : "InActive",
                        Ratio = new ActivityRatio()
                        {
                            Total = x.MaximumCountPerDay,
                            Completed = x.MaximumCountPerDay - 1 <= 0 ? x.MaximumCountPerDay : x.MaximumCountPerDay - 1
                        }
                    };

                    var autoActivityModuleDetails = new AutoActivityModuleDetails
                    {
                        ActivityDetailsModel = activityDetailsModel
                    };

                    accountsActivityDetailModel.AutoActivityModuleDetailsCollections.Add(autoActivityModuleDetails);                    
                });

                AccountsCollection.Add(accountsActivityDetailModel);
            }
            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }


    }
}
