using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Process;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateCampaignViewModel : INotifyPropertyChanged
    {
        private TabItemsControl tabItemsControl { get; set; } = new TabItemsControl();

        public PublisherCreateCampaignViewModel()
        {
            #region Command initilization

            NavigationCommand = new BaseCommand<object>(NavigationCanExecute, NavigationExecute);
            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            SelectDestinationCommand = new BaseCommand<object>(SelectDestinationCanExecute, SelectDestinationExecute);
            CampaignChangedCommand = new BaseCommand<object>(CampaignChangedCanExecute, CampaignChangedExecute);

            #endregion

            PostTabItems = InitializeTabs();
            BindTabItemsControlProperties();
            CampaignList = new ObservableCollection<string>(
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile()).Select(x => x.CampaignName));

            PublisherCreateCampaignModel.JobConfigurations.Weekday.Clear();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                PublisherCreateCampaignModel.JobConfigurations.Weekday.Add(new ContentSelectGroup
                {
                    Content = day.ToString(),
                    IsContentSelected = true
                });
            }
        }


        #region Properties

        private PublisherCreateCampaignModel _publisherCreateCampaignModel = new PublisherCreateCampaignModel();

        public PublisherCreateCampaignModel PublisherCreateCampaignModel
        {
            get
            {
                return _publisherCreateCampaignModel;
            }
            set
            {
                if (_publisherCreateCampaignModel == value)
                    return;
                _publisherCreateCampaignModel = value;
                OnPropertyChanged(nameof(PublisherCreateCampaignModel));
            }
        }
        private ObservableCollection<string> _campaignList = new ObservableCollection<string>();
        // To hold all available the campaign name
        //[ProtoMember(4)]
        public ObservableCollection<string> CampaignList
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
        private string _selectedItem;

        public string SelectedItem
        {
            get
            {
                return _selectedItem;
            }
            set
            {
                if (_selectedItem == value)
                    return;
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
            }
        }
        #region Command
        public ICommand NavigationCommand { get; set; }
        public ICommand SaveCommand { get; set; }
        public ICommand SelectDestinationCommand { get; set; }
        public ICommand CampaignChangedCommand { get; set; }
        #endregion

        public List<TabItemTemplates> PostTabItems { get; set; }

        #endregion

        private List<TabItemTemplates> InitializeTabs()
        {
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title= Application.Current.FindResource("DHlangCreatePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=> new PublisherDirectPosts(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title=Application.Current.FindResource("DHlangScrapePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>PublisherScrapePost.GetPublisherScrapePost(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangSharePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=>PublisherSharePost.GetPublisherSharePost(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangRssFeed")?.ToString(),
                    Content = new Lazy<UserControl>(()=> PublisherRssFeed.GetPublisherRssFeed(tabItemsControl))
                },
                new TabItemTemplates
                {
                    Title = Application.Current.FindResource("DHlangMonitorFolder")?.ToString(),
                    Content = new Lazy<UserControl>(()=>PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl))
                }
            };
            return tabItems;
        }

        #region Command Methods

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance();
                    break;
            }
        }

        private bool SaveCanExecute(object sender) => true;
        private void SaveExecute(object sender)
        {
            if (_publisherCreateCampaignModel.LstDestinationId.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please select atleast one Destination.");
                return;
            }

            try
            {
                #region Saving Campign to PublisherCampaign.bin file

                var lstCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(
                    ConstantVariable.GetPublisherCampaignFile());

                if (!string.IsNullOrEmpty(SelectedItem) || lstCampaign.Any(x => x.CampaignId == PublisherCreateCampaignModel.CampaignId))
                {
                    var campaignIndex = lstCampaign.IndexOf(lstCampaign.FirstOrDefault(x => x.CampaignName == SelectedItem));
                    lstCampaign[campaignIndex] = PublisherCreateCampaignModel;

                    if (GenericFileManager.UpdateModuleDetails<PublisherCreateCampaignModel>(lstCampaign,
                        ConstantVariable.GetPublisherCampaignFile()))
                        Dialog.ShowDialog("Success", "Campaign successfully updated.");
                }
                else
                {
                    if (GenericFileManager.AddModule<PublisherCreateCampaignModel>(PublisherCreateCampaignModel, ConstantVariable.GetPublisherCampaignFile()))
                        Dialog.ShowDialog("Success", "Campaign successfully saved.");
                    CampaignList.Add(PublisherCreateCampaignModel.CampaignName);
                }



                #endregion

                #region Saving post

                PostlistFileManager.SaveAll(PublisherCreateCampaignModel.CampaignId, new List<PublisherPostlistModel>());

                var publisherPostlistModel = new PublisherPostlistModel
                {
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    CreatedTime = DateTime.Now,
                    PostSource = PostSource.NormalPost,
                    PostCategory = PublisherCreateCampaignModel.PostDetailsModel.IsFdSellPost ? PostCategory.SellPost : PostCategory.OrdinaryPost,
                    PostQueuedStatus = PostQueuedStatus.Pending,
                    PostRunningStatus = PostRunningStatus.Active,
                    ExpiredTime = DateTime.Today.AddDays(10),
                    FdPostSettings = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.FdPostSettings,
                    GdPostSettings = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.GdPostSettings,
                    TdPostSettings = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.TdPostSettings,
                    LdPostSettings = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.LdPostSettings,
                    TumberPostSettings = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.TumberPostSettings,
                    RedditPostSetting = PublisherCreateCampaignModel.PostDetailsModel.PublisherPostSettings.RedditPostSetting,
                };

                if (PublisherCreateCampaignModel.PostDetailsModel.IsSinglePost)
                {
                    publisherPostlistModel.PostId = Utilities.GetGuid();
                    publisherPostlistModel.PostDescription = PublisherCreateCampaignModel.PostDetailsModel.PostDescription;
                    publisherPostlistModel.MediaList = PublisherCreateCampaignModel.PostDetailsModel.MediaViewer.MediaList;
                    publisherPostlistModel.PublisherInstagramTitle =
                        PublisherCreateCampaignModel.PostDetailsModel.PublisherInstagramTitle;
                    publisherPostlistModel.PdSourceUrl = PublisherCreateCampaignModel.PostDetailsModel.PdSourceUrl;
                    publisherPostlistModel.FdSellLocation =
                        PublisherCreateCampaignModel.PostDetailsModel.FdSellLocation;
                    publisherPostlistModel.FdSellPrice = PublisherCreateCampaignModel.PostDetailsModel.FdSellPrice;
                    publisherPostlistModel.FdSellProductTitle =
                        PublisherCreateCampaignModel.PostDetailsModel.FdSellProductTitle;
                    PostlistFileManager.Add(PublisherCreateCampaignModel.CampaignId, publisherPostlistModel);

                }
                else if (PublisherCreateCampaignModel.PostDetailsModel.IsMultiPost)
                {
                    PublisherCreateCampaignModel.LstPostDetailsModels.ForEach(post =>
                    {
                        publisherPostlistModel.PostId = Utilities.GetGuid();
                        publisherPostlistModel.PostDescription = post.PostDescription;
                        publisherPostlistModel.MediaList = post.MediaViewer.MediaList;
                        publisherPostlistModel.PublisherInstagramTitle = post.PublisherInstagramTitle;
                        publisherPostlistModel.PdSourceUrl = post.PdSourceUrl;
                        publisherPostlistModel.FdSellLocation = post.FdSellLocation;
                        publisherPostlistModel.FdSellPrice = post.FdSellPrice;
                        publisherPostlistModel.FdSellProductTitle = post.FdSellProductTitle;
                        PostlistFileManager.Add(PublisherCreateCampaignModel.CampaignId, publisherPostlistModel);
                    });
                }
                else
                {
                    var images = PublisherCreateCampaignModel.PostDetailsModel.MediaList;
                    images.ForEach(image =>
                    {
                        publisherPostlistModel.MediaList = new ObservableCollection<string> { image };
                        publisherPostlistModel.PostDescription = new Uri(image).Segments.Last();
                        publisherPostlistModel.PostId = Utilities.GetGuid();
                        PostlistFileManager.Add(PublisherCreateCampaignModel.CampaignId, publisherPostlistModel);
                    });
                }
                #endregion

                #region Fetch Post Details

                GenericFileManager.Delete<PublisherPostFetchModel>(y => PublisherCreateCampaignModel.CampaignId == y.CampaignId, ConstantVariable.GetPublisherPostFetchFile);

                var currentCampaignsFetchDetails = new List<PublisherPostFetchModel>();

                var generalSettingsModel = General.GetSingeltonGeneralObject().GeneralViewModel.GeneralModel;

                #region DirectPostPosts

                var directPostModel = new PublisherPostFetchModel
                {
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    CampaignName = PublisherCreateCampaignModel.CampaignName,
                    ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                    PostSource = PostSource.NormalPost,
                    SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                };

                currentCampaignsFetchDetails.Add(directPostModel);

                #endregion

                #region MonitorFolder

                if (PublisherCreateCampaignModel.LstFolderPath.Count > 0)
                {
                    var monitorFolderFetchModel = new PublisherPostFetchModel
                    {
                        CampaignId = PublisherCreateCampaignModel.CampaignId,
                        CampaignName = PublisherCreateCampaignModel.CampaignName,
                        ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                        PostSource = PostSource.MonitorFolderPost,
                        DelayForNext = generalSettingsModel.CheckMonitorFoldersminutes,
                        PostDetailsWithFilters =
                            JsonConvert.SerializeObject(PublisherCreateCampaignModel.LstFolderPath),
                        MaximumPostLimitToStore = generalSettingsModel.MaxPostCountToStore,
                        SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                    };

                    currentCampaignsFetchDetails.Add(monitorFolderFetchModel);
                }

                #endregion

                #region RssFeed

                if (PublisherCreateCampaignModel.LstFeedUrl.Count > 0)
                {
                    var rssFetchModel = new PublisherPostFetchModel
                    {
                        CampaignId = PublisherCreateCampaignModel.CampaignId,
                        CampaignName = PublisherCreateCampaignModel.CampaignName,
                        ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                        PostSource = PostSource.RssFeedPost,
                        PostDetailsWithFilters = JsonConvert.SerializeObject(PublisherCreateCampaignModel.LstFeedUrl),
                        DelayForNext = generalSettingsModel.CheckRssFeedsminutes,
                        MaximumPostLimitToStore = generalSettingsModel.MaxPostCountToStore,
                        SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                    };

                    currentCampaignsFetchDetails.Add(rssFetchModel);
                }

                #endregion

                #region ScrapePost

                if (PublisherCreateCampaignModel.ScrapePostModel.IsScrapeFacebookPost ||
                    PublisherCreateCampaignModel.ScrapePostModel.IsScrapePinterestPost ||
                    PublisherCreateCampaignModel.ScrapePostModel.IsScrapeTwitterPost)
                {
                    var scrapeFetchModel = new PublisherPostFetchModel
                    {
                        CampaignId = PublisherCreateCampaignModel.CampaignId,
                        CampaignName = PublisherCreateCampaignModel.CampaignName,
                        ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                        PostSource = PostSource.ScrapedPost,
                        PostDetailsWithFilters = JsonConvert.SerializeObject(PublisherCreateCampaignModel.ScrapePostModel),
                        MaximumPostLimitToStore = generalSettingsModel.MaxPostCountToStore,
                        SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                    };
                    currentCampaignsFetchDetails.Add(scrapeFetchModel);
                }

                #endregion

                #region SharePost

                if (PublisherCreateCampaignModel.SharePostModel.IsShareFdPagePost)
                {
                    var shareFetchModel = new PublisherPostFetchModel
                    {
                        CampaignId = PublisherCreateCampaignModel.CampaignId,
                        CampaignName = PublisherCreateCampaignModel.CampaignName,
                        ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                        PostSource = PostSource.SharePost,
                        PostDetailsWithFilters = JsonConvert.SerializeObject(PublisherCreateCampaignModel.SharePostModel),
                        MaximumPostLimitToStore = generalSettingsModel.MaxPostCountToStore,
                        SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                    };

                    currentCampaignsFetchDetails.Add(shareFetchModel);

                }

                #endregion

                GenericFileManager.AddRangeModule(currentCampaignsFetchDetails, ConstantVariable.GetPublisherPostFetchFile);

                var publisherPostFetcher = new PublisherPostFetcher();
                publisherPostFetcher.FetchPostsForCampaign(PublisherCreateCampaignModel.CampaignId);

                #endregion

                #region Updating PublisherDefaultPage

                var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                {
                    CampaignName = PublisherCreateCampaignModel.CampaignName,
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    StartDate = PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate,
                    EndDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                    CreatedDate = PublisherCreateCampaignModel.CreatedDate,
                    Status = PublisherCreateCampaignModel.CampaignStatus,
                    DestinationCount = PublisherCreateCampaignModel.LstDestinationId.Count,
                    IsRunSingleAccountPerCampaign = generalSettingsModel.IsDoNotPublishPostsChecked,
                    IsRotateDayChecked = PublisherCreateCampaignModel.JobConfigurations.IsRotateDayChecked,
                    TimeRange = PublisherCreateCampaignModel.JobConfigurations.TimeRange,
                    SpecificRunningTime = PublisherCreateCampaignModel.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                    ScheduledWeekday = PublisherCreateCampaignModel.JobConfigurations.Weekday,
                    PendingCount = publisherPostlistModel.LstPublishedPostDetailsModels.Count,
                    DestinationTimeout = generalSettingsModel.WaitMaxOf,
                    IsTakeRandomDestination = !PublisherCreateCampaignModel.JobConfigurations.IsPublishPostOnDestinationsChecked,
                    TotalRandomDestination = PublisherCreateCampaignModel.JobConfigurations.PublishOn,
                    MinRandomDestinationPerAccount = PublisherCreateCampaignModel.JobConfigurations.PostBetween.EndValue
                };

                PublisherInitialize.GetInstance.AddCampaignDetails(publisherCampaignStatusModel);

                #region Update Destination

                PublisherManageDestinationModel.AddCampaignToDestinationList(PublisherCreateCampaignModel.LstDestinationId, PublisherCreateCampaignModel.CampaignId);

                #endregion

                if (PublisherCreateCampaignModel.CampaignStatus == PublisherCampaignStatus.Active)
                    PublishScheduler.ScheduleTodaysPublisherByCampaign(PublisherCreateCampaignModel.CampaignId);

                ClearCurrentCampaigns();

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void ClearCurrentCampaigns()
        {
            PublisherCreateCampaignModel = new PublisherCreateCampaignModel();

            var defaultDays = new List<ContentSelectGroup>();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                defaultDays.Add(new ContentSelectGroup
                {
                    Content = day.ToString(),
                    IsContentSelected = true
                });
            }

            PublisherCreateCampaignModel.JobConfigurations.Weekday = defaultDays;
            SetDataContext();

            SelectedItem = string.Empty;
        }

        private bool SelectDestinationCanExecute(object sender) => true;

        private void SelectDestinationExecute(object sender)
        {
            var selectDestinations = new SelectDestinations(_publisherCreateCampaignModel.LstDestinationId);
            var dialog = new Dialog();
            var metroWindow = dialog.GetMetroWindow(selectDestinations, "Select Destination");
            var isCanceled = false;
            selectDestinations.btnCancel.Click += (cancelEventArgs, eventarg) =>
            {
                selectDestinations.PublisherManageDestinationViewModel.ListPublisherManageDestinationModels.Select(x =>
                {
                    x.IsSelected = false;
                    return x;
                });
                isCanceled = true;
                metroWindow.Close();
            };
            metroWindow.ShowDialog();

            if (!isCanceled)
            {
                var destinationId = selectDestinations.PublisherManageDestinationViewModel
                     .ListPublisherManageDestinationModels
                     .Where(x => x.IsSelected).Select(x => x.DestinationId).ToList();
                _publisherCreateCampaignModel.LstDestinationId = new ObservableCollection<string>(destinationId);
            }
        }

        private bool CampaignChangedCanExecute(object sender) => true;

        private void CampaignChangedExecute(object sender)
        {
            try
            {
                PublisherCreateCampaignModel = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>
                      (ConstantVariable.GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignName == (string)sender);
                BindTabItemsControlProperties();
                SetDataContext();
            }
            catch (Exception)
            {
                ClearCurrentCampaigns();
            }
        }

        private void SetDataContext()
        {
            var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);
            var publisherRssFeed = PublisherRssFeed.GetPublisherRssFeed(tabItemsControl);
            var publisherMonitorFolder = PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl);
            var publisherSharePost = PublisherSharePost.GetPublisherSharePost(tabItemsControl);
            var publisherScrapePost = PublisherScrapePost.GetPublisherScrapePost(tabItemsControl);

            SetPostContectData(publisherDirectPosts);
            SetPublisherRssFeedData(publisherRssFeed);
            SetPublisherMonitorFolder(publisherMonitorFolder);
            SetPublisherSharePost(publisherSharePost);
            SetPublisherScrapePost(publisherScrapePost);

        }
        private void SetPublisherSharePost(PublisherSharePost publisherScrapePost)
        {
            publisherScrapePost.PublisherSharePostViewModel.SharePostModel =
                PublisherCreateCampaignModel.SharePostModel;
        }
        private void SetPublisherScrapePost(PublisherScrapePost publisherScrapePost)
        {
            publisherScrapePost.PublisherScrapePostViewModel.ScrapePostModel =
                PublisherCreateCampaignModel.ScrapePostModel;
        }
        private void SetPublisherMonitorFolder(PublisherMonitorFolder publisherMonitorFolder)
        {
            publisherMonitorFolder.PublisherMonitorFolderViewModel.LstFolderPath =
                PublisherCreateCampaignModel.LstFolderPath;
        }
        private void SetPublisherRssFeedData(PublisherRssFeed publisherRssFeed)
        {
            publisherRssFeed.PublisherRssFeedViewModel.LstFeedUrl =
                PublisherCreateCampaignModel.LstFeedUrl;
        }
        private void SetPostContectData(PublisherDirectPosts publisherDirectPosts)
        {
            publisherDirectPosts.PublisherDirectPostsViewModel.PostDetailsModel =
                PublisherCreateCampaignModel.PostDetailsModel;
            publisherDirectPosts.PostContentControl.SetMedia();
            publisherDirectPosts.ImageMediaViewer.Initialize();
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void BindTabItemsControlProperties()
        {
            tabItemsControl.PublisherDirectPostsViewModel = new PublisherDirectPostsViewModel(tabItemsControl);
            tabItemsControl.PublisherRssFeedViewModel = new PublisherRssFeedViewModel(tabItemsControl);
            tabItemsControl.PublisherMonitorFolderViewModel = new PublisherMonitorFolderViewModel(tabItemsControl);
            tabItemsControl.PublisherSharePostViewModel = new PublisherSharePostViewModel(tabItemsControl);
            tabItemsControl.PublisherScrapePostViewModel = new PublisherScrapePostViewModel(tabItemsControl);
            tabItemsControl.LstFolderPath = PublisherCreateCampaignModel.LstFolderPath;
            tabItemsControl.PostDetailsModel = PublisherCreateCampaignModel.PostDetailsModel;
            tabItemsControl.LstFeedUrl = PublisherCreateCampaignModel.LstFeedUrl;
            tabItemsControl.SharePostModel = PublisherCreateCampaignModel.SharePostModel;
            tabItemsControl.ScrapePostModel = PublisherCreateCampaignModel.ScrapePostModel;
        }

        public class TabItemsControl
        {
            public PostDetailsModel PostDetailsModel { get; set; }
            public PublisherDirectPostsViewModel PublisherDirectPostsViewModel { get; set; }
            public ObservableCollection<PublisherRssFeedModel> LstFeedUrl { get; set; }
            public PublisherRssFeedViewModel PublisherRssFeedViewModel { get; set; }
            public ObservableCollection<PublisherMonitorFolderModel> LstFolderPath { get; set; }
            public PublisherMonitorFolderViewModel PublisherMonitorFolderViewModel { get; set; }
            public SharePostModel SharePostModel { get; set; }
            public PublisherSharePostViewModel PublisherSharePostViewModel { get; set; }
            public ScrapePostModel ScrapePostModel { get; internal set; }
            public PublisherScrapePostViewModel PublisherScrapePostViewModel { get; set; }

        }

    }
}