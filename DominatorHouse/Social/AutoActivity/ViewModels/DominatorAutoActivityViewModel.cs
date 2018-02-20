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

namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public class DominatorAutoActivityViewModel : BindableBase
    {

        private DominatorAutoActivityViewModel()
        {
            _selectedUserControl = new UserControl();
            AccountsCollection = new ObservableCollection<DominatorAccountModel>();
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

        public ObservableCollection<DominatorAccountModel> AccountsCollection { get; set; }

        private void InitializeAccounts()
        {

            AccountsCollection =
                new ObservableCollection<DominatorAccountModel>(AccountsFileManager.GetAll());

            AccountsCollectionView = CollectionViewSource.GetDefaultView(AccountsCollection);

        }


    }
}
