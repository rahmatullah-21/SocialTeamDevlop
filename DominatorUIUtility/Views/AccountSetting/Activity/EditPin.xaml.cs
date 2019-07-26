using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for EditPin.xaml
    /// </summary>
    public partial class EditPin : UserControl
    {
        public EditPin(IEditPinViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
