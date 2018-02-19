using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using ProtoBuf;

namespace DominatorUIUtility.ViewModel
{
    [ProtoContract]
    public class DominatorAccountViewModel : BindableBase
    {

        public DominatorAccountViewModel()
        {

            InitialAccountDetails();

            #region Command Initialization

            AddSingleAccountCommand = new BaseCommand<object>(AddSingleAccountCanExecute, AddSingleAccountExecute);

            LoadMultipleAccountsCommand = new BaseCommand<object>(LoadMultipleAccountsCanExecute, LoadMultipleAccountsExecute);

            // InfoCommand = new BaseCommand<object>(InfoCommandCanExecute, InfoCommandExecute);

            ContextMenuOpenCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);

            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);

            DeleteAccountsCommand = new BaseCommand<object>(DeleteAccountsCanExecute, DeleteAccountsExecute);

            SelectAccountCommand = new BaseCommand<object>(SelectAccountCanExecute, SelectAccountExecute);

            SelectAccountByStatusCommand = new BaseCommand<object>(SelectAccountByStatusCanExecute, SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new BaseCommand<object>(SelectAccountByGroupCanExecute, SelectAccountByGroupExecute);

            SingleAccountEditCommand = new BaseCommand<object>(SingleAccountEditCanExecute, SingleAccountEditExecute);

            SingleAccountDeleteCommand = new BaseCommand<object>(SingleAccountDeleteCanExecute, SingleAccountDeleteExecute);

            #endregion
        }

        #region Property

        private ICollectionView _accountCollectionView;

        public ICollectionView AccountCollectionView
        {
            get
            {
                return _accountCollectionView;
            }
            set
            {
                if (_accountCollectionView != null && _accountCollectionView == value)
                    return;
                SetProperty(ref _accountCollectionView, value);

            }
        }


        public ObservableCollection<DominatorAccountModel> LstDominatorAccountModel { get; set; } = new ObservableCollection<DominatorAccountModel>();

        public ObservableCollection<ContentSelectGroup> Groups { get; set; } = new ObservableCollection<ContentSelectGroup>();


        private GridViewHeader _gridHeaderColumn1 = new GridViewHeader();

        public GridViewHeader GridHeaderColumn1
        {
            get
            {
                return _gridHeaderColumn1;
            }
            set
            {
                if (_gridHeaderColumn1 != null && _gridHeaderColumn1 == value)
                    return;

                SetProperty(ref _gridHeaderColumn1, value);
            }
        }

        private int _displayColumnValue1 = 100;

        public int DisplayColumnValue1
        {
            get
            {
                return _displayColumnValue1;
            }
            set
            {
                if (_displayColumnValue1 == value)
                    return;

                SetProperty(ref _displayColumnValue1, value);

            }
        }




        private GridViewHeader _gridHeaderColumn2 = new GridViewHeader();

        public GridViewHeader GridHeaderColumn2
        {
            get
            {
                return _gridHeaderColumn2;
            }
            set
            {
                if (_gridHeaderColumn2 != null && _gridHeaderColumn2 == value)
                    return;

                SetProperty(ref _gridHeaderColumn2, value);
            }
        }


        private int _displayColumnValue2 = 100;

        public int DisplayColumnValue2
        {
            get
            {
                return _displayColumnValue2;
            }
            set
            {
                if (_displayColumnValue2 == value)
                    return;

                SetProperty(ref _displayColumnValue2, value);

            }
        }


        private GridViewHeader _gridHeaderColumn3 = new GridViewHeader();

        public GridViewHeader GridHeaderColumn3
        {
            get
            {
                return _gridHeaderColumn3;
            }
            set
            {
                if (_gridHeaderColumn3 != null && _gridHeaderColumn3 == value)
                    return;

                SetProperty(ref _gridHeaderColumn3, value);
            }
        }


        private int _displayColumnValue3 = 100;

        public int DisplayColumnValue3
        {
            get
            {
                return _displayColumnValue3;
            }
            set
            {
                if (_displayColumnValue3 == value)
                    return;

                SetProperty(ref _displayColumnValue3, value);

            }
        }


        private GridViewHeader _gridHeaderColumn4 = new GridViewHeader();

        public GridViewHeader GridHeaderColumn4
        {
            get
            {
                return _gridHeaderColumn4;
            }
            set
            {
                if (_gridHeaderColumn4 != null && _gridHeaderColumn4 == value)
                    return;

                SetProperty(ref _gridHeaderColumn4, value);
            }
        }


        private int _displayColumnValue4 = 100;

        public int DisplayColumnValue4
        {
            get
            {
                return _displayColumnValue4;
            }
            set
            {
                if (_displayColumnValue4 == value)
                    return;

                SetProperty(ref _displayColumnValue4, value);

            }
        }


        private SocialNetworks _socialNetwork = SocialNetworks.Facebook;

        public SocialNetworks SocialNetwork
        {
            get
            {
                return _socialNetwork;
            }
            set
            {
                if (_socialNetwork == value)
                    return;
                SetProperty(ref _socialNetwork, value);
            }
        }





        #endregion

        #region Command 

        public ICommand AddSingleAccountCommand { get; set; }
        public ICommand InfoCommand { get; set; }
        public ICommand LoadMultipleAccountsCommand { get; set; }
        public ICommand ContextMenuOpenCommand { get; set; }
        public ICommand DeleteAccountsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand SelectAccountCommand { get; set; }
        public ICommand SelectAccountByStatusCommand { get; set; }
        public ICommand SelectAccountByGroupCommand { get; set; }
        public ICommand SingleAccountEditCommand { get; set; }
        public ICommand SingleAccountDeleteCommand { get; set; }

        #endregion

        #region Add accounts

        private bool AddSingleAccountCanExecute(object sender) => true;

        private void AddSingleAccountExecute(object sender)
        {

            var objDominatorAccountBaseModel = new DominatorAccountBaseModel();

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "Add Account", "Save", false, DominatorHouseInitializer.ActiveSocialNetwork.ToString());



            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objAddUpdateAccountControl
            };

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog);

            objAddUpdateAccountControl.btnSave.Click += (senders, events) =>
            {
                if (string.IsNullOrEmpty(objDominatorAccountBaseModel.UserName) ||
                    string.IsNullOrEmpty(objDominatorAccountBaseModel.Password)) return;

                if (objAddUpdateAccountControl.ComboBoxSocialNetworks.Text.ToString() != objDominatorAccountBaseModel.AccountNetwork.ToString())
                    objDominatorAccountBaseModel.AccountNetwork =
                        (SocialNetworks)Enum.Parse(typeof(SocialNetworks), objAddUpdateAccountControl.ComboBoxSocialNetworks.Text.ToString());

                AddAccount(objDominatorAccountBaseModel);
                dialogWindow.Close();
            };

            objAddUpdateAccountControl.ComboBoxSocialNetworks.SelectionChanged += (senders, events) =>
            {
                var objComboBox = ((FrameworkElement)senders) as ComboBox;
                if (objComboBox != null)
                    objDominatorAccountBaseModel.AccountNetwork =
                        (SocialNetworks)Enum.Parse(typeof(SocialNetworks), objComboBox.SelectedValue.ToString());
            };

            objAddUpdateAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

            dialogWindow.ShowDialog();
        }

        private bool LoadMultipleAccountsCanExecute(object sender)
        {
            return true;
        }

        /// <summary>
        ///LoadMultipleAccountsExecute is used to load multiple accounts at a time
        ///GroupName:Username:Password:ProxyIp:ProxyPort:ProxyUsername:ProxyPassword
        ///GroupName:Username:Password:ProxyIp:ProxyPort
        ///GroupName:Username:Password
        ///Can load , instead of :
        ///If any values are null, we can use NA        
        /// </summary>
        /// <param name="sender"></param>
        private void LoadMultipleAccountsExecute(object sender)
        {
            //Read the accounts from text or csv files
            var loadedAccountlist = FileUtilities.FileBrowseAndReader();

            //if loaded text or csv contains no accounts then return
            if (loadedAccountlist == null) return;

            #region Add to bin files which are valid accounts 

            //Iterate the all accounts one by one
            foreach (var singleAccount in loadedAccountlist)
            {
                try
                {
                    var finalAccount = singleAccount.Replace(",", ":").Replace("NA", "");
                    var splitAccount = Regex.Split(finalAccount, ":");
                    if (splitAccount.Length <= 1) continue;

                    //assign the username, password and groupname
                    var groupname = splitAccount[0];

                    var socialNetwork = splitAccount[1];
                    var username = splitAccount[2];
                    var password = splitAccount[3];

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        continue;

                    var proxyaddress = string.Empty;
                    var proxyport = string.Empty;
                    var proxyusername = string.Empty;
                    var proxypassword = string.Empty;

                    switch (splitAccount.Length)
                    {
                        case 6:
                            proxyaddress = splitAccount[4];
                            proxyport = splitAccount[5];
                            break;
                        case 8:
                            proxyaddress = splitAccount[4];
                            proxyport = splitAccount[5];
                            proxyusername = splitAccount[6];
                            proxypassword = splitAccount[7];
                            break;
                    }

                    if (splitAccount.Length > 4)
                    {
                        if (string.IsNullOrEmpty(proxyaddress) || string.IsNullOrEmpty(proxyport))
                        {
                            proxyaddress = proxyport = proxyusername = proxypassword = string.Empty;
                        }
                        //valid the proxy ip and port
                        else if (!Proxy.IsValidProxyIp(proxyaddress) || !Proxy.IsValidProxyPort(proxyport))
                            continue;
                    }


                    var objDominatorAccountBaseModel = new DominatorAccountBaseModel
                    {
                        AccountGroup =
                        {
                            Content = groupname ?? ConstantVariable.UnGrouped
                        },
                        UserName = username,
                        Password = password,
                        AccountProxy =
                            {
                                ProxyIp = proxyaddress,
                                ProxyPort = proxyport,
                                ProxyUsername = proxyusername,
                                ProxyPassword = proxypassword
                            },
                        AccountNetwork =  (SocialNetworks) Enum.Parse(typeof(SocialNetworks), socialNetwork)
                    };

                    //add the account to DominatorAccountModel list and bin file
                    AddAccount(objDominatorAccountBaseModel);
                }
                catch (Exception ex)
                {
                    /*INFO*/
                    Console.WriteLine(ex.StackTrace);
                    GlobusLogHelper.log.Error(ex.Message);
                }
            }

            #endregion

        }

        public void AddAccount(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {

            #region Add Account
            //check the account is already present or not
            if (LstDominatorAccountModel.Any(x => x.AccountBaseModel.UserName == objDominatorAccountBaseModel.UserName))
            {
                /*INFO*/
                Console.WriteLine($@"Account [{objDominatorAccountBaseModel.UserName}] already added!");
                GlobusLogHelper.log.Info($@"Account [{objDominatorAccountBaseModel.UserName}] already added!");
                return;
            }

            //Initialize the given account to account model
            var dominatorAccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup =
                {
                    Content = objDominatorAccountBaseModel.AccountGroup.Content ?? ConstantVariable.UnGrouped
                },
                UserName = objDominatorAccountBaseModel.UserName,
                Password = objDominatorAccountBaseModel.Password,
                AccountProxy =
                {
                    ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword
                },
                Status = ConstantVariable.NotChecked,
                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1
            };            


            //serialize the given account, if its success then add to account model list
            if (AccountsFileManager.Add(dominatorAccountModel))
            {
                LstDominatorAccountModel.Add(dominatorAccountModel);
            }

            else
            {
                /*INFO*/
                Console.WriteLine($@"Account [{dominatorAccountModel.AccountBaseModel.UserName}] isn't saved!");
                GlobusLogHelper.log.Info($@"Account [{dominatorAccountModel.AccountBaseModel.UserName}] isn't saved!");
            }

            DataBaseHandler.CreateDataBase(objDominatorAccountBaseModel.UserName);

            #endregion

            #region Login Account And Update Follower And Following Count

            UpdateAccount(dominatorAccountModel);

            #endregion
        }


        public void UpdateAccount(DominatorAccountModel objDominatorAccountModel)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;

            Task.Factory.StartNew(() =>
            {
                //  UpdateAccountFollowerFollowing(objDominatorAccountModel);
            }, ct);

        }

        //private static void UpdateAccountFollowerFollowing(DominatorAccountModel objDominatorAccountModel)
        //{
        //    try
        //    {
        //        if (!LogInProcess.checkLogin(objDominatorAccountModel))
        //            return;
        //        if (!objDominatorAccountModel.IsUserLoggedIn)
        //        {
        //            LogInProcess logInProcess = new LogInProcess();
        //            logInProcess.LoginWithMobileDevice(ref objDominatorAccountModel);
        //        }

        //        if (!objDominatorAccountModel.IsUserLoggedIn)
        //            return;

        //        DataBaseConnectionCodeFirst.DataBaseConnection databaseConnection =
        //            DataBaseHandler.GetDataBaseConnectionInstance(objDominatorAccountModel.UserName);
        //        InstaFunct instaFunct = new InstaFunct(objDominatorAccountModel);

        //        try
        //        {
        //            UsernameInfoIgResponseHandler userInfo = instaFunct.SearchUsername(objDominatorAccountModel.UserName);
        //            objDominatorAccountModel.PostsCount = userInfo.MediaCount;
        //            objDominatorAccountModel.FollowersCount = userInfo.FollowerCount;
        //            objDominatorAccountModel.FollowingCount = userInfo.FollowingCount;
        //            List<InstagramUser> lstUserFollowers =
        //                instaFunct.GetUserFollowers(objDominatorAccountModel.UserId).UsersList;

        //            lstUserFollowers.ForEach(x =>
        //            {
        //                try
        //                {
        //                    Friendships friendship = new Friendships()
        //                    {
        //                        Username = x.Username,
        //                        IsPrivate = x.IsPrivate ? 1 : 0,
        //                        IsVerified = x.IsVerified ? 1 : 0,
        //                        UserId = x.Pk,
        //                        FullName = x.FullName,
        //                        HasAnonymousProfilePicture =
        //                            (x.HasAnonymousProfilePicture == null || x.HasAnonymousProfilePicture == false) ? 0 : 1,
        //                        ProfilePicUrl = x.ProfilePicUrl
        //                    };
        //                    databaseConnection.Add<Friendships>(friendship);
        //                }
        //                catch (Exception e)
        //                {
        //                    GlobusLogHelper.log.Error(e.Message);
        //                }
        //            });
        //        }
        //        catch (Exception Ex)
        //        {
        //            GlobusLogHelper.log.Error(Ex.Message);
        //        }

        //        try
        //        {
        //            List<InstagramUser> lstUserFollowings =
        //                instaFunct.GetUserFollowings(objDominatorAccountModel.UserId).UsersList;

        //            lstUserFollowings.ForEach(x =>
        //            {
        //                try
        //                {
        //                    Friendships friendship = new Friendships()
        //                    {
        //                        Username = x.Username,
        //                        IsPrivate = x.IsPrivate ? 1 : 0,
        //                        IsVerified = x.IsVerified ? 1 : 0,
        //                        UserId = x.Pk,
        //                        FullName = x.FullName,
        //                        HasAnonymousProfilePicture =
        //                            (x.HasAnonymousProfilePicture == null || x.HasAnonymousProfilePicture == false) ? 0 : 1,
        //                        ProfilePicUrl = x.ProfilePicUrl
        //                    };

        //                    databaseConnection.Add<Friendships>(friendship);
        //                }
        //                catch (Exception e)
        //                {
        //                    GlobusLogHelper.log.Error(e.Message);
        //                }
        //            });
        //        }
        //        catch (Exception ex)
        //        {
        //            GlobusLogHelper.log.Error(ex.Message);
        //        }

        //        GdBinFileHelper.UpdateAccount(objDominatorAccountModel);
        //    }
        //    catch (Exception Ex)
        //    {
        //        GlobusLogHelper.log.Error(Ex.Message + Ex.StackTrace);
        //    }

        //}

        #endregion

        #region Delete Accounts

        private bool SingleAccountDeleteCanExecute(object sender) => true;

        private void SingleAccountDeleteExecute(object sender)
        {
            DeleteAccountByContextMenu(sender);
        }


        private bool DeleteAccountsCanExecute(object sender)
        {
            return true;
        }

        private void DeleteAccountsExecute(object sender)
        {
            try
            {
                //collect the selected account
                var selectAccounts = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected == true).ToList();

                DeleteAccounts(selectAccounts);

            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        private bool DeleteAccounts(List<DominatorAccountModel> selectAccounts)
        {

            //if selectedaccount count is zero, it wont delete the bin file
            if (selectAccounts.Count == 0) return true;            
            
            //remove the selected accounts from account model
            selectAccounts.ForEach(item => LstDominatorAccountModel.Remove(item));


            //after removed serialize the remaining accounts 
            AccountsFileManager.Delete<DominatorAccountModel>(a => selectAccounts.FirstOrDefault(p => p.AccountId == a.AccountId) != null);

            return false;
        }

        public void DeleteAccountByContextMenu(object sender)
        {
            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            var selectedAccount = LstDominatorAccountModel.FirstOrDefault<DominatorAccountModel>(x => selectedRow != null && x.AccountBaseModel.AccountId == selectedRow.AccountBaseModel.AccountId);

            DeleteAccounts(new List<DominatorAccountModel> { selectedAccount });
            GlobusLogHelper.log.Info(selectedAccount + " Deleted");
        }


        #endregion

        #region ContextMenuIsOpen 

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {


            ContextMenuOpen(sender);
            var button = sender as Button;
            if (button == null || button.Name != "BtnSelect") return;

            var currentGroups = LstDominatorAccountModel.Select(x => x.AccountBaseModel.AccountGroup.Content).Distinct().ToList();

            Groups.Clear();

            var availableGroups = Groups.Select(x => x.Content).ToList();

            var newGroups = currentGroups.Except(availableGroups).ToList();

            if (newGroups.Count <= 0)
                return;

            newGroups.ForEach(x =>
            {
                Groups.Add(new ContentSelectGroup()
                {
                    Content = x,
                    IsContentSelected = false
                });
            });

        }

        private static void ContextMenuOpen(object sender)
        {
            try
            {
                var contextMenu = ((Button)sender).ContextMenu;
                if (contextMenu == null) return;
                contextMenu.DataContext = ((Button)sender).DataContext;
                contextMenu.IsOpen = true;
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        #endregion

        #region Export Accounts

        private bool ExportCanExecute(object sender) => true;

        private void ExportExecute(object sender)
        {

            var selectedAccounts = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected == true).ToList();

            if (selectedAccounts.Count <= 0)
                return;

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            const string header = "Username,Password,Account Group,Status,Follower,Followings,Posts,Proxy Address,Proxy Port,Proxy Username,Proxy Password";

            var filename = $"{exportPath}\\AccountExport {ConstantVariable.DateasFileName}.csv";

            if (!File.Exists(filename))
            {
                using (var streamWriter = new StreamWriter(filename, true))
                {
                    streamWriter.WriteLine(header);
                }
            }

            selectedAccounts.ForEach(account =>
            {
                try
                {
                    //var csvData = account.AccountBaseModel.UserName + "," + account.AccountBaseModel.Password + "," + account.AccountBaseModel.AccountGroup.Content + "," + account.AccountBaseModel.Status + "," + account.FollowersCount + "," +
                    //              account.FollowingCount + "," + account.PostsCount + "," + account.AccountBaseModel.AccountProxy.ProxyIp + "," + account.AccountBaseModel.AccountProxy.ProxyPort + "," + account.AccountBaseModel.AccountProxy.ProxyUsername + "," + account.AccountBaseModel.AccountProxy.ProxyPassword;

                    //using (var streamWriter = new StreamWriter(filename, true))
                    //{
                    //    streamWriter.WriteLine(csvData);
                    //}
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }

        #endregion

        #region Help Methods

        //private bool InfoCommandCanExecute(object sender) => true;

        //private void InfoCommandExecute(object sender) => IsOpenHelpControl = true;

        #endregion

        #region Edit Accounts

        private bool SingleAccountEditCanExecute(object sender) => true;

        private void SingleAccountEditExecute(object sender)
        {
            EditAccount(sender);
        }

        public void EditAccount(object sender)
        {

            var selectedAccount = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

          
            // var selectedAccount = LstDominatorAccountModel.FirstOrDefault<DominatorAccountModel>(x => selectedRow != null && x.RowNo == selectedRow.RowNo);

            if (selectedAccount == null) return;

            var objDominatorAccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup = new ContentSelectGroup() { Content = selectedAccount.AccountBaseModel.AccountGroup.Content },
                UserName = selectedAccount.AccountBaseModel.UserName,
                Password = selectedAccount.AccountBaseModel.Password,
                AccountProxy =
                {
                    ProxyIp = selectedAccount.AccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = selectedAccount.AccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword
                },
                AccountNetwork = selectedAccount.AccountBaseModel.AccountNetwork
            };

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "Update Account", "Update", !string.IsNullOrEmpty(selectedAccount.AccountBaseModel.AccountProxy.ProxyIp), objDominatorAccountBaseModel.AccountNetwork.ToString());

            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objAddUpdateAccountControl
            };

            var objDialog = new Dialog();

            var dialogWindow = objDialog.GetCustomDialog(customDialog);

            objAddUpdateAccountControl.btnSave.Click += (senders, events) =>
            {

                if (string.IsNullOrEmpty(objDominatorAccountBaseModel.UserName) ||
                    string.IsNullOrEmpty(objDominatorAccountBaseModel.Password)) return;

                selectedAccount.AccountBaseModel.AccountGroup.Content = objDominatorAccountBaseModel.AccountGroup.Content;
                selectedAccount.AccountBaseModel.UserName = objDominatorAccountBaseModel.UserName;
                selectedAccount.AccountBaseModel.Password = objDominatorAccountBaseModel.Password;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;
                selectedAccount.AccountBaseModel.AccountNetwork = objDominatorAccountBaseModel.AccountNetwork;

                //  File.Delete(ConstantVariable.GetIndexAccountPath() + $"//{ConstantVariable.AccountDetails}");

                //ProtoBuffBase.SerializeListObject<DominatorAccountModel>(LstDominatorAccountModel,
                //    ConstantVariable.GetIndexAccountPath() + $"//{ConstantVariable.AccountDetails}");

                dialogWindow.Close();

            };
            objAddUpdateAccountControl.btnCancel.Click += (senders, events) =>
            {
                dialogWindow.Close();
            };

            dialogWindow.ShowDialog();
        }


        #endregion

        #region Select Accounts

        private bool SelectAccountCanExecute(object sender) => true;

        private void SelectAccountExecute(object sender)
        {
            var selection = sender as string;

            if (selection == "SelectAll")
                SelectAllAccounts();
            else
                DeselectAllAccounts();
        }

        private bool SelectAccountByStatusCanExecute(object sender) => true;

        private void SelectAccountByStatusExecute(object sender)
        {
            SelectAccount(sender);
        }

        public void SelectAllAccounts()
        {
            LstDominatorAccountModel.Select(x =>
            {
                x.IsAccountManagerAccountSelected = true; return x;
            }).ToList();
        }

        public void DeselectAllAccounts()
        {
            LstDominatorAccountModel.Select(x =>
            {
                x.IsAccountManagerAccountSelected = false; return x;
            }).ToList();
        }

        public void SelectAccount(object sender)
        {

            DeselectAllAccounts();

            var menu = sender as string;

            switch (menu)
            {
                case "Working":
                    LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Success").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotWorking":
                    LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Failed").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
                case "NotChecked":
                    LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Not Checked").Select(x =>
                    {
                        x.IsAccountManagerAccountSelected = true;
                        return x;
                    }).ToList();
                    break;
            }
        }

        private bool SelectAccountByGroupCanExecute(object sender) => true;

        private void SelectAccountByGroupExecute(object sender)
        {
            SelectAccountByGroup(sender);
        }


        public void SelectAccountByGroup(object sender)
        {
            try
            {
                var checkedItem = sender as CheckBox;
                if (checkedItem == null) return;

                var currentGroup = ((FrameworkElement)sender).DataContext as ContentSelectGroup;

                Groups.Where(x => currentGroup != null && x.Content == currentGroup.Content.ToString()).Select(x =>
                {
                    LstDominatorAccountModel.Where(y => y.AccountBaseModel.AccountGroup.Content == x.Content).Select(y =>
                    {
                        if (currentGroup != null) y.IsAccountManagerAccountSelected = currentGroup.IsContentSelected;
                        return y;
                    }).ToList();
                    return x;
                }).ToList();
            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        #endregion

        #region Initialize AccountManager

        private object syncLoadAccounts = new object();

        public void InitialAccountDetails()
        {
            lock (syncLoadAccounts)
            {               
                var savedAccounts = AccountsFileManager.GetAll();

                var allGroups = new List<ContentSelectGroup>();

                try
                {
                    LstDominatorAccountModel.Clear();
                    savedAccounts.ForEach(account =>
                    {
                        LstDominatorAccountModel.Add(account);
                        allGroups.Add(account.AccountBaseModel.AccountGroup);

                        AccountCollectionView = CollectionViewSource.GetDefaultView(LstDominatorAccountModel);
                        //Global.ScheduleForEachModule(null, account);
                    });

                    foreach (var group in allGroups)
                    {
                        if (Groups.Any(x => x.Content.Equals
                                (group.Content, StringComparison.CurrentCultureIgnoreCase)) == false)
                            Groups.Add(group);
                    }
                }
                catch (Exception ex)
                {
                    /*DEBUG*/
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion


    }

    public class GridViewHeader : BindableBase
    {

        private string _headers;

        public string Header
        {
            get
            {
                return _headers;
            }
            set
            {
                if (_headers != null && _headers == value)
                    return;
                SetProperty(ref _headers, value);
            }
        }

        private bool _headerVisible = false;

        public bool HeaderVisible
        {
            get
            {
                return _headerVisible;
            }
            set
            {
                if (_headerVisible == value)
                    return;
                SetProperty(ref _headerVisible, value);
            }
        }
    }
}
