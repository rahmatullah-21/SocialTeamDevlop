using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class AccountDetailsSelectorViewModel : BindableBase
    {
        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            TextSearchCommand = new BaseCommand<object>(TextSearchCanExecute, TextSearchExecute);
        }


        public ICommand TextSearchCommand { get; set; }

        private bool TextSearchCanExecute(object sender) => true;

        private void TextSearchExecute(object sender)
        {
           
                if (string.IsNullOrEmpty(TextSearch) || string.IsNullOrWhiteSpace(TextSearch))
                    AccountDetailsSelectorView.Filter += null;
                else
                    AccountDetailsSelectorView.Filter += FilterByText;
          

        }

        private bool FilterByText(object sender)
        {
                try
                {
                    var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                    return objAccountDetailsSelectorModel != null &&
                           (objAccountDetailsSelectorModel.DetailName.
                                IndexOf(TextSearch, StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                            objAccountDetailsSelectorModel.DetailUrl.
                                IndexOf(TextSearch, StringComparison.InvariantCultureIgnoreCase) >= 0);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            return false;
        }


        private bool _isAllCampaignSelected;

        public bool IsAllCampaignSelected
        {
            get
            {
                return _isAllCampaignSelected;
            }
            set
            {
                if (_isAllCampaignSelected == value)
                    return;
                SetProperty(ref _isAllCampaignSelected, value);

                if (_isAllCampaignSelected)
                    SelectAllCampaign();
                else
                    SelectNoneCampaign();
            }
        }

        public void SelectAllCampaign()
        {
            ListAccountDetailsSelectorModels.Select(x =>
            {
                x.IsSelected = true; return x;
            }).ToList();
        }

        public void SelectNoneCampaign()
        {
            ListAccountDetailsSelectorModels.Select(x =>
            {
                x.IsSelected = false; return x;
            }).ToList();
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

        private bool _isProgressRingActive = true;

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
                if (_accountDetailsSelectorView == value)
                    return;
                _accountDetailsSelectorView = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorView));
            }
        }

        private string _detailsNameHeader = string.Empty;

        public string DetailsNameHeader
        {
            get
            {
                return _detailsNameHeader;
            }
            set
            {
                if (_detailsNameHeader == value)
                    return;
                _detailsNameHeader = value;
                OnPropertyChanged(nameof(DetailsNameHeader));
            }
        }

        private string _detailsUrlHeader = string.Empty;

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


        private string _title = string.Empty;

        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (_title == value)
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


        private List<string> _alreadySelectedList = new List<string>();

        public List<string> AlreadySelectedList
        {
            get
            {
                return _alreadySelectedList;
            }
            set
            {
                if (_alreadySelectedList == value)
                    return;
                _alreadySelectedList = value;
                OnPropertyChanged(nameof(AlreadySelectedList));
            }
        }

        private string _textSearch;

        public string TextSearch
        {
            get
            {
                return _textSearch;
            }
            set
            {
                _textSearch = value;
                OnPropertyChanged(nameof(TextSearch));
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