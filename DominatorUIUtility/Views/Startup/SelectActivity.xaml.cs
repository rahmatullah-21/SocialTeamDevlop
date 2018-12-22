using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{
    /// <summary>
    /// Interaction logic for SelectActivity.xaml
    /// </summary>
    public partial class SelectActivity : UserControl
    {
        public SelectActivity()
        {
            InitializeComponent();
           
        }

        public SelectActivity(ISaveSettingViewModel viewModel):this()
        {
            SelectActivityType.DataContext = viewModel;
        }
    }
}
