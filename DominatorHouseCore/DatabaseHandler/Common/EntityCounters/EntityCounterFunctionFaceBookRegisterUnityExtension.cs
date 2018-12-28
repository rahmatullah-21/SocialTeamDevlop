using DominatorHouseCore.DatabaseHandler.FdTables.Accounts;
using DominatorHouseCore.Enums;
using Unity;
using Unity.Extension;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.DatabaseHandler.Common.EntityCounters
{
    public class EntityCounterFunctionFaceBookRegisterUnityExtension : UnityContainerExtension
    {
        protected override void Initialize()
        {
            Container
                .RegisterInstance<IEntityCounterFunction<InteractedUsers>>(
                    new EntityCounterFunction<InteractedUsers>(
                        new DateEpochFilterPredicate<InteractedUsers>(
                            a => a.InteractionTimeStamp),
                        new ActivityTypeAsStringFilterPredicate<InteractedUsers>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedUsers>>(
                    new CounterKeyFactory<InteractedUsers>(SocialNetworks.Facebook, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPages>>(
                    new EntityCounterFunction<InteractedPages>(
                        new DateFilterPredicate<InteractedPages>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedPages>(
                            a => a.ActivityType)));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPages>>(
                    new CounterKeyFactory<InteractedPages>(SocialNetworks.Facebook, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedPosts>>(
                    new EntityCounterFunction<InteractedPosts>(
                        new DateFilterPredicate<InteractedPosts>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedPosts>(
                            a => a.ActivityType.ToString())));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedPosts>>(
                    new CounterKeyFactory<InteractedPosts>(SocialNetworks.Facebook, true));


            Container
                .RegisterInstance<IEntityCounterFunction<InteractedGroups>>(
                    new EntityCounterFunction<InteractedGroups>(
                        new DateFilterPredicate<InteractedGroups>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedGroups>(
                            a => a.ActivityType.ToString())));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedGroups>>(
                    new CounterKeyFactory<InteractedGroups>(SocialNetworks.Facebook, true));

            Container
                .RegisterInstance<IEntityCounterFunction<InteractedComments>>(
                    new EntityCounterFunction<InteractedComments>(
                        new DateFilterPredicate<InteractedComments>(
                            a => a.InteractionDateTime),
                        new ActivityTypeAsStringFilterPredicate<InteractedComments>(
                            a => a.ActivityType.ToString())));
            Container
                .RegisterInstance<ICounterKeyFactory<InteractedComments>>(
                    new CounterKeyFactory<InteractedComments>(SocialNetworks.Facebook, true));

           
        }
    }
}
