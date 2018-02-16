using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class ManageDestinationViewModel : BindableBase
    {
        private ManageDestinationModel _manageDestinationModel = new ManageDestinationModel();

        public ManageDestinationModel ManageDestinationModel
        {
            get
            {
                return _manageDestinationModel;
            }
            set
            {
                if (_manageDestinationModel == null & _manageDestinationModel == value)
                    return;
                SetProperty(ref _manageDestinationModel, value);
            }
        }
    }
}