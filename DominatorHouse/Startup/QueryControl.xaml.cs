using DominatorHouse.ViewModels.Startup;
using DominatorUIUtility.ViewModel.Startup;
using System.Windows.Controls;

namespace DominatorHouse.Startup
{
    /// <summary>
    /// Interaction logic for QueryControl.xaml
    /// </summary>
    public partial class QueryControl : UserControl
    {
        public QueryControl(ISaveSettingViewModel viewModel)
        {
            InitializeComponent();
            Query.DataContext = viewModel;
        }
    }
}
