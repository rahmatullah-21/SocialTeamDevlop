using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for RemoveVote.xaml
    /// </summary>
    public partial class RemoveVote : UserControl
    {
        public RemoveVote(IRemoveVoteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
