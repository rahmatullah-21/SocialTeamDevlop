using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums
{
    public enum PinterestAccountType
    {
        [Description("LangKeyBusinessMode")]
        Active,
        [Description("LangKeyNormalMode")]
        Inactive,
        [Description("LangKeyNotAvailable")]
        NotAvailable
    }
}
