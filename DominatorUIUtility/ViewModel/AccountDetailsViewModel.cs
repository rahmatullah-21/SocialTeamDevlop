using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.EmailService;

namespace DominatorUIUtility.ViewModel
{
    public class AccountDetailsViewModel : BindableBase
    {
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
                    if (!IsEmailVerificationCodeSent)
                        SetVerificationCodeVisibility(IsEmailVerification);
                    else if (IsEmailVerificationCodeSent && IsEmailVerification)
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
        private bool _isPhoneVerification;

        public bool IsPhoneVerification
        {
            get { return _isPhoneVerification; }
            set
            {
                if (SetProperty(ref _isPhoneVerification, value))
                    if (!IsPhoneVerificationCodeSent)
                        SetVerificationCodeVisibility(IsPhoneVerification);
                    else if (IsPhoneVerificationCodeSent && IsPhoneVerification)
                     SetVerificationCodeVisibility(false);
                  
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

        public AccountDetailsViewModel()
        {

        }

        public AccountDetailsViewModel(DominatorAccountModel dataContext)
        {
            DominatorAccountModel = dataContext;

            // Take backup of current DominatorAccountModel object
            UpdateOldDominatorAccountModel();

            SaveCommand = new BaseCommand<object>(SaveCanExecute, SaveExecute);
            CancelCommand = new BaseCommand<object>(CancelCanExecute, CancelExecute);
            AddNewCookiesCommand = new BaseCommand<object>(AddNewCookiesCanExecute, AddNewCookiesExecute);
            RemoveCookiesCommand = new BaseCommand<object>(RemoveCookiesCanExecute, RemoveCookiesExecute);
            VerifyAccountCommand = new BaseCommand<object>(VerifyAccountCanExecute, VerifyAccountExecute);
            SendVerificationCodeCommand = new BaseCommand<object>(SendVerificationCodeCanExecute, SendVerificationCodeExecute);

        }

        #region Commands

        public ICommand SaveCommand { get; set; }
        public ICommand CancelCommand { get; set; }
        public ICommand AddNewCookiesCommand { get; set; }
        public ICommand RemoveCookiesCommand { get; set; }
        public ICommand VerifyAccountCommand { get; set; }
        public ICommand SendVerificationCodeCommand { get; set; }

        #endregion

        private bool SaveCanExecute(object arg) => true;
        private void SaveExecute(object sender)
        {
            DominatorAccountModel.CookieHelperList?.ToList().ForEach(cookie =>
            {
                if (string.IsNullOrEmpty(cookie.Name) || string.IsNullOrEmpty(cookie.Value))
                    DominatorAccountModel.CookieHelperList.Remove(cookie);
            });

            EditAccount();

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
            });

            #endregion
        }
      
        void EditAccount()
        {

            if (OldDominatorAccountModel == null) return;

            var newAccountBaseModel = DominatorAccountModel.AccountBaseModel;

            try
            {
                if (string.IsNullOrEmpty(newAccountBaseModel.UserName) ||
                           string.IsNullOrEmpty(newAccountBaseModel.Password)) return;


                if ((!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                     string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort))
                    || (string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                        !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort))) return;

                if ((!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyUsername) &&
                     string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPassword))
                    || (string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyUsername) &&
                        !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPassword))) return;


                if (OldDominatorAccountModel.AccountBaseModel.UserName != DominatorAccountModel.AccountBaseModel.UserName
                    || OldDominatorAccountModel.AccountBaseModel.Password != DominatorAccountModel.AccountBaseModel.Password
                    || OldDominatorAccountModel.UserAgentWeb != DominatorAccountModel.UserAgentWeb)
                {
                    if (ObjectComparer.Compare(OldDominatorAccountModel.CookieHelperList,
                        DominatorAccountModel.CookieHelperList))
                    {
                        DominatorAccountModel.CookieHelperList?.Clear();
                        DominatorAccountModel.HttpHelper.GetRequestParameter().Cookies = new CookieCollection();
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            var proxyManagerViewModel = ServiceLocator.Current.GetInstance<IProxyManagerViewModel>();
            var oldproxies = ProxyFileManager.GetAllProxy();

            #region If proxy not empty or null

            if (!string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyIp) &&
                !string.IsNullOrEmpty(newAccountBaseModel.AccountProxy.ProxyPort))
            {
                var oldAccount = AccountsFileManager.GetAccountById(OldDominatorAccountModel.AccountId).AccountBaseModel;

                if (!proxyManagerViewModel.IsProxyAvailable(newAccountBaseModel, oldproxies, oldAccount, strategy))
                {
                    if (!proxyManagerViewModel.UpdateProxy(newAccountBaseModel, strategy))
                        proxyManagerViewModel.AddProxyIfNotExist(newAccountBaseModel, strategy);
                }
            }

            #endregion

            else
            {
                proxyManagerViewModel.UpdateProxy(newAccountBaseModel, oldproxies, strategy);
                try
                {
                    new SocinatorAccountBuilder(newAccountBaseModel.AccountId)
                               .AddOrUpdateProxy(new Proxy())
                               .SaveToBinFile();
                }
                catch (OperationCanceledException)
                {
                    throw new System.OperationCanceledException();
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            e.DebugLog("Cancellation requested before task completion!");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                    }
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
                throw new System.OperationCanceledException();
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            #endregion

            GlobusLogHelper.log.Info(Log.AccountEdited, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName);
        }

        private bool CancelCanExecute(object arg) => true;
        private void CancelExecute(object sender)
        {
            // Update current DominatorAccountModel
            UpdateCurrentDominatorAccountModel();

            // Back to AccountManager module
            var controlToSelect = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = controlToSelect;

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
                            Application.Current.Dispatcher.Invoke(
                                () => VerificationSectionVisibility = Visibility.Collapsed
                            );

                        else
                        {
                            DominatorAccountModel.VarificationCode = string.Empty;
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    //  CodeSectionVisibility = Visibility.Collapsed;

                                }
                            );
                        }
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
        private void UpdateCurrentDominatorAccountModel()
        {
            DominatorAccountModel.AccountBaseModel.UserName = OldDominatorAccountModel.AccountBaseModel.UserName;
            DominatorAccountModel.AccountBaseModel.Password = OldDominatorAccountModel.AccountBaseModel.Password;
            DominatorAccountModel.AccountBaseModel.AccountId = OldDominatorAccountModel.AccountBaseModel.AccountId;
            DominatorAccountModel.AccountBaseModel.AccountNetwork = OldDominatorAccountModel.AccountBaseModel.AccountNetwork;
            DominatorAccountModel.AccountBaseModel.AccountGroup.Content = OldDominatorAccountModel.AccountBaseModel.AccountGroup.Content;
            DominatorAccountModel.AccountBaseModel.AccountProxy = OldDominatorAccountModel.AccountBaseModel.AccountProxy;

            DominatorAccountModel.MailCredentials = OldDominatorAccountModel.MailCredentials;
            DominatorAccountModel.UserAgentWeb = OldDominatorAccountModel.UserAgentWeb;

            if (ObjectComparer.Compare(DominatorAccountModel.CookieHelperList, OldDominatorAccountModel.CookieHelperList))
            {
                DominatorAccountModel.CookieHelperList = OldDominatorAccountModel.CookieHelperList;
            }
        }
    }
}
