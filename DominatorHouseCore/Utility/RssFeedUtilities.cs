using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using HtmlAgilityPack;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Patterns;

namespace DominatorHouseCore.Utility
{
    public class RssFeedUtilities
    {
        public async Task RssFeedFetchMethod(string feedUrl, string feedTemplate, PostDetailsModel postDetailsModel, string campaignId, CancellationTokenSource cancellationTokenSource,int notifyCount ,string campaignName)
        {
            try
            {
                var campaignDetails = PostlistFileManager.GetAll(campaignId);

                var postdetails =    campaignDetails.Where(x => x.PostSource == PostSource.RssFeedPost).Select(x => x.ShareUrl).ToList();

                var httpHelper = new HttpHelper();
                var htmlResponse = await httpHelper.GetRequestAsync(feedUrl, cancellationTokenSource.Token);
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(htmlResponse.Response);
                var postItems = htmlDoc.DocumentNode.Descendants("item");

                var postlists = (from node in postItems
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
                                     ExpiredTime = postDetailsModel.PublisherPostSettings.GeneralPostSettings.IsExpireDate ?
                                     postDetailsModel.PublisherPostSettings.GeneralPostSettings.ExpireDate
                                     : DateTime.Now.AddYears(2),
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
                                 }).ToList();


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
             
                cancellationTokenSource.Token.ThrowIfCancellationRequested();

                PostlistFileManager.AddRange(campaignId, postlists);
                var publisherInitialize = PublisherInitialize.GetInstance;
                publisherInitialize.UpdatePostCounts(campaignId);

              
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

        private static string RemoveCdata(string node) => node?.Replace("<![CDATA[", "").Replace("]]>", "") ?? string.Empty;
    }
}