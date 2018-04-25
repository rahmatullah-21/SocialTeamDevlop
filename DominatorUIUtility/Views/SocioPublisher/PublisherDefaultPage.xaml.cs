using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherDefaultPage.xaml
    /// </summary>
    public partial class PublisherDefaultPage : UserControl
    {
        private PublisherDefaultPage()
        {
            InitializeComponent();
            PublisherDefault.DataContext = PublisherDefaultViewModel;
        }

        public PublisherDefaultViewModel PublisherDefaultViewModel { get; set; } = new PublisherDefaultViewModel();

        private static PublisherDefaultPage _indexPage;

        public static PublisherDefaultPage Instance { get; set; }
            = _indexPage ?? (_indexPage = new PublisherDefaultPage());

    }

    
}
