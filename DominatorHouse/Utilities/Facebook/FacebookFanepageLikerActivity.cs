using DominatorHouseCore.StartupActivity;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookFanepageLikerActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(FanpageLikerQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(FanpageLikerQueryParameters)).ToList();
        }
    }
}
