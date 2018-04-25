using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherCreateDestination.xaml
    /// </summary>
    public partial class PublisherCreateDestination : UserControl
    {
        public PublisherCreateDestination()
        {
            InitializeComponent();
            CreateDestination.DataContext = PublisherCreateDestinationsViewModel;
        }

        public PublisherCreateDestinationsViewModel PublisherCreateDestinationsViewModel { get; set; } =
            new PublisherCreateDestinationsViewModel();
    }
}
