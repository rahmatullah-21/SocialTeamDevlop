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
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;
using DominatorUIUtility.Views.SocioPublisher.Suggestions;
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
            SettingsCommand = new BaseCommand<object>(SettingsCanExecute, SettingsExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            EditSinglePostCommand = new BaseCommand<object>(EditSinglePostCanExecute, EditSinglePostExecute);
            DeleteSinglePostCommand = new BaseCommand<object>(DeleteSinglePostCanExecute, DeleteSinglePostExecute);
            DuplicateCommand = new BaseCommand<object>(DuplicateCanExecute, DuplicateExecute);
            PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
            ChangePostStatusCommand = new BaseCommand<object>(ChangePostStatusCanExecute, ChangePostStatusExecute);

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
        private CancellationTokenSource TokenSource { get; set; }

        ImmutableQueue<Action> pendingActions = ImmutableQueue<Action>.Empty;
        bool allPostsQueued;

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

                if (_isSelectAllPostlist)
                    SelectAllPostlist();
                else
                    SelectNonePostlist();
            }
        }


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

        private bool SelectCanExecute(object sender) => true;

        private void SelectExecute(object sender)
        {
            var moduleName = sender.ToString();
            switch (moduleName)
            {
                case "MenuSelectNone":
                    IsSelectAllPostList = false;
                    break;

                case "MenuSelectAll":
                    IsSelectAllPostList = true;
                    break;
            }
        }

        public void SelectAllPostlist()
        {
            PublisherPostlist.Select(x =>
            {
                x.IsPostlistSelected = true;
                return x;
            }).ToList();
        }

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
                var isSingleDuplicate = sender is PublisherPostlistModel;

                if (isSingleDuplicate)
                {
                    var postlistModel = (PublisherPostlistModel)sender;

                    var clonedPostModel = GetPostDeepClone(postlistModel);

                    clonedPostModel.GenerateClonePostId();


                    if (!Application.Current.Dispatcher.CheckAccess())
                        Application.Current.Dispatcher.Invoke(delegate
                        {
                            PublisherPostlist.Add(clonedPostModel);
                        });
                    else
                        PublisherPostlist.Add(clonedPostModel);

                    PostlistFileManager.Add(clonedPostModel.CampaignId, clonedPostModel);
                }
            }
            catch (Exception e)
            {
                e.DebugLog();
            }

        }
        public PublisherPostlistModel GetPostDeepClone(PublisherPostlistModel publisherPostlistModel)
                 => publisherPostlistModel.DeepClone();

        #endregion

        #region Delete

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {
            var isDeleteOptions = sender is string;

            if (!isDeleteOptions)
                return;

            var deleteOptions = (string)sender;
            var campaignId = PublisherPostlist.Select(x => x.CampaignId).FirstOrDefault();
            if (deleteOptions == "MenuDeleteTextPost")
            {
                var selectedPublisherPostlist = GetEmptyTextPosts();

                if (selectedPublisherPostlist.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "There is no post without an image!");
                    return;
                }
                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "Are you sure to delete post(s) with no images?", "Delete Anyways", "Don't delete");


                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                selectedPublisherPostlist.ForEach(x => PublisherPostlist.Remove(x));
            }
            else if (deleteOptions == "MenuDuplicateImages")
            {
                PublisherPostlist.ForEach(x =>
                {
                    x.MediaList = new ObservableCollection<string>(x.MediaList.Distinct());
                    x.ImagePointer = 0;
                    x.MediaCurrentPointer = 1;
                    x.TotalMediaCount = x.MediaList.Count;
                    x.UpdateNavigationPointer();
                });
                PostlistFileManager.UpdatePostlists(campaignId, PublisherPostlist);
            }
            else
            {
                var selectedPublisherPostlist = GetSelectedPosts();

                if (selectedPublisherPostlist.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "Please select atleast a post !!");
                    return;
                }
                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "Are you sure to delete all selected posts permanently?", "Delete Anyways", "Don't delete");

                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                PostlistFileManager.Delete(campaignId, x => selectedPublisherPostlist.FirstOrDefault(a => a.PostId == x.PostId) != null);
                selectedPublisherPostlist.ForEach(x => PublisherPostlist.Remove(x));
            }
        }

        private bool DeleteSinglePostCanExecute(object sender) => true;

        private void DeleteSinglePostExecute(object sender)
        {
            var isIndividualDelete = sender is PublisherPostlistModel;

            if (!isIndividualDelete)
                return;

            var campaign = (PublisherPostlistModel)sender;

            var dialogResult = Dialog.ShowCustomDialog(
                "Confirmation", "If you delete it, cant recover back \nAre you sure ?", "Delete Anyways", "Don't delete");

            if (dialogResult != MessageDialogResult.Affirmative)
                return;

            PostlistFileManager.Delete(campaign.CampaignId, y => campaign.PostId == y.PostId);

            PublisherPostlist.Remove(campaign);
        }

        private List<PublisherPostlistModel> GetSelectedPosts()
            => PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

        private List<PublisherPostlistModel> GetEmptyTextPosts()
            => PublisherPostlist.Where(x => x.MediaList.Count == 0).ToList();

        #endregion

        #region Edit

        private bool EditPostDetailsCanExecute(object sender) => true;

        private void EditPostDetailsExecute(object sender)
        {
            var selectedPost = PublisherPostlist.Where(x => x.IsPostlistSelected).ToList();

            Dialog dialog = new Dialog();
            PublisherUpdateMultiPost PublisherUpdateMultiPost = new PublisherUpdateMultiPost(selectedPost);
            var window = dialog.GetMetroWindow(PublisherUpdateMultiPost, "Edit post");
            window.ShowDialog();
        }

        private bool EditSinglePostCanExecute(object sender) => true;

        private void EditSinglePostExecute(object sender)
        {
            if (sender is PublisherPostlistModel)
            {
                try
                {
                    var currentPost = sender as PublisherPostlistModel;
                    Dialog dialog = new Dialog();
                    if (!string.IsNullOrEmpty(currentPost.ShareUrl))
                    {
                        PublisherEditShareUrl publisherEditShareUrl = new PublisherEditShareUrl(currentPost, PublisherPostlist);
                        var window = dialog.GetMetroWindow(publisherEditShareUrl, "Edit Share Url");
                        window.ShowDialog();
                    }
                    else
                    {
                        PublisherEditPost publisherEditPost = new PublisherEditPost(currentPost, PublisherPostlist);
                        var window = dialog.GetMetroWindow(publisherEditPost, "Edit Post");
                        window.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        #endregion

        #region Settings

        private bool SettingsCanExecute(object sender) => true;

        private void SettingsExecute(object sender)
            => OpenPostlistSettings(CampaignId);

        #endregion

        #region Open Context

        public bool OpenContextMenuCanExecute(object sender) => true;

        public void OpenContextMenuExecute(object sender)
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


        #endregion

        #region Open Settings

        public void OpenPostlistSettings(string campaignId)
        {
            var publisherPostlistSettingsModel = PostListSettingsFileManager.GetSettingsByCampaignId(campaignId) ?? new PublisherPostlistSettingsModel();

            publisherPostlistSettingsModel.CampaignId = publisherPostlistSettingsModel.CampaignId ?? campaignId;

            var objPublisherPostlistSettings = new PublisherPostlistSettings(publisherPostlistSettingsModel);

            var customDialog = new CustomDialog
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objPublisherPostlistSettings
            };

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog, "Postlist Settings");

            objPublisherPostlistSettings.ButtonSave.Click += (senders, events) =>
            {
                try
                {
                    objPublisherPostlistSettings.PublisherPostlistSettingsModel.AddOrUpdateBinFile
                        (objPublisherPostlistSettings.PublisherPostlistSettingsModel);

                    dialogWindow.Close();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            };

            objPublisherPostlistSettings.ButtonCancel.Click += (senders, events) => dialogWindow.Close();

            dialogWindow.ShowDialog();
        }



        #endregion

        #region Read Post details

        // public Task<IList<PublisherPostlistModel>> ReadPostList(string campaignId, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)

        private int PostCount { get; set; }


        public void ReadPostList(string campaignId, CancellationTokenSource tokenSource, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)
        {
            if (string.IsNullOrEmpty(campaignId))
                CampaignId = campaignId;

            TokenSource = tokenSource;

            PostCount = 0;

            var postItems = PostlistFileManager.GetAll(campaignId).Where(x => x.PostQueuedStatus == requiredPostList).ToList();

            ClearPostlists();
           
            if (postItems.Count==0)
            {
                IsProgressRingActive = false;
                return;
            }

            PostCount = postItems.Count;

            Thread.Sleep(50);

            allPostsQueued = false;

            try
            {
                var addPostList = new Task(() =>
                {
                    while (!allPostsQueued)
                    {
                        Thread.Sleep(50);
                        while (!pendingActions.IsEmpty)
                        {
                            try
                            {
                                TokenSource.Token.ThrowIfCancellationRequested();
                                Action perform;
                                pendingActions = pendingActions.Dequeue(out perform);
                                perform();
                            }
                            catch (OperationCanceledException ex)
                            {
                                ex.DebugLog();
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
                    }
                }, tokenSource.Token);
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

            Thread.Sleep(50);

            foreach (var post in postItems)
            {
                pendingActions = pendingActions.Enqueue(() => AddPostItems(post));
            }
        }

        public void ClearPostlists()
        {
            if (!Application.Current.Dispatcher.CheckAccess())
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    PublisherPostlist.Clear();
                    PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
                });
            }
            else
            {
                PublisherPostlist.Clear();
                PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
            }
        }

        private void AddPostItems(PublisherPostlistModel postItems)
        {
            try
            {
                TokenSource.Token.ThrowIfCancellationRequested();

                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        postItems.InitializePostData();
                        PublisherPostlist.Add(postItems);
                        PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);

                        if (PublisherPostlist.Count == PostCount)
                            IsProgressRingActive = false;
                        Thread.Sleep(10);
                    });
                }
                else
                {
                    postItems.InitializePostData();
                    PublisherPostlist.Add(postItems);
                    PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);

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

        #region Image Navigation

        //public void PreviousImage(object sender)
        //{
        //    var publisherPostlistModel = ((FrameworkElement)sender).DataContext as PublisherPostlistModel;

        //    if (publisherPostlistModel == null)
        //        return;

        //    publisherPostlistModel.ImagePointer--;
        //    publisherPostlistModel.CurrentMediaUrl = publisherPostlistModel.MediaList[publisherPostlistModel.ImagePointer];
        //    publisherPostlistModel.MediaCurrentPointer = publisherPostlistModel.MediaCurrentPointer - 1;
        //    publisherPostlistModel.UpdateNavigationPointer();
        //}


        //public void NextImage(object sender)
        //{
        //    var publisherPostlistModel = ((FrameworkElement)sender).DataContext as PublisherPostlistModel;

        //    if (publisherPostlistModel == null)
        //        return;

        //    publisherPostlistModel.ImagePointer++;
        //    publisherPostlistModel.CurrentMediaUrl = publisherPostlistModel.MediaList[publisherPostlistModel.ImagePointer];
        //    publisherPostlistModel.MediaCurrentPointer = publisherPostlistModel.MediaCurrentPointer + 1;
        //    publisherPostlistModel.UpdateNavigationPointer();
        //}

        #endregion

        private bool ChangePostStatusCanExecute(object sender) => true;

        private void ChangePostStatusExecute(object sender)
        {
            var statusToChange = ((Button)sender).Content;
            var campaignStatus = ((FrameworkElement)sender).DataContext as PublisherPostlistModel;

            switch (statusToChange.ToString())
            {
                case "Publish Now":
                    campaignStatus.PostQueuedStatus = PostQueuedStatus.Published;
                    break;
                case "Send to Pending":
                case "Re Add":
                    campaignStatus.PostQueuedStatus = PostQueuedStatus.Pending;
                    break;
                case "Send to Draft":
                    campaignStatus.PostQueuedStatus = PostQueuedStatus.Draft;
                    break;
            }

            PostlistFileManager.UpdatePostlists(campaignStatus.CampaignId, PublisherPostlist);
            PublisherPostlist.Remove(campaignStatus);


        }
    }
}