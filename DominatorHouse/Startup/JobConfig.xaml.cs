using DominatorHouse.ViewModels.Startup;
using System.Windows.Controls;

namespace DominatorHouse.Startup
{
    /// <summary>
    /// Interaction logic for JobConfigControl.xaml
    /// </summary>
    public partial class JobConfig : UserControl
    {
        public JobConfig(IJobConfigViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
