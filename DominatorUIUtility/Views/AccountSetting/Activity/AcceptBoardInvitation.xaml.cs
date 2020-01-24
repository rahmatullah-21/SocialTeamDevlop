using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for AcceptBoardInvitation.xaml
    /// </summary>
    public partial class AcceptBoardInvitation : UserControl
    {
        public AcceptBoardInvitation(IAcceptBoardInvitationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
