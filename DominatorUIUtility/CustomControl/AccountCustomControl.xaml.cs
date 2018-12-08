using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using DominatorUIUtility.Views;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AccountCustomControl.xaml
    /// </summary>
    public partial class AccountCustomControl : UserControl, INotifyPropertyChanged
    {
        private DominatorAccountViewModel _dominatorAccountViewModel;

        #region Property

        public DominatorAccountViewModel DominatorAccountViewModel
        {
            get
            {
                return _dominatorAccountViewModel;
            }
            set
            {
                _dominatorAccountViewModel = value;
                OnPropertyChanged(nameof(DominatorAccountViewModel));
            }
        }

        #endregion

        private AccountCustomControl()
        {
            _dominatorAccountViewModel = (DominatorAccountViewModel)ServiceLocator.Current.GetInstance<IDominatorAccountViewModel>();
            InitializeComponent();
            AccountModule.DataContext = DominatorAccountViewModel;
        }

        private static AccountCustomControl _accountCustomInstance;

        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNetworks, AccessorStrategies strategies)
        {
            if (_accountCustomInstance == null)
                _accountCustomInstance = new AccountCustomControl();

            _accountCustomInstance.GetRespectiveAccounts(socialNetworks);
            return _accountCustomInstance;
        }


        public static AccountCustomControl GetAccountCustomControl(AccessorStrategies strategies)
        {
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }
        public static AccountCustomControl GetAccountCustomControl(SocialNetworks socialNework)
        {
            return _accountCustomInstance ?? (_accountCustomInstance = new AccountCustomControl());
        }

        public void GetRespectiveAccounts(SocialNetworks socialNetworks)
        {
            DominatorAccountViewModel.LstDominatorAccountModel.ForEach(x =>
            {
                x.IsAccountManagerAccountSelected = false;
            });
        }

        private void CopyAccountId(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            if (!string.IsNullOrEmpty(dataContext.AccountId))
            {
                Clipboard.SetText(dataContext.AccountId);
                ToasterNotification.ShowSuccess("AccountId copied");
            }

        }

        private void ProfileDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                AccountManager.GetSingletonAccountManager(String.Empty, dataContext, dataContext.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteAccount(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null)
                DominatorAccountViewModel.DeleteAccountByContextMenu(dataContext);

        }

        public void GotoTools(object sender, RoutedEventArgs e)
        {
            var dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dominatorAccountModel == null)
                return;

            TabSwitcher.ChangeTabWithNetwork(3, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);
        }

        public void BrowserLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.AccountBrowserLogin(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }



        public void CheckinStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void UpdateFriendshipCount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionUpdateAccount(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void EditNetworkProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

                DominatorAccountViewModel.EditProfile(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void InstaCheckAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.ActionCheckAccount(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void FacebookRemovePhoneVerification(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountViewModel.RemovePhoneVerification(dominatorAccountModel);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
