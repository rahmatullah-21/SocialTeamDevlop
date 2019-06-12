using DominatorHouseCore.Enums.TumblrQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    class TumblrPostActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TumblrPostQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(TumblrPostQuery)).ToList();
        }
    }
}
