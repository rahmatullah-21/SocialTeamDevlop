using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateDestinationsViewModel : BindableBase
    {

        public PublisherCreateDestinationsViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            GetAccountGroupsCommand = new BaseCommand<object>(GetAccountGroupsCanExecute, GetAccountGroupsExecute);
            GetAccountPagesOrBoardsCommand = new BaseCommand<object>(GetAccountPagesOrBoardsCanExecute, GetAccountPagesOrBoardsExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectAccountDetailsCommand = new BaseCommand<object>(SelectAccountDetailsCanExecute, SelectAccountDetailsExecute);
            InitializeDestinationList();
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

        private ObservableCollection<PublisherCreateDestinationSelectModel> _listSelectDestination = new ObservableCollection<PublisherCreateDestinationSelectModel>();

        public ObservableCollection<PublisherCreateDestinationSelectModel> ListSelectDestination
        {
            get
            {
                return _listSelectDestination;
            }
            set
            {
                if (_listSelectDestination == value)
                    return;
                _listSelectDestination = value;
                OnPropertyChanged(nameof(ListSelectDestination));
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

        public ICommand NavigationCommand { get; set; }

        public ICommand GetAccountGroupsCommand { get; set; }

        public ICommand SelectAccountDetailsCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand GetAccountPagesOrBoardsCommand { get; set; }

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

        private bool GetAccountGroupsCanExecute(object sender) => true;

        private void GetAccountGroupsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            accountsDetailsSelector.GetGroupsPair(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, ListSelectDestination, PublisherCreateDestinationModel);

        }

        private bool GetAccountPagesOrBoardsCanExecute(object sender) => true;

        private void GetAccountPagesOrBoardsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            accountsDetailsSelector.GetPagesOrBoardsPair(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName, ListSelectDestination, PublisherCreateDestinationModel);
        }




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

                if (_isAllDestinationSelected)
                    SelectAllDestination();
                else
                    SelectNoneDestination();
            }
        }


        public void SelectAllDestination()
        {
            ListSelectDestination.Select(x =>
            {
                x.IsAccountSelected = true; return x;
            }).ToList();
        }

        public void SelectNoneDestination()
        {
            ListSelectDestination.Select(x =>
            {
                x.IsAccountSelected = false; return x;
            }).ToList();
        }


        private bool SelectAccountDetailsCanExecute(object sender) => true;

        private void SelectAccountDetailsExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "OwnProfile":
                    ListSelectDestination.Select(x =>
                    {
                        x.PublishonOwnWall = true; return x;
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


        public void LoadAllAccountsGroup()
        {

            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var accountDetailsSelector = new AccountDetailsSelector(UpdateGroupsDetails)
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

                window.Close();
            };

            accountDetailsSelector.btnCancel.Click += (sender, events) => { window.Close(); };

            window.Show();

            accountDetailsSelector.UpdateUiAllData();

        }

        private void  UpdateGroupsDetails(AccountDetailsSelector accountDetailsSelector)
        {
            var valuePairs = PublisherCreateDestinationModel.AccountGroupPair.ToList();

            var alreadySelectedGroups = valuePairs.Select(x => x.Value).ToList();

            var count = ListSelectDestination.Count;


            ListSelectDestination.ForEach(async x =>
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

            var count = ListSelectDestination.Count;


            ListSelectDestination.ForEach(async x =>
            {
                if (PagesAvailableInNetworks.Contains(x.SocialNetworks.ToString()))
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

                count--;

                if (count <= 0)
                    UpdateStatus(accountDetailsSelector);
            });


        }




        public List<string> GroupsAvailableInNetworks { get; set; } = new List<string> { "Facebook", "LinkedIn" };

        public List<string> PagesAvailableInNetworks { get; set; } = new List<string> { "Facebook", "Youtube", "Pinterest", "LinkedIn", "Gplus" };

        public void InitializeDestinationList()
        {
            var accounts = AccountsFileManager.GetAll();

            accounts.ForEach(x =>
            {
                var publisherCreateDestinationSelectModel = new PublisherCreateDestinationSelectModel()
                {
                    AccountId = x.AccountBaseModel.AccountId,
                    AccountName = x.AccountBaseModel.UserName,
                    SocialNetworks = x.AccountBaseModel.AccountNetwork,
                    IsGroupsAvailable = GroupsAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    IsPagesOrBoardsAvailable = PagesAvailableInNetworks.Contains(x.AccountBaseModel.AccountNetwork.ToString()),
                    PublishonOwnWall = false,
                    SelectedGroups = 0,
                    TotalGroups = x.DisplayColumnValue2 ?? 0,
                    TotalPagesOrBoards = x.DisplayColumnValue3 ?? 0,
                };
                ListSelectDestination.Add(publisherCreateDestinationSelectModel);
            });

            DestinationCollectionView = CollectionViewSource.GetDefaultView(ListSelectDestination);
        }


    }
}
