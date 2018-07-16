using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddUpdateAccountControl.xaml
    /// </summary>
    public partial class AccountDetail : UserControl
    {



        public DominatorAccountModel DominatorAccountModel { get; set; }
        public DominatorAccountModel OldDominatorAccountModel { get; set; }


        /// <summary>
        /// Constructor with default data context
        /// </summary>
        public AccountDetail()
        {
            InitializeComponent();
        }

        public AccountDetail(DominatorAccountModel dataContext) : this()
        {
            DominatorAccountModel = dataContext;
            this.DataContext = DominatorAccountModel;
            OldDominatorAccountModel = new DominatorAccountModel();
            OldDominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel
            {
                UserName = DominatorAccountModel.AccountBaseModel.UserName,
                Password = DominatorAccountModel.AccountBaseModel.Password,
            };
            OldDominatorAccountModel.UserAgentWeb = DominatorAccountModel.UserAgentWeb;
            OldDominatorAccountModel.CookieHelperList = DominatorAccountModel.CookieHelperList;
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            var controlToselect = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
            var lstAccount = controlToselect.DominatorAccountViewModel.LstDominatorAccountModel;
            var indexOfAccount = lstAccount.IndexOf(lstAccount.FirstOrDefault(x => x.AccountId == DominatorAccountModel.AccountId));

            lstAccount[indexOfAccount].AccountBaseModel.UserName = OldDominatorAccountModel.UserName;
            lstAccount[indexOfAccount].AccountBaseModel.Password = OldDominatorAccountModel.AccountBaseModel.Password;
            lstAccount[indexOfAccount].UserAgentWeb = OldDominatorAccountModel.UserAgentWeb;
            lstAccount[indexOfAccount].CookieHelperList = OldDominatorAccountModel.CookieHelperList;

            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = controlToselect;

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

            var socinatorAccountBuilder = new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId);
            DominatorAccountModel.CookieHelperList.ToList().ForEach(cookie =>
                {
                    if (string.IsNullOrEmpty(cookie.Name) || string.IsNullOrEmpty(cookie.Value))
                        DominatorAccountModel.CookieHelperList.Remove(cookie);
                });

            socinatorAccountBuilder.AddOrUpdateDominatorAccountBase(DominatorAccountModel.AccountBaseModel)
                                   .AddOrUpdateCookies(DominatorAccountModel.Cookies)
                                   .AddOrUpdateUserAgentWeb(DominatorAccountModel.UserAgentWeb)
                                   .SaveToBinFile();


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
                            DominatorAccountModel.CookieHelperList.Clear();
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

                        socinatorAccountBuilder.AddOrUpdateDisplayColumn1(DominatorAccountModel.DisplayColumnValue1)
                                               .AddOrUpdateDisplayColumn2(DominatorAccountModel.DisplayColumnValue2)
                                               .AddOrUpdateDisplayColumn3(DominatorAccountModel.DisplayColumnValue3)
                                               .AddOrUpdateDisplayColumn4(DominatorAccountModel.DisplayColumnValue4)
                                               .SaveToBinFile();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
                OldDominatorAccountModel.AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = DominatorAccountModel.AccountBaseModel.UserName,
                    Password = DominatorAccountModel.AccountBaseModel.Password,
                };
                OldDominatorAccountModel.UserAgentWeb = DominatorAccountModel.UserAgentWeb;
                OldDominatorAccountModel.CookieHelperList = DominatorAccountModel.CookieHelperList;
            });


            #endregion

            GlobusLogHelper.log.Info(Log.AccountEdited, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.UserName);
            //   AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);
        }



        private void BtnAddNewCookies_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel.CookieHelperList.Add(new CookieHelper
                {
                    Name = string.Empty,
                    Value = string.Empty

                });
                Cookies.ItemsSource = null;
                Cookies.ItemsSource = DominatorAccountModel.CookieHelperList;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void BtnRemoveNewCookies_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
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


        private void BtnVerifyAccount_OnClick(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory = SocinatorInitialize
                .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory();

            if (!string.IsNullOrEmpty(DominatorAccountModel.VarificationCode))
            {
                var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
                var verificationType = ChkEmailVerification.IsChecked == true ? VerificationType.Email : VerificationType.Phone;
                Task.Factory.StartNew(() =>
                {
                    if (accountVerificationFactory.VerifyAccountAsync(DominatorAccountModel, verificationType,
                        DominatorAccountModel.Token).Result)
                        Application.Current.Dispatcher.Invoke(
                            () => verificationSection.Visibility = Visibility.Collapsed
                        );

                    else
                        Application.Current.Dispatcher.Invoke(
                            () => verificationSection.Visibility = Visibility.Visible
                        );
                });

            }
        }

        private void BtnSendVerificationCode_OnClick(object sender, RoutedEventArgs e)
        {
            var networkCoreFactory = SocinatorInitialize
                .GetSocialLibrary(DominatorAccountModel.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory();

            var accountVerificationFactory = networkCoreFactory.AccountVerificationFactory;
            var verificationType = ChkEmailVerification.IsChecked == true ? VerificationType.Email : VerificationType.Phone;
            Task.Factory.StartNew(() =>
            {
                if (accountVerificationFactory
                .SendVerificationCode(DominatorAccountModel, verificationType, DominatorAccountModel.Token).Result)
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            CodeSection.Visibility = Visibility.Visible;
                            GlobusLogHelper.log.Info(Log.SentVerificationCode, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);
                        });
                else
                    Application.Current.Dispatcher.Invoke(
                        () =>
                        {
                            CodeSection.Visibility = Visibility.Collapsed;
                            GlobusLogHelper.log.Info(Log.FailedToSendVerificationCodeFaild, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, verificationType);

                        });

            });
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.IsDefault = true;
            }
        }
        private void OnVerificationKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnVerifyAccount.IsDefault = true;
            }
        }

    }
}
