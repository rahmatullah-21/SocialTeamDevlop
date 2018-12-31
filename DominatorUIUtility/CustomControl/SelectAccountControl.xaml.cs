using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectAccountControl.xaml
    /// </summary>
    public partial class SelectAccountControl : UserControl, INotifyPropertyChanged
    {
        private bool IsUnCheckedFromAccountDetails { get; set; }

        private SelectAccountViewModel _objAccountViewModel = new SelectAccountViewModel();

        public SelectAccountViewModel ObjAccountViewModel
        {
            get { return _objAccountViewModel; }
            set
            {
                if (_objAccountViewModel == value)
                    return;
                _objAccountViewModel = value;
                OnPropertyChanged(nameof(ObjAccountViewModel));
            }
        }


        public SelectAccountControl(ICollection<string> lstSelectedAccount, bool filterForActiveSocialNetwork = true)
        {
            InitializeComponent();

            DataContext = ObjAccountViewModel;

            var accountList = ServiceLocator.Current.GetInstance<IDominatorAccountViewModel>().LstDominatorAccountModel;
            var savedAccounts = accountList.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork);

            ObjAccountViewModel.LstSelectAccount.Clear();
            savedAccounts.ForEach(x =>
            {

                //add only account status should be success

                if (x.AccountBaseModel.Status == AccountStatus.Success)
                {
                    ObjAccountViewModel.LstSelectAccount.Add(new SelectAccountModel
                    {
                        UserName = x.UserName,
                        GroupName = x.AccountBaseModel.AccountGroup.Content
                    });
                    if (ObjAccountViewModel.SelectAccountModel.Groups.Any(group => group.Content == x.AccountBaseModel.AccountGroup.Content) == false)
                        ObjAccountViewModel.SelectAccountModel.Groups.Add(
                            new ContentSelectGroup
                            {
                                Content = x.AccountBaseModel.AccountGroup.Content
                            });
                }
            });

            //Select the account which is already selected
            ObjAccountViewModel.LstSelectAccount?.ToList().ForEach(x =>
            {
                x.IsAccountSelected = lstSelectedAccount.Contains(x.UserName);
            });

            //Assign the view to ICollectionView         
            ObjAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(ObjAccountViewModel.LstSelectAccount);
        }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            ObjAccountViewModel.SelectDeselectAccountByGroup(true);
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {

            if (IsUnCheckedFromAccountDetails)
                return;

            ObjAccountViewModel.SelectDeselectAccountByGroup(false);
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void cmbAllGroups_DropDownClosed(object sender, EventArgs e)
        {
            ObjAccountViewModel.AccountGroupSelected();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            FilterAccount();
        }

        private void FilterAccount()
        {
            try
            {
                switch (cmbSearchFilter.SelectedIndex)
                {
                    case 0:
                        ObjAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                        break;
                    case 1:
                        ObjAccountViewModel.AccountCollectionView.Filter = FilterByAccounts;
                        break;
                    default:
                        if (!string.IsNullOrEmpty(txtSearch.Text))
                            ObjAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                        else
                            ObjAccountViewModel.AccountCollectionView.Filter = null;
                        break;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool FilterByGroupName(object groupName)
        {
            try
            {
                var objAccountViewModel = groupName as SelectAccountViewModel;

                return objAccountViewModel != null &&
                       objAccountViewModel.SelectAccountModel.GroupName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }

        private bool FilterByAccounts(object accountName)
        {
            try
            {
                var objAccountViewModel = accountName as SelectAccountViewModel;
                return objAccountViewModel != null &&
                       objAccountViewModel.SelectAccountModel.UserName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        /// <summary>
        ///     SelectDeselectAllAccount method take a boolean value.
        ///     pass true if you want to select all account.
        ///     pass false if you want to deselect all account.
        /// </summary>
        /// <param name="isChecked"></param>
        private void SelectDeselectAllAccount(bool isChecked)
        {
            try
            {
                ObjAccountViewModel.LstSelectAccount.Select(x =>
                {
                    x.IsAccountSelected = isChecked;
                    return x;
                }).ToList();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public ObservableCollectionBase<string> GetSelectedAccount()
        {
            return new ObservableCollectionBase<string>(ObjAccountViewModel.LstSelectAccount
                .Where(x => x.IsAccountSelected).Select(x => x.UserName).ToList());
        }

        private void Filter(object sender, SelectionChangedEventArgs e)
        {
            FilterAccount();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}