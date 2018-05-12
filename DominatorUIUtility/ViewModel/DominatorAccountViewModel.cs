using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using System.Windows.Threading;
using ControlzEx.Standard;
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
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
        public class AccessorStrategies
        {
            public Func<SocialNetworks, bool> _determine_available;
            public Action<string> _inform_warnings;
            public Action<DominatorAccountModel> ActionCheckAccount;
            public Action<DominatorAccountModel> AccountBrowserLogin;
            public Action<DominatorAccountModel> action_UpdateFollower;
        }
        private DataBaseConnectionGlobal DataBaseConnectionGlb { get; set; }

        public DominatorAccountViewModel(AccessorStrategies strategyPack)
        {
            this.strategyPack = strategyPack;

            InitialAccountDetails();
            DataBaseConnectionGlb = DataBaseHandler.GetDataBaseConnectionGlobalInstance();
            #region Command Initialization

            AddSingleAccountCommand = new BaseCommand<object>(AddSingleAccountCanExecute, (o) => AddSingleAccountExecute(o, this.strategyPack._determine_available, this.strategyPack._inform_warnings));

            LoadMultipleAccountsCommand = new BaseCommand<object>(LoadMultipleAccountsCanExecute, (o) => LoadMultipleAccountsExecute(o, this.strategyPack._determine_available, this.strategyPack._inform_warnings));

            InfoCommand = new BaseCommand<object>(InfoCommandCanExecute, InfoCommandExecute);

            ContextMenuOpenCommand = new BaseCommand<object>(OpenContextMenuCanExecute, OpenContextMenuExecute);

            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);

            DeleteAccountsCommand = new BaseCommand<object>(DeleteAccountsCanExecute, DeleteAccountsExecute);

            SelectAccountCommand = new BaseCommand<object>(SelectAccountCanExecute, SelectAccountExecute);

            SelectAccountByStatusCommand = new BaseCommand<object>(SelectAccountByStatusCanExecute, SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new BaseCommand<object>(SelectAccountByGroupCanExecute, SelectAccountByGroupExecute);

            SingleAccountEditCommand = new BaseCommand<object>(SingleAccountEditCanExecute, SingleAccountEditExecute);

            SingleAccountDeleteCommand = new BaseCommand<object>(SingleAccountDeleteCanExecute, SingleAccountDeleteExecute);

            UpdateAccountDetailsCommand = new BaseCommand<object>(UpdateAccountDetailsCanExecute, UpdateAccountDetailsExecute);

            UpdateGroupCommand = new BaseCommand<object>(UpdateGroupDetailsCanExecute, UpdateGroupDetailsExecute);

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

        public ObservableCollection<ContentSelectGroup> Groups
        {
            get
            {
                return _groups;
            }
            set
            {
                _groups = value;
                OnPropertyChanged(nameof(Groups));
            }
        }


        private List<string> _visibleColumns = new List<string>();

        public IEnumerable<string> VisibleColumns
        {
            get
            {
                return _visibleColumns;
            }
            set
            {
                if (value != null && !value.SequenceEqual(_visibleColumns))
                {
                    _visibleColumns = value.ToList();
                    OnPropertyChanged();
                }
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

        private bool _isOpenHelpControl;

        public bool IsOpenHelpControl
        {
            get { return _isOpenHelpControl; }
            set
            {
                if (_isOpenHelpControl == value)
                    return;
                SetProperty(ref _isOpenHelpControl, value);
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
        public ICommand UpdateAccountDetailsCommand { get; set; }

        public ICommand UpdateGroupCommand { get; set; }
        #endregion

        #region Add accounts

        private bool AddSingleAccountCanExecute(object sender) => true;

        private void AddSingleAccountExecute(object sender, Func<SocialNetworks, bool> isNetworkAvailable, Action<string> warn)
        {
            var objDominatorAccountBaseModel = new DominatorAccountBaseModel();

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "Add Account", "Save", false, SocinatorInitialize.ActiveSocialNetwork.ToString());

            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objAddUpdateAccountControl
            };

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog);

            objAddUpdateAccountControl.btnSave.Click += (senders, events) =>
            {
                try
                {
                    if (string.IsNullOrEmpty(objDominatorAccountBaseModel.UserName) ||
                               string.IsNullOrEmpty(objDominatorAccountBaseModel.Password)) return;

                    if ((!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                        string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                        || (string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                            !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))) return;

                    if ((!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                         string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))
                        || (string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                            !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))) return;

                    if (objAddUpdateAccountControl.ComboBoxSocialNetworks.Text.ToString() != objDominatorAccountBaseModel.AccountNetwork.ToString())
                        objDominatorAccountBaseModel.AccountNetwork =
                            (SocialNetworks)Enum.Parse(typeof(SocialNetworks), objAddUpdateAccountControl.ComboBoxSocialNetworks.Text.ToString());

                    dialogWindow.Close();

                    Task.Factory.StartNew(() =>
                    {
                        AddAccount(objDominatorAccountBaseModel, act =>
                        {
                            var th = new Thread(() => act()) { IsBackground = true };
                            th.Start();
                            return () => th.Abort();
                        });
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            };

            objAddUpdateAccountControl.btnCancel.Click += (senders, events) => dialogWindow.Close();

            dialogWindow.ShowDialog();
        }

        private bool LoadMultipleAccountsCanExecute(object sender)
        {
            return true;
        }

        ImmutableQueue<Action> pending = ImmutableQueue<Action>.Empty;
        bool allAccountsQueued;

        /// <summary>
        ///LoadMultipleAccountsExecute is used to load multiple accounts at a time
        ///GroupName:Username:Password:ProxyIp:ProxyPort:ProxyUsername:ProxyPassword
        ///GroupName:Username:Password:ProxyIp:ProxyPort
        ///GroupName:Username:Password
        ///Can load , instead of :
        ///If any values are null, we can use NA        
        /// </summary>
        /// <param name="sender"></param>
        private void LoadMultipleAccountsExecute(object sender, Func<SocialNetworks, bool> isNetworkAvailable, Action<string> warn)
        {
            //Read the accounts from text or csv files
            try
            {

                var loadedAccountlist = FileUtilities.FileBrowseAndReader();

                //if loaded text or csv contains no accounts then return
                if (loadedAccountlist == null) return;

                #region Add to bin files which are valid accounts

                ////add the account to DominatorAccountModel list and bin file
                allAccountsQueued = false;

                try
                {
                    new Thread(() =>
                       {
                           while (!allAccountsQueued)
                           {
                               Thread.Sleep(50);
                               while (!pending.IsEmpty)
                               {
                                   Action act;
                                   pending = pending.Dequeue(out act);
                                   act();
                               }
                           }
                       })
                    { IsBackground = true }.Start();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

                Thread.Sleep(50);
                //Iterate the all accounts one by one
                foreach (var singleAccount in loadedAccountlist)
                {
                    try
                    {
                        var finalAccount = singleAccount.Replace(",", ":").Replace("<NA>", "");
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
                            {
                                GlobusLogHelper.log.Info(Log.ImportFailed, socialNetwork, username, "Proxy address or Proxy port");
                                continue;

                            }
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
                            AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork)
                        };

                        if (isNetworkAvailable(objDominatorAccountBaseModel.AccountNetwork))
                        {
                            pending = pending.Enqueue(() => AddAccount(objDominatorAccountBaseModel,
                                (action) =>
                                {
                                    pending = pending.Enqueue(action);
                                    return () =>
                                    {
                                        var oldqueue = pending;
                                        pending = ImmutableQueue<Action>.Empty;
                                        oldqueue
                                            .Except(new[] { action })
                                            .ForEach(it => pending = pending.Enqueue(it));
                                    };
                                }));
                        }
                        else
                        {
                            warn(string.Format("The account {0} cannot be imported because {1} is not available.",
                                objDominatorAccountBaseModel,
                                objDominatorAccountBaseModel.AccountNetwork));
                        }
                    }
                    catch (Exception ex)
                    {
                        /*INFO*/
                        Console.WriteLine(ex.StackTrace);
                        GlobusLogHelper.log.Error(ex.Message);
                    }
                }

                allAccountsQueued = true;

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AddAccount(DominatorAccountBaseModel objDominatorAccountBaseModel, Func<Action, Action> secondaryTaskStrategyReturningCancellation)
        {
            #region Add Account
            //check the account is already present or not
            if (LstDominatorAccountModel.Any(x => x.AccountBaseModel.UserName == objDominatorAccountBaseModel.UserName && x.AccountBaseModel.AccountNetwork == objDominatorAccountBaseModel.AccountNetwork))
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.AlreadyAddedAccount, objDominatorAccountBaseModel.AccountNetwork, objDominatorAccountBaseModel.UserName);
                return;
            }

            objDominatorAccountBaseModel.AccountGroup.Content = string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content) ? ConstantVariable.UnGrouped : objDominatorAccountBaseModel.AccountGroup.Content;

            //Initialize the given account to account model
            var dominatorAccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup =
                {
                    Content = string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content) ? ConstantVariable.UnGrouped : objDominatorAccountBaseModel.AccountGroup.Content
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
                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork,
                AccountId = objDominatorAccountBaseModel.AccountId
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1,
                AccountId = dominatorAccountBaseModel.AccountId
            };

            var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            dominatorAccountModel.Token.Register(cancel);

            //serialize the given account, if its success then add to account model list
            if (AccountsFileManager.Add(dominatorAccountModel))
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        LstDominatorAccountModel.Add(dominatorAccountModel);
                    }));
                }
                else
                {
                    LstDominatorAccountModel.Add(dominatorAccountModel);
                }

                GlobusLogHelper.log.Info(Log.AddedAccount, objDominatorAccountBaseModel.AccountNetwork, objDominatorAccountBaseModel.UserName);
            }
            else
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.NotAddedAccount, objDominatorAccountBaseModel.AccountNetwork, objDominatorAccountBaseModel.UserName);

            }

            //Testing 
            var accountDetails = AccountsFileManager.GetAll();

            var databaseCreation = secondaryTaskStrategyReturningCancellation(() =>
            {
                IDatabaseConnection databaseConnection = SocinatorInitialize
                    .GetSocialLibrary(objDominatorAccountBaseModel.AccountNetwork).GetNetworkCoreFactory()
                    .AccountDatabase;

                var dbContext = databaseConnection.GetContext(objDominatorAccountBaseModel.AccountId);

                var dbOperations = new DbOperations();

                DataBaseHandler.DbInitialCounters[objDominatorAccountBaseModel.AccountNetwork](dbContext, dbOperations);

                // DataBaseHandler.CreateDataBase(objDominatorAccountBaseModel.AccountId, objDominatorAccountBaseModel.AccountNetwork, DatabaseType.AccountType);

                #region Saving Account detail to AccountDetails database

                DataBaseConnectionGlb.Add<AccountDetails>(new AccountDetails
                {
                    AccountNetwork = objDominatorAccountBaseModel.AccountNetwork.ToString(),
                    AccountId = objDominatorAccountBaseModel.AccountId,
                    AccountGroup = dominatorAccountBaseModel.AccountGroup.Content,
                    UserName = objDominatorAccountBaseModel.UserName,
                    Password = objDominatorAccountBaseModel.Password,
                    UserFullName = objDominatorAccountBaseModel.UserFullName,
                    Status = objDominatorAccountBaseModel.Status,
                    ProxyIP = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUserName = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword,
                    UserAgent = dominatorAccountModel.UserAgentWeb,
                    AddedDate = DateTime.Now
                });

                #endregion
            });
            dominatorAccountModel.Token.Register(databaseCreation);

            #endregion


            try
            {
                var accountFactory = SocinatorInitialize.GetSocialLibrary(objDominatorAccountBaseModel.AccountNetwork)
                    .GetNetworkCoreFactory().AccountUpdateFactory;

                if (typeof(IAccountUpdateFactoryAsync).IsAssignableFrom(accountFactory.GetType()))
                {
                    // this account supports async modules
                    var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;
                    asyncAccount
                        .CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token)
                        .ContinueWith(checkSucceeded =>
                    {
                        if (checkSucceeded.Result)
                        {
                            return asyncAccount.UpdateDetailsAsync(dominatorAccountModel, dominatorAccountModel.Token);
                        }
                        return new Task(() => { });
                    })
                    .Start();
                }
                else
                {
                    // TODO: Add on-deleted cancellation mechanics for non-async modules 
                    var cancelUpdate = secondaryTaskStrategyReturningCancellation(() =>
                    {
                        accountFactory.CheckStatus(dominatorAccountModel);
                        accountFactory.UpdateDetails(dominatorAccountModel);
                    });
                    dominatorAccountModel.Token.Register(cancelUpdate);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public void UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {
            if (string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) && string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                return;
            List<ProxyManagerModel> ProxyDetail = ProxyFileManager.GetAllProxy();

            bool IsProxyAvailable = false;
            foreach (var proxy in ProxyDetail)
            {
                if (objDominatorAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                  && objDominatorAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                {
                    IsProxyAvailable = true;

                    foreach (var proxy2 in ProxyDetail)
                    {
                        var account2 = proxy2.AccountsAssignedto.FirstOrDefault(x => x.UserName == objDominatorAccountBaseModel.UserName);
                        if (account2 != null)
                        {
                            proxy2.AccountsAssignedto.Remove(account2);
                            ProxyFileManager.EditProxy(proxy2);
                            break;
                        }
                    }
                    var account = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == objDominatorAccountBaseModel.UserName);
                    if (account == null)
                    {
                        proxy.AccountsAssignedto.Add(new AccountAssign
                        {
                            UserName = objDominatorAccountBaseModel.UserName,
                            AccountNetwork = objDominatorAccountBaseModel.AccountNetwork
                        });
                        ProxyFileManager.EditProxy(proxy);
                    }

                    break;
                }
            }
            if (!IsProxyAvailable)
            {
                ProxyManagerModel ProxyManagerModel = new ProxyManagerModel
                {
                    AccountProxy ={
                        ProxyName=$"Proxy {ProxyDetail.Count+1 }",
                    ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword
                }
                };

                ProxyManagerModel.AccountsAssignedto.Add(new AccountAssign
                {
                    UserName = objDominatorAccountBaseModel.UserName,
                    AccountNetwork = objDominatorAccountBaseModel.AccountNetwork
                });
                ProxyFileManager.SaveProxy(ProxyManagerModel);
            }
            ProxyDetail = ProxyFileManager.GetAllProxy();
        }




        #endregion

        #region Delete Accounts


        public void DeleteAccountFromProxy(List<DominatorAccountModel> objDominatorAccountBaseModel)
        {
            var allProxy = ProxyFileManager.GetAllProxy();
            allProxy?.ForEach(proxy =>
            {
                try
                {
                    objDominatorAccountBaseModel.ForEach(account =>
                           {
                               var assignedAccount = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName);
                               proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName));
                               proxy.AccountsToBeAssign.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName));


                           });
                }
                catch (Exception ex)
                {
                }
                ProxyFileManager.EditProxy(proxy);
            });
            allProxy = ProxyFileManager.GetAllProxy();
            var v = AccountsFileManager.GetAll();
        }

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

                if (selectAccounts.Count == 0)
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                        "Please select atleast one account !!");
                    return;
                }
                var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete all selected account permanently \nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));
                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                Task.Factory.StartNew(() => { DeleteAccounts(selectAccounts); });

            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void DeleteAccounts(IEnumerable<DominatorAccountModel> selectAccounts)
        {

            //remove the selected accounts from account model
            selectAccounts.ForEach(item =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    LstDominatorAccountModel.Remove(item);
                });
                DataBaseConnectionGlb.Remove<AccountDetails>(user =>
                    user.AccountNetwork == item.AccountBaseModel.AccountNetwork.ToString() &&
                    user.UserName == item.UserName);
                GlobusLogHelper.log.Info(Log.DeleteAccount, item.AccountBaseModel.AccountNetwork, item.AccountBaseModel.UserName);
                item.NotifyDeleted();
            });


            // remove from file
            AccountsFileManager.Delete(x => selectAccounts.FirstOrDefault(a => a.AccountId == x.AccountId) != null);
            DeleteAccountFromProxy(selectAccounts.ToList());

            //also delete the associated files
            DataBaseHandler.DeleteDatabase(selectAccounts.Select(acct => acct.AccountId));

        }

        public void DeleteAccountByContextMenu(object sender)
        {
            var selectedRow = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            var selectedAccount = LstDominatorAccountModel.FirstOrDefault<DominatorAccountModel>(x => selectedRow != null && x.AccountBaseModel.AccountId == selectedRow.AccountBaseModel.AccountId);

            if (selectedAccount == null)
                return;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete all selected account permanently \nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            DeleteAccounts(new[] { selectedAccount });


        }

        #endregion

        #region ContextMenuIsOpen 

        private bool OpenContextMenuCanExecute(object sender) => true;

        private void OpenContextMenuExecute(object sender)
        {


            ContextMenuOpen(sender);
            var button = sender as Button;
            if (button == null || button.Name != "BtnSelect") return;
            var currentGroups = new List<string>();

            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                currentGroups = LstDominatorAccountModel.Select(x => x.AccountBaseModel.AccountGroup.Content).Distinct().ToList();
            else
            {
                currentGroups = LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x => x.AccountBaseModel.AccountGroup.Content).Distinct().ToList();

            }

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
            if (selectedAccounts.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                    "Please select atleast one account !!");
                return;
            }


            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            const string header = "AccountNetwok,Username,Password,Account Group,Status,Proxy Address,Proxy Port,Proxy Username,Proxy Password";

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
                    var csvData = account.AccountBaseModel.AccountNetwork + ","
                    + account.AccountBaseModel.UserName + ","
                    + account.AccountBaseModel.Password + ","
                    + account.AccountBaseModel.AccountGroup.Content + ","
                    + account.AccountBaseModel.Status + ","
                    + account.AccountBaseModel.AccountProxy.ProxyIp + ","
                    + account.AccountBaseModel.AccountProxy.ProxyPort + ","
                    + account.AccountBaseModel.AccountProxy.ProxyUsername + ","
                    + account.AccountBaseModel.AccountProxy.ProxyPassword;

                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(csvData);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            });
        }

        #endregion

        #region Help Methods

        private bool InfoCommandCanExecute(object sender) => true;

        private void InfoCommandExecute(object sender) => IsOpenHelpControl = true;

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


                if ((!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                     string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))
                    || (string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                        !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPort))) return;

                if ((!string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                     string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))
                    || (string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyUsername) &&
                        !string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountProxy.ProxyPassword))) return;

                selectedAccount.AccountBaseModel.AccountGroup.Content = objDominatorAccountBaseModel.AccountGroup.Content;
                selectedAccount.AccountBaseModel.UserName = objDominatorAccountBaseModel.UserName;
                selectedAccount.AccountBaseModel.Password = objDominatorAccountBaseModel.Password;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;
                selectedAccount.AccountBaseModel.AccountNetwork = objDominatorAccountBaseModel.AccountNetwork;

                AccountsFileManager.Edit(selectedAccount);
                UpdateProxy(objDominatorAccountBaseModel);
                GlobusLogHelper.log.Info(Log.AccountEdited, objDominatorAccountBaseModel.AccountNetwork, objDominatorAccountBaseModel.UserName);

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
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                LstDominatorAccountModel.Select(x =>
                {
                    x.IsAccountManagerAccountSelected = true; return x;
                }).ToList();
            }
            else
            {
                LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
                 {
                     x.IsAccountManagerAccountSelected = true; return x;
                 }).ToList();

            }
        }

        public void DeselectAllAccounts()
        {
            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                LstDominatorAccountModel.Select(x =>
                {
                    x.IsAccountManagerAccountSelected = false; return x;
                }).ToList();
            }
            else
            {
                LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
                {
                    x.IsAccountManagerAccountSelected = false; return x;
                }).ToList();
            }
        }

        public void SelectAccount(object sender)
        {

            DeselectAllAccounts();

            var menu = sender as string;

            switch (menu)
            {
                case "Working":
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Success").Select(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                            return x;
                        }).ToList();
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Success" && x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
                       {
                           x.IsAccountManagerAccountSelected = true;
                           return x;
                       }).ToList();
                    }
                    break;
                case "NotWorking":

                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.ForEach(x =>
                        {
                            switch (x.AccountBaseModel.Status)
                            {
                                case "Success":
                                case "Not Checked":
                                case "Logging...":
                                    break;
                                default:
                                    x.IsAccountManagerAccountSelected = true;
                                    break;
                            }
                        });
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(x =>
                         {
                             switch (x.AccountBaseModel.Status)
                             {
                                 case "Success":
                                 case "Not Checked":
                                 case "Logging...":
                                     break;
                                 default:
                                     x.IsAccountManagerAccountSelected = true;
                                     break;
                             }
                         });
                    }
                    break;
                case "NotChecked":
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Not Checked").Select(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                            return x;
                        }).ToList();
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == "Not Checked" && x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                            return x;
                        }).ToList();
                    }
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
        private AccessorStrategies strategyPack;
        private ObservableCollection<ContentSelectGroup> _groups = new ObservableCollection<ContentSelectGroup>();

        public void InitialAccountDetails()
        {
            lock (syncLoadAccounts)
            {
                var accountList = new ObservableCollection<DominatorAccountModel>();
                AccountsFileManager.FillList(accountList);
                var savedAccounts = accountList.ToList();

                try
                {
                    LstDominatorAccountModel.Clear();
                    savedAccounts.ForEach(account =>
                    {
                        LstDominatorAccountModel.Add(account);

                        AccountCollectionView = CollectionViewSource.GetDefaultView(LstDominatorAccountModel);
                        //Global.ScheduleForEachModule(null, account);
                    });
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


        #region Update Account status & details

        ImmutableQueue<Action> _checkPendingList = ImmutableQueue<Action>.Empty;

        bool _allSelectedAccountsQueued;

        private List<string> _updateAccountList = new List<string>();

        private void UpdateAccountDetailsExecute(object sender)
        {
            var selectedAccount = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList();

            if (selectedAccount == null) return;

            if (selectedAccount.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                    "Please select account to update !!");
                return;
            }
            var updateMenuItem = sender as string;


            if (updateMenuItem == "StopProcess")
            {
                StopProcess();
                return;
            }

            try
            {

                _allSelectedAccountsQueued = false;
                new Thread(() =>
                    {
                        while (!_allSelectedAccountsQueued)
                        {
                            Thread.Sleep(50);
                            while (!_checkPendingList.IsEmpty)
                            {
                                Action act;
                                _checkPendingList = _checkPendingList.Dequeue(out act);
                                act();
                            }
                        }
                    })
                { IsBackground = true }.Start();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            Thread.Sleep(50);

            selectedAccount.ForEach(account =>
            {
                var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                    .GetNetworkCoreFactory().AccountUpdateFactory;
                try
                {
                    if (!_updateAccountList.Contains(account.AccountBaseModel.UserName))
                        _checkPendingList = _checkPendingList.Enqueue(() =>
                            MultipleUpdate(account, updateMenuItem, accountFactory));
                    else
                        GlobusLogHelper.log.Info(Log.AlreadyUpdatingAccount, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });

            _allSelectedAccountsQueued = true;
        }

        private void MultipleUpdate(DominatorAccountModel account, string updateMenuItem, IAccountUpdateFactory accountFactory)
        {
            if (typeof(IAccountUpdateFactoryAsync).IsAssignableFrom(accountFactory.GetType()))
            {
                // this account supports async modules
                var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;
                if (updateMenuItem == "UpdateAllDetail")
                {
                    if (account.Token.IsCancellationRequested)
                        account.CancellationSource = new CancellationTokenSource();

                    var updateAccount = new Task(() =>
                    {
                        try
                        {
                            _updateAccountList.Add(account.UserName);
                            account.Token.ThrowIfCancellationRequested();
                            var checkResult = asyncAccount.CheckStatusAsync(account, account.Token).Result;
                            if (checkResult)
                            {
                                account.Token.ThrowIfCancellationRequested();
                                asyncAccount.UpdateDetailsAsync(account, account.Token);
                            }
                        }
                        catch (OperationCanceledException ex)
                        {
                            ex.DebugLog("Cancellation Requested!");
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                    }, account.Token);
                    updateAccount.Start();
                }
                else if (updateMenuItem == "CheckAccountStatus")
                {
                    var checkAccount = new Task(() =>
                    {
                        asyncAccount.CheckStatusAsync(account, account.Token);
                    }, account.Token);
                    checkAccount.Start();
                }

                else if (updateMenuItem == "StopProcess")
                {
                    StopProcess();
                }
            }
        }

        private void StopProcess()
        {
            Task.Factory.StartNew(() =>
            {
                _updateAccountList.ForEach(accountSelected =>
                {
                    var accountFullDetails = LstDominatorAccountModel.FirstOrDefault(x => x.UserName == accountSelected);

                    accountFullDetails?.NotifyDeleted();

                    if (accountFullDetails != null)
                        GlobusLogHelper.log.Info(Log.StopUpdatingAccount, accountFullDetails.AccountBaseModel.AccountNetwork, accountFullDetails.AccountBaseModel.UserName);
                });
                _updateAccountList.Clear();
            });
        }

        private bool UpdateAccountDetailsCanExecute(object sender) => true;
        #endregion


        #region Update Group

        private void UpdateGroupDetailsExecute(object sender)
        {
            lock (syncLoadAccounts)
            {
                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() => { Groups.Clear(); });
                }
                else
                    Groups.Clear();

                var allGroups = new List<ContentSelectGroup>();
                try
                {
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.ForEach(account =>
                        {
                            allGroups.Add(account.AccountBaseModel.AccountGroup);
                        });
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(account =>
                        {
                            allGroups.Add(account.AccountBaseModel.AccountGroup);
                        });
                    }
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

        private bool UpdateGroupDetailsCanExecute(object sender) => true;

        #endregion

        public void AccountBrowserLogin(DominatorAccountModel model)
            => strategyPack.AccountBrowserLogin(model);

        public void ActionCheckAccount(DominatorAccountModel model)
            => strategyPack.ActionCheckAccount(model);

        public void ActionUpdateAccount(DominatorAccountModel model)
            => strategyPack.action_UpdateFollower(model);

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
