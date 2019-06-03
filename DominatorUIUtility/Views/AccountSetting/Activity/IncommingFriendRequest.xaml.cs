using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for IncommingFriendRequest.xaml
    /// </summary>
    public partial class IncommingFriendRequest : UserControl
    {
        public IncommingFriendRequest(IIncommingFriendRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
