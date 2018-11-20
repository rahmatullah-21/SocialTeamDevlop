using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Process;
using DominatorUIUtility.Views.Publisher.AdvancedSettings;
using Newtonsoft.Json;
using JobConfiguration = DominatorUIUtility.Views.SocioPublisher.JobConfiguration;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherCreateCampaignViewModel : INotifyPropertyChanged
    {

        #region Constructor

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

            PublisherCreateCampaignModel.JobConfigurations.InitializeDefaultJobConfiguration();

            JobConfiguration = JobConfiguration.GetInstance(PublisherCreateCampaignModel.JobConfigurations);

            JobConfigurationControl = JobConfiguration;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Tab controls for keeping respective post sources
        /// </summary>
        private TabItemsControl tabItemsControl { get; set; } = new TabItemsControl();

        private PublisherCreateCampaignModel _publisherCreateCampaignModel = new PublisherCreateCampaignModel();
        /// <summary>
        /// Create campaign Model for Publisher
        /// </summary>
        public PublisherCreateCampaignModel PublisherCreateCampaignModel
        {
            get
            {
                return _publisherCreateCampaignModel;
            }
            set
            {
                _publisherCreateCampaignModel = value;
                OnPropertyChanged(nameof(PublisherCreateCampaignModel));
            }
        }

        /// <summary>
        /// Current Job Configuarion control which holds UI 
        /// </summary>
        public JobConfiguration JobConfiguration { get; set; }

        private UserControl _jobConfigurationControl;

        /// <summary>
        /// Job Configuration control
        /// </summary>
        public UserControl JobConfigurationControl
        {
            get
            {
                return _jobConfigurationControl;
            }
            set
            {
                _jobConfigurationControl = value;
                OnPropertyChanged(nameof(JobConfigurationControl));
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
        // Selected Campaign
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



        private string _pageTitle;
        /// <summary>
        /// To specify the Campaign is belongs to Edit Campaign or Save Campaign
        /// </summary>
        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }
            set
            {
                if (_pageTitle == value)
                    return;

                _pageTitle = value;

                OnPropertyChanged(nameof(PageTitle));
            }
        }




        public List<TabItemTemplates> PostTabItems { get; set; }

        #endregion

        #region Command

        /// <summary>
        /// Send back to default page
        /// </summary>
        public ICommand NavigationCommand { get; set; }

        /// <summary>
        /// Save the current Campaign
        /// </summary>
        public ICommand SaveCommand { get; set; }

        /// <summary>
        /// For selecting saved destinations
        /// </summary>
        public ICommand SelectDestinationCommand { get; set; }

        /// <summary>
        /// Switching between one campaign for another campaign
        /// </summary>
        public ICommand CampaignChangedCommand { get; set; }

        #endregion

        #region Command Methods

        private bool NavigationCanExecute(object sender) => true;

        private void NavigationExecute(object sender)
        {
            var module = sender.ToString();
            switch (module)
            {
                case "Back":
                    // Return the current Ui to Default Page
                    PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                        = PublisherDefaultPage.Instance();
                    // Call Clear current campaigns
                    ClearCurrentCampaigns();
                    break;
            }
        }

        private bool SaveCanExecute(object sender) => true;

        private void SaveExecute(object sender)
        {

            #region Validations

            // Verify whether timer setted or not
            if (_publisherCreateCampaignModel.JobConfigurations.LstTimer.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please select proper time to run!");
                return;
            }

            // Verify whether destination selected or not
            if (_publisherCreateCampaignModel.LstDestinationId.Count == 0)
            {
                Dialog.ShowDialog("Warning", "Please select atleast one Destination.");
                return;
            }

            // Check whether campaign has end date or not 
            if (!PublisherCreateCampaignModel.JobConfigurations.IsCampaignHasEndDateChecked)
                PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate = null;

            // Check whether campaign has start date or not 
            if (!PublisherCreateCampaignModel.JobConfigurations.IsCampaignHasStartDateChecked)
                PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate = null;


            // Check whether campaign's end date has greater than start date
            if (PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate != null &&
                PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate != null &&
                PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate >
                PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate)
            {
                Dialog.ShowDialog("Warning", "Please check campaign's start time, Should be less than end time!");
                return;
            }
            // If end date already expired, then mark as completed
            if (PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate != null
                && PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate < DateTime.Now)
                PublisherCreateCampaignModel.CampaignStatus = PublisherCampaignStatus.Completed;

            #endregion

            try
            {
                // Gettings general settings of current campaign
                var generalSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                    .FirstOrDefault(x => x.CampaignId == PublisherCreateCampaignModel.CampaignId) ?? new GeneralModel();

                #region Saving post

                // Default post list model
                var publisherPostlistModel = new PublisherPostlistModel
                {
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    CreatedTime = DateTime.Now,
                    PostSource = PostSource.NormalPost,
                    PostQueuedStatus = PostQueuedStatus.Pending,
                    PostRunningStatus = PostRunningStatus.Active,
                    ExpiredTime = null
                };

                // Collect all Campaign saved posts 
                var campaignDetails = PostlistFileManager.GetAll(PublisherCreateCampaignModel.CampaignId);

                // Gather post Ids
                var postIdlist = campaignDetails.Select(x => x.PostId).ToList();

                // Get the direct post UI object
                var directpostViewModel = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl).PublisherDirectPostsViewModel;

                // Used for image multipost unique options
                var mediaUrl = new List<string>();

                //Calculate how much post should add in post list
                var maxPostCount = generalSettingsModel.MaxPostCountToStore - campaignDetails.Count;

                if (maxPostCount > 0)
                {
                    var postCount = 0;

                    // Direct Post Sections
                    #region Direct Post Sections

                    // Add all post in PublisherCreateCampaignModel.PostCollection to Bin file
                    foreach (var post in PublisherCreateCampaignModel.PostCollection)
                    {
                        // Checking whether post contains any one (Post Description , Atleast one media count, Title, Source Url)
                        if (!string.IsNullOrEmpty(post.PostDescription) ||
                            post.MediaViewer.MediaList.Count > 0 ||
                            !string.IsNullOrEmpty(post.PublisherInstagramTitle) ||
                            !string.IsNullOrEmpty(post.PdSourceUrl))
                        {

                            if (postCount >= maxPostCount)
                                break;

                            // Get deep clone of the post 
                            var postData = post.DeepClone();

                            // Check whether current post is belongs to multiple images or not
                            if (postData.IsMultipleImagePost)
                            {
                                // check whether user need to use File name as post description
                                if (!post.IsUseFileNameAsDescription)
                                {
                                    // postData.PostDescription = string.Empty;
                                    postData.PostDescription = "image " + (PublisherCreateCampaignModel.PostCollection.IndexOf(post) + 1);
                                }
                                // check whether user need to use unique post
                                if (post.IsUniquePost)
                                {
                                    if (mediaUrl.Contains(postData.MediaList[0]))
                                        continue;

                                    mediaUrl.Add(postData.MediaList[0]);
                                }
                            }

                            // Add the item into bin file
                            AddPostlists(postIdlist, postData);

                            postCount++;

                            // Check whether client needs to readd post or not
                            if (!postData.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
                                continue;

                            // Iterate the current post to readding times 
                            for (var readdCount = 1; readdCount < postData.PublisherPostSettings.GeneralPostSettings.ReaddCount; readdCount++)
                            {
                                if (postCount >= maxPostCount)
                                    break;
                                var newpost = postData.DeepClone();
                                newpost.PostDetailsId = Utilities.GetGuid();
                                AddPostlists(postIdlist, newpost);
                                postCount++;
                            }
                        }
                    }
                    #endregion

                    //Share Post sections
                    #region Share Post sections
                    if (PublisherCreateCampaignModel.SharePostModel.IsShareCustomPostList)
                    {
                        // Split the share post items by new line
                        var shareUrls = Regex
                            .Split(PublisherCreateCampaignModel.SharePostModel.ShareAddCustomPostList, "\r\n").ToList();

                        // Add the item into post list bin files
                        foreach (var shareUrl in shareUrls)
                        {
                            if (postCount >= maxPostCount)
                                break;
                            publisherPostlistModel.PostId = Utilities.GetGuid();
                            publisherPostlistModel.ShareUrl = shareUrl.Trim();
                            publisherPostlistModel.PostSource = PostSource.SharePost;
                            PostlistFileManager.Add(PublisherCreateCampaignModel.CampaignId, publisherPostlistModel);
                            postCount++;
                        }
                    }
                    #endregion
                }


                #endregion

                #region Fetch Post Details

                PublisherPostFetcher.StopFetchingPostsByCampaignId(PublisherCreateCampaignModel.CampaignId);

                // Assign New objects to hold post fetcher
                var currentCampaignsFetchDetails = new List<PublisherPostFetchModel>();

                // Direct post fetcher details, This is useful for getting campaign Name while running pharse
                #region DirectPostPosts


                var directPostModel = new PublisherPostFetchModel
                {
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    CampaignName = PublisherCreateCampaignModel.CampaignName,
                    ExpireDate = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate,
                    PostSource = PostSource.NormalPost,
                    SelectedDestinations = PublisherCreateCampaignModel.LstDestinationId,
                    MaximumPostLimitToStore = generalSettingsModel.MaxPostCountToStore,
                };

                currentCampaignsFetchDetails.Add(directPostModel);

                #endregion

                // Monitor Folder details with maximum count, notify count, Destination, delay for fetching every new posts
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
                        NotifyCount = generalSettingsModel.TriggerNotificationCount,

                    };

                    currentCampaignsFetchDetails.Add(monitorFolderFetchModel);
                }

                #endregion

                // Rss Feed details with maximum count, notify count, Destination, delay for fetching every new posts
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
                        NotifyCount = generalSettingsModel.TriggerNotificationCount
                    };

                    currentCampaignsFetchDetails.Add(rssFetchModel);
                }

                #endregion

                // scrape post details, This is useful for only for Facebook,Pinterest, Twitter
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
                        NotifyCount = generalSettingsModel.TriggerNotificationCount,
                        ScrapeCount = PublisherCreateCampaignModel.ScrapePostModel.ScrapeCount,
                        DelayForNext = PublisherCreateCampaignModel.ScrapePostModel.StartScrapeOnXminute
                    };
                    currentCampaignsFetchDetails.Add(scrapeFetchModel);
                }

                #endregion

                // share post details, This is useful for only for Facebook
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
                        NotifyCount = generalSettingsModel.TriggerNotificationCount,
                        ScrapeCount = PublisherCreateCampaignModel.SharePostModel.ScrapeCount,
                        DelayForNext = PublisherCreateCampaignModel.SharePostModel.StartScrapeOnXminute
                    };
                    currentCampaignsFetchDetails.Add(shareFetchModel);
                }

                #endregion

                GenericFileManager.AddRangeModule(currentCampaignsFetchDetails, ConstantVariable.GetPublisherPostFetchFile);

                var publisherPostFetcher = new PublisherPostFetcher();
                publisherPostFetcher.FetchPostsForCampaign(PublisherCreateCampaignModel.CampaignId);

                #endregion

                #region Updating PublisherDefaultPage

                // Calculate the start date of the campaigns
                DateTime? startTime = null;
                if (PublisherCreateCampaignModel.JobConfigurations.IsCampaignHasStartDateChecked)
                    startTime = PublisherCreateCampaignModel.JobConfigurations.CampaignStartDate;

                // Calculate the end date of the campaigns
                DateTime? endTime = null;
                if (PublisherCreateCampaignModel.JobConfigurations.IsCampaignHasEndDateChecked)
                    endTime = PublisherCreateCampaignModel.JobConfigurations.CampaignEndDate;


                // Current Campaign Status Details for display in default pages
                var publisherCampaignStatusModel = new PublisherCampaignStatusModel
                {
                    CampaignName = PublisherCreateCampaignModel.CampaignName,
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    StartDate = startTime,
                    EndDate = endTime,
                    CreatedDate = PublisherCreateCampaignModel.CreatedDate,
                    UpdatedTime = PublisherCreateCampaignModel.UpdatedDate,
                    Status = PublisherCreateCampaignModel.CampaignStatus,
                    DestinationCount = PublisherCreateCampaignModel.LstDestinationId.Count,
                    IsRotateDayChecked = PublisherCreateCampaignModel.JobConfigurations.IsRotateDayChecked,
                    TimeRange = PublisherCreateCampaignModel.JobConfigurations.TimeRange,
                    IsRandomRunningTime = PublisherCreateCampaignModel.JobConfigurations.IsRandomizePublishingTimerChecked,
                    MaximumTime = PublisherCreateCampaignModel.JobConfigurations.MaxPost,
                    SpecificRunningTime = PublisherCreateCampaignModel.JobConfigurations.LstTimer.Select(x => x.MidTime).ToList(),
                    ScheduledWeekday = PublisherCreateCampaignModel.JobConfigurations.Weekday,
                    PendingCount = publisherPostlistModel.LstPublishedPostDetailsModels.Count,
                    IsTakeRandomDestination = !PublisherCreateCampaignModel.JobConfigurations.IsPublishPostOnDestinationsChecked,
                    TotalRandomDestination = PublisherCreateCampaignModel.JobConfigurations.RandomDestinationCount,
                    MinRandomDestinationPerAccount = PublisherCreateCampaignModel.JobConfigurations.PostBetween.EndValue,
                };

                var CampaignStatusModel = PublisherInitialize.GetInstance.GetSavedCampaigns().FirstOrDefault(x => x.CampaignId == PublisherCreateCampaignModel.CampaignId);
                if (CampaignStatusModel != null)
                {
                    UpdateCampaign(publisherCampaignStatusModel, CampaignStatusModel);
                }
                else
                {
                    // Get the object of Publisher Instance
                    var publishIntialize = PublisherInitialize.GetInstance;

                    // Add current campaigns to default pate
                    publishIntialize.AddCampaignDetails(publisherCampaignStatusModel);
                }




                #region Update Destination

                // Update the destination with current campaign
                PublisherManageDestinationModel.AddCampaignToDestinationList(PublisherCreateCampaignModel.LstDestinationId, PublisherCreateCampaignModel.CampaignId);

                #endregion

                #endregion

                #region Saving Campign to PublisherCampaign.bin file

                var lstCampaign = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(
                    ConstantVariable.GetPublisherCampaignFile());

                //PublisherCreateCampaignModel.PostDetailsModel = new PostDetailsModel();
                PublisherCreateCampaignModel.UpdatedDate = DateTime.Now;
                //PublisherCreateCampaignModel.LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();

                #region Update the Campaigns

                if (lstCampaign.Any(x => x.CampaignId == PublisherCreateCampaignModel.CampaignId))
                {
                    var campaignIndex = lstCampaign.IndexOf(lstCampaign.FirstOrDefault(x => x.CampaignName == SelectedItem));

                    if (campaignIndex < 0)
                        return;

                    lstCampaign[campaignIndex] = PublisherCreateCampaignModel;

                    if (GenericFileManager.UpdateModuleDetails<PublisherCreateCampaignModel>(lstCampaign,
                        ConstantVariable.GetPublisherCampaignFile()))
                        Dialog.ShowDialog("Success", "Campaign successfully updated.");

                    // Stop Scheduled Activities
                    PublishScheduler.StopPublishingPosts(PublisherCreateCampaignModel.CampaignId);
                }

                #endregion

                #region Save the campaigns
                else
                {
                    if (GenericFileManager.AddModule<PublisherCreateCampaignModel>(PublisherCreateCampaignModel, ConstantVariable.GetPublisherCampaignFile()))
                        Dialog.ShowDialog("Success", "Campaign successfully saved.");

                    CampaignList.Add(PublisherCreateCampaignModel.CampaignName);
                }
                #endregion

                #endregion

                // If campaign is active then schedule for posting
                if (PublisherCreateCampaignModel.CampaignStatus == PublisherCampaignStatus.Active)
                    PublishScheduler.ScheduleTodaysPublisherByCampaign(PublisherCreateCampaignModel.CampaignId);

                // Send back to default page
                PublisherHome.Instance.PublisherHomeViewModel.PublisherHomeModel.SelectedUserControl
                    = PublisherDefaultPage.Instance();

                // Clear current objects
                ClearCurrentCampaigns();

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateCampaign(PublisherCampaignStatusModel publisherCampaignStatusModel,
            PublisherCampaignStatusModel CampaignStatusModel)
        {
            publisherCampaignStatusModel.PendingCount = CampaignStatusModel.PendingCount + PublisherCreateCampaignModel
                                                            .PostCollection
                                                            .Count(x => x.PostQueuedStatus == PostQueuedStatus.Pending);
            publisherCampaignStatusModel.DraftCount = CampaignStatusModel.DraftCount + PublisherCreateCampaignModel
                                                          .PostCollection
                                                          .Count(x => x.PostQueuedStatus == PostQueuedStatus.Draft);
            publisherCampaignStatusModel.PublishedCount =
                CampaignStatusModel.PublishedCount + PublisherCreateCampaignModel.PostCollection
                    .Count(x => x.PostQueuedStatus == PostQueuedStatus.Published);
            // Finding current items
            var currentItem =
                PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.FirstOrDefault(x =>
                    x.CampaignId == PublisherCreateCampaignModel.CampaignId);

            // Get the index of the current campaign
            var index = PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.IndexOf(currentItem);

            // Substutite with proper index
            PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels[index] = publisherCampaignStatusModel;

            var LstPublishedPostDetailsModels = PostlistFileManager.GetAll(publisherCampaignStatusModel.CampaignId);
            PublisherCreateCampaignModel.PostCollection.ForEach(post =>
            {
                var postlistModel = new PublisherPostlistModel
                {
                    CampaignId = PublisherCreateCampaignModel.CampaignId,
                    CreatedTime = DateTime.Now,
                    PostSource = PostSource.NormalPost,
                    PostQueuedStatus = post.PostQueuedStatus,
                    PostRunningStatus = PostRunningStatus.Active,
                    PostDescription = post.PostDescription,
                    PdSourceUrl = post.PdSourceUrl,
                    PublisherInstagramTitle = post.PublisherInstagramTitle,
                    MediaList = post.MediaViewer.MediaList,
                    FdSellLocation = post.FdSellLocation,
                    PostId = post.PostDetailsId,
                    PostCategory = post.IsFdSellPost ? PostCategory.SellPost : PostCategory.OrdinaryPost,
                };
                LstPublishedPostDetailsModels.Add(postlistModel);
            });

            // Update the post details to bin file
            PostlistFileManager.UpdatePostlists(PublisherCreateCampaignModel.CampaignId, LstPublishedPostDetailsModels);
        }

        /// <summary>
        /// Save the post to Bin file
        /// </summary>
        /// <param name="postIdlist">For Identify where current post is need to update or create new</param>
        /// <param name="post">Valid post details</param>
        private void AddPostlists(List<string> postIdlist, PostDetailsModel post)
        {
            // Calculate the expire date for the campaigns
            DateTime? expireDate = null;

            if (post.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
                expireDate = post.PublisherPostSettings.GeneralPostSettings.ExpireDate;

            // Initialize all the post details from current post list
            var postlistModel = new PublisherPostlistModel
            {
                CampaignId = PublisherCreateCampaignModel.CampaignId,
                CreatedTime = DateTime.Now,
                PostSource = PostSource.NormalPost,
                PostQueuedStatus = post.PostQueuedStatus,
                PostRunningStatus = PostRunningStatus.Active,
                PostDescription = post.PostDescription,
                MediaList = post.MediaViewer.MediaList,
                PublisherInstagramTitle = post.PublisherInstagramTitle,
                PdSourceUrl = post.PdSourceUrl,
                FdSellLocation = post.FdSellLocation,
                FdSellPrice = post.FdSellPrice,
                FdSellProductTitle = post.FdSellProductTitle,
                IsFdSellPost = post.IsFdSellPost,
                PostId = post.PostDetailsId,
                GeneralPostSettings = post.PublisherPostSettings.GeneralPostSettings,
                FdPostSettings = post.PublisherPostSettings.FdPostSettings,
                GdPostSettings = post.PublisherPostSettings.GdPostSettings,
                TdPostSettings = post.PublisherPostSettings.TdPostSettings,
                LdPostSettings = post.PublisherPostSettings.LdPostSettings,
                TumberPostSettings = post.PublisherPostSettings.TumberPostSettings,
                RedditPostSetting = post.PublisherPostSettings.RedditPostSetting,
                PublisherPostSettings = post.PublisherPostSettings,
                ExpiredTime = expireDate,
                PostCategory = post.IsFdSellPost ? PostCategory.SellPost : PostCategory.OrdinaryPost,
            };

            // Assign Created Date Time
            postlistModel.CreatedTime = post.CreatedDateTime;

            // Update the post details
            if (postIdlist.Contains(post.PostDetailsId))
            {
                var savedPost = PostlistFileManager.GetByPostId(PublisherCreateCampaignModel.CampaignId, post.PostDetailsId);
                postlistModel.LstPublishedPostDetailsModels = savedPost.LstPublishedPostDetailsModels;
                postlistModel.PostQueuedStatus = savedPost.PostQueuedStatus;
                PostlistFileManager.UpdatePost(PublisherCreateCampaignModel.CampaignId, postlistModel);
            }
            // Add new post details
            else
                PostlistFileManager.Add(PublisherCreateCampaignModel.CampaignId, postlistModel);
        }

        /// <summary>
        /// Clear the current campaign model, and make the Page title to Create campaign
        /// </summary>
        public void ClearCurrentCampaigns()
        {
            PublisherCreateCampaignModel = new PublisherCreateCampaignModel();
            PublisherCreateCampaignModel.JobConfigurations.InitializeDefaultJobConfiguration();
            CampaignList = new ObservableCollection<string>(
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile()).Select(x => x.CampaignName));
            SelectedItem = null;
            PageTitle = Application.Current.FindResource("LangKeyCreateCampaign")?.ToString();
            SetDataContext();
        }

        private bool SelectDestinationCanExecute(object sender) => true;

        private void SelectDestinationExecute(object sender)
        {
            // Calling Select Destination UI with selected destinations
            var selectDestinations = new SelectDestinations(_publisherCreateCampaignModel.LstDestinationId);
            var dialog = new Dialog();

            // Pass the UI object with Title of the Page
            var metroWindow = dialog.GetMetroWindow(selectDestinations, "Select Destination");
            var isCanceled = false;

            // Cancel button event
            selectDestinations.btnCancel.Click += (cancelEventArgs, eventarg) =>
            {
                // Make all destination to un selected
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
                // Get the selected destination Id
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
                // Fetch the campaign Models from bin file
                var campaignlists = GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>
                    (ConstantVariable.GetPublisherCampaignFile());

                // Get the current campaign Name 
                PublisherCreateCampaignModel = campaignlists.FirstOrDefault(x => x.CampaignName == (string)sender);

                // Initialize the respective tab items of post sources
                BindTabItemsControlProperties();

                // If campaigns is present then make the campaign Title as Edit Campaign
                PageTitle = Application.Current.FindResource("LangKeyEditCampaign")?.ToString();

                // Assign the Data Context
                SetDataContext();
            }
            catch (Exception)
            {
                // Clear current campaign objects
                ClearCurrentCampaigns();
            }
        }

        /// <summary>
        /// To set the data context for Post fetecher details
        /// </summary>
        private void SetDataContext()
        {
            // get the direct posts ui 
            var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);

            // get the rss posts ui 
            var publisherRssFeed = PublisherRssFeed.GetPublisherRssFeed(tabItemsControl);

            // get the monitor folder posts ui 
            var publisherMonitorFolder = PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl);

            // get the share post UI
            var publisherSharePost = PublisherSharePost.GetPublisherSharePost(tabItemsControl);

            // get the scrape post UI
            var publisherScrapePost = PublisherScrapePost.GetPublisherScrapePost(tabItemsControl);

            // get the job configurations
            JobConfiguration = JobConfiguration.GetInstance(PublisherCreateCampaignModel.JobConfigurations);

            JobConfigurationControl = JobConfiguration;

            // set the direct post saved post details
            SetPostContectData(publisherDirectPosts);

            // set the rss feed post saved post details
            SetPublisherRssFeedData(publisherRssFeed);

            // set the monitor folder saved post details
            SetPublisherMonitorFolder(publisherMonitorFolder);

            // set the share saved post details
            SetPublisherSharePost(publisherSharePost);

            // set the scrape post input details
            SetPublisherScrapePost(publisherScrapePost);

        }

        /// <summary>
        /// To change the share posts details
        /// </summary>
        /// <param name="publisherScrapePost"><see cref="PublisherSharePost"/> given scarpe post details</param>
        private void SetPublisherSharePost(PublisherSharePost publisherScrapePost)
        {
            // assign the share post model with given input, which holds already saved details
            publisherScrapePost.PublisherSharePostViewModel.SharePostModel =
                PublisherCreateCampaignModel.SharePostModel;
        }

        /// <summary>
        /// To change the scrape posts details
        /// </summary>
        /// <param name="publisherScrapePost"><see cref="PublisherScrapePost"/>given input while saving</param>
        private void SetPublisherScrapePost(PublisherScrapePost publisherScrapePost)
        {
            // Assign the scrape posts model
            publisherScrapePost.PublisherScrapePostViewModel.ScrapePostModel =
                PublisherCreateCampaignModel.ScrapePostModel;
        }

        /// <summary>
        /// To change the monitor folder details
        /// </summary>
        /// <param name="publisherMonitorFolder"><see cref="PublisherMonitorFolder"/> given input details</param>
        private void SetPublisherMonitorFolder(PublisherMonitorFolder publisherMonitorFolder)
        {
            // Assign model for to Ui 
            publisherMonitorFolder.PublisherMonitorFolderViewModel.LstFolderPath =
                PublisherCreateCampaignModel.LstFolderPath;

            publisherMonitorFolder.PublisherMonitorFolderViewModel.PublisherMonitorFolderModel = new PublisherMonitorFolderModel();

            // Set the saved media url
            publisherMonitorFolder.PostContentControl.SetMedia();
        }

        /// <summary>
        /// To change the Rss Feed 
        /// </summary>
        /// <param name="publisherRssFeed"><see cref="PublisherRssFeed"/> Rss feed details</param>
        private void SetPublisherRssFeedData(PublisherRssFeed publisherRssFeed)
        {
            // Assign already saved feed url's
            publisherRssFeed.PublisherRssFeedViewModel.LstFeedUrl =
                PublisherCreateCampaignModel.LstFeedUrl;
            publisherRssFeed.PublisherRssFeedViewModel.PublisherRssFeedModel = new PublisherRssFeedModel();

            // Set the save media urls
            publisherRssFeed.PostContentControl.SetMedia();
        }

        /// <summary>
        /// To change the direct post details
        /// </summary>
        /// <param name="publisherDirectPosts"><see cref="PublisherDirectPosts"/>Saved direct post details</param>
        private void SetPostContectData(PublisherDirectPosts publisherDirectPosts)
        {
            // Assign direct post model
            publisherDirectPosts.PublisherDirectPostsViewModel.PostDetailsModel =
                PublisherCreateCampaignModel.PostDetailsModel;

            //Set the Images and initialize neccessary image viewer details
            publisherDirectPosts.PostContentControl.SetMedia();
            publisherDirectPosts.ImageMediaViewer.Initialize();
        }

        #endregion

        #region By Default Methods

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Initialize publisher post source details
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


        // Post Source UI
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


        private List<TabItemTemplates> InitializeTabs()
        {

            // UI for Direct Posts
            var tabItems = new List<TabItemTemplates>
            {
                new TabItemTemplates
                {
                    Title= Application.Current.FindResource("LangKeyCreatePost")?.ToString(),
                    Content=new Lazy<UserControl>(()=> new PublisherDirectPosts(tabItemsControl))
                }
            };

            // Scarpe post UI, If any one of network(Facebook,Pinterest, Twitter)
            #region Scrape Posts

            if (FeatureFlags.IsNetworkAvailable(SocialNetworks.Facebook) ||
                FeatureFlags.IsNetworkAvailable(SocialNetworks.Pinterest) ||
                FeatureFlags.IsNetworkAvailable(SocialNetworks.Twitter))
            {
                tabItems.Add(new TabItemTemplates
                {
                    Title = Application.Current.FindResource("LangKeyScrapePost")?.ToString(),
                    Content = new Lazy<UserControl>(() => PublisherScrapePost.GetPublisherScrapePost(tabItemsControl))
                });
            }

            #endregion

            // Share Post, Rss and Monitor Folder Ul
            #region Share , Monitor Folder, Rss

            tabItems.Add(new TabItemTemplates
            {
                Title = Application.Current.FindResource("LangKeySharePost")?.ToString(),
                Content = new Lazy<UserControl>(() => PublisherSharePost.GetPublisherSharePost(tabItemsControl))
            });


            tabItems.Add(new TabItemTemplates
            {
                Title = Application.Current.FindResource("LangKeyRSSFeed")?.ToString(),
                Content = new Lazy<UserControl>(() => PublisherRssFeed.GetPublisherRssFeed(tabItemsControl))
            });

            tabItems.Add(new TabItemTemplates
            {
                Title = Application.Current.FindResource("LangKeyMonitorFolder")?.ToString(),
                Content = new Lazy<UserControl>(() => PublisherMonitorFolder.GetPublisherMonitorFolder(tabItemsControl))
            });

            #endregion

            return tabItems;
        }

        #endregion
    }
}