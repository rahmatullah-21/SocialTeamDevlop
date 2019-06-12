using DominatorHouseCore.Enums.TumblrQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    class TumblrUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TumblrQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(TumblrQuery)).ToList();
        }
    }
}
