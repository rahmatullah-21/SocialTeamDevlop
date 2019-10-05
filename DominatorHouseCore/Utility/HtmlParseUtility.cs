using System;
using System.Collections.Generic;
using HtmlAgilityPack;

namespace DominatorHouseCore.Utility
{
    //public static class HtmlParseUtility
    //{
    //    public static readonly string NotFound = "Not Found";

    //    public static string GetInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
    //        string attributeValue)
    //    {
    //        var htmlDoc = new HtmlDocument();
    //        htmlDoc.LoadHtml(pageSource);
    //        return htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerHtml.ToString();
    //    }

    //    public static string GetAttributeValueFromId(string pageSource, string idValue, string attributeName)
    //    {
    //        var htmlDoc = new HtmlDocument();
    //        htmlDoc.LoadHtml(pageSource);
    //        return htmlDoc.GetElementbyId(idValue).GetAttributeValue(attributeName, NotFound);

    //    }


    //}

    public static class HtmlParseUtility
    {
        public static readonly string NotFound = "Not Found";

        public static string GetInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            return htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerHtml;
        }

        public static List<string> GetListInnerHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            htmlDoc.DocumentNode.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']")
                .ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
            return lstInnerhtml;
        }


        public static string GetAttributeValueFromId(string pageSource, string idValue, string attributeName)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            return htmlDoc.GetElementbyId(idValue).GetAttributeValue(attributeName, NotFound);

        }

        public static string GetOuterHtmlFromTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            return htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").OuterHtml;
        }

        public static List<string> GetListInnerHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            htmlDoc.DocumentNode
                .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                .ForEach(x => { lstInnerhtml.Add(x.InnerHtml.ToString()); });
            return lstInnerhtml;
        }
        public static List<string> GetListOuterHtmlFromPartialTagName(string pageSource, string tagName, string attributeName,
            string attributeValue)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            htmlDoc.DocumentNode
                .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });
            return lstInnerhtml;
        }
        public static List<string> GetListOuterHtmlFromPartialTagNameWhereInnerTagIs(string pageSource, string tagName, string attributeName,
            string attributeValue, string innertag)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            htmlDoc.DocumentNode
                .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
                .Descendants($"{innertag}")
                .ForEach(x => { lstInnerhtml.Add(x.OuterHtml.ToString()); });
            return lstInnerhtml;
        }

        public static List<string> GetListInnerHtmlFromPartialTagNameWhereInnerTagIs(string pageSource, string tagName, string attributeName,
          string attributeValue, string innertag)
        {
            var lstInnerhtml = new List<string>();
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(pageSource);
            htmlDoc.DocumentNode
            .SelectNodes($"//{tagName}[starts-with(@{attributeName}, '{attributeValue}')]")
            .Descendants($"{innertag}")
            .ForEach(x => { lstInnerhtml.Add(x.InnerText.ToString()); });
            return lstInnerhtml;
        }
        public static string GetAllInnerTextFromTags(string pageSource)
        {
            var text = string.Empty;
            var htmlDoc = new HtmlDocument();
            try
            {
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode.InnerText;

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return text;

        }

        /// <summary>
        /// Get inner text from single node by atteribute
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="tagName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <returns></returns>
        public static string GetInnerTextFromSingleNode(string pageSource, string tagName, string attributeName, string attributeValue)
        {
            var text = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return text;
        }


        /// <summary>
        /// Get inner text from single node by atteribute and index
        /// </summary>
        /// <param name="pageSource"></param>
        /// <param name="tagName"></param>
        /// <param name="attributeName"></param>
        /// <param name="attributeValue"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetInnerTextWithNodeIndex(string pageSource, string tagName, string attributeName, string attributeValue, int index)
        {
            var text = string.Empty;
            try
            {
                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(pageSource);
                text = htmlDoc.DocumentNode.SelectNodes($"//{tagName}[@{attributeName}='{attributeValue}']")[index].InnerText;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return text;
        }
    }
}