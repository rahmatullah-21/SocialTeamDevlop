using System.ComponentModel;

namespace DominatorHouseCore.Enums.PdQuery
{
    public enum PDPinQueries
    {
        [Description("LangKeyKeywords")]
        Keywords = 1,
        [Description("LangKeyCustomUsers")]
        Customusers = 2,
        [Description("LangKeyCustomBoard")]
        CustomBoard = 3,
        [Description("LangKeyCustomPin")]
        CustomPin = 4
    }
}
