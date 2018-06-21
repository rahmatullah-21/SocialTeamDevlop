using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Request;
using HtmlAgilityPack;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{  
    public class RssFeedUtilities
    {
        public async Task RssFeedFetchMethod(string feedUrl, string feedTemplate,string campaignId)
        {
            try
            {
                var httpHelper = new HttpHelper();
                var htmlResponse = await httpHelper.GetRequestAsync(feedUrl,new CancellationToken());
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
                    select new PublisherPostlistModel
                    {
                        MediaList = new ObservableCollection<string>(),
                        CampaignId = campaignId,
                        CreatedTime = DateTime.Now,
                        ExpiredTime = DateTime.Now.AddYears(1),
                        PostId = Utilities.GetGuid(),
                        PostCategory = PostCategory.OrdinaryPost,
                        PostQueuedStatus = PostQueuedStatus.Pending,
                        PostRunningStatus = PostRunningStatus.Active,
                        PostSource = PostSource.RssFeedPost,
                        PostDescription = feedTemplate.Replace("[FeedTitle]", title)
                            .Replace("[FeedDescription]", description)
                            .Replace("[FeedUrl]", link)
                            .Replace("[FeedPublishedDate]", pubDate),
                        ShareUrl = link
                    }).ToList();

                PostlistFileManager.AddRange(campaignId, postlists);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static string RemoveCdata(string node) => node?.Replace("<![CDATA[", "").Replace("]]>", "") ?? string.Empty;
    }
}