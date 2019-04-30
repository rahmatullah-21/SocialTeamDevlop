using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{

    public partial class SelectNetwork : UserControl
    {
        public SelectNetwork()
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISelectNetworkViewModel>();
            StartupUi.DataContext = viewModel;
        }

    }
}