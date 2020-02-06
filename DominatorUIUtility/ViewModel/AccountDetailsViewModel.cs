using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.EmailService;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using LegionUIUtility.CustomControl;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.Extensions;
using Unity;

namespace LegionUIUtility.ViewModel
{
    public class AccountDetailsViewModel : BindableBase
    {
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountScopeFactory _accountScopeFactory;
        private readonly IHttpHelper _httpHelper;
        private readonly IProxyFileManager _proxyFileManager;

        #region Properties
        public DominatorAccountModel DominatorAccountModel { get; set; }
        public DominatorAccountModel OldDominatorAccountModel { get; set; }
        private AccessorStrategies strategy;
        private bool _isEmailVerification;
        public bool IsEmailVerification
        {
            get { return _isEmailVerification; }
            set
            {
                if (_isEmailVerification == value)
                    return;
                if (SetProperty(ref _isEmailVerification, value))
                    if (!IsEmailVerificationCodeSent && IsEmailVerification)
                        SetVerificationCodeVisibility(true);
                    else if (IsEmailVerificationCodeSent && IsEmailVerification)
                        SetVerificationCodeVisibility(false);
            }
        }
        private bool _isPhoneVerification;

        public bool IsPhoneVerification
        {
            get { return _isPhoneVerification; }
            set
            {
                if (SetProperty(ref _isPhoneVerification, value))
                    if (!IsPhoneVerificationCodeSent && IsPhoneVerification)
                        SetVerificationCodeVisibility(true);
                    else if (IsPhoneVerificationCodeSent && IsPhoneVerification)
                        SetVerificationCodeVisibility(false);

            }
        }
        private Visibility _verificationSectionVisibility;

        public Visibility VerificationSectionVisibility
        {
            get { return _verificationSectionVisibility; }
            set
            {
                if (_verificationSectionVisibility == value)
                    return;
                SetProperty(ref _verificationSectionVisibility, value);
            }
        }
        private Visibility _codeSectionVisibility = Visibility.Collapsed;


        public Visibility CodeSectionVisibility
        {
            get { return _codeSectionVisibility; }
            set
            {
                if (_codeSectionVisibility == value)
                    return;
                SetProperty(ref _codeSectionVisibility, value);
            }
        }

        private Visibility _btnSendVerificationCodeVisibility = Visibility.Collapsed;


        public Visibility BtnSendVerificationCodeVisibility
        {
            get { return _btnSendVerificationCodeVisibility; }
            set
            {
                SetProperty(ref _btnSendVerificationCodeVisibility, value);
            }
        }
        private void SetVerificationCodeVisibility(bool isVerification)
        {
            if (isVerification)
            {
                CodeSectionVisibility = Visibility.Collapsed;
                BtnSendVerificationCodeVisibility = Visibility.Visible;
            }
            else
            {
                CodeSectionVisibility = Visibility.Visible;
                BtnSendVerificationCodeVisibility = Visibility.Collapsed;
            }
        }
        private bool _isEmailVerificationCodeSent;

        public bool IsEmailVerificationCodeSent
        {
            get { return _isEmailVerificationCodeSent; }
            set { SetProperty(ref _isEmailVerificationCodeSent, value); }
        }
        private bool _isPhoneVerificationCodeSent;

        public bool IsPhoneVerificationCodeSent
        {
            get { return _isPhoneVerificationCodeSent; }
            set { SetProperty(ref _isPhoneVerificationCodeSent, value); }
        }
        private Visibility _sendResetPasswordLinkVisibility = Visibility.Collapsed;


        public Visibility SendResetPasswordLinkVisibility
        {
            get { return _sendResetPasswordLinkVisibility; }
            set
            {
                SetProperty(ref _sendResetPasswordLinkVisibility, value);
            }
        }


        //private bool _isManualVerify;
        //public bool IsManualVerify
        //{
        //    get
        //    {
        //        return _isManualVerify;
        //    }
        //    set
        //    {
        //        SetProperty(ref _isManualVerify, value);
        //    }
        //}
        private string _jsonCookies;
        public string JsonCookies
        {
            get { return _jsonCookies; }
            set
            {
                SetProperty(ref _jsonCookies, value);
            }
        }
        #endregion

        #region Constructors

        public AccountDetailsViewModel(DominatorAccountModel dataContext)
        {
            _accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            _accountScopeFactory = ServiceLocator.Current.GetInstance<IAccountScopeFactory>();
            _proxyFileManager = ServiceLocator.Current.GetInstance<IProxyFileManager>();
            _httpHelper = _accountScopeFactory[dataContext.AccountId]
                .Resolve<IHttpHelper>(dataContext.AccountBaseModel.AccountNetwork.ToString());
            DominatorAccountModel = dataContext;

            // Take backup of current DominatorAccountModel object
            UpdateOldDominatorAccountModel();

            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            CancelCommand = new BaseCommand<object>(CancelCanExecute, CancelExecute);
            AddNewCookiesCommand = new BaseCommand<object>(AddNewCookiesCanExecute, AddNewCookiesExecute);
            RemoveCookiesCommand = new BaseCommand<object>(RemoveCookiesCanExecute, RemoveCookiesExecute);
            VerifyAccountCommand = new BaseCommand<object>(VerifyAccountCanExecute, VerifyAccountExecute);
            SendVerificationCodeCommand = new BaseCommand<object>(SendVerificationCodeCanExecute, SendVerificationCodeExecute);
            SetNewPasswordCommand = new BaseCommand<object>(sender => true, SetNewPasswordExecute);
            SendResetPasswordLinkCommand = new BaseCommand<object>(sender => true, SendResetPasswordLinkExecute);
            CopyCommand = new BaseCommand<object>(CopyCanExecute, CopyExecute);
            SaveJsonCookiesCommand = new BaseCommand<object>(SaveJsonCookiesCanExecute, SaveJsonCookiesExecute);
        }
        #endregion

        #region Commands

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddNewCookiesCommand { get; set; }
        public ICommand RemoveCookiesCommand { get; set; }
        public ICommand VerifyAccountCommand { get; set; }
        public ICommand SendVerificationCodeCommand { get; set; }
        public ICommand SetNewPasswordCommand { get; set; }
        public ICommand SendResetPasswordLinkCommand { get; set; }
        public ICommand CopyCommand { get; set; }
        public ICommand SaveJsonCookiesCommand { get; set; }
        #endregion

        private bool SaveCanExecute(object arg) => true;
        private void SaveExecute(object sender)
        {
            if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.TryingToLogin)
            {
                Dialog.ShowDialog("LangKeyLogin".FromResourceDictionary(), "LangKeyAlreadyCheckingLoginSoWait".FromResourceDictionary());
                return;
            }
            else if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.UpdatingDetails)
            {
                Dialog.ShowDialog("LangKeyLogin".FromResourceDictionary(), "LangKeyAlreadyUpdatingDetailsSoWait".FromResourceDictionary());
                return;
            }


            DominatorAccountModel.CookieHelperList?.ToList().ForEach(cookie =>
            {
                if (string.IsNullOrEmpty(cookie.Name) || string.IsNullOrEmpty(cookie.Value))
                    DominatorAccountModel.CookieHelperList.Remove(cookie);
            });

            if (!EditAccount())
                return;

            #region Checking status
            
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var networkCoreFactory = SocinatorInitialize
                        .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory();

                    var accountFactory = networkCoreFactory.AccountUpdateFactory;
                    var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;

                    DominatorAccountModel.AccountBaseModel.Status = AccountStatus.TryingToLogin;

                    await asyncAccount.CheckStatusAsync(DominatorAccountModel, DominatorAccountModel.Token);

                    if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        ScheduleActivity();
                        await asyncAccount.UpdateDetailsAsync(DominatorAccountModel, DominatorAccountModel.Token);
                        new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                            .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                            .SaveToBinFile();
                    }
                    else
                    {
                        DominatorAccountModel.DisplayColumnValue1 = 0;
                        DominatorAccountModel.DisplayColumnValue2 = 0;
                        DominatorAccountModel.DisplayColumnValue3 = 0;
                        DominatorAccountModel.DisplayColumnValue4 = 0;

                        new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId)
                        .AddOrUpdateDisplayColumn1(DominatorAccountModel.DisplayColumnValue1)
                        .AddOrUpdateDisplayColumn2(DominatorAccountModel.DisplayColumnValue2)
                        .AddOrUpdateDisplayColumn3(DominatorAccountModel.DisplayColumnValue3)
                        .AddOrUpdateDisplayColumn4(DominatorAccountModel.DisplayColumnValue4)
                        .AddOrUpdateProxy(DominatorAccountModel.AccountBaseModel.AccountProxy)
                        .AddOrUpdateMailCredentials(DominatorAccountModel.MailCredentials)
                        .AddOrUpdateIsAutoVerifyByEmail(DominatorAccountModel.IsAutoVerifyByEmail)
                        .SaveToBinFile();
                    }

                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                finally
                {
                    var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                    globalDbOperation.UpdateAccountDetails(DominatorAccountModel);
                }
            });

            #endregion
        }

        private void ScheduleActivity()
        {
            var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
            runningActivityManager.ScheduleIfAccountGotSucess(DominatorAccountModel);
        }

        private bool EditAccount()
        {

            if (OldDominatorAccountModel == null) return false;

            var newAccountBaseModel = DominatorAccountModel.AccountBaseModel;
            IProxyManagerViewModel proxyManagerViewModel = null;
            try
            {
                if (string.IsNullOrEmpty(newAccountBaseModel.UserName) ||
                           string.IsNullOrEmpty(newAccountBaseModel.Password))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName,
                        "LangKeyAccount".FromResourceDictionary(), "LangKeySavingAccountFailedUserOrPasswordEmpty".FromResourceDictionary());
                    return false;
                }


                if ((!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                     string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort))
                    || (string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                        !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort)))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName,
                      "LangKeyAccount".FromResourceDictionary(), "LangKeySavingAccountFailedProxyIpOrProxyPortEmptyButNotBoth".FromResourceDictionary());
                    return false;
                }


                if ((!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyUsername) &&
                     string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPassword))
                    || (string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyUsername) &&
                        !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPassword)))
                {
                    GlobusLogHelper.log.Info(Log.CustomMessage, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName,
                      "LangKeyAccount".FromResourceDictionary(), "LangKeySavingAccountFailedProxyUsernameOrProxyPasswordEmptyButNotBoth".FromResourceDictionary());
                    return false;
                }


                if (OldDominatorAccountModel.AccountBaseModel.UserName != DominatorAccountModel.AccountBaseModel.UserName
                    || OldDominatorAccountModel.AccountBaseModel.Password != DominatorAccountModel.AccountBaseModel.Password
                    || OldDominatorAccountModel.UserAgentWeb != DominatorAccountModel.UserAgentWeb)
                {
                    DominatorAccountModel.CookieHelperList?.Clear();
                    _httpHelper.GetRequestParameter().Cookies = new CookieCollection();
                }
                proxyManagerViewModel = ServiceLocator.Current.GetInstance<IProxyManagerViewModel>();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }


            var oldproxies = _proxyFileManager.GetAllProxy();

            #region If proxy not empty or null

            if (!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort))
            {
                var oldAccount = _accountsFileManager.GetAccountById(OldDominatorAccountModel.AccountId).AccountBaseModel;

                if (proxyManagerViewModel != null && !proxyManagerViewModel.IsProxyAvailable(newAccountBaseModel, oldproxies, oldAccount, strategy))
                {
                    if (!proxyManagerViewModel.UpdateProxy(newAccountBaseModel, strategy))
                        proxyManagerViewModel?.AddProxyIfNotExist(newAccountBaseModel, strategy);
                }
            }

            #endregion

            else
            {
                proxyManagerViewModel?.UpdateProxy(newAccountBaseModel, oldproxies, strategy);
                try
                {
                    new SocinatorAccountBuilder(newAccountBaseModel.AccountId)
                               .AddOrUpdateProxy(new Proxy())
                               .SaveToBinFile();
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException();
                }
                catch (AggregateException ae)
                {
                    ae.HandleOperationCancellation();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            #region Save data into bin file

            try
            {
                DominatorAccountModel.Token.ThrowIfCancellationRequested();

                new SocinatorAccountBuilder(newAccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(newAccountBaseModel)
                    .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                    .AddOrUpdateUserAgentWeb(DominatorAccountModel.UserAgentWeb)
                    .SaveToBinFile();

                #region Save email creds

                if (DominatorAccountModel.IsAutoVerifyByEmail && !ObjectComparer.Compare(
                        OldDominatorAccountModel.MailCredentials,
                        DominatorAccountModel.MailCredentials))
                {
                    new SocinatorAccountBuilder(newAccountBaseModel.AccountId)
                        .AddOrUpdateMailCredentials(DominatorAccountModel.MailCredentials)
                        .SaveToBinFile();
                }

                #endregion

                // Update Old DominatorAccountModel object
                UpdateOldDominatorAccountModel();
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion

            GlobusLogHelper.log.Info(Log.AccountEdited, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName);
            return true;
        }

        private bool CancelCanExecute(object arg) => true;
        private void CancelExecute(object sender)
        {
            // Update current DominatorAccountModel
            UpdateCurrentDominatorAccountModel();

            // Back to AccountManager module
            var controlToSelect = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = controlToSelect;
            AccountManagerViewModel.GetSingletonAccountManagerViewModel().LastControlType = "AccountManager";
        }
        private bool AddNewCookiesCanExecute(object arg) => true;
        private void AddNewCookiesExecute(object sender)
        {
            try
            {
                DominatorAccountModel.CookieHelperList.Add(new CookieHelper
                {
                    Name = string.Empty,
                    Value = string.Empty

                });
                var Cookies = sender as DataGrid;
                Cookies.ItemsSource = null;
                Cookies.ItemsSource = DominatorAccountModel.CookieHelperList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private bool RemoveCookiesCanExecute(object arg) => true;
        private void RemoveCookiesExecute(object sender)
        {
            try
            {
                var Cookies = sender as DataGrid;
                var selectedItem = Cookies.SelectedItem;
                Cookies.ItemsSource = null;

                DominatorAccountModel.CookieHelperList.Remove(selectedItem as CookieHelper);
                Cookies.ItemsSource = DominatorAccountModel.CookieHelperList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private bool VerifyAccountCanExecute(object arg) => true;
        private void VerifyAccountExecute(object sender)
        {
            try
            {
                var networkCoreFactory = SocinatorInitialize
                        .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory();

                if (!string.IsNullOrEmpty(DominatorAccountModel.VarificationCode))
                {
                    var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
                    var verificationType = IsEmailVerification ? VerificationType.Email : VerificationType.Phone;
                    Task.Factory.StartNew(() =>
                    {
                        if (accountVerificationFactory.VerifyAccountAsync(DominatorAccountModel, verificationType,
                            DominatorAccountModel.Token).Result)
                        {
                            Application.Current.Dispatcher.Invoke(
                                    () => VerificationSectionVisibility = Visibility.Collapsed
                                );
                          ScheduleActivity();
                        }
                        DominatorAccountModel.VarificationCode = string.Empty;

                    });

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private bool SendVerificationCodeCanExecute(object arg) => true;
        private void SendVerificationCodeExecute(object sender)
        {
            try
            {
                IsEmailVerificationCodeSent = false;
                IsPhoneVerificationCodeSent = false;
                BtnSendVerificationCodeVisibility = Visibility.Collapsed;

                var networkCoreFactory = SocinatorInitialize
                    .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                    .GetNetworkCoreFactory();

                var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
                var verificationType = IsEmailVerification ? VerificationType.Email : VerificationType.Phone;
                Task.Factory.StartNew(() =>
                {
                    if (DominatorAccountModel.IsAutoVerifyByEmail)
                    {
                        accountVerificationFactory.AutoVerifyByEmail(DominatorAccountModel,
                            DominatorAccountModel.Token);
                    }
                    else
                    {
                        if (accountVerificationFactory
                            .SendVerificationCode(DominatorAccountModel, verificationType, DominatorAccountModel.Token).Result)
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    if (IsEmailVerification)
                                        IsEmailVerificationCodeSent = true;
                                    else
                                        IsPhoneVerificationCodeSent = true;
                                    CodeSectionVisibility = Visibility.Visible;
                                    // GlobusLogHelper.log.Info(Log.SentVerificationCode, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);
                                });
                        else
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    BtnSendVerificationCodeVisibility = Visibility.Visible;
                                    CodeSectionVisibility = Visibility.Collapsed;
                                    // GlobusLogHelper.log.Info(Log.FailedToSendVerificationCodeFaild, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);

                                });
                    }


                });
            }
            catch (Exception ex)
            {
                BtnSendVerificationCodeVisibility = Visibility.Visible;
                ex.DebugLog();
            }
        }

        private void UpdateOldDominatorAccountModel(DominatorAccountModel accountModel = null)
        {
            if (accountModel == null)
                accountModel = DominatorAccountModel;

            OldDominatorAccountModel = new DominatorAccountModel();

            OldDominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup = new ContentSelectGroup
                {
                    Content = accountModel.AccountBaseModel.AccountGroup.Content,
                },
                UserName = accountModel.AccountBaseModel.UserName,
                Password = accountModel.AccountBaseModel.Password,
                AccountId = accountModel.AccountId,
                AccountNetwork = accountModel.AccountBaseModel.AccountNetwork,
                AccountProxy =  {
                    ProxyIp = accountModel.AccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = accountModel.AccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = accountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = accountModel.AccountBaseModel.AccountProxy.ProxyPassword
                }
            };

            OldDominatorAccountModel.MailCredentials = new MailCredentials
            {
                Username = accountModel.MailCredentials.Username,
                Password = accountModel.MailCredentials.Password,
                Hostname = accountModel.MailCredentials.Hostname,
                Port = accountModel.MailCredentials.Port
            };

            OldDominatorAccountModel.UserAgentWeb = accountModel.UserAgentWeb;
            accountModel.CookieHelperList.ForEach(x => { OldDominatorAccountModel.CookieHelperList.Add(x); });
            OldDominatorAccountModel.AccountId = accountModel.AccountId;
        }
        public void UpdateCurrentDominatorAccountModel()
        {
            DominatorAccountModel.AccountBaseModel.UserName = OldDominatorAccountModel.AccountBaseModel.UserName;
            DominatorAccountModel.AccountBaseModel.Password = OldDominatorAccountModel.AccountBaseModel.Password;
            DominatorAccountModel.AccountBaseModel.AccountId = OldDominatorAccountModel.AccountBaseModel.AccountId;
            DominatorAccountModel.AccountBaseModel.AccountNetwork = OldDominatorAccountModel.AccountBaseModel.AccountNetwork;
            DominatorAccountModel.AccountBaseModel.AccountGroup.Content = OldDominatorAccountModel.AccountBaseModel.AccountGroup.Content;
            DominatorAccountModel.AccountBaseModel.AccountProxy = OldDominatorAccountModel.AccountBaseModel.AccountProxy;
            DominatorAccountModel.AccountBaseModel.UserFullName = OldDominatorAccountModel.AccountBaseModel.UserFullName;

            DominatorAccountModel.MailCredentials = OldDominatorAccountModel.MailCredentials;
            DominatorAccountModel.UserAgentWeb = OldDominatorAccountModel.UserAgentWeb;
            DominatorAccountModel.IsAutoVerifyByEmail = OldDominatorAccountModel.IsAutoVerifyByEmail;
            DominatorAccountModel.IsUseSSL = OldDominatorAccountModel.IsUseSSL;
            DominatorAccountModel.AccountBaseModel.IsChkTwoFactorLogin = OldDominatorAccountModel.AccountBaseModel.IsChkTwoFactorLogin;

            if (ObjectComparer.Compare(DominatorAccountModel.CookieHelperList, OldDominatorAccountModel.CookieHelperList))
            {
                DominatorAccountModel.CookieHelperList = OldDominatorAccountModel.CookieHelperList;
            }
        }

        private void SetNewPasswordExecute(object sender)
        {
            try
            {
                var networkCoreFactory = SocinatorInitialize
                     .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                     .GetNetworkCoreFactory();

                var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
                Task.Factory.StartNew(() =>
                {
                    if (DominatorAccountModel.IsAutoVerifyByEmail)
                    {
                        if (accountVerificationFactory.AutoVerifyByEmail(DominatorAccountModel,
                            DominatorAccountModel.Token).Result)
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "LangKeyResetPassword".FromResourceDictionary(), "LangKeyPasswordChangedSuccessfully".FromResourceDictionary());
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "LangKeyResetPassword".FromResourceDictionary(), "LangKeyFailedToChangePassword".FromResourceDictionary());
                    }
                    else
                    {
                        if (accountVerificationFactory.VerifyAccountAsync(DominatorAccountModel, VerificationType.Email,
                            DominatorAccountModel.Token).Result)
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "LangKeyResetPassword".FromResourceDictionary(), "LangKeyPasswordChangedSuccessfully".FromResourceDictionary());
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName,
                                "LangKeyResetPassword".FromResourceDictionary(), "LangKeyFailedToChangePassword".FromResourceDictionary());
                    }
                    OldDominatorAccountModel = DominatorAccountModel;

                });
            }
            catch (Exception ex)
            {
                BtnSendVerificationCodeVisibility = Visibility.Visible;
                ex.DebugLog();
            }

        }
        private void SendResetPasswordLinkExecute(object sender)
        {
            try
            {
                var networkCoreFactory = SocinatorInitialize
                    .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                    .GetNetworkCoreFactory();

                var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
                Task.Factory.StartNew(() =>
                {
                    if (DominatorAccountModel.IsManualVerify)
                    {
                        if (accountVerificationFactory.SendVerificationCode(DominatorAccountModel,
                            VerificationType.Email,
                            DominatorAccountModel.Token).Result)
                        {
                            SendResetPasswordLinkVisibility = Visibility.Visible;
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool CopyCanExecute(object arg) => true;
        private void CopyExecute(object sender)
        {
            try
            {
                var proxy = DominatorAccountModel.AccountBaseModel?.AccountProxy;
                var proxyString = "";
                if (!string.IsNullOrWhiteSpace(proxy?.ProxyIp))
                {
                    if (!string.IsNullOrWhiteSpace(proxy.ProxyUsername))
                        proxyString = $"|{proxy.ProxyIp}:{proxy.ProxyPort}:{proxy.ProxyUsername}:{proxy.ProxyPassword}:";
                    else
                        proxyString = $"|{proxy.ProxyIp}:{proxy.ProxyPort}";
                }
                var details = $"{DominatorAccountModel.AccountBaseModel.AccountNetwork.ToString()}|{DominatorAccountModel.AccountBaseModel.UserName}:{DominatorAccountModel.AccountBaseModel.Password}{proxyString}";
                new AutoItTool().CopyToClip(details);
                ToasterNotification.ShowSuccess("LangKeyDetailsCopiedToClipboard".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SaveJsonCookiesCanExecute(object arg) => true;

        private void SaveJsonCookiesExecute(object sender)
        {
            var expireString = "";
            try
            {
                if (string.IsNullOrWhiteSpace(JsonCookies?.Trim()))
                    return;
                
                var jsonHand = new JsonHandler("{\"object\" :" + JsonCookies + "}");
                
                if ((DominatorAccountModel.CookieHelperList != null && DominatorAccountModel.CookieHelperList.Count > 0) || (DominatorAccountModel.BrowserCookieHelperList != null && DominatorAccountModel.BrowserCookieHelperList.Count > 0))
                {
                    if (Dialog.ShowCustomDialog("LangKeySaveCookies".FromResourceDictionary(), "LangKeyWannaReplaceOldCookieWithNewOne".FromResourceDictionary(), "LangKeyYes".FromResourceDictionary(), "LangKeyNo".FromResourceDictionary()) == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Negative)
                        return;
                }

                var token = jsonHand.GetJToken("object");
                DominatorAccountModel.CookieHelperList = new System.Collections.Generic.HashSet<CookieHelper>();
                DominatorAccountModel.BrowserCookieHelperList = new System.Collections.Generic.HashSet<CookieHelper>();

                foreach (var t in token)
                {
                    var name = jsonHand.GetJTokenValue(t, "name");
                    var value = jsonHand.GetJTokenValue(t, "value");
                    var domain = jsonHand.GetJTokenValue(t, "domain");
                    var path = jsonHand.GetJTokenValue(t, "path");
                    expireString = jsonHand.GetJTokenValue(t, "expirationDate").Replace(",",".").Split('.')[0];
                    DateTime expire = DateTime.Now.AddMonths(6);

                    if (!string.IsNullOrWhiteSpace(expireString))
                            expire = DateTimeUtilities.EpochToDateTimeUtc(Convert.ToInt64(expireString) * 1000);

                    var httpOnly = jsonHand.GetJTokenValue(t, "httpOnly").ToLower() == "true" ? true: false;
                    var secure = jsonHand.GetJTokenValue(t, "secure").ToLower() == "true" ? true : false;

                    var cookie = new CookieHelper()
                    {
                        Name = name,
                        Value = value,
                        Domain = domain,
                        Expires = expire,
                        HttpOnly = httpOnly,
                        Secure = secure
                    };
                    DominatorAccountModel.CookieHelperList.Add(cookie);
                    if (DominatorAccountModel.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook)
                        DominatorAccountModel.BrowserCookieHelperList.Add(cookie);
                }
                
                Dialog.ShowDialog("LangKeySaveCookies".FromResourceDictionary(), "LangKeyCookiesSavedNowLogin".FromResourceDictionary());
            }
            catch (Exception ex)
            {
                if (ex.Message?.Contains(" parsing ") ?? false)
                {
                    ToasterNotification.ShowError("Cookies are not in a valid json text form.");
                }
                else
                {
                    ex.DebugLog(!string.IsNullOrWhiteSpace(expireString) ? $"expireString:{expireString}" : "");
                    ToasterNotification.ShowError("Oops! An error occured.");
                }
            }
        }
    }
}
