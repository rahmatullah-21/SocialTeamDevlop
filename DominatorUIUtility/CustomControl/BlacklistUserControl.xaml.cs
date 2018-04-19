using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.DHEnum;
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
        private DataBaseConnectionGlobal DataBaseConnectionGlb { get; set; }

        private bool IsUnCheckedFromUser { get; set; }
        BlacklistUserModel BlacklistUserModel { get; set; } = new BlacklistUserModel();
        public BlacklistUserControl()
        {
            InitializeComponent();
            DataBaseConnectionGlb = DataBaseHandler.GetDataBaseConnectionGlobalInstance("Global");
            DataBaseConnectionGlb.Get<BlackWhiteListUser>()?.Where(
                x => x.Network == SocinatorInitialize.ActiveSocialNetwork.ToString() && x.CategoryType == UserType.BlackListedUser.ToString()).ForEach(user =>
                {
                    BlacklistUserModel.LstBlackListUsers.Add(new BlacklistUserModel
                    {
                        BlacklistUser = user.UserName
                    });
                });
            MainGrid.DataContext = BlacklistUserModel;
            BlacklistUserModel.LstBlackListUsers.CollectionChanged += UpdateBlackListUsers;
        }

        private void UpdateBlackListUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!BlacklistUserModel.LstBlackListUsers.Any(x => x.IsBlackListUserChecked)
                || BlacklistUserModel.LstBlackListUsers.Count == 0)
            {
                IsUnCheckedFromUser = true;
                if (!BlacklistUserModel.IsAllBlackListUserChecked)
                    return;
                SelectAll.Unchecked -= SelectAll_OnUnchecked;
                BlacklistUserModel.IsAllBlackListUserChecked = false;
                SelectAll.Unchecked += SelectAll_OnUnchecked;
                IsUnCheckedFromUser = false;
            }


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
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (!BlacklistUserModel.LstBlackListUsers.Any(x => x.BlacklistUser == userName))
                        {
                            BlacklistUserModel.LstBlackListUsers.Add(
                                new BlacklistUserModel()
                                {
                                    BlacklistUser = userName
                                });
                            DataBaseConnectionGlb.Add<BlackWhiteListUser>(new BlackWhiteListUser()
                            {
                                UserName = userName,
                                CategoryType = UserType.BlackListedUser.ToString(),
                                AddedDateTime = DateTime.Now,
                                Network = SocinatorInitialize.ActiveSocialNetwork.ToString()
                            });
                        }
                        else
                            GlobusLogHelper.log.Info($"{userName} already added to Blacklist");
                    }
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
            if (IsUnCheckedFromUser)
                return;

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
            if (selectedUser.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                    "Please select atleast on user");
                return;
            }
            selectedUser.ForEach(x =>
            {
                BlacklistUserModel.LstBlackListUsers.Remove(x);
                DataBaseConnectionGlb.Remove<BlackWhiteListUser>(user =>
                    user.Network == SocinatorInitialize.ActiveSocialNetwork.ToString() && user.UserName == x.BlacklistUser);
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

            if (BlacklistUserModel.IsAllBlackListUserChecked)
            {
                if (BlacklistUserModel.LstBlackListUsers.Any(x => !x.IsBlackListUserChecked))
                {
                    IsUnCheckedFromUser = true;
                    if (!BlacklistUserModel.IsAllBlackListUserChecked)
                        return;
                    SelectAll.Unchecked -= SelectAll_OnUnchecked;
                    BlacklistUserModel.IsAllBlackListUserChecked = false;
                    SelectAll.Unchecked += SelectAll_OnUnchecked;
                    IsUnCheckedFromUser = false;
                }

            }
        }
    }
}
