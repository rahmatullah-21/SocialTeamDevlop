using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for WatchPartyInviter.xaml
    /// </summary>
    public partial class WatchPartyInviter : UserControl
    {
        public WatchPartyInviter(IWatchPartyInviterViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
