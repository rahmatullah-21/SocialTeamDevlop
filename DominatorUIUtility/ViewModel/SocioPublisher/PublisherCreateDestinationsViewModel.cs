using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
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

           var alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair
                .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

            var selected = accountsDetailsSelector.GetGroupsPair(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName);

            PublisherCreateDestinationModel.AccountGroupPair.AddRange(selected);

            alreadySelectedGroups = PublisherCreateDestinationModel.AccountGroupPair
                .Where(x => x.Key == publisherCreateDestinationSelectModel.AccountId).Select(x => x.Value).ToList();

            var createDestinationSelectModel = ListSelectDestination.FirstOrDefault(x => x.AccountId == publisherCreateDestinationSelectModel.AccountId);

            var account = AccountsFileManager.GetAccountById(publisherCreateDestinationSelectModel.AccountId);

            if (createDestinationSelectModel != null)
                createDestinationSelectModel.GroupSelectorText =
                    $"{alreadySelectedGroups.Count}/{account.DisplayColumnValue3}";
        }



        private bool GetAccountPagesOrBoardsCanExecute(object sender) => true;

        private void GetAccountPagesOrBoardsExecute(object sender)
        {
            var publisherCreateDestinationSelectModel = (PublisherCreateDestinationSelectModel)sender;

            var accountsDetailsSelector = SocinatorInitialize
                .GetSocialLibrary(publisherCreateDestinationSelectModel.SocialNetworks)
                .GetNetworkCoreFactory().AccountDetailsSelectors;

            var selectedPages = accountsDetailsSelector.GetPagesOrBoardsPair(publisherCreateDestinationSelectModel.AccountId, publisherCreateDestinationSelectModel.AccountName);
            PublisherCreateDestinationModel.AccountPagesBoardsPair.AddRange(selectedPages);
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
