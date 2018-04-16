using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for BlacklistUserControl.xaml
    /// </summary>
    public partial class BlacklistUserControl : UserControl
    {
        BlacklistUserModel BlacklistUserModel = new BlacklistUserModel();
        public BlacklistUserControl()
        {
            InitializeComponent();
            MainGrid.DataContext = BlacklistUserModel;
        }

        private void RefreshBlacklistedUsers_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Txtusername.Clear();
        }

        private void BtnAddtoBlacklist_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtusername.Text.Trim()))
            {
                Txtusername.Text.Split('\n').ForEach(user =>
                   {

                       if (!BlacklistUserModel.LstBlackListUsers.Any(x => x.BlacklistUser == user.Trim()))
                       {
                           BlacklistUserModel.LstBlackListUsers.Add(
                                  new BlacklistUserModel()
                                  {
                                      BlacklistUser = user.Trim()
                                  });
                       }
                       else
                           GlobusLogHelper.log.Info($"{user.Trim()} already added to Blacklist");

                   });
            }
            Txtusername.Clear();
        }

        private void SelectAll_OnChecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(true);
        }

        private void SelectAll_OnUnchecked(object sender, RoutedEventArgs e)
        {
            CheckUncheckAll(false);
        }
        private void CheckUncheckAll(bool isChecked)
        {
            BlacklistUserModel.LstBlackListUsers.Select(x =>
            {
                x.IsBlackListUserChecked = isChecked;
                return x;
            }).ToList();
        }

        private void DeletedSelected_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedUser = BlacklistUserModel.LstBlackListUsers.Where(x => x.IsBlackListUserChecked).ToList();
            selectedUser.ForEach(x =>
            {
                BlacklistUserModel.LstBlackListUsers.Remove(x);
            });

        }

        private void ChkBlacklistuser_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!BlacklistUserModel.IsAllBlackListUserChecked)
            {
                if (BlacklistUserModel.LstBlackListUsers.All(x => x.IsBlackListUserChecked))
                {
                    SelectAll.Checked -= SelectAll_OnChecked;
                    BlacklistUserModel.IsAllBlackListUserChecked = true;
                    SelectAll.Checked += SelectAll_OnChecked;

                } 
            }
        }

        private void ChkBlacklistuser_OnUnchecked(object sender, RoutedEventArgs e)
        {

            if (!BlacklistUserModel.IsAllBlackListUserChecked)
            {
                if (BlacklistUserModel.LstBlackListUsers.Any(x => !x.IsBlackListUserChecked))
                {
                    SelectAll.Checked -= SelectAll_OnUnchecked;
                    BlacklistUserModel.IsAllBlackListUserChecked = false;
                    SelectAll.Checked += SelectAll_OnUnchecked;

                } 
            }
        }
    }
}
