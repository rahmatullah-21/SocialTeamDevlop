using DominatorHouseCore.Enums.QdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraFollowActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(FollowerQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(FollowerQuery)).ToList();
        }
    }
}
