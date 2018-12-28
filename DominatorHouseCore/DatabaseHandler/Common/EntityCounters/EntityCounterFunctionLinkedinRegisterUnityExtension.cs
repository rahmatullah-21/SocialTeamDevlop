using DominatorHouseCore.DatabaseHandler.LdTables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionLinkedinRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateFilterPredicate<InteractedUsers>(
                            a => a.InteractionDatetime),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.LinkedIn, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedJobs>>(
                    new EntityCounterFunction<InteractedJobs>(
                        new DateFilterPredicate<InteractedJobs>(
                            a => a.InteractionDatetime),
                        new ActivityTypeAsStringFilterPredicate<InteractedJobs>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedJobs>>(
                    new CounterKeyFactory<InteractedJobs>(SocialNetworks.LinkedIn, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateFilterPredicate<InteractedPosts>(
                            a => a.InteractionDatetime),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.LinkedIn, true));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedCompanies>>(
                    new EntityCounterFunction<InteractedCompanies>(
                        new DateFilterPredicate<InteractedCompanies>(
                            a => a.InteractionDatetime),
                        new ActivityTypeAsStringFilterPredicate<InteractedCompanies>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedCompanies>>(
                    new CounterKeyFactory<InteractedCompanies>(SocialNetworks.LinkedIn, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedGroups>>(
                    new EntityCounterFunction<InteractedGroups>(
                        new DateFilterPredicate<InteractedGroups>(
                            a => a.InteractionDatetime),
                        new ActivityTypeAsStringFilterPredicate<InteractedGroups>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedGroups>>(
                    new CounterKeyFactory<InteractedGroups>(SocialNetworks.LinkedIn, true));

        }
    }
}