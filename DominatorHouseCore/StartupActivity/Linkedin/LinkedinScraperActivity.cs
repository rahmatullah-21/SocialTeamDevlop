#region

using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Enums.LdQuery;
using DominatorHouseCore.Utility;

#endregion

namespace DominatorHouseCore.StartupActivity.Linkedin
{
    internal class LinkedinScraperActivity : BaseActivity
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
                    listQueryType.Add(query.GetDescriptionAttr()?.FromResourceDictionary());
            });
            return listQueryType;
        }
    }

    internal class LinkedinUserScraperActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(LDScraperUserQueryParameters);
        }

        public override List<string> GetQueryType()
        {
            var listQueryType = new List<string>();

            listQueryType.Add(LDScraperUserQueryParameters.Keyword.GetDescriptionAttr()?.FromResourceDictionary());
            listQueryType.Add(LDScraperUserQueryParameters.ProfileUrl.GetDescriptionAttr()?.FromResourceDictionary());
            listQueryType.Add(LDScraperUserQueryParameters.SearchUrl.GetDescriptionAttr()?.FromResourceDictionary());

            return listQueryType;
        }
    }
}