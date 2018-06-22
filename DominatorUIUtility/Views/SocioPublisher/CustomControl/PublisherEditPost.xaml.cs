using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    /// Interaction logic for PublisherEditPost.xaml
    /// </summary>
    public partial class PublisherEditPost : UserControl
    {
        private PublisherPostlistModel PostlistModel { get; set; }
        private ObservableCollection<PublisherPostlistModel> LstPostListModel { get; set; }
        public PublisherEditPost()
        {
            InitializeComponent();
        }

        public PublisherEditPost(PublisherPostlistModel publisherPostlistModel, ObservableCollection<PublisherPostlistModel> LstPostListModel) : this()
        {
            this.LstPostListModel = LstPostListModel;
            PostlistModel = publisherPostlistModel.DeepClone();
            this.DataContext = PostlistModel;

        }

        private void PublisherEditPost_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                PostContentControl.SetMedia();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var indexToUpdate = LstPostListModel.FindIndex(posts => posts.PostId == PostlistModel.PostId);
                LstPostListModel[indexToUpdate] = PostlistModel;
                PostlistFileManager.UpdatePostlists(PostlistModel.CampaignId, LstPostListModel);
               
            }
            catch (Exception ex)
            {
               ex.DebugLog();
            }
            Dialog.CloseDialog(sender);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Dialog.CloseDialog(sender);
        }


    }
}
