using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DominatorHouseCore;
using DominatorHouseCore.Patterns;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{   
    public class PublisherDefaultViewModel : BindableBase
    {
        public PublisherDefaultViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SingleCloneCommand = new BaseCommand<object>(SingleCloneCanExecute, SingleCloneExecute);
            InitializeDefaultCampaignStatus();
        }

        public ICommand NavigationCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand SingleCloneCommand { get; set; }

        [field: NonSerialized]
        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels { get; set; } = new ObservableCollection<PublisherCampaignStatusModel>();

        private ICollectionView _publisherCampaignStatusModelView;

        public ICollectionView PublisherCampaignStatusModelView
        {
            get
            {
                return _publisherCampaignStatusModelView;
            }
            set
            {
                if (_publisherCampaignStatusModelView != null && _publisherCampaignStatusModelView == value)
                    return;
                SetProperty(ref _publisherCampaignStatusModelView, value);
            }
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

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var moduleName = sender.ToString();

            switch (moduleName)
            {
                case "ManageDestinations":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherManageDestinations.Instance;
                    break;
                case "ManagePosts":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = new PublisherManagePosts();
                    break;
                case "CreateCampaigns":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = new PublisherCreateCampaigns();
                    break;
            }
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
                case "SelectNone":
                    IsAllCampaignSelected = false;
                    break;

                case "SelectAll":
                case "MenuSelectAll":
                    IsAllCampaignSelected = true;
                    break;
            }
        }

        private bool SingleCloneCanExecute(object sender) => true;
     
        private void SingleCloneExecute(object sender)
        {
            try
            {
                var campaignStatus = (PublisherCampaignStatusModel)sender;

                if (campaignStatus == null) return;

                var clonedCampaignStatus = GetCampaginDeepClone(campaignStatus);

                clonedCampaignStatus.GenerateCloneCampaign(campaignStatus.CampaignName);

                AddCampaignDetails(clonedCampaignStatus);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void SelectAllCampaign()
        {
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = true; return x;
            }).ToList();
        }

        public void SelectNoneCampaign()
        {
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = false; return x;
            }).ToList();
        }

        public void InitializeDefaultCampaignStatus()
        {
            PublisherCampaignStatusModelView = CollectionViewSource.GetDefaultView(ListPublisherCampaignStatusModels);

            var publisherCampaignStatusModel = new PublisherCampaignStatusModel();
            publisherCampaignStatusModel.GenerateCampaign();
            for (int i = 0; i < 5; i++)
            {
                var campaignStatus = GetCampaginDeepClone(publisherCampaignStatusModel);
                campaignStatus.GenerateCampaign();
                campaignStatus.CampaignName = campaignStatus.CampaignName + RandomUtilties.GetRandomNumber(100);
                AddCampaignDetails(campaignStatus);
            }
        }

        public bool AddCampaignDetails(PublisherCampaignStatusModel publisherCampaignStatusModel)
        {
            if (ListPublisherCampaignStatusModels.Any(x => x.CampaignName == publisherCampaignStatusModel.CampaignName))
            {
                GlobusLogHelper.log.Info("Campaign name already present!");
                return false;
            }

            if (publisherCampaignStatusModel.ValidDateTime())
            {
                try
                {
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                        });
                    else
                        ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }
            return false;
        }

        public PublisherCampaignStatusModel GetCampaginDeepClone(PublisherCampaignStatusModel publisherCampaignStatusModel)
            => publisherCampaignStatusModel.DeepClone();
    }
}
