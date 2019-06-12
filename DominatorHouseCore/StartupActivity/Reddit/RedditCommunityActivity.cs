using DominatorHouseCore.Enums.RdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Reddit
{
    class RedditCommunityActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(CommunityQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(CommunityQuery)).ToList();
        }
    }
}
