using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class TumblrViewModel : BindableBase
    {
        private TumblrModel _tumblrModel = new TumblrModel();

        public TumblrModel TumblrModel
        {
            get
            {
                return _tumblrModel;
            }
            set
            {
                if (_tumblrModel == value)
                    return;
                SetProperty(ref _tumblrModel, value);
            }
        }
    }
}
