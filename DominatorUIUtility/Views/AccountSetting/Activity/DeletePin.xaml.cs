using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for DeletePin.xaml
    /// </summary>
    public partial class DeletePin : UserControl
    {
        public DeletePin(IDeletePinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
