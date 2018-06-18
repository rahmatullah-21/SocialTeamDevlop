using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorUIUtility.Behaviours;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDefaultViewModel : BindableBase
    {
        public PublisherDefaultViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SelectionCommand = new BaseCommand<object>(SelectionCanExecute, SelectionExecute);
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            CampaignCloneCommand = new BaseCommand<object>(CampaignCloneCanExecute, CampaignCloneExecute);
            DeleteCampaignCommand = new BaseCommand<object>(DeleteCampaignCanExecute, DeleteCampaignExecute);
            InitializeDefaultCampaignStatus();
        }

        public ICommand NavigationCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand CampaignCloneCommand { get; set; }

        public ICommand DeleteCampaignCommand { get; set; }


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
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
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

        private bool CampaignCloneCanExecute(object sender) => true;

        private void CampaignCloneExecute(object sender)
        {
            try
            {
                var isSingleDuplicate = sender is PublisherCampaignStatusModel;

                if (isSingleDuplicate)
                {
                    var campaignStatus = (PublisherCampaignStatusModel)sender;

                    var clonedCampaignStatus = GetCampaginDeepClone(campaignStatus);

                    clonedCampaignStatus.GenerateCloneCampaign(campaignStatus.CampaignName);

                    AddCampaignDetails(clonedCampaignStatus);
                }
                else
                {
                    GetSelectedCampaigns().ForEach(campaign =>
                    {
                        var clonedCampaignStatus = GetCampaginDeepClone(campaign);

                        clonedCampaignStatus.GenerateCloneCampaign(campaign.CampaignName);

                        AddCampaignDetails(clonedCampaignStatus);
                    });
                }
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

        public bool DeleteCampaignCanExecute(object sender) => true;

        public void DeleteCampaignExecute(object sender)
        {

            var isIndividualDelete = sender is PublisherCampaignStatusModel;

            if (isIndividualDelete)
            {
                var campaign = (PublisherCampaignStatusModel)sender;

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Confirmation", "If you delete it, cant recover back \nAre you sure ?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;


                ListPublisherCampaignStatusModels.Remove(campaign);
            }
            else
            {
                var publisherCampaignStatusModels = GetSelectedCampaigns();

                if (publisherCampaignStatusModels.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                        "Please select atleast one campaign !!");
                    return;
                }

                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Confirmation", "If you delete it will delete all selected campaign permanently \nAre you sure ?",
                    MessageDialogStyle.AffirmativeAndNegative,
                    Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                publisherCampaignStatusModels.ForEach(x => ListPublisherCampaignStatusModels.Remove(x));
            }
        }

        private List<PublisherCampaignStatusModel> GetSelectedCampaigns()
            => ListPublisherCampaignStatusModels.Where(x => x.IsSelected).ToList();

        public void InitializeDefaultCampaignStatus()
        {
            PublisherCampaignStatusModelView = CollectionViewSource.GetDefaultView(ListPublisherCampaignStatusModels);

            var allCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            allCampaign.ForEach(camp =>
            {
                var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                {
                    CampaignName = camp.CampaignName,
                    CampaignId = camp.CampaignId,
                    StartDate = camp.JobConfigurations.CampaignStartDate,
                    EndDate = camp.JobConfigurations.CampaignEndDate,
                    CreatedDate = camp.CreatedDate,
                    Status = camp.CampaignStatus,
                    DestinationCount = camp.LstDestinationId.Count,
                    IsRotateDayChecked =  camp.JobConfigurations.IsRotateDayChecked,
                    TimeRange = camp.JobConfigurations.TimeRange,
                    SpecificRunningTime = camp.JobConfigurations.LstTimer.Select(x=> x.MidTime).ToList(),
                    ScheduledWeekday = camp.JobConfigurations.Weekday
                };
                AddCampaignDetails(publisherCampaignStatusModel);
            });           
        }

        public void SchedulePublisherCampaigns()
        {
            ListPublisherCampaignStatusModels.ForEach(campaigns =>
            {
                if (campaigns.IsRotateDayChecked)
                {
                    var random = new Random();


                }
                else
                {
                    
                }
            });
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
