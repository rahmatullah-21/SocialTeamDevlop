using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Unfollow.xaml
    /// </summary>
    public partial class Unfollow : UserControl
    {
        public Unfollow(IFollowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
