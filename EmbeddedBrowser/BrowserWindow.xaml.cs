using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Markup;
using CefSharp;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Request;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls;
using Prism.Commands;

namespace EmbeddedBrowser
{
    /// <summary>
    ///     Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : MetroWindow, INotifyPropertyChanged, IComponentConnector, IDisposable
    {

        private readonly object _syncLock = new object();

        private readonly object _googleLock = new object();

        public BrowserWindow()
        {
            InitializeComponent();
            WindowBrowsers.DataContext = this;
            SerachCommand = new DelegateCommand(GoToUrl);
        }

        private void GoToUrl()
        {
            Browser.Load(UrlBar.Text); 
        }

        public BrowserWindow(DominatorAccountModel dominatorAccountModel)
            : this()
        {
            DominatorAccountModel = dominatorAccountModel;

            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath = $"{ConstantVariable.GetCachePathDirectory()}\\{dominatorAccountModel.AccountId}"
            });
            Browser.MenuHandler = new MenuHandler();
            Browser.RequestHandler = new RequestHandlerCustom(this);
            var url = GetNetworksHomeUrl();
            Browser.Address = url;
            UrlBar.Text = url;
            Browser.IsBrowserInitializedChanged += LoadSettings;

        }
        public ICommand SerachCommand { get; }
        public string TargetUrl { get; set; } = string.Empty;

        public BrowserWindow(DominatorAccountModel dominatorAccountModel, string targetUrl, bool CustomUse)
             : this()
        {

            DominatorAccountModel = dominatorAccountModel;
            TargetUrl = targetUrl;

            Browser.RequestContext = new RequestContext(new RequestContextSettings
            {
                CachePath = $"{ConstantVariable.GetCachePathDirectory()}\\{dominatorAccountModel.AccountId}"
            });

            Browser.RequestHandler = new RequestHandlerCustom(this);

            var url = string.Empty;

            if (CustomUse)
                url = targetUrl;

            else
                url = GetNetworksHomeUrl();

            Browser.Address = url;
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

        enum ActType
        {
            ClickByClass,
            ClickById,
            EnterValueById,
            EnterValueByName
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
                Thread.Sleep((int)(delayBefore * 1000));

            if(Browser.IsDisposed) return;
            switch (actType)
            {
                case ActType.ClickByClass:
                    Browser.ExecuteScriptAsync($"document.getElementsByClassName('{element}')[{clickIndex}].click()");
                    break;

                case ActType.ClickById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{element}').click()");
                    break;

                case ActType.EnterValueById:
                    Browser.ExecuteScriptAsync($"document.getElementById('{element}').value= '{value}'");
                    break;

                case ActType.EnterValueByName:
                    Browser.ExecuteScriptAsync($"document.getElementsByName('{element}')[{clickIndex}].value= '{value}'");
                    break;
            }
            if (delayAfter > 0)
                Thread.Sleep((int)(delayAfter * 1000));
        }

        /// <summary>
        /// Press any key n times with delay between each pressed 
        /// </summary>
        /// <param name="n">Number of pressing</param>
        /// <param name="delay">Delay between each press  (In milliseconds)</param>
        /// <param name="ke">Browser KeyEvent</param>
        /// <param name="winKeyCode">WindowsKeycode of any key in keyboard</param>
        /// /// <param name="delayAtLast">Set delay at last (In seconds)</param>
        private void PressAnyKey(int n, int delay = 90, KeyEvent ke = new KeyEvent(), int winKeyCode = 0, double delayAtLast = 0)
        {
            if (winKeyCode != 0)
                ke.WindowsKeyCode = winKeyCode;

            for (var i = 0; i < n; i++)
            {
                Thread.Sleep(delay);
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            }
            if (delayAtLast > 0)
                Thread.Sleep((int)(delayAtLast * 1000));
        }

        /// <summary>
        /// Enter Characters in TextBox  
        /// </summary>
        /// <param name="charString">String to be entered</param>
        /// <param name="typingDelay">Delay between typing</param>
        /// <param name="delayBefore">Set delay before the typing</param>
        /// <param name="delayAtLast">Set delay at last</param>
        private void EnterChars(string charString, double typingDelay = 0.09, double delayBefore = 0, double delayAtLast = 0)
        {
            if (string.IsNullOrEmpty(charString)) return;

            if (delayBefore > 0)
                Thread.Sleep((int)(delayBefore * 1000));

            charString.ToList().ForEach(x =>
            {
                var ke = new KeyEvent
                {
                    WindowsKeyCode = x,
                    FocusOnEditableField = false,
                    IsSystemKey = false,
                    Type = KeyEventType.Char
                };
                Thread.Sleep((int)(typingDelay * 1000));
                Browser.GetBrowser().GetHost().SendKeyEvent(ke);
            });
            if (delayAtLast > 0)
                Thread.Sleep((int)(delayAtLast * 1000));
        }

        private bool _htmlHasUserName;
        private void GoogleBrowserLogin(string html)
        {
            try
            {
                if (html == "<html><head></head><body></body></html>") return;
                // BrowserAct(ActType.ClickById,"sign-in-btn",delayAfter:3);
                lock (_googleLock)
                {
                    if(_isLoggedIn) return;
                    
                    string pageText;
                    var last30Secs = DateTime.Now;
                    while (string.IsNullOrEmpty((pageText = Browser.GetTextAsync().Result).Trim()))
                    {
                        Thread.Sleep(1000);
                        if(last30Secs.AddSeconds(30) < DateTime.Now) return;
                    }
                    
                    if(!_htmlHasUserName)
                        _htmlHasUserName = html.ToLower().Contains($"\"opep7c\":\"{DominatorAccountModel.UserName.ToLower()}\"") 
                                           || pageText.Contains("Protect your account") && pageText.ToLower().Contains(DominatorAccountModel.UserName.ToLower());
                    SetGoogleLangAsEng(pageText, _htmlHasUserName);

                    if (!IsGoogleAccountLoginFailed(pageText, ref html))
                    {
                        if (/*html.Contains("identifierNext")*/ pageText.Contains("\nForgot email?\n") && (pageText.Contains("\nEmail or phone\n") || pageText.ToLower().Contains($"\n{DominatorAccountModel.AccountBaseModel.UserName.Trim().ToLower()}\n")) && !pageText.Contains("Confirm the recovery email address"))
                        {
                            BrowserAct(ActType.EnterValueById, "identifierId", value: DominatorAccountModel.AccountBaseModel.UserName, delayAfter: 1);
                            PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key //BrowserAct(ActType.ClickById,"identifierNext", 3, 2);
                            return;
                        }
                        if (/*html.Contains("passwordNext")*/(pageText.Trim().ToLower().Contains(DominatorAccountModel.AccountBaseModel.UserName.Trim().ToLower()) || pageText.Contains("To continue, first verify it's you")) && pageText.Contains("\nEnter your password\n"))
                        {
                            BrowserAct(ActType.EnterValueByName, "password", value: DominatorAccountModel.AccountBaseModel.Password, delayAfter: 1);
                            PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3);//Press Enter key //BrowserAct(ActType.ClickById,"passwordNext", 2, 2);
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
                if (_isLoggedIn || TargetUrl.ToLower().Contains("www.youtube.com/watch?")
                                || htmlHasUserName || string.IsNullOrEmpty(pageText) || pageText== "Account\n\n\n"
                                || pageText.Contains("Protect your account") || pageText.Contains("Loading, please wait ...")
                                || pageText.Contains("English (") || pageText.Contains("Personal info"))
                    return;

                // Open Google Language ListBox in Browser
                BrowserAct(ActType.ClickByClass, "vRMGwf oJeWuf",delayAfter:1);
                
                var ke = new KeyEvent();
                PressAnyKey(70, ke: ke, winKeyCode: 38); // Press Up Arrow Key
                PressAnyKey(8, ke: ke, winKeyCode: 40,delayAtLast:0.5); // Press Down Arrow Key
                PressAnyKey(1, 0, ke, 13,1); // Press Enter Key
            }
            catch
            { /* Ignored */}

        }

        public bool SetVideoQuality;

        private void SetVideoQualityAs144P()
        {
            if (SetVideoQuality || !TargetUrl.ToLower().Contains("www.youtube.com/watch?")) return;

            if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube)
                BrowserAct(ActType.ClickByClass, "ytp-ad-skip-button-icon", 1.5); // for Skipping add

            BrowserAct(ActType.ClickByClass, "ytp-volume-slider", 3, 0.1); // To Open Volume Slider

            var ke = new KeyEvent();
            PressAnyKey(21, 100, ke, 40); //Press Down Arrow key 40 times to mute the music

            BrowserAct(ActType.ClickByClass, "ytp-ad-skip-button-icon", 3, 0.5); // for Skipping add

            //ClickByClass("ytp-mute-button ytp-button"); // Direct Mute the music

            BrowserAct(ActType.ClickByClass, "ytp-button ytp-settings-button", 0.5, 1.5); // Click Youtube MediaPlayer Setting Button

            PressAnyKey(4, 900, ke, 40, 1); //Press Down Arrow key 4 times
            PressAnyKey(1, 0, ke, 39, 1); //Press right Arrow key 1 time
            PressAnyKey(6, 900, ke, 40, 1); //Press Down Arrow key 6 times
            PressAnyKey(1, 0, ke, 38, 1); //Press up Arrow key 1 time
            PressAnyKey(1, 0, ke, 13); //Press Enter key

            SetVideoQuality = true;

            new System.Threading.Tasks.Task(() =>
            {
                while (!Browser.IsDisposed)
                {
                    if (!Browser.IsDisposed)
                        BrowserAct(ActType.ClickByClass, "ytp-ad-skip-button-icon", 3.5); // for Skipping add
                }
            }).Start();

        }
        
        private bool RetypeEmail()
        {
            var isRetype = true;
            if (DominatorAccountModel.AccountBaseModel.Status != AccountStatus.ReTypeEmail && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AlternateEmail))
            {
                BrowserAct(ActType.EnterValueById, "identifierId", 1.5, 2, DominatorAccountModel.AccountBaseModel.AlternateEmail);
                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3.5); //Press Enter key //BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 2.5);
                var pageText = Browser.GetTextAsync().Result;
                if (isRetype = pageText.Contains("The email you entered is incorrect. Try again.") || pageText.Contains("Try again with a valid email address"))
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", "Alternate Email is incorrect for verification");
            }

            if (isRetype)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypeEmail;
            
            return isRetype;
        }

        private bool RetypePhoneNumber()
        {
            var isRetype = true;
            if (DominatorAccountModel.AccountBaseModel.Status != AccountStatus.ReTypePhoneNumber && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AlternateEmail))
            {
                EnterChars(DominatorAccountModel.AccountBaseModel.PhoneNumber, delayAtLast: 1);
                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);
                var pageText = Browser.GetTextAsync().Result;
                if (isRetype = pageText.Contains("This number doesn't match the one you provided. Try again."))
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", $"This number({DominatorAccountModel.AccountBaseModel.PhoneNumber}) doesn't match the one you provided. Try again.");
            }

            if (isRetype)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.ReTypePhoneNumber;

            return isRetype;
        }

        private bool VerifyCodeFromPhone()
        {
            if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.PhoneVerification
               || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
               || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount)
                return true;

            PressAnyKey(3, 200, winKeyCode: 9); //Press Tab 3 Times key

            var last2Min = DateTime.Now;
            while (DominatorAccountModel.VarificationCode.Trim().Length < 6 && !Browser.IsDisposed && last2Min.AddMinutes(2) > DateTime.Now)
                Thread.Sleep(1000);  // Waiting to get code from UI

            var isWrong = true;
            if (DominatorAccountModel.VarificationCode.Trim().Length>0)
            {
                EnterChars(DominatorAccountModel.VarificationCode.Trim(), 0.3);  //Entering Verification Code
                EnterChars(DominatorAccountModel.VarificationCode.Trim(), 0.3); //Re-Entering Verification Code
                PressAnyKey(6, 300, winKeyCode: 37, delayAtLast: 0.5); //Taking Cursor 6 step behind on Code Text
                PressAnyKey(6, 300, winKeyCode: 8, delayAtLast: 0.5); //Now removing first 5 numbers of entered code

                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key

                var pageText = Browser.GetTextAsync().Result;

                isWrong = pageText.Contains("Wrong code. Try again.") || pageText.Contains("Wrong number of digits. Please try again.");
                if (isWrong)
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                        "You have entered wrong Verification code. Try again.");
                else if (isWrong = (pageText.Contains("Too many failed attempts") && pageText.Contains("Unavailable because of too many failed attempts. Try again in a few hours.")))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                        "Too many failed attempts on Phone Verification. Try again in a few hours.");
                    DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                } 
            }

            if (isWrong && DominatorAccountModel.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
            
            DominatorAccountModel.VarificationCode = "";
            return isWrong;
        }

        private bool AddPhoneNumber()
        {
            var isWrong = true;
            if (!(DominatorAccountModel.AccountBaseModel.Status == AccountStatus.TooManyAttemptsOnPhoneVerification
                  || DominatorAccountModel.AccountBaseModel.Status == AccountStatus.AddPhoneNumberToYourAccount)
                && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber))
            {
                //BrowserAct(ActType.ClickByClass, "whsOnd zHQkBf");
                EnterChars(DominatorAccountModel.AccountBaseModel.PhoneNumber, delayAtLast: 1);
                PressAnyKey(1, 0, winKeyCode: 13, delayAtLast: 3); //Press Enter key // BrowserAct(ActType.ClickByClass, "RveJvd snByac", delayAfter: 3);

                var text = Browser.GetTextAsync().Result;
                isWrong = !text.Contains("A text message with a 6-digit verification code was just sent to");
                if (!isWrong)
                {
                    var number = Utilities.GetBetween(text, "code was just sent to", "\n");
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", $"A text message with a 6-digit verification code was just sent to {number}. Enter the code within 2 minutes.");
                }
                else if (isWrong = text.Contains("Sorry, Google didn't recognise the number that you have entered. Please check the country and number."))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage,
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", $"Google didn't recognise the number that you have entered. Please check the country and number({ DominatorAccountModel.AccountBaseModel.PhoneNumber}).");
                }
            }

            if (isWrong && DominatorAccountModel.AccountBaseModel.Status != AccountStatus.TooManyAttemptsOnPhoneVerification)
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.AddPhoneNumberToYourAccount;
            
            return isWrong;
        }

        bool _loginFailed;
        private bool IsGoogleAccountLoginFailed(string pageText, ref string html)
        {
            if (_isLoggedIn) return false;

            #region Google, Set with English Language

            if (!pageText.Contains("English (")) return false;
            var loginFailed = false;
            if (pageText.Contains("Couldn't find your Google Account")
                || pageText.Contains("Enter a valid email or phone number")
                || pageText.Contains("Wrong password. Try again or click Forgot password to reset it")
                || pageText.Contains("Your password was changed"))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;
                loginFailed = true;
            }
            else if (pageText.Contains("Change password")
                     && pageText.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                     && pageText.Contains("Those passwords didn't match. Try again."))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", "Those passwords didn't match. Try again.");
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
                loginFailed = true;
            }
            else if (pageText.Contains("Change password")
                     && pageText.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                     && pageText.Contains("Use a mix of letters, numbers and symbols to create a stronger password"))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login", "Use a mix of letters, numbers and symbols to create a stronger password");
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
                loginFailed = true;
            }
            else if (pageText.Contains("Change password")
                && pageText.Contains("There's been suspicious activity on your Google Account. For your protection, you need to change your password.")
                && pageText.Contains("Create password"))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.SetNewPassword;
                loginFailed = true;
            }
            else if (pageText.Contains("Unavailable because of too many attempts. Please try again later.")
                || pageText.Contains("It is not available because too many attempts have been failed. Try again in a few hours."))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                loginFailed = true;
            }
            else if (pageText.Contains("Confirm your recovery email"))
            {
                BrowserAct(ActType.ClickByClass, "vdE7Oc", delayAfter: 2.5);
            }
            else if (pageText.Contains("Confirm the recovery email address"))
            {
                loginFailed = RetypeEmail();
                if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.AlternateEmail.Trim()))
                    DominatorAccountModel.AccountBaseModel.AlternateEmail =
                        Utilities.GetBetween(pageText, "your account:", "\n").Trim();
            }
            else if (pageText.Contains("Confirm your recovery phone number"))
            {
                BrowserAct(ActType.ClickByClass, "vdE7Oc", delayAfter: 2.5);
            }
            else if (pageText.Contains("Confirm the phone number"))
            {
                loginFailed = RetypePhoneNumber();
                if (string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.PhoneNumber.Trim()))
                    DominatorAccountModel.AccountBaseModel.PhoneNumber =
                        Utilities.GetBetween(pageText, "security settings:", "\n").Trim();
            }
            else if (pageText.Contains("Enter a phone number to get a text message with a verification code"))
            {
                loginFailed = AddPhoneNumber();
            }
           else if (pageText.Contains("Too many failed attempts") && pageText.Contains("Unavailable because of too many failed attempts. Try again in a few hours."))
            {
                GlobusLogHelper.log.Info(Log.CustomMessage,
                    DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, "Account Browser Login",
                    "Too many failed attempts on Phone Verification. Try again in a few hours.");
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                loginFailed = true;
            }
            else if (pageText.Contains("A text message with a 6-digit verification code was just sent to"))
            {
                loginFailed = VerifyCodeFromPhone();
            }
            else if (pageText.Contains("Get a verification code")
                || pageText.Contains("Do you have your phone?")
                || pageText.Contains("Google will send a notification to your phone to verify that it's you")
            )
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                loginFailed = true;
            }
            else if (pageText.Contains("Type the text you hear or see")
                     || pageText.Contains("Google couldn't verify this account belongs to you.")
                     || pageText.Contains("This device isn't recognized. For your security, Google wants to make sure that it's really you.")
                     || pageText.Contains("This device isn't recognised. For your security, Google wants to make sure that it's really you."))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                loginFailed = true;
            }
            else if (pageText.Contains("An error occurred. please try again.") || pageText.ToLower().Contains("something went wrong"))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Failed;
                loginFailed = true;
            }
            else if (pageText.Contains("Protect your account") && pageText.Contains("Tell Google how to reach you in case you forget your password"))
            {
                html = DominatorAccountModel.UserName.ToLower();
                return false;
            }
            else if (pageText.Contains("You've tried to sign in too many times."))
            {
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnSignIn;
                loginFailed = true;
            }

            if (loginFailed && !_loginFailed)
            {
                _loginFailed = true;
                DominatorAccountModel.IsUserLoggedIn = false;
                DominatorAccountModel.Cookies = new CookieCollection();

                new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                    .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                    .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                    .SaveToBinFile();
            }
            #endregion

            return loginFailed;
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
                        cookieCollection.Add(new System.Net.Cookie
                        {
                            Expires = (DateTime)item.Expires,
                            Name = item.Name,
                            Value = item.Value,
                            Domain = item.Domain,
                            Path = item.Path,
                            Secure = item.Secure
                        });
                    }
                    catch
                    {
                        //ignored
                    }
                }

                var requestParameters = (RequestParameters)DominatorAccountModel.HttpHelper.GetRequestParameter();
                requestParameters.Cookies = cookieCollection;
                DominatorAccountModel.HttpHelper.SetRequestParameter(requestParameters);

                var url = SocialHomeUrls();

                IResponseParameter objResponseParameter = (ResponseParameter)DominatorAccountModel.HttpHelper.GetRequest(url);

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
                }

                _isLoggedIn = true;
                _loginFailed =false;

                CreateChannelOnYoutube();

                DominatorAccountModel.Cookies = cookieCollection;
                DominatorAccountModel.IsUserLoggedIn = true;
                DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                  .AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                  .AddOrUpdateLoginStatus(DominatorAccountModel.IsUserLoggedIn)
                  .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                   .SaveToBinFile();

                //AccountsFileManager.Edit(DominatorAccountModel);
                GlobusLogHelper.log.Info($"Browser login successfull with {DominatorAccountModel.AccountBaseModel.UserName} !");
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
            {
                //ignored
            }
        }

        private void LinkedInBrowserLogin(string html)
        {
            if (!string.IsNullOrEmpty(html) && html.Contains("LinkedIn: Log In or Sign Up") && html.Contains("Be great at what you do") && html.Contains("By clicking Join now, you agree to the LinkedIn"))
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

        private void PinterestBrowserLogin(string html)
        {
            if (!string.IsNullOrEmpty(html) && html.Contains("type=\"email\"") || html.Contains("type=\"password\""))
            {
                if (!string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.UserName) && !string.IsNullOrEmpty(DominatorAccountModel.AccountBaseModel.Password))
                {
                    KeyEvent k = new KeyEvent();

                    Browser.GetBrowser().GetHost().SendKeyEvent(k);

                    var userName = DominatorAccountModel.AccountBaseModel.UserName;
                    Browser.ExecuteScriptAsync("document.getElementsByName(\"id\")[0].click()");
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                    userName.ToList<char>().ForEach((x) =>
                    {
                        k = new KeyEvent
                        {
                            WindowsKeyCode = (int)x,
                            FocusOnEditableField = false,
                            IsSystemKey = false,
                            Type = KeyEventType.Char
                        };
                        Thread.Sleep(50);
                        Browser.GetBrowser().GetHost().SendKeyEvent(k);
                    });

                    k = new KeyEvent();
                    k.FocusOnEditableField = false;
                    k.WindowsKeyCode = 9;
                    k.IsSystemKey = false;
                    k.Type = KeyEventType.KeyDown;
                    Browser.GetBrowser().GetHost().SendKeyEvent(k);

                    var password = " " + DominatorAccountModel.AccountBaseModel.Password;
                    Browser.ExecuteScriptAsync("document.getElementsByName(\"password\")[0].click()");
                    password.ToList<char>().ForEach((x) =>
                    {
                        k = new KeyEvent();
                        k.WindowsKeyCode = (int)x;
                        k.FocusOnEditableField = false;
                        k.IsSystemKey = false;
                        k.Type = KeyEventType.Char;
                        Browser.GetBrowser().GetHost().SendKeyEvent(k);
                    });

                    k = new KeyEvent();
                    k.FocusOnEditableField = false;
                    k.WindowsKeyCode = 13;
                    k.IsSystemKey = false;
                    k.Type = KeyEventType.KeyDown;
                    Browser.GetBrowser().GetHost().SendKeyEvent(k);
                    Thread.Sleep(1000);
                    if (!html.Contains("type=\"email\""))
                        Browser.ExecuteScriptAsync("document.getElementsByClassName('red SignupButton active')[0].click()");
                    else
                        Browser.ExecuteScriptAsync("document.getElementsByClassName('SignupButton')[0].click()");
                    Thread.Sleep(2000);

                }
            }

            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result) && result.Contains("\"isAuth\": true"))
            {
                LoadPostPage(true);
            }


        }

        private void InstagramBrowserLogin(string html)
        {
            if (html.Contains("Phone number, username, or email"))
            {
                KeyEvent k = new KeyEvent();
                k.FocusOnEditableField = false;
                k.WindowsKeyCode = 9;
                k.IsSystemKey = false;
                k.Type = KeyEventType.KeyDown;
                Browser.GetBrowser().GetHost().SendKeyEvent(k);

                var userName = " " + DominatorAccountModel.AccountBaseModel.UserName;
                Browser.ExecuteScriptAsync("document.getElementsByName(\"username\")[0].click()");
                Thread.Sleep(TimeSpan.FromSeconds(1));


                foreach (var x in userName.ToList<char>())
                {
                    k = new KeyEvent
                    {
                        WindowsKeyCode = (int)x,
                        FocusOnEditableField = false,
                        IsSystemKey = false,
                        Type = KeyEventType.Char
                    };
                    Browser.GetBrowser().GetHost().SendKeyEvent(k);
                }

                Thread.Sleep(TimeSpan.FromSeconds(1));
                k = new KeyEvent();
                k.FocusOnEditableField = false;
                k.WindowsKeyCode = 9;
                k.IsSystemKey = false;
                k.Type = KeyEventType.KeyDown;
                Browser.GetBrowser().GetHost().SendKeyEvent(k);


                var password = " " + DominatorAccountModel.AccountBaseModel.Password;
                //cefBrowser.ExecuteScriptAsync("document.getElementsByName(\"password\")[0].click()");
                foreach (var x in password.ToList<char>())
                {
                    k = new KeyEvent();
                    k.WindowsKeyCode = (int)x;
                    k.FocusOnEditableField = false;
                    k.IsSystemKey = false;
                    k.Type = KeyEventType.Char;
                    Browser.GetBrowser().GetHost().SendKeyEvent(k);
                }
                //password.ToList<char>().ForEach((x) =>
                //{
                //    k = new KeyEvent();
                //    k.WindowsKeyCode = (int)x;
                //    k.FocusOnEditableField = false;
                //    k.IsSystemKey = false;
                //    k.Type = KeyEventType.Char;
                //    Browser.GetBrowser().GetHost().SendKeyEvent(k);

                //});

                k = new KeyEvent();
                k.FocusOnEditableField = false;
                k.WindowsKeyCode = 13;
                k.IsSystemKey = false;
                k.Type = KeyEventType.KeyDown;
                Browser.GetBrowser().GetHost().SendKeyEvent(k);
                Thread.Sleep(1000);

                var updatedHtml = Browser.GetSourceAsync().Result;

                var require = updatedHtml.Contains("choice_1") && updatedHtml.Contains("choice_0");
                if (!require && !updatedHtml.Contains("Submit"))
                {
                    // Browser.ExecuteScriptAsync("document.getElementsByClassName(\"_5f5mN\")[0].click()");
                    Browser.ExecuteScriptAsync("document.getElementsByClassName(\"sqdOP\")[1].click()");
                    Thread.Sleep(2000);
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
                Browser.ExecuteScriptAsync(
                    "document.getElementsByClassName('js-username-field email-input js-initial-focus')[0].value= '" +
                    DominatorAccountModel.AccountBaseModel.UserName + "'");

                Thread.Sleep(2000);
                Browser.ExecuteScriptAsync("document.getElementsByClassName('js-password-field')[0].value= '" +
                                           DominatorAccountModel.AccountBaseModel.Password.Replace("'", "\\'") + "'");
                Thread.Sleep(2000);
                Browser.ExecuteScriptAsync(
                    "document.getElementsByClassName('submit EdgeButton EdgeButton--primary EdgeButtom--medium')[0].click()");
                Thread.Sleep(4000);

            }

            var result = GetLoggedInPageSource();

            if (!string.IsNullOrEmpty(result) && result.Contains("signout") && result.Contains("timeline-tweet-box"))
            {
                LoadPostPage(true);
            }
        }

        private void QuoraLogin(string html)
        {
            lock (_syncLock)
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
            {
                LoadPostPage(true);
            }
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
        internal class MenuHandler : IContextMenuHandler
        {
            private const int Refresh = 1;
            private const int Back = 2;
            private const int Forward = 3;
            void IContextMenuHandler.OnBeforeContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model)
            {
                //To disable the menu then call clear
                model.Clear();
                //Add new custom menu items
                //model.AddItem((CefMenuCommand)Refresh, "Refresh");
                //model.AddItem((CefMenuCommand)Back, "Back");
                //model.AddItem((CefMenuCommand)Forward, "Forward");
              
            }
            bool IContextMenuHandler.OnContextMenuCommand(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, CefMenuCommand commandId, CefEventFlags eventFlags)
            {
                if ((int)commandId == Refresh)
                {
                    browser.Reload();
                }
                if ((int)commandId == Back)
                {
                    browser.GoBack();
                }
                if ((int)commandId == Forward)
                {
                    browser.GoForward();
                }

                return false;
            }

            void IContextMenuHandler.OnContextMenuDismissed(IWebBrowser browserControl, IBrowser browser, IFrame frame)
            {

            }

            bool IContextMenuHandler.RunContextMenu(IWebBrowser browserControl, IBrowser browser, IFrame frame, IContextMenuParams parameters, IMenuModel model, IRunContextMenuCallback callback)
            {
                return false;
            }
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