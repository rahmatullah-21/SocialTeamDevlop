using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;

namespace EmbeddedBrowser
{
    /// <summary>
    ///     Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window, INotifyPropertyChanged, IComponentConnector, IDisposable
    {
        public BrowserWindow()
        {
            InitializeComponent();
            WindowBrowsers.DataContext = this;
        }

        public BrowserWindow(DominatorAccountModel dominatorAccountModel)
            : this()
        {
            DominatorAccountModel = dominatorAccountModel;

            InitializeComponent();

            var accountCacheId = string.Empty;

            //try
            //{
            //    AccountCacheID = DataBaseConnection.DataBaseConnection.getSingleData<tbl_AccountCacheDetails>(x => x.Account == account.username).CacheID;
            //}
            //catch (Exception)
            //{
            //}
            //if (string.IsNullOrEmpty(AccountCacheID))
            //{
            //    try
            //    {
            //        AccountCacheID = Guid.NewGuid().ToString();
            //        tbl_AccountCacheDetails objtbl_AccountCacheDetails = new tbl_AccountCacheDetails();
            //        objtbl_AccountCacheDetails.Account = account.username;
            //        objtbl_AccountCacheDetails.CacheID = AccountCacheID;
            //        objtbl_AccountCacheDetails.ActionDate = DateTime.Now.ToString();
            //        DataBaseConnection.DataBaseConnection.AddData<tbl_AccountCacheDetails>(objtbl_AccountCacheDetails);
            //    }
            //    catch (Exception ex)
            //    {
            //        GlobusLogHelper.log.Error(ex.Message);
            //    }
            //}

            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath =
                    $"{Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)}\\{"FD"}\\cache\\{accountCacheId}"
            });

            Browser.RequestHandler = new RequestHandlerCustom(this);

            Browser.IsBrowserInitializedChanged += LoadSettings;
        }

        public DominatorAccountModel DominatorAccountModel { get; set; }


        public event PropertyChangedEventHandler PropertyChanged;


        private void ButtonCheckIp_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Load("https://app.multiloginapp.com/WhatIsMyIP");
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void LoadSettings(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            if (!Browser.IsBrowserInitialized)
                return;
            try
            {
                Cef.UIThreadTaskFactory.StartNew(delegate
                {
                    try
                    {
                        if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername) &&
                            !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword))
                            Browser.RequestHandler = new ProxyRequestHandler(
                                DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                                DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword, this);

                        // get the proxyip from objDominatorAccountModel object
                        var ProxyIp = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;

                        // get the proxyport from objDominatorAccountModel object
                        var ProxyPort = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort;

                        // get the current browser request context
                        var requestContext = Browser.GetBrowser().GetHost().RequestContext;

                        if (!string.IsNullOrEmpty(ProxyIp) && !string.IsNullOrEmpty(ProxyPort))
                        {
                            // declare the dictionary for passing proxy ip and proxy port
                            var DictProxyIpPort = new Dictionary<string, object>();

                            DictProxyIpPort.Add("mode", "fixed_servers");

                            DictProxyIpPort.Add("server", "" + ProxyIp + ":" + ProxyPort + "");

                            string error;

                            var success = requestContext.SetPreference("proxy", DictProxyIpPort, out error);
                        }
                        else
                        {
                            var DictProxyIpPort = new Dictionary<string, object>();

                            DictProxyIpPort.Add("mode", "direct");

                            string error;

                            var success = requestContext.SetPreference("proxy", DictProxyIpPort, out error);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                    }
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

            Browser.Load("https://www.facebook.com");

            Browser.LoadingStateChanged += BrowserOnLoaded;
        }


        private void BrowserOnLoaded(object sender, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            try
            {
                if (Dispatcher.Invoke(() => !Browser.IsLoaded))
                    return;


                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) &&
                    !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    Browser.ExecuteScriptAsync("document.getElementById('email').value= '" +
                                               DominatorAccountModel.AccountBaseModel.UserName + "'");

                    Browser.ExecuteScriptAsync("document.getElementById('pass').value= '" +
                                               DominatorAccountModel.AccountBaseModel.Password + "'");

                    Thread.Sleep(1000);

                    Browser.ExecuteScriptAsync("document.getElementById('u_0_5').click()");
                }

                Browser.LoadingStateChanged -= BrowserOnLoaded;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }


        private void btnFacebookLogin_Click(object sender, RoutedEventArgs e)
        {
            Browser.Load("https://www.facebook.com");
        }

        public void Dispose()
        {
            Dispose(true);
        }

        public void Dispose(bool ForceDispose)
        {
            Browser.Dispose();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Dispose();
        }


        private class RequestHandlerCustom : IRequestHandler
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
                if (!embedBrowser.Dispatcher.CheckAccess())
                    embedBrowser.Dispatcher.BeginInvoke(new Action(delegate
                    {
                        embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
                    }));
                else
                    embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
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
                        embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
                    }));
                else
                    embedBrowser.UrlBar.Text = embedBrowser.Browser.Address;
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
}