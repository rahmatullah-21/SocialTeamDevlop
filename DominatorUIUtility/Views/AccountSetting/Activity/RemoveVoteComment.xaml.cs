using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for RemoveVoteComment.xaml
    /// </summary>
    public partial class RemoveVoteComment : UserControl
    {
        public RemoveVoteComment(IRemoveVoteCommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
