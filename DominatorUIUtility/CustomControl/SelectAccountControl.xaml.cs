using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    ///     Interaction logic for SelectAccountControl.xaml
    /// </summary>
    public partial class SelectAccountControl : UserControl
    {
        private readonly SelectAccountViewModel _objAccountViewModel = new SelectAccountViewModel();

        public SelectAccountControl(ICollection<string> lstSelectedAccount, bool filterForActiveSocialNetwork = true)
        {
            InitializeComponent();

            DataContext = _objAccountViewModel;

            var accountModels = filterForActiveSocialNetwork
                ? AccountsFileManager.GetAll(SocinatorInitialize.ActiveSocialNetwork)
                : AccountsFileManager.GetAll();
          
            _objAccountViewModel.LstSelectAccount.Clear();

            accountModels.ForEach(x =>
            {
                _objAccountViewModel.LstSelectAccount.Add(new SelectAccountViewModel
                {
                    UserName = x.UserName,
                    GroupName = x.AccountBaseModel.AccountGroup.Content
                });

                if (_objAccountViewModel.Groups.Any(group => group.Content == x.AccountBaseModel.AccountGroup.Content) == false)
                    _objAccountViewModel.Groups.Add(
                        new ContentSelectGroup
                        {
                            Content = x.AccountBaseModel.AccountGroup.Content
                        });
            });

            //Select the account which is already selected
            _objAccountViewModel.LstSelectAccount?.ToList().ForEach(x =>
            {
                x.IsAccountSelected = lstSelectedAccount.Contains(x.UserName);
            });

            //Assign the view to ICollectionView         
            _objAccountViewModel.AccountCollectionView =
                CollectionViewSource.GetDefaultView(_objAccountViewModel.LstSelectAccount.ToList());
        }

        private CheckBox GroupCheck { get; set; }

        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
            GroupCheck = sender as CheckBox;
            SelectDeselectAccountByGroup(true);
            AccountGroupSelected();
        }

        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAccountByGroup(false);
            AccountGroupSelected();
        }

        private void cmbAllGroups_DropDownClosed(object sender, EventArgs e)
        {
            AccountGroupSelected();
        }

        private void txtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            switch (cmbSearchFilter.SelectedIndex)
            {
                case 0:
                    _objAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                    break;
                case 1:
                    _objAccountViewModel.AccountCollectionView.Filter = FilterByAccounts;
                    break;
                default:
                    _objAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                    break;
            }
        }

        private bool FilterByGroupName(object groupName)
        {
            try
            {
                var objAccountViewModel = groupName as SelectAccountViewModel;

                return objAccountViewModel != null &&
                       objAccountViewModel.GroupName.IndexOf(txtSearch.Text,
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
                       objAccountViewModel.UserName.IndexOf(txtSearch.Text,
                           StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        /// <summary>
        ///     SelectDeselectAccountByGroup method take a boolean value.
        ///     pass true if you want to select account by selected groups.
        ///     pass false if you want to deselect account by selected groups.
        /// </summary>
        /// <param name="isChecked"></param>
        private void SelectDeselectAccountByGroup(bool isChecked)
        {
            try
            {
                var checkedGroup = _objAccountViewModel.Groups.Where(group => group.IsContentSelected == isChecked);
                _objAccountViewModel.LstSelectAccount.ForEach(account =>
                {
                    checkedGroup.ForEach(group =>
                    {
                        if (account.GroupName == group.Content)
                            account.IsAccountSelected = isChecked;
                    });
                });
                if (_objAccountViewModel.LstSelectAccount.All(account => account.IsAccountSelected == isChecked))
                    _objAccountViewModel.IsAllAccountSelected = isChecked;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
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
                _objAccountViewModel.LstSelectAccount.Select(x =>
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

        private void AccountGroupSelected()
        {
            try
            {
                var selectedGroups = _objAccountViewModel.Groups.Count(x => x.IsContentSelected);
                cmbAllGroups.Text = $"{selectedGroups} {FindResource("langGroupSelected").ToString()}";
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public ObservableCollectionBase<string> GetSelectedAccount()
        {
            return new ObservableCollectionBase<string>(_objAccountViewModel.LstSelectAccount
                .Where(x => x.IsAccountSelected).Select(x => x.UserName).ToList());
        }

        private void AllAccount_OnChecked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAllAccount(true);
        }

        private void AllAccount_OnUnchecked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAllAccount(false);
            _objAccountViewModel.Groups.Select(group =>
            {
                group.IsContentSelected = false;
                return group;
            }).ToList();
        }

        private void SingleAccount_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (!_objAccountViewModel.IsAllAccountSelected)
                return;
            if (GroupCheck!=null)
            {
                GroupCheck.Unchecked -= chkgroup_Unchecked;
                _objAccountViewModel.LstSelectAccount.ForEach(account =>
                {
                    if (!account.IsAccountSelected)
                        _objAccountViewModel.Groups.ForEach(group =>
                        {
                            if (account.GroupName == group.Content && group.IsContentSelected)
                                group.IsContentSelected = false;
                        });
                });
                AccountGroupSelected();
                GroupCheck.Unchecked += chkgroup_Unchecked; 
            }
            AllAccount.Unchecked -= AllAccount_OnUnchecked;
            _objAccountViewModel.IsAllAccountSelected = false;
            AllAccount.Unchecked += AllAccount_OnUnchecked;
        }
    }
}