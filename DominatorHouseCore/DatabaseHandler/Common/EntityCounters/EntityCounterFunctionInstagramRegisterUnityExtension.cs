using DominatorHouseCore.DatabaseHandler.GdTables.Accounts;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Utility;
using Unity;
using Unity.Extension;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionInstagramRegisterUnityExtension : UnityContainerExtension
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
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Instagram, true));

            Container
              .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                  new EntityCounterFunction<InteractedPosts>(
                      new DateFilterPredicate<InteractedPosts>(
                          a => a.InteractionDate.EpochToDateTimeLocal()),
                      new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                          a => a.ActivityType.ToString())));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Instagram, true));

            Container
              .RegisterInstance<IEntityCounterFunction<UnfollowedUsers>>(
                  new EntityCounterFunction<UnfollowedUsers>(
                      new DateEpochFilterPredicate<UnfollowedUsers>(
                          a => a.InteractionDate)));
            Container
                .RegisterInstance<ICounterKeyFactory<UnfollowedUsers>>(
                    new CounterKeyFactory<UnfollowedUsers>(SocialNetworks.Instagram, false));


            Container
                .RegisterInstance<IEntityCounterFunction<HashtagScrape>>(
                    new EntityCounterFunction<HashtagScrape>(
                        new DateFilterPredicate<HashtagScrape>(
                            a => a.Date.EpochToDateTimeLocal())));
            Container
                .RegisterInstance<ICounterKeyFactory<HashtagScrape>>(
                    new CounterKeyFactory<HashtagScrape>(SocialNetworks.Instagram, false));
            
        }
    }
}