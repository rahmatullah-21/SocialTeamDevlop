using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for PostCommentor.xaml
    /// </summary>
    public partial class PostCommentor : UserControl
    {
        public PostCommentor(IPostCommentorViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
