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
                SetProperty(ref _isEmailVerification, value);
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


        public AccountDetailsViewModel()
        {

        }

        public AccountDetailsViewModel(DominatorAccountModel dataContext)
        {
            DominatorAccountModel = dataContext;

            #region Backup of current account

            OldDominatorAccountModel = new DominatorAccountModel();
            OldDominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup = new ContentSelectGroup
                {
                    Content = DominatorAccountModel.AccountBaseModel.AccountGroup.Content,
                },
                UserName = DominatorAccountModel.AccountBaseModel.UserName,
                Password = DominatorAccountModel.AccountBaseModel.Password,
                AccountId = DominatorAccountModel.AccountId,
                AccountProxy =  {
                    ProxyIp = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword
                }

            };
            OldDominatorAccountModel.UserAgentWeb = DominatorAccountModel.UserAgentWeb;
            OldDominatorAccountModel.CookieHelperList = DominatorAccountModel.CookieHelperList;
            OldDominatorAccountModel.AccountId = DominatorAccountModel.AccountId;

            #endregion

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
                    if (OldDominatorAccountModel.AccountBaseModel.UserName != DominatorAccountModel.AccountBaseModel.UserName
                        || OldDominatorAccountModel.AccountBaseModel.Password != DominatorAccountModel.AccountBaseModel.Password
                        || OldDominatorAccountModel.UserAgentWeb != DominatorAccountModel.UserAgentWeb)
                    {
                        if (ObjectComparer.Compare(OldDominatorAccountModel.CookieHelperList, DominatorAccountModel.CookieHelperList))
                            DominatorAccountModel.CookieHelperList?.Clear();
                    }

                    await asyncAccount.CheckStatusAsync(DominatorAccountModel, DominatorAccountModel.Token);


                    if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                        await asyncAccount.UpdateDetailsAsync(DominatorAccountModel, DominatorAccountModel.Token);
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
                OldDominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel
                {
                    AccountGroup = new ContentSelectGroup
                    {
                        Content = DominatorAccountModel.AccountBaseModel.AccountGroup.Content,
                    },
                    UserName = DominatorAccountModel.AccountBaseModel.UserName,
                    Password = DominatorAccountModel.AccountBaseModel.Password,
                    AccountId = DominatorAccountModel.AccountId,
                    AccountProxy =  {
                        ProxyIp = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp,
                        ProxyPort = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort,
                        ProxyUsername = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername,
                        ProxyPassword = DominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword
                    }
                };
                OldDominatorAccountModel.UserAgentWeb = DominatorAccountModel.UserAgentWeb;
                OldDominatorAccountModel.CookieHelperList = DominatorAccountModel.CookieHelperList;

            });


            #endregion


            //   AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);

        }
        private bool SaveCanExecute(object arg) => true;
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

                OldDominatorAccountModel.AccountBaseModel.AccountGroup.Content = newAccountBaseModel.AccountGroup.Content;
                if (OldDominatorAccountModel.AccountBaseModel.UserName != newAccountBaseModel.UserName || OldDominatorAccountModel.AccountBaseModel.Password != newAccountBaseModel.Password)
                {
                    OldDominatorAccountModel.Cookies = new CookieCollection();
                }
                OldDominatorAccountModel.AccountBaseModel.AccountGroup = new ContentSelectGroup
                {
                    Content = newAccountBaseModel.AccountGroup.Content,
                };
                OldDominatorAccountModel.AccountBaseModel.UserName = newAccountBaseModel.UserName;
                OldDominatorAccountModel.AccountBaseModel.Password = newAccountBaseModel.Password;
                OldDominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp = newAccountBaseModel.AccountProxy.ProxyIp;
                OldDominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort = newAccountBaseModel.AccountProxy.ProxyPort;
                OldDominatorAccountModel.AccountBaseModel.AccountProxy.ProxyUsername = newAccountBaseModel.AccountProxy.ProxyUsername;
                OldDominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPassword = newAccountBaseModel.AccountProxy.ProxyPassword;
                OldDominatorAccountModel.AccountBaseModel.AccountNetwork = newAccountBaseModel.AccountNetwork;


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
                    if (!proxyManagerViewModel.UpdateProxy(OldDominatorAccountModel.AccountBaseModel, strategy))
                        proxyManagerViewModel.AddProxyIfNotExist(OldDominatorAccountModel.AccountBaseModel, strategy);
                }

                try
                {
                    OldDominatorAccountModel.Token.ThrowIfCancellationRequested();

                    new SocinatorAccountBuilder(newAccountBaseModel.AccountId)
                        .AddOrUpdateDominatorAccountBase(newAccountBaseModel)
                        .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                        .AddOrUpdateUserAgentWeb(DominatorAccountModel.UserAgentWeb)
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
            GlobusLogHelper.log.Info(Log.AccountEdited, newAccountBaseModel.AccountNetwork, newAccountBaseModel.UserName);

        }
        private bool CancelCanExecute(object arg) => true;
        private void CancelExecute(object sender)
        {
            var controlToselect = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
            var lstAccount = controlToselect.DominatorAccountViewModel.LstDominatorAccountModel;
            var indexOfAccount = lstAccount.IndexOf(lstAccount.FirstOrDefault(x => x.AccountId == DominatorAccountModel.AccountId));

            lstAccount[indexOfAccount].AccountBaseModel.AccountGroup = OldDominatorAccountModel.AccountBaseModel.AccountGroup;
            lstAccount[indexOfAccount].AccountBaseModel.UserName = OldDominatorAccountModel.UserName;
            lstAccount[indexOfAccount].AccountBaseModel.Password = OldDominatorAccountModel.AccountBaseModel.Password;
            lstAccount[indexOfAccount].UserAgentWeb = OldDominatorAccountModel.UserAgentWeb;
            lstAccount[indexOfAccount].CookieHelperList = OldDominatorAccountModel.CookieHelperList;

            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = controlToselect;

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
            var button = (Button)sender;
           
            try
            {
                button.Visibility = Visibility.Collapsed;
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
                                    CodeSectionVisibility = Visibility.Visible;
                                    // GlobusLogHelper.log.Info(Log.SentVerificationCode, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);
                                });
                        else
                            Application.Current.Dispatcher.Invoke(
                                () =>
                                {
                                    button.Visibility = Visibility.Visible;
                                    CodeSectionVisibility = Visibility.Collapsed;
                                    // GlobusLogHelper.log.Info(Log.FailedToSendVerificationCodeFaild, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);

                                });
                    }
                    

                });
            }
            catch (Exception ex)
            {
                button.Visibility = Visibility.Visible;
                ex.DebugLog();
            }
        }
    }
}
