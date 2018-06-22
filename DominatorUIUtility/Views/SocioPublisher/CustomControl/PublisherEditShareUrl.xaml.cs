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

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    /// Interaction logic for PublisherEditSourceUrl.xaml
    /// </summary>
    public partial class PublisherEditShareUrl : UserControl
    {
        private PublisherPostlistModel currentPostData;
        private ObservableCollection<PublisherPostlistModel> lstPublisherPostlist;

        public PublisherEditShareUrl()
        {
            InitializeComponent();
        }

        public PublisherEditShareUrl(PublisherPostlistModel currentPost, ObservableCollection<PublisherPostlistModel> lstPublisherPostlist):this()
        {
            currentPostData = currentPost.DeepClone();
            this.lstPublisherPostlist = lstPublisherPostlist;
            this.DataContext = currentPostData;
        }

        private void PublisherEditSourceUrl_OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                MediaViewerControl.Initialize();
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
                var indexToUpdate = lstPublisherPostlist.FindIndex(posts => posts.PostId == currentPostData.PostId);
                lstPublisherPostlist[indexToUpdate] = currentPostData;
                PostlistFileManager.UpdatePostlists(currentPostData.CampaignId, lstPublisherPostlist);
               
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
