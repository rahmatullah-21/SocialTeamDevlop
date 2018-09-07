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
    public class WhiteListViewModel : BindableBase
    {
        public WhiteListViewModel()
        {
            AddtoWhiteListCommand = new BaseCommand<object>((sender) => true, AddToWhiteList);
            ClearCommand = new BaseCommand<object>((sender) => true, ClearUser);
            RefreshCommand = new BaseCommand<object>((sender) => true, Refresh);
            SelectCommand = new BaseCommand<object>((sender) => true, Select);
            DeleteCommand = new BaseCommand<object>((sender) => true, Delete);
            BindingOperations.EnableCollectionSynchronization(LstWhiteListUsers, _lock);
        }


        private static object _lock = new object();
        public ICommand AddtoWhiteListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbContext dbContext { get; set; }

        private DbOperations dbOperations { get; set; }

        private bool IsUnCheckedFromUser { get; set; }


        private WhitelistUserModel _whitelistUserModel = new WhitelistUserModel();

        public WhitelistUserModel WhitelistUserModel
        {
            get
            {
                return _whitelistUserModel;
            }
            set
            {
                if (_whitelistUserModel == value)
                    return;
                _whitelistUserModel = value;
                SetProperty(ref _whitelistUserModel, value);
            }
        }
        private bool _isAllWhiteistUserChecked;

        public bool IsAllWhiteListUserChecked
        {
            get
            {
                return _isAllWhiteistUserChecked;
            }
            set
            {
                if (value == _isAllWhiteistUserChecked)
                    return;
                _isAllWhiteistUserChecked = value;
                SelectAll(_isAllWhiteistUserChecked);
                SetProperty(ref _isAllWhiteistUserChecked, value);
            }
        }
        private string _whitelistUser;

        public string WhitelistUser
        {
            get
            {
                return _whitelistUser;
            }
            set
            {
                if (value == _whitelistUser)
                    return;
                SetProperty(ref _whitelistUser, value);
            }
        }
        private ObservableCollection<WhitelistUserModel> _lstWhiteListUsers = new ObservableCollection<WhitelistUserModel>();

        public ObservableCollection<WhitelistUserModel> LstWhiteListUsers
        {
            get
            {
                return _lstWhiteListUsers;
            }
            set
            {
                if (value == _lstWhiteListUsers)
                    return;
                SetProperty(ref _lstWhiteListUsers, value);
            }
        }

        public void InitializeData()
        {
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            dbContext = DataBaseConnectionGlb.GetDbContext(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser);
            dbOperations = new DbOperations(dbContext);
            ThreadFactory.Instance.Start(() =>
            {
                dbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                        new WhitelistUserModel
                        {
                            WhitelistUser = user.UserName
                        }));

                    Thread.Sleep(50);
                });
            });
        }

        private void AddToWhiteList(object sender)
        {
            if (string.IsNullOrEmpty(WhitelistUser.Trim()))
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
                    WhitelistUser.Split('\n').ForEach(user =>
                    {
                        var userName = user.Trim();
                        if (!string.IsNullOrEmpty(userName))
                        {
                            if (!LstWhiteListUsers.Any(x => string.Compare(x.WhitelistUser, userName, StringComparison.InvariantCultureIgnoreCase) == 0)
                                     && !whiteListUser.Any(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) == 0))
                            {
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    LstWhiteListUsers.Add(new WhitelistUserModel()
                                    {
                                        WhitelistUser = userName
                                    });
                                    dbOperations.Add<WhiteListUser>(new WhiteListUser()
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
                    WhitelistUser = string.Empty;
                });
               
            }
        }
        private void ClearUser(object sender)
        {
            WhitelistUser = string.Empty;
        }
        private void Refresh(object sender)
        {
            LstWhiteListUsers.Clear();
            ThreadFactory.Instance.Start(() =>
            {
                dbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                        new WhitelistUserModel
                        {
                            WhitelistUser = user.UserName
                        }));
                    Thread.Sleep(50);
                });
            });
        }
        private void SelectAll(bool isChecked)
        {
            if (IsUnCheckedFromUser)
                return;
            LstWhiteListUsers.Select(x => { x.IsWhiteListUserChecked = isChecked; return x; }).ToList();
        }

        private void Select(object sender)
        {
            if (LstWhiteListUsers.All(x => x.IsWhiteListUserChecked))
                IsAllWhiteListUserChecked = true;

            else
            {
                if (IsAllWhiteListUserChecked)
                    IsUnCheckedFromUser = true;
                IsAllWhiteListUserChecked = false;

            }
        }


        private void Delete(object sender)
        {
            var selectedUser = LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
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
                        LstWhiteListUsers.Remove(x);
                        dbOperations.Remove<BlackListUser>(user => user.UserName == x.WhitelistUser);
                    });
                    Thread.Sleep(50);
                });
            });
        }

    }
}
