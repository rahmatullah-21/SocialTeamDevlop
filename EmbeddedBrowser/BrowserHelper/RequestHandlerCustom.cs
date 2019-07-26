using System;
using System.Security.Cryptography.X509Certificates;
using CefSharp;
using System.IO;
using System.Collections.Generic;

namespace EmbeddedBrowser.BrowserHelper
{
    public class RequestHandlerCustom : IRequestHandler
    {
        private readonly BrowserWindow embedBrowser;

        public List<MemoryStreamResponseFilter> responseList = new List<MemoryStreamResponseFilter>();

        public bool IsNeedResourceData { get; set; }

        public RequestHandlerCustom(BrowserWindow embedBrowser, bool isNeedResourceData = false)
        {
            this.embedBrowser = embedBrowser;
            IsNeedResourceData = isNeedResourceData;
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
            try
            {
                if(IsNeedResourceData)
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


    public class MemoryStreamResponseFilter : IResponseFilter
    {
        private MemoryStream memoryStream;

        bool IResponseFilter.InitFilter()
        {
            //NOTE: We could initialize this earlier, just one possible use of InitFilter
            memoryStream = new MemoryStream();
            return true;
        }

        FilterStatus IResponseFilter.Filter(Stream dataIn, out long dataInRead, Stream dataOut, out long dataOutWritten)
        {
            if (dataIn == null)
            {
                dataInRead = 0;
                dataOutWritten = 0;

                return FilterStatus.Done;
            }

            //Calculate how much data we can read, in some instances dataIn.Length is
            //greater than dataOut.Length
            dataInRead = Math.Min(dataIn.Length, dataOut.Length);
            dataOutWritten = dataInRead;

            var readBytes = new byte[dataInRead];
            dataIn.Read(readBytes, 0, readBytes.Length);
            dataOut.Write(readBytes, 0, readBytes.Length);

            //Write buffer to the memory stream
            memoryStream.Write(readBytes, 0, readBytes.Length);

            //If we read less than the total amount avaliable then we need
            //return FilterStatus.NeedMoreData so we can then write the rest
            if (dataInRead < dataIn.Length)
            {
                return FilterStatus.NeedMoreData;
            }

            if (memoryStream.Length > 0)
                Data = memoryStream.ToArray();

            return FilterStatus.Done;
        }

        void IDisposable.Dispose()
        {
            memoryStream.Dispose();
            memoryStream = null;
        }

        private byte[] _data;

        public byte[] Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
            }
        }
    }
}
