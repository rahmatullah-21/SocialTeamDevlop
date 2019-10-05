using DominatorUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace DominatorUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for WelcomeTweet.xaml
    /// </summary>
    public partial class WelcomeTweet : UserControl
    {
        public WelcomeTweet(IWelcomeTweetViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
