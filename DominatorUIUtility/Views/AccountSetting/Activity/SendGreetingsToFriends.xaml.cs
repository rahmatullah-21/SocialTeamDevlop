using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendGreetingsToFriends.xaml
    /// </summary>
    public partial class SendGreetingsToFriends : UserControl
    {
        public SendGreetingsToFriends(ISendGreetingsToFriendsViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
