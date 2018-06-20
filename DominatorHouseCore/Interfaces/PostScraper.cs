using System.Collections.Generic;
using System.Threading.Tasks;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

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

        public void ScrapeRssPosts(string campaignId, string feedUrl, string feedTemplate)
        {
            Task.Factory.StartNew(async () =>
            {
                var rssFeedUtilities = new RssFeedUtilities();
                await rssFeedUtilities.RssFeedFetchMethod(feedUrl, feedTemplate, campaignId);
            });
        }

        #endregion

        #region MonitorFolder

        public void FetchMonitorFoldersPosts(string campaignId, string folderUrl, string postTemplate)
        {
            Task.Factory.StartNew(() =>
            {
                var monitorFolderUtilites = new MonitorFolderUtilites();
                monitorFolderUtilites.GetFoldersFileDetails(folderUrl, campaignId, postTemplate);
            });
        }

        #endregion

    }
}