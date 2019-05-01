using System.Windows.Controls;
using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.Views.Startup
{

    public partial class StartUpMain : UserControl
    {
        public StartUpMain(SocialNetworks selectedNetworks)
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISaveSettingViewModel>();
            viewModel.SelectedIndex = 0;
            StratupGrid.DataContext = viewModel;
        }

    }
}