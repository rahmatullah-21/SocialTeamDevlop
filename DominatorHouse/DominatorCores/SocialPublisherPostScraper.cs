using DominatorHouseCore.Interfaces;

namespace Legion.DominatorCores
{
    public class SocialPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary() => new SocialPublisherPostDetailsScraper();
    }
}