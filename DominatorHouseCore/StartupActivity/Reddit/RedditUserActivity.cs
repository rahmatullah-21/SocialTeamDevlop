using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(UserQuery)).Cast<UserQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
