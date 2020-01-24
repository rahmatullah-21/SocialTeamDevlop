using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Like.xaml
    /// </summary>
    public partial class Like : UserControl
    {
        public Like(ILikeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
