using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Downvote.xaml
    /// </summary>
    public partial class Downvote : UserControl
    {
        public Downvote(IDownvoteViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
