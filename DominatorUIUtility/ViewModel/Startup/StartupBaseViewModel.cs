using Prism.Mvvm;
using System.Windows.Input;
using Prism.Regions;

namespace DominatorUIUtility.ViewModel.Startup
{
    public class StartupBaseViewModel : BindableBase
    {
        public IRegionManager regionManager;
        public StartupBaseViewModel(IRegionManager region)
        {
            regionManager = region;
        }
        #region Commands
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        #endregion

        protected void OnNextClick(string next)
        {
            regionManager.RequestNavigate("StartupRegion", next);
        }
        protected void OnPreviousClick(string previous)
        {
            regionManager.RequestNavigate("StartupRegion", previous);
        }
    }
}
