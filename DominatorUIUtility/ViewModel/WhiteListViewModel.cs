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
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using Prism.Commands;

namespace DominatorUIUtility.ViewModel
{
    public interface IPrivateWhiteListViewModel
    {

    }
    public class WhiteListViewModel : BindableBase
    {
        public WhiteListViewModel()
        {
            AddToWhiteListCommand = new BaseCommand<object>((sender) => true, AddToWhiteList);
            ClearCommand = new BaseCommand<object>((sender) => true, ClearUser);
            RefreshCommand = new BaseCommand<object>((sender) => true, Refresh);
            SelectCommand = new BaseCommand<object>((sender) => true, Select);
            DeleteCommand = new BaseCommand<object>((sender) => true, Delete);
            ImportCommand = new DelegateCommand(ImportUser);
            ExportCommand = new DelegateCommand(ExportUser);
            BindingOperations.EnableCollectionSynchronization(LstWhiteListUsers, _lock);
        }

        #region Commands
        public ICommand AddToWhiteListCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand ImportCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        #endregion

        #region Properties

        private static object _lock = new object();
        private IGlobalDatabaseConnection DataBaseConnectionGlb { get; set; }

        private DbOperations DbOperations { get; set; }

        private DbOperations BlackListDbOperations { get; set; }

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

        private PrivateWhitelistUserModel _privateWhitelistUserModel = new PrivateWhitelistUserModel();

        public PrivateWhitelistUserModel PrivateWhitelistUserModel
        {
            get { return _privateWhitelistUserModel; }
            set { SetProperty(ref _privateWhitelistUserModel, value); }
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
                SetProperty(ref _isAllWhiteistUserChecked, value);
                SelectAll(_isAllWhiteistUserChecked);
                if (IsUnCheckedFromUser)
                    IsUnCheckedFromUser = false;
            }
        }
        private string _whitelistUser = string.Empty;

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

        List<WhiteListUser> _whiteListUser = new List<WhiteListUser>();
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

        #endregion
        public void InitializeData()
        {
            DataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
            DbOperations = new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.WhiteListedUser));

            BlackListDbOperations =
                new DbOperations(DataBaseConnectionGlb.GetSqlConnection(SocinatorInitialize.ActiveSocialNetwork, UserType.BlackListedUser));

            ThreadFactory.Instance.Start(() =>
            {
                DbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                                                     new WhitelistUserModel
                                                     {
                                                         WhitelistUser = user.UserName
                                                     }));
                    Thread.Sleep(5);
                });
            });
        }

        public virtual void AddToWhiteList(object sender)
        {
            if (string.IsNullOrEmpty(WhitelistUser.Trim()))
            {
                GlobusLogHelper.log.Info("Error:- Please enter a username to add to the Whitelist.");
                return;
            }

            Task.Factory.StartNew(() =>
            {
                var lstuser = WhitelistUser.Split('\n');
                WhitelistUser = string.Empty;
                _whiteListUser = new List<WhiteListUser>();

                AddToDB(lstuser?.ToList());
            });
        }

        public virtual void AddToDB(List<string> lstuser)
        {
            List<BlackListUser> lstBlackListUser = BlackListDbOperations?.Get<BlackListUser>();
            lstuser?.ForEach(user =>
            {
                var userName = user.Trim();
                if (!string.IsNullOrEmpty(userName))
                {
                    if (LstWhiteListUsers.All(x => string.Compare(x.WhitelistUser, userName, StringComparison.InvariantCultureIgnoreCase) != 0)
                        && lstBlackListUser.All(x => string.Compare(x.UserName, userName, StringComparison.InvariantCultureIgnoreCase) != 0))
                    {

                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _whiteListUser.Add(new WhiteListUser()
                            {
                                UserName = userName
                            });
                        }
                        );
                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SocinatorInitialize.ActiveSocialNetwork,
                            userName, UserType.WhiteListedUser,
                            $"{userName} already added to Whitelist/Blacklist. Click on refresh button to view updated list.");
                    }
                }
            });

            // Remove duplicates
            _whiteListUser = _whiteListUser.GroupBy(x => x.UserName).Select(y => y.First()).ToList();

            DbOperations.AddRange(_whiteListUser);

            if (_whiteListUser.Count > 0)
                ToasterNotification.ShowSuccess(
                    $"Successfully added {_whiteListUser.Count} distinct user{(_whiteListUser.Count > 1 ? "s" : "")}. Click on refresh button to view updated list");
        }

        private void ClearUser(object sender)
        {
            WhitelistUser = string.Empty;
        }
        public virtual void Refresh(object sender)
        {
            LstWhiteListUsers.Clear();
            ThreadFactory.Instance.Start(() =>
            {
                DbOperations.Get<WhiteListUser>()?.ForEach(user =>
                {
                    Application.Current.Dispatcher.Invoke(() => LstWhiteListUsers.Add(
                        new WhitelistUserModel
                        {
                            WhitelistUser = user.UserName
                        }));
                    Thread.Sleep(5);
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

        public virtual void Delete(object sender)
        {
            var selectedUser = LstWhiteListUsers.Where(x => x.IsWhiteListUserChecked).ToList();
            if (selectedUser.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast one user");
                return;
            }
            Task.Factory.StartNew(() =>
            {
                selectedUser.ForEach(x =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        LstWhiteListUsers.Remove(x);
                        DbOperations.Remove<WhiteListUser>(user => user.UserName == x.WhitelistUser);
                    });
                    Thread.Sleep(50);
                });
            });
        }

        private void ExportUser()
        {
            var selectedUsers = LstWhiteListUsers?.Where(x => x.IsWhiteListUserChecked);
            if (selectedUsers?.Count() == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast one user !!");
                return;
            }

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;


            var filename = $"{exportPath}\\{SocinatorInitialize.ActiveSocialNetwork}_WhiteList {ConstantVariable.DateasFileName}.csv";

            selectedUsers.ForEach(user =>
            {
                try
                {
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(user.WhitelistUser);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
            Dialog.ShowDialog("Export WhiteList user", $"WhiteListed user Successfully exported to [ {filename} ]");
        }

        private void ImportUser()
        {
            var lstUser = FileUtilities.FileBrowseAndReader();
            AddToDB(lstUser);
        }
    }
}
