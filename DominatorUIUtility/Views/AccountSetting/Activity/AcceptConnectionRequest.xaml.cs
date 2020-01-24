using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for AcceptConnectionRequest.xaml
    /// </summary>
    public partial class AcceptConnectionRequest : UserControl
    {
        public AcceptConnectionRequest(IAcceptConnectionRequestViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
