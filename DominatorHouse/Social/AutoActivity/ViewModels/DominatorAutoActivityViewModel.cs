using System.Collections.ObjectModel;
using System.Windows.Controls;
using DominatorHouse.Social.AutoActivity.Views;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;

namespace DominatorHouse.Social.AutoActivity.ViewModels
{
    public class DominatorAutoActivityViewModel  : BindableBase
    {

        private DominatorAutoActivityViewModel()
        {
            _selectedUserControl = new UserControl();
            ObservableCollection = new ObservableCollection<DominatorAccountModel>() ;

        }

        private static DominatorAutoActivityViewModel ObjDominatorAutoActivityViewModel { get; set; } = null;

        public static DominatorAutoActivityViewModel GetSingletonDominatorAutoActivityViewModel()
        {
            return ObjDominatorAutoActivityViewModel ?? (ObjDominatorAutoActivityViewModel = new DominatorAutoActivityViewModel());
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

        public void CallRespectiveView(SocialNetworks networks)
        {
            switch (networks)
            {
                case SocialNetworks.Social:
                    SelectedUserControl = SocialAutoActivity.GetSingletonSocialAutoActivity();
                    break;
                case SocialNetworks.Instagram:
                    SelectedUserControl = GramDominatorUI.TabManager.ToolTabs.GetSingletonToolTabs();
                    break;

                case SocialNetworks.Twitter:
                    //SelectedUserControl= TwtDominatorUI.TabManager.ToolsTab.GetSingletonToolTabs();
                    break;          
            }
        }

        public ObservableCollection<DominatorAccountModel> ObservableCollection { get; set; } 




    }
}
