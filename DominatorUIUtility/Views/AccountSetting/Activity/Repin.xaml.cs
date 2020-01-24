using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Repin.xaml
    /// </summary>
    public partial class Repin : UserControl
    {
        public Repin(IRepinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
