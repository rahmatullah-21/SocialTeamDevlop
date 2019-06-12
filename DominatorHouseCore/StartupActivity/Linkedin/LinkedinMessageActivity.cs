using DominatorHouseCore.Enums.LdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    class LinkedinMessageActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDMessengerQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(LDMessengerQueryParameters)).ToList();
        }
    }
}
