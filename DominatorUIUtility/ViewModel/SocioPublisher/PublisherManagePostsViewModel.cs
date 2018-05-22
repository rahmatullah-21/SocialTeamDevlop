using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherManagePostsViewModel : BindableBase
    {
        public PublisherManagePostsViewModel()
        {
            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            TabChangeCommand = new BaseCommand<object>(TabChangeCanExecute, TabChangeExecute);
            InitializeTabs();
        }

        #region Properties

        public ICommand NavigationCommand { get; set; }

        public ICommand TabChangeCommand { get; set; }

        public List<string> ManagePostTabItems { get; set; } = new List<string>();

        private string _selectedTabs = ConstantVariable.DraftPostList;
        public string SelectedTabs
        {
            get
            {
                return _selectedTabs;
            }
            set
            {
                if (_selectedTabs == value)
                    return;
                _selectedTabs = value;
                OnPropertyChanged(nameof(SelectedTabs));
            }
        }


        private IdNameBinderModel _selectedCampaignDetails;

        public IdNameBinderModel SelectedCampaignDetails
        {
            get
            {
                return _selectedCampaignDetails;
            }
            set
            {
                if (_selectedCampaignDetails == value)
                    return;
                _selectedCampaignDetails = value;
                OnPropertyChanged(nameof(SelectedCampaignDetails));
                PublisherManagePostDrafts.Instance.PublisherManagePostDraftsViewModel.CampaignId = SelectedCampaignDetails.Id;
                PublisherManagePostPending.Instance.PublisherManagePostPendingViewModel.CampaignId = SelectedCampaignDetails.Id;
                PublisherManagePostPublished.Instance.PublisherManagePostPublishedViewModel.CampaignId = SelectedCampaignDetails.Id;
            }
        }


        private List<IdNameBinderModel> _campaignList = new List<IdNameBinderModel>();
        public List<IdNameBinderModel> CampaignList
        {
            get
            {
                return _campaignList;
            }
            set
            {
                if (_campaignList == value)
                    return;
                _campaignList = value;
                OnPropertyChanged(nameof(CampaignList));
            }
        }


        private UserControl _selectedTabsUserControls = PublisherManagePostDrafts.Instance;



        public UserControl SelectedTabsUserControls
        {
            get
            {
                return _selectedTabsUserControls;
            }
            set
            {
                if (Equals(_selectedTabsUserControls, value))
                    return;
                _selectedTabsUserControls = value;
                OnPropertyChanged(nameof(SelectedTabsUserControls));
            }
        }

        #endregion

        private void InitializeTabs()
        {
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostDraft")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPending")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPublished")?.ToString());

            CampaignList.Add(new IdNameBinderModel { Id = "campaign1", Name = "Campaign1" });
            CampaignList.Add(new IdNameBinderModel { Id = "campaign2", Name = "Campaign2" });
            CampaignList.Add(new IdNameBinderModel { Id = "campaign3", Name = "Campaign3" });

            SelectedCampaignDetails = CampaignList[0];

            SelectedTabs = ConstantVariable.DraftPostList;
            var draftView = PublisherManagePostDrafts.Instance;
            Task.Factory.StartNew(async () => await draftView.PublisherManagePostDraftsViewModel.ReadPostDetails(SelectedCampaignDetails.Id));
     
        }

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance;
                    break;
            }
        }

        private bool TabChangeCanExecute(object sender) => true;

        private void TabChangeExecute(object sender)
        {
            var selectedButton = sender as string;

            if (selectedButton == ConstantVariable.DraftPostList)
            {
                SelectedTabs = ConstantVariable.DraftPostList;
                var draftView = PublisherManagePostDrafts.Instance;
                draftView.PublisherManagePostDraftsViewModel.CampaignId = SelectedCampaignDetails.Id;
                Task.Factory.StartNew(async () => await draftView.PublisherManagePostDraftsViewModel.ReadPostDetails(SelectedCampaignDetails.Id));
                SelectedTabsUserControls = draftView;
            }
            else if (selectedButton == ConstantVariable.PendingPostList)
            {
                SelectedTabs = ConstantVariable.PendingPostList;
                var pendingView = PublisherManagePostPending.Instance;
                pendingView.PublisherManagePostPendingViewModel.CampaignId = SelectedCampaignDetails.Id;
                Task.Factory.StartNew(async () => await pendingView.PublisherManagePostPendingViewModel.ReadPostDetails(SelectedCampaignDetails.Id));
                SelectedTabsUserControls = pendingView;
            }
            else
            {
                SelectedTabs = ConstantVariable.PublishedPostList;
                var publishedView = PublisherManagePostPublished.Instance;
                publishedView.PublisherManagePostPublishedViewModel.CampaignId = SelectedCampaignDetails.Id;
                Task.Factory.StartNew(async () => await publishedView.PublisherManagePostPublishedViewModel.ReadPostDetails(SelectedCampaignDetails.Id));
                SelectedTabsUserControls = publishedView;
            }
        }
    }


}