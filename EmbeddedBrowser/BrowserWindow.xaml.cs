using CefSharp;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using EmbeddedBrowser.BrowserHelper;
using Unity;

namespace EmbeddedBrowser
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : INotifyPropertyChanged, IDisposable
    {
        private IAccountScopeFactory _accountScopeFactory;
        private IHttpHelper _httpHelper;
        
        private readonly object _cefLock = new object();

        public ICommand SearchCommand { get; }
        public string TargetUrl { get; set; } = string.Empty;
        public bool CustomUse { get; set; }
        public bool SkipYoutubeAd { get; set; }
        public bool FoundAd { get; set; }
        public bool VerifyingAccount { get; set; }

        public BrowserWindow()
        {
            InitializeComponent();
            WindowBrowsers.DataContext = this;
            SearchCommand = new DelegateCommand(GoToUrl);
        }

        public BrowserWindow(DominatorAccountModel dominatorAccountModel, string targetUrl = "", bool customUse = false, bool skipAd = false)
            : this()
        {
            DominatorAccountModel = dominatorAccountModel;
            TargetUrl = targetUrl;
            CustomUse = customUse;
            SkipYoutubeAd = skipAd;
            
            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath = $"{ConstantVariable.GetCachePathDirectory()}\\{DominatorAccountModel.AccountId}"
            });
            
            Browser.MenuHandler = new MenuHandler();
            Browser.RequestHandler = new RequestHandlerCustom(this);
            Browser.LifeSpanHandler = new BrowserLifeSpanHandler();

            InitializeGoogleLoginStatusActions();

            var url = CustomUse && !string.IsNullOrEmpty(TargetUrl) ? TargetUrl : GetNetworksHomeUrl();
            UrlBar.Text= Browser.Address = url;
            Browser.IsBrowserInitializedChanged += LoadSettings; 
        }
        

        /// <summary>
        /// Set Account Model Cookies into the browser
        /// </summary>
        public async Task SetCookie()
        {
            try
            {
                var accountCookie = DominatorAccountModel.Cookies;
                var callBack = new TaskCompletionCallback();

                if (accountCookie.Count == 0)
                {
                    Browser.RequestContext.GetDefaultCookieManager(callBack).DeleteCookies();
                    return;
                }
                
                var cefInitialCookies =await Browser.RequestContext.GetDefaultCookieManager(callBack)
                    .VisitAllCookiesAsync();
                
                if(cefInitialCookies.Count!=0)
                Browser.RequestContext.GetDefaultCookieManager(callBack).DeleteCookies();

                foreach (var accCookie in accountCookie)
                {
                    var cook = (System.Net.Cookie)accCookie;

                    var cefCookie = new CefSharp.Cookie
                    {
                        HttpOnly = cook.HttpOnly,
                        Name = cook.Name,
                        Value = cook.Value,
                        Expires = cook.Expires,
                        Domain = cook.Domain,
                        Secure = cook.Secure,
                        Path = cook.Path
                    };

                    var url = "https://www" + (!cook.Domain.StartsWith(".") ? "." : "") + cook.Domain;

                    var set = Browser.RequestContext.GetDefaultCookieManager(callBack).SetCookie(url, cefCookie);

                    //if (!set) { /*Is cookie set ?*/ }
                }

                if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube)
                {
                    CustomUse = true;
                    if (string.IsNullOrEmpty(TargetUrl))
                        TargetUrl = SocialHomeUrls();
                    var url = CustomUse && !string.IsNullOrEmpty(TargetUrl) ? TargetUrl : GetNetworksHomeUrl();
                    Browser.Address = url;
                    UrlBar.Text = url;
                }

                // Just to check that how many cookie was inserted
                //cefInitialCookies = await Browser.RequestContext.GetDefaultCookieManager(callBack).VisitAllCookiesAsync();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void GoToUrl()
        {
            Browser.Load(UrlBar.Text);
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
            var homePage = DominatorAccountModel.AccountBaseModel.AccountNetwork==SocialNetworks.Youtube && DominatorAccountModel.IsUserLoggedIn ? 
                           SocialHomeUrls() : GetNetworksHomeUrl();
            Browser.Load(homePage);
        }

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool InitializedWell;
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

            var homePage = GetNetworksHomeUrl();
            Browser.Load(homePage);

            Browser.LoadingStateChanged += BrowserOnLoaded;
            InitializedWell = true;
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

                if (!isLoaded) return;
                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) &&
                    !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    Browser.GetSourceAsync().ContinueWith(taskHtml =>
                    {
                        DominatorAccountModel.Token.ThrowIfCancellationRequested();
                        try
                        {
                            var html = taskHtml.Result;
                            if (html != null)
                            {
                                switch (DominatorAccountModel.AccountBaseModel.AccountNetwork)
                                {
                                    case SocialNetworks.Facebook:
                                        FacebookBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Instagram:
                                        InstagramBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Twitter:
                                        TwitterLogin(html);
                                        break;
                                    case SocialNetworks.Pinterest:
                                        PinterestBrowserLogin(html);
                                        break;
                                    case SocialNetworks.LinkedIn:
                                        LinkedInBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Reddit:
                                        RedditBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Quora:
                                        QuoraLogin(html);
                                        break;
                                    case SocialNetworks.Gplus:
                                        GoogleBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Youtube:
                                        GoogleBrowserLogin(html);
                                        break;
                                    case SocialNetworks.Tumblr:
                                        TumblrBrowserLogin(html);
                                        break;
                                    default:
                                        throw new ArgumentOutOfRangeException();
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });
                }

            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
            }
        }

        private void FacebookBrowserLogin(string html)
        {
            if (html.Contains("royal_login_button"))
            {
                Thread.Sleep(3000);
                Browser.ExecuteScriptAsync("document.getElementById('email').value= '" +
                                           DominatorAccountModel.AccountBaseModel.UserName + "'");

                Browser.ExecuteScriptAsync("document.getElementById('pass').value= '" +
                                           DominatorAccountModel.AccountBaseModel.Password + "'");

                Thread.Sleep(1000);

                Browser.ExecuteScriptAsync("document.getElementById('u_0_5').click()");

                if (string.IsNullOrEmpty(TargetUrl))
                {
                    Browser.LoadingStateChanged -= BrowserOnLoaded;
                    return;
                }
            }
            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result) && result.Contains("profile_icon"))
            {
                LoadPostPage(true);
            }
        }
        
        public enum ActType
        {
            ClickByClass,
            ClickById,
            ClickByName,
            EnterValueByClass,
            EnterValueById,
            EnterValueByName,
            GetValueByName,
            GetValueByTagName,
            GetLengthByClass
        }

        /// <summary>
        /// Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="delayAfter">delay after the action (In seconds)</param>
        /// <param name="value">value which is going to be entered</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        private void BrowserAct(ActType actType, string element, double delayBefore = 0, double delayAfter = 0, string value = "", int clickIndex = 0)
        {
            if (delayBefore > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));

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
                    Browser.ExecuteScriptAsync($"document.getElementsByClassName('{element}')[{clickIndex}].value= '{value}'");
                    break;

                case ActType.EnterValueById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{element}').value= '{value}'");
                    break;

                case ActType.EnterValueByName:
                    Browser.ExecuteScriptAsync($"document.getElementsByName('{element}')[{clickIndex}].value= '{value}'");
                    break;
            }
            if (delayAfter > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayAfter));
        }

        /// <summary>
        /// Browser actions
        /// </summary>
        /// <param name="actType">Type of activity doing on browser window</param>
        /// <param name="element">type of element by which the action gonna be performed</param>
        /// <param name="delayBefore">delay before the action (In seconds)</param>
        /// <param name="clickIndex">Sometimes multiple buttons have same tag-value</param>
        private string GetElementValue(ActType actType, string element, double delayBefore = 0, int clickIndex = 0)
        {
            if (delayBefore > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));

            if (Browser.IsDisposed) return "";
            switch (actType)
            {
                case ActType.GetValueByName:
                    return Browser.EvaluateScriptAsync($"document.getElementsByName('{element}')[{clickIndex}].value").Result?.Result?.ToString() ?? "";
                case ActType.GetLengthByClass:
                    return Browser.EvaluateScriptAsync($"document.getElementsByClassName('{element}').length").Result?.Result?.ToString() ?? "";
                default:
                    return "";
            }
        }

        /// <summary>
        /// Press any key n times with delay between each pressed 
        /// </summary>
        /// <param name="n">Number of pressing</param>
        /// <param name="delay">Delay between each press  (In milliseconds)</param>
        /// <param name="ke">Browser KeyEvent</param>
        /// <param name="winKeyCode">WindowsKeycode of any key in keyboard</param>
        /// /// <param name="delayAtLast">Set delay at last (In seconds)</param>
        private void PressAnyKey(int n=1, int delay = 90, KeyEvent ke = new KeyEvent(), int winKeyCode = 0, double delayAtLast = 0)
        {
            if (winKeyCode != 0)
                ke.WindowsKeyCode = winKeyCode;

            for (var i = 0; i < n; i++)
            {
                Thread.Sleep(delay);
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            }
            if (delayAtLast > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayAtLast));
        }

        /// <summary>
        /// Enter Characters in TextBox  
        /// </summary>
        /// <param name="charString">String to be entered</param>
        /// <param name="typingDelay">Delay between typing</param>
        /// <param name="delayBefore">Set delay before the typing</param>
        /// <param name="delayAtLast">Set delay at last</param>
        private void EnterChars(string charString, double typingDelay = 0.09, double delayBefore = 0,
            double delayAtLast = 0)
        {
            if (string.IsNullOrEmpty(charString)) return;

            if (delayBefore > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayBefore));

            var ke = new KeyEvent {FocusOnEditableField = true, IsSystemKey = false, Type = KeyEventType.Char};

            charString.ToList().ForEach(x =>
            {
                ke.WindowsKeyCode = x;
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
                Thread.Sleep(TimeSpan.FromSeconds(typingDelay));
            });
            if (delayAtLast > 0)
                Thread.Sleep(TimeSpan.FromSeconds(delayAtLast));
        }

        private void CustomLog(string message)=> GlobusLogHelper.log.Info(Log.CustomMessage,
            DominatorAccountModel.AccountBaseModel.AccountNetwork,
            DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", message);

        private bool _htmlHasUserName;
        private string _html;
        private string _pageText;
        private void GoogleBrowserLogin(string html)
        {
            try
            {
                if (html == "<html><head></head><body></body></html>") return;
                // BrowserAct(ActType.ClickById,"sign-in-btn",delayAfter:3);
                lock (_cefLock)
                {
                   // SetCookie();
                    _html = html;
                    if (_isLoggedIn || Browser.IsDisposed) return;

                    var last30Secs = DateTime.Now;
                    while (string.IsNullOrEmpty((_pageText = Browser.GetTextAsync().Result).Trim()))
                    {
                        Thread.Sleep(1000);
                        if (last30Secs.AddSeconds(30) < DateTime.Now) return;
                    }

                    if (!_htmlHasUserName)
                        _htmlHasUserName = _html.ToLower().Contains($"\"opep7c\":\"{DominatorAccountModel.UserName.ToLower()}\"")
                                           || _pageText.Contains("Protect your account") && _pageText.ToLower().Contains(DominatorAccountModel.UserName.ToLower());
                    SetGoogleLangAsEng(_pageText, _htmlHasUserName);

                    if (!_isLoggedIn && (_pageText.Contains("Verify your identity") ||_pageText.Contains("\n\nEnter verification code\n\n") || _pageText.Contains("English (")) && !IsGoogleAccountLoginFailed())
                    {
                        if (/*Html.Contains("identifierNext")*/ _pageText.Contains("\nForgot email?\n") && (_pageText.Contains("\nEmail or phone\n") || _pageText.ToLower().Contains($"\n{DominatorAccountModel.AccountBaseModel.UserName.Trim().ToLower()}\n")) && !_pageText.Contains("Confirm the recovery email address"))
                        {
                            BrowserAct(ActType.EnterValueById, "identifierId", value: DominatorAccountModel.AccountBaseModel.UserName, delayAfter: 1);
                            PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key //BrowserAct(ActType.ClickById,"identifierNext", 3, 2);
                            _pageText = Browser.GetTextAsync().Result;
                            IsGoogleAccountLoginFailed();
                            return;
                        }
                        if (/*Html.Contains("passwordNext")*/(_pageText.Trim().ToLower().Contains(DominatorAccountModel.AccountBaseModel.UserName.Trim().ToLower()) || _pageText.Contains("To continue, first verify it's you")) && _pageText.Contains("\nEnter your password\n"))
                        {
                            BrowserAct(ActType.EnterValueByName, "password", value: DominatorAccountModel.AccountBaseModel.Password, delayAfter: 1);
                            PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3);//Press Enter key //BrowserAct(ActType.ClickById,"passwordNext", 2, 2);
                            _pageText = Browser.GetTextAsync().Result;
                            IsGoogleAccountLoginFailed();
                            return;
                        }
                    }

                    if (!_loginFailed && !_isLoggedIn && _htmlHasUserName)
                    {
                        if (string.IsNullOrEmpty(TargetUrl))
                            TargetUrl = SocialHomeUrls();

                        var result = GetLoggedInPageSource();

                        if (!string.IsNullOrEmpty(result))
                            LoadPostPage(true);

                        Thread.Sleep(3000);
                        SaveCookie();
                    }
                    SetVideoQualityAs144P();
                }
            }
            catch
            { /*ignored*/}
        }
        
        private void SetGoogleLangAsEng(string pageText, bool htmlHasUserName)
        {
            try
            {
                if (_isLoggedIn || Uri.UnescapeDataString(TargetUrl.ToLower()).Contains("www.youtube.com/watch?") 
                                || TargetUrl == "https://www.youtube.com/"
                                || htmlHasUserName || string.IsNullOrEmpty(pageText) || pageText == "Account\n\n\n"
                                || pageText.Contains("Protect your account") || pageText.Contains("Loading, please wait ...")
                                || pageText.Contains("English (") || pageText.Contains("Personal info"))
                    return;

                // Open Google Language ListBox in Browser
                BrowserAct(ActType.ClickByClass, "vRMGwf oJeWuf", delayAfter: 1);

                var ke = new KeyEvent();
                PressAnyKey(70, ke: ke, winKeyCode: 38); // Press Up Arrow Key
                PressAnyKey(8, ke: ke, winKeyCode: 40, delayAtLast: 0.5); // Press Down Arrow Key
                PressAnyKey(1, 0, ke, 13, 1); // Press Enter Key
            }
            catch
            { /* Ignored */}

        }
        
        private Dictionary<Predicate<string>, Func<bool>> _predicateDict;
        private bool _loginFailed;
        private void InitializeGoogleLoginStatusActions()
        {
            if (!(DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Gplus ||
                  DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube))
                return;

            VerifyingAccount = DominatorAccountModel.IsVerificationCodeSent;
            _accountScopeFactory = ServiceLocator.Current.GetInstance<IAccountScopeFactory>();
            _httpHelper = _accountScopeFactory[DominatorAccountModel.AccountId]
                .Resolve<IHttpHelper>(DominatorAccountModel.AccountBaseModel.AccountNetwork.ToString());

            _predicateDict = new Dictionary<Predicate<string>, Func<bool>>
            {
                {
                    pageData =>
                        pageData.Contains("Couldn't find your Google Account") || pageData.Contains("Enter a valid email or phone number")
                                                                               || pageData.Contains("Wrong password. Try again or click Forgot password to reset it")
                                                                               || pageData.Contains("Your password was changed"),
                    InvalidCredentials
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains("Those passwords didn't match. Try again."),
                    SetNewPasswordNotMatched
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains("Use a mix of letters, numbers and symbols to create a stronger password"),
                    SetNewPasswordCreateStrongPassword
                },
                {
                    pageData => pageData.Contains("Change password")
                                && pageData.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                                && pageData.Contains("Create password"),
                    SetNewPasswordAfterSuspiciousActivity
                },
                {
                    pageData => pageData.Contains("Try another way to sign in\nGet a verification code at") && pageData.Contains("\nConfirm your recovery phone number\n"),
                    ClickOptionConfirmRecoveryClickIndex2
                },
                {
                    pageData =>
                        pageData.Contains("Unavailable because of too many attempts. Please try again later.")
                        || pageData.Contains("It is not available because too many attempts have been failed. Try again in a few hours.")
                        || pageData.Contains("Too many failed attempts") &&
                        pageData.Contains("Unavailable because of too many failed attempts. Try again in a few hours."),
                    ManyAttemptsOnPhoneVerification
                },
                {
                    pageData => pageData.Contains("Confirm your recovery email") ||
                                pageData.Contains("Confirm your recovery phone number"),
                    ClickOptionConfirmRecovery
                },
                {pageData => pageData.Contains("Confirm the recovery email address"), ConfirmRecoveryEmailAddress},
                {pageData => pageData.Contains("Confirm the phone number"), ConfirmRecoveryPhoneNumber},
                {
                    pageData => pageData.Contains("Enter a phone number to get a text message with a verification code") ||
                                pageData.Contains("Provide a phone number to continue. We'll send a verification code you can use to sign in.") ||
                    pageData.Contains("Provide a phone number to continue. We'll send a verification code that you can use to sign in."),
                    AddPhoneNumber
                },
                {
                    pageData => pageData.Contains("Enter verification code") || pageData.Contains("A text message with a 6-digit verification code was just sent to"),
                    VerifyCodeFromPhone
                },
                {
                    pageData => pageData.Contains("Get a verification code")
                                || pageData.Contains("Do you have your phone?")
                                || pageData.Contains("Google will send a notification to your phone to verify that it's you"),
                    NeedPhoneVerification
                },
                {
                    pageData => pageData.Contains("Type the text you hear or see")
                                || pageData.Contains("Google couldn't verify this account belongs to you.")
                                || pageData.Contains("This device isn't recognized. For your security, Google wants to make sure that it's really you.")
                                || pageData.Contains("This device isn't recognised. For your security, Google wants to make sure that it's really you."),
                    NeedsVerification
                },
                {
                    pageData => pageData.Contains("An error occurred. please try again.") ||
                                pageData.ToLower().Contains("something went wrong"),
                    FailedGotUnknownError
                },
                {
                    pageData => pageData.Contains("Protect your account") &&
                                pageData.Contains("Tell Google how to reach you in case you forget your password"),
                    () =>
                    {
                        _html = DominatorAccountModel.UserName.ToLower();
                        return false;
                    }
                },
                {pageData => pageData.Contains("You've tried to sign in too many times."), TooManyAttemptsOnSignIn}
            };
        }

        private bool IsGoogleAccountLoginFailed()
        {
            var predicateKey = _predicateDict.Keys.FirstOrDefault(x => x.Invoke(_pageText));

            if (predicateKey==null) return false;
            var loginFailed = _predicateDict[predicateKey].Invoke();
            if (loginFailed)
                VerifyingAccount = false;
            if (!loginFailed || _loginFailed) return loginFailed;

            _loginFailed = true;
            DominatorAccountModel.IsUserLoggedIn = false;
            _httpHelper.GetRequestParameter().Cookies = new CookieCollection();
            return true;
        }

        private bool RetypeEmail()
        {
            var isRetype = true;
            if (DominatorAccountModel.AccountBaseModel.Status != AccountStatus.ReTypeEmail && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AlternateEmail) && !DominatorAccountModel.AccountBaseModel.AlternateEmail.Contains("•"))
            {
                BrowserAct(ActType.EnterValueById, "identifierId", 1.5, 2, DominatorAccountModel.AccountBaseModel.AlternateEmail);
                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3.5); //Press Enter key //BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 2.5);
                var pageText = Browser.GetTextAsync().Result;
                isRetype = pageText.Contains("The email you entered is incorrect. Try again.") || pageText.Contains("Try again with a valid email address");
                if (isRetype)
                    CustomLog("Alternate Email is incorrect for verification");
            }

            if (isRetype)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypeEmail;

            return isRetype;
        }

        private bool RetypePhoneNumber()
        {
            var isRetype = true;
            if (DominatorAccountModel.AccountBaseModel.Status != AccountStatus.ReTypePhoneNumber && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber) && !DominatorAccountModel.AccountBaseModel.PhoneNumber.Contains("•"))
            {
                if(_cameHereByClickingOption)
                {
                    PressAnyKey(5, 150, winKeyCode: 9, delayAtLast: 0.5); // Press Tab 5 times 
                    BrowserAct(ActType.EnterValueById, "phoneNumberId", delayAfter: 1.5, value: DominatorAccountModel.AccountBaseModel.PhoneNumber);
                    _cameHereByClickingOption = false;
                }
                else
                    EnterChars(DominatorAccountModel.AccountBaseModel.PhoneNumber, delayAtLast: 1);

                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);

                var pageText = Browser.GetTextAsync().Result;
                isRetype = pageText.Contains("This number doesn't match the one you provided. Try again.");
                if(isRetype)
                    CustomLog($"This number({DominatorAccountModel.AccountBaseModel.PhoneNumber}) doesn't match the one you provided. Try again.");
            }

            if (isRetype)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypePhoneNumber;

            return isRetype;
        }

        private bool VerifyCodeFromPhone()
        {
            if (((DominatorAccountModel.AccountBaseModel.Status == AccountStatus.PhoneVerification && !VerifyingAccount)
               || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
               || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount) && !DominatorAccountModel.IsVerificationCodeSent)
                return true;
            if(!Browser.GetTextAsync().Result.Contains("\n\nEnter verification code\n\n"))
             PressAnyKey(3, 200, winKeyCode: 9); //Press Tab 3 Times key
            var isWrong = true;
            var iterateNTimes = 0;
            var codeBefore = "";
            do
            {
                var last2Min = DateTime.Now;

                while ((!DominatorAccountModel.IsVerificationCodeSent || codeBefore != DominatorAccountModel.VarificationCode.Trim() &&
                        DominatorAccountModel.VarificationCode.Trim().Length < 6) && !Browser.IsDisposed && last2Min.AddMinutes(2) > DateTime.Now)
                    Thread.Sleep(2000); // Waiting to get code from UI

                codeBefore = DominatorAccountModel.VarificationCode.Trim();
                if (DominatorAccountModel.VarificationCode.Trim().Length > 0)
                {
                    Browser.SelectAll();
                    PressAnyKey(winKeyCode:8,delayAtLast:0.2);
                    EnterChars(" " + DominatorAccountModel.VarificationCode.Trim(), 0.3); //Entering Verification Code
                    //EnterChars(DominatorAccountModel.VarificationCode.Trim(), 0.3); //Entering Verification Code
                    //EnterChars(DominatorAccountModel.VarificationCode.Trim(), 0.3); //Re-Entering Verification Code
                    //PressAnyKey(6, 300, winKeyCode: 37, delayAtLast: 0.5); //Taking Cursor 6 step behind on Code Text
                    //PressAnyKey(6, 300, winKeyCode: 8, delayAtLast: 0.5); //Now removing first 5 numbers of entered code
                    if (!Browser.GetTextAsync().Result.Contains("\n\nEnter verification code\n\n"))
                        PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 5); //Press Enter key
                    else
                        BrowserAct(ActType.ClickByName, "VerifyPhone", delayAfter: 3);

                    var pageText = Browser.GetTextAsync().Result;

                    isWrong = pageText.Contains("That code doesn't match the one we sent.") || pageText.Contains("Wrong code. Try again.") || pageText.Contains("Wrong number of digits. Please try again.");
                    if (isWrong)
                    {
                        iterateNTimes++;
                        if (iterateNTimes < 2)
                        {
                            PressAnyKey(6, 300, winKeyCode: 46, delayAtLast: 0.5); //Now removing all digits of entered code
                            CustomLog("You have entered wrong Verification code. Try again.");
                            DominatorAccountModel.VarificationCode = "";
                            continue;
                        }
                    }
                    else
                    {
                        isWrong = pageText.Contains("Too many failed attempts. The previous code sent to you is no longer valid.")|| pageText.Contains("Too many attempts. Please try again later.") || pageText.Contains("Too many failed attempts")
                                  && pageText.Contains("Unavailable because of too many failed attempts. Try again in a few hours.");
                        if (isWrong)
                        {
                            CustomLog("Too many failed attempts on Phone Verification. Try again in a few hours.");
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                        }
                    }
                }
                break;
            } while (true);

            if (isWrong && DominatorAccountModel.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
            else
               DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;

            VerifyingAccount = DominatorAccountModel.IsVerificationCodeSent = false;

            DominatorAccountModel.VarificationCode = "";
            return isWrong;
        }
        
        private bool InvalidCredentials()
        {
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
            return true;
        }

        private bool SetNewPasswordNotMatched()
        {
            CustomLog("Those passwords didn't match. Try again.");
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool SetNewPasswordCreateStrongPassword()
        {
            CustomLog("Use a mix of letters, numbers and symbols to create a stronger password");
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool SetNewPasswordAfterSuspiciousActivity()
        {
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
            return true;
        }

        private bool ManyAttemptsOnPhoneVerification()
        {
            CustomLog("Too many failed attempts on Phone Verification. Try again in a few hours.");
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
            return true;
        }
        
        private bool ClickOptionConfirmRecovery()
        {
            BrowserAct(ActType.ClickByClass, "vdE7Oc", delayAfter: 2.5);
            return false;
        }

        bool _cameHereByClickingOption;
        private bool ClickOptionConfirmRecoveryClickIndex2()
        {
            BrowserAct(ActType.ClickByClass, "vdE7Oc", delayAfter: 2.5, clickIndex: 2);
            _cameHereByClickingOption = true;
            return false;
        }

        private bool ConfirmRecoveryEmailAddress()
        {
           var loginFailed = RetypeEmail();
            var gotEmailFromPage = Utilities.GetBetween(_pageText, "your account:", "\n").Trim();
            if (!IsExistingEmailOrNumberSame(DominatorAccountModel.AccountBaseModel.AlternateEmail.Trim(),
                gotEmailFromPage))
                DominatorAccountModel.AccountBaseModel.AlternateEmail = gotEmailFromPage;
           return loginFailed;
        }

        private bool ConfirmRecoveryPhoneNumber()
        {
           var loginFailed = RetypePhoneNumber();
            var gotNumberFromPage = Utilities.GetBetween(_pageText, "security settings:", "\n").Replace(" ", "")
                .Replace("(", "").Replace(")", "").Replace("-", "").Replace("_", "").Trim();
            if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber.Trim()) || !IsExistingEmailOrNumberSame(DominatorAccountModel.AccountBaseModel.PhoneNumber.Trim(),
                gotNumberFromPage))
                DominatorAccountModel.AccountBaseModel.PhoneNumber = gotNumberFromPage;
            return loginFailed;
        }

        private bool AddPhoneNumber()
        {
            if (!DominatorAccountModel.IsVerificationCodeSent &&
                !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber)
                 && !DominatorAccountModel.AccountBaseModel.PhoneNumber.Contains("•"))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                return true;
            }

            var isWrong = true;
            if (!(DominatorAccountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
                  || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount)
                && DominatorAccountModel.IsVerificationCodeSent
                && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber)
                && !DominatorAccountModel.AccountBaseModel.PhoneNumber.Contains("•"))
            {
               // DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                var text = Browser.GetTextAsync().Result;
                if (text.Contains("Provide a phone number to continue. We'll send a verification code that you can use to sign in.") 
                    || text.Contains("Provide a phone number to continue. We'll send a verification code you can use to sign in."))
                {
                    //PressAnyKey(9, 150, winKeyCode: 9,
                    //    delayAtLast: 1);
                }
                //BrowserAct(ActType.ClickByClass, "whsOnd zHQkBf");
                EnterChars(DominatorAccountModel.AccountBaseModel.PhoneNumber, delayAtLast: 1);
                if (text.Contains("Provide a phone number to continue. We'll send a verification code that you can use to sign in.")
                    || text.Contains("Provide a phone number to continue. We'll send a verification code you can use to sign in."))
                {
                    BrowserAct(ActType.ClickByName, "SendCode",delayAfter:4);
                }
                else
                    PressAnyKey(1, 0, winKeyCode: 13,
                    delayAtLast: 4); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);

                text = Browser.GetTextAsync().Result;
                isWrong = !(text.Contains("A text message with a 6-digit verification code was just sent to")|| text.Contains("\n\nEnter verification code\n\n"));

                if (!isWrong)
                {
                    var number = Utilities.GetBetween(text, "code was just sent to", "\n");
                    if (string.IsNullOrEmpty(number))
                        number = "the number you submitted";
                    CustomLog($"A text message with a 6-digit verification code was just sent to {number}. Enter the code within 2 minutes.");
                }
                else
                {
                    isWrong = text.Contains("The phone number was invalid. Please correct it and try again.") || text.Contains("There was a problem with your phone number") || text.Contains("Sorry, Google didn't recognise the number that you have entered. Please check the country and number.") ||
                              text.Contains("Sorry, Google didn't recognize the number that you have entered. Please check the country and number.") ||
                              text.Contains("This phone number has already been used too many times for verification.");

                    if (isWrong && text.Contains("There was a problem with your phone number"))
                    {
                        CustomLog($"There was a problem with your phone number({DominatorAccountModel.AccountBaseModel.PhoneNumber}). Please use another phone number");
                        DominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                    else if (isWrong && text.Contains("This phone number has already been used too many times for verification."))
                    {
                        CustomLog($"This phone number({DominatorAccountModel.AccountBaseModel.PhoneNumber}) has already been used too many times for verification. Please use another phone number");
                        DominatorAccountModel.AccountBaseModel.PhoneNumber = "";
                        DominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                    else if (isWrong)
                    {
                        CustomLog($"Google didn't recognize the number that you have entered. Please check the country code and number({DominatorAccountModel.AccountBaseModel.PhoneNumber}).Please re-enter the phone number and try again later");
                        DominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
                    }
                }
            }

            if (isWrong && DominatorAccountModel.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
            {
                VerifyingAccount = false;
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
            }

            DominatorAccountModel.IsVerificationCodeSent = isWrong;
            return isWrong;
        }

        private bool NeedPhoneVerification()
        {
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
            return true;
        }

        private bool NeedsVerification()
        {
            if(_pageText.Contains("Type the text you hear or see") && DominatorAccountModel.AccountBaseModel.Status != AccountStatus.NeedsVerification)
                CustomLog("Captcha Found (Reason: Might be same IP is getting hit so fast with google login)");
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
            return true;
        }

        private bool FailedGotUnknownError()
        {
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
            return true;
        }

        private bool TooManyAttemptsOnSignIn()
        {
            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnSignIn;
            return true;
        }

        private bool IsExistingEmailOrNumberSame(string enteredNumber, string alreadyExistedNumberInAccount)
        {
            try
            {
                var reverseAlreadyExistedNumberInAccount = alreadyExistedNumberInAccount.Reverse().ToList();

                var indexIterate = 0;
                foreach (var eachNumb in enteredNumber.Reverse())
                {
                    var selectedDigit = reverseAlreadyExistedNumberInAccount[indexIterate];
                    if (selectedDigit != '•' && selectedDigit != eachNumb)
                        return false;
                    ++indexIterate;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return true;
            }
        }
        
        private bool _isLoggedIn;
        private void SaveCookie()
        {
            if (_isLoggedIn) return;

            try
            {
                var lstCookies = Browser.RequestContext.GetDefaultCookieManager(new TaskCompletionCallback())
                    .VisitAllCookiesAsync().Result;

                var cookieCollection = new CookieCollection();

                foreach (var item in lstCookies)
                {
                    try
                    {
                        if (item.Expires != null)
                            cookieCollection.Add(new System.Net.Cookie
                            {
                                Expires = (DateTime) item.Expires,
                                Name = item.Name,
                                Value = item.Value,
                                Domain = item.Domain,
                                Path = item.Path,
                                Secure = item.Secure
                            });
                    }
                    catch
                    {/*ignored*/}
                }

                var requestParameters = (RequestParameters)_httpHelper.GetRequestParameter();
                requestParameters.Cookies = cookieCollection;
                _httpHelper.SetRequestParameter(requestParameters);

                var url = SocialHomeUrls();

                IResponseParameter objResponseParameter = (ResponseParameter)_httpHelper.GetRequest(url);

                if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Gplus)
                {
                    var googlePlusAcc = Utilities.GetBetween(objResponseParameter.Response, "\"oPEP7c\":\"", "\"");
                    if (string.IsNullOrEmpty(googlePlusAcc) || cookieCollection.Count < 2)
                        return;

                    DominatorAccountModel.AccountBaseModel.ProfileId = googlePlusAcc;
                }

                if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube)
                {
                    if (!(objResponseParameter.Response.ToLower().Contains(DominatorAccountModel.UserName.ToLower()) || objResponseParameter.Response.Contains("{\"iconType\":\"SUBSCRIPTIONS\"}") || objResponseParameter.Response.Contains("\"LOGGED_IN\":true")))
                        return;

                    DominatorAccountModel.AccountBaseModel.ProfileId =
                        Utilities.GetBetween(objResponseParameter.Response, "\"delegatedSessionId\":\"", "\"");
                    if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.ProfileId))
                        DominatorAccountModel.AccountBaseModel.ProfileId = "Default Channel";
                    DominatorAccountModel.AccountBaseModel.UserId =
                        Utilities.GetBetween(objResponseParameter.Response, "\"key\":\"creator_channel_id\",\"value\":\"", "\"");
                }

                _isLoggedIn = true;
                _loginFailed = false;

                CreateChannelOnYoutube();

                DominatorAccountModel.Cookies = cookieCollection;
                DominatorAccountModel.IsUserLoggedIn = true;
                VerifyingAccount = DominatorAccountModel.IsVerificationCodeSent = false;
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                  .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                  .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                  .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                   .SaveToBinFile();

                //AccountsFileManager.Edit(DominatorAccountModel);
                CustomLog("Browser login successfull.");
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
            }
        }

        private void CreateChannelOnYoutube()
        {
            try
            {
                if (DominatorAccountModel.AccountBaseModel.AccountNetwork != SocialNetworks.Youtube) return;

                Browser.Load("https://www.youtube.com/create_channel");
                Thread.Sleep(1000);
                for (var i = 0; i < 10; i++)
                {
                    var htmlData = Browser.GetSourceAsync().Result;
                    if (htmlData.Contains("youtube.com/create_channel\">") && htmlData.Contains("id=\"create-channel-identity-lb"))
                    {
                        Thread.Sleep(5000);
                        Browser.ExecuteScriptAsync("document.getElementById('create-channel-submit-button').click()");
                        break;
                    }
                    if (htmlData.Contains("\"editChannelButtons\""))
                        break;
                    Thread.Sleep(1000);
                }
                Thread.Sleep(1000);
                Browser.Load("https://www.youtube.com/");
            }
            catch
            {/*ignored*/}
        }

        public bool SetVideoQuality;

        private void SetVideoQualityAs144P()
        {
            if (SetVideoQuality || !Uri.UnescapeDataString(TargetUrl.ToLower()).Contains("www.youtube.com/watch?")) return;

            if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube)
            {
                new Task(() =>
                {
                    while (!Browser.IsDisposed)
                    {
                        if (!Browser.IsDisposed)
                           FoundAd = GetElementValue(ActType.GetLengthByClass, "ytp-ad-skip-button-icon", 1.5) == "1";
                        if (FoundAd) { /*Just for Checking*/ }
                        if (!Browser.IsDisposed && SkipYoutubeAd && FoundAd)
                            BrowserAct(ActType.ClickByClass, "ytp-ad-skip-button-icon", 3.0); // for Skipping add
                    }
                }).Start();
            }
            
            BrowserAct(ActType.ClickByClass, "ytp-volume-slider", 3, 0.1); // To Open Volume Slider

            var ke = new KeyEvent();
            PressAnyKey(21, 100, ke, 40,2); //Press Down Arrow key 40 times to mute the music

            //ClickByClass("ytp-mute-button ytp-button"); // Direct Mute the music

            while (FoundAd)
            { Thread.Sleep(2000);}

            BrowserAct(ActType.ClickByClass, "ytp-button ytp-settings-button", 1, 1.5); // Click Youtube MediaPlayer Setting Button

            PressAnyKey(4, 900, ke, 40, 1); //Press Down Arrow key 4 times
            PressAnyKey(1, 0, ke, 39, 1); //Press right Arrow key 1 time
            PressAnyKey(6, 900, ke, 40, 1); //Press Down Arrow key 6 times
            PressAnyKey(1, 0, ke, 38, 1); //Press up Arrow key 1 time
            PressAnyKey(1, 0, ke, 13); //Press Enter key

            SetVideoQuality = true;
            
        }

        private void PinterestBrowserLogin(string html)
        {
            lock (_cefLock)
            {
                if (!string.IsNullOrEmpty(html)
                    && (html.Contains("type=\"email\"") || html.Contains("type=\"password\""))
                    && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName)
                    && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password)
                    && !Browser.IsDisposed)
                {
                    var getPageText = Browser.GetTextAsync().Result;
                    if (getPageText.Contains("that password isn't right.") || getPageText.Contains("Reset your password"))
                        return;

                    // Click on username textbox
                    BrowserAct(ActType.ClickByName, "id", delayAfter: 0.5);

                    // Enter account's username in username textbox
                    EnterChars(DominatorAccountModel.AccountBaseModel.UserName, delayBefore: 1);

                    // Press Tab button now
                    PressAnyKey(1, winKeyCode: 9, delayAtLast: 1);

                    // Click on password textbox
                    BrowserAct(ActType.ClickByName, "password", delayAfter: 0.5);

                    // Enter account's password in password textbox
                    EnterChars(" " + DominatorAccountModel.AccountBaseModel.Password, 0.1, delayAtLast: 0.5);

                    // Press Tab button now
                    PressAnyKey(1, winKeyCode: 9, delayAtLast: 1);

                    // Click on Login button 
                    BrowserAct(ActType.ClickByClass, !html.Contains("type=\"email\"") ? "red SignupButton active" : "SignupButton", delayAfter: 2);
                }

                var result = GetLoggedInPageSource();

                if (!string.IsNullOrEmpty(result) && result.Contains("\"isAuth\": true"))
                    LoadPostPage(true);
            }
        }

        private void InstagramBrowserLogin(string html)
        {
            lock (_cefLock)
            {
                if (html.Contains("Phone number, username, or email"))
                {
                    // Press Tab
                    PressAnyKey(winKeyCode: 9);

                    // Click over username textbox
                    BrowserAct(ActType.ClickByName, "username", delayAfter: 1);

                    // Enter username
                    EnterChars(" " + DominatorAccountModel.AccountBaseModel.UserName,0.15, delayAtLast: 1);

                    // Press Tab
                    PressAnyKey(winKeyCode: 9);

                    // Enter password
                    EnterChars(" " + DominatorAccountModel.AccountBaseModel.Password, 0.15, delayAtLast: 1);

                    // Press Enter
                    PressAnyKey(winKeyCode: 9, delayAtLast: 1);

                    // Get Loaded PageSource
                    var updatedHtml = Browser.GetSourceAsync().Result;

                    var require = updatedHtml.Contains("choice_1") && updatedHtml.Contains("choice_0");
                    if (!require && !updatedHtml.Contains("Submit"))
                    {
                        //BrowserAct(ActType.ClickByClass, "_5f5mN");
                        BrowserAct(ActType.ClickByClass, "sqdOP", clickIndex: 1, delayAfter: 2);
                    }
                } 
            }

            var result = GetLoggedInPageSource();
            if (!string.IsNullOrEmpty(result) && result.Contains("logged-in"))
            {
                LoadPostPage(true);
            }
        }

        private void TwitterLogin(string html)
        {
            if (html != null && html.Contains("js-username-field email-input js-initial-focus") && html.Contains("js-password-field"))
            {
                lock (_cefLock)
                {
                    // Enter Username
                    BrowserAct(ActType.EnterValueByClass, "js-username-field email-input js-initial-focus", value: DominatorAccountModel.AccountBaseModel.UserName, delayAfter: 2);

                    // Enter password
                    BrowserAct(ActType.EnterValueByClass, "js-password-field", value: DominatorAccountModel.AccountBaseModel.Password.Replace("'", "\\'"), delayAfter: 2);

                    // Click on submit
                    BrowserAct(ActType.ClickByClass, "submit EdgeButton EdgeButton--primary EdgeButtom--medium", delayAfter: 4); 
                }
            }

            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result) && result.Contains("signout") && result.Contains("timeline-tweet-box"))
                LoadPostPage(true);
        }
        
        private void LinkedInBrowserLogin(string html)
        {
            if (!string.IsNullOrEmpty(html) && html.Contains("LinkedIn: Log In or Sign Up"))
            {
                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    Browser.ExecuteScriptAsync("document.getElementById('login-email').value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");

                    Browser.ExecuteScriptAsync("document.getElementById('login-password').value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");

                    Browser.ExecuteScriptAsync("document.getElementById('login-submit').disabled = false");

                    Thread.Sleep(4000);

                    Browser.ExecuteScriptAsync("document.getElementById('login-submit').click()");

                }
            }
            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result))
            {
                if (!result.Contains("LinkedIn: Log In or Sign Up") && !result.Contains("Be great at what you do") && !result.Contains("By clicking Join now, you agree to the LinkedIn") && !result.Contains("Your LinkedIn account has been temporarily restricted"))
                {
                    if (!result.Contains("Sign in to LinkedIn") || !result.Contains("Sign in"))
                    {
                        LoadPostPage(true);
                    }
                }

            }
        }
        
        private void QuoraLogin(string html)
        {
            lock (_cefLock)
            {
                if (html != null && html.Contains("name=\"password\"") && html.Contains("name=\"email\""))
                {
                    Browser.ExecuteScriptAsync("document.getElementsByName('email')[1].value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");

                    Browser.ExecuteScriptAsync("document.getElementsByName('password')[1].value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");

                    Browser.ExecuteScriptAsync("document.getElementsByClassName('submit_button ignore_interaction submit_button_disabled')[0].class='submit_button ignore_interaction'");

                    Browser.ExecuteScriptAsync("document.getElementsByClassName('submit_button ignore_interaction')[0].click()");
                }

                var result = GetLoggedInPageSource();

                if (!string.IsNullOrEmpty(result) && result.Contains("\"logged_in\": true"))
                {
                    LoadPostPage(true);
                }
            }
        }

        private void RedditBrowserLogin(string html)
        {
            if (html.Contains("loginUsername"))
            {
                Browser.ExecuteScriptAsync("document.getElementById('loginUsername').value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");
                Browser.ExecuteScriptAsync("document.getElementById('loginPassword').value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                Browser.ExecuteScriptAsync("document.getElementsByClassName('AnimatedForm__submitButton')[0].click()");
            }

            if (html.Contains("login_login-main"))
            {
                Browser.ExecuteScriptAsync("document.getElementsByName('user')[0].value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");
                Browser.ExecuteScriptAsync("document.getElementsByName('passwd')[0].value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                Thread.Sleep(1000);
                Browser.ExecuteScriptAsync("document.getElementsByClassName('submit').click()");
            }

            var result = GetLoggedInPageSource();

            if (result != null && result.Contains("Log out") || result.Contains("logged in"))
                LoadPostPage(true);
        }

        private void TumblrBrowserLogin(string html)
        {
            if (html.Contains("signup_view determine active"))
            {
                Browser.ExecuteScriptAsync("document.getElementById('signup_determine_email').value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");
                Browser.ExecuteScriptAsync("document.getElementById('signup_forms_submit').click()");
            }
            if (html.Contains("signup_login_btn active"))
            {
                Browser.ExecuteScriptAsync("document.getElementById('signup_password').value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                Browser.ExecuteScriptAsync("document.getElementById('signup_forms_submit').click()");
            }
            if (html.Contains("signup_view magiclink active"))
            {
                // Browser.ExecuteScriptAsync("document.getElementById('signup_forms_submit').click()");
                Browser.ExecuteScriptAsync("document.getElementsByClassName('forgot_password_link')[0].click()");
            }
            if (html.Contains("loginUsername"))
            {
                Browser.ExecuteScriptAsync("document.getElementById('loginUsername').value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");
                Browser.ExecuteScriptAsync("document.getElementById('loginPassword').value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                Browser.ExecuteScriptAsync("document.getElementsByClassName('AnimatedForm__submitButton')[0].click()");
            }

            if (html.Contains("login_login-main"))
            {
                Browser.ExecuteScriptAsync("document.getElementsByName('user')[0].value= '" + DominatorAccountModel.AccountBaseModel.UserName + "'");
                Browser.ExecuteScriptAsync("document.getElementsByName('passwd')[0].value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                Thread.Sleep(1000);
                Browser.ExecuteScriptAsync("document.getElementsByClassName('submit').click()");
            }

            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result) && result.Contains("'User_Logged_In', 'Yes'") || result.Contains("logged_in"))
            {
                LoadPostPage(true);
            }
        }

        private void LoadPostPage(bool isLoggedIn)
        {
            if (isLoggedIn)
            {
                Browser.Load(TargetUrl);
                Browser.LoadingStateChanged -= BrowserOnLoaded;
            }
        }

        private string GetLoggedInPageSource()
        {
            if (!string.IsNullOrEmpty(TargetUrl) && TargetUrl != "Not Published Yet")
            {
                var sourceAsync = Browser.GetSourceAsync();
                return sourceAsync.Result;
            }
            return string.Empty;
        }

        public string GetLoggedInPageSourceLinkedin()
        {
            if (!string.IsNullOrEmpty(TargetUrl) && TargetUrl != "Not Published Yet")
            {
                var sourceAsync = Browser.GetSourceAsync();
                return sourceAsync.Result;
            }
            return String.Empty;
        }

        public string GetNetworksHomeUrl()
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

        private void ButtonBack_OnClick(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoBack)
                Browser.Back();
        }

        private void ButtonForward_OnClick(object sender, RoutedEventArgs e)
        {
            if (Browser.CanGoForward)
                Browser.Forward();
        }

        private void ButtonRefresh_OnClick(object sender, RoutedEventArgs e)
        {
            Browser.Reload();
        }

    }
}