using DominatorHouseCore.Enums.RdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(PostQuery)).Cast<PostQuery>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
    class RedditRemoveVoteActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(PostQuery);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();
            listQueryType.Add(PostQuery.CustomUrl.GetDescriptionAttr()?.FromResourceDictionary());
            return listQueryType;
        }
    }
}
