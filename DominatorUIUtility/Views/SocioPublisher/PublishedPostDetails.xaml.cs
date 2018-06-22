using System;
using System.Collections.Generic;
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
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublishedPostDetails.xaml
    /// </summary>
    public partial class PublishedPostDetails : UserControl
    {
        private PublisherPostlistModel currentData;
        private string postId;

        public PublishedPostDetails()
        {
            InitializeComponent();
        }

        public PublishedPostDetails(PublisherPostlistModel currentData):this()
        {
            this.currentData = currentData;
            var data=PostlistFileManager.GetByPostId(currentData.CampaignId, currentData.PostId);
            this.DataContext = data;
        }

    }
}
