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
        public AccountDetailsSelectorViewModel()
        {
            AccountDetailsSelectorView = CollectionViewSource.GetDefaultView(ListAccountDetailsSelectorModels);
            TextSearchCommand = new BaseCommand<object>(TextSearchCanExecute, TextSearchExecute);
            OwnPageCheckedCommand = new BaseCommand<object>(OwnPageCheckedCanExecute, OwnPageCheckedExecute);
            LikedPageCheckedCommand = new BaseCommand<object>(LikedPageCheckedCanExecute, LikedPageCheckedExecute);
            OwnGroupCheckedCommand = new BaseCommand<object>(OwnGroupCheckedCanExecute, OwnGroupCheckedExecute);
            JoinedGroupCheckedCommand = new BaseCommand<object>(JoinedGroupCheckedCanExecute, JoinedGroupCheckedExecute);
        }


        public ICommand TextSearchCommand { get; set; }
        public ICommand OwnPageCheckedCommand { get; set; }
        public ICommand LikedPageCheckedCommand { get; set; }
        public ICommand OwnGroupCheckedCommand { get; set; }
        public ICommand JoinedGroupCheckedCommand { get; set; }


        private bool OwnPageCheckedCanExecute(object sender) => true;

        private bool LikedPageCheckedCanExecute(object sender) => true;

        private bool OwnGroupCheckedCanExecute(object sender) => true;
        private bool JoinedGroupCheckedCanExecute(object sender) => true;

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
                else
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
                else
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
                else
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
                else
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
                else
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
                else
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
                else
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
                else
                    return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

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
            List<AccountDetailsSelectorModel> CallsList = AccountDetailsSelectorView
                                        .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = true; return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = true; return x;
            //}).ToList();
        }

        public void SelectNoneCampaign()
        {
            List<AccountDetailsSelectorModel> CallsList = AccountDetailsSelectorView
                                       .Cast<AccountDetailsSelectorModel>().ToList();

            CallsList.Select(x =>
            {
                x.IsSelected = false; return x;
            }).ToList();
            //ListAccountDetailsSelectorModels.Select(x =>
            //{
            //    x.IsSelected = false; return x;
            //}).ToList();
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

        private bool _isOwnPageSelected = true;

        public bool IsOwnPageSelected
        {
            get
            {
                return _isOwnPageSelected;
            }
            set
            {
                if (_isOwnPageSelected == value)
                    return;
                _isOwnPageSelected = value;
                OnPropertyChanged(nameof(IsOwnPageSelected));
            }
        }

        private bool _isPageOptionVisible = true;

        public bool IsPageOptionVisible
        {
            get
            {
                return _isPageOptionVisible;
            }
            set
            {
                if (_isPageOptionVisible == value)
                    return;
                _isPageOptionVisible = value;
                OnPropertyChanged(nameof(IsPageOptionVisible));
            }
        }


        private bool _isLikedPageSelected = true;

        public bool IsLikedPageSelected
        {
            get
            {
                return _isLikedPageSelected;
            }
            set
            {
                if (_isLikedPageSelected == value)
                    return;
                _isLikedPageSelected = value;
                OnPropertyChanged(nameof(IsLikedPageSelected));
            }
        }


        private bool _isGroupOptionVisible = true;

        public bool IsGroupOptionVisible
        {
            get
            {
                return _isGroupOptionVisible;
            }
            set
            {
                if (_isGroupOptionVisible == value)
                    return;
                _isGroupOptionVisible = value;
                OnPropertyChanged(nameof(_isGroupOptionVisible));
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