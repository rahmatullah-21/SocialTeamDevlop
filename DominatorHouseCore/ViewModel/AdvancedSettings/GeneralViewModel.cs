using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
   public class GeneralViewModel : BindableBase
    {
        private GeneralModel _generalModel = new GeneralModel();

        public GeneralModel GeneralModel
        {
            get
            {
                return _generalModel;
            }
            set
            {
                if (_generalModel == value)
                    return;
                SetProperty(ref _generalModel, value);
            }
        }
    }
}
