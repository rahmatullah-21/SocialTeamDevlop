using System.ComponentModel;

namespace DominatorHouseCore.Enums.LdQuery
{
    public enum LDEngageQueryParameters
    {
        [Description("LangKeySomeonesPostS")]
        SomeonesPosts,
        [Description("LangKeyMyConnectionsPostS")]
        MyConnectionsPosts,
        [Description("LangKeyMyGroupsPostS")]
        MyGroupsPosts,
        [Description("LangKeyCustomPostsList")]
        CustomPosts,
    }
}
