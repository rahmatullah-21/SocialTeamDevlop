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
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Process;
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
            ActiveSelectedCampaignCommand = new BaseCommand<object>(ActiveSelectedCampaignCanExecute, ActiveSelectedCampaignExecute);
            PauseSelectedCampaignCommand = new BaseCommand<object>(PauseSelectedCampaignCanExecute, PauseSelectedCampaignExecute);
            PublishNowSelectedCampaignCommand = new BaseCommand<object>(PublishNowSelectedCampaignCanExecute, PublishNowSelectedCampaignExecute);
            InitializeDefaultCampaignStatus();
        }

        #region Command

        public ICommand NavigationCommand { get; set; }

        public ICommand OpenContextMenuCommand { get; set; }

        public ICommand SelectionCommand { get; set; }

        public ICommand CampaignCloneCommand { get; set; }

        public ICommand DeleteCampaignCommand { get; set; }

        public ICommand ActiveSelectedCampaignCommand { get; set; }

        public ICommand PauseSelectedCampaignCommand { get; set; }

        public ICommand PublishNowSelectedCampaignCommand { get; set; }

        #endregion

        #region Properties

        public ObservableCollection<PublisherCampaignStatusModel> ListPublisherCampaignStatusModels
        {
            get
            {
                return _listPublisherCampaignStatusModels;
            }
            set
            {
                _listPublisherCampaignStatusModels = value;
                OnPropertyChanged(nameof(ListPublisherCampaignStatusModels));
            }
        }

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
        private ObservableCollection<PublisherCampaignStatusModel> _listPublisherCampaignStatusModels = new ObservableCollection<PublisherCampaignStatusModel>();

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

                SelectAll(_isAllCampaignSelected);
                _isUncheckedFromList = false;
            }
        }
        private bool _isUncheckedFromList { get; set; }
        public void SelectAll(bool isAllSelected)
        {
            if (_isUncheckedFromList)
                return;
            ListPublisherCampaignStatusModels.Select(x =>
            {
                x.IsSelected = isAllSelected; return x;
            }).ToList();
        }
        #endregion


        private bool PublishNowSelectedCampaignCanExecute(object sender) => true;

        private void PublishNowSelectedCampaignExecute(object sender)
        {
            var selectedCampaigns = GetSelectedCampaigns();
            selectedCampaigns.ForEach(x => PublishScheduler.SchedulePublishNowByCampaign(x.CampaignId));
        }

        private bool PauseSelectedCampaignCanExecute(object sender) => true;

        private void PauseSelectedCampaignExecute(object sender)
        {
            var selectedCampaigns = GetSelectedCampaigns();
            selectedCampaigns.ForEach(x =>
            {
                PublishScheduler.StopPublishingPosts(x.CampaignId);
                PublisherInitialize.GetInstance.UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Paused);
                InitializeDefaultCampaignStatus();
            });
        }


        private bool ActiveSelectedCampaignCanExecute(object sender) => true;

        private void ActiveSelectedCampaignExecute(object sender)
        {
            var selectedCampaigns = GetSelectedCampaigns();
            selectedCampaigns.ForEach(x =>
            {
                PublishScheduler.ScheduleTodaysPublisherByCampaign(x.CampaignId);
                PublisherInitialize.GetInstance.UpdateCampaignStatus(x.CampaignId, PublisherCampaignStatus.Active);
                InitializeDefaultCampaignStatus();
            });
        }



        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            if (sender is PublisherCampaignStatusModel)
            {
                try
                {
                    var createCampign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();

                    var currentCampaign = GenericFileManager
                        .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
                        .FirstOrDefault(campaign =>
                            campaign.CampaignId == (sender as PublisherCampaignStatusModel).CampaignId);

                    createCampign.PublisherCreateCampaignViewModel.SelectedItem = currentCampaign?.CampaignName;

                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl = createCampign;

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else
            {
                try
                {
                    var moduleName = sender.ToString();

                    switch (moduleName)
                    {
                        case "ManageDestinations":
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                PublisherManageDestinations.Instance();
                            break;
                        case "ManagePosts":
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                new PublisherManagePosts();
                            break;
                        case "CreateCampaigns":
                            PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl =
                                PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
                            break;
                    }
                }
                catch (Exception ex)
                {

                    ex.DebugLog();
                }
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
                case "SelectManually":

                    if (ListPublisherCampaignStatusModels.All(x => x.IsSelected))
                        IsAllCampaignSelected = true;
                    else
                    {
                        if (IsAllCampaignSelected)
                            _isUncheckedFromList = true;
                        IsAllCampaignSelected = false;
                    }
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
                    SaveClonedCampaign(clonedCampaignStatus, campaignStatus.CampaignId);
                    GlobusLogHelper.log.Info(campaignStatus.CampaignName + "Successfully duplicated.");
                    PublisherInitialize.GetInstance.AddCampaignDetails(clonedCampaignStatus);

                    var allSavedPosts = PostlistFileManager.GetAll(campaignStatus.CampaignId);

                    var clonedPostlist = new List<PublisherPostlistModel>();
                    allSavedPosts.ForEach(x =>
                    {
                        x.GenerateClonePostId();
                        clonedPostlist.Add(x);
                    });

                    PostlistFileManager.UpdatePostlists(clonedCampaignStatus.CampaignId, clonedPostlist);

                    PublisherInitialize.GetInstance.UpdatePostStatus(clonedCampaignStatus.CampaignId);

                    var publisherPostFetchModel =
                        GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                            .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatus.CampaignId);

                    PublisherManageDestinationModel.AddCampaignToDestinationList(publisherPostFetchModel?.SelectedDestinations, clonedCampaignStatus.CampaignId);

                }
                else
                {
                    GetSelectedCampaigns().ForEach(campaign =>
                    {
                        var clonedCampaignStatus = GetCampaginDeepClone(campaign);
                        clonedCampaignStatus.GenerateCloneCampaign(campaign.CampaignName);
                        SaveClonedCampaign(clonedCampaignStatus, campaign.CampaignId);
                        PublisherInitialize.GetInstance.AddCampaignDetails(clonedCampaignStatus);

                        var allSavedPosts = PostlistFileManager.GetAll(campaign.CampaignId);

                        var clonedPostlist = new List<PublisherPostlistModel>();
                        allSavedPosts.ForEach(x =>
                        {
                            x.GenerateClonePostId();
                            clonedPostlist.Add(x);
                        });

                        PostlistFileManager.UpdatePostlists(clonedCampaignStatus.CampaignId, clonedPostlist);
                        PublisherInitialize.GetInstance.UpdatePostStatus(clonedCampaignStatus.CampaignId);

                        var publisherPostFetchModel =
                            GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

                        PublisherManageDestinationModel.AddCampaignToDestinationList(
                            publisherPostFetchModel?.SelectedDestinations, clonedCampaignStatus.CampaignId);

                    });
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void SaveClonedCampaign(PublisherCampaignStatusModel clonedCampaignStatus, string campaignId)
        {
            try
            {
                var duplicatedCampaign = GenericFileManager
                       .GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
                       .FirstOrDefault(campaign => campaign.CampaignId == campaignId);
                duplicatedCampaign.CampaignName = clonedCampaignStatus.CampaignName;
                duplicatedCampaign.CampaignId = clonedCampaignStatus.CampaignId;

                GenericFileManager.AddModule<PublisherCreateCampaignModel>(duplicatedCampaign,
                    ConstantVariable.GetPublisherCampaignFile());

                PublisherCreateCampaigns.Instance.PublisherCreateCampaignViewModel.CampaignList.Add(duplicatedCampaign.CampaignName);
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

                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "If you delete it, cant recover back \nAre you sure ?", "Delete Anyways", "Don't delete");

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                GenericFileManager.Delete<PublisherCreateCampaignModel>(y => campaign.CampaignId == y.CampaignId,
                    ConstantVariable.GetPublisherCampaignFile());

                PublisherCreateCampaigns.Instance.PublisherCreateCampaignViewModel.CampaignList.Remove(campaign.CampaignName);
                ListPublisherCampaignStatusModels.Remove(campaign);


                GenericFileManager.Delete<PublisherPostFetchModel>(y => campaign.CampaignId == y.CampaignId, ConstantVariable.GetPublisherPostFetchFile);

                PublishScheduler.StopPublishingPosts(campaign.CampaignId);

                PublisherManageDestinationModel.RemoveDestinationFromCampaign(campaign.CampaignId);
            }
            else
            {
                var publisherCampaignStatusModels = GetSelectedCampaigns();

                if (publisherCampaignStatusModels.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "Please select atleast one campaign !!");
                    return;
                }

                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "If you delete it will delete all selected campaign permanently \nAre you sure ?",
                    "Delete Anyways", "Don't delete");

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                GenericFileManager.Delete<PublisherCreateCampaignModel>(x => publisherCampaignStatusModels.FirstOrDefault(a => a.CampaignId == x.CampaignId) != null,
                    ConstantVariable.GetPublisherCampaignFile());

                publisherCampaignStatusModels.ForEach(x =>
                {
                    ListPublisherCampaignStatusModels.Remove(x);
                    PublisherCreateCampaigns.Instance.PublisherCreateCampaignViewModel.CampaignList.Remove(x.CampaignName);
                    PublishScheduler.StopPublishingPosts(x.CampaignId);
                    PublisherManageDestinationModel.RemoveDestinationFromCampaign(x.CampaignId);
                });


                GenericFileManager.Delete<PublisherPostFetchModel>(x => publisherCampaignStatusModels.FirstOrDefault(a => a.CampaignId == x.CampaignId) != null,
                    ConstantVariable.GetPublisherPostFetchFile);

            }
        }

        private List<PublisherCampaignStatusModel> GetSelectedCampaigns()
            => ListPublisherCampaignStatusModels.Where(x => x.IsSelected).ToList();

        public void InitializeDefaultCampaignStatus()
        {
            ListPublisherCampaignStatusModels = PublisherInitialize.GetInstance.GetSavedCampaigns();
            PublisherCampaignStatusModelView = null;
            PublisherCampaignStatusModelView = CollectionViewSource.GetDefaultView(ListPublisherCampaignStatusModels);
        }

        public PublisherCampaignStatusModel GetCampaginDeepClone(PublisherCampaignStatusModel publisherCampaignStatusModel)
            => publisherCampaignStatusModel.DeepClone();
    }
}
