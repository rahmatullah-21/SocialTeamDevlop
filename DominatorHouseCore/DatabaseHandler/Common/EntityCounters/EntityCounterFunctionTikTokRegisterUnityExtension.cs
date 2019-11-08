using DominatorHouseCore.DatabaseHandler.TtdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity.Extension;
using Unity;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionTikTokRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
              .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                  new EntityCounterFunction<InteractedUsers>(
                      new DateEpochFilterPredicate<InteractedUsers>(a => a.Date),
                      new ActivityTypeAsStringFilterPredicate<InteractedUsers>(a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.TikTok, true));

        }
    }
}
