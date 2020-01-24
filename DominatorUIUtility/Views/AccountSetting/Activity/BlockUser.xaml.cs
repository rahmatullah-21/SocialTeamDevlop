using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for BlockUser.xaml
    /// </summary>
    public partial class BlockUser : UserControl
    {
        public BlockUser(IBlockUserViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
