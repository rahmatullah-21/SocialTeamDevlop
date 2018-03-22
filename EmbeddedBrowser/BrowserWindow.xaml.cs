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
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

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
          
            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath = $"{ConstantVariable.GetCachePathDirectory()}\\{dominatorAccountModel.AccountId}"
            });
        
            Browser.RequestHandler = new RequestHandlerCustom(this);

            Browser.IsBrowserInitializedChanged += LoadSettings;
        }


        private DominatorAccountModel _dominatorAccountModel;
        public DominatorAccountModel DominatorAccountModel
        {
            get
            {
                return _dominatorAccountModel;
            }
            set
            {
                _dominatorAccountModel = value;
                OnPropertyChanged(nameof(DominatorAccountModel));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void ButtonCheckIp_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Load("https://app.multiloginapp.com/WhatIsMyIP");
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var homePage = GetNetworksHomeUrl();
            Browser.Load(homePage);
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
                        var proxyIp = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp;

                        // get the proxyport from objDominatorAccountModel object
                        var proxyPort = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort;

                        // get the current browser request context
                        var requestContext = Browser.GetBrowser().GetHost().RequestContext;

                        if (!string.IsNullOrEmpty(proxyIp) && !string.IsNullOrEmpty(proxyPort))
                        {
                            // declare the dictionary for passing proxy ip and proxy port
                            var dictProxyIpPort = new Dictionary<string, object>
                            {
                                {"mode", "fixed_servers"},
                                {"server", "" + proxyIp + ":" + proxyPort + ""}
                            };

                            string error;

                            var success = requestContext.SetPreference("proxy", dictProxyIpPort, out error);
                        }
                        else
                        {
                            var dictProxyIpPort = new Dictionary<string, object> {{"mode", "direct"}};


                            string error;

                            var success = requestContext.SetPreference("proxy", dictProxyIpPort, out error);
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

            var homePage = GetNetworksHomeUrl();
            Browser.Load(homePage);

            Browser.LoadingStateChanged += BrowserOnLoaded;
        }


        private void BrowserOnLoaded(object sender, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            try
            {
                bool isLoaded = false;

                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        isLoaded = Browser.IsLoaded;
                    }
                    catch (Exception e)
                    {
                      e.DebugLog(); 
                    }                   
                });
                    
                if(!isLoaded) return;

                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) &&
                    !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
                    {
                        case SocialNetworks.Facebook:
                            FacebookBrowserLogin();
                            break;
                        case SocialNetworks.Instagram:
                            break;
                        case SocialNetworks.Twitter:
                            break;
                        case SocialNetworks.Pinterest:
                            break;
                        case SocialNetworks.LinkedIn:
                            break;
                        case SocialNetworks.Reddit:
                            break;
                        case SocialNetworks.Social:
                            break;
                        case SocialNetworks.Quora:
                            break;
                        case SocialNetworks.Gplus:
                            break;
                        case SocialNetworks.Youtube:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                   
                }

                Browser.LoadingStateChanged -= BrowserOnLoaded;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
            }
        }

        private void FacebookBrowserLogin()
        {


            Browser.ExecuteScriptAsync("document.getElementById('email').value= '" +
                                       DominatorAccountModel.AccountBaseModel.UserName + "'");

            Browser.ExecuteScriptAsync("document.getElementById('pass').value= '" +
                                       DominatorAccountModel.AccountBaseModel.Password + "'");

            Thread.Sleep(1000);

            Browser.ExecuteScriptAsync("document.getElementById('u_0_5').click()");
        }

        public string GetNetworksHomeUrl()
        {
            switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Facebook:
                    return "https://www.facebook.com";                   
                case SocialNetworks.Instagram:
                    break;
                case SocialNetworks.Twitter:
                    break;
                case SocialNetworks.Pinterest:
                    break;
                case SocialNetworks.LinkedIn:
                    break;
                case SocialNetworks.Reddit:
                    break;
                case SocialNetworks.Social:
                    break;
                case SocialNetworks.Quora:
                    break;
                case SocialNetworks.Gplus:
                    break;
                case SocialNetworks.Youtube:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return null;
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