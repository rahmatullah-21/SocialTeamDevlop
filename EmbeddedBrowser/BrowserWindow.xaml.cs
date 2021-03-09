using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using CefSharp;
using CefSharp.Wpf;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.EmbeddedBrowser;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using EmbeddedBrowser.BrowserHelper;
using Prism.Commands;
using Cookie = CefSharp.Cookie;

namespace EmbeddedBrowser
{
    /// <summary>
    ///     Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : INotifyPropertyChanged, IDisposable
    {
        private readonly CancellationToken _token;

        public BrowserWindow()
        {
            if (!Cef.IsInitialized)
            {
                var settings = new CefSettings();
                settings.CommandLineArgsDisabled = false;
                settings.CefCommandLineArgs.Add("--disable-webgl", "1");
                settings.CefCommandLineArgs.Add("--disable-reading-from-canvas", "1");

                // Set BrowserSubProcessPath based on app bitness at runtime
                //settings.BrowserSubprocessPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                //                                       Environment.Is64BitProcess ? "x64" : "x86",
                //                                       "CefSharp.BrowserSubprocess.exe");

                // Make sure you set performDependencyCheck false
                //Cef.Initialize(settings, performDependencyCheck: false, browserProcessHandler: null);

                Cef.Initialize(settings);
            }

            InitializeComponent();
            WindowBrowsers.DataContext = this;
            SearchCommand = new DelegateCommand(() =>
            {
                if (string.IsNullOrEmpty(UrlBar.Text))
                    return;
                GoToUrl(UrlBar.Text);
                UrlBar.Text = "";
            });
        }

        public BrowserWindow(DominatorAccountModel dominatorAccountModel, string targetUrl = "", bool customUse = false,
            bool skipAd = false, bool isNeedResourceData = false,
            bool browserLoginMessageToDisplay = true)
            : this()
        {
            DominatorAccountModel = dominatorAccountModel;
            _token = DominatorAccountModel.Token;
            BrowserName =
                $"{DominatorAccountModel.AccountBaseModel.AccountNetwork} Browser{(!string.IsNullOrWhiteSpace(DominatorAccountModel.AccountBaseModel.AccountName) ? " - " + DominatorAccountModel.AccountBaseModel.AccountName : "")}";
            TargetUrl = targetUrl;
            CustomUse = customUse;
            IsNeedResourceData = isNeedResourceData;
            RequestHandlerCustom = new RequestHandlerCustom(this, isNeedResourceData, dominatorAccountModel.AccountBaseModel.AccountNetwork);

            //SkipYoutubeAd = skipAd;
            BrowserLoginMessage = browserLoginMessageToDisplay;

            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath = "" //$"{ConstantVariable.GetCachePathDirectory()}\\{DominatorAccountModel.AccountId}"
            });
            Cef.UIThreadTaskFactory.StartNew(() =>
            {
                if (Browser.RequestContext.CanSetPreference("webrtc.ip_handling_policy"))
                {
                    var error = string.Empty;
                    Browser.RequestContext.SetPreference("webrtc.ip_handling_policy", "disable_non_proxied_udp",
                        out error);
                }
            });
            Browser.MenuHandler = new MenuHandler();
            Browser.RequestHandler = RequestHandlerCustom;

            if (DominatorAccountModel.AccountBaseModel.AccountNetwork != SocialNetworks.Facebook)
                Browser.LifeSpanHandler = new BrowserLifeSpanHandler();

            SetUrlWidth(SystemParameters.PrimaryScreenWidth);

            var url = CustomUse && !string.IsNullOrEmpty(TargetUrl) ? TargetUrl : GetNetworksLoginUrl();
            SetUrl(url);
            Browser.IsBrowserInitializedChanged += LoadSettings;
        }

        public BrowserWindow(DominatorAccountModel dominatorAccountModel, CancellationToken cancellationToken,
            string targetUrl = "", bool customUse = false, bool skipAd = false, bool isNeedResourceData = false,
            bool browserLoginMessageToDisplay = true) : this(dominatorAccountModel, targetUrl, customUse, skipAd,
            isNeedResourceData, browserLoginMessageToDisplay)
        {
            _token = cancellationToken;
        }

        private RequestHandlerCustom RequestHandlerCustom { get; }

        private ProxyRequestHandler ProxyRequestHandler { get; set; }
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void Sleep(double seconds = 1)
        {
            Task.Delay(TimeSpan.FromSeconds(seconds)).Wait(_token);
        }

        #region Properties

        private string TargetUrl { get; } = string.Empty;

        private bool CustomUse { get; }

        public bool VerifyingAccount { get; set; }

        public ICommand SearchCommand { get; }

        private DominatorAccountModel _dominatorAccountModel;

        public bool IsCaptchaSolved { get; set; }
        public string CaptchaResponse { get; set; }

        public DominatorAccountModel DominatorAccountModel
        {
            get => _dominatorAccountModel;
            set
            {
                _dominatorAccountModel = value;
                OnPropertyChanged(nameof(DominatorAccountModel));
            }
        }

        private string _browserName;

        public string BrowserName
        {
            get => _browserName;
            set
            {
                if (value == _browserName)
                    return;
                _browserName = value;
                OnPropertyChanged(nameof(BrowserName));
            }
        }

        private string _searchUrl;

        public string SearchUrl
        {
            get => _searchUrl;
            set
            {
                if (_searchUrl == value)
                    return;
                _searchUrl = value;
                OnPropertyChanged(nameof(SearchUrl));
            }
        }

        private double _searchUrlTextBoxWidth = 830;

        public double SearchUrlTextBoxWidth
        {
            get => _searchUrlTextBoxWidth;
            set
            {
                if (_searchUrlTextBoxWidth == value)
                    return;
                _searchUrlTextBoxWidth = value;
                OnPropertyChanged(nameof(SearchUrlTextBoxWidth));
            }
        }

        public bool BrowserLoginMessage { get; set; } = true;

        public bool IsLoaded { get; set; }

        public bool IsLoggedIn { get; set; }

        private bool IsNeedResourceData { get; }

        #endregion

        #region CefSharp Utilities

        /// <summary>
        ///     Set Account Model Cookies into the browser
        /// </summary>
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
                        {
                            ProxyRequestHandler = new ProxyRequestHandler(
                                DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                                DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword, this,
                                IsNeedResourceData, DominatorAccountModel.AccountBaseModel.AccountNetwork);

                            Browser.RequestHandler = ProxyRequestHandler;
                        }

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
                            requestContext.SetPreference("proxy", dictProxyIpPort, out error);
                        }
                        else
                        {
                            var dictProxyIpPort = new Dictionary<string, object> { { "mode", "direct" } };

                            string error;
                            var success = requestContext.SetPreference("proxy", dictProxyIpPort, out error);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(ex.StackTrace);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var homePage = GetNetworksLoginUrl();
            if (!CustomUse)
                Browser.Load(homePage);

            Browser.LoadingStateChanged += BrowserOnLoaded;
        }

        private void BrowserOnLoaded(object sender, LoadingStateChangedEventArgs loadingStateChangedEventArgs)
        {
            try
            {
                IsLoaded = false;

                Dispatcher.Invoke(() =>
                {
                    try
                    {
                        IsLoaded = Browser.IsLoaded;
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }
                });
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
            }
        }

        public void GoToUrl(string url = null)
        {
            Browser.Load(url ?? SearchUrl);
        }

        public void Dispose()
        {
            Browser.Dispose();
        }

        public void LoadPostPage(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                Browser.Load(TargetUrl);
                Browser.LoadingStateChanged -= BrowserOnLoaded;
            }
        }


        public bool IsDisposed => Browser.IsDisposed;


        public bool FoundAd { get; set; }


        public void LoadPostPage()
        {
            if (CustomUse || string.IsNullOrEmpty(TargetUrl))
                return;
            Browser.Load(TargetUrl);
            Browser.LoadingStateChanged -= BrowserOnLoaded;
        }

        private string GetLoggedInPageSource()
        {
            return !string.IsNullOrEmpty(TargetUrl) && TargetUrl != "Not Published Yet"
                ? Browser.GetSourceAsync().Result
                : string.Empty;
        }

        /// <summary>
        ///     Get Current PageSource
        /// </summary>
        /// <returns></returns>
        public string GetPageSource()
        {
            return Browser.GetSourceAsync().Result;
        }

        public void SelectAllText()
        {
            Browser.SelectAll();
        }

        public async Task<string> PageText()
        {
            return await Browser.GetTextAsync();
        }

        public async Task SetCookie()
        {
            try
            {
                if (DominatorAccountModel.Cookies.Count == 0)
                    return;

                var cookies = DominatorAccountModel.BrowserCookies.Count != 0
                              && DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook
                    ? DominatorAccountModel.BrowserCookies
                    : DominatorAccountModel.Cookies;

                var callBack = new TaskCompletionCallback();

                foreach (var accCookie in cookies)
                {
                    var cook = (System.Net.Cookie)accCookie;

                    var cefCookie = new Cookie
                    {
                        HttpOnly = cook.HttpOnly,
                        Name = cook.Name,
                        Value = cook.Value,
                        Expires = cook.Expires,
                        Domain = cook.Domain,
                        Secure = cook.Secure,
                        Path = cook.Path
                    };

                    var url = "";
                    if (cefCookie.Domain.Contains("www."))
                        url = "https://" + cefCookie.Domain.TrimStart('.');
                    else if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                             cefCookie.Domain.Contains("pinterest"))
                        url = "https://" +
                              (cefCookie.Domain.StartsWith(".pinterest") || cefCookie.Domain.StartsWith("pinterest")
                                  ? "www."
                                  : "") + cefCookie.Domain.TrimStart('.');
                    else
                        url = "https://www" + (!cefCookie.Domain.StartsWith(".") ? "." : "") + cefCookie.Domain;

                    var set = Browser.RequestContext.GetCookieManager(callBack).SetCookie(url, cefCookie);

                    //if (!set) { /*Is cookie set ?*/ }
                }

                if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube && !CustomUse)
                    SetUrl(SocialHomeUrls());

                // Just to check that how many cookie was inserted
                var cefInitialCookies = await BrowserCookies(callBack);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SetUrl(string url)
        {
            if (SearchUrl == url)
                return;
            Browser.Address = SearchUrl = url;
        }

        private void SetUrlWidth( double width)
        {
            if (width <= 1280)
                SearchUrlTextBoxWidth = 630;
            else if (width > 1280 && width < 1440)
                SearchUrlTextBoxWidth = 730;
        }

        public async Task BrowserSetCookie()
        {
            try
            {
                if (DominatorAccountModel.BrowserCookies.Count == 0)
                    return;

                var callBack = new TaskCompletionCallback();

                foreach (var accCookie in DominatorAccountModel.BrowserCookies)
                {
                    var cook = (System.Net.Cookie)accCookie;

                    var cefCookie = new Cookie
                    {
                        HttpOnly = cook.HttpOnly,
                        Name = cook.Name,
                        Value = cook.Value,
                        Expires = cook.Expires,
                        Domain = cook.Domain,
                        Secure = cook.Secure,
                        Path = cook.Path
                    };

                    var url = "";
                    if (cefCookie.Domain.Contains("www."))
                        url = "https://" + cefCookie.Domain.TrimStart('.');
                    else if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Pinterest &&
                             cefCookie.Domain.Contains("pinterest"))
                        url = "https://" +
                              (cefCookie.Domain.StartsWith(".pinterest") || cefCookie.Domain.StartsWith("pinterest")
                                  ? "www."
                                  : "") + cefCookie.Domain.TrimStart('.');
                    else
                        url = "https://www" + (!cefCookie.Domain.StartsWith(".") ? "." : "") + cefCookie.Domain;

                    var set = Browser.RequestContext.GetCookieManager(callBack).SetCookie(url, cefCookie);

                    //if (!set) { /*Is cookie set ?*/ }
                }

                if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube && !CustomUse)
                    SetUrl(SocialHomeUrls());

                // Just to check that how many cookie was inserted
                var cefInitialCookies = await BrowserCookies(callBack);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task ClearCookies()
        {
            try
            {
                var callBack = new TaskCompletionCallback();

                var set = Browser.RequestContext.GetCookieManager(callBack).DeleteCookiesAsync("", "");

                Refresh();

                await Task.Delay(500, _token);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async Task<bool> SaveCookies(bool showLoginSuccessLog = true)
        {
            if (IsLoggedIn) return false;

            try
            {
                await Task.Delay(1000, _token);

                IsLoggedIn = true;

                DominatorAccountModel.Cookies = await BrowserCookiesIntoModel();
                DominatorAccountModel.IsUserLoggedIn = true;
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                    .SaveToBinFile();
                if (showLoginSuccessLog)
                    CustomLog("Browser login successful.");
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> BrowserSaveCookies(bool showLoginSuccessLog = true)
        {
            if (IsLoggedIn) return false;

            try
            {
                await Task.Delay(1000, _token);

                IsLoggedIn = true;

                DominatorAccountModel.BrowserCookies = await BrowserCookiesIntoModel();

                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateBrowserCookies(DominatorAccountModel.BrowserCookies)
                    .SaveToBinFile();
                DominatorAccountModel.IsUserLoggedIn = true;
                if (showLoginSuccessLog)
                    CustomLog("Browser login successful.");
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }

        public async Task<CookieCollection> BrowserCookiesIntoModel()
        {
            try
            {
                var cookieCollection = new CookieCollection();

                foreach (var item in await BrowserCookies())
                    try
                    {
                        var cookie = new System.Net.Cookie
                        {
                            Name = item.Name,
                            Value = item.Value,
                            Domain = item.Domain,
                            Path = item.Path,
                            Secure = item.Secure
                        };
                        if (item.Expires != null)
                            cookie.Expires = (DateTime)item.Expires;

                        cookieCollection.Add(cookie);
                    }
                    catch
                    {
                        /*ignored*/
                    }

                return cookieCollection;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return null;
            }
        }

        public async Task<List<Cookie>> BrowserCookies(TaskCompletionCallback callBack = null)
        {
            return await Browser.RequestContext.GetCookieManager(callBack ?? new TaskCompletionCallback())
                .VisitAllCookiesAsync();
        }

        #endregion

        #region Window UI Interaction

        private void ButtonCheckIp_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Load("https://app.multiloginapp.com/WhatIsMyIP");
        }

        private void ButtonSendCaptcha_OnClick(object sender, RoutedEventArgs e)
        {
            string script = "(function() {return document.getElementById('g-recaptcha-response').value;})();";

            var javascriptResponse = Browser.GetMainFrame().EvaluateScriptAsync(script);
            Thread.Sleep(2000);
            var response = javascriptResponse.Result;
            if (response.Success && !string.IsNullOrEmpty(response.Result.ToString()))
            {
                CaptchaResponse = response.Result.ToString();
                IsCaptchaSolved = response.Success;
            }
            else
            {
                var message = "CaptchaErrorMessage".FromResourceDictionary();
                Dialog.ShowDialog("LangKeyCaptchaError".FromResourceDictionary(), message);
            }
        }

        private void ButtonLogin_OnClick(object sender, RoutedEventArgs e)
        {
            var homePage =
                DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube &&
                DominatorAccountModel.IsUserLoggedIn
                    ? SocialHomeUrls()
                    : GetNetworksLoginUrl();
            Browser.Load(homePage);
        }

        private void BtnCopyUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(SearchUrl))
                    return;
                new AutoItTool().CopyToClip(SearchUrl);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnPasteUrl_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UrlBar.Text = new AutoItTool().GetLastCopied();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            Dispose();
        }

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {
            GoBack();
        }

        private void ButtonForward_OnClick(object sender, RoutedEventArgs e)
        {
            GoForward();
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            Refresh();
        }

        public void GoBack(int nTimes = 1)
        {
            while (nTimes > 0)
            {
                if (!Browser.CanGoBack)
                    return;
                Browser.GetBrowser().GoBack();
                nTimes--;
                if (nTimes != 0)
                    Sleep(0.5);
            }
        }

        public void GoForward(int nTimes = 1)
        {
            while (nTimes > 0)
            {
                if (!Browser.CanGoForward)
                    return;
                Browser.Forward();
                nTimes--;
                if (nTimes != 0)
                    Sleep(0.5);
            }
        }

        public void Refresh()
        {
            Browser.Reload();
        }

        #endregion

        #region Browser Automation Changes

        public void ChooseFileFromDialog(string filePath = "", List<string> pathList = null)
        {
            var fileDialogHandler = new TempFileDialogHandler(this, filePath, pathList);
            Browser.DialogHandler = fileDialogHandler;
        }

        /// <summary>
        ///     Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        public string GetElementValue(ActType actType, string element, double delayBefore = 0, int clickIndex = 0)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (Browser.IsDisposed) return "";
            switch (actType)
            {
                case ActType.GetValueByName:
                    return Browser.EvaluateScriptAsync($"document.getElementsByName('{element}')[{clickIndex}].value")
                               .Result?.Result?.ToString() ?? "";
                case ActType.GetLengthByClass:
                    return Browser.EvaluateScriptAsync($"document.getElementsByClassName('{element}').length").Result
                               ?.Result?.ToString() ?? "";
                default:
                    return "";
            }
        }

        /// <summary>
        ///     Press any key n times with delay between each pressed
        /// </summary>
        /// <param name="n">Number of pressing</param>
        /// <param name="delay">Delay between each press  (In milliseconds)</param>
        /// <param name="ke">Browser KeyEvent</param>
        /// <param name="winKeyCode">WindowsKeycode of any key in keyboard</param>
        /// ///
        /// <param name="delayAtLast">Set delay at last (In seconds)</param>
        /// <param name="isShiftDown"></param>
        public void PressAnyKey(int n = 1, double delay = 1, KeyEvent ke = new KeyEvent(),
            int winKeyCode = 0, double delayAtLast = 0, bool isShiftDown = false)
        {
            if (winKeyCode != 0)
                ke.WindowsKeyCode = winKeyCode;

            if (isShiftDown) ke.Modifiers = CefEventFlags.ShiftDown;

            if (Browser.IsDisposed) return;

            for (var i = 0; i < n; i++)
            {
                Sleep(delay);
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            }

            if (delayAtLast > 0)
                Sleep(delayAtLast);
        }

        /// <summary>
        ///     Get the Mouse to click on a specific location(xLoc,yLoc)
        /// </summary>
        /// <param name="xLoc">x-cordinate location</param>
        /// <param name="yLoc">y-cordinate location</param>
        /// <param name="mouseButton">Mouse Button Type</param>
        /// <param name="delayBefore">Delay before click</param>
        /// <param name="delayAfter">Delay after click</param>
        public void MouseClick(int xLoc, int yLoc, MouseButtonType mouseButton = MouseButtonType.Left,
            double delayBefore = 0, double delayAfter = 0)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (Browser.IsDisposed) return;

            // mouseUp(4th parameter) = false , MouseButton to be pressed
            Browser.GetBrowser().GetHost().SendMouseClickEvent(xLoc, yLoc, mouseButton, false, 1, CefEventFlags.None);
            Sleep(0.1);
            // mouseUp(4th parameter) = true , MouseButton to be released
            Browser.GetBrowser().GetHost().SendMouseClickEvent(xLoc, yLoc, mouseButton, true, 1, CefEventFlags.None);

            if (delayAfter > 0)
                Sleep(delayAfter);
        }

        /// <summary>
        ///     Enter Characters in TextBox
        /// </summary>
        /// <param name="charString">String to be entered</param>
        /// <param name="typingDelay">Delay between typing</param>
        /// <param name="delayBefore">Set delay before the typing</param>
        /// <param name="delayAtLast">Set delay at last</param>
        public void EnterChars(string charString, double typingDelay = 0.09, double delayBefore = 0,
            double delayAtLast = 0)
        {
            if (string.IsNullOrEmpty(charString)) return;

            if (delayBefore > 0)
                Sleep(delayBefore);

            var ke = new KeyEvent { FocusOnEditableField = true, IsSystemKey = false, Type = KeyEventType.Char };

            if (Browser.IsDisposed) return;

            charString.ToList().ForEach(x =>
            {
                ke.WindowsKeyCode = x;
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
                Sleep(typingDelay);
            });
            if (delayAtLast > 0)
                Sleep(delayAtLast);
        }

        public async Task<string> GetPageSourceAsync(double delay)
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(delay), _token);
                return await Browser.GetSourceAsync();
            }
            catch (ArgumentException)
            {
            }
            // ReSharper disable once EmptyGeneralCatchClause
            catch
            {
            }

            return string.Empty;
        }

        public async Task<string> GetPageSourceAsync()
        {
            try
            {
                await Task.Delay(TimeSpan.FromSeconds(1), _token);
                return await Browser.GetSourceAsync();
            }
            catch (ArgumentException)
            {
            }
            catch
            {
                // ignored
            }

            return string.Empty;
        }

        public async Task<string> GoToCustomUrl(string url, int delayAfter = 0)
        {
            Browser.Load(url);
            await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
            return await Browser.GetSourceAsync();
        }

        public async Task PressAnyKeyUpdated(int winKeyCode = 13, int n = 1, int delay = 90, double delayAtLast = 0,
            bool isShiftDown = false)
        {
            var ke = new KeyEvent();
            if (winKeyCode != 0)
                ke.WindowsKeyCode = winKeyCode;

            if (isShiftDown) ke.Modifiers = CefEventFlags.ShiftDown;

            if (Browser.IsDisposed) return;

            for (var i = 0; i < n; i++)
            {
                await Task.Delay(delay, _token);
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            }

            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), _token);
        }

        public async Task PressCombinedKey(int winFirstKeyCode, int winSecondKeyCode,
            double delayAtLast = 0)
        {
            if (Browser.IsDisposed) return;
            var ke = new KeyEvent
            {
                WindowsKeyCode = winFirstKeyCode,
                Type = KeyEventType.RawKeyDown
            };

            var ke2 = new KeyEvent
            {
                WindowsKeyCode = winSecondKeyCode,
                Type = KeyEventType.RawKeyDown
            };

            Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            await Task.Delay(100, _token);
            Browser.GetBrowser().GetHost().SendKeyEvent(ke2);
            await Task.Delay(90, _token);

            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), _token);
        }

        public async Task EnterCharsAsync(string charString, double typingDelay = 0.09, double delayBefore = 0,
            double delayAtLast = 0)
        {
            if (string.IsNullOrEmpty(charString)) return;

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            var ke = new KeyEvent { FocusOnEditableField = true, IsSystemKey = false, Type = KeyEventType.Char };

            if (Browser.IsDisposed) return;

            foreach (var caharacter in charString.ToList())
            {
                ke.WindowsKeyCode = caharacter;
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
                await Task.Delay(TimeSpan.FromSeconds(typingDelay), _token);
            }

            if (delayAtLast > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAtLast), _token);
        }

        public async Task BrowserActAsync(ActType actType, AttributeType attributeType, string attributeValue,
            string value = "", double delayBefore = 0, double delayAfter = 0, int index = 0, int scrollByPixel = 100)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            if (Browser.IsDisposed)
                return;

            if (!string.IsNullOrEmpty(attributeValue) && attributeValue.Contains(@"\"))
                attributeValue = attributeValue.Replace(@"\", "\\\\");

            switch (actType)
            {
                case ActType.EnterByQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].value= '{value}'");
                    break;

                case ActType.EnterValue:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].value= '{value}'");
                    break;

                case ActType.ActByQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].click()");
                    break;

                case ActType.ScrollWindow:
                    Browser.ExecuteScriptAsync($"window.scrollBy(0, {scrollByPixel});");
                    break;

                case ActType.ScrollIntoView:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].scrollIntoView()");
                    break;

                case ActType.ScrollIntoViewQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].scrollIntoView()");
                    break;

                case ActType.CustomActType:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].{value}");
                    break;

                case ActType.CustomActByQueryType:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{index}].{value}");
                    break;

                case ActType.EnterValueById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{attributeValue}').value= '{value}'");
                    break;
                case ActType.ClickById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{attributeValue}').click()");
                    break;

                default:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{index}].{actType.GetDescriptionAttr()}");
                    break;
            }

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
        }

        /// <summary>
        ///     Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="delayAfter">delay after the action (In seconds)</param>
        /// <param name="value">value which is going to be entered</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        /// <param name="attributeType"></param>
        /// <param name="attributeValue"></param>
        /// <param name="scrollByPixel"></param>
        public void BrowserAct(ActType actType, string element = "", double delayBefore = 0, double delayAfter = 0,
            string value = "", int clickIndex = 0,
            AttributeType attributeType = AttributeType.Null, string attributeValue = "", int scrollByPixel = 100)
        {
            if (delayBefore > 0)
                Sleep(delayBefore);

            if (Browser.IsDisposed) return;

            if (!string.IsNullOrEmpty(value) && value.Contains(@"\"))
                value = value.Replace(@"\", "\\\\");

            switch (actType)
            {
                case ActType.ClickByClass:
                    Browser.ExecuteScriptAsync($"document.getElementsByClassName('{element}')[{clickIndex}].click()");
                    break;

                case ActType.ClickById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{element}').click()");
                    break;

                case ActType.ClickByName:
                    Browser.ExecuteScriptAsync($"document.getElementsByName('{element}')[{clickIndex}].click()");
                    break;

                case ActType.EnterValueByClass:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsByClassName('{element}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterValueById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{element}').value= '{value}'");
                    break;

                case ActType.EnterValueByName:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsByName('{element}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterByQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterValue:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.ActByQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].click()");
                    break;

                case ActType.ScrollWindow:
                    Browser.ExecuteScriptAsync($"window.scrollBy(0, {scrollByPixel});");
                    break;

                case ActType.ScrollIntoView:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].scrollIntoView()");
                    break;

                case ActType.ScrollIntoViewQuery:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].scrollIntoView()");
                    break;

                case ActType.CustomActType:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{value}");
                    break;

                case ActType.CustomActByQueryType:
                    Browser.ExecuteScriptAsync(
                        $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].{value}");
                    break;

                default:
                    Browser.ExecuteScriptAsync(
                        $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{actType.GetDescriptionAttr()}");
                    break;
            }

            if (delayAfter > 0)
                Sleep(delayAfter);
        }


        public async Task MouseClickAsync(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
            MouseClickType mouseClickType = MouseClickType.Left)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            var mouseButton = mouseClickType == MouseClickType.Left ? MouseButtonType.Left
                : mouseClickType == MouseClickType.Right ? MouseButtonType.Right : MouseButtonType.Middle;

            if (Browser.IsDisposed) return;

            // mouseUp(4th parameter) = false , MouseButton to be pressed
            Browser.GetBrowser().GetHost().SendMouseClickEvent(xLoc, yLoc, mouseButton, false, 1, CefEventFlags.None);
            await Task.Delay(100, _token);
            // mouseUp(4th parameter) = true , MouseButton to be released
            Browser.GetBrowser().GetHost().SendMouseClickEvent(xLoc, yLoc, mouseButton, true, 1, CefEventFlags.None);

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
        }


        public async Task MouseScrollAsync(int xLoc, int yLoc, int scrollByXLoc = 0, int scrollByYLoc = 0,
            double delayBefore = 0, double delayAfter = 0,
            int clickLeavEvent = 0)
        {

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            if (Browser.IsDisposed) return;


            // mouseUp(4th parameter) = false , MouseButton to be pressed
            Browser.GetBrowser().GetHost().SendMouseMoveEvent(new MouseEvent(xLoc, yLoc, CefEventFlags.None), false);
            await Task.Delay(100, _token);

            Browser.GetBrowser().GetHost().SendMouseWheelEvent(new MouseEvent(xLoc, yLoc, CefEventFlags.None),
                scrollByXLoc, scrollByYLoc);

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
        }

        public async Task MouseHoverAsync(int xLoc, int yLoc, double delayBefore = 0, double delayAfter = 0,
            int clickLeavEvent = 0)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            if (Browser.IsDisposed) return;


            // mouseUp(4th parameter) = false , MouseButton to be pressed
            Browser.GetBrowser().GetHost().SendMouseMoveEvent(new MouseEvent(xLoc, yLoc, CefEventFlags.None), false);
            await Task.Delay(100, _token);
            // mouseUp(4th parameter) = true , MouseButton to be released
            //Browser.GetBrowser().GetHost().SendMouseClickEvent(xLoc, yLoc, mouseButton, true, 1, CefEventFlags.RightMouseButton);

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
        }


        public async Task<List<string>> GetListInnerHtml(ActType actType, AttributeType attributeType,
            string attributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, string value = "")
        {
            if (Browser.IsDisposed)
                return
                    new List<string>();

            var listNodes = new List<string>();

            var itemCount = actType == ActType.ActByQuery
                ? int.Parse(await GetElementValueAsync(ActType.GetLengthByQuery, attributeType, attributeValue)) - 1
                : int.Parse(await GetElementValueAsync(ActType.GetLength, attributeType, attributeValue)) - 1;

            while (itemCount >= 0)
            {
                listNodes.Add(await GetElementValueAsync(actType, attributeType, attributeValue, valueType
                    , clickIndex: itemCount));
                itemCount--;
            }

            return listNodes;
        }

        public async Task<List<string>> GetListInnerHtmlChildElement(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {
            if (Browser.IsDisposed)
                return
                    new List<string>();

            var listNodes = new List<string>();

            var itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetChildElementValueAsync(
                                                                          ActType.GetLengthByCustomQuery,
                                                                          parentAttributeType,
                                                                          parentAttributeValue, childAttributeName,
                                                                          childAttributeValue, valueType, delayBefore,
                                                                          parentIndex, childIndex)) - 1
                : actType == ActType.GetValue ? int.Parse(await GetChildElementValueAsync(ActType.GetLength,
                                                    parentAttributeType,
                                                    parentAttributeValue, childAttributeName, childAttributeValue,
                                                    valueType, delayBefore, parentIndex, childIndex)) - 1
                : int.Parse(await GetChildElementValueAsync(ActType.GetLengthByQuery, parentAttributeType,
                      parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore,
                      parentIndex, childIndex)) - 1;

            while (itemCount >= 0)
            {
                listNodes.Add(await GetChildElementValueAsync(actType, parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    itemCount));
                itemCount--;
            }

            return listNodes;
        }


        public async Task<int> GetItemCountInnerHtml(ActType actType, AttributeType attributeType,
            string attributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, string value = "")
        {
            if (Browser.IsDisposed)
                return 0;

            var listNodes = new List<string>();

            var itemCount = actType == ActType.ActByQuery
                ? int.Parse(await GetElementValueAsync(ActType.GetLengthByQuery, attributeType, attributeValue))
                : int.Parse(await GetElementValueAsync(ActType.GetLength, attributeType, attributeValue));

            return itemCount;
        }

        public async Task<int> GetCountInnerHtmlChildElement(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {
            if (Browser.IsDisposed)
                return 0;

            var itemCount = actType == ActType.CustomActByQueryType ? int.Parse(await GetChildElementValueAsync(
                    ActType.GetLengthByCustomQuery, parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex))
                : actType == ActType.GetValue ? int.Parse(await GetChildElementValueAsync(ActType.GetLength,
                    parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex)) : int.Parse(await GetChildElementValueAsync(ActType.GetLengthByQuery,
                    parentAttributeType,
                    parentAttributeValue, childAttributeName, childAttributeValue, valueType, delayBefore, parentIndex,
                    childIndex));

            return itemCount;
        }

        public async Task<string> GetElementValueAsync(ActType actType, AttributeType attributeType,
            string attributeValue, ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int clickIndex =
                0
            , string value = "")
        {
            JavascriptResponse jsResponse = null;

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);
            try
            {
                var z =
                    $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].{valueType.GetDescriptionAttr()}";

                if (Browser.IsDisposed) return "";

                if (!Browser.CanExecuteJavascriptInMainFrame)
                    return "";

                switch (actType)
                {
                    case ActType.GetValue:
                        jsResponse = await Browser.EvaluateScriptAsync(
                            $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{valueType.GetDescriptionAttr()}");
                        break;

                    case ActType.GetLength:
                        jsResponse =
                            await Browser.EvaluateScriptAsync(
                                $"document.getElementsBy{attributeType}('{attributeValue}').length");
                        break;

                    case ActType.GetLengthByQuery:
                        return (await Browser.EvaluateScriptAsync(
                                   $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]').length")
                               )?.Result?.ToString() ?? "0";

                    case ActType.GetLengthByCustomQuery:
                        jsResponse =
                            await Browser.EvaluateScriptAsync(
                                $"document.getElementsBy{attributeType}('{attributeValue}').{value}.length");
                        break;

                    case ActType.GetAttribute:
                        jsResponse = await Browser.EvaluateScriptAsync(
                            $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].getAttribute('{valueType.GetDescriptionAttr()}')");
                        break;

                    case ActType.CustomActByQueryType:
                        jsResponse = await Browser.EvaluateScriptAsync(
                            $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].{value}");
                        break;
                    case ActType.CustomActType:
                        jsResponse = await Browser.EvaluateScriptAsync(
                            $"document.getElementsBy{attributeType}('{attributeValue}')[{clickIndex}].{value}.{valueType.GetDescriptionAttr()}");
                        break;
                    default:
                        jsResponse = await Browser.EvaluateScriptAsync(
                            $"document.querySelectorAll('[{attributeType.GetDescriptionAttr()}=\"{attributeValue}\"]')[{clickIndex}].{valueType.GetDescriptionAttr()}");
                        break;
                }
            }
            catch
            {
                // ignored
            }

            return jsResponse != null && jsResponse.Success ? jsResponse.Result?.ToString() : "";
        }

        public async Task<string> GetChildElementValueAsync(ActType actType, AttributeType parentAttributeType,
            string parentAttributeValue, AttributeType childAttributeName, string childAttributeValue,
            ValueTypes valueType = ValueTypes.InnerHtml, double delayBefore = 0, int parentIndex = 0,
            int childIndex = 0)
        {
            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            var doc =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}";

            var doc2 =
                $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length";

            if (Browser.IsDisposed) return "";

            if (!Browser.CanExecuteJavascriptInMainFrame)
                return "";

            switch (actType)
            {
                case ActType.GetValue:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.Result?.ToString() ?? "";

                case ActType.GetLength:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length")
                           )?.Result?.ToString() ?? "0";

                case ActType.GetLengthByQuery:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}').length")
                           )?.Result?.ToString() ?? "0";

                case ActType.GetLengthByCustomQuery:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]').length")
                           )?.Result?.ToString() ?? "0";

                case ActType.GetAttribute:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].getAttribute('{valueType.GetDescriptionAttr()}')")
                           )?.Result?.ToString() ?? "";

                case ActType.ActByQuery:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.querySelectorAll('[{parentAttributeType.GetDescriptionAttr()}=\"{parentAttributeValue}\"]')[{parentIndex}].getElementsBy{childAttributeName}('{childAttributeValue}')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.Result?.ToString() ?? "";
                default:
                    return (await Browser.EvaluateScriptAsync(
                               $"document.getElementsBy{parentAttributeType}('{parentAttributeValue}')[{parentIndex}].querySelectorAll('[{childAttributeName.GetDescriptionAttr()}=\"{childAttributeValue}\"]')[{childIndex}].{valueType.GetDescriptionAttr()}")
                           )?.Result?.ToString() ?? "";
            }
        }


        public int LastCurrentCount = -1;

        public async Task ExpandAllSeeMore()
        {
            var postCount = int.Parse(await GetElementValueAsync(ActType.GetLength, AttributeType.ClassName,
                "see_more_link_inner",
                ValueTypes.OuterHtml));

            while (postCount-- > 0)
                await BrowserActAsync(ActType.Click, AttributeType.ClassName, "see_more_link_inner", delayAfter: 0.25,
                    index: postCount);
        }

        public async Task<List<Tuple<int, string, string, string, string>>> ExpandAllAdViewOptions(int postCount,
            int lastCount, int lastCurrentAdCount = 0)
        {

            var tupleAdsDetals = new List<Tuple<int, string, string, string, string>>();
            await Task.Delay(5000, _token);

            while (LastCurrentCount++ <= postCount * (lastCount + 1))
            {
                Browser.ExecuteScriptAsync(
                    $"document.getElementsByClassName('_5jmm _5pat _3lb4')[{LastCurrentCount}].querySelectorAll('[data-testid=\"post_chevron_button\"]')[0].scrollIntoView()");
                var fullAdDetails = await GetElementValueAsync(ActType.GetValue, AttributeType.ClassName,
                    "_5jmm _5pat _3lb4", ValueTypes.OuterHtml, clickIndex: LastCurrentCount);
                if (!fullAdDetails.Contains("sponsored") || !fullAdDetails.Contains("Sponsored"))
                {
                    await Task.Delay(3000, _token);
                    continue;
                }

                await Task.Delay(2000, _token);
                await BrowserActAsync(ActType.ScrollWindow, AttributeType.Null, "", scrollByPixel: -50);
                var javascriptResponse =
                    await ExecuteScriptAsync(
                        $"document.getElementsByClassName('_5jmm _5pat _3lb4')[{LastCurrentCount}].outerHTML");

                var values =
                    Utilities.GetBetween(javascriptResponse.Result.ToString(), "id=\"feed_subtitle",
                        "\""); //_263;2085460778154235;0;3006433072723663;1583140012:8116025295315885125:5:0:32239
                var splittedValues = Regex.Split(values, ";");
                var ownerId = splittedValues[1];
                var postId = splittedValues[3];
                splittedValues = Regex.Split(splittedValues[4], ":");
                var AdId = splittedValues[1];
                var dateTime = splittedValues[0];

                tupleAdsDetals.Add(
                    new Tuple<int, string, string, string, string>(LastCurrentCount, postId, AdId, ownerId, dateTime));
            }

            return tupleAdsDetals;
        }

        public JavascriptResponse ExecuteScript(string script, int delayInSec = 2)
        {
            var resp = Browser.EvaluateScriptAsync(script).Result;
            Task.Delay(TimeSpan.FromSeconds(delayInSec)).Wait(_token);
            return resp;
        }

        public async Task<JavascriptResponse> ExecuteScriptAsync(string script, int delayInSec = 2)
        {
            var resp = await Browser.EvaluateScriptAsync(script);
            await Task.Delay(TimeSpan.FromSeconds(delayInSec), _token);
            return resp;
        }

        public KeyValuePair<int, int> GetXAndY(AttributeType attributeType = AttributeType.Id, string elementName = "",
            int index = 0)
        {
            var xAndY = new KeyValuePair<int, int>();
            var scripty = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().top"
                : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().top";
            var scriptx = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().left"
                : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().left";

            if (ExecuteScript(scriptx, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptx, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scripty, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public async Task<KeyValuePair<int, int>> GetXAndYAsync(AttributeType attributeType = AttributeType.Id,
            string elementName = "", int index = 0,
            string customScriptX = "", string customScriptY = "",
            CoordinateDirection horizontalDirection = CoordinateDirection.Left,
            CoordinateDirection verticalDirection = CoordinateDirection.Top)
        {
            var xAndY = new KeyValuePair<int, int>();

            var scripty = !string.IsNullOrEmpty(customScriptY)
                ? customScriptY
                : attributeType == AttributeType.Id
                    ? $"$('#{elementName}').offset().{verticalDirection.GetDescriptionAttr()}"
                    : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().{verticalDirection.GetDescriptionAttr()}";
            var scriptx = !string.IsNullOrEmpty(customScriptX)
                ? customScriptX
                : attributeType == AttributeType.Id
                    ? $"$('#{elementName}').offset().{horizontalDirection.GetDescriptionAttr()}"
                    : $"document.getElementsByClassName('{elementName}')[{index}].getBoundingClientRect().{horizontalDirection.GetDescriptionAttr()}";


            if ((await ExecuteScriptAsync(scriptx, 0)).Success)
            {
                var scriptResponse = await ExecuteScriptAsync(scriptx, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = await ExecuteScriptAsync(scripty, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public static int ConvertDoubleAndInt(string input)
        {
            var doubleResult = Convert.ToDouble(input);
            return Convert.ToInt32(doubleResult);
        }

        public string GetNetworksLoginUrl()
        {
            switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Facebook:
                    return "https://www.facebook.com";
                case SocialNetworks.Instagram:
                    return "https://www.instagram.com/accounts/login/";
                case SocialNetworks.Twitter:
                    return "https://twitter.com/login";
                case SocialNetworks.Pinterest:
                    return "https://www.pinterest.com/login/";
                case SocialNetworks.LinkedIn:
                    return "https://www.linkedin.com";
                case SocialNetworks.Reddit:
                    return "https://www.reddit.com/login";
                case SocialNetworks.Quora:
                    return "https://www.quora.com/";
                case SocialNetworks.Gplus:
                    return "https://accounts.google.com/signin";
                case SocialNetworks.Youtube:
                    return "https://accounts.google.com/signin";
                case SocialNetworks.Tumblr:
                    return "https://www.tumblr.com/login";
                case SocialNetworks.Social:
                    return "https://www.google.com";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private string SocialHomeUrls()
        {
            switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Gplus:
                    return "https://plus.google.com/";
                case SocialNetworks.Youtube:
                    return "https://www.youtube.com/";
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void CustomLog(string message)
        {
            GlobusLogHelper.log.Info(Log.CustomMessage,
                DominatorAccountModel.AccountBaseModel.AccountNetwork,
                DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", message);
        }

        //For reddit json data
        public async Task<string> GoToCustomUrlAndGetPageSource(string url, string startSearchText, string startEndText,
            int delayAfter = 0)
        {
            var response = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(url))
                    Browser.Load(url);

                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
                var lstResponseStream = ProxyRequestHandler == null
                    ? RequestHandlerCustom.ResourceRequestHandler.ResponseList.DeepCloneObject()
                    : ProxyRequestHandler.ResourceRequestHandler.ResponseList.DeepCloneObject();
                lstResponseStream.RemoveAll(x => x.Data == null);
                var responseStream = lstResponseStream.FirstOrDefault(x =>
                    x.Data.Count() > 0 && GetStringFromByte(x.Data, startSearchText, startEndText));
                if (responseStream != null)
                    response = Encoding.UTF8.GetString(responseStream.Data);
                return response;
            }
            catch
            {
                // ignored
            }

            _token.ThrowIfCancellationRequested();
            return response;
        }

        //For deleting data present in responseList
        public void ClearResources()
        {
            if (ProxyRequestHandler == null)
                RequestHandlerCustom.ResourceRequestHandler.ResponseList.Clear();
            else
                ProxyRequestHandler.ResourceRequestHandler.ResponseList.Clear();
        }

        //For reddit json data
        public async Task<string> GetPageSourceCustomAsync(string startSearchText, string startEndText)
        {
            var response = string.Empty;
            try
            {
                var lstResponseStream = ProxyRequestHandler == null
                    ? RequestHandlerCustom.ResourceRequestHandler.ResponseList.DeepCloneObject()
                    : ProxyRequestHandler.ResourceRequestHandler.ResponseList.DeepCloneObject();
                lstResponseStream.RemoveAll(x => x.Data == null);
                _token.ThrowIfCancellationRequested();
                var responseStream = lstResponseStream.FirstOrDefault(x =>
                    x.Data.Length > 0 && GetStringFromByte(x.Data, startSearchText, startEndText));

                if (responseStream != null)
                    response = Encoding.UTF8.GetString(responseStream.Data);
                return response;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch
            {
                // ignored
            }

            return response;
        }

        //To check reddit json data
        private bool GetStringFromByte(byte[] data, string startSearchText, string startEndText)
        {
            try
            {
                var searchText = Encoding.UTF8.GetString(data);
                if (searchText.Contains(startSearchText) && searchText.Contains(startEndText))
                    return true;
                return false;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        //Get json data for pagination
        public async Task<string> GetPaginationData(string startSearchText, bool isContains = false
            , string endString = "")
        {
            var response = string.Empty;
            try
            {
                await Task.Delay(10, _token);
                var lstResponseStream = ProxyRequestHandler == null
                    ? RequestHandlerCustom.ResourceRequestHandler.ResponseList.DeepCloneObject()
                    : ProxyRequestHandler.ResourceRequestHandler.ResponseList.DeepCloneObject();
                lstResponseStream.RemoveAll(x => x.Data == null);
                var responseStream = lstResponseStream.FirstOrDefault(x =>
                    x.Data.Count() > 0 && GetPaginatoinDataFromByte(x.Data, startSearchText, isContains, endString));
                if (responseStream != null)
                    response = Encoding.UTF8.GetString(responseStream.Data);
                return response;
            }
            catch
            {
                // ignored
            }

            _token.ThrowIfCancellationRequested();
            return response;
        }


        public async Task<List<string>> GetPaginationDataList(string startSearchText, bool isContains = false
            , string endString = "")
        {
            var responseList = new List<string>();
            try
            {
                await Task.Delay(10, _token);
                var lstResponseStream = ProxyRequestHandler == null
                    ? RequestHandlerCustom.ResourceRequestHandler.ResponseList.DeepCloneObject()
                    : ProxyRequestHandler.ResourceRequestHandler.ResponseList.DeepCloneObject();
                lstResponseStream.RemoveAll(x => x.Data == null);
                var responseStreamList = lstResponseStream.Where(x =>
                    x.Data.Count() > 0 && GetPaginatoinDataFromByte(x.Data, startSearchText, isContains, endString));
                foreach (var responseStream in responseStreamList)
                {
                    _token.ThrowIfCancellationRequested();
                    try
                    {
                        responseList.Add(Encoding.UTF8.GetString(responseStream.Data));
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                return responseList;
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch
            {
                // ignored
            }

            _token.ThrowIfCancellationRequested();
            return responseList;
        }

        //To check reddit json data 
        private bool GetPaginatoinDataFromByte(byte[] data, string startSearchText, bool isContains = false,
            string endString = "")
        {
            try
            {
                //for (;;);{__ar:
                var searchText = Encoding.UTF8.GetString(data);
                if (isContains && searchText.Contains(startSearchText))
                    return true;
                if (isContains && string.IsNullOrEmpty(endString) && searchText.Contains(endString) &&
                    searchText.Contains(startSearchText))
                    return true;
                if (searchText.StartsWith(startSearchText))
                    return true;
                return false;
            }
            catch
            {
                // ignored
            }

            return false;
        }

        //Get json data list for pagination(for pinterest)
        public async Task<List<string>> GetPaginationDataList(string startSearchText, bool isContains = false)
        {
            var response = string.Empty;
            var lstJsonData = new List<string>();
            try
            {
                await Task.Delay(100, _token);
                var lstResponseStream = new List<MemoryStreamResponseFilter>();

                var isSuccess = false;

                int chkCount = 0;

                while (!isSuccess && chkCount < 25)
                {
                    _token.ThrowIfCancellationRequested();
                    try
                    {
                        lstResponseStream = ProxyRequestHandler == null
                            ? RequestHandlerCustom.ResourceRequestHandler.ResponseList.DeepCloneObject()
                            : ProxyRequestHandler.ResourceRequestHandler.ResponseList.DeepCloneObject();
                        isSuccess = true;
                    }
                    catch
                    {
                        // ignored
                    }
                    chkCount++;
                }

                if (lstResponseStream.Count == 0)
                {
                    lstResponseStream = ProxyRequestHandler == null
                        ? RequestHandlerCustom.ResourceRequestHandler.ResponseList
                        : ProxyRequestHandler.ResourceRequestHandler.ResponseList;
                }

                lstResponseStream.RemoveAll(x => x.Data == null);
                var responseStream = lstResponseStream.Where(x =>
                    x.Data != null && x.Data.Length > 0 && GetPaginatoinDataFromByte(x.Data, startSearchText, isContains));
                foreach (var v in responseStream)
                {
                    _token.ThrowIfCancellationRequested();
                    lstJsonData.Add(response = Encoding.UTF8.GetString(v.Data));
                }
            }
            catch
            {
                // ignored
            }
            finally
            {
                RequestHandlerCustom.ResourceRequestHandler.ResponseList = new List<MemoryStreamResponseFilter>();
            }

            _token.ThrowIfCancellationRequested();
            return lstJsonData;
        }

        public string CurrentUrl()
        {
            var urlNow = "";
            if (!Dispatcher.CheckAccess())
                Dispatcher.Invoke(delegate { urlNow = Browser.Address; });
            else
                urlNow = Browser.Address;
            return urlNow;
        }

        public KeyValuePair<int, int> GetEndXAndY(AttributeType attributeType = AttributeType.Id,
            string elementName = "")
        {
            var xAndY = new KeyValuePair<int, int>();
            var scriptY = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().bottom"
                : $"document.getElementsByClassName('{elementName}')[0].getBoundingClientRect().bottom";
            var scriptX = attributeType == AttributeType.Id
                ? $"$('#{elementName}').offset().right"
                : $"document.getElementsByClassName('{elementName}')[0].getBoundingClientRect().right";

            if (ExecuteScript(scriptX, 0).Success)
            {
                var scriptResponse = ExecuteScript(scriptX, 0);
                var x = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                scriptResponse = ExecuteScript(scriptY, 0);
                var y = ConvertDoubleAndInt(scriptResponse.Result.ToString());
                xAndY = new KeyValuePair<int, int>(x, y);
                return xAndY;
            }

            return xAndY;
        }

        public async Task<IFrame> GetFrame(string url)
        {
            IFrame frame = null;

            var identifiers = Browser.GetBrowser().GetFrameIdentifiers();

            foreach (var i in identifiers)
            {
                var v = Browser.GetBrowser().GetFrameNames();
                frame = Browser.GetBrowser().GetFrame(i);
                if (frame.Url.Contains(url))
                    return frame;
                var document = await frame.GetSourceAsync();
            }

            return null;
        }

        public async Task<string> GetElementValueAsyncFromFrame(IFrame frame, string script)
        {
            await Task.Delay(1000, _token);
            var jsResponse = await frame.EvaluateScriptAsync(script);
            return jsResponse.Success ? jsResponse.Result?.ToString() : jsResponse.Message;
        }

        public async Task ExecuteJSAsyncFromFrame(IFrame frame, string script)
        {
            await Task.Delay(10000, _token);

            frame.ExecuteJavaScriptAsync(script);
        }

        public async Task SelectTextAsync(int stratXlocation, int startYLocation, int moveToXLocation,
            int moveToYLocation, double delayBefore = 0, double delayAfter = 0,
            int clickLeavEvent = 0)
        {
            var mouseButton = MouseButtonType.Left;

            if (delayBefore > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayBefore), _token);

            if (Browser.IsDisposed) return;

            await MouseClickAsync(stratXlocation, startYLocation);
            await Task.Delay(1000, _token);

            //Browser.GetBrowser().GetHost().SendMouseClickEvent(stratXlocation + moveToXLocation, moveToYLocation, mouseButton, true, 0, CefEventFlags.ShiftDown);
            Browser.GetBrowser().GetHost().SendMouseClickEvent(stratXlocation + moveToXLocation, moveToYLocation,
                mouseButton, false, 1, CefEventFlags.ShiftDown);
            Browser.GetBrowser().GetHost().SendMouseClickEvent(stratXlocation + moveToXLocation, moveToYLocation,
                mouseButton, true, 1, CefEventFlags.ShiftDown);

            if (delayAfter > 0)
                await Task.Delay(TimeSpan.FromSeconds(delayAfter), _token);
        }

        public JavascriptResponse EvaluateScript(string script, int delayInSec = 2)
        {
            var resp = Browser.EvaluateScriptAsync(script).Result;
            Task.Delay(TimeSpan.FromSeconds(delayInSec)).Wait(_token);
            return resp;
        }


        public void SetResourceLoadInstance()
        {
            if (ProxyRequestHandler == null)
                RequestHandlerCustom.ResourceRequestHandler.IsNeedResourceData = true;
            else
                ProxyRequestHandler.ResourceRequestHandler.IsNeedResourceData = true;
        }

        public void ReSetResourceLoadInstance()
        {
            if (ProxyRequestHandler == null)
                RequestHandlerCustom.ResourceRequestHandler.IsNeedResourceData = false;
            else
                ProxyRequestHandler.ResourceRequestHandler.IsNeedResourceData = false;
        }

        public List<KeyValuePair<string, MemoryStreamResponseFilter>> TwitterJsonResponse()
        {
            try
            {
                var lstResponseStream = ProxyRequestHandler == null
                    ? RequestHandlerCustom.ResourceRequestHandler.TwitterresponseList.DeepCloneObject()
                    : ProxyRequestHandler.ResourceRequestHandler.TwitterresponseList.DeepCloneObject();
                return lstResponseStream;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<bool> CopyPasteContentAsync(string message = "", int winKeyCode = 13, int delay = 90,
            double delayAtLast = 0,
            CefEventFlags flags = CefEventFlags.ControlDown)
        {
            try
            {
                var ke = new KeyEvent();
                if (winKeyCode != 0)
                    ke.WindowsKeyCode = winKeyCode;

                ke.Modifiers = flags;

                var copiedText = string.Empty;

                if (Browser.IsDisposed) return false;

                await Task.Delay(delay, _token);

                var isRunning = false;

                if (!string.IsNullOrEmpty(message))
                {
                    isRunning = true;
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            copiedText = Clipboard.GetText();
                            Clipboard.SetText(message);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        isRunning = false;
                    });
                }


                while (isRunning)
                    await Task.Delay(25);

                Browser.GetBrowser().GetHost().SendKeyEvent(ke);

                if (delayAtLast > 0)
                    await Task.Delay(TimeSpan.FromSeconds(delayAtLast), _token);

                if (!string.IsNullOrEmpty(message))
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        try
                        {
                            Clipboard.Clear();
                            Clipboard.SetText(copiedText);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });

                return false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        public void CopyPasteContent(string message = "", int winKeyCode = 13, int delay = 90, double delayAtLast = 0,
            CefEventFlags flags = CefEventFlags.ControlDown)
        {
            CopyPasteContentAsync(message, winKeyCode, delay, delayAtLast).Wait();
        }

        #endregion
    }
}