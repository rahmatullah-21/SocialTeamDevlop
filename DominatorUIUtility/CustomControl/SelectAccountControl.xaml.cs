using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for SelectAccountControl.xaml
    /// </summary>
    public partial class SelectAccountControl : UserControl
    {
        public DominatorAccountViewModel objDominatorAccountViewModel = new DominatorAccountViewModel();
        public SelectAccountControl(List<string> lstSelectedAccount, bool filterForActiveSocialNetwrok = true)
        {
            InitializeComponent();

            this.DataContext = objDominatorAccountViewModel;

            //Read all accounts from bin files

            var items = filterForActiveSocialNetwrok                                               ?
                        AccountsFileManager.GetAll(DominatorHouseInitializer.ActiveSocialNetwork)  :
                        AccountsFileManager.GetAll();

            //Iterate all account model from bin file and add to DominatorAccountViewModel object
            objDominatorAccountViewModel.LstDominatorAccountModel.Clear();
            items.ForEach(x => objDominatorAccountViewModel.LstDominatorAccountModel.Add(x));

            
            objDominatorAccountViewModel.LstDominatorAccountModel.ForEach(x => 
            {
                if (objDominatorAccountViewModel.Groups.Any(group => group.Content == x.AccountBaseModel.AccountGroup.Content) == false)
                    objDominatorAccountViewModel.Groups.Add(x.AccountBaseModel.AccountGroup);
            });
               

            //Select the account which is already selected
            objDominatorAccountViewModel.LstDominatorAccountModel?.ToList().ForEach(x =>
            {
                x.IsAccountSelected = lstSelectedAccount.Contains(x.AccountBaseModel.UserName);
            });

            //Assign the view to ICollectionView         
            objDominatorAccountViewModel.AccountCollectionView = CollectionViewSource.GetDefaultView(objDominatorAccountViewModel.LstDominatorAccountModel.ToList());
            
        }
      
        private void chkgroup_Checked(object sender, RoutedEventArgs e)
        {
                SelectDeselectAccountByGroup(true);
                AccountGroupSelected();
        }
        private void chkgroup_Unchecked(object sender, RoutedEventArgs e)
        {
                SelectDeselectAccountByGroup(false);
                AccountGroupSelected();
        }
        private void chkSelectAllAccount_Checked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAllAccount(true);
        }
        private void chkSelectAllAccount_Unchecked(object sender, RoutedEventArgs e)
        {
            SelectDeselectAllAccount(false);
            objDominatorAccountViewModel.Groups.Select(group => { group.IsContentSelected = false; return group; }).ToList();
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
                    objDominatorAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                    break;
                case 1:
                    objDominatorAccountViewModel.AccountCollectionView.Filter = FilterByAccounts;
                    break;
                default:
                    objDominatorAccountViewModel.AccountCollectionView.Filter = FilterByGroupName;
                    break;
            }

        }
        private bool FilterByGroupName(object groupName)
        {
            try
            {
                DominatorAccountModel objDominatorAccountModel = groupName as DominatorAccountModel;
                return objDominatorAccountModel.AccountBaseModel.AccountGroup.Content.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }

            return false;
        }
        private bool FilterByAccounts(object AccountName)
        {
            try
            {
                DominatorAccountModel objDominatorAccountModel = AccountName as DominatorAccountModel;
                return objDominatorAccountModel.AccountBaseModel.UserName.IndexOf(txtSearch.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception ex)
            {
            }
            return false;
        }

        /// <summary>
        /// SelectDeselectAccountByGroup method take a boolean value.
        /// pass true if you want to select account by selected groups.
        /// pass false if you want to deselect account by selected groups.
        /// </summary>
        /// <param name="IsChecked"></param>
        private void SelectDeselectAccountByGroup(bool IsChecked)
        {
            try
            {
                var checkedGroup = objDominatorAccountViewModel.Groups.Where(group => group.IsContentSelected == IsChecked);
                objDominatorAccountViewModel.LstDominatorAccountModel.ForEach(account =>
                {
                    checkedGroup.ForEach(group =>
                    {
                        if (account.AccountBaseModel.AccountGroup.Content == group.Content)
                            account.IsAccountSelected = IsChecked;
                    });

                });
                if (objDominatorAccountViewModel.LstDominatorAccountModel.All(account => account.IsAccountSelected == true))
                    chkSelectAllAccount.IsChecked = true;
            }
            catch (Exception ex)
            {

            }
        }

        /// <summary>
        /// SelectDeselectAllAccount method take a boolean value.
        /// pass true if you want to select all account.
        /// pass false if you want to deselect all account.
        /// </summary>
        /// <param name="IsChecked"></param>
        private void SelectDeselectAllAccount(bool IsChecked)
        {
            try
            {
                objDominatorAccountViewModel.LstDominatorAccountModel.Select(x => { x.IsAccountSelected = IsChecked; return x; }).ToList();
            }
            catch (Exception ex)
            {
            }
        }
        private void AccountGroupSelected()
        {
            try
            {
                int selectedGroups = objDominatorAccountViewModel.Groups.Count(x => x.IsContentSelected == true);
                cmbAllGroups.Text = $"{selectedGroups} {FindResource("langGroupSelected").ToString()}";
            }
            catch (Exception ex)
            {
            }
        }
        public ObservableCollectionBase<string> GetSelectedAccount()
        {
            return new ObservableCollectionBase<string>(objDominatorAccountViewModel.LstDominatorAccountModel.Where(x => x.IsAccountSelected == true).Select(x => x.AccountBaseModel.UserName).ToList());
        }
        private void lstviewAccounts_SizeChanged(object sender, SizeChangedEventArgs e)
        {
          //  GridViewColumns.SetGridViewColumnsWidthToStartWidth(lstviewAccounts, e);
        }
    }
}
