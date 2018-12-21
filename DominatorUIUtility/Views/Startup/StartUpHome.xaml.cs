
using System.Windows;
using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
{

    public partial class StartUpHome : UserControl
    {
        public StartUpHome(SocialNetworks selectedNetworks)
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<IStartUpHomeViewModel>();
            StartupUi.DataContext = viewModel;
        }

    }
}