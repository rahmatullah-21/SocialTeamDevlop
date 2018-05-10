using System.Windows.Controls;
using DominatorUIUtility.ViewModel.SocioPublisher;

namespace DominatorUIUtility.Views.SocioPublisher
{
    /// <summary>
    /// Interaction logic for PublisherManagePostDrafts.xaml
    /// </summary>
    public partial class PublisherManagePostDrafts : UserControl
    {
        private PublisherManagePostDrafts()
        {
            InitializeComponent();
            DraftPostList.DataContext = PublisherManagePostDraftsViewModel;
        }

        public PublisherManagePostDraftsViewModel PublisherManagePostDraftsViewModel { get; set; } =
            new PublisherManagePostDraftsViewModel();

        private static PublisherManagePostDrafts _instance;

        public static PublisherManagePostDrafts Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePostDrafts());

    }
}
