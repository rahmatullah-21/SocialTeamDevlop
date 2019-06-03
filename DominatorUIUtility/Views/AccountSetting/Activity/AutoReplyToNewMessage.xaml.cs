using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for AutoReplyToNewMessage.xaml
    /// </summary>
    public partial class AutoReplyToNewMessage : UserControl
    {
        public AutoReplyToNewMessage(IAutoReplyToNewMessageViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
