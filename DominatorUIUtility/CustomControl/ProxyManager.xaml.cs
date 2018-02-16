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
using DominatorUIUtility.Behaviours;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager : UserControl
    {
        public ProxyManager()
        {
            InitializeComponent();
        }

        private void btnAddProxy_Click(object sender, RoutedEventArgs e)
        {
            AddOrUpdateProxyControl ObjAddProxyControl = new AddOrUpdateProxyControl();
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(ObjAddProxyControl, FindResource("langAddProxy").ToString());
            window.Show();
        }
    }
}
