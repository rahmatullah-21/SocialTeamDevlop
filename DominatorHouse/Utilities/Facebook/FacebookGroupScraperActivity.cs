using DominatorHouseCore.Enums.FdQuery;
using DominatorHouseCore.StartupActivity;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Legion.Utilities.Facebook
{
    class FacebookGroupScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GroupScraperParameter);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(GroupScraperParameter)).ToList();
        }
    }
}
