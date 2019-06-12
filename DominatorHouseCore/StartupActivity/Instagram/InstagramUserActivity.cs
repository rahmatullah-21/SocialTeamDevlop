using DominatorHouseCore.Enums.GdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdUserQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(GdUserQuery)).ToList();
        }
    }
}
