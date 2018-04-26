using System.Windows.Controls;
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
