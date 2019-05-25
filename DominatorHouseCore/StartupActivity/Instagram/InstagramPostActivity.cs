using DominatorHouseCore.Enums.GdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Instagram
{
    public class InstagramPostActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(GdPostQuery)).ToList();
        }
    }
}
