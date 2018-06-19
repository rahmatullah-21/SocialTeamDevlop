using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IFdScrapePosts
    {
         IEnumerable<PublisherPostlistModel> ScrapeOwnWallPosts(string accountId, int count);
         IEnumerable<PublisherPostlistModel> ScrapePagePosts(string accountId, string url, int count);
         IEnumerable<PublisherPostlistModel> ScrapeGroupPosts(string accountId, string url, int count);
         IEnumerable<PublisherPostlistModel> ScrapeFriendPosts(string accountId, string url, int count);
    }
}