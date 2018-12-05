using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using HtmlAgilityPack;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Patterns;

namespace DominatorHouseCore.Utility
{
    public class RssFeedUtilities
    {
        /// <summary>
        /// Fetching Post Details from Rss Feed
        /// </summary>
        /// <param name="feedUrl">Feed Url</param>
        /// <param name="feedTemplate">Feed Template</param>
        /// <param name="postDetailsModel">Post Details-> from here we are getting post's general settings</param>
        /// <param name="campaignId">Campaign Id</param>
        /// <param name="cancellationTokenSource">Cancellation Token</param>
        /// <param name="maximumPostLimitToStore">Specify Maximum Count to store for a campaign</param>
        /// <param name="campaignName">Campaign Name</param>
        /// <returns></returns>
        //public async Task RssFeedFetchMethod(string feedUrl, string feedTemplate, PostDetailsModel postDetailsModel, string campaignId, CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        //{
        //    try
        //    {
        //        // Get all Campaign Details
        //        var campaignDetails = PostlistFileManager.GetAll(campaignId);

        //        // Get Rss Campaign Details
        //        var postdetails = campaignDetails.Where(x => x.PostSource == PostSource.RssFeedPost).Select(x => x.ShareUrl).ToList();

        //        //// Requesting Rss feed urls
        //        //var httpHelper = new HttpHelper();
        //        //var htmlResponse = await httpHelper.GetRequestAsync(feedUrl, cancellationTokenSource.Token);

        //        var client = new WebClient
        //        {
        //            Headers =
        //            {
        //                ["User-Agent"] =
        //                "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36"
        //            }
        //        };
        //        ServicePointManager.Expect100Continue = true;
        //        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //        var pageSource =await client.DownloadDataTaskAsync(feedUrl);

        //        var htmlResponse = Encoding.UTF8.GetString(pageSource);

        //        var htmlDoc = new HtmlDocument();
        //        htmlDoc.LoadHtml(htmlResponse);
        //        var postItems = htmlDoc.DocumentNode.Descendants("item");

        //        DateTime? expireDate = null;
        //        // Calculate Expire date of the post
        //        if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
        //            expireDate = postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate;

        //        // Scrape the posts from Http Response
        //        #region Http Response
        //        var postlists = (from node in postItems
        //                         let innerHtml = node.InnerHtml
        //                         let title = RemoveCdata(node.Element("title").InnerHtml)
        //                         let description = RemoveCdata(node.Element("description").InnerHtml)
        //                         let link = RemoveCdata(node.Element("link").NextSibling.InnerText)
        //                         let pubDate = RemoveCdata(node.Element("pubdate").InnerHtml)
        //                         let url = RemoveCdata(node.Element("url")?.InnerHtml)
        //                         where !postdetails.Contains(link)
        //                         select new PublisherPostlistModel
        //                         {
        //                             MediaList = new ObservableCollection<string>(postDetailsModel.MediaViewer.MediaList),
        //                             CampaignId = campaignId,
        //                             CreatedTime = DateTime.Now,
        //                             ExpiredTime = expireDate,
        //                             PostId = Utilities.GetGuid(),
        //                             PostCategory = PostCategory.OrdinaryPost,
        //                             PostQueuedStatus = PostQueuedStatus.Pending,
        //                             PostRunningStatus = PostRunningStatus.Active,
        //                             PostSource = PostSource.RssFeedPost,
        //                             PostDescription = WebUtility.HtmlDecode(feedTemplate.Replace("[FeedTitle]", title)
        //                                 .Replace("[FeedDescription]", description)
        //                                 .Replace("[FeedUrl]", link)
        //                                 .Replace("[FeedPublishedDate]", pubDate)),
        //                             ShareUrl = link,
        //                             PdSourceUrl = postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", link),
        //                             PublisherInstagramTitle = postDetailsModel.PublisherInstagramTitle.Replace("[FeedTitle]", title),
        //                             GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
        //                             FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
        //                             GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
        //                             TdPostSettings = postDetailsModel.PublisherPostSettings.TdPostSettings,
        //                             LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
        //                             TumberPostSettings = postDetailsModel.PublisherPostSettings.TumberPostSettings,
        //                             RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
        //                             FdSellLocation = postDetailsModel.FdSellLocation,
        //                             FdSellPrice = postDetailsModel.FdSellPrice,
        //                             FdSellProductTitle = postDetailsModel.FdSellProductTitle,
        //                             IsFdSellPost = postDetailsModel.IsFdSellPost,
        //                             PublisherPostSettings = postDetailsModel.PublisherPostSettings
        //                         }).ToList();

        //        #endregion

        //        // If Readd Checked, then readd the same posts in selected/given times
        //        #region Readd
        //        if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
        //        {
        //            var duplicatedPostlist = new List<PublisherPostlistModel>();

        //            foreach (var post in postlists)
        //            {
        //                try
        //                {
        //                    for (var readdIndex = 1; readdIndex < postDetailsModel.PublisherPostSettings.GeneralPostSettings.ReaddCount; readdIndex++)
        //                    {
        //                        var newPost = post.DeepClone();
        //                        newPost.PostId = Utilities.GetGuid();
        //                        duplicatedPostlist.Add(newPost);
        //                    }
        //                }
        //                catch (Exception ex)
        //                {
        //                    ex.DebugLog();
        //                }
        //            }

        //            postlists.AddRange(duplicatedPostlist);
        //        }
        //        #endregion

        //        // Check whether cancellation token arised or not
        //        cancellationTokenSource.Token.ThrowIfCancellationRequested();

        //        campaignDetails = PostlistFileManager.GetAll(campaignId);

        //        var postCount = maximumPostLimitToStore - campaignDetails.Count;

        //        // Checking whether maximum count reached or not
        //        #region Add to bin Files
        //        if (postCount > 0)
        //        {
        //            var neededPostLists = postlists.Take(postCount).ToList();
        //            PostlistFileManager.AddRange(campaignId, neededPostLists);
        //            var publisherInitialize = PublisherInitialize.GetInstance;
        //            publisherInitialize.UpdatePostCounts(campaignId);
        //        }
        //        else
        //        {
        //            // Inform the maximum post has reached via Toaster notification
        //            ToasterNotification.ShowInfomation($"Maximum Postlist Reached: {campaignName} already have {maximumPostLimitToStore}+ posts in postlist!");

        //        }
        //        #endregion
        //    }
        //    catch (OperationCanceledException ex)
        //    {
        //        ex.DebugLog("Cancellation Requested!");
        //    }
        //    catch (AggregateException ae)
        //    {
        //        foreach (var e in ae.InnerExceptions)
        //        {
        //            if (e is TaskCanceledException || e is OperationCanceledException)
        //                e.DebugLog("Cancellation Requested!");
        //            else
        //                e.DebugLog(e.StackTrace + e.Message);
        //        }
        //    }
        //    catch (ArgumentNullException ex)
        //    {
        //        ex.DebugLog();
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.DebugLog();
        //    }
        //}


        public async Task RssFeedFetchMethod(string feedUrl, string feedTemplate, PostDetailsModel postDetailsModel, string campaignId, CancellationTokenSource cancellationTokenSource, int maximumPostLimitToStore, string campaignName)
        {
            try
            {
                // Get all Campaign Details
                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                // Get Rss Campaign Details
                var postdetails = campaignDetails.Where(x => x.PostSource == PostSource.RssFeedPost).Select(x => x.ShareUrl).ToList();

                //// Requesting Rss feed urls
                //var httpHelper = new HttpHelper();
                //var htmlResponse = await httpHelper.GetRequestAsync(feedUrl, cancellationTokenSource.Token);

                var client = new WebClient
                {
                    Headers =
                    {
                        ["User-Agent"] =
                        "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/67.0.3396.99 Safari/537.36"
                    }
                };
                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var pageSource = await client.DownloadDataTaskAsync(feedUrl);

                var htmlResponse = Encoding.UTF8.GetString(pageSource);

                // ReSharper disable once RedundantAssignment
                var postlists = new List<PublisherPostlistModel>();

                var htmlDoc = new HtmlDocument();

                htmlResponse = WebUtility.HtmlDecode(htmlResponse);

                htmlDoc.LoadHtml(htmlResponse);

                DateTime? expireDate = null;
                // Calculate Expire date of the post
                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate)
                    expireDate = postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate;


                var postItems = htmlDoc.DocumentNode.Descendants("item");

                if (postItems.Count() == 0)
                {
                    postItems = htmlDoc.DocumentNode.Descendants("entry");

                    // Scrape the posts from Http Response
                    #region Http Response
                    postlists = (from node in postItems
                                 let innerHtml = node.InnerHtml
                                 let title = RemoveCdata(node.Element("title").InnerHtml)
                                 let content = WebUtility.HtmlDecode(RemoveCdata(node.Element("content").InnerHtml))
                                 let descriptionDetails = Regex.Matches(content, "(<p|<p>)(.*?)(<h2|<div)", RegexOptions.Singleline)
                                 let descriptionDetailsNew = descriptionDetails == null || descriptionDetails.Count == 0 ? null : Regex.Matches(content, "(<p|<p>)(.*)(</p>)", RegexOptions.Singleline)
                                 let description = descriptionDetailsNew == null || descriptionDetailsNew.Count == 0 ? String.Empty : descriptionDetailsNew[0].Groups[0].ToString()
                                 let link = RemoveCdata(node.Element("link").InnerText)
                                 let newLink = string.IsNullOrEmpty(link) ? Utilities.GetBetween(node.Element("link").OuterHtml, "href=\"", "\"") : link
                                 let pubDate = RemoveCdata(node.Element("published").InnerHtml)
                                 where !postdetails.Contains(link)
                                 select new PublisherPostlistModel
                                 {
                                     MediaList = new ObservableCollection<string>(postDetailsModel.MediaViewer.MediaList),
                                     CampaignId = campaignId,
                                     CreatedTime = DateTime.Now,
                                     ExpiredTime = expireDate,
                                     PostId = Utilities.GetGuid(),
                                     PostCategory = PostCategory.OrdinaryPost,
                                     PostQueuedStatus = PostQueuedStatus.Pending,
                                     PostRunningStatus = PostRunningStatus.Active,
                                     PostSource = PostSource.RssFeedPost,
                                     PostDescription = WebUtility.HtmlDecode(feedTemplate.Replace("[FeedTitle]", title)
                                         .Replace("[FeedDescription]", HtmlParseUtility.GetAllInnerTextFromTags(description))
                                         .Replace("[FeedUrl]", newLink)
                                         .Replace("[FeedPublishedDate]", pubDate)),
                                     ShareUrl = newLink,
                                     PdSourceUrl = postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", newLink),
                                     PublisherInstagramTitle = postDetailsModel.PublisherInstagramTitle.Replace("[FeedTitle]", title),
                                     GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
                                     FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
                                     GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
                                     TdPostSettings = postDetailsModel.PublisherPostSettings.TdPostSettings,
                                     LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
                                     TumberPostSettings = postDetailsModel.PublisherPostSettings.TumberPostSettings,
                                     RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
                                     FdSellLocation = postDetailsModel.FdSellLocation,
                                     FdSellPrice = postDetailsModel.FdSellPrice,
                                     FdSellProductTitle = postDetailsModel.FdSellProductTitle,
                                     IsFdSellPost = postDetailsModel.IsFdSellPost,
                                     PublisherPostSettings = postDetailsModel.PublisherPostSettings
                                 }).ToList();

                    #endregion

                }
                else
                {
                    // Scrape the posts from Http Response
                    #region Http Response
                    postlists = (from node in postItems
                                 let innerHtml = node.InnerHtml
                                 let title = RemoveCdata(node.Element("title").InnerHtml)
                                 let description = RemoveCdata(node.Element("description").InnerHtml)
                                 let link = RemoveCdata(node.Element("link").NextSibling.InnerText)
                                 let pubDate = RemoveCdata(node.Element("pubdate").InnerHtml)
                                 let url = RemoveCdata(node.Element("url")?.InnerHtml)
                                 where !postdetails.Contains(link)
                                 select new PublisherPostlistModel
                                 {
                                     MediaList = new ObservableCollection<string>(postDetailsModel.MediaViewer.MediaList),
                                     CampaignId = campaignId,
                                     CreatedTime = DateTime.Now,
                                     ExpiredTime = expireDate,
                                     PostId = Utilities.GetGuid(),
                                     PostCategory = PostCategory.OrdinaryPost,
                                     PostQueuedStatus = PostQueuedStatus.Pending,
                                     PostRunningStatus = PostRunningStatus.Active,
                                     PostSource = PostSource.RssFeedPost,
                                     PostDescription = WebUtility.HtmlDecode(feedTemplate.Replace("[FeedTitle]", title)
                                         .Replace("[FeedDescription]", description)
                                         .Replace("[FeedUrl]", link)
                                         .Replace("[FeedPublishedDate]", pubDate)),
                                     ShareUrl = link,
                                     PdSourceUrl = postDetailsModel.PdSourceUrl.Replace("[FeedUrl]", link),
                                     PublisherInstagramTitle = postDetailsModel.PublisherInstagramTitle.Replace("[FeedTitle]", title),
                                     GeneralPostSettings = postDetailsModel.PublisherPostSettings.GeneralPostSettings,
                                     FdPostSettings = postDetailsModel.PublisherPostSettings.FdPostSettings,
                                     GdPostSettings = postDetailsModel.PublisherPostSettings.GdPostSettings,
                                     TdPostSettings = postDetailsModel.PublisherPostSettings.TdPostSettings,
                                     LdPostSettings = postDetailsModel.PublisherPostSettings.LdPostSettings,
                                     TumberPostSettings = postDetailsModel.PublisherPostSettings.TumberPostSettings,
                                     RedditPostSetting = postDetailsModel.PublisherPostSettings.RedditPostSetting,
                                     FdSellLocation = postDetailsModel.FdSellLocation,
                                     FdSellPrice = postDetailsModel.FdSellPrice,
                                     FdSellProductTitle = postDetailsModel.FdSellProductTitle,
                                     IsFdSellPost = postDetailsModel.IsFdSellPost,
                                     PublisherPostSettings = postDetailsModel.PublisherPostSettings
                                 }).ToList();

                    #endregion
                }



                // If Readd Checked, then readd the same posts in selected/given times
                #region Readd
                if (postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsReaddCount)
                {
                    var duplicatedPostlist = new List<PublisherPostlistModel>();

                    foreach (var post in postlists)
                    {
                        try
                        {
                            for (var readdIndex = 1; readdIndex < postDetailsModel.PublisherPostSettings.GeneralPostSettings.ReaddCount; readdIndex++)
                            {
                                var newPost = post.DeepClone();
                                newPost.PostId = Utilities.GetGuid();
                                duplicatedPostlist.Add(newPost);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    postlists.AddRange(duplicatedPostlist);
                }
                #endregion

                // Check whether cancellation token arised or not
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                campaignDetails = PostlistFileManager.GetAll(campaignId);

                var postCount = maximumPostLimitToStore - campaignDetails.Count;

                // Checking whether maximum count reached or not
                #region Add to bin Files
                if (postCount > 0)
                {
                    var neededPostLists = postlists.Take(postCount).ToList();
                    PostlistFileManager.AddRange(campaignId, neededPostLists);
                    var publisherInitialize = PublisherInitialize.GetInstance;
                    publisherInitialize.UpdatePostCounts(campaignId);
                }
                else
                {
                    // Inform the maximum post has reached via Toaster notification
                    ToasterNotification.ShowInfomation($"Maximum Postlist Reached: {campaignName} already have {maximumPostLimitToStore}+ posts in postlist!");

                }
                #endregion
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
                        e.DebugLog("Cancellation Requested!");
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

        /// <summary>
        /// Remove Cdata Details from Rss Feed
        /// </summary>
        /// <param name="node">Rss node Details</param>
        /// <returns></returns>
        private static string RemoveCdata(string node) => node?.Replace("<![CDATA[", "").Replace("]]>", "") ?? string.Empty;
    }
}