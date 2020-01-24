using LegionUIUtility.ViewModel;
using System.Windows.Controls;

namespace LegionUIUtility.CustomControl
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
            proxyManagerViewModel.ProxyDataGrid = ProxyDataGrid;
        }
    }
}
