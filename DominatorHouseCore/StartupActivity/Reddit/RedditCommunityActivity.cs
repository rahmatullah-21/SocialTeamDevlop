using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(CommunityQuery)).Cast<CommunityQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
