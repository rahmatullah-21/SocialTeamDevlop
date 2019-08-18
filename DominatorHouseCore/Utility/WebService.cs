using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public interface IWebService
    {
        byte[] GetImageBytesFromUrl(string url);
        bool CheckProxy(Uri uri, WebProxy proxy);
    }

    public sealed class WebService : IWebService
    {
        public byte[] GetImageBytesFromUrl(string url)
        {
            using (var webClient = new WebClient())
            {
               return webClient.DownloadData(url);
            }
        }
        public bool CheckProxy(Uri uri, WebProxy proxy)
        {
            try
            {
                using (var webClient = new WebClientExtended())
                {
                    var address = "http://google.com/";
                    webClient.DownloadString(address);
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }

        private class WebClientExtended : WebClient
        {
            protected override WebRequest GetWebRequest(Uri uri)
            {
                var webRequest = (HttpWebRequest)base.GetWebRequest(uri);
                if (webRequest == null)
                    throw new ArgumentException();
                int num1 = 5000;
                webRequest.Timeout = num1;
                int num2 = 0;
                webRequest.KeepAlive = num2 != 0;
                webRequest.ServicePoint.SetTcpKeepAlive(false, 1000, 5000);
                return webRequest;
            }
        }
    }
}
