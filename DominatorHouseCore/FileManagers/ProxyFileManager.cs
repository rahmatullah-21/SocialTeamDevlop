using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DominatorHouseCore.FileManagers
{
    public interface IProxyFileManager
    {
        bool SaveProxy(ProxyManagerModel proxy);
        List<ProxyManagerModel> GetAllProxy();
        void EditProxy(ProxyManagerModel proxy);
        void EditAllProxy(List<ProxyManagerModel> proxy);
        void Delete(Predicate<ProxyManagerModel> match);
        ProxyManagerModel GetProxyById(string ProxyId);
        Task UpdateProxyStatusAsync(ProxyManagerModel currentProxyManager, string url);
    }

    public class ProxyFileManager : IProxyFileManager
    {
        private readonly IBinFileHelper _binFileHelper;

        public ProxyFileManager()
        {
            _binFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
        }

        public bool SaveProxy(ProxyManagerModel proxy)
        {
            try
            {
                _binFileHelper.SaveProxy(proxy);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public List<ProxyManagerModel> GetAllProxy()
        {
            return _binFileHelper.GetProxyDetails();
        }


        public void EditProxy(ProxyManagerModel proxy) => _binFileHelper.UpdateProxy(proxy);
        public void EditAllProxy(List<ProxyManagerModel> proxy) => _binFileHelper.UpdateAllProxy(proxy);


        public void Delete(Predicate<ProxyManagerModel> match)
        {
            var proxy = _binFileHelper.GetProxyDetails();
            proxy.RemoveAll(match);
            _binFileHelper.UpdateAllProxy(proxy);
        }


        public ProxyManagerModel GetProxyById(string ProxyId)
        {
            return GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyId == ProxyId);
        }
        public async Task UpdateProxyStatusAsync(ProxyManagerModel currentProxyManager, string url)
        {
            try
            {
                if (string.IsNullOrEmpty(url))
                {
                    GlobusLogHelper.log.Info("Please enter some URL in the input field, which will be used to verify the proxy.");
                    return;
                }
                var request = (HttpWebRequest)WebRequest.Create(new Uri(url));
                request.Timeout = 5000;

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
                    GlobusLogHelper.log.Info(Log.ProxyVerificationStarted, SocialNetworks.Social, currentProxyManager.AccountProxy.ProxyIp + ":" + currentProxyManager.AccountProxy.ProxyPort);

                    using (var response = (HttpWebResponse)await request.GetResponseAsync())
                    {
                        currentProxyManager.Status = response.StatusCode.ToString() == "OK" ? "Working" : "Not Working";
                    }

                    stopWatch.Stop();
                    var ts = stopWatch.Elapsed;
                    currentProxyManager.ResponseTime = $"{ts.Milliseconds} milli seconds";
                }
            }
            catch (Exception)
            {
                if (currentProxyManager != null)
                {
                    currentProxyManager.Status = "Fail";
                    currentProxyManager.Failures = 1;
                }
            }

            EditProxy(currentProxyManager);

        }
    }
}
