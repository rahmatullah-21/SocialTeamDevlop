using DominatorHouseCore.Enums.PdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Pinterest
{
    public class PinterestPinActivity : BaseActivity
    {
        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(PDPinQueries)).ToList();
        }
    }
}
