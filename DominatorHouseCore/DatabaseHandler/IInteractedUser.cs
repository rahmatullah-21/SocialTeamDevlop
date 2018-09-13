using System;

namespace DominatorHouseCore.DatabaseHandler
{
    public interface IInteractedUser
    {
        string ActivityType { get; }

        DateTime InteractionDateTime { get; }

        int InteractionTimeStamp { get; }
    }
}
