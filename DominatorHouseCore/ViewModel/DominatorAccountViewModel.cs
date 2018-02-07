using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DataBaseConnection.CommonDatabaseConnection.Tables.Account;
using DominatorHouseCore.Command;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.ViewModel
{
    public class DominatorAccountViewModel
    {
        public DominatorAccountViewModel()
        {
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

        public ObservableCollection<DominatorAccountModel> LstDominatorAccountModel { get; set; } = new ObservableCollection<DominatorAccountModel>();

        public ObservableCollection<ContentSelectGroup> Groups { get; set; } = new ObservableCollection<ContentSelectGroup>();

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

            //var objSingleDominatorAccountModel = new SingleDominatorAccountModel
            //{
            //    BtnContent = "Save",
            //    PageTitle = "Add Single Account"
            //};

            //var objSingleAccountControl = new SingleAccountControl(objSingleDominatorAccountModel);

            //var customDialog = new CustomDialog()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Content = objSingleAccountControl
            //};

            //var objDialog = new Dialog();
            //var dialogWindow = objDialog.GetCustomDialog(customDialog);

            //objSingleAccountControl.btnSave.Click += (senders, events) =>
            //{
            //    if (string.IsNullOrEmpty(objSingleDominatorAccountModel.UserName) ||
            //        string.IsNullOrEmpty(objSingleDominatorAccountModel.Password)) return;

            //    AddAccount(objSingleDominatorAccountModel);
            //    dialogWindow.Close();
            //};

            //objSingleAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

            //dialogWindow.ShowDialog();

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
                    var username = splitAccount[1];
                    var password = splitAccount[2];

                    if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                        continue;

                    var proxyaddress = string.Empty;
                    var proxyport = string.Empty;
                    var proxyusername = string.Empty;
                    var proxypassword = string.Empty;

                    switch (splitAccount.Length)
                    {
                        case 5:
                            proxyaddress = splitAccount[3];
                            proxyport = splitAccount[4];
                            break;
                        case 7:
                            proxyaddress = splitAccount[3];
                            proxyport = splitAccount[4];
                            proxyusername = splitAccount[5];
                            proxypassword = splitAccount[6];
                            break;
                    }

                    if (splitAccount.Length > 3)
                    {
                        if (string.IsNullOrEmpty(proxyaddress) || string.IsNullOrEmpty(proxyport))
                        {
                            proxyaddress = proxyport = proxyusername = proxypassword = string.Empty;
                        }
                        //valid the proxy ip and port
                        else if (!Proxy.IsValidProxyIp(proxyaddress) || !Proxy.IsValidProxyPort(proxyport))
                            continue;
                    }


                    //var objSingleDominatorAccountModel = new SingleDominatorAccountModel
                    //{
                    //    GroupName = groupname ?? ConstantVariable.UnGrouped,
                    //    UserName = username,
                    //    Password = password,
                    //    AccountProxy =
                    //        {
                    //            ProxyIp = proxyaddress,
                    //            ProxyPort = proxyport,
                    //            ProxyUsername = proxyusername,
                    //            ProxyPassword = proxypassword
                    //        }
                    //};

                    //add the account to DominatorAccountModel list and bin file
                   // AddAccount(objSingleDominatorAccountModel);
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

        //public void AddAccount(SingleDominatorAccountModel objSingleDominatorAccountModel)
        //{

        //    #region Add Account
        //    //check the account is already present or not
        //    if (LstDominatorAccountModel.Any(x => x.UserName == objSingleDominatorAccountModel.UserName))
        //    {
        //        /*INFO*/
        //        Console.WriteLine($@"Account [{objSingleDominatorAccountModel.UserName}] already added!");
        //        GlobusLogHelper.log.Info($@"Account [{objSingleDominatorAccountModel.UserName}] already added!");
        //        return;
        //    }

        //    //Initialize the given account to account model
        //    var objDominatorAccountModel = new DominatorAccountModel
        //    {
        //        RowNo = LstDominatorAccountModel.Count + 1,
        //        AccountGroup =
        //        {
        //            AccountGroupName = objSingleDominatorAccountModel.GroupName ?? ConstantVariable.UnGrouped
        //        },
        //        UserName = objSingleDominatorAccountModel.UserName,
        //        Password = objSingleDominatorAccountModel.Password,
        //        AccountProxy =
        //        {
        //            ProxyIp = objSingleDominatorAccountModel.AccountProxy.ProxyIp,
        //            ProxyPort = objSingleDominatorAccountModel.AccountProxy.ProxyPort,
        //            ProxyUsername = objSingleDominatorAccountModel.AccountProxy.ProxyUsername,
        //            ProxyPassword = objSingleDominatorAccountModel.AccountProxy.ProxyPassword
        //        },
        //        FollowersCount = 0,
        //        FollowingCount = 0,
        //        PostsCount = 0,
        //        Status = ConstantVariable.NotChecked
        //    };

        //    DirectoryUtilities.CreateDirectory(ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram));

        //    //serialize the given account, if its success then add to account model list
        //    if (ProtoBuffBase.SerializeObjects<DominatorAccountModel>(objDominatorAccountModel,
        //        ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}"))
        //    {
        //        LstDominatorAccountModel.Add(objDominatorAccountModel);
        //    }
        //    else
        //    {
        //        /*INFO*/
        //        Console.WriteLine($@"Account [{objDominatorAccountModel.UserName}] isn't saved!");
        //        GlobusLogHelper.log.Info($@"Account [{objDominatorAccountModel.UserName}] isn't saved!");
        //    }

        //    DataBaseHandler.CreateDataBase(objSingleDominatorAccountModel.UserName);

        //    #endregion

        //    #region Login Account And Update Follower And Following Count

        //    UpdateAccount(objDominatorAccountModel);

        //    #endregion
        //}


        //public void UpdateAccount(DominatorAccountModel objDominatorAccountModel)
        //{
        //    CancellationTokenSource cts = new CancellationTokenSource();
        //    CancellationToken ct = cts.Token;

        //    Task.Factory.StartNew(() =>
        //    {
        //        UpdateAccountFollowerFollowing(objDominatorAccountModel);
        //    }, ct);

        //}


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

            DirectoryUtilities.CreateDirectory(ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram));

            //delete the account bin file 
            File.Delete(ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}");

            //remove the selected accounts from account model
            selectAccounts.ForEach(item => LstDominatorAccountModel.Remove(item));

            //after removed serialize the remaining accounts 
            ProtoBuffBase.SerializeListObject<DominatorAccountModel>(LstDominatorAccountModel,
                ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}");
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

            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            var selectedAccount = LstDominatorAccountModel.FirstOrDefault<DominatorAccountModel>(x => selectedRow != null && x.RowNo == selectedRow.RowNo);

            if (selectedAccount == null) return;

            //var objSingleDominatorAccountModel = new SingleDominatorAccountModel
            //{
            //    BtnContent = "Update",
            //    PageTitle = "Update Single Account",
            //    GroupName = selectedAccount.AccountGroup.AccountGroupName,
            //    UserName = selectedAccount.UserName,
            //    Password = selectedAccount.Password,
            //    AccountProxy =
            //    {
            //        ProxyIp = selectedAccount.AccountProxy.ProxyIp,
            //        ProxyPort = selectedAccount.AccountProxy.ProxyPort,
            //        ProxyUsername = selectedAccount.AccountProxy.ProxyUsername,
            //        ProxyPassword = selectedAccount.AccountProxy.ProxyPassword
            //    }
            //};

            //var objSingleAccountControl = new SingleAccountControl(objSingleDominatorAccountModel);

            //var customDialog = new CustomDialog()
            //{
            //    HorizontalAlignment = HorizontalAlignment.Center,
            //    Content = objSingleAccountControl
            //};

            //var objDialog = new Dialog();

            //var dialogWindow = objDialog.GetCustomDialog(customDialog);

            //objSingleAccountControl.btnSave.Click += (senders, events) =>
            //{

            //    if (string.IsNullOrEmpty(objSingleDominatorAccountModel.UserName) ||
            //        string.IsNullOrEmpty(objSingleDominatorAccountModel.Password)) return;

            //    selectedAccount.AccountGroup.AccountGroupName = objSingleDominatorAccountModel.GroupName;
            //    selectedAccount.UserName = objSingleDominatorAccountModel.UserName;
            //    selectedAccount.Password = objSingleDominatorAccountModel.Password;
            //    selectedAccount.AccountProxy.ProxyIp = objSingleDominatorAccountModel.AccountProxy.ProxyIp;
            //    selectedAccount.AccountProxy.ProxyPort = objSingleDominatorAccountModel.AccountProxy.ProxyPort;
            //    selectedAccount.AccountProxy.ProxyUsername = objSingleDominatorAccountModel.AccountProxy.ProxyUsername;
            //    selectedAccount.AccountProxy.ProxyPassword = objSingleDominatorAccountModel.AccountProxy.ProxyPassword;

            //    File.Delete(ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}");

            //    ProtoBuffBase.SerializeListObject<DominatorAccountModel>(LstDominatorAccountModel,
            //        ConstantVariable.GetIndexAccountPath(SocialNetworks.Instagram) + $"//{ConstantVariable.AccountDetails}");

            //    dialogWindow.Close();

            //};
            //objSingleAccountControl.btnCancel.Click += (senders, events) =>
            //{
            //    dialogWindow.Close();
            //};

            //dialogWindow.ShowDialog();
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
            //lock (syncLoadAccounts)
            //{
            //    var savedAccounts = GdBinFileHelper.ReadAccounts();

            //    var allGroups = new List<ContentSelectGroup>();

            //    try
            //    {
            //        LstDominatorAccountModel.Clear();
            //        savedAccounts.ForEach(account =>
            //        {
            //            LstDominatorAccountModel.Add(account);
            //            allGroups.Add(account.AccountGroup);
            //            Global.ScheduleForEachModule(null, account);
            //        });

            //        foreach (var group in allGroups)
            //        {
            //            if (Groups.Any(x => x.Content.Equals
            //                    (group.Content, StringComparison.CurrentCultureIgnoreCase)) == false)
            //                Groups.Add(group);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        /*DEBUG*/
            //        Console.WriteLine(ex.StackTrace);
            //    }
            //}
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        #endregion


    }
}
