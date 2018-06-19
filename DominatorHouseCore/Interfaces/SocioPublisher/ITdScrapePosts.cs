using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces.SocioPublisher
{
    public interface ITdScrapePosts
    {
        IEnumerable<PublisherPostlistModel> ScrapeUserPosts(string accountId,string fromUser, int count);
        IEnumerable<PublisherPostlistModel> ScrapeSearchPosts(string accountId, string keyword, int count);
    }
}