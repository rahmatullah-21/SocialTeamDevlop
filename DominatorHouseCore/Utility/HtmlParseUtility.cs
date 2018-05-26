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
            return htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").InnerHtml.ToString();
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
            return htmlDoc.DocumentNode.SelectSingleNode($"//{tagName}[@{attributeName}='{attributeValue}']").OuterHtml.ToString();
        }

    }
}