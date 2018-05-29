using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public static class ProxyFileManager
    {
        public static bool SaveProxy(ProxyManagerModel proxy)
        {
            try
            {
                BinFileHelper.SaveProxy(proxy);
                GlobusLogHelper.log.Debug($"Proxy successfully saved");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static List<ProxyManagerModel> GetAllProxy()
        {
            return BinFileHelper.GetProxyDetails();
        }


        public static void EditProxy(ProxyManagerModel proxy) => BinFileHelper.UpdateProxy(proxy);
        public static void EditAllProxy(List<ProxyManagerModel> proxy) => BinFileHelper.UpdateAllProxy(proxy);


        public static void Delete(Predicate<ProxyManagerModel> match)
        {
            var proxy = BinFileHelper.GetProxyDetails();
            var toDelete = proxy.FindAll(match);
            proxy.RemoveAll(match);
            BinFileHelper.UpdateAllProxy(proxy);
        }

       
        public static ProxyManagerModel GetProxyById(string ProxyId)
        {
           return GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyId == ProxyId);
        }
        public static async Task UpdateProxyStatusAsync(ProxyManagerModel currentProxyManager,string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    GlobusLogHelper.log.Info("Please enter some URL in the input field, which will be used to verify the proxy.");
                    return;
                }
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));

                if (currentProxyManager != null)
                {
                    var webProxy = new WebProxy(currentProxyManager.AccountProxy.ProxyIp, int.Parse(currentProxyManager.AccountProxy.ProxyPort))
                    {
                        BypassProxyOnLocal = true
                    };
                    if (!string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyUsername)
                        && !string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyPassword))
                    {
                        webProxy.Credentials = new NetworkCredential(currentProxyManager.AccountProxy.ProxyUsername, currentProxyManager.AccountProxy.ProxyPassword);
                    }

                    request.Proxy = webProxy;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using (var response = (HttpWebResponse)await request.GetResponseAsync())
                    {
                        currentProxyManager.Status = response.StatusCode.ToString() == "OK" ? "Working" : "Not Working";
                    }

                    stopWatch.Stop();
                    var ts = stopWatch.Elapsed;
                    currentProxyManager.ResponseTime = $"{ts.Milliseconds} milli seconds";
                }
            }
            catch (Exception ex)
            {
                if (currentProxyManager != null)
                {
                    currentProxyManager.Status = "Fail";
                    currentProxyManager.Failures = 1;
                }
            }

            ProxyFileManager.EditProxy(currentProxyManager);

        }
    }
}
