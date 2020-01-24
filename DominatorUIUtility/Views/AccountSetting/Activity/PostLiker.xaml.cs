using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for PostLiker.xaml
    /// </summary>
    public partial class PostLiker : UserControl
    {
        public PostLiker(IPostLikerViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
