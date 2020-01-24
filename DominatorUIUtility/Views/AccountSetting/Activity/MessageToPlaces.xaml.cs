using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for MessageToPlaces.xaml
    /// </summary>
    public partial class MessageToPlaces : UserControl
    {
        public MessageToPlaces(IMessageToPlacesViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
