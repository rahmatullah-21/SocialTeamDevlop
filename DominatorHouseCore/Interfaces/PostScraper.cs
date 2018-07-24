using System.Collections.ObjectModel;
using System.Threading;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Interfaces
{
    public abstract class PostScraper
    {

        #region Post Scrapers

        /// <summary>
        /// To fetch the posts from selected destinations
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="scrapePostDetails"><see cref="ScrapePostModel"/>Neccessary given input for scrape Posts</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="count">Specifies how many need to scrape</param>
        public virtual void ScrapePosts(string accountId, string campaignId, ScrapePostModel scrapePostDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        { }

        #endregion

        #region Scrape page post url 

        /// <summary>
        /// To Scrape the post from facebook page
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="fdPagePostUrlScraperDetails">Scraping Details <see cref="SharePostModel"/></param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="count">Specifies how many need to scrape</param>
        // Note : Only for Facebook
        public virtual void ScrapeFdPagePostUrl(string accountId, string campaignId, SharePostModel fdPagePostUrlScraperDetails, CancellationTokenSource cancellationTokenSource, int count = 10)
        { }

        #endregion

        #region Rss feed

        /// <summary>
        /// Strating scraping Rss Post details
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="rssFeedModels"> Collection of <see cref="PublisherRssFeedModel"/> rss feed models.</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="maximumPostLimitToStore">Maximum post limit for the postlists</param>
        /// <param name="campaignName">Campaign Name</param>
        public void ScrapeRssPosts(string campaignId, ObservableCollection<PublisherRssFeedModel> rssFeedModels, CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            ThreadFactory.Instance.Start(() =>
            {
                //Create a object of Rss Feed Utilities
                var rssFeedUtilities = new RssFeedUtilities();
                rssFeedModels.ForEach(async x =>
                {
                    // Call the scraper methods for Rss Post Fetcing
                    await rssFeedUtilities.RssFeedFetchMethod(x.FeedUrl, x.FeedTemplate,x.PostDetailsModel, campaignId, cancellationTokenSource, maximumPostLimitToStore, campaignName);
                });               
            }, cancellationTokenSource.Token);
        }

        #endregion

        #region MonitorFolder

        /// <summary>
        /// 
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="monitorFolderModels">Collection of <see cref="PublisherMonitorFolderModel"/> monitor folder models.</param>
        /// <param name="cancellationTokenSource">Cancellation Token Source for stop the running task</param>
        /// <param name="maximumPostLimitToStore">Maximum post limit for the postlists</param>
        /// <param name="campaignName">Campaign Name</param>
        public void FetchMonitorFoldersPosts(string campaignId, ObservableCollection<PublisherMonitorFolderModel> monitorFolderModels ,CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            ThreadFactory.Instance.Start(() =>
            {
                //Create a object of Monitor Folder Utilities
                var monitorFolderUtilites = new MonitorFolderUtilites();
                monitorFolderModels.ForEach( x =>
                {
                    // Call the scraper methods for folders Post details
                    monitorFolderUtilites.GetFoldersFileDetails(x.FolderPath, campaignId, x.FolderTemplate,x.PostDetailsModel, cancellationTokenSource, maximumPostLimitToStore, campaignName);
                });
            }, cancellationTokenSource.Token);
        }

        #endregion

    }
}