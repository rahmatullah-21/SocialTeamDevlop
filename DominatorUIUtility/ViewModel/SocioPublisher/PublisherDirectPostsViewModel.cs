using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel : BindableBase
    {

        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
            SearchCommand = new BaseCommand<object>(SearchCanExecute, SearchExecute);
          //  PostDetailsModel.MediaViewer.MediaList.Add(@"C:\Users\Public\Pictures\Sample Pictures\1.jpg");            
        }

       

        #endregion

        #region Properties

        public ICommand MultiplePostCommand { get; set; }
        public ICommand SearchCommand { get; set; }

        private PostDetailsModel _postDetailsModel = new PostDetailsModel();

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
           
        }
        #endregion

    }


}