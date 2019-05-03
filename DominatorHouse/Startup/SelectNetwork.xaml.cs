using System.Windows.Controls;
using DominatorHouse.ViewModels.Startup;

namespace DominatorHouse.Startup
{
    public partial class SelectNetwork : UserControl
    {
        public SelectNetwork(ISelectNetworkViewModel viewModel)
        {
            InitializeComponent();
            StartupUi.DataContext = viewModel;
        }
    }
}