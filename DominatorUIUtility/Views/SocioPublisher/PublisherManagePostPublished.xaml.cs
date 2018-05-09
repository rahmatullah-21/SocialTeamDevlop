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
    /// Interaction logic for PublisherManagePostPublished.xaml
    /// </summary>
    public partial class PublisherManagePostPublished : UserControl
    {
        private PublisherManagePostPublished()
        {
            InitializeComponent();
        }

        private static PublisherManagePostPublished _instance;

        public static PublisherManagePostPublished Instance { get; set; }
            = _instance ?? (_instance = new PublisherManagePostPublished());

    }
}
