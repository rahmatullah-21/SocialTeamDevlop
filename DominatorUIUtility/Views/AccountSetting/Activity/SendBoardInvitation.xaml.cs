using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for SendBoardInvitation.xaml
    /// </summary>
    public partial class SendBoardInvitation : UserControl
    {
        public SendBoardInvitation(ISendBoardInvitationViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
