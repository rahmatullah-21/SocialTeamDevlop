using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;

namespace EmbeddedBrowser.BrowserHelper
{
    public class RequestHandlerCustom : IRequestHandler
    {
        private readonly BrowserWindow embedBrowser;

        public RequestHandlerCustom(BrowserWindow embedBrowser)
        {
            this.embedBrowser = embedBrowser;
        }

        public bool GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame, bool isProxy,
            string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (isProxy)
            {
                callback.Continue(embedBrowser.DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                    embedBrowser.DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword);

                return true;
            }
            return false;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            bool isRedirect)
        {
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IRequestCallback callback)
        {
            callback.Dispose();
            return CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser browserControl, IBrowser browser, CefErrorCode errorCode,
            string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame, string targetUrl,
            WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }

        public void OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
        }

        public bool OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("https://www.facebook.com");
        }

        public bool OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl, long newSize,
            IRequestCallback callback)
        {
            return false;
        }

        public void OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser,
            CefTerminationStatus status)
        {
        }

        public void OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
        }


        public void OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
         {
            if (embedBrowser.Browser.IsDisposed) return;
            if (!embedBrowser.Dispatcher.CheckAccess())
                embedBrowser.Dispatcher.BeginInvoke(new Action(delegate
                {
                    if (embedBrowser.Browser.Address == "https://www.youtube.com/oops")
                    {
                        embedBrowser.Browser.Address = embedBrowser.UrlBar.Text;
                        return;
                    }
                    if (embedBrowser.Browser.Address == "https://accounts.google.com/CookieMismatch")
                    {
                        embedBrowser.Browser.Address = embedBrowser.UrlBar.Text = "https://myaccount.google.com/";
                        return;
                    }
                    if (embedBrowser.UrlBar.Text ==embedBrowser.Browser.Address)
                    return;

                    embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
                }));
            else
            {
                if (embedBrowser.Browser.Address == "https://www.youtube.com/oops")
                {
                    embedBrowser.Browser.Address = embedBrowser.UrlBar.Text;
                    return;
                }
                if (embedBrowser.Browser.Address == "https://accounts.google.com/CookieMismatch")
                {
                    embedBrowser.Browser.Address = embedBrowser.UrlBar.Text = "https://myaccount.google.com/";
                    return;
                }
                if (embedBrowser.UrlBar.Text == embedBrowser.Browser.Address)
                    return;
                embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
            }
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, ref string newUrl)
        {
        }

        public bool OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response)
        {
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy,
            string host, int port, X509Certificate2Collection certificates,
            ISelectClientCertificateCallback callback)
        {
            return false;
        }
    }
}
