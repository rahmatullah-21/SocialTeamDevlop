using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for EditComment.xaml
    /// </summary>
    public partial class EditComment : UserControl
    {
        public EditComment(IEditCommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
