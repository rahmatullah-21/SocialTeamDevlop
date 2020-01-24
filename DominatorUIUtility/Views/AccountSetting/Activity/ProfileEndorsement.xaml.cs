using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for ProfileEndorsement.xaml
    /// </summary>
    public partial class ProfileEndorsement : UserControl
    {
        public ProfileEndorsement(IProfileEndorsementViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
