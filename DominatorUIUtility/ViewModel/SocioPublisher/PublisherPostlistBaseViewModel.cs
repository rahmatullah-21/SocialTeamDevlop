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
using System.Windows.Documents;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;
using DominatorUIUtility.Views.SocioPublisher.Suggestions;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using PublisherEditPost = DominatorUIUtility.Views.SocioPublisher.CustomControl.PublisherEditPost;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherPostlistBaseViewModel : BindableBase
    {
        public PublisherPostlistBaseViewModel()
        {
            OpenContextMenuCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);
            SelectCommand = new BaseCommand<object>(SelectCanExecute, SelectExecute);
            EditCommand = new BaseCommand<object>(EditPostDetailsCanExecute, EditPostDetailsExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            EditSinglePostCommand = new BaseCommand<object>(EditSinglePostCanExecute, EditSinglePostExecute);
            DeleteSinglePostCommand = new BaseCommand<object>(DeleteSinglePostCanExecute, DeleteSinglePostExecute);
            DuplicateCommand = new BaseCommand<object>(DuplicateCanExecute, DuplicateExecute);
            PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
            ChangePostStatusCommand = new BaseCommand<object>(ChangePostStatusCanExecute, ChangePostStatusExecute);
            RefreshCommand = new BaseCommand<object>(RefreshCanExecute, RefreshExecute);
        }

        #region Properties  

        public ICommand OpenContextMenuCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteSinglePostCommand { get; set; }
        public ICommand EditSinglePostCommand { get; set; }
        public ICommand DuplicateCommand { get; set; }
        public ICommand ChangePostStatusCommand { get; set; }
        public ICommand RefreshCommand { get; set; }

        private CancellationTokenSource TokenSource { get; set; }

        ImmutableQueue<Action> pendingActions = ImmutableQueue<Action>.Empty;
        bool allPostsQueued;

        /// <summary>
        /// To Specify the postlist model
        /// </summary>
        private ObservableCollection<PublisherPostlistModel> _publisherPostlist = new ObservableCollection<PublisherPostlistModel>();

        public ObservableCollection<PublisherPostlistModel> PublisherPostlist
        {
            get
            {
                return _publisherPostlist;
            }
            set
            {
                if (_publisherPostlist == value)
                    return;
                _publisherPostlist = value;
                OnPropertyChanged(nameof(PublisherPostlist));
            }
        }

        private ICollectionView _postCollectionView;

        public ICollectionView PostCollectionView
        {
            get
            {
                return _postCollectionView;
            }
            set
            {
                if (_postCollectionView == value)
                    return;
                _postCollectionView = value;
                OnPropertyChanged(nameof(PostCollectionView));
            }
        }


        private string _campaignId = string.Empty;
        /// <summary>
        /// To specify the campaign Id
        /// </summary>
        public string CampaignId
        {
            get
            {
                return _campaignId;
            }
            set
            {
                if (_campaignId == value)
                    return;
                _campaignId = value;
                OnPropertyChanged(nameof(CampaignId));
            }
        }

        private bool _isSelectAllPostlist;

        public bool IsSelectAllPostList
        {
            get
            {
                return _isSelectAllPostlist;
            }
            set
            {
                if (_isSelectAllPostlist == value)
                    return;
                _isSelectAllPostlist = value;
                OnPropertyChanged(nameof(IsSelectAllPostList));
                // Update the selection status of post list
                SelectAllPostlist(IsSelectAllPostList);
                _isUncheckedFromList = false;
            }
        }
        private bool _isUncheckedFromList { get; set; }

        private bool _isProgressRingActive = true;

        public bool IsProgressRingActive
        {
            get
            {
                return _isProgressRingActive;
            }
            set
            {
                if (_isProgressRingActive == value)
                    return;
                _isProgressRingActive = value;
                OnPropertyChanged(nameof(IsProgressRingActive));
            }
        }

        #endregion

        #region Select
        /// <summary>
        /// To Update the posts selection changes
        /// </summary>
        /// <param name="isSelected">Pass the post current status</param>
        public void SelectAllPostlist(bool isSelected)
        {
            if (_isUncheckedFromList)
                return;
            // Update the post status
            PublisherPostlist.Select(x =>
            {
                x.IsPostlistSelected = isSelected;
                return x;
            }).ToList();
        }

        private bool SelectCanExecute(object sender) => true;

        private void SelectExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                // De-select all posts in postlist
                case "MenuSelectNone":
                    IsSelectAllPostList = false;
                    break;
                // Select all posts in postlist

                case "MenuSelectAll":
                    IsSelectAllPostList = true;
                    break;
                // Validate If all posts are selected then change the header checkbox to selected
                case "SelectManually":
                    if (PublisherPostlist.All(x => x.IsPostlistSelected))
                        IsSelectAllPostList = true;
                    else
                    {
                        if (IsSelectAllPostList)
                            _isUncheckedFromList = true;
                        IsSelectAllPostList = false;
                    }
                    break;
            }
        }

        /// <summary>
        /// Select all posts
        /// </summary>
        public void SelectAllPostlist()
        {
            PublisherPostlist.Select(x =>
            {
                x.IsPostlistSelected = true;
                return x;
            }).ToList();
        }

        /// <summary>
        /// Deselect all posts
        /// </summary>
        public void SelectNonePostlist()
        {
            PublisherPostlist.Select(x =>
            {
                x.IsPostlistSelected = false;
                return x;
            }).ToList();
        }

        #endregion

        #region Duplicate

        private bool DuplicateCanExecute(object sender) => true;

        private void DuplicateExecute(object sender)
        {
            try
            {
                // Validate whether sender from PublisherPostlistModel
                var isSingleDuplicate = sender is PublisherPostlistModel;

                // If its true
                if (isSingleDuplicate)
                {
                    // Get the posts
                    var postlistModel = (PublisherPostlistModel)sender;

                    // Call for getting deep clone
                    var clonedPostModel = GetPostDeepClone(postlistModel);

                    // Generate the post Id
                    clonedPostModel.GenerateClonePostId();

                    // Add to the list
                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            PublisherPostlist.Add(clonedPostModel);
                        });
                    else
                        PublisherPostlist.Add(clonedPostModel);

                    // Update bin file
                    PostlistFileManager.Add(clonedPostModel.CampaignId, clonedPostModel);
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

        }


        /// <summary>
        /// To Get the deep clone of the posts
        /// </summary>
        /// <param name="publisherPostlistModel">Pass the post list model</param>
        /// <returns></returns>
        public PublisherPostlistModel GetPostDeepClone(PublisherPostlistModel publisherPostlistModel)
                 => publisherPostlistModel.DeepClone();

        #endregion

        #region Delete

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {
            // Validate the delete options passed with action name or not
            var isDeleteOptions = sender is string;

            if (!isDeleteOptions)
                return;

            var deleteOptions = (string)sender;
            var campaignId = PublisherPostlist.Select(x => x.CampaignId).FirstOrDefault();

            // Deleting only Post which contains text post
            if (deleteOptions == "MenuDeleteTextPost")
            {
                // Get all Empty posts from collection of current campaign
                var selectedPublisherPostlist = GetEmptyTextPosts();

                if (selectedPublisherPostlist.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "There is no post without an image!");
                    return;
                }
                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "Are you sure to delete post(s) with no images?", "Delete Anyways", "Don't delete");


                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                // Iterate, Delete one by one
                selectedPublisherPostlist.ForEach(x =>
                {
                    PublisherPostlist.Remove(x);
                    PostlistFileManager.Delete(campaignId, y => x.PostId == y.PostId);
                });
            }

            // Delete post's duplicate images
            else if (deleteOptions == "MenuDuplicateImages")
            {
                // Make the Media list as unique file
                PublisherPostlist.ForEach(x =>
                {
                    x.MediaList = new ObservableCollection<string>(x.MediaList.Distinct());
                    x.ImagePointer = 0;
                    x.MediaCurrentPointer = 1;
                    x.TotalMediaCount = x.MediaList.Count;
                    // Update the navigation pointer
                    x.UpdateNavigationPointer();
                });

                // Update the post's bin file for the campaign
                PostlistFileManager.UpdatePostlists(campaignId, PublisherPostlist);
            }
            else
            {
                // Get all selected posts from postlist
                var selectedPublisherPostlist = GetSelectedPosts();

                if (selectedPublisherPostlist.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "Please select atleast a post !!");
                    return;
                }
                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "Are you sure to delete all selected posts permanently?", "Delete Anyways", "Don't delete");

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                // Delete the posts from bin file
                PostlistFileManager.Delete(campaignId, x => selectedPublisherPostlist.FirstOrDefault(a => a.PostId == x.PostId) != null);

                // Remove the publisher posts from binding lists
                selectedPublisherPostlist.ForEach(x =>
                {
                    PublisherPostlist.Remove(x);
                    // Remove the deletion process after post has successfully published
                    GenericFileManager.Delete<PostDeletionModel>(y => x.CampaignId == y.CampaignId && x.PostId == y.PostId, ConstantVariable.GetDeletePublisherPostModel);
                });
            }

            // Update the post counts
            PublisherInitialize.GetInstance.UpdatePostStatus(campaignId);
        }

        private bool DeleteSinglePostCanExecute(object sender) => true;

        private void DeleteSinglePostExecute(object sender)
        {
            // Validate the sender is from PublisherPostlistModel
            var isIndividualDelete = sender is PublisherPostlistModel;

            if (!isIndividualDelete)
                return;

            var campaign = (PublisherPostlistModel)sender;

            var dialogResult = Dialog.ShowCustomDialog(
                "Confirmation", "If you delete it, cant recover back \nAre you sure ?", "Delete Anyways", "Don't delete");

            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            // Delete the posts from bin file
            PostlistFileManager.Delete(campaign.CampaignId, y => campaign.PostId == y.PostId);

            // Remove from deletion process after post has successfully published
            GenericFileManager.Delete<PostDeletionModel>(y => campaign.CampaignId == y.CampaignId && campaign.PostId == y.PostId, ConstantVariable.GetDeletePublisherPostModel);

            // Remove from bin file
            PublisherPostlist.Remove(campaign);

            // Update the post counts
            PublisherInitialize.GetInstance.UpdatePostStatus(campaign.CampaignId);
        }

        /// <summary>
        /// To get all selected posts
        /// </summary>
        /// <returns></returns>
        private List<PublisherPostlistModel> GetSelectedPosts()
            => PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

        /// <summary>
        /// To get the empty text posts
        /// </summary>
        /// <returns></returns>
        private List<PublisherPostlistModel> GetEmptyTextPosts()
            => PublisherPostlist.Where(x => x.MediaList.Count == 0).ToList();

        #endregion

        #region Edit

        private bool EditPostDetailsCanExecute(object sender) => true;

        private void EditPostDetailsExecute(object sender)
        {
            // get the selected posts
            var selectedPost = PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

            if (selectedPost.Count == 0)
            {
                GlobusLogHelper.log.Info("Please select the post before edit!");
                return;
            }

            var dialog = new Dialog();
            // Pass to UI with selected posts
            var publisherUpdateMultiPost = new PublisherUpdateMultiPost(selectedPost);
            var window = dialog.GetMetroWindow(publisherUpdateMultiPost, "Edit post");
            window.ShowDialog();
        }

        private bool EditSinglePostCanExecute(object sender) => true;

        private void EditSinglePostExecute(object sender)
        {
            // Validate the sender is an PublisherPostlistModel ?
            if (sender is PublisherPostlistModel)
            {
                try
                {
                    // cast sender to postlistmodel
                    var currentPost = sender as PublisherPostlistModel;

                    var dialog = new Dialog();
                    // Pass the selected posts to edit UI
                    var publisherEditPost = new PublisherEditPost(currentPost, PublisherPostlist);
                    var window = dialog.GetMetroWindow(publisherEditPost, "Edit Post");
                    window.ShowDialog();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        #endregion

        #region Open Context

        public bool OpenContextMenuCanExecute(object sender) => true;

        public void OpenContextMenuExecute(object sender)
        {
            try
            {
                // Enable the context menu on button left click
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        #endregion

        #region Read Post details

        // public Task<IList<PublisherPostlistModel>> ReadPostList(string campaignId, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)

        private int PostCount { get; set; }

        /// <summary>
        /// To fetch the posts from bin file with specified post queued status
        /// </summary>
        /// <param name="campaignId">Campaign Id from where post are going to fetch</param>
        /// <param name="tokenSource">Cancellation token source</param>
        /// <param name="requiredPostList">Post queued Status</param>
        public void ReadPostList(string campaignId, CancellationTokenSource tokenSource, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)
        {
            // Call to clear already binding posts
            ClearPostlists();
            Thread.Sleep(1000);
            // Assign Cancellation Token
            TokenSource = tokenSource;

            // Validate the campaign Id is null or empty
            if (!string.IsNullOrEmpty(campaignId))
                CampaignId = campaignId;


            PostCount = 0;

            // Get all posts with specified post queued status
            var postItems = PostlistFileManager.GetAll(campaignId).Where(x => x.PostQueuedStatus == requiredPostList).ToList();

            //// Call to clear already binding posts
            //ClearPostlists();

            if (postItems.Count == 0)
            {
                IsProgressRingActive = false;
                return;
            }
            IsProgressRingActive = true;
            PostCount = postItems.Count;

            Thread.Sleep(50);

            allPostsQueued = false;

            #region Call for dynamic process
            // Here whenever action is enqueue to queue, the following process will fetch and invoke the action
            try
            {


                var addPostList = new Task(() =>
                {
                    pendingActions = ImmutableQueue<Action>.Empty;
                    // Add the posts to queue, so that process will run in different work
                    foreach (var post in postItems)
                    {
                        pendingActions = pendingActions.Enqueue(() => AddPostItems(post));
                    }
                    while (!pendingActions.IsEmpty)
                    {
                        try
                        {
                            // Check whether process cancelled or not
                            TokenSource.Token.ThrowIfCancellationRequested();
                            Action perform;
                            Thread.Sleep(1000);
                            // Dequeue and invoke the action
                            pendingActions = pendingActions.Dequeue(out perform);
                            TokenSource.Token.ThrowIfCancellationRequested();
                            perform();
                        }
                        catch (OperationCanceledException ex)
                        {
                            ex.DebugLog();
                            ClearPostlists();
                            break;
                            // throw new OperationCanceledException();
                        }
                        catch (AggregateException ae)
                        {
                            foreach (var e in ae.InnerExceptions)
                            {
                                if (e is TaskCanceledException || e is OperationCanceledException)
                                    e.DebugLog("Cancellation requested before task completion!");
                                else
                                    e.DebugLog(e.StackTrace + e.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }
                    allPostsQueued = true;

                }, TokenSource.Token);
                addPostList.Start();
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion



        }

        /// <summary>
        /// Clear all posts from binded postlists
        /// </summary>
        public void ClearPostlists()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherPostlist.Clear();
                    // Update the collection view
                    //   PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
                });
            }
            else
            {
                // Clear and Update the collection view
                PublisherPostlist.Clear();
                //  PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
            }
        }

        private void AddPostItems(PublisherPostlistModel postItems)
        {
            try
            {
                // Validate whether cancellation is requested or not
                TokenSource.Token.ThrowIfCancellationRequested();

                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Update post items status
                        postItems.InitializePostData();

                        // Get count of tried details
                        var triedCount =
                            postItems.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                        // Get the success rate
                        var successCount =
                            postItems.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                        // Update the tried and success status
                        postItems.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                        // Check whether posts are already present or not
                        if (PublisherPostlist.All(x => x.PostId != postItems.PostId))
                        {
                            TokenSource.Token.ThrowIfCancellationRequested();

                            // If post id is not present, then add into the post
                            PublisherPostlist.Add(postItems);
                            //PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
                        }

                        if (PublisherPostlist.Count == PostCount)
                            IsProgressRingActive = false;
                        Thread.Sleep(10);
                    });
                }
                else
                {
                    // Update post items status
                    postItems.InitializePostData();

                    TokenSource.Token.ThrowIfCancellationRequested();

                    // Add and updating binding soruce
                    PublisherPostlist.Add(postItems);
                    // PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);

                    if (PublisherPostlist.Count == PostCount)
                        IsProgressRingActive = false;
                    Thread.Sleep(10);
                }
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        #region Change Post Status

        private bool ChangePostStatusCanExecute(object sender) => true;

        private void ChangePostStatusExecute(object sender)
        {
            try
            {
                // Get the process name
                var statusToChange = ((Button)sender).Content;

                // Get the post list model
                var publisherPostlistModel = ((FrameworkElement)sender).DataContext as PublisherPostlistModel;

                if (publisherPostlistModel == null)
                    return;

                // Get post list item
                var post = PostlistFileManager.GetByPostId(publisherPostlistModel.CampaignId, publisherPostlistModel.PostId);

                // If action is publish now options
                if (statusToChange.ToString() == (string)Application.Current.FindResource("LangKeyPublishNow"))
                {
                    PublishNow(publisherPostlistModel);
                }

                // Readd operation
                else if (statusToChange.ToString() == (string)Application.Current.FindResource("LangKeyReAdd"))
                {
                    // Get the deep clone of the posts
                    var readdPost = post.DeepClone();

                    // Generate the post id
                    readdPost.GenerateNewPostId();

                    // Update the post queued status into pending
                    readdPost.PostQueuedStatus = PostQueuedStatus.Pending;
                    readdPost.LstPublishedPostDetailsModels = new ObservableCollection<PublishedPostDetailsModel>();

                    // Get the general settings
                    var generalModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                               (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                               .FirstOrDefault(x => x.CampaignId == readdPost.CampaignId) ?? new GeneralModel();

                    // Check whether need to keep the initial create time for the posts
                    if (!generalModel.IsKeepPostsInitialCreationDate)
                        readdPost.CreatedTime = DateTime.Now;

                    // Add to post's bin file
                    PostlistFileManager.Add(readdPost.CampaignId, readdPost);

                    // Update the post status
                    PublisherInitialize.GetInstance.UpdatePostStatus(publisherPostlistModel.CampaignId);
                }
                // Switch to pending 
                else if (statusToChange.ToString() == (string)Application.Current.FindResource("LangKeySendToPending"))
                {
                    // Update the post's queued status to pending 
                    publisherPostlistModel.PostQueuedStatus = PostQueuedStatus.Pending;
                    post.PostQueuedStatus = PostQueuedStatus.Pending;
                    // Update the status
                    PostProcessOfStatusChange(publisherPostlistModel, post);
                }

                // Switch to draft list
                else if (statusToChange.ToString() == (string)Application.Current.FindResource("LangKeySendToDraft"))
                {
                    // Update post queue to draft
                    publisherPostlistModel.PostQueuedStatus = PostQueuedStatus.Draft;
                    post.PostQueuedStatus = PostQueuedStatus.Draft;

                    // Update the bin file
                    PostProcessOfStatusChange(publisherPostlistModel, post);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void PostProcessOfStatusChange(PublisherPostlistModel campaignStatus, PublisherPostlistModel post)
        {
            try
            {
                if (!Application.Current.CheckAccess())
                {
                    Application.Current.Invoke(() =>
                    {
                        // Update the post 
                        PostlistFileManager.UpdatePost(campaignStatus.CampaignId, post);
                        // Update the campaigns post count
                        PublisherInitialize.GetInstance.UpdatePostStatus(campaignStatus.CampaignId);
                        PublisherPostlist.Remove(campaignStatus);
                    });
                }
                else
                {
                    // Update the post 
                    PostlistFileManager.UpdatePost(campaignStatus.CampaignId, post);
                    // Update the campaigns post count
                    PublisherInitialize.GetInstance.UpdatePostStatus(campaignStatus.CampaignId);
                    PublisherPostlist.Remove(campaignStatus);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void PublishNow(PublisherPostlistModel postlistModel)
        {
            if (postlistModel != null)
            {
                try
                {
                    ThreadFactory.Instance.Start(() =>
                    {
                        // Call to publish the post immediately without checking the time interval
                        PublishScheduler.StartPublishingPosts(postlistModel);
                    });
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            e.DebugLog("Cancellation requested before task completion!");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        #endregion

        #region Refresh 

        public bool RefreshCanExecute(object sender) => true;

        public void RefreshExecute(object sender)
        {
            // Stop already running posts
            var cancellationToken = PostLoadingCancellation();
            try
            {
                var queuestatus = (PostQueuedStatus)sender;
                // Call to Refresh the posts, here we need to pass the campaign Id , Cancellation token and respective queued status
                ThreadFactory.Instance.Start(() => ReadPostList(CampaignId, cancellationToken, queuestatus), cancellationToken.Token);
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private CancellationTokenSource PostLoadingCancellation()
        {
            // Call to cancel the running task
            CancelRunningTask();

            // Get the new cancellation token and enqueue to queue
            var cancellationToken = new CancellationTokenSource();
            QueueCancellationTokenSources.Enqueue(cancellationToken);

            // return the cancelltion token for passing along with thread
            return cancellationToken;
        }

        public Queue<CancellationTokenSource> QueueCancellationTokenSources { get; set; }
            = new Queue<CancellationTokenSource>();

        public void CancelRunningTask()
        {
            // Dequeue one by one and make cancel the process
            while (QueueCancellationTokenSources.Count > 0)
                QueueCancellationTokenSources.Dequeue().Cancel();
        }

        #endregion

    }
}