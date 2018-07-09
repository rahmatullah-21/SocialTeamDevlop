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
using DominatorHouseCore.Utility;

namespace DominatorUIUtility.Views.SocioPublisher.CustomControl
{
    /// <summary>
    /// Interaction logic for PublisherUpdateMultiPost.xaml
    /// </summary>
    public partial class PublisherUpdateMultiPost : UserControl
    {
        public ObservableCollection<PublisherPostlistModel> LstPostDetail { get; set; }

        public PublisherUpdateMultiPost()
        {
            InitializeComponent();
        }

        public PublisherUpdateMultiPost(List<PublisherPostlistModel> selectedPost) : this()
        {
            LstPostDetail = new ObservableCollection<PublisherPostlistModel>(selectedPost);
            MainGrid.DataContext = this;
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                PostlistFileManager.UpdatePostlists(LstPostDetail.FirstOrDefault().CampaignId, LstPostDetail);
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
