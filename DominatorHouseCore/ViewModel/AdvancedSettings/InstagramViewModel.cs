using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
   public class InstagramViewModel : BindableBase
    {
        private InstagramModel _instagramModel = new InstagramModel();

        public InstagramModel InstagramModel
        {
            get
            {
                return _instagramModel;
            }
            set
            {
                if (_instagramModel == value)
                    return;
                SetProperty(ref _instagramModel, value);
            }
        }
    }
}
