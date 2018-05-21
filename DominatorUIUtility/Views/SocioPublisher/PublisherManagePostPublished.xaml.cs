using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPublished.xaml
    /// </summary>
    public partial class PublisherManagePostPublished : UserControl
    {
        private PublisherManagePostPublished()
        {
            InitializeComponent();
            PublishedPostList.DataContext = PublisherManagePostPublishedViewModel;
        }

        public PublisherManagePostPublishedViewModel PublisherManagePostPublishedViewModel { get; set; } =
            new PublisherManagePostPublishedViewModel();

        private static PublisherManagePostPublished _instance;

        public static PublisherManagePostPublished Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePostPublished());

    }
}
