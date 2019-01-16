using DominatorUIUtility.ViewModel;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager : UserControl
    {
        public ProxyManager(IProxyManagerViewModel proxyManagerViewModel)
        {

            InitializeComponent();
            MainGrid.DataContext = proxyManagerViewModel;
        }

        private void ProxyDataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = true;
        }
    }
}
