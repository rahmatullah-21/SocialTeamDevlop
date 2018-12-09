using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateDestinationsViewModel : BindableBase
    {
        //ConstructorS
        private readonly IAccountsFileManager _accountsFileManager;
        public PublisherCreateDestinationsViewModel()
        {
            _accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            GetSingleAccountGroupsCommand = new BaseCommand<object>(GetSingleAccountGroupsCanExecute, GetSingleAccountGroupsExecute);
            GetSingleAccountPagesOrBoardsCommand = new BaseCommand<object>(GetSingleAccountPagesOrBoardsCanExecute, GetSingleAccountPagesOrBoardsExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectAllAccountDetailsCommand = new BaseCommand<object>(SelectAccountDetailsCanExecute, SelectAccountDetailsExecute);
            SaveDestinationCommand = new BaseCommand<object>(SaveDestinationCanExecute, SaveDestinationExecute);
            ClearCommand = new BaseCommand<object>(ClearCanExecute, ClearExecute);
            StatusSyncCommand = new BaseCommand<object>(SyncCanExecute, SyncExecute);
            AddFreshAccounts = new BaseCommand<object>(AddFreshAccountCanExecute, AddFreshAccountExecute);
            AddCustomDestinationCommand = new BaseCommand<object>(AddCustomDestinationCanExecute, AddCustomDestinationExecute);
            NetworkSelectionChangedCommand = new BaseCommand<object>((sender) => true, NetworkSelectionChangedExecute);
            InitializeProperties();
            InitializeDestinationList();
            IsSavedDestination = false;
        }

        private void NetworkSelectionChangedExecute(object sender)
        {
            try
            {
                SelectedNetworks = (SocialNetworks)sender;
                FilterByNetwork();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void FilterByNetwork()
        {
            try
            {
                if (SelectedNetworks == SocialNetworks.Social)
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        DestinationCollectionView.Filter = x =>
                            ((PublisherCreateDestinationSelectModel)x).AccountName.IndexOf(FilterText,
                                StringComparison.CurrentCultureIgnoreCase) >= 0;
                    else DestinationCollectionView.Filter = (x) => true;
                }
                else
                {
                    if (!string.IsNullOrEmpty(FilterText))
                        DestinationCollectionView.Filter =
                            x => ((PublisherCreateDestinationSelectModel)x).SocialNetworks == SelectedNetworks && ((PublisherCreateDestinationSelectModel)x).AccountName.IndexOf(_filterText, StringComparison.CurrentCultureIgnoreCase) >= 0;
                    else DestinationCollectionView.Filter = x => ((PublisherCreateDestinationSelectModel)x).SocialNetworks == SelectedNetworks;

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void InitializeProperties()
        {
            Title = "Create Destination";
            IsAllDestinationSelected = false;
            EditDestinationId = string.Empty;
            IsSavedDestination = false;
            PublisherCreateDestinationModel = PublisherCreateDestinationModel.DestinationDefaultBuilder();
        }

        #region Commands
        public ICommand AddFreshAccounts { get; set; }

        public ICommand StatusSyncCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand NavigationCommand { get; set; }

        public ICommand GetSingleAccountGroupsCommand { get; set; }

        public ICommand SelectAllAccountDetailsCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand GetSingleAccountPagesOrBoardsCommand { get; set; }

        public ICommand SaveDestinationCommand { get; set; }

        public ICommand AddCustomDestinationCommand { get; set; }
        public ICommand NetworkSelectionChangedCommand { get; set; }
        #endregion

        #region Properties

        private SocialNetworks _selectedNetworks = SocialNetworks.Social;

        public SocialNetworks SelectedNetworks
        {
            get
            {
                return _selectedNetworks;
            }
            set
            {
                if (_selectedNetworks == value)
                    return;
                SetProperty(ref _selectedNetworks, value);
            }
        }

        private string _filterText;

        public string FilterText
        {
            get
            {
                return _filterText;
            }
            set
            {
                if (_filterText == value)
                    return;
                SetProperty(ref _filterText, value);
                FilterByNetwork();
            }
        }
        private HashSet<SocialNetworks> _availableNetworks = SocinatorInitialize.AvailableNetworks;
        public HashSet<SocialNetworks> AvailableNetworks
        {
            get
            {
                return _availableNetworks;
            }
            set
            {
                _availableNetworks = value;
                OnPropertyChanged(nameof(AvailableNetworks));
            }
        }

        private PublisherCreateDestinationModel _publisherCreateDestinationModel = PublisherCreateDestinationModel.DestinationDefaultBuilder();

        public PublisherCreateDestinationModel PublisherCreateDestinationModel
        {
            get
            {
                return _publisherCreateDestinationModel;
            }
            set
            {
                if (_publisherCreateDestinationModel == value)
                    return;
                _publisherCreateDestinationModel = value;
                OnPropertyChanged(nameof(PublisherCreateDestinationModel));
            }
        }

        private ICollectionView _destinationCollectionView;

        public ICollectionView DestinationCollectionView
        {
            get
            {
                return _destinationCollectionView;
            }
            set
            {
                if (_destinationCollectionView != null && _destinationCollectionView == value)
                    return;
                SetProperty(ref _destinationCollectionView, value);

            }
        }

        public string EditDestinationId { get; set; } = string.Empty;

        public bool IsSavedDestination { get; set; }

        private string _title = string.Empty;
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                _title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        private bool _isAllDestinationSelected;
        public bool IsAllDestinationSelected
        {
            get
            {
                return _isAllDestinationSelected;
            }
            set
            {
                if (_isAllDestinationSelected == value)
                    return;
                SetProperty(ref _isAllDestinationSelected, value);
                SelectAllDestination(_isAllDestinationSelected);
                _isUncheckedFromList = false;
            }
        }


        private List<string> _needToUpdateAccounts = new List<string>();


        public List<string> GroupsAvailableInNetworks { get; set; } = new List<string> { "Facebook", "LinkedIn", "Reddit" };

        public List<string> BoardsOrPagesAvailableInNetworks { get; set; } = new List<string> { "Facebook", "Youtube", "Pinterest", "LinkedIn", "Gplus", "Tumblr" };


        #endregion

        #region Navigation

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                // Send back to manage destinations
                case "Back":
                    ClearCurrentDestination();
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherManageDestinations.Instance();
                    break;
            }
        }

        #endregion

        #region Get Single Account Groups Details

        private bool GetSingleAccountGroupsCanExecute(object sender) => true;

        private void GetSingleAccountGroupsExecute(object sender)
        {
            // get the selected accounts destinations model
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            // get already selected group pairs
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            // Get the initial selector details and also passing the action for getting the group details
            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountGroupsDetails, publisherCreateDestinationSelectModel)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "Select Groups",
                    DetailsUrlHeader = "Group Url",
                    DetailsNameHeader = "Group Name",
                    AlreadySelectedList = alreadySelectedGroups
                }
            };

            var dialog = new Dialog();

            // display the dialog window
            var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Groups");

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove already saved group pairs 
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountGroupPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y => x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                // get currectly selected groups from UI objects
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // get the full destination details
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                // Append with destination details of the accounts
                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Add to account's group pair
                PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

                alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

                var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                // Get the group selector details
                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.GroupSelectorText = $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            // Trigger the action
            accountDetailsSelector.UpdateUiSingleData();

        }

        private async Task UpdateSingleAccountGroupsDetails(AccountDetailsSelector accountDetailsSelector, PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            // Get the account group pair
            // var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            // Get already selected groups
            // var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var alreadySelectedGroups = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedList;

            if (GroupsAvailableInNetworks.Contains(publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                // Get the factory for account selector for a network
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                // fetch the groups details for particular accounts
                var groups = await accountsDetailsSelector.GetGroupsDetails(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, alreadySelectedGroups);

                groups.ForEach(group =>
                {
                    group.Network = publisherCreateDestinationSelectModel.SocialNetworks;
                    group.IsSelected = alreadySelectedGroups.Contains(group.DetailUrl);
                    // Add the group details to Ui's view model 
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group));
                    }
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group);
                });
            }

            // Update the status of details selector
            UpdateStatus(accountDetailsSelector);
        }

        #endregion

        #region Get Single Account Page Details

        private bool GetSingleAccountPagesOrBoardsCanExecute(object sender) => true;

        private void GetSingleAccountPagesOrBoardsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            // get the page or board pair from collection with the account Id
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            // get the factory pattern for the network of an account
            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            // Fetch the page details only
            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            // Pass the fetching activity functions as action to UI
            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountPagesDetails, publisherCreateDestinationSelectModel)
            {
                // Find whether page or board, its vary based on each network
                AccountDetailsSelectorViewModel =
                {
                    Title = $"Select {accountsDetailsSelector.DisplayAsPageOrBoards}",
                    DetailsUrlHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Url",
                    DetailsNameHeader = $"{accountsDetailsSelector.DisplayAsPageOrBoards} Name",
                    AlreadySelectedList = alreadySelectedPages
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, $"Select {accountsDetailsSelector.DisplayAsPageOrBoards}");

            // Defining the save buttons click events
            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                // Remove all the saved accounts page or boards pair
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y => x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                // Get the selected pairs
                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                // Add to key value pair
                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                // Get the destination full details of a page or board
                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                // Update with destination details
                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                // Get the already selected page details 
                alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

                //
                var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.PagesOrBoardsSelectorText = $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiSingleData();
        }

        private async Task UpdateSingleAccountPagesDetails(AccountDetailsSelector accountDetailsSelector, PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            //var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            //var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var alreadySelectedPages = accountDetailsSelector.AccountDetailsSelectorViewModel.AlreadySelectedList;
            if (BoardsOrPagesAvailableInNetworks.Contains(publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var pagesOrBoards = await accountsDetailsSelector.GetPagesDetails(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, alreadySelectedPages);

                pagesOrBoards.ForEach(page =>
                {
                    page.Network = publisherCreateDestinationSelectModel.SocialNetworks;
                    page.IsSelected = alreadySelectedPages.Contains(page.DetailUrl);
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page));
                    }
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(page);
                });
            }

            UpdateStatus(accountDetailsSelector);
        }


        #endregion

        #region Open Context

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #endregion

        #region Selection execution for all destination to select

        private bool SelectionCanExecute(object sender) => true;

        private void SelectionExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "MenuSelectNone":
                    IsAllDestinationSelected = false;
                    break;

                case "MenuSelectAll":
                    IsAllDestinationSelected = true;
                    break;
                case "SelectManually":

                    if (PublisherCreateDestinationModel.ListSelectDestination.All(x => x.IsAccountSelected))
                        IsAllDestinationSelected = true;
                    else
                    {
                        if (IsAllDestinationSelected)
                            _isUncheckedFromList = true;
                        IsAllDestinationSelected = false;
                    }
                    break;
            }
        }

        private bool _isUncheckedFromList { get; set; }
        #endregion

        #region Select Destination fucntionality , and also selection menu options

        public void SelectAllDestination(bool isChecked)
        {
            if (_isUncheckedFromList)
                return;
            PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
           {
               x.IsAccountSelected = isChecked;
               return x;
           }).ToList();
        }

        private bool SelectAccountDetailsCanExecute(object sender) => true;

        private void SelectAccountDetailsExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "OwnProfile":
                    PublisherCreateDestinationModel.ListSelectDestination.Select(x =>
                    {
                        x.PublishonOwnWall = true;
                        return x;
                    }).ToList();
                    break;
                case "Groups":
                    LoadAllAccountsGroup();
                    break;
                case "Pages":
                    LoadAllAccountsPagesOrBoards();
                    break;
            }
        }

        #endregion

        #region Get all account's pages and groups details

        public void LoadAllAccountsGroup()
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateAllGroupsDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "Select Groups",
                    DetailsUrlHeader = "Group Url",
                    DetailsNameHeader = "Group Name",
                    AlreadySelectedList = alreadySelectedGroups
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Groups");

            accountDetailsSelector.btnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountGroupPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y => x.Key == y.AccountId && y.DestinationType == ConstantVariable.Group);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItemsDestinations(ConstantVariable.Group).ToList();

                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                    var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == selectedItems.Key);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.GroupSelectorText = $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";
                });

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();

        }

        private void UpdateAllGroupsDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var count = PublisherCreateDestinationModel.ListSelectDestination.Count;

            PublisherCreateDestinationModel.ListSelectDestination.ForEach(async x =>
            {
                if (GroupsAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                {
                    var accountsDetailsSelector = SocinatorInitialize
                        .GetSocialLibrary(x.SocialNetworks)
                        .GetNetworkCoreFactory().AccountDetailsSelectors;

                    var groups = await accountsDetailsSelector.GetGroupsDetails(x.AccountId, x.AccountName, alreadySelectedGroups);

                    groups.ForEach(group =>
                    {
                        group.Network = x.SocialNetworks;
                        group.IsSelected = alreadySelectedGroups.Contains(group.DetailUrl);
                        if (!Application.Current.Dispatcher.CheckAccess())
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                                accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group));
                        }
                        else
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group);
                    });
                }

                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });

        }

        public void LoadAllAccountsPagesOrBoards()
        {
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdatePagesDetails)
            {
                AccountDetailsSelectorViewModel =
                {
                    Title = "Select Pages/Boards",
                    DetailsUrlHeader = "Pages/Boards Url",
                    DetailsNameHeader = "Pages/Boards Name",
                    AlreadySelectedList = alreadySelectedPages
                }
            };

            var dialog = new Dialog();

            var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Pages/Boards");

            accountDetailsSelector.btnSave.Click += (sender, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                    PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(y => x.Key == y.AccountId && y.DestinationType == ConstantVariable.PageOrBoard);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                var destinationDetails = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItemsDestinations(ConstantVariable.PageOrBoard).ToList();

                PublisherCreateDestinationModel.DestinationDetailsModels.AddRange(destinationDetails);

                keyValuePairs.ForEach(selectedItems =>
                {
                    alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(x => x.Key == selectedItems.Key).Select(x => x.Value).ToList();

                    var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == selectedItems.Key);

                    if (createDestinationSelectModel != null)
                        createDestinationSelectModel.PagesOrBoardsSelectorText = $"{alreadySelectedPages.Count}/{createDestinationSelectModel.TotalPagesOrBoards}";
                });
                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();
        }

        private void UpdatePagesDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.ToList();

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var count = PublisherCreateDestinationModel.ListSelectDestination.Count;

            PublisherCreateDestinationModel.ListSelectDestination.ForEach(async x =>
            {
                try
                {
                    if (BoardsOrPagesAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
                    {
                        var accountsDetailsSelector = SocinatorInitialize
                            .GetSocialLibrary(x.SocialNetworks)
                            .GetNetworkCoreFactory().AccountDetailsSelectors;

                        var pages = await accountsDetailsSelector.GetPagesDetails(x.AccountId, x.AccountName, alreadySelectedPages);

                        pages.ForEach(group =>
                        {
                            group.Network = x.SocialNetworks;
                            group.IsSelected = alreadySelectedPages.Contains(group.DetailUrl);
                            if (!Application.Current.Dispatcher.CheckAccess())
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                    accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group));
                            }
                            else
                                accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group);
                        });
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });
        }


        private static void UpdateStatus(AccountDetailsSelector accountDetailsSelector)
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                    accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText = accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0 ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !" : "No row(s) found !";
                });
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText = accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0 ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !" : "No row(s) found !";
            }
        }

        #endregion

        #region Initialize Updates

        public void InitializeDestinationList()
        {
            var accounts = _accountsFileManager.GetAll().Where(x => x.AccountBaseModel.Status == AccountStatus.Success);

            if (!Application.Current.CheckAccess())
                Application.Current.Dispatcher.Invoke(() => { PublisherCreateDestinationModel.ListSelectDestination.Clear(); });
            else
                PublisherCreateDestinationModel.ListSelectDestination.Clear();

            accounts.ForEach(x =>
            {

                var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    IsOwnWallAvailable = x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                    IsGroupsAvailable =
                        GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsPagesOrBoardsAvailable =
                        BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0,
                };

                publisherCreateDestinationSelectModel.GroupSelectorText =
                    GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                        : "NA";

                publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                    BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                        ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                        : "NA";

                if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork))
                {
                    PublisherCreateDestinationModel.ListSelectDestination.Add(publisherCreateDestinationSelectModel);
                }
            });

            DestinationCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);

        }

        #endregion

        #region Validate Destinations

        public bool IsDuplicate()
        {

            if (!string.IsNullOrEmpty(EditDestinationId))
                return false;

            var availableCount = PublisherManageDestinations.Instance().PublisherManageDestinationViewModel
                     .ListPublisherManageDestinationModels.Count;

            if (availableCount == 0)
                return false;

            // check destination name is already present or not 
            var isPresent = false;

            foreach (var x in PublisherManageDestinations.Instance().PublisherManageDestinationViewModel
                .ListPublisherManageDestinationModels)
            {
                if (x.DestinationName == PublisherCreateDestinationModel.DestinationName)
                    isPresent = true;

                if (isPresent)
                    break;
            }

            return isPresent;
        }

        #endregion

        #region Save Destinations

        private bool SaveDestinationCanExecute(object sender) => true;

        private void SaveDestinationExecute(object sender)
        {
            // Check whether destination name is already present or not 
            if (!IsDuplicate())
            {
                // Clear all pre saved selected accounts Id and own wall profile
                PublisherCreateDestinationModel.SelectedAccountIds.Clear();
                PublisherCreateDestinationModel.PublishOwnWallAccount.Clear();
                PublisherCreateDestinationModel.AccountsWithNetwork.Clear();

                var selectedAccountsCount =
                    PublisherCreateDestinationModel.ListSelectDestination.Count(x => x.IsAccountSelected);

                if (selectedAccountsCount == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                        "Warning", "Please select accounts, You have selected only destinations !");
                    return;
                }

                PublisherCreateDestinationModel.ListSelectDestination.ForEach(x =>
                {
                    // Check the account has been selected or not
                    if (x.IsAccountSelected)
                    {
                        PublisherCreateDestinationModel.SelectedAccountIds.Add(x.AccountId);
                        PublisherCreateDestinationModel.AccountsWithNetwork.Add(new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));

                        if (x.PublishonOwnWall)
                        {
                            PublisherCreateDestinationModel.PublishOwnWallAccount.Add(x.AccountId);
                            PublisherCreateDestinationModel.DestinationDetailsModels.Add(new PublisherDestinationDetailsModel
                            {
                                AccountId = x.AccountId,
                                SocialNetworks = x.SocialNetworks,
                                DestinationType = ConstantVariable.OwnWall,
                                DestinationUrl = x.AccountId,
                                PublisherPostlistModel = new PublisherPostlistModel(),
                                DestinationGuid = Utilities.GetGuid(),
                                AccountName = x.AccountName
                            });
                        }
                        else
                        {
                            PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(z =>
                                z.DestinationType == ConstantVariable.OwnWall && z.AccountId == x.AccountId);
                        }
                    }
                    else
                    {
                        // If account has selected, remove from selected lists
                        var unwantedGroups = PublisherCreateDestinationModel.AccountGroupPair.Where(y => y.Key == x.AccountId).Select(y => y.Key);
                        PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(z => unwantedGroups.Contains(z.Key));

                        var unwantedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(y => y.Key == x.AccountId).Select(y => y.Key);
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(z => unwantedPages.Contains(z.Key));

                        PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(z =>
                            z.AccountId == x.AccountId);

                        PublisherCreateDestinationModel.CustomDestinations.RemoveAll(z => z.Key == x.AccountId);
                    }
                });

                PublisherCreateDestinationModel.AccountGroupPair =
                    PublisherCreateDestinationModel.AccountGroupPair.Distinct().ToList();

                PublisherCreateDestinationModel.PublishOwnWallAccount =
                    PublisherCreateDestinationModel.PublishOwnWallAccount.Distinct().ToList();

                PublisherCreateDestinationModel.SelectedAccountIds =
                    PublisherCreateDestinationModel.SelectedAccountIds.Distinct().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair =
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Distinct().ToList();

                if (PublisherCreateDestinationModel.AccountGroupPair.Count == 0 &&
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Count == 0 &&
                    PublisherCreateDestinationModel.PublishOwnWallAccount.Count == 0 &&
                    PublisherCreateDestinationModel.CustomDestinations.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                        "Warning", "Please select destination!");
                    return;
                }

                // New Destination
                if (string.IsNullOrEmpty(EditDestinationId))
                {
                    PublisherCreateDestinationModel.AddDestination(PublisherCreateDestinationModel);

                    var publisherManageDestinationModel = new PublisherManageDestinationModel
                    {
                        AccountCount = PublisherCreateDestinationModel.SelectedAccountIds.Count,
                        CampaignsCount = 0,
                        CreatedDate = DateTime.Now,
                        DestinationId = PublisherCreateDestinationModel.DestinationId,
                        DestinationName = PublisherCreateDestinationModel.DestinationName,
                        GroupsCount = PublisherCreateDestinationModel.AccountGroupPair.Count,
                        IsSelected = false,
                        PagesOrBoardsCount = PublisherCreateDestinationModel.AccountPagesBoardsPair.Count,
                        WallsOrProfilesCount = PublisherCreateDestinationModel.PublishOwnWallAccount.Count,
                        CustomDestinationsCount = PublisherCreateDestinationModel.CustomDestinations.Count,
                        IsAddNewGroups = PublisherCreateDestinationModel.IsAddedNewGroups,
                        IsRemoveGroupsRequiresValidation = PublisherCreateDestinationModel.IsRemoveGroupsRequiresApproval
                    };

                    PublisherManageDestinations.Instance().PublisherManageDestinationViewModel.AddDestinations(
                        publisherManageDestinationModel, true);
                }
                // Edit Destination
                else
                {
                    PublisherCreateDestinationModel.UpdateDestination(PublisherCreateDestinationModel);

                    var publisherManageDestinationModel = PublisherManageDestinations.Instance().PublisherManageDestinationViewModel.GetManageDestination(EditDestinationId);

                    // To update the destination name
                    publisherManageDestinationModel.DestinationName
                        = PublisherCreateDestinationModel.DestinationName;

                    // To update the selected account count
                    publisherManageDestinationModel.AccountCount =
                        PublisherCreateDestinationModel.SelectedAccountIds.Count;

                    // To update the group count
                    publisherManageDestinationModel.GroupsCount =
                        PublisherCreateDestinationModel.AccountGroupPair.Count;

                    // To update the page or boards counts
                    publisherManageDestinationModel.PagesOrBoardsCount =
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.Count;

                    // To update the wall count 
                    publisherManageDestinationModel.WallsOrProfilesCount =
                        PublisherCreateDestinationModel.PublishOwnWallAccount.Count;

                    publisherManageDestinationModel.CustomDestinationsCount =
                        PublisherCreateDestinationModel.CustomDestinations.Count;

                    publisherManageDestinationModel.IsAddNewGroups
                        = PublisherCreateDestinationModel.IsAddedNewGroups;

                    publisherManageDestinationModel.IsRemoveGroupsRequiresValidation =
                        PublisherCreateDestinationModel.IsRemoveGroupsRequiresApproval;

                    // To call a method to update the manage destination user interface
                    PublisherManageDestinations.Instance().PublisherManageDestinationViewModel.UpdateDestinations(
                        publisherManageDestinationModel);
                }

                InitializeProperties();

                IsSavedDestination = true;

                PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                    = PublisherManageDestinations.Instance();
            }
            else
            {
                GlobusLogHelper.log.Info("Validation Failed!");
            }
        }

        #endregion

        #region Clear Destination



        private bool ClearCanExecute(object sender) => true;

        private void ClearExecute(object sender)
        {
            ClearCurrentDestination();
        }

        public void ClearCurrentDestination()
        {
            try
            {
                InitializeProperties();
                InitializeDestinationList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Sync Destination

        private bool SyncCanExecute(object sender) => true;

        private void SyncExecute(object sender)
        {
            try
            {
                var selectedSyncAccount = sender as PublisherCreateDestinationSelectModel;
                new Action(async () => { await UpdateSyncStatusAsync(selectedSyncAccount); }).Invoke();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public List<PublisherCreateDestinationSelectModel> GetSyncUpdateDestinations()
            => PublisherCreateDestinationModel.ListSelectDestination.Where(x => x.StatusSyncContent == ConstantVariable.NeedUpdateStatusSync).ToList();


        public async Task UpdateSyncStatusAsync(PublisherCreateDestinationSelectModel selectedSyncAccount)
        {
            var currentAccountDetails = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == selectedSyncAccount.AccountId);

            if (currentAccountDetails == null)
                return;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(selectedSyncAccount.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            var currentGroups = await accountsDetailsSelector.GetGroupsUrls(selectedSyncAccount.AccountId, selectedSyncAccount.AccountName);

            var currentPages = await accountsDetailsSelector.GetPageOrBoardUrls(selectedSyncAccount.AccountId, selectedSyncAccount.AccountName);

            PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(x => x.Key == selectedSyncAccount.AccountId && !currentGroups.Contains(x.Value));

            PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(x => x.Key == selectedSyncAccount.AccountId && !currentPages.Contains(x.Value));

            currentAccountDetails.TotalGroups = currentGroups.Count;

            currentAccountDetails.TotalPagesOrBoards = currentPages.Count;

            currentAccountDetails.SelectedGroups =
                PublisherCreateDestinationModel.AccountGroupPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.SelectedPagesOrBoards =
                PublisherCreateDestinationModel.AccountPagesBoardsPair.Count(x => x.Key == selectedSyncAccount.AccountId);

            currentAccountDetails.UpdatePagesOrBoardsText();

            currentAccountDetails.UpdateGroupText();

            currentAccountDetails.StatusSyncContent = ConstantVariable.FineStatusSync;
        }

        #endregion

        #region Add fresh accounts

        private bool AddFreshAccountCanExecute(object sender) => true;

        private void AddFreshAccountExecute(object sender)
        {
            try
            {
                var accounts = _accountsFileManager.GetAll();
                accounts.ForEach(x =>
                {
                    if (PublisherCreateDestinationModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                    {
                        var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel()
                        {
                            AccountId = x.AccountBaseModel.AccountId,
                            AccountName = x.AccountBaseModel.UserName,
                            SocialNetworks = x.AccountBaseModel.AccountNetwork,
                            IsOwnWallAvailable = x.AccountBaseModel.AccountNetwork != SocialNetworks.Pinterest,
                            IsGroupsAvailable =
                                GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsPagesOrBoardsAvailable =
                                BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            PublishonOwnWall = false,
                            SelectedGroups = 0,
                            TotalGroups = x.DisplayColumnValue2 ?? 0,
                            TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0,
                        };
                        publisherCreateDestinationSelectModel.GroupSelectorText =
                            GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalGroups
                                : "NA";

                        publisherCreateDestinationSelectModel.PagesOrBoardsSelectorText =
                            BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString())
                                ? "0" + "/" + publisherCreateDestinationSelectModel.TotalPagesOrBoards
                                : "NA";
                        if (SocinatorInitialize.IsNetworkAvailable(x.AccountBaseModel.AccountNetwork))
                        {
                            PublisherCreateDestinationModel.ListSelectDestination.Add(
                                publisherCreateDestinationSelectModel);
                        }
                    }
                });
                DestinationCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion


        #region Add Custom Destination Command

        private bool AddCustomDestinationCanExecute(object sender) => true;

        private void AddCustomDestinationExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = sender as PublisherCreateDestinationSelectModel;

            if (publisherCreateDestinationSelectModel == null)
                return;

            var valuePairs = PublisherCreateDestinationModel.CustomDestinations.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            var alreadySavedCustomDestination = new ObservableCollection<PublisherCustomDestinationModel>();

            valuePairs.ForEach(x =>
            {
                var publisherCustomDestinationModel = new PublisherCustomDestinationModel
                {
                    DestinationType = x.Value.DestinationType,
                    DestinationValue = x.Value.DestinationValue
                };
                alreadySavedCustomDestination.Add(publisherCustomDestinationModel);
            });

            var publisherAddCustomDestination = PublisherAddCustomDestination.GetPublisherAddCustomDestination(alreadySavedCustomDestination);
            var dialog = new Dialog();
            var window = dialog.GetMetroWindow(publisherAddCustomDestination, "Add Custom Destination");

            publisherAddCustomDestination.ButtonSave.Click += (senders, args) =>
            {
                var savedNewCustomDestination = publisherAddCustomDestination.GetSavedCustomDestination();
                var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                PublisherCreateDestinationModel.CustomDestinations.RemoveAll(x => x.Key == publisherCreateDestinationSelectModel.AccountId);

                PublisherCreateDestinationModel.DestinationDetailsModels.RemoveAll(x =>
                    x.AccountId == publisherCreateDestinationSelectModel.AccountId && x.IsCustomDestintions);

                savedNewCustomDestination.ForEach(x =>
                {
                    PublisherCreateDestinationModel.CustomDestinations.Add(new KeyValuePair<string, PublisherCustomDestinationModel>(publisherCreateDestinationSelectModel.AccountId, x));

                    PublisherCreateDestinationModel.DestinationDetailsModels.Add(new PublisherDestinationDetailsModel
                    {
                        AccountId = publisherCreateDestinationSelectModel.AccountId,
                        DestinationType = x.DestinationType,
                        DestinationUrl = x.DestinationValue,
                        SocialNetworks = publisherCreateDestinationSelectModel.SocialNetworks,
                        PublisherPostlistModel = new PublisherPostlistModel(),
                        IsCustomDestintions = true,
                        DestinationGuid = Utilities.GetGuid(),
                        AccountName = publisherCreateDestinationSelectModel.AccountName
                    });
                });

                publisherAddCustomDestination.ResetCurrectObject();

                if (createDestinationSelectModel != null)
                {
                    createDestinationSelectModel.CustomDestinationSelectorText = $"{ PublisherCreateDestinationModel.CustomDestinations.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList().Count}";
                }
                window.Close();
            };
            publisherAddCustomDestination.ButtonCancel.Click += (senders, args) =>
            {
                window.Close();
            };


            window.Show();
        }

        #endregion

        #region Edit Destination

        public void EditDestination()
        {
            Title = "Edit Destination";

            InitializeDestinationList();

            var saveDestination = PublisherCreateDestinationModel.GetDestination(EditDestinationId);

            var currentlyAvailableAccounts =
                PublisherCreateDestinationModel.ListSelectDestination.Select(x => x.AccountId).ToList();

            foreach (var savedDestination in saveDestination.ListSelectDestination)
            {
                if (!currentlyAvailableAccounts.Contains(savedDestination.AccountId))
                {
                    savedDestination.StatusSyncContent = ConstantVariable.NotAvailableAccountSync;
                    continue;
                }

                var currentAccountDetails = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == savedDestination.AccountId);

                if (currentAccountDetails == null)
                    return;

                if (savedDestination.TotalGroups != currentAccountDetails.TotalGroups ||
                    savedDestination.TotalPagesOrBoards != currentAccountDetails.TotalPagesOrBoards)
                    savedDestination.StatusSyncContent = ConstantVariable.NeedUpdateStatusSync;

            }

            PublisherCreateDestinationModel = saveDestination;

            PublisherCreateDestinationModel.UpdateDestination(saveDestination);

            DestinationCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);

        }

        #endregion   

    }
}
