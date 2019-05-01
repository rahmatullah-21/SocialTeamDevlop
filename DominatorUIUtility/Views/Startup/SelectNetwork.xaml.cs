using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{

    public partial class SelectNetwork : UserControl
    {
        ISelectNetworkViewModel viewModel;
        public SelectNetwork()
        {
            InitializeComponent();
             viewModel = ServiceLocator.Current.GetInstance<ISelectNetworkViewModel>();
            StartupUi.DataContext = viewModel;
        }

    }
}