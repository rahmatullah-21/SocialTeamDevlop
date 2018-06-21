using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface IFdScrapePosts
    {
         IEnumerable<PublisherPostlistModel> ScrapeOwnWallPosts(string accountId, string campaignId, int count);
         IEnumerable<PublisherPostlistModel> ScrapePagePosts(string accountId, string campaignId, string url, int count);
         IEnumerable<PublisherPostlistModel> ScrapeGroupPosts(string accountId, string campaignId, string url, int count);
         IEnumerable<PublisherPostlistModel> ScrapeFriendPosts(string accountId, string campaignId, string url, int count);      
    }
}