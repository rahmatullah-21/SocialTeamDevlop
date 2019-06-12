using DominatorHouseCore.StartupActivity;
using FaceDominatorCore.FDEnums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouse.Utilities.Facebook
{
    class FacebookUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(FdUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(FdUserQueryParameters)).ToList();
        }
    }
}
