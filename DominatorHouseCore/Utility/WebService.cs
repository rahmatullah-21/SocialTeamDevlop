using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace DominatorHouseCore.Utility
{
    public interface IWebService
    {
        byte[] GetImageBytesFromUrl(string url);
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
    }
}
