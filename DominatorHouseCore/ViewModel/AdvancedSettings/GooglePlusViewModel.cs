using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class GooglePlusViewModel : BindableBase
    {
        private GooglePlusModel _googlePlusModel = new GooglePlusModel();

        public GooglePlusModel GooglePlusModel
        {
            get
            {
                return _googlePlusModel;
            }
            set
            {
                if (_googlePlusModel == value)
                    return;
                SetProperty(ref _googlePlusModel, value);
            }
        }
    }
}
