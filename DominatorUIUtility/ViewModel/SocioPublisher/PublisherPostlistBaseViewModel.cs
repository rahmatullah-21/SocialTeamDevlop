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
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

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

            PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
        }

        #region Properties  

        public ICommand OpenContextMenuCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand EditCommand { get; set; }
        public ICommand SettingsCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand DeleteSinglePostCommand { get; set; }
        public ICommand EditSinglePostCommand { get; set; }

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
            }
        }


        #endregion

        #region Select

        private bool SelectCanExecute(object sender) => true;

        private void SelectExecute(object sender)
        {

        }

        #endregion

        #region Delete

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {

        }

        private bool DeleteSinglePostCanExecute(object sender) => true;

        private void DeleteSinglePostExecute(object sender)
        {

        }


        #endregion

        #region Edit

        private bool EditPostDetailsCanExecute(object sender) => true;

        private void EditPostDetailsExecute(object sender)
        {

        }

        private bool EditSinglePostCanExecute(object sender) => true;

        private void EditSinglePostExecute(object sender)
        {

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

        public void ReadPostList(string campaignId, CancellationTokenSource tokenSource, PostQueuedStatus requiredPostList = PostQueuedStatus.Draft)
        {
            if (string.IsNullOrEmpty(campaignId))
                CampaignId = campaignId;

            TokenSource = tokenSource;

            var postItems = new ObservableCollection<PublisherPostlistModel>();

            #region InitializeData

            try
            {
                for (int i = 0; i < 1; i++)
                {
                    var postlist = new PublisherPostlistModel
                    {
                        PostDescription = "Descriptions",
                        Title = "Title",
                        MediaList = new ObservableCollection<string>(),
                        CreatedTime = DateTime.Now,
                        PostSource = PostSource.NormalPost,
                        PostCategory = PostCategory.OrdinaryPost,
                        CampaignId = "campaignId",
                        SellPostPrice = 100,
                        SellPostAvailableLocation = "India",
                        PostMacro = new Dictionary<string, string>(),
                        PostRunningStatus = PostRunningStatus.Active,
                        PostQueuedStatus = PostQueuedStatus.Draft,
                        ExpiredTime = DateTime.Now.AddDays(7)
                    };
                    postlist.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\1.jpg");
                    postlist.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");                
                    postItems.Add(postlist);

                    var postlist1 = new PublisherPostlistModel
                    {
                        PostDescription = "Descriptions",
                        Title = "Title",
                        MediaList = new ObservableCollection<string>(),
                        CreatedTime = DateTime.Now,
                        PostSource = PostSource.NormalPost,
                        PostCategory = PostCategory.OrdinaryPost,
                        CampaignId = "campaignId",
                        SellPostPrice = 100,
                        SellPostAvailableLocation = "India",
                        PostMacro = new Dictionary<string, string>(),
                        PostRunningStatus = PostRunningStatus.Active,
                        PostQueuedStatus = PostQueuedStatus.Published,
                        ExpiredTime = DateTime.Now.AddDays(7)
                    };
                    postlist1.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\1.jpg");
                    postlist1.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");                    
                    postItems.Add(postlist1);

                    var postlist2 = new PublisherPostlistModel
                    {
                        PostDescription = "Descriptions",
                        Title = "Title",
                        MediaList = new ObservableCollection<string>(),
                        CreatedTime = DateTime.Now,
                        PostSource = PostSource.NormalPost,
                        PostCategory = PostCategory.OrdinaryPost,
                        CampaignId = "campaignId",
                        SellPostPrice = 100,
                        SellPostAvailableLocation = "India",
                        PostMacro = new Dictionary<string, string>(),
                        PostRunningStatus = PostRunningStatus.Active,
                        PostQueuedStatus = PostQueuedStatus.Pending,
                        ExpiredTime = DateTime.Now.AddDays(7)
                    };
                    postlist2.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\1.jpg");
                    postlist2.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\2.jpg");                   
                    postItems.Add(postlist2);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            #endregion

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
                                throw new OperationCanceledException();
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
                });
            }
            else
            {
                PublisherPostlist.Clear();
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
                        PublisherPostlist.Add(postItems);
                        PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
                    });
                }
                else
                {
                    PublisherPostlist.Add(postItems);
                    PostCollectionView = CollectionViewSource.GetDefaultView(PublisherPostlist);
                }
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

    }
}