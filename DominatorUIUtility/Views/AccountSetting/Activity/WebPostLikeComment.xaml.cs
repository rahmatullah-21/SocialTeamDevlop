using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for WebPostLikeComment.xaml
    /// </summary>
    public partial class WebPostLikeComment : UserControl
    {
        public WebPostLikeComment(IWebPostLikeCommentViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
