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
using DominatorUIUtility.Views.SocioPublisher.CustomControl;

namespace DominatorUIUtility.Views.SocioPublisher.Suggestions
{
    /// <summary>
    /// Interaction logic for PublisherEditPost.xaml
    /// </summary>
    public partial class PublisherEditPost : UserControl
    {
        public PublisherEditPost()
        {
            InitializeComponent();
        }

        public PublisherEditPost(PublisherPostlistModel publisherPostlistModel):this()
        {
            EditPost.DataContext = publisherPostlistModel;
        }

     
    }
}
