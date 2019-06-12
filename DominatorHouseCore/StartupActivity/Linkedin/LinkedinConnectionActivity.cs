using DominatorHouseCore.Enums.LdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    class LinkedinConnectionActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDGrowConnectionUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(LDGrowConnectionUserQueryParameters)).ToList();
        }
    }
}
