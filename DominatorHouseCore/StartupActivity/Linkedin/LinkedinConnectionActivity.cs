using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(LDGrowConnectionUserQueryParameters)).Cast<LDGrowConnectionUserQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }
}
