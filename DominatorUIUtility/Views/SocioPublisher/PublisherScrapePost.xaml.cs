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
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherScrapePost.xaml
    /// </summary>
    public partial class PublisherScrapePost : UserControl
    {
        public PublisherScrapePost()
        {
            InitializeComponent();
        }

        public PublisherScrapePost(ScrapePostModel ScrapePostModel):this()
        {
            MainGrid.DataContext = ScrapePostModel;
        }

    }
}
