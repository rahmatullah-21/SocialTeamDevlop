using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class AccountDetailsSelectorViewModel : BindableBase
    {
        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
        }

        private ObservableCollection<AccountDetailsSelectorModel> _listAccountDetailsSelectorModels = new ObservableCollection<AccountDetailsSelectorModel>();

        public ObservableCollection<AccountDetailsSelectorModel> ListAccountDetailsSelectorModels
        {
            get
            {
                return _listAccountDetailsSelectorModels;
            }
            set
            {
                if (_listAccountDetailsSelectorModels == value)
                    return;
                _listAccountDetailsSelectorModels = value;
                OnPropertyChanged(nameof(ListAccountDetailsSelectorModels));
            }
        }

        private bool _isProgressRingActive=true;

        public bool IsProgressRingActive
        {
            get
            {
                return _isProgressRingActive;
            }
            set
            {
                if (_isProgressRingActive == value)
                    return;
                _isProgressRingActive = value;
                OnPropertyChanged(nameof(IsProgressRingActive));
            }
        }


        private ICollectionView _accountDetailsSelectorView;

        public ICollectionView AccountDetailsSelectorView
        {
            get
            {
                return _accountDetailsSelectorView;
            }
            set
            {
                if(_accountDetailsSelectorView == value)
                    return;
                _accountDetailsSelectorView = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorView));
            }
        }

        private string _detailsNameHeader=string.Empty;

        public string DetailsNameHeader
        {
            get
            {
                return _detailsNameHeader;
            }
            set
            {
                if(_detailsNameHeader == value)
                    return;
                _detailsNameHeader = value;
                OnPropertyChanged(nameof(DetailsNameHeader));
            }
        }

        private string _detailsUrlHeader= string.Empty;

        public string DetailsUrlHeader
        {
            get
            {
                return _detailsUrlHeader;
            }
            set
            {
                if (_detailsUrlHeader == value)
                    return;
                _detailsUrlHeader = value;
                OnPropertyChanged(nameof(DetailsUrlHeader));
            }
        }


        private string _title= string.Empty;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if(_title == value)
                    return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }


        private string _statusText = "Fetching..";

        public string StatusText
        {
            get
            {
                return _statusText;
            }
            set
            {
                if (_statusText == value)
                    return;
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }


        public IEnumerable<KeyValuePair<string, string>> GetSelectedItems()
        {
            var selectedItems = new List<KeyValuePair<string, string>>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedItems.Add(new KeyValuePair<string, string>(x.AccountId, x.DetailUrl));
            });
            return selectedItems;
        }
    }
}