using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.PdQuery
{
    public enum PDPinQueries
    {
        [Description("PDlangKeywords")]
        Keywords = 1,
        [Description("PDlangCustomUser")]
        Customusers = 2,
        [Description("PDlangCustomBoard")]
        CustomBoard = 3,
        [Description("PDlangCustomPin")]
        CustomPin = 4
    }
}
