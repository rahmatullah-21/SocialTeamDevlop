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


        public void AccountDetailsBuilder(IEnumerable<AccountDetailsSelectorModel> accountDetails, string title,string detailNameHeader,string detaildUrlHeader)
        {
            ListAccountDetailsSelectorModels = new ObservableCollection<AccountDetailsSelectorModel>(accountDetails);
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            Title = title;
            DetailsUrlHeader = detaildUrlHeader;
            DetailsNameHeader = detailNameHeader;
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