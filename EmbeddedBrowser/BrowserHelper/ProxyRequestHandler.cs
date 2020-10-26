using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ProxyRequestHandler : IRequestHandler
    {
        private readonly BrowserWindow embedBrowser;

        private readonly string password;

        private readonly string userName;

        public ResourceRequestHandler resourceRequestHandler;
        
        public bool IsNeedResourceData { get; set; }

        public ProxyRequestHandler(string userName, string password, BrowserWindow embedBrowser
            , bool isNeedResourceData = false, DominatorHouseCore.Enums.SocialNetworks sn = DominatorHouseCore.Enums.SocialNetworks.Social)
        {
            // get the proxy username
            this.userName = userName;

            // get the proxy password
            this.password = password;

            this.embedBrowser = embedBrowser;

            IsNeedResourceData = isNeedResourceData;

            resourceRequestHandler = new ResourceRequestHandler(embedBrowser, userName, password, IsNeedResourceData,sn);
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(chromiumWebBrowser, browser, frame, targetUrl, targetDisposition, userGesture);
        }

        public IResourceRequestHandler GetResourceRequestHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool isNavigation, bool isDownload, string requestInitiator, ref bool disableDefaultHandling)
        {
            return resourceRequestHandler;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (isProxy)
            {
                callback.Continue(userName, password);

                return true;
            }

            return false;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            return false;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {

        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {

        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {

        }
    }
}