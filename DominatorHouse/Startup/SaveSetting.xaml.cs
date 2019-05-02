using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouse.ViewModels.Startup;

namespace DominatorHouse.Startup
{
    /// <summary>
    /// Interaction logic for SelectActivity.xaml
    /// </summary>
    public partial class SaveSetting : UserControl
    {
        public SaveSetting(SocialNetworks selectedNetworks)
        {
            InitializeComponent();
            var viewModel = ServiceLocator.Current.GetInstance<ISaveSettingViewModel>();
            viewModel.SelectedIndex = 0;
            viewModel.SourceUserControl = viewModel.LstUserControls[viewModel.SelectedIndex];
            SaveSettingUi.DataContext = viewModel;
        }
    }
}
