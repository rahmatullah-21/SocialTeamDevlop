using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums.DHEnum;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel
{
    public interface IPrivateBlickListViewModel
    {

    }
    public class BlackListViewModel : BindableBase
    {
        public BlackListViewModel()
        {
            AddToBlackListCommand = new BaseCommand<object>((sender) => true, AddToBlackList);
            ClearCommand = new BaseCommand<object>((sender) => true, ClearUser);
            RefreshCommand = new BaseCommand<object>((sender) => true, Refresh);
            SelectCommand = new BaseCommand<object>((sender) => true, Select);
            DeleteCommand = new BaseCommand<object>((sender) => true, Delete);
            BindingOperations.EnableCollectionSynchronization(LstBlackListUsers, _lock);
        }


        private static object _lock = new object();
        public ICommand AddToBlackListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbOperations DbOperations { get; set; }

        private DbOperations WhiteListDbOperations { get; set; }

        private bool IsUnCheckedFromUser { get; set; }

        private PrivateBlacklistUserModel _privateBlacklistUserModel = new PrivateBlacklistUserModel();

        public PrivateBlacklistUserModel PrivateBlacklistUserModel
        {
            get
            {
                return _privateBlacklistUserModel;
            }
            set
            {
                if (_privateBlacklistUserModel == value)
                    return;
                _privateBlacklistUserModel = value;
                SetProperty(ref _privateBlacklistUserModel, value);
            }
        }
        private BlacklistUserModel _blacklistUserModel = new BlacklistUserModel();

        public BlacklistUserModel BlacklistUserModel
        {
            get
            {
                return _blacklistUserModel;
            }
            set
            {
                if (_blacklistUserModel == value)
                    return;
                _blacklistUserModel = value;
                SetProperty(ref _blacklistUserModel, value);
            }
        }
        private bool _isAllBlackListUserChecked;

        public bool IsAllBlackListUserChecked
        {
            get
            {
                return _isAllBlackListUserChecked;
            }
            set
            {
                if (value == _isAllBlackListUserChecked)
                    return;
                SetProperty(ref _isAllBlackListUserChecked, value);

                SelectAll(_isAllBlackListUserChecked);
                if (IsUnCheckedFromUser)
                    IsUnCheckedFromUser = false;
            }
        }
        private string _blacklistUser = string.Empty;

        public string BlacklistUser
        {
            get
            {
                return _blacklistUser;
            }
            set
            {
                if (value == _blacklistUser)
                    return;
                SetProperty(ref _blacklistUser, value);
            }
        }

        List<BlackListUser> _blackListUser = new List<BlackListUser>();

        private ObservableCollection<BlacklistUserModel> _lstBlackListUsers = new ObservableCollection<BlacklistUserModel>();

        public ObservableCollection<BlacklistUserModel> LstBlackListUsers
        {
            get
            {
                return _lstBlackListUsers;
            }
            set
            {
                if (value == _lstBlackListUsers)
                    return;
                SetProperty(ref _lstBlackListUsers, value);
            }
        }

        public void InitializeData()
        {
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            DbOperations = new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser));

            WhiteListDbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser));

            ThreadFactory.Instance.Start(() =>
            {
                DbOperations.Get<BlackListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                        new BlacklistUserModel
                        {
                            BlacklistUser = user.UserName
                        }));

                    Thread.Sleep(5);
                });
            });
        }

        public virtual void AddToBlackList(object sender)
        {
            if (string.IsNullOrEmpty(BlacklistUser.Trim()))
            {
                GlobusLogHelper.log.Info("Error:- Please enter an username to add to the Blacklist.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstUser = BlacklistUser.Split('\n');
                BlacklistUser = string.Empty;
                _blackListUser = new List<BlackListUser>();


                List<BlackListUser> lstBlackListUser = DbOperations.Get<BlackListUser>();
                List<WhiteListUser> lstWhitelistUser = WhiteListDbOperations.Get<WhiteListUser>();

                lstUser.ForEach(user =>
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (lstBlackListUser.All(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0)
                            && lstWhitelistUser.All(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                _blackListUser.Add(new BlackListUser()
                                {
                                    UserName = userName
                                });
                            }
                            );
                        }
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork,
                                userName, UserType.BlackListedUser,
                                $"{userName} already added to Blacklist/Whitelist. Click on refresh button to view updated list.");
                        }
                    }
                });

                // Remove duplicates
                _blackListUser = _blackListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

                DbOperations.AddRange(_blackListUser);

                if (_blackListUser.Count > 0)
                    ToasterNotification.ShowSuccess(
                        $"Successfully added {_blackListUser.Count} distinct user{(_blackListUser.Count > 1 ? "s" : "")}. Click on refresh button to view updated list");
            });
        }
        private void ClearUser(object sender)
        {
            BlacklistUser = string.Empty;
        }
        public virtual void Refresh(object sender)
        {
            LstBlackListUsers.Clear();
            ThreadFactory.Instance.Start(() =>
            {
                DbOperations.Get<BlackListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                        new BlacklistUserModel
                        {
                            BlacklistUser = user.UserName
                        }));
                    Thread.Sleep(5);
                });
            });
        }
        private void SelectAll(bool isChecked)
        {
            if (IsUnCheckedFromUser)
                return;
            LstBlackListUsers.Select(x => { x.IsBlackListUserChecked = isChecked; return x; }).ToList();
        }

        private void Select(object sender)
        {
            if (LstBlackListUsers.All(x => x.IsBlackListUserChecked))
                IsAllBlackListUserChecked = true;

            else
            {
                if (IsAllBlackListUserChecked)
                    IsUnCheckedFromUser = true;
                IsAllBlackListUserChecked = false;

            }
        }
        public virtual void Delete(object sender)
        {
            var selectedUser = LstBlackListUsers.Where(x => x.IsBlackListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast on user");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                selectedUser.ForEach(x =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LstBlackListUsers.Remove(x);
                        DbOperations.Remove<BlackListUser>(user => user.UserName == x.BlacklistUser);
                    });
                    Thread.Sleep(50);
                });
            });
        }
    }
}
