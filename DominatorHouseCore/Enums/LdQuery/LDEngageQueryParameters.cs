using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDEngageQueryParameters
    {
        [Description("LDlangSomeonesPosts")]
        SomeonesPosts,
        [Description("LDlangMyConnectionsPosts")]
        MyConnectionsPosts,
        [Description("LDlangMyGroupsPosts")]
        MyGroupsPosts,
        [Description("LDlangCustomPosts")]
        CustomPosts,
    }
}
