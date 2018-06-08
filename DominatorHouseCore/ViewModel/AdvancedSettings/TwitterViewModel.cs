using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel.AdvancedSettings
{
    public class TwitterViewModel : BindableBase
    {
        private TwitterModel _twitterModel = new TwitterModel();

        public TwitterModel TwitterModel
        {
            get
            {
                return _twitterModel;
            }
            set
            {
                if (_twitterModel == value)
                    return;
                SetProperty(ref _twitterModel, value);
            }
        }
    }
}
