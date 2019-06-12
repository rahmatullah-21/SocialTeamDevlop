using DominatorHouseCore.Enums.GdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramPostActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(GdPostQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(GdPostQuery)).ToList();
        }
    }
}
