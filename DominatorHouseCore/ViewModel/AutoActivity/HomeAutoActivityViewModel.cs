using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AutoActivity
{
    public class HomeAutoActivityViewModel : BindableBase
    {

        private UserControlSwitchViewModel _userControlSwitchViewModel = new SocialAutoActivityViewModel();

        public UserControlSwitchViewModel UserControlSwitchViewModel
        {
            get
            {
                return _userControlSwitchViewModel;
            }
            set
            {
                if( _userControlSwitchViewModel==value)
                    return;
                SetProperty(ref _userControlSwitchViewModel, value);
            }
        }

    }
}
