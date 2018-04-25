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
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManageDestinations.xaml
    /// </summary>
    public partial class PublisherManageDestinations : UserControl
    {
        private PublisherManageDestinations()
        {
            InitializeComponent();
            ManageDestination.DataContext = PublisherManageDestinationViewModel;
        }

        public PublisherManageDestinationViewModel PublisherManageDestinationViewModel { get; set; } =
            new PublisherManageDestinationViewModel();

        private static PublisherManageDestinations _indexPage;

        public static PublisherManageDestinations Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherManageDestinations());
    }
}
