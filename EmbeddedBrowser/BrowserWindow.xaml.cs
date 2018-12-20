using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Windows;
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
            Browser.IsBrowserInitializedChanged += LoadSettings;

        }

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


        private void GoogleBrowserLogin(string html)
        {
            try
            {
                Browser.ExecuteScriptAsync("document.getElementById('sign-in-btn').click()");

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            Thread.Sleep(3000);

            if (html.Contains("identifierNext"))
            {
                try
                {
                    if (!AskingForRecoveryEmail())
                        Browser.ExecuteScriptAsync("document.getElementById('identifierId').value= '" +
                                                   DominatorAccountModel.AccountBaseModel.UserName + "'");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                Thread.Sleep(2000);
                try
                {
                    Browser.ExecuteScriptAsync("document.getElementById('identifierNext').click()");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                Thread.Sleep(2000);
            }
            if (html.Contains("passwordNext"))
            {
                try
                {
                    Browser.ExecuteScriptAsync("document.getElementsByName('password')[0].value= '" + DominatorAccountModel.AccountBaseModel.Password + "'");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                Thread.Sleep(2000);
                try
                {
                    Browser.ExecuteScriptAsync("document.getElementById('passwordNext').click()");

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                Thread.Sleep(2000);
            }

            string username = DominatorAccountModel.UserName.ToLower();

            if (!IsGoogleAccountLoginFailed()
                && html.ToLower().Contains(username)
                && !string.IsNullOrEmpty(html)
                && html != "<html><head></head><body></body></html>"
                && SaveCookie())
            {
                if (string.IsNullOrEmpty(TargetUrl))
                    TargetUrl = SocialHomeUrls();

                var result = GetLoggedInPageSource();

                if (!string.IsNullOrEmpty(result))
                    LoadPostPage(true);

            }
        }

        private bool AskingForRecoveryEmail()
        {
            try
            {
                if (_dominatorAccountModel.IsUserLoggedIn) return false;

                var pageText = Browser.GetTextAsync().Result;

                if (
                    (pageText.Contains("English (") /*Google, Set with English Language*/ &&
                     pageText.Contains("Confirm the recovery email address that you added to your account:"))
                    ||
                    (pageText.Contains("Español (España)") /*Google, Set with Español (España) Language*/ &&
                     pageText.Contains(
                         "Confirma la dirección de correo electrónico alternativa que has añadido a tu cuenta:"))
                    ||
                    (pageText.Contains("Português (Brasil)?") /*Google, Set with Português (Brasil)? Language*/ &&
                     pageText.Contains("Confirme o endereço de e-mail de recuperação adicionado à sua conta:"))
                )
                {
                    DominatorAccountModel.IsUserLoggedIn = false;
                    DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool IsGoogleAccountLoginFailed()
        {
            try
            {
                if (!_dominatorAccountModel.IsUserLoggedIn)
                {
                    string PageText = Browser.GetTextAsync().Result;

                    #region Google, Set with English Language
                    if (PageText.Contains("English ("))
                    {
                        if (PageText.Contains("Couldn't find your Google Account")
                                       || PageText.Contains("Enter a valid email or phone number")
                                       || PageText.Contains("Wrong password. Try again or click Forgot password to reset it")
                                       || PageText.Contains("Your password was changed"))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;

                            return true;
                        }
                        else if (PageText.Contains("Type the text you hear or see") || PageText.Contains("Confirm your recovery email"))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                            return true;
                        }
                        else if (PageText.Contains("Unavailable because of too many attempts. Please try again later.")
                            || PageText.Contains("It is not available because too many attempts have been failed. Try again in a few hours."))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                            return true;
                        }
                        else if (PageText.Contains("Get a verification code at")
                            || PageText.Contains("This device isn't recognised. For your security, Google wants to make sure that it's really you.")
                            || PageText.Contains("Do you have your phone?")
                            || PageText.Contains("Google will send a notification to your phone to verify that it's you")
                            )
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                            return true;
                        }
                    }

                    #endregion

                    #region Google, Set with Español (España) Language
                    else if (PageText.Contains("Español (España)"))
                    {
                        if (PageText.Contains("No se ha podido encontrar tu cuenta de Google")
                                       || PageText.Contains("Introduce una dirección de correo electrónico o un número de teléfono válidos")
                                       || PageText.Contains("Contraseña incorrecta. Vuelve a intentarlo o haz clic en Contraseña olvidada para cambiarla.")
                                       || PageText.Contains("Tu contraseña se ha cambiado hace"))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;

                            return true;
                        }
                        else if (PageText.Contains("Type the text you hear or see"))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                            return true;
                        }
                        else if (PageText.Contains("No está disponible porque se han realizado demasiados intentos. Inténtalo de nuevo más tarde.")
                             || PageText.Contains("No está disponible porque se han fallado demasiados intentos.Inténtalo de nuevo dentro de unas horas."))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                            return true;
                        }
                        else if (PageText.Contains("Recibe un código de verificación en el número")
                            || PageText.Contains("No se reconoce este dispositivo. Por tu seguridad, Google quiere comprobar que seas tú.")
                            || PageText.Contains("Tienes tu teléfono?")
                            || PageText.Contains("Google enviará una notificación a tu teléfono para verificar tu identidad")
                            )
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                            return true;
                        }
                    }
                    #endregion

                    #region Google, Set with Português (Brasil)? Language
                    else if (PageText.Contains("Português (Brasil)?"))
                    {
                        if (PageText.Contains("Não foi possível encontrar sua Conta do Google")
                                       || PageText.Contains("Digite um e-mail ou número de telefone válido")
                                       || PageText.Contains("Senha incorreta. Tente novamente ou clique em \"Esqueceu a senha?\" para redefini-la.")
                                       || PageText.Contains("Sua senha foi alterada há "))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.InvalidCredentials;

                            return true;
                        }
                        else if (PageText.Contains("Type the text you hear or see"))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.NeedsVerification;
                            return true;
                        }
                        else if (PageText.Contains("Indisponível devido a tentativas em excesso. Tente novamente mais tarde.")
                            || PageText.Contains("It is not available because too many attempts have been failed. Try again in a few hours."))
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TooManyAttemptsOnPhoneVerification;
                            return true;
                        }
                        else if (PageText.Contains("Receber um código de verificação no número ")
                            || PageText.Contains("This device isn't recognised. For your security, Google wants to make sure that it's really you.")
                            || PageText.Contains("Você está com seu smartphone?")
                            || PageText.Contains("O Google enviará uma notificação ao seu smartphone para verificar sua identidade")
                            )
                        {
                            DominatorAccountModel.IsUserLoggedIn = false;
                            DominatorAccountModel.AccountBaseModel.Status = AccountStatus.PhoneVerification;
                            return true;
                        }
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
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

        private bool _isLoggedIn;
        private bool SaveCookie()
        {
            lock (_googleLock)
            {
                if (_isLoggedIn) return false;

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
                                Expires = (DateTime) item.Expires,
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
                        string GooglePlusAcc = Utilities.GetBetween(objResponseParameter.Response, "\"oPEP7c\":\"", "\"");
                        if (string.IsNullOrEmpty(GooglePlusAcc) || cookieCollection.Count < 2)
                            return false;

                        DominatorAccountModel.AccountBaseModel.ProfileId = GooglePlusAcc;
                    }

                    if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Youtube)
                    {
                        if (objResponseParameter.Response.Contains("Sign in now to see your channels") && !string.IsNullOrEmpty(objResponseParameter.Response) && objResponseParameter.Response != "<html><head></head><body></body></html>")
                            return false;
                    }

                    _isLoggedIn = true;
                    DominatorAccountModel.Cookies = cookieCollection;
                    DominatorAccountModel.IsUserLoggedIn = true;
                    DominatorAccountModel.AccountBaseModel.Status = AccountStatus.Success;

                    var socinatorAccountBuilder = new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
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
                return true;
            }
        }
        

    }
}