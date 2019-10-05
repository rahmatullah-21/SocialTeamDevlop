using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendMessageToFollower.xaml
    /// </summary>
    public partial class SendMessageToFollower : UserControl
    {
        public SendMessageToFollower(ISendMessageToFollowerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
