using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;
using ToastNotifications.Messages;

namespace DominatorHouseCore.Process
{
    public class PublisherPostFetcher
    {

        public ConcurrentDictionary<string, CancellationTokenSource> FetchingCampaignId { get; set; } = new ConcurrentDictionary<string, CancellationTokenSource>();


        public void StartFetchingPostData()
        {
            var getFetchDetails =
                GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                    .GetPublisherPostFetchFile);

            getFetchDetails.ForEach(FetchPosts);
        }

        public void FetchPostsForCampaign(string campaignId)
        {
            var postFetchModels = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).Where(x => x.CampaignId == campaignId && x.PostSource != PostSource.NormalPost);

            ThreadFactory.Instance.Start(() => { postFetchModels.ForEach(FetchPosts); });
        }

        public void StopFetchingPosts(string campaignId)
        {
            if (!FetchingCampaignId.ContainsKey(campaignId))
                return;

            var cancellationToken = FetchingCampaignId[campaignId];
            cancellationToken.Cancel();
        }

        public void FetchPosts(PublisherPostFetchModel publisherPostFetchModel)
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
                    ToasterNotification.ShowInfomation(
                        $"{publisherPostFetchModel.CampaignName} have more than {publisherPostFetchModel.MaximumPostLimitToStore} posts in their postlist");
                    //GlobusLogHelper.log.Info(
                    //    $"{publisherPostFetchModel.CampaignName} have more than {publisherPostFetchModel.MaximumPostLimitToStore} posts in their postlist. Can't fetch new posts!");
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
                        JobManager.AddJob(() =>
                         {
                             postScraper.ScrapeRssPosts(publisherPostFetchModel.CampaignId, postFetchDetails);
                         }, s => s.WithName($"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}").ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
                        break;
                    case PostSource.MonitorFolderPost:
                        JobManager.AddJob(() =>
                        {
                            postScraper.FetchMonitorFoldersPosts(publisherPostFetchModel.CampaignId, postFetchDetails);
                        }, s => s.WithName($"{publisherPostFetchModel.CampaignId}-{PostSource.RssFeedPost.ToString()}").ToRunOnceAt(DateTime.Now.AddSeconds(2)).AndEvery(publisherPostFetchModel.DelayForNext).Minutes());
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

                                    if (publisherPostFetchModel.PostSource == PostSource.SharePost)
                                    {
                                        if (networkWithAccount.Key == SocialNetworks.Facebook)
                                            networkPostScraper.ScrapeFdPagePostUrl(networkWithAccount.Value, publisherPostFetchModel.CampaignId, postFetchDetails);
                                    }
                                    else if (publisherPostFetchModel.PostSource == PostSource.ScrapedPost)
                                        networkPostScraper.ScrapePosts(networkWithAccount.Value, publisherPostFetchModel.CampaignId, postFetchDetails);
                                }
                            });
                        });
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}