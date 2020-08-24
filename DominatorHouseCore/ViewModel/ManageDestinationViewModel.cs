#region

using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.ViewModel
{
    public class ManageDestinationViewModel : BindableBase
    {
        private ManageDestinationModel _manageDestinationModel = new ManageDestinationModel();

        public ManageDestinationModel ManageDestinationModel
        {
            get => _manageDestinationModel;
            set
            {
                if ((_manageDestinationModel == null) & (_manageDestinationModel == value))
                    return;
                SetProperty(ref _manageDestinationModel, value);
            }
        }
    }
}