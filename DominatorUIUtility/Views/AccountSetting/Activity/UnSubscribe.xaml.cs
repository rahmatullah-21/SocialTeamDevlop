using LegionUIUtility.ViewModel.Startup.ModuleConfig;
using System.Windows.Controls;

namespace LegionUIUtility.Views.AccountSetting.Activity
{
    /// <summary>
    /// Interaction logic for Unsubscribe.xaml
    /// </summary>
    public partial class UnSubscribe : UserControl
    {
        public UnSubscribe(IUnsubscribeViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
