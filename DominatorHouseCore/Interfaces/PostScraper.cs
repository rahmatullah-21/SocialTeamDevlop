using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public abstract class PostScraper
    {

        #region Post Scrapers

        public virtual void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails,int count = 10)
        { }

        #endregion

        #region Scrape page post url 

        // Note : Only for Facebook
        public virtual void ScrapeFdPagePostUrl(string accountId, string campaignId, SharePostModel fdPagePostUrlScraperDetails, int count = 10)
        { }

        #endregion

        #region Rss feed

        public void ScrapeRssPosts(string campaignId, ObservableCollection<PublisherRssFeedModel> rssFeedModels)
        {
            Task.Factory.StartNew(() =>
            {
                var rssFeedUtilities = new RssFeedUtilities();
                rssFeedModels.ForEach(async x =>
                {
                    await rssFeedUtilities.RssFeedFetchMethod(x.FeedUrl, x.FeedTemplate, campaignId);
                });               
            });
        }

        #endregion

        #region MonitorFolder

        public void FetchMonitorFoldersPosts(string campaignId, ObservableCollection<PublisherMonitorFolderModel> monitorFolderModels )
        {
            Task.Factory.StartNew(() =>
            {
                var monitorFolderUtilites = new MonitorFolderUtilites();
                monitorFolderModels.ForEach( x =>
                {
                     monitorFolderUtilites.GetFoldersFileDetails(x.FolderPath, campaignId, x.FolderTemplate );
                });
            });
        }

        #endregion

    }
}