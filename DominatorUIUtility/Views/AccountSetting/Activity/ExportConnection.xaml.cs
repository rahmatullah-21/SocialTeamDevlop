using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for ExportConnection.xaml
    /// </summary>
    public partial class ExportConnection : UserControl
    {
        public ExportConnection(IExportConnectionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
