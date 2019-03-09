using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ProxyRequestHandler : IRequestHandler
    {
        private readonly BrowserWindow embedBrowser;

        private readonly string password;

        private readonly string userName;


        public ProxyRequestHandler(string userName, string password, BrowserWindow embedBrowser)
        {
            // get the proxy username
            this.userName = userName;

            // get the proxy password
            this.password = password;

            this.embedBrowser = embedBrowser;
        }


        bool IRequestHandler.OnBeforeBrowse(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, bool isRedirect)
        {
            return false;
        }

        bool IRequestHandler.OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return OnOpenUrlFromTab(browserControl, browser, frame, targetUrl, targetDisposition, userGesture);
        }


        bool IRequestHandler.OnCertificateError(IWebBrowser browserControl, IBrowser browser,
            CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            return false;
        }

        void IRequestHandler.OnPluginCrashed(IWebBrowser browserControl, IBrowser browser, string pluginPath)
        {
        }

        CefReturnValue IRequestHandler.OnBeforeResourceLoad(IWebBrowser browserControl, IBrowser browser,
            IFrame frame, IRequest request, IRequestCallback callback)
        {
            return CefReturnValue.Continue;
        }

        bool IRequestHandler.GetAuthCredentials(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            if (isProxy)
            {
                callback.Continue(userName, password);

                return true;
            }

            return false;
        }

        void IRequestHandler.OnRenderProcessTerminated(IWebBrowser browserControl, IBrowser browser,
            CefTerminationStatus status)
        {
        }

        bool IRequestHandler.OnQuotaRequest(IWebBrowser browserControl, IBrowser browser, string originUrl,
            long newSize, IRequestCallback callback)
        {
            return false;
        }


        bool IRequestHandler.OnProtocolExecution(IWebBrowser browserControl, IBrowser browser, string url)
        {
            return url.StartsWith("mailto");
        }

        void IRequestHandler.OnRenderViewReady(IWebBrowser browserControl, IBrowser browser)
        {
        }

        bool IRequestHandler.OnResourceResponse(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            return false;
        }

        IResponseFilter IRequestHandler.GetResourceResponseFilter(IWebBrowser browserControl, IBrowser browser,
            IFrame frame, IRequest request, IResponse response)
        {
            return null;
        }

        void IRequestHandler.OnResourceLoadComplete(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
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
                    if (embedBrowser.UrlBar.Text == embedBrowser.Browser.Address)
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

        public bool OnSelectClientCertificate(IWebBrowser browserControl, IBrowser browser, bool isProxy,
            string host, int port, X509Certificate2Collection certificates,
            ISelectClientCertificateCallback callback)
        {
            return false;
        }

        public void OnResourceRedirect(IWebBrowser browserControl, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, ref string newUrl)
        {
        }


        protected virtual bool OnOpenUrlFromTab(IWebBrowser browserControl, IBrowser browser, IFrame frame,
            string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            return false;
        }
    }
}