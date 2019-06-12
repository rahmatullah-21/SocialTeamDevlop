using DominatorHouseCore.Enums.LdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    class LinkedinGroupActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDGroupQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(LDGroupQueryParameters)).ToList();
        }
    }
}
