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
