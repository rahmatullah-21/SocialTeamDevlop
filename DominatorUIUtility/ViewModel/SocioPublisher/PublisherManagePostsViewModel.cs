using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
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
            SelectionChangedCommand = new BaseCommand<object>(SelectionChangedCanExecute, SelectionChangedExecute);
            InitializeTabs();
        }


        #region Properties

        public ICommand NavigationCommand { get; set; }

        public ICommand TabChangeCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public List<string> ManagePostTabItems { get; set; } = new List<string>();

        private string _selectedTabs = string.Empty;
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

        private UserControl _selectedTabsUserControls = new PublisherManagePostDrafts();

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

        public Queue<CancellationTokenSource> QueueCancellationTokenSources { get; set; } = new Queue<CancellationTokenSource>();

        #endregion

        private void InitializeTabs()
        {
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostDraft")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPending")?.ToString());
            ManagePostTabItems.Add(Application.Current.FindResource("DHlangManagePostPublished")?.ToString());

            var campaignDetails = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile());

            campaignDetails.ForEach(campaign =>
                CampaignList.Add(new IdNameBinderModel { Id = campaign.CampaignId, Name = campaign.CampaignName }));


            if (campaignDetails.Count > 0)
                SelectedCampaignDetails = CampaignList[0];

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

        public void TabChangeExecute(object sender)
        {
            var selectedButton = sender as string;

            if (selectedButton == ConstantVariable.DraftPostList)
            {
                try
                {
                    if (SelectedCampaignDetails.Id == null)
                        return;

                    SelectedTabs = ConstantVariable.DraftPostList;
                    var draftView = new PublisherManagePostDrafts();
                    SelectedTabsUserControls = draftView;
                    var cancellationToken = PostLoadingCancellation();
                    Task.Factory.StartNew(() => draftView.PublisherManagePostDraftsViewModel.ReadPostList(SelectedCampaignDetails.Id, cancellationToken), cancellationToken.Token);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else if (selectedButton == ConstantVariable.PendingPostList)
            {
                try
                {
                    if (SelectedCampaignDetails.Id == null)
                        return;

                    SelectedTabs = ConstantVariable.PendingPostList;
                    var pendingView = new PublisherManagePostPending();
                    SelectedTabsUserControls = pendingView;
                    var cancellationToken = PostLoadingCancellation();
                    Task.Factory.StartNew(() => pendingView.PublisherManagePostPendingViewModel.ReadPostList(SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Pending), cancellationToken.Token);

                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
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
                    if (SelectedCampaignDetails.Id == null)
                        return; 

                    SelectedTabs = ConstantVariable.PublishedPostList;
                    var publishedView = new PublisherManagePostPublished();
                    SelectedTabsUserControls = publishedView;
                    var cancellationToken = PostLoadingCancellation();
                    Task.Factory.StartNew(() => publishedView.PublisherManagePostPublishedViewModel.ReadPostList(SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Published), cancellationToken.Token);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Request Cancelled!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        private CancellationTokenSource PostLoadingCancellation()
        {
            CancelRunningTask();
            var cancellationToken = new CancellationTokenSource();
            QueueCancellationTokenSources.Enqueue(cancellationToken);
            return cancellationToken;
        }

        private bool SelectionChangedCanExecute(object sender) => true;

        private void SelectionChangedExecute(object sender)
        {
            var cancellationToken = PostLoadingCancellation();

            switch (SelectedTabs)
            {
                case "Draft":
                    try
                    {
                        var draftView = new PublisherManagePostDrafts();
                        Task.Factory.StartNew(
                            () => draftView.PublisherManagePostDraftsViewModel.ReadPostList(SelectedCampaignDetails.Id,
                                cancellationToken), cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                    break;
                case "Pending":
                    try
                    {
                        var pendingView = new PublisherManagePostPending();
                        Task.Factory.StartNew(() => pendingView.PublisherManagePostPendingViewModel.ReadPostList(SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Pending), cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                    break;
                case "Published":
                    try
                    {
                        var publishedView = new PublisherManagePostPublished();
                        Task.Factory.StartNew(() => publishedView.PublisherManagePostPublishedViewModel.ReadPostList(SelectedCampaignDetails.Id, cancellationToken, PostQueuedStatus.Published), cancellationToken.Token);
                    }
                    catch (OperationCanceledException ex)
                    {
                        ex.DebugLog("Request Cancelled!");
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.Message);
                    }
                    break;
            }

        }

        public void CancelRunningTask()
        {
            while (QueueCancellationTokenSources.Count > 0)
                QueueCancellationTokenSources.Dequeue().Cancel();
        }
    }


}