using System.ComponentModel;

namespace DominatorHouseCore.Enums.FdQuery
{
    public enum EventType
    {
        [Description("Create Private Event")]
        CreatePrivateEvent = 1,
        [Description("Create Public Event")]
        CreatePublicEvent = 2,

    }
}
