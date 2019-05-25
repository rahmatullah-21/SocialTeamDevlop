using DominatorHouseCore.Enums.TumblrQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    class TumblrBroadcastMessageActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(TumblrBroadcastMessageQuery)).ToList();
        }
    }
}
