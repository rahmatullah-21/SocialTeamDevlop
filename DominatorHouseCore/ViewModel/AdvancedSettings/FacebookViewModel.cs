using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class FacebookViewModel : BindableBase
    {
        private FacebookModel _facebookModel = new FacebookModel();

        public FacebookModel FacebookModel
        {
            get
            {
                return _facebookModel;
            }
            set
            {
                if (_facebookModel == value)
                    return;
                SetProperty(ref _facebookModel, value);
            }
        }
    }
}
