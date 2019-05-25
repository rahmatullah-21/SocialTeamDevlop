using DominatorHouseCore.Enums.TdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Twitter
{
    class TwitterUserActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(TdUserInteractionQueryEnum)).ToList();
        }
    }
}

