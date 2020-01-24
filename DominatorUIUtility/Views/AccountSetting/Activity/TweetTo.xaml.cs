using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for TweetTo.xaml
    /// </summary>
    public partial class TweetTo : UserControl
    {
        public TweetTo(ITweetToViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
