using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(LDMessengerQueryParameters)).Cast<LDMessengerQueryParameters>().ToList().ForEach(query =>
            {
                listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
           
        }
    }
}
