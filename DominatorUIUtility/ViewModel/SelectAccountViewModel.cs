using System.Collections.ObjectModel;
using System.ComponentModel;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel
{
    class SelectAccountViewModel : BindableBase
    {
        private bool _isAccountSelected;
        public bool IsAccountSelected
        {
            get
            {
                return _isAccountSelected;
            }
            set
            {
                if (_isAccountSelected == value)
                    return;
                SetProperty(ref _isAccountSelected, value);

            }
        }
        private bool _isAllAccountSelected;
        public bool IsAllAccountSelected
        {
            get
            {
                return _isAllAccountSelected;
            }
            set
            {
                if (_isAllAccountSelected == value)
                    return;
                SetProperty(ref _isAllAccountSelected, value);

            }
        }

        private string _userName;
        public string UserName
        {
            get
            {
                return _userName;
            }
            set
            {
                if (_userName == value)
                    return;
                SetProperty(ref _userName, value);

            }
        }
        private string _groupName;
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                if (_groupName == value)
                    return;
                SetProperty(ref _groupName, value);

            }
        }


        private ICollectionView _accountCollectionView;

        public ICollectionView AccountCollectionView
        {
            get
            {
                return _accountCollectionView;
            }
            set
            {
                if (_accountCollectionView != null && _accountCollectionView == value)
                    return;
                SetProperty(ref _accountCollectionView, value);

            }
        }
        private ObservableCollection<ContentSelectGroup> _groups = new ObservableCollection<ContentSelectGroup>();
        public ObservableCollection<ContentSelectGroup> Groups {
            get
            {
                return _groups;
            }
            set
            {
                if (_groups == value)
                    return;
                SetProperty(ref _groups, value);

            }
        } 


        public ObservableCollection<SelectAccountViewModel> LstSelectAccount { get; set; } = new ObservableCollection<SelectAccountViewModel>();

    }
}
