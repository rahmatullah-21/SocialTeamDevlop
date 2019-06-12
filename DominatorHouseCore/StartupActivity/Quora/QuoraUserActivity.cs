using DominatorHouseCore.Enums.QdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Quora
{
    public class QuoraUserActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(UserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(UserQueryParameters)).ToList();
        }
    }
}
