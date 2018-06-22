using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using FaceDominatorUI.FdCoreLibrary;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherCoreBuilder : PublisherCoreLibraryBuilder
    {
        private static SocialPublisherCoreBuilder _instance;

        public static SocialPublisherCoreBuilder Instance(IPublisherCoreFactory publisherCoreFactory)
            => _instance ?? (_instance = new SocialPublisherCoreBuilder(publisherCoreFactory));

        private SocialPublisherCoreBuilder(IPublisherCoreFactory publisherCoreFactory)
            : base(publisherCoreFactory)
        {
            FdInitialiser.RegisterModules();
            AddNetwork(SocialNetworks.Social)
                .AddPostScraper(new SocialPublisherPostScraper());
        }

        public IPublisherCoreFactory GetSocialPublisherCoreObjects() => PublisherCoreFactory;
    }
}