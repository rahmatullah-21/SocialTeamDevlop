using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Unlike.xaml
    /// </summary>
    public partial class Unlike : UserControl
    {
        public Unlike(IUnlikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
