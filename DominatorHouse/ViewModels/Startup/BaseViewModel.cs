using Prism.Commands;
using Prism.Regions;
using System.Windows.Input;

namespace DominatorHouse.ViewModels.Startup
{
    class BaseViewModel
    {
        IRegionManager _region;
        public BaseViewModel(IRegionManager region)
        {
            _region = region;
            NextCommand = new DelegateCommand<string>(OnNextClick);
            PreviousCommand = new DelegateCommand<string>(OnPreviousClick);
        }
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        private void OnNextClick(string next)
        {
            _region.RequestNavigate("StartupRegion", next);
            string userType = string.Empty;
            string network = string.Empty;
            //if (SelectedIndex == 0)
            //    userType = ServiceLocator.Current.GetInstance<ISelectUserTypeViewModel>().SelectedUser;
            //if (SelectedIndex == 1)
            //{
            //    network = ServiceLocator.Current.GetInstance<ISelectNetworkViewModel>().SelectedNetwork;
            //    SetNetwork(network);
            //}

            //var button = sender as Button;
            //if (button.Content.ToString() == "Finish")
            //{
            //    var genericFileManager = ServiceLocator.Current.GetInstance<IGenericFileManager>();
            //    genericFileManager.Save(SaveSettingModel, ConstantVariable.GetModuleConfigPath(SelectedNetwork.ToString()));
            //    Dialog.CloseDialog(sender);
            //    return;
            //}
        }
        private void OnPreviousClick(string previous)
        {
            _region.RequestNavigate("StartupRegion", previous);
        }
    }
}
