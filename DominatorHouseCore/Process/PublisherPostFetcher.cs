using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;

namespace DominatorHouseCore.Process
{
    public class PublisherPostFetcher
    {
        /// <summary>
        /// To keep the cancellation token for the campaign to avoid scraping after deleted the campaign
        /// </summary>
        public static ConcurrentDictionary<string, CancellationTokenSource> FetchingCampaignsCancellationToken { get; set; } = new ConcurrentDictionary<string, CancellationTokenSource>();

        /// <summary>
        /// Campaign Id which are running under RSS or Monitor Folder
        /// </summary>
        public static SortedSet<string> JobFetcherId { get; set; } = new SortedSet<string>();

        /// <summary>
        /// To Start fetching the post from Scrape Post, Share Post(only for Facebook), Rss and Monitor folder
        /// </summary>
        public void StartFetchingPostData()
        {
            // Get the post fetch details from bin file <see cref="ConstantVariable.GetPublisherPostFetchFile" /> other than normal post
            var getFetchDetails =
                GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                    .GetPublisherPostFetchFile).Where(x => x.PostSource != PostSource.NormalPost);

            // Iterate the post fetch detail
            getFetchDetails.ForEach(postFetchModel =>
            {
                // Register the campaign its running from current fetcher with respective post source and get the cancellation token
                var cancellationTokenSource = RegisterPostFetcher(postFetchModel.CampaignId, postFetchModel.PostSource);

                // Call the fetch methods with passing respective cancellation source
                FetchPosts(postFetchModel, cancellationTokenSource);
            });
        }

        /// <summary>
        /// To receive the campaign Id for give post source and campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource"><see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource"/></param>
        /// <returns></returns>
        public static string GetCampaignFetcherId(string campaignId, PostSource postSource)
            => $"{campaignId}-{postSource.ToString()}-PostFetcher";

        /// <summary>
        /// Start fetching the post by using campaing Id, Its used in while saving the campaign and cloned campaigns
        /// </summary>
        /// <param name="campaignId">campaign Id</param>
        public void FetchPostsForCampaign(string campaignId)
        {
            try
            {
                // Collect the respective Fetcher model
                var postFetchModels = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                      .GetPublisherPostFetchFile).Where(x => x.CampaignId == campaignId && x.PostSource != PostSource.NormalPost);

                // Call with Task factory
                ThreadFactory.Instance.Start(() =>
                {
                    // Iterate the post fetcher, because for a same campaign we may have to run RSS , Monitor Folder
                    postFetchModels.ForEach(postFetchModel =>
                    {
                        // Register the campaign its running from current fetcher with respective post source and get the cancellation token
                        var cancellationTokenSource = RegisterPostFetcher(campaignId, postFetchModel.PostSource);

                        // Call the fetch methods with passing respective cancellation source
                        FetchPosts(postFetchModel, cancellationTokenSource);
                    });
                });
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Register the campaign with their running cancellation token per every post source
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource"><see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource"/></param>
        /// <returns></returns>
        private CancellationTokenSource RegisterPostFetcher(string campaignId, PostSource postSource)
        {
            // Get new cancellation token
            var cancellationTokenSource = new CancellationTokenSource();

            // Get the unique fetcher name, its generated from campaign Id and Post source
            var fetcherName = GetCampaignFetcherId(campaignId, postSource);

            // Its already present, update the new Cancellation token source. Otherwise add new cancellation along with fetcher name
            if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherName))
            {
                // Register fercher name with cancellation token
                cancellationTokenSource = FetchingCampaignsCancellationToken.GetOrAdd(fetcherName, cancellationTokenSource);
            }
            else
            {
                // If, fetcher already prensent, stop all running instance
                StopFetchingPosts(campaignId, postSource);

                // Update cancellation token source along with fetcher name
                FetchingCampaignsCancellationToken.AddOrUpdate(fetcherName, cancellationTokenSource, (fetcher, oldvalue) =>
                {
                    if (oldvalue == null)
                        throw new ArgumentNullException(nameof(oldvalue));

                    oldvalue = cancellationTokenSource;
                    return oldvalue;
                });
            }

            return cancellationTokenSource;
        }

        /// <summary>
        /// Stop post fetcher by using campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void StopFetchingPostsByCampaignId(string campaignId)
        {
            try
            {
                // Get the fetcher details from the campaign ID
                var getFetchDetails =
                       GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                           .GetPublisherPostFetchFile).Where(x => x.PostSource != PostSource.NormalPost && x.CampaignId == campaignId);

                // Iterate all fetcher from list
                getFetchDetails.ForEach(fetchModel =>
                {
                    // Call stop fetching 
                    StopFetchingPosts(campaignId, fetchModel.PostSource);
                });

                // Delete all fetcher
                GenericFileManager.Delete<PublisherPostFetchModel>(x => x.CampaignId == campaignId, ConstantVariable
                    .GetPublisherPostFetchFile);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Stop running post fetcher details
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="postSource"><see cref=" DominatorHouseCore.Enums.SocioPublisher.PostSource"/></param>
        public static void StopFetchingPosts(string campaignId, PostSource postSource)
        {
            try
            {
                // Get the unique fetcher name, its generated from campaign Id and Post source
                var fetcherCampaignId = GetCampaignFetcherId(campaignId, postSource);

                // Its not present in running fetcher list, simply return
                if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherCampaignId))
                    return;

                // Gather all Rss and monitor folder fetcher Id
                var relatedPostFectcher = JobFetcherId.Where(x => x.Contains(campaignId));

                // Remove all post fetcher details
                relatedPostFectcher.ForEach(JobManager.RemoveJob);

                // Remove Post fetcher Id from Sorted set
                JobFetcherId.RemoveWhere(x => x.Contains(campaignId));

                // Get the cancellation of fetcher
                var cancellationToken = FetchingCampaignsCancellationToken[fetcherCampaignId];

                //Cancel the operation which is running
                cancellationToken.Cancel();
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Start fetching post for all Scarpe post(Facebook,Twitter,Pinterest),Share post(Facebook), Rss and Monitor Folder
        /// </summary>
        /// <param name="publisherPostFetchModel"><see cref=""/></param>
        /// <param name="cancellationTokenSource"></param>
        public void FetchPosts(PublisherPostFetchModel publisherPostFetchModel, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (publisherPostFetchModel.PostSource == PostSource.NormalPost)
                    return;

                //var generaldata = GenericFileManager.GetModuleDetails<GeneralModel>
                //    (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                //    .FirstOrDefault(x => x.CampaignId == publisherPostFetchModel.CampaignId) ?? new GeneralModel();


                // Check whether campaign expired or not
                if (publisherPostFetchModel.ExpireDate != null && publisherPostFetchModel.ExpireDate < DateTime.Now)
                {
                    GlobusLogHelper.log.Info(
                        $"{publisherPostFetchModel.CampaignName} expired on {publisherPostFetchModel.ExpireDate}");
                    return;
                }

                dynamic postFetchDetails = null;

                // Collect neccessary details for fetching and assign to dynamic type variable
                switch (publisherPostFetchModel.PostSource)
                {
                    case PostSource.SharePost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<SharePostModel>(publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.ScrapedPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ScrapePostModel>(publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.RssFeedPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ObservableCollection<PublisherRssFeedModel>>(publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.MonitorFolderPost:
                        postFetchDetails =
                            JsonConvert.DeserializeObject<ObservableCollection<PublisherMonitorFolderModel>>(publisherPostFetchModel.PostDetailsWithFilters);
                        break;
                    case PostSource.NormalPost:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                // get the post scraper object for Rss feed and monitor folder
                var postScraper = PublisherInitialize.GetPublisherLibrary(SocialNetworks.Social).GetPublisherCoreFactory()
                    .PostScraper.GetPostScraperLibrary();

                // Call the respective post scraper methods
                switch (publisherPostFetchModel.PostSource)
                {
                    case PostSource.RssFeedPost:
                        // Get the proper name for monitor job process
                        var jobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}";

                        var currentCampaignstatus = PublisherInitialize.GetInstance.GetSavedCampaigns().FirstOrDefault(x => x.CampaignId == publisherPostFetchModel.CampaignId);
                        if (currentCampaignstatus?.Status != PublisherCampaignStatus.Active)
                            return;
                        // Register to sorted set
                        JobFetcherId.Add(jobName);
                        // Add the Job for Rss feed 
                        JobManager.AddJob(() =>
                        {
                            // Call the Rss feed fetcher
                            postScraper.ScrapeRssPosts(publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource, publisherPostFetchModel.MaximumPostLimitToStore, publisherPostFetchModel.CampaignName);
                        }, s => s.WithName(jobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.MonitorFolderPost:
                        // Get the proper name for monitor job process
                        var monitorJobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.MonitorFolderPost.ToString()}";

                        // Register to sorted set
                        JobFetcherId.Add(monitorJobName);

                        // Add the Job for Monitor Folder
                        JobManager.AddJob(() =>
                        {
                            // Call the Monitor folder fetcher
                            postScraper.FetchMonitorFoldersPosts(publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource, publisherPostFetchModel.MaximumPostLimitToStore, publisherPostFetchModel.CampaignName);
                        }, s => s.WithName(monitorJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.NormalPost:
                        break;
                    default:
                        // Iterate the selected destination Id
                        publisherPostFetchModel.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            // Get the details of Destination Id
                            var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                            // Itereate the destination
                            destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                            {
                                if (SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                {
                                    // Get the proper library for publisher
                                    var networkPostScraper = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key).GetPublisherCoreFactory()
                                   .PostScraper.GetPostScraperLibrary();

                                    try
                                    {
                                        if (publisherPostFetchModel.PostSource == PostSource.SharePost)
                                        {
                                            // Call Share post from facebook
                                            if (networkWithAccount.Key == SocialNetworks.Facebook)
                                            {
                                                var scrapeJobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.SharePost.ToString()}";

                                                // Register to sorted set
                                                JobFetcherId.Add(scrapeJobName);

                                                // Add the Job for scrape 
                                                JobManager.AddJob(() =>
                                                {
                                                    networkPostScraper.ScrapeFdPagePostUrl(networkWithAccount.Value, publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource, publisherPostFetchModel.ScrapeCount);

                                                }, s => s.WithName(scrapeJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                            }
                                        }
                                        // Scarpe the posts from Facebook, Twitter, Pinterest
                                        else if (publisherPostFetchModel.PostSource == PostSource.ScrapedPost)
                                        {
                                            // Get the proper name for scrape job process
                                            var scrapeJobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.ScrapedPost.ToString()}";

                                            // Register to sorted set
                                            JobFetcherId.Add(scrapeJobName);

                                            // Add the Job for scrape 
                                            JobManager.AddJob(() =>
                                            {
                                                networkPostScraper.ScrapePosts(networkWithAccount.Value,
                                                    publisherPostFetchModel.CampaignId, postFetchDetails,
                                                    cancellationTokenSource, publisherPostFetchModel.ScrapeCount);
                                            }, s => s.WithName(scrapeJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                                        }

                                    }
                                    catch (OperationCanceledException ex)
                                    {
                                        ex.DebugLog("Cancellation Requested!");
                                    }
                                    catch (AggregateException ae)
                                    {
                                        foreach (var e in ae.InnerExceptions)
                                        {
                                            if (e is TaskCanceledException || e is OperationCanceledException)
                                                e.DebugLog("Cancellation requested before task completion!");
                                            else
                                                e.DebugLog(e.StackTrace + e.Message);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ex.DebugLog();
                                    }
                                }
                            });
                        });
                        break;
                }
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}