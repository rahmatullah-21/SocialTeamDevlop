using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel.Common;
using System;
using System.Threading.Tasks;

namespace DominatorUIUtility.ViewModel
{
    public interface IVerifyProxiesViewModel
    {
        Task Verify(params ProxyManagerModel[] models);
    }

    public class VerifyProxiesViewModel : SynchronizedViewModel, IVerifyProxiesViewModel
    {
        private int _verified;
        private int _total;
        private string _urlToUseToVerifyProxies = "https://www.google.com";

        public string URLToUseToVerifyProxies
        {
            get { return _urlToUseToVerifyProxies; }
            set { SetProperty(ref _urlToUseToVerifyProxies, value); }
        }

        public int Total
        {
            get { return _total; }
            set { SetProperty(ref _total, value); }
        }

        public int Verified
        {
            get { return _verified; }
            set { SetProperty(ref _verified, value); }
        }

        public async Task Verify(params ProxyManagerModel[] models)
        {
            await this.ExecuteSynchronized(VerifyInternal, models);
        }

        private async Task VerifyInternal(params ProxyManagerModel[] models)
        {
            Total = models.Length;
            Verified = 0;
            foreach (var model in models)
            {
                await CheckProxyAsync(model);
            }
        }

        private async Task CheckProxyAsync(ProxyManagerModel currentProxyManager)
        {
            try
            {
                await ProxyFileManager.UpdateProxyStatusAsync(currentProxyManager, URLToUseToVerifyProxies);
                GlobusLogHelper.log.Info(Log.ProxyVerificationCompleted, SocialNetworks.Social,
                    currentProxyManager.AccountProxy.ProxyIp + ":" + currentProxyManager.AccountProxy.ProxyPort);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            finally
            {
                Verified++;
            }
        }
    }
}
