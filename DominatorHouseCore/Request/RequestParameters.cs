using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using DominatorHouseCore.Models;
using GramDominatorCore.Request;
using Newtonsoft.Json.Linq;

namespace DominatorHouseCore.Requests
{

    /// <summary>
    /// Here we can set respected reqest headers like useragent , referer , host , cookies , These will be added while creating request .  
    /// </summary>
    public class RequestParameters : IRequestParameters
    {
        public RequestParameters(string url)
        {
            this.Url = url;
        }
        public RequestParameters()
        {
          
        }

        public virtual System.Net.CookieCollection Cookies { get; set; }

        public WebHeaderCollection Headers { get; set; } = new WebHeaderCollection();


        public Models.Proxy Proxy { get; set; }
     
        public bool KeepAlive { get; set; }

        public string Accept { get; set; }

        public string Referer { get; set; }
        public string ContentType { get; set; } 
        public string UserAgent { get; set; }

      

        public string Url { get; set; }

        public byte[] PostData { get; set; }

        public bool IsMultiPart { get; set; }


        public Dictionary<string, FileData> fileList = new Dictionary<string, FileData>();
        public Dictionary<string, string> parameters = new Dictionary<string, string>();
        public Dictionary<string, string> postItems = new Dictionary<string, string>();


        #region fucntions 
        /// <summary>
        /// GenerateMultipartBoundary
        /// </summary>
        /// <returns></returns>
        private static string GenerateMultipartBoundary()
        {
            StringBuilder stringBuilder = new StringBuilder();
            int max = "-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".Length - 1;
            for (int index = 0; index < 30; ++index)
                stringBuilder.Append("-_1234567890abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"[RandomUtilties.GetRandomNumber(max, 0)]);
            return stringBuilder.ToString();
        }


        /// <summary>
        /// CreateMultipartBody
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public virtual byte[] CreateMultipartBody(string jsonString)
        {
            JObject jobject = JObject.Parse(jsonString);
            string multipartBoundary = GenerateMultipartBoundary();
            string str1 = string.Format("--{0}", (object)multipartBoundary);
            string str2 = string.Format("--{0}--", (object)multipartBoundary);
            StringBuilder stringBuilder = new StringBuilder();

            using (MemoryStream memoryStream = new MemoryStream())
            {
                stringBuilder.AppendLine(str1);
                foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
                {
                    stringBuilder.AppendLine(str1);
                    stringBuilder.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", (object)keyValuePair.Key));
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(keyValuePair.Value.ToString());
                }
                foreach (KeyValuePair<string, string> postItem in this.postItems)
                {
                    stringBuilder.AppendLine(str1);
                    stringBuilder.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"", (object)postItem.Key));
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine(postItem.Value);
                }
                foreach (KeyValuePair<string, FileData> file in this.fileList)
                {
                    stringBuilder.AppendLine(str1);
                    stringBuilder.AppendLine(string.Format("Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"", (object)file.Key, (object)file.Value.FileName));
                    foreach (string header in (NameObjectCollectionBase)file.Value.Headers)
                        stringBuilder.AppendLine(string.Format("{0}: {1}", (object)header, (object)file.Value.Headers[header]));
                    stringBuilder.AppendLine();
                    byte[] bytes = Encoding.UTF8.GetBytes(stringBuilder.ToString());
                    memoryStream.Write(bytes, 0, bytes.Length);
                    stringBuilder.Clear();
                    memoryStream.Write(file.Value.Contents, 0, file.Value.Contents.Length);
                }

                byte[] bytes1 = Encoding.UTF8.GetBytes(Environment.NewLine + str2);
                memoryStream.Write(bytes1, 0, bytes1.Length);
                this.AddHeader("Content-Type", string.Format("multipart/form-data; boundary={0}", (object)multipartBoundary));

                return memoryStream.ToArray();
            }
        }


        /// <summary>
        /// CreateSimplePost
        /// </summary>
        /// <param name="jsonString"></param>
        /// <returns></returns>
        public byte[] CreateSimplePost(string jsonString)
        {
            JObject jobject = JObject.Parse(jsonString);
            StringBuilder stringBuilder = new StringBuilder();
            foreach (KeyValuePair<string, JToken> keyValuePair in jobject)
            {
                stringBuilder.Append(keyValuePair.Key);
                stringBuilder.Append("=");
                stringBuilder.Append((object)keyValuePair.Value);
                stringBuilder.Append("&");
            }
            foreach (KeyValuePair<string, string> postItem in this.postItems)
            {
                stringBuilder.Append(postItem.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(postItem.Value);
                stringBuilder.Append("&");
            }
            --stringBuilder.Length;
            return Encoding.UTF8.GetBytes(stringBuilder.ToString());
        }

        /// <summary>
        /// AddHeader
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddHeader(string key, string value)
        {
            try
            {
                this.Headers.Add(key, value);
            }
            catch (Exception Ex)
            {
            }
        }

        /// <summary>
        /// AddParam
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddParam(string key, string value)
        {
            this.parameters.Add(key, value);
        }

        /// <summary>
        /// Adds Unsigned Post
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public void AddUnsignedPost(string key, string value)
        {
            this.postItems.Add(key, value);
        }

        /// <summary>
        /// AddFile
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="fileName"></param>
        public void AddFile(string key, byte[] data, string fileName)
        {
            NameValueCollection headers = new NameValueCollection();
            headers.Add("Content-Type", "application/octet-stream");
            headers.Add("Content-Transfer-Encoding", "binary");
            fileName = Path.GetFileName(fileName);
            string fileName1 = fileName;
            byte[] contents = data;
            FileData fileData = new FileData(headers, fileName1, contents);
            this.fileList.Add(key, fileData);
            this.IsMultiPart = true;
        }


        /// <summary>
        /// GenerateUrl
        /// </summary>
        /// <returns></returns>
        public string GenerateUrl()
        {
            string[] array =
                this.parameters.Select<KeyValuePair<string, string>, string>
                (
                    (Func<KeyValuePair<string, string>, string>)(x => string.Format("{0}={1}", (object)WebUtility.UrlEncode(x.Key), (object)WebUtility.UrlEncode(x.Value)))).ToArray<string>();
            return string.Format("{0}{1}{2}", (object)this.Url, array.Length != 0 ? (object)"?" : (object)string.Empty, (object)string

                .Join("&", array));
        }



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string GenerateUrl(string url)
        {
            string[] array =
                this.parameters.Select<KeyValuePair<string, string>, string>
                (
                    (Func<KeyValuePair<string, string>, string>)(x => string.Format("{0}={1}", WebUtility.UrlEncode(x.Key), WebUtility.UrlEncode(x.Value)))).ToArray<string>();
            return string.Format("{0}{1}{2}", this.Url, array.Length != 0 ? "?" : string.Empty,string
                .Join("&", array));
        }

        #endregion

    }


    public interface IRequestParameters
    {
        WebHeaderCollection Headers { get; set; }

        System.Net.CookieCollection Cookies { get;  set;}

        string ContentType { get; set; }

        string UserAgent { get; set; }

        Proxy Proxy { get; set; }

        bool KeepAlive { get; set; }

        string Accept { get; set; }

        string Referer { get; set; }

        string Url { get; set; }

        byte[] PostData { get; set; }

        bool IsMultiPart { get; set; }

        void AddHeader(string key, string value);
    }
}
