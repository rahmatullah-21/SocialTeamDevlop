using DominatorHouseCore.Enums.TdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Twitter
{
    class TwitterTweetActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(TdTweetInteractionQueryEnum);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(TdTweetInteractionQueryEnum)).ToList();
        }
    }
}
