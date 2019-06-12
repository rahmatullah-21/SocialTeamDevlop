using DominatorHouseCore.Enums.QdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraBroadcastMessagesActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(BroadcastMessageQuery);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(BroadcastMessageQuery)).ToList();
        }
    }
}
