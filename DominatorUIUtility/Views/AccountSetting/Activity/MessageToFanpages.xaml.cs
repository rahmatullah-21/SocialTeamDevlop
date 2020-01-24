using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for MessageToFanpages.xaml
    /// </summary>
    public partial class MessageToFanpages : UserControl
    {
        public MessageToFanpages(IMessageToFanpagesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
