using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Models.NetworkActivitySetting;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouse.ViewModels.Startup;

namespace DominatorHouse.Startup
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
