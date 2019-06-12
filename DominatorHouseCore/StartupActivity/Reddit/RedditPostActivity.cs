using DominatorHouseCore.Enums.RdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Reddit
{
    class RedditPostActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PostQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(PostQuery)).ToList();
        }
    }
}
