using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Tumblr
{
    class TumblrBroadcastMessageActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TumblrBroadcastMessageQuery);

        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(TumblrBroadcastMessageQuery)).Cast<TumblrBroadcastMessageQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
