using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
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
        private ICollectionView _accountDetailsSelectorView;


        private List<string> _alreadySelectedList = new List<string>();

        private string _detailsNameHeader = string.Empty;

        private string _detailsUrlHeader = string.Empty;


        private bool _isAllCampaignSelected;


        private bool _isGroupOptionVisible = true;


        private bool _isLikedPageSelected = true;

        private bool _isOwnPageSelected = true;

        private bool _isPageOptionVisible = true;

        private bool _isProgressRingActive = true;

        private ObservableCollection<AccountDetailsSelectorModel> _listAccountDetailsSelectorModels =
            new ObservableCollection<AccountDetailsSelectorModel>();


        private string _statusText = "Fetching..";

        private string _textSearch;


        private string _title = string.Empty;

        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            TextSearchCommand = new BaseCommand<object>(TextSearchCanExecute, TextSearchExecute);
            OwnPageCheckedCommand = new BaseCommand<object>(OwnPageCheckedCanExecute, OwnPageCheckedExecute);
            LikedPageCheckedCommand = new BaseCommand<object>(LikedPageCheckedCanExecute, LikedPageCheckedExecute);
            OwnGroupCheckedCommand = new BaseCommand<object>(OwnGroupCheckedCanExecute, OwnGroupCheckedExecute);
            JoinedGroupCheckedCommand =
                new BaseCommand<object>(JoinedGroupCheckedCanExecute, JoinedGroupCheckedExecute);
        }


        public ICommand TextSearchCommand { get; set; }
        public ICommand OwnPageCheckedCommand { get; set; }
        public ICommand LikedPageCheckedCommand { get; set; }
        public ICommand OwnGroupCheckedCommand { get; set; }
        public ICommand JoinedGroupCheckedCommand { get; set; }

        public bool IsAllCampaignSelected
        {
            get => _isAllCampaignSelected;
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

        public ObservableCollection<AccountDetailsSelectorModel> ListAccountDetailsSelectorModels
        {
            get => _listAccountDetailsSelectorModels;
            set
            {
                if (_listAccountDetailsSelectorModels == value)
                    return;
                _listAccountDetailsSelectorModels = value;
                OnPropertyChanged(nameof(ListAccountDetailsSelectorModels));
            }
        }

        public bool IsProgressRingActive
        {
            get => _isProgressRingActive;
            set
            {
                if (_isProgressRingActive == value)
                    return;
                _isProgressRingActive = value;
                OnPropertyChanged(nameof(IsProgressRingActive));
            }
        }

        public ICollectionView AccountDetailsSelectorView
        {
            get => _accountDetailsSelectorView;
            set
            {
                if (_accountDetailsSelectorView == value)
                    return;
                _accountDetailsSelectorView = value;
                OnPropertyChanged(nameof(AccountDetailsSelectorView));
            }
        }

        public string DetailsNameHeader
        {
            get => _detailsNameHeader;
            set
            {
                if (_detailsNameHeader == value)
                    return;
                _detailsNameHeader = value;
                OnPropertyChanged(nameof(DetailsNameHeader));
            }
        }

        public string DetailsUrlHeader
        {
            get => _detailsUrlHeader;
            set
            {
                if (_detailsUrlHeader == value)
                    return;
                _detailsUrlHeader = value;
                OnPropertyChanged(nameof(DetailsUrlHeader));
            }
        }

        public string Title
        {
            get => _title;
            set
            {
                if (_title == value)
                    return;
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public string StatusText
        {
            get => _statusText;
            set
            {
                if (_statusText == value)
                    return;
                _statusText = value;
                OnPropertyChanged(nameof(StatusText));
            }
        }

        public List<string> AlreadySelectedList
        {
            get => _alreadySelectedList;
            set
            {
                if (_alreadySelectedList == value)
                    return;
                _alreadySelectedList = value;
                OnPropertyChanged(nameof(AlreadySelectedList));
            }
        }

        public string TextSearch
        {
            get => _textSearch;
            set
            {
                _textSearch = value;
                OnPropertyChanged(nameof(TextSearch));
            }
        }

        public bool IsOwnPageSelected
        {
            get => _isOwnPageSelected;
            set
            {
                if (_isOwnPageSelected == value)
                    return;
                _isOwnPageSelected = value;
                OnPropertyChanged(nameof(IsOwnPageSelected));
            }
        }

        public bool IsPageOptionVisible
        {
            get => _isPageOptionVisible;
            set
            {
                if (_isPageOptionVisible == value)
                    return;
                _isPageOptionVisible = value;
                OnPropertyChanged(nameof(IsPageOptionVisible));
            }
        }

        public bool IsLikedPageSelected
        {
            get => _isLikedPageSelected;
            set
            {
                if (_isLikedPageSelected == value)
                    return;
                _isLikedPageSelected = value;
                OnPropertyChanged(nameof(IsLikedPageSelected));
            }
        }

        public bool IsGroupOptionVisible
        {
            get => _isGroupOptionVisible;
            set
            {
                if (_isGroupOptionVisible == value)
                    return;
                _isGroupOptionVisible = value;
                OnPropertyChanged(nameof(_isGroupOptionVisible));
            }
        }


        private bool OwnPageCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool LikedPageCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool OwnGroupCheckedCanExecute(object sender)
        {
            return true;
        }

        private bool JoinedGroupCheckedCanExecute(object sender)
        {
            return true;
        }

        private void OwnPageCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;

            if (IsOwnPageSelected)
                AccountDetailsSelectorView.Filter += FilterByOwnPageSelect;
            else
                AccountDetailsSelectorView.Filter += FilterByOwnPageRemove;
        }

        private bool FilterByOwnPageRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           !objAccountDetailsSelectorModel.IsOwnPage;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByOwnPageSelect(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           objAccountDetailsSelectorModel.IsOwnPage;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void LikedPageCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;
            if (IsLikedPageSelected)
                AccountDetailsSelectorView.Filter += FilterByLikedPageSelected;
            else
                AccountDetailsSelectorView.Filter += FilterByLikedPageRemove;
        }

        private bool FilterByLikedPageSelected(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           objAccountDetailsSelectorModel.IsLikePage;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByLikedPageRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsFanpage &&
                           !objAccountDetailsSelectorModel.IsLikePage;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private void OwnGroupCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;
            if (IsOwnPageSelected)
                AccountDetailsSelectorView.Filter += FilterByOwnGroupSelect;
            else
                AccountDetailsSelectorView.Filter += FilterByOwnGroupRemove;
        }

        private void JoinedGroupCheckedExecute(object sender)
        {
            AccountDetailsSelectorView.Filter += null;

            if (IsLikedPageSelected)
                AccountDetailsSelectorView.Filter += FilterByJoinedGroupSelected;
            else
                AccountDetailsSelectorView.Filter += FilterByJoinedGroupRemove;
        }

        private bool FilterByJoinedGroupRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           !objAccountDetailsSelectorModel.IsJoinedGroup;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }


        private bool FilterByOwnGroupSelect(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsOwnPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           objAccountDetailsSelectorModel.IsOwnGroup;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByOwnGroupRemove(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           !objAccountDetailsSelectorModel.IsOwnGroup;
                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByJoinedGroupSelected(object sender)
        {
            try
            {
                var objAccountDetailsSelectorModel = sender as AccountDetailsSelectorModel;

                if (!IsLikedPageSelected)
                    return objAccountDetailsSelectorModel != null &&
                           objAccountDetailsSelectorModel.IsGroup &&
                           objAccountDetailsSelectorModel.IsJoinedGroup;
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool TextSearchCanExecute(object sender)
        {
            return true;
        }

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
                       (objAccountDetailsSelectorModel.DetailName.IndexOf(TextSearch,
                            StringComparison.InvariantCultureIgnoreCase) >= 0 ||
                        objAccountDetailsSelectorModel.DetailUrl.IndexOf(TextSearch,
                            StringComparison.InvariantCultureIgnoreCase) >= 0);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public void SelectAllCampaign()
        {
            var CallsList = AccountDetailsSelectorView
                .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = true;
                return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = true; return x;
            //}).ToList();
        }

        public void SelectNoneCampaign()
        {
            var CallsList = AccountDetailsSelectorView
                .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = false;
                return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = false; return x;
            //}).ToList();
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

        public IEnumerable<KeyValuePair<string, string>> GetNonSelectedItems()
        {
            var selectedItems = new List<KeyValuePair<string, string>>();
            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (!x.IsSelected)
                    selectedItems.Add(new KeyValuePair<string, string>(x.AccountId, x.DetailUrl));
            });
            return selectedItems;
        }

        public IEnumerable<PublisherDestinationDetailsModel> GetSelectedItemsDestinations(string destinationType)
        {
            var selectedDestinations = new List<PublisherDestinationDetailsModel>();

            ListAccountDetailsSelectorModels.ForEach(x =>
            {
                if (x.IsSelected)
                    selectedDestinations.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = x.AccountId,
                        DestinationType = destinationType,
                        DestinationUrl = x.DetailUrl,
                        SocialNetworks = x.Network,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = x.AccountName
                    });
            });

            return selectedDestinations;
        }
    }
}