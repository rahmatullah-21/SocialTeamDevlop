using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel : BindableBase
    {


        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            SearchCommand = new BaseCommand<object>(SearchCanExecute, SearchExecute);
            SaveCurrentPostCommand = new BaseCommand<object>(CanExecuteSaveSinglePost, CanSaveSinglePost);
            LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();
            BindingOperations.EnableCollectionSynchronization(LstPostDetailsModels, _lock);
        }

        public PublisherDirectPostsViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl) : this()
        {
            this.tabItemsControl = tabItemsControl;
            PostDetailsModel = tabItemsControl.PostDetailsModel;
        }

        #endregion

        #region Properties
        private static object _lock = new object();
        private ObservableCollection<PostDetailsModel> _lstPostDetailsModels;

        public ObservableCollection<PostDetailsModel> LstPostDetailsModels
        {
            get
            {
                return _lstPostDetailsModels;
            }
            set
            {
                if (_lstPostDetailsModels == value)
                    return;
                SetProperty(ref _lstPostDetailsModels, value);
            }
        }

        /// <summary>
        /// Post source details
        /// </summary>
        private PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;

        /// <summary>
        /// Opening mulitple post window
        /// </summary>
        public ICommand MultiplePostCommand { get; set; }

        /// <summary>
        /// Importing Posts from Csv
        /// </summary>
        public ICommand ImportFromCsvCommand { get; set; }

        /// <summary>
        /// Getting multiple image posts 
        /// </summary>
        public ICommand SearchCommand { get; set; }

        /// <summary>
        /// Saving posts to create campaign view model
        /// </summary>
        public ICommand SaveCurrentPostCommand { get; set; }


        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

        /// <summary>
        /// Keep post details, which holds all needed data about post
        /// </summary>
        public PostDetailsModel PostDetailsModel
        {
            get
            {
                return _postDetailsModel;
            }
            set
            {
                if (_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }

        private PublisherMediaViewerModel _publisherMultipleImagesMediaViewerModel = new PublisherMediaViewerModel();

        /// <summary>
        /// To specify the media viewer details
        /// </summary>
        public PublisherMediaViewerModel PublisherMultipleImagesMediaViewerModel
        {
            get
            {
                return _publisherMultipleImagesMediaViewerModel;
            }
            set
            {
                if (_publisherMultipleImagesMediaViewerModel == value)
                    return;
                _publisherMultipleImagesMediaViewerModel = value;
                OnPropertyChanged(nameof(PublisherMultipleImagesMediaViewerModel));
            }
        }

        public List<string> MediaList { get; set; } = new List<string>();


        #endregion

        #region Methods

        public bool CanExecuteSaveSinglePost(object sender) => true;

        /// <summary>
        /// Save the post list
        /// </summary>
        /// <param name="sender"></param>
        public void CanSaveSinglePost(object sender)
        {
            var saveLocation = sender as string;

            try
            {
                // Get the create campaign Post list model
                var createCampaignModel = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                      .PublisherCreateCampaignModel;

                // get all post collection 
                var postCollectionDetails = createCampaignModel.PostCollection;

                // Is need to display logger message
                var isLoggerNeeded = false;

                #region Single Post

                // Single Posts save post
                if (!string.IsNullOrEmpty(PostDetailsModel.PostDescription) ||
                            PostDetailsModel.MediaViewer.MediaList.Count > 0 ||
                            !string.IsNullOrEmpty(PostDetailsModel.PublisherInstagramTitle) ||
                            !string.IsNullOrEmpty(PostDetailsModel.PdSourceUrl))
                {
                    isLoggerNeeded = true;

                    // Getting deep clone
                    var cloneObject = PostDetailsModel.DeepClone();

                    // Assign created date time
                    cloneObject.CreatedDateTime = DateTime.Now;

                    // Post Id
                    cloneObject.PostDetailsId = Utilities.GetGuid();

                    // Post Queued status
                    cloneObject.PostQueuedStatus = saveLocation == "SaveToPending"
                        ? PostQueuedStatus.Pending
                        : PostQueuedStatus.Draft;

                    // Adding to post collections
                    postCollectionDetails.Add(cloneObject);

                    // Clearing current post objects
                    PostDetailsModel = new PostDetailsModel();
                    var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);
                    publisherDirectPosts.PostContentControl.SetMedia();
                    publisherDirectPosts.ImageMediaViewer.Initialize();

                    var createCampaign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
                    createCampaign.PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.PostDetailsModel =
                        new PostDetailsModel();
                    tabItemsControl.PostDetailsModel = new PostDetailsModel();
                }
                #endregion

                #region Multiple Post

                // Check whether multiple posts and image posts contains posts or not
                if (createCampaignModel.LstPostDetailsModels.Count > 0 ||
                            createCampaignModel.LstMultipleImagePostCollection.Count > 0)
                {
                    // Iterate the Multiple posts images
                    createCampaignModel.LstPostDetailsModels.ForEach(post =>
                    {
                        post.PostQueuedStatus = saveLocation == "SaveToPending"
                            ? PostQueuedStatus.Pending
                            : PostQueuedStatus.Draft;

                        // Add to Post Collections 
                        postCollectionDetails.Add(post);
                    });

                    // Iterate the Multiple image posts
                    createCampaignModel.LstMultipleImagePostCollection.ForEach(post =>
                    {
                        post.PostQueuedStatus = saveLocation == "SaveToPending"
                            ? PostQueuedStatus.Pending
                            : PostQueuedStatus.Draft;

                        // Add to Post Collections 
                        postCollectionDetails.Add(post);
                    });

                    if (!Application.Current.Dispatcher.CheckAccess())
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            //Clear the current object values
                            createCampaignModel.LstMultipleImagePostCollection =
                                new ObservableCollection<PostDetailsModel>();
                            createCampaignModel.LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();
                        });
                    }
                    else
                    {
                        //Clear the current object values
                        createCampaignModel.LstMultipleImagePostCollection =
                            new ObservableCollection<PostDetailsModel>();
                        createCampaignModel.LstPostDetailsModels = new ObservableCollection<PostDetailsModel>();
                    }

                    isLoggerNeeded = true;


                    var createCampaign = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns();
                    createCampaign.PublisherCreateCampaignViewModel.PublisherCreateCampaignModel.PostDetailsModel =
                        new PostDetailsModel();

                    // Clearing current direct posts
                    PostDetailsModel = new PostDetailsModel();
                    var publisherDirectPosts = PublisherDirectPosts.GetPublisherDirectPosts(tabItemsControl);
                    publisherDirectPosts.ImageMediaViewer.Initialize();
                }

                #endregion

                #region Save logger infomations

                if (!isLoggerNeeded)
                    return;

                var loggerMessage = new Func<string>(Application.Current.FindResource($@"LangKeyPostSaved").ToString).Invoke();
                if (Application.Current != null)
                {
                    if (!string.IsNullOrEmpty(loggerMessage))
                        GlobusLogHelper.log.Info(loggerMessage);
                }

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public bool CanExecuteMultiPost(object sender) => true;

        /// <summary>
        /// Open Multiple post list window
        /// </summary>
        /// <param name="sender"></param>
        public void ExecuteMultiPost(object sender)
        {
            //ThreadFactory.Instance.Start(() =>
            //{
            try
            {
                // Get the object of multiple post UI
                var publisherMultiplePost = new PublisherMultiplePost();

                // Get the core dialog object
                var dialog = new Dialog();

                // Pass the object with Title
                var window = dialog.GetMetroWindow(publisherMultiplePost, "Multiple Post");

                //DisplayAttribute the dialog
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            //}); 
        }

        private bool SearchCanExecute(object sender) => true;

        /// <summary>
        /// SearchExecute is used for fetching the image posts from the url
        /// </summary>
        /// <param name="sender"></param>
        private void SearchExecute(object sender)
        {
            try
            {
                var mediaViewer = (MediaViewer)sender;

                // check whether Image url is empty or not
                if (string.IsNullOrEmpty(PostDetailsModel.ImagesUrl))
                    return;

                // Start scraping the image url from ImageExtracter.ExtractImageUrls
                PostDetailsModel.MediaList = new ObservableCollection<string>(ImageExtracter.ExtractImageUrls(PostDetailsModel.ImagesUrl));

                // Add the scraped medias to postdetails collection
                PostDetailsModel.MediaList.ForEach(x => MediaList.Add(x));

                // Get the Create campaign View model object for multiple image post
                var postDetails = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstMultipleImagePostCollection;

                // Iterate all the media and add into post detail model
                foreach (var image in PostDetailsModel.MediaList)
                {
                    // Assign the image url
                    var publisherMediaViewerModel = new PublisherMediaViewerModel { MediaList = new ObservableCollection<string> { image } };

                    // Create a new object with fetched image urls
                    var postDetailsModel = new PostDetailsModel
                    {
                        MediaList = new ObservableCollection<string> { image },
                        PostDetailsId = Utilities.GetGuid(),
                        PostDescription = new Uri(image).Segments.Last(),
                        CreatedDateTime = DateTime.Now,
                        MediaViewer = publisherMediaViewerModel,
                        IsMultipleImagePost = true
                    };

                    //Added post lists
                    postDetails.Add(postDetailsModel);
                }

                mediaViewer.MediaList = PostDetailsModel.MediaList;
                // Re intialize post lists
                mediaViewer?.Initialize();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool ImportFromCsvCanExecute(object sender) => true;

        /// <summary>
        /// Import the posts from csv file
        /// </summary>
        /// <param name="sender"></param>
        private void ImportFromCsvExecute(object sender)
        {
            // select the file path
            var listPostDetailsModel = FileUtilities.FileBrowseAndReader();

            if (listPostDetailsModel.Count == 0)
                return;
            try
            {
                // Get all post details from campaign View model
                LstPostDetailsModels = PublisherCreateCampaigns.GetSingeltonPublisherCreateCampaigns().PublisherCreateCampaignViewModel
                    .PublisherCreateCampaignModel.LstPostDetailsModels;

                // Get the object of multiple post UI
                var publisherMultiplePost = new PublisherMultiplePost(LstPostDetailsModels);


                // Get the core dialog object
                var dialog = new Dialog();

                // Pass the object with Title
                var window = dialog.GetMetroWindow(publisherMultiplePost, "Multiple Post");

                //DisplayAttribute the dialog
                window.Show();

                //publisherMultiplePost.UpdatePostDetails.Invoke(publisherMultiplePost);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            // Split with separator
            var separator = ConstantVariable.Separator;

            var mediaUtilites = new MediaUtilites();

            // select the file path
            // var listPostDetailsModel = FileUtilities.FileBrowseAndReader();


            ThreadFactory.Instance.Start(() =>
            {

                // Iterate selected file name
                listPostDetailsModel.ForEach(x =>
                {
                    PostDetailsModel postDetailsModel = new PostDetailsModel();
                    try
                    {
                        // Split the file details
                        var allData = x.Split('\t');

                        postDetailsModel.PostDescription = allData[0];

                        // Media list

                        #region Medialist

                        var mediaUrl = Regex.Split(allData[1], separator).ToList();
                        mediaUrl.ForEach(media =>
                        {
                            if (File.Exists(media))
                                postDetailsModel.MediaViewer.MediaList.Add(mediaUtilites.GetThumbnail(media));

                        });

                        #endregion

                        // Title
                        postDetailsModel.PublisherInstagramTitle = allData[2];

                        // Source url
                        postDetailsModel.PdSourceUrl = allData[3];

                        // Facebook Sell post details

                        #region FdSell

                        var Fdsell = Regex.Split(allData[4], separator);
                        if (string.Compare(Fdsell[0], "Yes", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            string.Compare(Fdsell[0], "Y", StringComparison.CurrentCultureIgnoreCase) == 0 ||
                            string.Compare(Fdsell[0], "True", StringComparison.CurrentCultureIgnoreCase) == 0)
                        {
                            postDetailsModel.IsFdSellPost = true;
                            postDetailsModel.FdSellProductTitle = Fdsell[1];
                            postDetailsModel.FdSellPrice = double.Parse(Fdsell[2]);
                            postDetailsModel.FdSellLocation = Fdsell[3];
                        }

                        #endregion
                        // Created date
                        postDetailsModel.CreatedDateTime = DateTime.Now;

                        // Post id
                        postDetailsModel.PostDetailsId = Utilities.GetGuid();

                        // Add to Collections
                        //postDetails.Add(postDetailsModel);
                        Application.Current.Dispatcher.InvokeAsync(() => LstPostDetailsModels.Add(postDetailsModel));
                        Thread.Sleep(50);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                });
            });

            // If all post read, open and show in UI for Updation
            //if (postDetails?.Count != 0)
            //{
            //    try
            //    {
            //        // Get the object of multiple post UI
            //        var publisherMultiplePost = new PublisherMultiplePost(postDetails);

            //        // Get the core dialog object
            //        var dialog = new Dialog();

            //        // Pass the object with Title
            //        var window = dialog.GetMetroWindow(publisherMultiplePost, "Multiple Post");

            //        //DisplayAttribute the dialog
            //        window.ShowDialog();
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.DebugLog();
            //    }
            //}

        }

        #endregion

    }


}