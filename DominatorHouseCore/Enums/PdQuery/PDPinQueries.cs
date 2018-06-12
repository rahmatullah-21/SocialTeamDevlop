using System.ComponentModel;

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
