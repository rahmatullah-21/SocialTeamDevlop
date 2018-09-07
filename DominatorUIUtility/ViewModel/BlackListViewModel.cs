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
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel
{
    public class BlackListViewModel : BindableBase
    {
        public BlackListViewModel()
        {
            AddtoBlackListCommand = new BaseCommand<object>((sender) => true, AddtoBlackList);
            ClearCommand = new BaseCommand<object>((sender) => true, ClearUser);
            RefreshCommand = new BaseCommand<object>((sender) => true, Refresh);
            SelectCommand = new BaseCommand<object>((sender) => true, Select);
            DeleteCommand = new BaseCommand<object>((sender) => true, Delete);
            BindingOperations.EnableCollectionSynchronization(LstBlackListUsers, _lock);
        }


        private static object _lock = new object();
        public ICommand AddtoBlackListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbContext dbContext { get; set; }

        private DbOperations dbOperations { get; set; }

        private bool IsUnCheckedFromUser { get; set; }


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
                _isAllBlackListUserChecked = value;
                SelectAll(_isAllBlackListUserChecked);
                SetProperty(ref _isAllBlackListUserChecked, value);
            }
        }
        private string _blacklistUser;

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
            dbContext = DataBaseConnectionGlb.GetDbContext(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser);
            dbOperations = new DbOperations(dbContext);
            ThreadFactory.Instance.Start(() =>
            {
                dbOperations.Get<BlackListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                        new BlacklistUserModel
                        {
                            BlacklistUser = user.UserName
                        }));

                    Thread.Sleep(50);
                });
            });
        }

        private void AddtoBlackList(object sender)
        {
            if (string.IsNullOrEmpty(BlacklistUser.Trim()))
            {
                GlobusLogHelper.log.Info("Error:- Please enter an username to add to the Blacklist.");
            }
            else
            {
                DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                dbContext = DataBaseConnectionGlb.GetDbContext(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
                var whiteListdbOperations = new DbOperations(dbContext);
                var whiteListUser = whiteListdbOperations.Get<WhiteListUser>();
                Task.Factory.StartNew(() =>
                {
                    BlacklistUser.Split('\n').ForEach(user =>
                {
                    var userName = user.Trim();
                    if (!string.IsNullOrEmpty(userName))
                    {
                        if (!LstBlackListUsers.Any(x => string.Compare(x.BlacklistUser, userName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                 && !whiteListUser.Any(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0))
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                LstBlackListUsers.Add(new BlacklistUserModel()
                                {
                                    BlacklistUser = userName
                                });
                                dbOperations.Add<BlackListUser>(new BlackListUser()
                                {
                                    UserName = userName,
                                    AddedDateTime = DateTime.Now,

                                });
                            }
                        );
                            Thread.Sleep(50);
                        }
                        else
                            GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork, userName, UserType.BlackListedUser, $"{userName} already added to Blacklist/Whitelist");

                    }
                });
                    BlacklistUser = string.Empty;
                });

            }
        }
        private void ClearUser(object sender)
        {
            BlacklistUser = string.Empty;
        }
        private void Refresh(object sender)
        {
            LstBlackListUsers.Clear();
            ThreadFactory.Instance.Start(() =>
            {
                dbOperations.Get<BlackListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstBlackListUsers.Add(
                        new BlacklistUserModel
                        {
                            BlacklistUser = user.UserName
                        }));
                    Thread.Sleep(50);
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


        private void Delete(object sender)
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
                        dbOperations.Remove<BlackListUser>(user => user.UserName == x.BlacklistUser);
                    });
                    Thread.Sleep(50);
                });
            });
        }

    }


}
