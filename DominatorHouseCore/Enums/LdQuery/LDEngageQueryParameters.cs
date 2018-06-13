using System.ComponentModel;

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
