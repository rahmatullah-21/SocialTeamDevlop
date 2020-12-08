#region

using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;

#endregion

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionQuoraRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.Date),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Quora, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedAnswers>>(
                    new EntityCounterFunction<InteractedAnswers>(
                        new DateFilterPredicate<InteractedAnswers>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedAnswers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedAnswers>>(
                    new CounterKeyFactory<InteractedAnswers>(SocialNetworks.Quora, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateEpochFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate),
                        new ActivityTypeFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Quora, true));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedQuestion>>(
                    new EntityCounterFunction<InteractedQuestion>(
                        new DateFilterPredicate<InteractedQuestion>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedQuestion>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedQuestion>>(
                    new CounterKeyFactory<InteractedQuestion>(SocialNetworks.Quora, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedMessage>>(
                    new EntityCounterFunction<InteractedMessage>(
                        new DateFilterPredicate<InteractedMessage>(
                            a => a.InteractionDate),
                        new ActivityTypeAsStringFilterPredicate<InteractedMessage>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedMessage>>(
                    new CounterKeyFactory<InteractedMessage>(SocialNetworks.Quora, true));

            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateEpochFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Quora, false));
        }
    }
}