using System.Collections.Generic;
using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces
{
    public abstract class PostScraper
    {

        #region Facebook

        public virtual IEnumerable<PublisherPostlistModel> FdScrapeOwnWallPosts(string accountId, string campaignId, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> FdScrapePagePosts(string accountId, string campaignId, string url, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> FdScrapeGroupPosts(string accountId, string campaignId, string url, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> FdScrapeFriendPosts(string accountId, string campaignId, string url, int count) => null;

        #endregion

        #region Pinterest

        public virtual IEnumerable<PublisherPostlistModel> PdScrapeBoardPosts(string accountId, string campaignId, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> PdScrapeUsersPosts(string accountId, string campaignId, string url, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> PdScrapeCategoryPosts(string accountId, string campaignId, string url, int count) => null;
        public virtual IEnumerable<PublisherPostlistModel> PdScrapeSearchPinPosts(string accountId, string campaignId, string keyword, int count) => null;

        #endregion

        #region Twitter

        public virtual IEnumerable<PublisherPostlistModel> TdScrapeUserPosts(string accountId, string campaignId, string fromUser, int count) => null;

        public virtual IEnumerable<PublisherPostlistModel> TdScrapeSearchPosts(string accountId, string campaignId, string keyword, int count) => null;

        #endregion

        #region Rss feed

        public IEnumerable<PublisherPostlistModel> ScrapeRssPosts(string feedUrl, string feedTemplate)
        {
            return new List<PublisherPostlistModel>();
        }

        #endregion

        #region MonitorFolder

        public IEnumerable<PublisherPostlistModel> FetchMonitorFoldersPosts(string folderUrl, string postTemplate)
        {
            return new List<PublisherPostlistModel>();
        }

        #endregion

    }
}