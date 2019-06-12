using DominatorHouseCore.Enums.YdQuery;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.StartupActivity.Youtube
{
    public class YoutubeChannelActivity : BaseActivity
    {
        public override Type GetEnumType()
        {
            return typeof(YdScraperParameters);
        }

        public override List<string> GetQueryType()
        {
            return Enum.GetNames(typeof(YdScraperParameters)).ToList();
        }
    }
}
