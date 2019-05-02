using DominatorHouse.ViewModels.Startup;
using DominatorUIUtility.ViewModel.Startup;
using System.Windows.Controls;

namespace DominatorHouse.Startup
{
    /// <summary>
    /// Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfig : UserControl
    {
        public JobConfig(ISaveSettingViewModel viewModel)
        {
            InitializeComponent();
            JobConfigGrid.DataContext = viewModel;
        }
    }
}
