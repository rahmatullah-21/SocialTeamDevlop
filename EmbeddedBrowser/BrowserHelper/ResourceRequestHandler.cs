using System;
using System.Collections.Generic;
using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Enums;

namespace EmbeddedBrowser.BrowserHelper
{
    public class ResourceRequestHandler : IResourceRequestHandler
    {
        private readonly BrowserWindow _embedBrowser;
        readonly SocialNetworks _Sn;
        public List<MemoryStreamResponseFilter> ResponseList = new List<MemoryStreamResponseFilter>();

        public List<KeyValuePair<string, MemoryStreamResponseFilter>> TwitterresponseList =
            new List<KeyValuePair<string, MemoryStreamResponseFilter>>();

        public ResourceRequestHandler(BrowserWindow embedBrowser, bool isNeedResourceData = false, SocialNetworks sn = SocialNetworks.Social)
        {
            // get the proxy username

            // get the proxy password

            this._embedBrowser = embedBrowser;

            IsNeedResourceData = isNeedResourceData;

            _Sn = sn;
        }

        public bool IsNeedResourceData { get; set; }

        public ICookieAccessFilter GetCookieAccessFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return null;
        }

        public IResourceHandler GetResourceHandler(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return null;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response)
        {
            try
            {

                if (IsNeedResourceData)
                {
                    var dataFilter = new MemoryStreamResponseFilter();
                    ResponseList.Add(dataFilter);
                    if (_Sn == SocialNetworks.Twitter)
                        TwitterresponseList.Add(new KeyValuePair<string, MemoryStreamResponseFilter>(request.Url, dataFilter));
                    return dataFilter;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return new MemoryStreamResponseFilter();
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IRequestCallback callback)
        {
            callback.Dispose();
            return CefReturnValue.Continue;
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request)
        {
            return request.Url.StartsWith("https://www.facebook.com");
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame,
            IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            try
            {
                if (_embedBrowser.Browser.IsDisposed) return;
                if (!_embedBrowser.Dispatcher.CheckAccess())
                {
                    _embedBrowser.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        if (_embedBrowser.Browser.Address == "https://www.youtube.com/oops")
                        {
                            _embedBrowser.Browser.Address = _embedBrowser.SearchUrl;
                            return;
                        }

                        if (_embedBrowser.Browser.Address == "https://accounts.google.com/CookieMismatch")
                        {
                            _embedBrowser.Browser.Address = _embedBrowser.SearchUrl = "https://myaccount.google.com/";
                            return;
                        }

                        if (_embedBrowser.SearchUrl == _embedBrowser.Browser.Address)
                            return;
                        if (string.IsNullOrWhiteSpace(_embedBrowser.UrlBar.Text))
                            _embedBrowser.UrlBar.Text = "";
                        _embedBrowser.SearchUrl = _embedBrowser.Browser.Address;
                    }));
                }
                else
                {
                    if (_embedBrowser.Browser.Address == "https://www.youtube.com/oops")
                    {
                        _embedBrowser.Browser.Address = _embedBrowser.SearchUrl;
                        return;
                    }

                    if (_embedBrowser.Browser.Address == "https://accounts.google.com/CookieMismatch")
                    {
                        _embedBrowser.Browser.Address = _embedBrowser.SearchUrl = "https://myaccount.google.com/";
                        return;
                    }

                    if (_embedBrowser.SearchUrl == _embedBrowser.Browser.Address)
                        return;
                    if (string.IsNullOrWhiteSpace(_embedBrowser.UrlBar.Text))
                        _embedBrowser.UrlBar.Text = "";

                    _embedBrowser.SearchUrl = _embedBrowser.Browser.Address;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            IResponse response, ref string newUrl)
        {
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request,
            IResponse response)
        {
            return false;
        }
    }
}