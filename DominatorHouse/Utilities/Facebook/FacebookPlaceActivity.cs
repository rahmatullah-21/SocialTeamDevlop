using DominatorHouseCore.StartupActivity;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookPlaceActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return null;// Enum.GetNames(typeof(PlaceQueryParameters)).ToList();
        }
    }
}
