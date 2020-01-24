using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendMessageToNewFriends.xaml
    /// </summary>
    public partial class SendMessageToNewFriends : UserControl
    {
        public SendMessageToNewFriends(ISendMessageToNewFriendsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
