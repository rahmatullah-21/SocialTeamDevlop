using DominatorHouseCore.Enums.RdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Reddit
{
    class RedditUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(UserQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(UserQuery)).ToList();
        }
    }
}
