using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel.SocioPublisher
{
    public class PublisherDirectPostsViewModel
    {

        #region Constructor

        public PublisherDirectPostsViewModel()
        {
            MultiplePostCommand = new BaseCommand<object>(CanExecuteMultiPost, ExecuteMultiPost);
        }

        #endregion

        #region Properties

        public ICommand MultiplePostCommand { get; set; }

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
        #endregion

    }
}