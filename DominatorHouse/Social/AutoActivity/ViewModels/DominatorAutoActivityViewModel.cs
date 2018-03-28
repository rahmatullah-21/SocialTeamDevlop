using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
using GramDominatorUI.GDViews.Tools.Follow;


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
                   // SelectedUserControl= TwtDominatorUI.TabManager.ToolsTab.GetSingletonToolTabs();
                    break;
                case SocialNetworks.Pinterest:
                   // SelectedUserControl = PinDominator.TabManager.ToolTabs.GetSingletonToolTabs();
                    break;
                case SocialNetworks.Gplus:                   
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Facebook:
                    break;
                case SocialNetworks.Quora:
                    break;
                case SocialNetworks.LinkedIn:
                    break;
                case SocialNetworks.Youtube:
                    break;

            }
        }

        public void CallRespectiveView(SocialNetworks networks, string selectedAccounts)
        {
            switch (networks)
            {
                case SocialNetworks.Social:
                    SelectedUserControl = SocialAutoActivity.GetSingletonSocialAutoActivity();
                    InitializeAccounts();
                    break;
                case SocialNetworks.Instagram:
                    SelectedUserControl = GramDominatorUI.TabManager.ToolTabs.GetSingletonToolTabs();
                    var data = FollowConfiguration.GetSingeltonObjectFollowConfiguration();
                    data.AccountGrowthHeader.AccountItemSource = AccountsFileManager.GetUsers(SocialNetworks.Instagram);
                    data.AccountGrowthHeader.SelectedItem = selectedAccounts;
                    SelectedDominatorAccounts.GdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Twitter:
                    //SelectedUserControl= TwtDominatorUI.TabManager.ToolsTab.GetSingletonToolTabs();
                    //var tddata = TwtDominatorUI.TDViews.Tools.Follow.FollowConfiguration.GetSingeltonObjectFollowConfiguration();
                    //tddata.accountgrothHeader.AccountItemSource = AccountsFileManager.GetUsers(SocialNetworks.Twitter);
                    //tddata.accountgrothHeader.SelectedItem = selectedAccounts;
                    //SelectedDominatorAccounts.TdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Pinterest:
                    //SelectedUserControl = PinDominator.TabManager.ToolTabs.GetSingletonToolTabs();
                    //var pddata = PinDominator.PDViews.Tools.Follow.FollowConfiguration.GetSingeltonObjectFollowConfiguration();
                    //pddata.accountGrowthHeader.AccountItemSource = AccountsFileManager.GetUsers(SocialNetworks.Pinterest);
                    //pddata.accountGrowthHeader.SelectedItem = selectedAccounts;
                    //SelectedDominatorAccounts.PdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Gplus:                 
                    SelectedDominatorAccounts.GplusAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Reddit:
                    SelectedDominatorAccounts.RdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Facebook:
                    //SelectedUserControl =new FaceDominatorUI.FDViews.TabManager.ToolsTab();
                    //var fddata = FaceDominatorUI.FDViews.Tools.SendRequest.SendRequestTools
                    //    .GetSingeltonObjectSendRequestTools();
                    //fddata.accountGrowthHeader.AccountItemSource = AccountsFileManager.GetUsers(SocialNetworks.Facebook);
                    //fddata.accountGrowthHeader.SelectedItem = selectedAccounts;                  
                    //SelectedDominatorAccounts.FdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Quora:
                    SelectedDominatorAccounts.QdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.LinkedIn:
                    SelectedDominatorAccounts.LdAccounts = selectedAccounts;
                    break;
                case SocialNetworks.Youtube:
                    SelectedDominatorAccounts.YdAccounts = selectedAccounts;
                    break;
            }
        }


        public ObservableCollection<AccountsActivityDetailModel> AccountsCollection { get; set; }

        public void InitializeAccounts()
        {
            var accounts = AccountsFileManager.GetAll();

            AccountsCollection =
                new ObservableCollection<AccountsActivityDetailModel>();

            if (accounts != null)
                foreach (var account in accounts)
                {
                    var accountsActivityDetailModel = new AccountsActivityDetailModel
                    {
                        DominatorAccountModel = account,
                        AutoActivityModuleDetailsCollections = new ObservableCollection<AutoActivityModuleDetails>()
                    };

                    account.ActivityManager.LstModuleConfiguration.ForEach(x =>
                    {
                        var randomTotal = RandomUtilties.GetRandomNumber(100, 0);
                        var randomCompleted = RandomUtilties.GetRandomNumber(randomTotal, 0);

                        var activityDetailsModel = new ActivityDetailsModel()
                        {
                            Title = x.ActivityType.ToString(),
                            Status = x.IsEnabled ? "Active" : "InActive",
                            Ratio = new ActivityRatio()
                            {
                                // Once total and Completed value are binded then remove the following two lines                               
                                Total = randomTotal,
                                Completed = randomCompleted

                                // Uncomment once done
                                //    Total = x.MaximumCountPerDay,
                                //    Completed = x.MaximumCountPerDay - 1 <= 0 ? x.MaximumCountPerDay : x.MaximumCountPerDay - 1
                            }
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
                            var randomTotal = RandomUtilties.GetRandomNumber(100, 0);
                            var randomCompleted = RandomUtilties.GetRandomNumber(randomTotal, 0);

                            var activityDetailsModel = new ActivityDetailsModel()
                            {
                                Title = x.ToString(),
                                Status = "InActive",
                                Ratio = new ActivityRatio()
                                {
                                    // Once total and Completed value are binded then remove the following two lines                               
                                    Total = randomTotal,
                                    Completed = randomCompleted

                                    // Uncomment once done
                                    //    Total = x.MaximumCountPerDay,
                                    //    Completed = x.MaximumCountPerDay - 1 <= 0 ? x.MaximumCountPerDay : x.MaximumCountPerDay - 1
                                }
                            };

                            var autoActivityModuleDetails = new AutoActivityModuleDetails
                            {
                                ActivityDetailsModel = activityDetailsModel
                            };

                            accountsActivityDetailModel.AutoActivityModuleDetailsCollections.Add(
                                autoActivityModuleDetails);
                        });
                    }

                    accountsActivityDetailModel.IsExpand = true;
                    AccountsCollection.Add(accountsActivityDetailModel);
                }
            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }


    }
}
