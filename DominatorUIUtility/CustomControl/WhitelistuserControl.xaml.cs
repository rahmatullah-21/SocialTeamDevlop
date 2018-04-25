using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for WhitelistuserControl.xaml
    /// </summary>
    public partial class WhitelistuserControl : UserControl
    {
        private DataBaseConnectionGlobal DataBaseConnectionGlb { get; set; }

        private bool IsUnCheckedFromUser { get; set; }
        WhitelistUserModel WhitelistUserModel { get; set; } = new WhitelistUserModel();
        public WhitelistuserControl()
        {
            InitializeComponent();
            DataBaseConnectionGlb = DataBaseHandler.GetDataBaseConnectionGlobalInstance();
            DataBaseConnectionGlb.Get<BlackWhiteListUser>()?.Where(x=>
                x.Network==SocinatorInitialize.ActiveSocialNetwork.ToString()&&x.CategoryType==UserType.WhiteListedUser.ToString()).ForEach(user =>
            {
                WhitelistUserModel.LstWhiteListUsers.Add(new WhitelistUserModel
                {
                    WhitelistUser = user.UserName
                });
            });
            MainGrid.DataContext = WhitelistUserModel;
            WhitelistUserModel.LstWhiteListUsers.CollectionChanged += UpdateWhiteListUsers;
        }

        private void UpdateWhiteListUsers(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!WhitelistUserModel.LstWhiteListUsers.Any(x => x.IsWhiteListUserChecked)
                || WhitelistUserModel.LstWhiteListUsers.Count == 0)
            {
                IsUnCheckedFromUser = true;
                if (!WhitelistUserModel.IsAllWhiteListUserChecked)
                    return;
                SelectAll.Unchecked -= SelectAll_OnUnchecked;
                WhitelistUserModel.IsAllWhiteListUserChecked = false;
                SelectAll.Unchecked += SelectAll_OnUnchecked;
                IsUnCheckedFromUser = false;
            }

        }

        private void RefreshWhitelistedUsers_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            Txtusername.Clear();
        }

        private void BtnAddtoWhitelist_OnClick(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Txtusername.Text.Trim()))
            {

                Txtusername.Text.Split('\n').ForEach(user =>
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (!WhitelistUserModel.LstWhiteListUsers.Any(x => x.WhitelistUser == userName))
                        {
                            WhitelistUserModel.LstWhiteListUsers.Add(
                                new WhitelistUserModel()
                                {
                                    WhitelistUser = userName
                                });
                            DataBaseConnectionGlb.Add<BlackWhiteListUser>(new BlackWhiteListUser()
                            {
                                UserName = userName,
                                CategoryType = UserType.WhiteListedUser.ToString(),
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

        private void ChkWhitlistuser_OnChecked(object sender, RoutedEventArgs e)
        {
            if (!WhitelistUserModel.IsAllWhiteListUserChecked)
            {
                if (WhitelistUserModel.LstWhiteListUsers.All(x => x.IsWhiteListUserChecked))
                {
                    SelectAll.Checked -= SelectAll_OnChecked;
                    WhitelistUserModel.IsAllWhiteListUserChecked = true;
                    SelectAll.Checked += SelectAll_OnChecked;

                }
            }
        }

        private void ChkWhitlistuser_OnUnchecked(object sender, RoutedEventArgs e)
        {
            if (WhitelistUserModel.IsAllWhiteListUserChecked)
            {
                if (WhitelistUserModel.LstWhiteListUsers.Any(x => !x.IsWhiteListUserChecked))
                {
                    IsUnCheckedFromUser = true;
                    if (!WhitelistUserModel.IsAllWhiteListUserChecked)
                        return;
                    SelectAll.Unchecked -= SelectAll_OnUnchecked;
                    WhitelistUserModel.IsAllWhiteListUserChecked = false;
                    SelectAll.Unchecked += SelectAll_OnUnchecked;
                    IsUnCheckedFromUser = false;
                }
            }
        }

        private void DeletedSelected_OnClick(object sender, RoutedEventArgs e)
        {
            var selectedUser = WhitelistUserModel.LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                    "Please select atleast on user");
                return;
            }
            selectedUser.ForEach(x =>
            {
                WhitelistUserModel.LstWhiteListUsers.Remove(x);
                DataBaseConnectionGlb.Remove<BlackWhiteListUser>(user => 
                    user.Network == SocinatorInitialize.ActiveSocialNetwork.ToString() && user.UserName == x.WhitelistUser);
            });
        }
        private void CheckUncheckAll(bool isChecked)
        {
            WhitelistUserModel.LstWhiteListUsers.Select(x =>
            {
                x.IsWhiteListUserChecked = isChecked;
                return x;
            }).ToList();
        }
    }
}
