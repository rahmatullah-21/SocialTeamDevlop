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

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateDestinationsViewModel : BindableBase
    {
        //ConstructorS
        public PublisherCreateDestinationsViewModel()
        {
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
            InitializeProperties();
            InitializeDestinationList();
            IsSavedDestination = false;
        }

        public void InitializeProperties()
        {
            Title = "Create Destination";
            IsAllDestinationSelected = false;
            EditDestinationId = string.Empty;
            IsSavedDestination = false;
            PublisherCreateDestinationModel = PublisherCreateDestinationModel.DestinationDefaultBuilder();
        }

        #region Properties

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


        private List<string> _needToUpdateAccounts = new List<string>();


        public List<string> GroupsAvailableInNetworks { get; set; } = new List<string> { "Facebook", "LinkedIn" };

        public List<string> BoardsOrPagesAvailableInNetworks { get; set; } = new List<string> { "Facebook", "Youtube", "Pinterest", "LinkedIn", "Gplus", "Tumblr" };


        #endregion

        #region Navigation

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherManageDestinations.Instance;
                    break;
            }
        }

        #endregion

        #region Get Single Account Groups Details

        private bool GetSingleAccountGroupsCanExecute(object sender) => true;

        private void GetSingleAccountGroupsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

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

            var window = dialog.GetMetroWindow(accountDetailsSelector, "Select Groups");

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountGroupPair.Remove(x);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

                alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

                var createDestinationSelectModel = PublisherCreateDestinationModel.ListSelectDestination.FirstOrDefault(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId);

                if (createDestinationSelectModel != null)
                    createDestinationSelectModel.GroupSelectorText = $"{alreadySelectedGroups.Count}/{createDestinationSelectModel.TotalGroups}";

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (senderDetails, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiSingleData();

        }

        private async Task UpdateSingleAccountGroupsDetails(AccountDetailsSelector accountDetailsSelector, PublisherCreateDestinationSelectModel publisherCreateDestinationSelectModel)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            if (GroupsAvailableInNetworks.Contains(publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var groups = await accountsDetailsSelector.GetGroupsDetails(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, alreadySelectedGroups);

                groups.ForEach(group =>
                {
                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                            accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group));
                    }
                    else
                        accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Add(group);
                });
            }

            UpdateStatus(accountDetailsSelector);
        }

        #endregion

        #region Get Single Account Page Details

        private bool GetSingleAccountPagesOrBoardsCanExecute(object sender) => true;

        private void GetSingleAccountPagesOrBoardsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            var valuePairs = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList();

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateSingleAccountPagesDetails, publisherCreateDestinationSelectModel)
            {
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

            accountDetailsSelector.btnSave.Click += (senderDetails, events) =>
            {
                valuePairs.ForEach(x =>
                {
                    PublisherCreateDestinationModel.AccountPagesBoardsPair.Remove(x);
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

                alreadySelectedPages = PublisherCreateDestinationModel.AccountPagesBoardsPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

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
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).ToList(); ;

            var alreadySelectedPages = valuePairs.Select(x => x.Value).ToList();

            if (BoardsOrPagesAvailableInNetworks.Contains(publisherCreateDestinationSelectModel.SocialNetworks.ToString()))
            {
                var accountsDetailsSelector = SocinatorInitialize
                    .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                    .GetNetworkCoreFactory().AccountDetailsSelectors;

                var pagesOrBoards = await accountsDetailsSelector.GetPagesDetails(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, alreadySelectedPages);

                pagesOrBoards.ForEach(page =>
                {
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
                GlobusLogHelper.log.Error(ex.Message);
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
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountGroupPair.AddRange(keyValuePairs);

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
                });

                var keyValuePairs = accountDetailsSelector.AccountDetailsSelectorViewModel.GetSelectedItems().ToList();

                PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(keyValuePairs);

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
                    accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText = accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0 ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !" : $"No row(s) found !";
                });
            }
            else
            {
                accountDetailsSelector.AccountDetailsSelectorViewModel.IsProgressRingActive = false;
                accountDetailsSelector.AccountDetailsSelectorViewModel.StatusText = accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count > 0 ? $"{accountDetailsSelector.AccountDetailsSelectorViewModel.ListAccountDetailsSelectorModels.Count} row(s) found !" : $"No row(s) found !";
            }
        }

        #endregion

        #region Initialize Updates

        public void InitializeDestinationList()
        {
            var accounts = AccountsFileManager.GetAll();

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

                PublisherCreateDestinationModel.ListSelectDestination.Add(publisherCreateDestinationSelectModel);
            });

            DestinationCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);

        }

        #endregion

        #region Validate Destinations

        public bool IsDuplicate()
        {

            if (!string.IsNullOrEmpty(EditDestinationId))
                return false;

            var availableCount = PublisherManageDestinations.Instance.PublisherManageDestinationViewModel
                     .ListPublisherManageDestinationModels.Count;

            if (availableCount == 0)
                return false;

            // check destination name is already present or not 
            var isPresent = false;

            foreach (var x in PublisherManageDestinations.Instance.PublisherManageDestinationViewModel
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
                PublisherCreateDestinationModel. AccountsWithNetwork.Clear();
                PublisherCreateDestinationModel.ListSelectDestination.ForEach(x =>
                {
                    if (x.IsAccountSelected)
                    {
                        PublisherCreateDestinationModel.SelectedAccountIds.Add(x.AccountId);
                        PublisherCreateDestinationModel.AccountsWithNetwork.Add(new KeyValuePair<SocialNetworks, string>(x.SocialNetworks, x.AccountId));

                        if (x.PublishonOwnWall)
                            PublisherCreateDestinationModel.PublishOwnWallAccount.Add(x.AccountId);
                    }
                    else
                    {
                        var unwantedGroups = PublisherCreateDestinationModel.AccountGroupPair.Where(y => y.Key == x.AccountId).Select(y => y.Key);
                        PublisherCreateDestinationModel.AccountGroupPair.RemoveAll(z => unwantedGroups.Contains(z.Key));
                        PublisherCreateDestinationModel.AccountPagesBoardsPair.RemoveAll(z => unwantedGroups.Contains(z.Key));
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
                    PublisherCreateDestinationModel.PublishOwnWallAccount.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                        "Warning", "Please select destination!");
                    return;
                }

                if (PublisherCreateDestinationModel.SelectedAccountIds.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                        "Warning", "Please select accounts, You have selected only destinations !");
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
                        WallsOrProfilesCount = PublisherCreateDestinationModel.PublishOwnWallAccount.Count
                    };

                    PublisherManageDestinations.Instance.PublisherManageDestinationViewModel.AddDestinations(
                        publisherManageDestinationModel, true);
                }
                // Edit Destination
                else
                {
                    PublisherCreateDestinationModel.UpdateDestination(PublisherCreateDestinationModel);

                    var publisherManageDestinationModel = PublisherManageDestinations.Instance.PublisherManageDestinationViewModel.GetManageDestination(EditDestinationId);

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

                    // To call a method to update the manage destination user interface
                    PublisherManageDestinations.Instance.PublisherManageDestinationViewModel.UpdateDestinations(
                        publisherManageDestinationModel);
                }

                InitializeProperties();

                IsSavedDestination = true;

                PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                    = PublisherManageDestinations.Instance;
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
            try
            {
                InitializeProperties();
                InitializeDestinationList();
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
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
                GlobusLogHelper.log.Error(ex.Message);
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

            var currentPages = await accountsDetailsSelector.GetGroupsUrls(selectedSyncAccount.AccountId, selectedSyncAccount.AccountName);

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
                var accounts = AccountsFileManager.GetAll();
                accounts.ForEach(x =>
                {
                    if (PublisherCreateDestinationModel.ListSelectDestination.All(y => y.AccountId != x.AccountId))
                    {
                        var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel()
                        {
                            AccountId = x.AccountBaseModel.AccountId,
                            AccountName = x.AccountBaseModel.UserName,
                            SocialNetworks = x.AccountBaseModel.AccountNetwork,
                            IsGroupsAvailable = GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                            IsPagesOrBoardsAvailable = BoardsOrPagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
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
                        PublisherCreateDestinationModel.ListSelectDestination.Add(publisherCreateDestinationSelectModel);
                    }
                });
                DestinationCollectionView = CollectionViewSource.GetDefaultView(PublisherCreateDestinationModel.ListSelectDestination);
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
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
