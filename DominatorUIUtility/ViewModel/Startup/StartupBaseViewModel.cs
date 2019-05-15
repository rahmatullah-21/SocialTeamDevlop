using Prism.Mvvm;
using System.Windows.Input;
using Prism.Regions;
using DominatorHouseCore.Models.NetworkActivitySetting;
using System.Linq;
using System.Collections.Generic;

namespace DominatorUIUtility.ViewModel.Startup
{
    public class StartupBaseViewModel : BindableBase
    {
        public IRegionManager regionManager;
        //SelectActivityModel SelectActivityModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISelectActivityViewModel>()?.SelectActivityModel;
        List<ActivityChecked> LstCheckedActivity;
        static int selectedIndex = 0;
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
            var SelectActivityModel = CommonServiceLocator.ServiceLocator.Current.GetInstance<ISelectActivityViewModel>()?.SelectActivityModel;
            LstCheckedActivity = SelectActivityModel.LstNetworkActivityType.Where(x => x.IsActivity).ToList();
            var next = LstCheckedActivity[selectedIndex].ActivityType;
            selectedIndex++;
            regionManager.RequestNavigate("StartupRegion", next);
        }
        protected void NevigatePrevious()
        {
            var previous = LstCheckedActivity[selectedIndex].ActivityType;
            selectedIndex--;
            regionManager.RequestNavigate("StartupRegion", previous);
        }

    }
}
