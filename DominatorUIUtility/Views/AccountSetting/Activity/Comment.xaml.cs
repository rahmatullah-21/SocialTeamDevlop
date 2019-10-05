using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Like.xaml
    /// </summary>
    public partial class Comment : UserControl
    {
        public Comment(ICommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
