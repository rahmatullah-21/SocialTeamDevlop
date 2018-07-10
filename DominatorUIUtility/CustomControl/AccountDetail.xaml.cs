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
        public DominatorAccountModel OldDominatorAccountModel { get;}
      

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
            OldDominatorAccountModel = DominatorAccountModel.Clone();
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            DominatorAccountModel = OldDominatorAccountModel;
            AccountManagerViewModel.GetSingletonAccountManagerViewModel().SelectedUserControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social);

        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

            var socinatorAccountBuilder = new SocinatorAccountBuilder(DominatorAccountModel.AccountBaseModel.AccountId);

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

                accountVerificationFactory.VerifyAccountAsync(DominatorAccountModel, DominatorAccountModel.Token);
            }
        }
    }
}
