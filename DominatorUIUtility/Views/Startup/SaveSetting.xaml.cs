using System.Windows.Controls;
using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorUIUtility.ViewModel.Startup;

namespace DominatorUIUtility.Views.Startup
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
            viewModel.SetNetwork(selectedNetworks);
            viewModel.SelectedIndex =0;
            viewModel.SourceUserControl = viewModel.LstUserControls[viewModel.SelectedIndex];
            SaveSettingUi.DataContext = viewModel;
        }
    }
}
