using System.Windows.Controls;
using LegionUIUtility.ViewModel.SocioPublisher;

namespace LegionUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherHome.xaml
    /// </summary>
    public partial class PublisherHome : UserControl 
    {
        private PublisherHome()
        {
            InitializeComponent();
            PublisherHomeViewModel.SetDefaultHomePage();
            UserControlPublisherHome.DataContext = PublisherHomeViewModel.PublisherHomeModel;
        }

        private static PublisherHome _indexPage;

        public static PublisherHome Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherHome());

        public PublisherHomeViewModel PublisherHomeViewModel { get; set; } = new PublisherHomeViewModel();

    }
}
