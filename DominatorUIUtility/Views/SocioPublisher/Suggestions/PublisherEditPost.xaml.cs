using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Views.SocioPublisher.CustomControl;
using MahApps.Metro.Controls;

namespace DominatorUIUtility.Views.SocioPublisher.Suggestions
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
            var indexToUpdate = LstPostListModel.FindIndex(posts => posts.PostId == PostlistModel.PostId);
            LstPostListModel[indexToUpdate] = PostlistModel;
            PostlistFileManager.UpdatePostlists(PostlistModel.CampaignId, LstPostListModel);
            Dialog.CloseDialog(sender);
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Dialog.CloseDialog(sender);
        }


    }
}
