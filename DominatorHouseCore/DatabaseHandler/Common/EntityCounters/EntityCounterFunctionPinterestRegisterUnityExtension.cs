using DominatorHouseCore.DatabaseHandler.PdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionPinterestRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.InteractionTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Pinterest, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedBoards>>(
                    new EntityCounterFunction<InteractedBoards>(
                        new TimespanFilterPredicate<InteractedBoards>(
                            a => a.InteractionDate),
                        new ActivityTypeFilterPredicate<InteractedBoards>(
                            a => a.OperationType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedBoards>>(
                    new CounterKeyFactory<InteractedBoards>(SocialNetworks.Pinterest, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new TimespanFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate),
                        new ActivityTypeFilterPredicate<InteractedPosts>(
                            a => a.OperationType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Pinterest, true));


            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Pinterest, false));
        }
    }
}