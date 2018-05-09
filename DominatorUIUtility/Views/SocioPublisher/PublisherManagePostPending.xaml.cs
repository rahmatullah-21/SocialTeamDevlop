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
        }

        private static PublisherManagePostPending _instance;

        public static PublisherManagePostPending Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePostPending());

    }
}
