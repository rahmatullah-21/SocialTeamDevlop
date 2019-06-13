using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;
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
            var listQueryType = new List<string>();
            Enum.GetValues(typeof(LDScraperUserQueryParameters)).Cast<LDScraperUserQueryParameters>().ForEach(query =>
            {
                if (query != LDScraperUserQueryParameters.Input)
                {
                    listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
                }
            });
            return listQueryType;
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
            return new List<string> { "Search Url" };
        }
    }
}
