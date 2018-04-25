using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePosts.xaml
    /// </summary>
    public partial class PublisherManagePosts : UserControl
    {
        public PublisherManagePosts()
        {
            InitializeComponent();
            ManagePost.DataContext = PublisherManagePostsViewModel;
        }

        public PublisherManagePostsViewModel PublisherManagePostsViewModel { get; set; } =
            new PublisherManagePostsViewModel();
    }
}
