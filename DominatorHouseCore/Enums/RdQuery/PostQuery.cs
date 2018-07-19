using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.RdQuery
{
    public enum PostQuery
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomurl")]
        CustomUrl = 2,
        [Description("LangKeySocinatorPublisherCampaign")]
        SocinatorPublisherCampaign,
    }
}
