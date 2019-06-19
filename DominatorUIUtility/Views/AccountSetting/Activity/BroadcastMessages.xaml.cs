using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for BroadcastMessages.xaml
    /// </summary>
    public partial class BroadcastMessages : UserControl
    {
        public BroadcastMessages(IBroadcastMessagesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
