using CefSharp;
using System;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ResourceRequestHandler : IResourceRequestHandler
    {
        private readonly BrowserWindow embedBrowser;

        private readonly string password;

        private readonly string userName;

        public List<MemoryStreamResponseFilter> responseList = new List<MemoryStreamResponseFilter>();

        public bool IsNeedResourceData { get; set; }

        public ResourceRequestHandler(BrowserWindow embedBrowser
            , string userName = "", string password = "", bool isNeedResourceData = false)
        {
            // get the proxy username
            this.userName = userName;

            // get the proxy password
            this.password = password;

            this.embedBrowser = embedBrowser;

            IsNeedResourceData = isNeedResourceData;
        }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return null;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            try
            {
                if (IsNeedResourceData)
                {
                    var dataFilter = new MemoryStreamResponseFilter();
                    responseList.Add(dataFilter);
                    return dataFilter;
                }
            }
            catch (Exception ex)
            {

            }
            return new MemoryStreamResponseFilter();
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            callback.Dispose();
            return CefReturnValue.Continue;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            return request.Url.StartsWith("https://www.facebook.com");
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
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

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {

        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            return false;
        }
    }
}
