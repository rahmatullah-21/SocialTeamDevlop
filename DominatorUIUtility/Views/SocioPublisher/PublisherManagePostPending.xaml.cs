using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostPending.xaml
    /// </summary>
    public partial class PublisherManagePostPending : UserControl
    {
        private PublisherManagePostPending()
        {
            InitializeComponent();
            PendingPostLists.DataContext = PublisherManagePostPendingViewModel;
        }

        public PublisherManagePostPendingViewModel PublisherManagePostPendingViewModel { get; set; } =
            new PublisherManagePostPendingViewModel();

        private static PublisherManagePostPending _instance;

        public static PublisherManagePostPending Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePostPending());

    }
}
