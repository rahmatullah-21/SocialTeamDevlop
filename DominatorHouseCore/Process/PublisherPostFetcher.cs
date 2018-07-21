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
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;

namespace DominatorHouseCore.Process
{
    public class PublisherPostFetcher
    {
        public static ConcurrentDictionary<string, CancellationTokenSource> FetchingCampaignsCancellationToken { get; set; } = new ConcurrentDictionary<string, CancellationTokenSource>();

        public static SortedSet<string> RssMonitorFetcherId { get; set; } = new SortedSet<string>();

        public void StartFetchingPostData()
        {
            var getFetchDetails =
                GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                    .GetPublisherPostFetchFile).Where(x=> x.PostSource != PostSource.NormalPost);

            getFetchDetails.ForEach(postFetchModel =>
            {
                var cancellationTokenSource = RegisterPostFetcher(postFetchModel.CampaignId, postFetchModel.PostSource);
                FetchPosts(postFetchModel, cancellationTokenSource);
            });
        }

        public static string GetCampaignFetcherId(string campaignId, PostSource postSource) => $"{campaignId}-{postSource.ToString()}-PostFetcher";

        public void FetchPostsForCampaign(string campaignId)
        {
            try
            {
                var postFetchModels = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                      .GetPublisherPostFetchFile).Where(x => x.CampaignId == campaignId && x.PostSource != PostSource.NormalPost);

                ThreadFactory.Instance.Start(() =>
                {
                    postFetchModels.ForEach(postFetchModel =>
                    {
                        var cancellationTokenSource = RegisterPostFetcher(campaignId, postFetchModel.PostSource);
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

        private CancellationTokenSource RegisterPostFetcher(string campaignId , PostSource postSource)
        {
            var cancellationTokenSource = new CancellationTokenSource();

            var fetcherName = GetCampaignFetcherId(campaignId, postSource);

            if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherName))
            {
                cancellationTokenSource = FetchingCampaignsCancellationToken.GetOrAdd(fetcherName, cancellationTokenSource);
            }
            else
            {
                StopFetchingPosts(campaignId, postSource);

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

        public static void StopFetchingPostsByCampaignId(string campaignId)
        {
            try
            {
                var getFetchDetails =
                       GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                           .GetPublisherPostFetchFile).Where(x => x.PostSource != PostSource.NormalPost && x.CampaignId == campaignId);

                getFetchDetails.ForEach(fetchModel => { StopFetchingPosts(campaignId, fetchModel.PostSource); });
                
                GenericFileManager.Delete<PublisherPostFetchModel>(x => x.CampaignId == campaignId, ConstantVariable
                    .GetPublisherPostFetchFile);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void StopFetchingPosts(string campaignId , PostSource postSource)
        {
            try
            {                
                var fetcherCampaignId = GetCampaignFetcherId(campaignId, postSource);

                if (!FetchingCampaignsCancellationToken.ContainsKey(fetcherCampaignId))
                    return;

                var relatedPostFectcher = RssMonitorFetcherId.Where(x => x.Contains(campaignId));

                relatedPostFectcher.ForEach(JobManager.RemoveJob);

                RssMonitorFetcherId.RemoveWhere(x => x.Contains(campaignId));

                var cancellationToken = FetchingCampaignsCancellationToken[fetcherCampaignId];
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

        public void FetchPosts(PublisherPostFetchModel publisherPostFetchModel, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                if (publisherPostFetchModel.PostSource == PostSource.NormalPost)
                    return;

                // Check whether campaign expired or not
                if (publisherPostFetchModel.ExpireDate != null && publisherPostFetchModel.ExpireDate < DateTime.Now)
                {
                    GlobusLogHelper.log.Info(
                        $"{publisherPostFetchModel.CampaignName} expired on {publisherPostFetchModel.ExpireDate}");
                    return;
                }

                // Get all post lists and count 
                var alreadyPresentedCount = PostlistFileManager.GetAll(publisherPostFetchModel.CampaignId).Count;

                // Check already max posts has reached or not

                if (alreadyPresentedCount >= publisherPostFetchModel.MaximumPostLimitToStore)
                {
                    GlobusLogHelper.log.Info(
                        $"{publisherPostFetchModel.CampaignName} have more than {publisherPostFetchModel.MaximumPostLimitToStore} posts in their postlist. Can't fetch new posts!");
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
                        var jobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}";
                        RssMonitorFetcherId.Add(jobName);
                        JobManager.AddJob(() =>
                        {
                            postScraper.ScrapeRssPosts(publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource, publisherPostFetchModel.NotifyCount, publisherPostFetchModel.CampaignName);
                        }, s => s.WithName(jobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.MonitorFolderPost:
                        var monitorJobName = $"{publisherPostFetchModel.CampaignId}-{PostSource.MonitorFolderPost.ToString()}";
                        RssMonitorFetcherId.Add(monitorJobName);
                        JobManager.AddJob(() =>
                        {
                            postScraper.FetchMonitorFoldersPosts(publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource, publisherPostFetchModel.NotifyCount, publisherPostFetchModel.CampaignName);
                        }, s => s.WithName(monitorJobName).ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.NormalPost:
                        break;
                    default:
                        publisherPostFetchModel.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                            destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                            {
                                if (SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                {
                                    var networkPostScraper = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key).GetPublisherCoreFactory()
                                   .PostScraper.GetPostScraperLibrary();

                                    try
                                    {
                                        if (publisherPostFetchModel.PostSource == PostSource.SharePost)
                                        {
                                            if (networkWithAccount.Key == SocialNetworks.Facebook)
                                                networkPostScraper.ScrapeFdPagePostUrl(networkWithAccount.Value, publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource);
                                        }
                                        else if (publisherPostFetchModel.PostSource == PostSource.ScrapedPost)
                                            networkPostScraper.ScrapePosts(networkWithAccount.Value, publisherPostFetchModel.CampaignId, postFetchDetails, cancellationTokenSource);
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