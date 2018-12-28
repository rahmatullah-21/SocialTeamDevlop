using DominatorHouseCore.DatabaseHandler.GplusTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionGplusRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
              .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                  new EntityCounterFunction<InteractedUsers>(
                      new DateFilterPredicate<InteractedUsers>(
                          a => a.Date.EpochToDateTimeLocal()),
                      new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                          a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Gplus, false));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateFilterPredicate<InteractedPosts>(
                            a => a.InteractionDate.EpochToDateTimeLocal()),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Gplus, true));

            Container
                .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                    new EntityCounterFunction<UnfollowedUsers>(
                        new DateFilterPredicate<UnfollowedUsers>(
                            a => a.InteractionDate.EpochToDateTimeLocal())));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Gplus, false));

            Container
                  .RegisterInstance<IEntityCounterFunction<InteractedCommunities>>(
                      new EntityCounterFunction<InteractedCommunities>(
                          new DateFilterPredicate<InteractedCommunities>(
                              a => a.InteractionDate.EpochToDateTimeLocal()),
                          new ActivityTypeAsStringFilterPredicate<InteractedCommunities>(
                              a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedCommunities>>(
                    new CounterKeyFactory<InteractedCommunities>(SocialNetworks.Gplus, true));

            //Container
            //     .RegisterInstance<IEntityCounterFunction<InteractedGplusComments>>(
            //         new EntityCounterFunction<InteractedGplusComments>(
            //             new DateFilterPredicate<InteractedGplusComments>(
            //                 a => a.InteractionTimeStamp.EpochToDateTimeLocal()),
            //             new ActivityTypeAsStringFilterPredicate<InteractedCommunities>(
            //                 a => a.ActivityType)));
            //Container
            //    .RegisterInstance<ICounterKeyFactory<InteractedGplusComments>>(
            //        new CounterKeyFactory<InteractedGplusComments>(SocialNetworks.Gplus, true));
        }
    }
}
