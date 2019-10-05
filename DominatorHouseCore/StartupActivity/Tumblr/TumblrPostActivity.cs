using DominatorHouseCore.Enums.TumblrQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(TumblrPostQuery)).Cast<TumblrPostQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
