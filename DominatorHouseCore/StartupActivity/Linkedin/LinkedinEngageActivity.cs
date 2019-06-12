using DominatorHouseCore.Enums.LdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    class LinkedinEngageActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDEngageQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(LDEngageQueryParameters)).ToList();
        }
    }
}
