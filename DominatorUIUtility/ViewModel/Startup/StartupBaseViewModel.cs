using Prism.Mvvm;
using System.Windows.Input;
using Prism.Regions;
using System.Collections.Generic;

namespace DominatorUIUtility.ViewModel.Startup
{
    public class StartupBaseViewModel : BindableBase
    {
        public IRegionManager regionManager;

        static int selectedIndex = 0;
        public static List<string> NavigationList { get; set; }
        public StartupBaseViewModel(IRegionManager region)
        {
            regionManager = region;
        }
        #region Commands
        public ICommand NextCommand { get; set; }
        public ICommand PreviousCommand { get; set; }
        #endregion

        protected void NevigateNext()
        {
            if (selectedIndex >= NavigationList.Count - 1)
                return;
            selectedIndex++;
            var next = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", next);
        }
        protected void NevigatePrevious()
        {
            if (selectedIndex <= 0)
                return;
            selectedIndex--;
            var previous = NavigationList[selectedIndex];
            regionManager.RequestNavigate("StartupRegion", previous);
        }

    }
}
