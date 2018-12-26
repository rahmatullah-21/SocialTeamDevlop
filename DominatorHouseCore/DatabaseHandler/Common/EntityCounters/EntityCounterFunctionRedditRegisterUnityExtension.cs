using DominatorHouseCore.DatabaseHandler.RdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionRedditRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance(
                    new EntityCounterFunction<InteractedSubreddit>(
                        new DateFilterPredicate<InteractedSubreddit>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedSubreddit>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedSubreddit>>(
                    new CounterKeyFactory<InteractedSubreddit>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance(
                    new EntityCounterFunction<InteractedPost>(
                        new DateFilterPredicate<InteractedPost>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedPost>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPost>>(
                    new CounterKeyFactory<InteractedPost>(SocialNetworks.Reddit, true));

            Container
                .RegisterInstance(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Reddit, false));
        }
    }
}
