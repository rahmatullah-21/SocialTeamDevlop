using DominatorHouseCore.Enums.LdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    class LinkedinScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDScraperUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(LDScraperUserQueryParameters)).ToList();
        }
    }

    class LinkedinCompanyScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDScraperUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            return new List<string> { "SearchUrl" };
        }
    }
}
