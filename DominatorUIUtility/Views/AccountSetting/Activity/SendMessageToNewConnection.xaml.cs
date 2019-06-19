using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendMessageToNewConnection.xaml
    /// </summary>
    public partial class SendMessageToNewConnection : UserControl
    {
        public SendMessageToNewConnection(ISendMessageToNewConnectionViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
