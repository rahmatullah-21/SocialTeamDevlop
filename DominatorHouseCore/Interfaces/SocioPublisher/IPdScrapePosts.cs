using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IPdScrapePosts
    {
        IEnumerable<PublisherPostlistModel> ScrapeBoardPosts(string accountId, int count);
        IEnumerable<PublisherPostlistModel> ScrapeUsersPosts(string accountId, string url, int count);
        IEnumerable<PublisherPostlistModel> ScrapeCategoryPosts(string accountId, string url, int count);
        IEnumerable<PublisherPostlistModel> ScrapeSearchPinPosts(string accountId, string keyword, int count);
    }
}