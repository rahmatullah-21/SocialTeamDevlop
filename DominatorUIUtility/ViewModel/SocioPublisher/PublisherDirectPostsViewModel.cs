using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel : BindableBase
    {
        private PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl;
        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
            ImportFromCsvCommand = new BaseCommand<object>(ImportFromCsvCanExecute, ImportFromCsvExecute);
            SearchCommand = new BaseCommand<object>(SearchCanExecute, SearchExecute);           
        }

      

        public PublisherDirectPostsViewModel(PublisherCreateCampaignViewModel.TabItemsControl tabItemsControl):this()
        {
            this.tabItemsControl = tabItemsControl;
            _postDetailsModel = tabItemsControl.PostDetailsModel;
        }

        #endregion

        #region Properties

        public ICommand MultiplePostCommand { get; set; }
        public ICommand ImportFromCsvCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        private PostDetailsModel _postDetailsModel =new PostDetailsModel();

        public PostDetailsModel PostDetailsModel
        {
            get
            {
                return _postDetailsModel;
            }
            set
            {
                if(_postDetailsModel == value)
                    return;
                _postDetailsModel = value;
                OnPropertyChanged(nameof(PostDetailsModel));
            }
        }


        private bool _isBool= true;

        public bool IsBool
        {
            get
            {
                return _isBool;
            }
            set
            {
                if(IsBool == value)
                    return;
                _isBool = value;
                OnPropertyChanged(nameof(IsBool));
            }
        }


        private PublisherMediaViewerModel _publisherMultipleImagesMediaViewerModel = new PublisherMediaViewerModel();

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


        #endregion

        #region Methods

        public bool CanExecuteMultiPost(object sender) => true;

        public void ExecuteMultiPost(object sender)
        {
            try
            {
                var publisherMultiplePost = new PublisherMultiplePost
                    {                       
                        ShowInTaskbar = true,
                        ShowActivated = true,
                        Topmost = false,
                        ResizeMode = ResizeMode.NoResize,
                        WindowStyle = WindowStyle.SingleBorderWindow,
                        WindowStartupLocation = WindowStartupLocation.CenterScreen,
                        ShowTitleBar = true,
                        ShowCloseButton = true,
                        WindowTransitionsEnabled = false,                      
                        BorderThickness = new Thickness(0),
                        GlowBrush = Brushes.Black,
                };
                publisherMultiplePost.Show();

                //var customDialog = new CustomDialog
                //{
                //    HorizontalAlignment = HorizontalAlignment.Center,
                //    Content = publisherMultiplePost
                //};
                //var objDialog = new Dialog();
                //var dialogWindow = objDialog.GetCustomDialog(customDialog, "Multiple Postlist");
                //dialogWindow.ShowDialog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SearchCanExecute(object sender) => true;

        private void SearchExecute(object sender)
        {

            var mediaViewer = (MediaViewer) sender;

            mediaViewer.MediaList =
               new ObservableCollection<string>(ImageExtracter.ExtractImageUrls(PostDetailsModel.ImagesUrl));

            mediaViewer?.Initialize();

        }
        private bool ImportFromCsvCanExecute(object sender) => true;

        private void ImportFromCsvExecute(object sender)
        {
            var loadedAccountlist = FileUtilities.FileBrowseAndReader();

        }
        #endregion

    }


}