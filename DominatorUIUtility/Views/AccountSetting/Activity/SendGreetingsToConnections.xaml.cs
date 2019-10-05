using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendGreetingsToConnections.xaml
    /// </summary>
    public partial class SendGreetingsToConnections : UserControl
    {
        public SendGreetingsToConnections(ISendGreetingsToConnectionsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
