using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class PinterestViewModel : BindableBase
    {
        private PinterestModel _pinterestModel = new PinterestModel();

        public PinterestModel PinterestModel
        {
            get
            {
                return _pinterestModel;
            }
            set
            {
                if (_pinterestModel == value)
                    return;
                SetProperty(ref _pinterestModel, value);
            }
        }
    }
}
