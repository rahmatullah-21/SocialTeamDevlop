using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using HtmlAgilityPack;

namespace DominatorHouseCore.Utility
{
    public class ImageExtracter
    {
        public static async Task<IEnumerable<string>> ExtractImageUrls(string url, bool isBackgroundImageNeed = false)
        {
            var webClient = new WebClient();
            var pageResult = await webClient.DownloadStringTaskAsync(new Uri(url));
            var imageUrl = new List<string>();
            var htmlDocument = new HtmlDocument
            {
                OptionAutoCloseOnEnd = true,
                OptionCheckSyntax = false,
                OptionFixNestedTags = true
            };

            htmlDocument.LoadHtml(pageResult);
            var htmlNodeCollection = htmlDocument.DocumentNode.SelectNodes("//img[@src]");

            if (htmlNodeCollection != null)
                imageUrl.AddRange(RemoveInvalidUrls(htmlNodeCollection.Select(node => node.Attributes["src"].Value)));

            if (!isBackgroundImageNeed)
                return imageUrl;

            using (var enumerator = htmlDocument.DocumentNode.Descendants().Where(d =>
            {
                if (d.Attributes.Contains("style"))
                    return d.Attributes["style"].Value.Contains("background:url");
                return false;
            }).ToList().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    var input = enumerator.Current?.Attributes["style"].Value;
                    var regex = new Regex(".*?background:url\\('?(?<bgpath>.*)'?\\).*?", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace);
                    if (input != null && regex.IsMatch(input))
                        imageUrl.Add(regex.Match(input).Groups["bgpath"].Value);
                }
            }
            return imageUrl;
        }

        public static IEnumerable<string> RemoveInvalidUrls(IEnumerable<string> urls)
        {
            var validUrls = new List<string>();
            urls.ToList().ForEach(x =>
            {
                if (IsValidUrl(x))
                    validUrls.Add(DecodeHtml(x));
            });
            return validUrls;
        }

        public static string DecodeHtml(string text)
            => HttpUtility.HtmlDecode(text);

        public static bool IsValidUrl(string sourceUrl)
        {
            sourceUrl = HttpUtility.UrlPathEncode(sourceUrl);
            return CheckUrlValid(sourceUrl);
        }

        public static bool CheckUrlValid(string source)
        {
            Uri result;
            if (Uri.TryCreate(source, UriKind.Absolute, out result))
                return result.Scheme == Uri.UriSchemeHttp | result.Scheme == Uri.UriSchemeHttps;
            return false;
        }





    }
}