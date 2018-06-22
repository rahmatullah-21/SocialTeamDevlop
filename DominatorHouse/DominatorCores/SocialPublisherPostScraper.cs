using DominatorHouseCore.Interfaces;

namespace DominatorHouse.DominatorCores
{
    public class SocialPublisherPostScraper : IPublisherPostScraper
    {
        public PostScraper GetPostScraperLibrary() => new SocialPublisherPostDetailsScraper();
    }
}