using DominatorHouseCore;
using DominatorUIUtility.ViewModel;
using System;
using System.Windows.Controls;
using Unity;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager : UserControl
    {


        public ProxyManagerViewModel ProxyManagerViewModel
        {
            get
            {
                return (ProxyManagerViewModel)DominatorHouseCore.IoC.Container.Resolve<IProxyManagerViewModel>();
            }
        }

        public ProxyManager(DominatorAccountViewModel.AccessorStrategies strategies)
        {

            InitializeComponent();
            _proxyManagerInstance = this;
            ProxyManagerViewModel._strategies = strategies;
            MainGrid.DataContext = ProxyManagerViewModel;
        }

        private static ProxyManager _proxyManagerInstance = null;
        public static ProxyManager GetProxyManagerControl(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            try
            {
                return _proxyManagerInstance ?? (_proxyManagerInstance = new ProxyManager(strategies));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return _proxyManagerInstance;
            }
        }

    }
}
