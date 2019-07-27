using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Factories;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Command;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.DatabaseHandler.DHTables;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.IoC;
using MahApps.Metro.Controls.Dialogs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prism.Commands;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorUIUtility.Views;
using BindableBase = Prism.Mvvm.BindableBase;
using DominatorUIUtility.ViewModel.Startup;
using Unity;

namespace DominatorUIUtility.ViewModel
{
    public interface IDominatorAccountViewModel
    {
        IAccountCollectionViewModel LstDominatorAccountModel { get; }
    }

    [ProtoContract]
    public class DominatorAccountViewModel : BindableBase, IDominatorAccountViewModel
    {
        private readonly IProxyManagerViewModel _proxyManagerViewModel;
        private readonly ISoftwareSettings _softwareSettings;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IDataBaseHandler _dataBaseHandler;
        private readonly IProxyFileManager _proxyFileManager;

        private bool _IsProgressActive = false;
        public IAccountCollectionViewModel LstDominatorAccountModel { get; }

        public ObservableCollection<ContentSelectGroup> Groups { get; }
        public ISelectedNetworkViewModel SelectedNetworkViewModel { get; }

        private ImmutableQueue<Action> _pendingActions = ImmutableQueue<Action>.Empty;

        private bool _allAccountsQueued;

        public bool IsProgressActive
        {
            get
            {
                return _IsProgressActive;
            }
            set
            {
                if (_IsProgressActive == value)
                    return;
                SetProperty(ref _IsProgressActive, value);
            }
        }

        private readonly object _syncLoadAccounts = new object();

        #region Property
        private string _contactSupportLink = ConstantVariable.ContactSupportLink;

        public string ContactSupportLink
        {
            get { return _contactSupportLink; }
            set { SetProperty(ref _contactSupportLink, value); }
        }
        private string _knowledgeBaseLink = "https://help.socinator.com/support/solutions/folders/42000095344";

        public string KnowledgeBaseLink
        {
            get { return _knowledgeBaseLink; }
            set { SetProperty(ref _knowledgeBaseLink, value); }
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

        private ObservableCollection<GridViewColumn> _visibleColumns;

        public ObservableCollection<GridViewColumn> VisibleColumns
        {
            get { return _visibleColumns; }
            set { SetProperty(ref _visibleColumns, value); }
        }
        IMainViewModel _mainViewModel;

        #region Command 

        public ICommand AddSingleAccountCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand LoadMultipleAccountsCommand { get; }
        public ICommand DeleteAccountsCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SelectAccountCommand { get; }
        public ICommand SelectAccountByStatusCommand { get; }
        public ICommand SelectAccountByGroupCommand { get; }
        public ICommand SingleAccountDeleteCommand { get; }
        public ICommand UpdateAccountDetailsCommand { get; }
        public ICommand UpdateGroupCommand { get; }
        public ICommand UpdateUserCradCommand { get; }
        public ICommand ProfileDetailsCommand { get; }
        public ICommand DeleteAccountCommand { get; }
        public ICommand BrowserLoginCommand { get; }
        public ICommand GotoToolsCommand { get; }
        public ICommand CheckLoginCommand { get; }
        public ICommand UpdateFriendshipCommand { get; }
        public ICommand EditNetworkProfileCommand { get; }
        public ICommand CopyAccountIdCommand { get; }
        public ICommand SettingWizardCommand { get; }
        public ICommand ActivateBrowserAutomationCommand { get; }
        public ICommand DeActivateBrowserAutomationCommand { get; }

        #endregion


        public DominatorAccountViewModel(IMainViewModel mainViewModel, ISelectedNetworkViewModel selectedNetworkViewModel, IProxyManagerViewModel proxyManagerViewModel, ISoftwareSettings softwareSettings, IAccountsFileManager accountsFileManager, IAccountCollectionViewModel accountCollectionViewModel, IDataBaseHandler dataBaseHandler, IProxyFileManager proxyFileManager)
        {
            _mainViewModel = mainViewModel;
            SelectedNetworkViewModel = selectedNetworkViewModel;
            _proxyManagerViewModel = proxyManagerViewModel;
            _softwareSettings = softwareSettings;
            _accountsFileManager = accountsFileManager;
            strategyPack = mainViewModel.Strategies;
            Groups = new ObservableCollection<ContentSelectGroup>();
            BindingOperations.EnableCollectionSynchronization(Groups, AccountCollectionViewModel.SyncObject);
            LstDominatorAccountModel = accountCollectionViewModel;
            _dataBaseHandler = dataBaseHandler;
            _proxyFileManager = proxyFileManager;


            BindingOperations.EnableCollectionSynchronization(LstDominatorAccountModel, AccountCollectionViewModel.SyncObject);

            var visibleHeaders = DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider().VisibleHeaders;
            VisibleColumns = new ObservableCollection<GridViewColumn>(visibleHeaders.Select((name, colIndex) => new GridViewColumn
            {
                DisplayMemberBinding = new Binding($"DisplayColumnValue{colIndex + 1}"),
                Header = name,
                Width = 130
            }));
            BindingOperations.EnableCollectionSynchronization(VisibleColumns, AccountCollectionViewModel.SyncObject);

            InitialAccountDetails();

            #region Command Initialization

            AddSingleAccountCommand = new DelegateCommand(AddSingleAccountExecute);

            LoadMultipleAccountsCommand = new DelegateCommand(() => LoadMultipleAccountsExecute(strategyPack._determine_available, strategyPack._inform_warnings));

            InfoCommand = new DelegateCommand(InfoCommandExecute);

            ExportCommand = new DelegateCommand(ExportExecute);

            DeleteAccountsCommand = new DelegateCommand(DeleteAccountsExecute);

            SelectAccountCommand = new DelegateCommand<bool?>(SelectAccountExecute);

            SelectAccountByStatusCommand = new DelegateCommand<string>(SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new DelegateCommand<ContentSelectGroup>(SelectAccountByGroupExecute);

            SingleAccountDeleteCommand = new DelegateCommand<DominatorAccountModel>(SingleAccountDeleteExecute);

            UpdateAccountDetailsCommand = new BaseCommand<object>(UpdateAccountDetailsCanExecute, UpdateAccountDetailsExecute);

            UpdateGroupCommand = new DelegateCommand(UpdateGroupDetailsExecute);

            UpdateUserCradCommand = new BaseCommand<object>((sender) => true, UpdateUserCradExecute);

            ActivateBrowserAutomationCommand = new BaseCommand<object>(ActivateBrowserAutomationCanExecute, ActivateBrowserAutomationExecute);

            DeActivateBrowserAutomationCommand = new BaseCommand<object>(DeActivateBrowserAutomationCommandCanExecute, DeActivateBrowserAutomationCommandExecute);

            #region Context Menu Command

            ProfileDetailsCommand = new DelegateCommand<DominatorAccountModel>(ProfileDetails);
            DeleteAccountCommand = new DelegateCommand<DominatorAccountModel>(DeleteAccountByContextMenu);
            BrowserLoginCommand = new DelegateCommand<DominatorAccountModel>(AccountBrowserLogin);
            GotoToolsCommand = new DelegateCommand<DominatorAccountModel>(GotoTools);
            CheckLoginCommand = new DelegateCommand<DominatorAccountModel>(AccountStatusChecker);
            UpdateFriendshipCommand = new DelegateCommand<DominatorAccountModel>(AccountUpdate);
            EditNetworkProfileCommand = new DelegateCommand<DominatorAccountModel>(EditProfile);
            CopyAccountIdCommand = new DelegateCommand<DominatorAccountModel>(CopyAccountId);

            #endregion

            #endregion

            #region Custom Setting Command

            SettingWizardCommand = new DelegateCommand<DominatorAccountModel>(CustomSetting);

            #endregion

            SelectedNetworkViewModel.ItemSelected += SelectedNetworkViewModel_ItemSelected;
        }

        private void CustomSetting(DominatorAccountModel account)
        {
            var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
            viewModel.SelectedNetwork = account.AccountBaseModel.AccountNetwork.ToString();
            viewModel.SelectAccount = account;
            ModuleSetting.Instance.Show();
        }

        private void SelectedNetworkViewModel_ItemSelected(object sender, SocialNetworks? e)
        {
            if (e.HasValue)
            {
                var spec = (e == SocialNetworks.Social) ?
                    DominatorAccountCountFactory.Instance.GetColumnSpecificationProvider() :
                    SocinatorInitialize.GetSocialLibrary(e.Value)
                        .GetNetworkCoreFactory()
                        .AccountCountFactory.GetColumnSpecificationProvider();
                lock (_syncLoadAccounts)
                {
                    VisibleColumns.Clear();
                    VisibleColumns.AddRange(spec.VisibleHeaders.Select((name, colIndex) => new GridViewColumn
                    {
                        DisplayMemberBinding = new Binding($"DisplayColumnValue{colIndex + 1}"),
                        Header = name,
                        Width = 130
                    }));
                }
            }
        }

        #region Add accounts


        private void AddSingleAccountExecute()
        {
            var objDominatorAccountBaseModel = new DominatorAccountBaseModel();

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "LangKeyAddAccount".FromResourceDictionary(), "LangKeySave".FromResourceDictionary(), false, SocinatorInitialize.ActiveSocialNetwork);

            var customDialog = new CustomDialog()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Content = objAddUpdateAccountControl
            };

            objDominatorAccountBaseModel.AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks),
                    objAddUpdateAccountControl.ComboBoxSocialNetworks.Text);

            var objDialog = new Dialog();
            var dialogWindow = objDialog.GetCustomDialog(customDialog, "LangKeyAddAccount".FromResourceDictionary());

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


                    objDominatorAccountBaseModel.Status = AccountStatus.NotChecked;
                    dialogWindow.Close();

                    if (LstDominatorAccountModel.Count + 1 >=
                        SocinatorInitialize.MaximumAccountCount)
                    {
                        GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                    }

                    ThreadFactory.Instance.Start(() =>
                    {
                        AddAccount(objDominatorAccountBaseModel, String.Empty, act =>
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

        /// <summary>
        ///LoadMultipleAccountsExecute is used to load multiple accounts at a time
        ///GroupName:Username:Password:ProxyIp:ProxyPort:ProxyUsername:ProxyPassword
        ///GroupName:Username:Password:ProxyIp:ProxyPort
        ///GroupName:Username:Password
        ///Can load , instead of :
        ///If any values are null, we can use NA        
        /// </summary>
        /// <param name="sender"></param>
        private void LoadMultipleAccountsExecute(Func<SocialNetworks, bool> isNetworkAvailable, Action<string> warn)
        {
            //Read the accounts from text or csv files
            try
            {

                var loadedAccountlist = FileUtilities.FileBrowseAndReader();

                //if loaded text or csv contains no accounts then return
                if (loadedAccountlist == null || loadedAccountlist.Count == 0) return;

                #region Add to bin files which are valid accounts

                ////add the account to DominatorAccountModel list and bin file
                _allAccountsQueued = false;


                if (loadedAccountlist.Count + LstDominatorAccountModel.Count >
                    SocinatorInitialize.MaximumAccountCount)
                {
                    GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                }

                try
                {
                    new Thread(() =>
                       {
                           while (!_allAccountsQueued)
                           {
                               Thread.Sleep(50);
                               while (!_pendingActions.IsEmpty)
                               {
                                   Action act;
                                   _pendingActions = _pendingActions.Dequeue(out act);
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
                        #region Getting Account Details from loadedAccountlist

                        // var finalAccount = singleAccount.Replace(",", ":").Replace("<NA>", "");
                        var finalAccount = singleAccount.Replace("<NA>", "\t");
                        var splitAccount = Regex.Split(finalAccount.TrimEnd(), "\t");
                        //var splitAccount = Regex.Split(finalAccount, ":");
                        if (splitAccount.Length <= 1) continue;

                        //assign the username, password and groupname
                        var groupname = splitAccount[0];

                        var socialNetwork = splitAccount[1];
                        if (socialNetwork == "AccountNetwork" || socialNetwork == SocialNetworks.Social.ToString()) continue;
                        var username = splitAccount[2];
                        var password = splitAccount[3];

                        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                            continue;

                        var proxyaddress = string.Empty;
                        var proxyport = string.Empty;
                        var proxyusername = string.Empty;
                        var proxypassword = string.Empty;
                        var status = AccountStatus.NotChecked.ToString();
                        var cookies = string.Empty;
                        var alternetEmail = string.Empty;
                        var banned = string.Empty;

                        switch (splitAccount.Length)
                        {
                            case 5:
                                alternetEmail = splitAccount[4];
                                break;
                            case 6:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                break;
                            case 7:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                alternetEmail = splitAccount[6];
                                break;
                            case 8:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                break;
                            case 9:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                alternetEmail = splitAccount[8];
                                break;
                            case 10:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                break;
                            case 11:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                break;
                            case 12:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
                                alternetEmail = splitAccount[10];
                                banned = splitAccount[11];
                                break;
                        }

                        if (splitAccount.Length > 4)
                        {
                            if (string.IsNullOrEmpty(proxyaddress) || string.IsNullOrEmpty(proxyport))
                            {
                                proxyaddress = proxyport = proxyusername = proxypassword = string.Empty;
                            }
                            //valid the proxy ip and port
                            //else if (!Proxy.IsValidProxyIp(proxyaddress) || !Proxy.IsValidProxyPort(proxyport))
                            //{
                            //    GlobusLogHelper.log.Info(Log.ImportFailed, socialNetwork, username, "Proxy address or Proxy port");
                            //    continue;

                            //}
                        }

                        #endregion

                        #region Creating new Account with Account Details

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
                            AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork),
                            Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), status),
                            AlternateEmail = alternetEmail,
                            Banned = banned
                        };

                        #endregion

                        if (isNetworkAvailable(objDominatorAccountBaseModel.AccountNetwork))
                        {

                            if (SocinatorInitialize.ActiveSocialNetwork == objDominatorAccountBaseModel.AccountNetwork || SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                            {
                                _pendingActions = _pendingActions.Enqueue(() => AddAccount(objDominatorAccountBaseModel, cookies,
                                                                              (action) =>
                                                                              {
                                                                                  _pendingActions = _pendingActions.Enqueue(action);
                                                                                  return () =>
                                                                                  {
                                                                                      var oldqueue = _pendingActions;
                                                                                      _pendingActions = ImmutableQueue<Action>.Empty;
                                                                                      oldqueue
                                                                                          .Except(new[] { action })
                                                                                          .ForEach(it => _pendingActions = _pendingActions.Enqueue(it));
                                                                                  };
                                                                              }));
                            }
                        }
                        else
                        {
                            warn(string.Format("The account {0} cannot be imported because {1} is not available.",
                                objDominatorAccountBaseModel,
                                objDominatorAccountBaseModel.AccountNetwork));
                            GlobusLogHelper.log.Info(SocinatorInitialize.ActiveSocialNetwork + "\tThe account {0} cannot be imported because {1} is not available.",
                                objDominatorAccountBaseModel.UserName,
                                objDominatorAccountBaseModel.AccountNetwork);
                        }
                    }
                    catch (Exception ex)
                    {
                        /*INFO*/
                        Console.WriteLine(ex.StackTrace);
                        ex.DebugLog();
                    }
                }

                _allAccountsQueued = true;

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void AddAccount(DominatorAccountBaseModel objDominatorAccountBaseModel, string cookies,
            Func<Action, Action> secondaryTaskStrategyReturningCancellation)
        {
            #region Check account limits

            if (LstDominatorAccountModel.Count >= SocinatorInitialize.MaximumAccountCount)
            {
                return;
            }

            #endregion

            #region Add Account

            //check the account is already present or not
            if (LstDominatorAccountModel.Any(x =>
                x.AccountBaseModel.UserName == objDominatorAccountBaseModel.UserName &&
                x.AccountBaseModel.AccountNetwork == objDominatorAccountBaseModel.AccountNetwork))
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.AlreadyAddedAccount, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName);
                return;
            }

            objDominatorAccountBaseModel.AccountGroup.Content =
                string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content)
                    ? ConstantVariable.UnGrouped
                    : objDominatorAccountBaseModel.AccountGroup.Content;

            //Initialize the given account to account model
            var dominatorAccountBaseModel = new DominatorAccountBaseModel
            {
                AccountGroup =
                {
                    Content = string.IsNullOrEmpty(objDominatorAccountBaseModel.AccountGroup.Content)
                        ? ConstantVariable.UnGrouped
                        : objDominatorAccountBaseModel.AccountGroup.Content
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
                Status = string.IsNullOrEmpty(objDominatorAccountBaseModel.Status.ToString())
                    ? AccountStatus.NotChecked : objDominatorAccountBaseModel.Status,
                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork,
                AccountId = objDominatorAccountBaseModel.AccountId,
                IsChkTwoFactorLogin = objDominatorAccountBaseModel.IsChkTwoFactorLogin,
                AlternateEmail = objDominatorAccountBaseModel.AlternateEmail
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1,
                AccountId = dominatorAccountBaseModel.AccountId
            };
            if (!string.IsNullOrEmpty(cookies)/* && dominatorAccountModel.AccountBaseModel.AccountNetwork != SocialNetworks.Youtube*/)
                try
                {
                    dominatorAccountModel.CookieHelperList = JArray.Parse(cookies.Replace("<>", ",")).ToObject<HashSet<CookieHelper>>();
                }
                catch (Exception ex)
                {

                }
            List<ProxyManagerModel> oldproxies = _proxyFileManager.GetAllProxy();

            //var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            //dominatorAccountModel.Token.Register(cancel);
            if (!String.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                !String.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyPort))
            {
                if (!IsDuplicatProxyAvailable(objDominatorAccountBaseModel, oldproxies, null))
                {
                    if (!UpdateProxy(dominatorAccountBaseModel))
                        AddProxyIfNotExist(dominatorAccountBaseModel);
                }
            }

            var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            dominatorAccountModel.Token.Register(cancel);


            //serialize the given account, if its success then add to account model list
            if (_accountsFileManager.Add(dominatorAccountModel))
            {

                if (!Application.Current.Dispatcher.CheckAccess())
                {
                    Application.Current.Dispatcher.Invoke(() => LstDominatorAccountModel.Add(dominatorAccountModel));
                }
                else
                {
                    LstDominatorAccountModel.Add(dominatorAccountModel);
                }

                GlobusLogHelper.log.Info(Log.Added, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName, "LangKeyAccounts".FromResourceDictionary());
            }
            else
            {
                /*INFO*/
                GlobusLogHelper.log.Info(Log.NotAddedAccount, objDominatorAccountBaseModel.AccountNetwork,
                    objDominatorAccountBaseModel.UserName);

            }

            //Testing 
            var databaseCreation = secondaryTaskStrategyReturningCancellation(() =>
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                #region Saving Account detail to AccountDetails database

                globalDbOperation.Add(new AccountDetails
                {
                    AccountNetwork = objDominatorAccountBaseModel.AccountNetwork.ToString(),
                    AccountId = objDominatorAccountBaseModel.AccountId,
                    AccountGroup = dominatorAccountBaseModel.AccountGroup.Content,
                    UserName = objDominatorAccountBaseModel.UserName,
                    Password = objDominatorAccountBaseModel.Password,
                    UserFullName = objDominatorAccountBaseModel.UserFullName,
                    Status = objDominatorAccountBaseModel.Status.ToString(),
                    ProxyIP = objDominatorAccountBaseModel.AccountProxy.ProxyIp,
                    ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort,
                    ProxyUserName = objDominatorAccountBaseModel.AccountProxy.ProxyUsername,
                    ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword,
                    UserAgent = dominatorAccountModel.UserAgentWeb,
                    AddedDate = DateTime.Now,
                    Cookies = cookies
                });

                #endregion
            });
            dominatorAccountModel.Token.Register(databaseCreation);

            #endregion

            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!softwareSettings.IsDoNotAutoLoginAccountsWhileAddingToSoftware)
            {
                try
                {
                    var accountFactory = SocinatorInitialize.GetSocialLibrary(objDominatorAccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;

                    if (typeof(IAccountUpdateFactoryAsync).IsAssignableFrom(accountFactory.GetType()))
                    {
                        // this account supports async modules
                        var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;

                        try
                        {
                            asyncAccount
                                .CheckStatusAsync(dominatorAccountModel, dominatorAccountModel.Token)
                                .ContinueWith(checkSucceeded =>
                                {
                                    try
                                    {
                                        if (checkSucceeded.Result)
                                        {
                                            return asyncAccount.UpdateDetailsAsync(dominatorAccountModel,
                                                dominatorAccountModel.Token);
                                        }

                                        return new Task(() => { });
                                    }
                                    catch (OperationCanceledException)
                                    {
                                        return new Task(() => { });
                                    }
                                    catch (AggregateException ae)
                                    {
                                        foreach (var e in ae.InnerExceptions)
                                        {
                                            if (e is TaskCanceledException || e is OperationCanceledException)
                                                e.DebugLog("Cancellation requested before task completion!");
                                            else
                                                e.DebugLog(e.StackTrace + e.Message);
                                        }

                                        return new Task(() => { });
                                    }
                                    catch (Exception)
                                    {
                                        return new Task(() => { });
                                    }
                                })
                                .Start();
                        }
                        catch (OperationCanceledException)
                        {
                            throw new OperationCanceledException();
                        }
                        catch (AggregateException ae)
                        {
                            foreach (var e in ae.InnerExceptions)
                            {
                                if (e is TaskCanceledException || e is OperationCanceledException)
                                    e.DebugLog("Cancellation requested before task completion!");
                                else
                                    e.DebugLog(e.StackTrace + e.Message);
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
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

        }
        public bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {
            var oldproxies = _proxyFileManager.GetAllProxy();

            bool isProxyUpdated = false;
            try
            {
                var oldAccount = _accountsFileManager.GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork).AccountBaseModel;

                isProxyUpdated = IsProxyUpdated(objDominatorAccountBaseModel, oldproxies, oldAccount);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isProxyUpdated;
        }
        private bool IsDuplicatProxyAvailable(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> oldProxies,
         DominatorAccountBaseModel oldAccount)
        {
            bool isDuplicatProxyAvailable = false;
            foreach (var proxy in oldProxies)
            {
                #region To check for duplicate proxy 
                try
                {

                    if (objAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                        && objAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                    {
                        #region If other proxy with same ip/port is present
                        try
                        {
                            if (string.IsNullOrEmpty(proxy.AccountProxy.ProxyUsername) || proxy.AccountProxy.ProxyUsername != objAccountBaseModel.AccountProxy.ProxyUsername)
                                proxy.AccountProxy.ProxyUsername = objAccountBaseModel.AccountProxy.ProxyUsername;

                            if (string.IsNullOrEmpty(proxy.AccountProxy.ProxyPassword) || proxy.AccountProxy.ProxyPassword != objAccountBaseModel.AccountProxy.ProxyPassword)
                                proxy.AccountProxy.ProxyPassword = objAccountBaseModel.AccountProxy.ProxyPassword;

                            objAccountBaseModel.AccountProxy = proxy.AccountProxy;

                            var accountTomodified = new AccountAssign
                            {
                                UserName = objAccountBaseModel.UserName,
                                AccountNetwork = objAccountBaseModel.AccountNetwork
                            };

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    if (oldAccount != null)
                                    {
                                        oldProxies.ForEach(pr =>
                                        {

                                            var accountToRemove = pr.AccountsAssignedto.FirstOrDefault(acc =>
                                                acc.UserName == oldAccount.UserName &&
                                                acc.AccountNetwork == oldAccount.AccountNetwork);

                                            if (accountToRemove != null)
                                            {
                                                pr.AccountsAssignedto.Remove(accountToRemove);
                                                _proxyFileManager.EditProxy(pr);
                                            }

                                        });
                                        var proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == oldAccount.AccountProxy.ProxyIp
                                                                                                                                        && x.AccountProxy.ProxyPort == oldAccount.AccountProxy.ProxyPort);
                                        // ReSharper disable once ConstantConditionalAccessQualifier
                                        proxyToUpdate?.AccountsAssignedto.Remove(proxyToUpdate?.AccountsAssignedto.FirstOrDefault(x => x.UserName == oldAccount.UserName && x.AccountNetwork == oldAccount.AccountNetwork));

                                        proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
                                                                                                                                    && x.AccountProxy.ProxyPort == objAccountBaseModel.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Add(
                                            new AccountAssign
                                            {
                                                UserName = objAccountBaseModel.UserName,
                                                AccountNetwork = objAccountBaseModel.AccountNetwork
                                            });

                                    }
                                    else
                                    {
                                        var proxyToUpdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
                                                                                                                                        && x.AccountProxy.ProxyPort == objAccountBaseModel.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Add(
                                            new AccountAssign
                                            {
                                                UserName = objAccountBaseModel.UserName,
                                                AccountNetwork = objAccountBaseModel.AccountNetwork
                                            });
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }


                                proxy.AccountsAssignedto.Add(accountTomodified);

                                _proxyFileManager.EditProxy(proxy);
                                _proxyManagerViewModel.AccountsAlreadyAssigned.Add(
                                    new AccountAssign
                                    {
                                        UserName = accountTomodified.UserName,
                                        AccountNetwork = accountTomodified.AccountNetwork
                                    });
                            });



                            isDuplicatProxyAvailable = true;
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        break;
                        // }


                        #endregion

                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog(ex.Message);
                }

                #endregion
            }

            return isDuplicatProxyAvailable;
        }
        private bool IsProxyUpdated(DominatorAccountBaseModel objDominatorAccountBaseModel, List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount)
        {
            bool isProxyUpdated = false;
            foreach (var proxy in oldProxies)
            {
                #region If old proxy for account is updated

                if (objDominatorAccountBaseModel.AccountProxy.ProxyIp != oldAccount.AccountProxy.ProxyIp
                    || objDominatorAccountBaseModel.AccountProxy.ProxyPort != oldAccount.AccountProxy.ProxyPort)
                {
                    if (objDominatorAccountBaseModel.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                    {
                        isProxyUpdated = true;
                        proxy.AccountProxy.ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp;
                        proxy.AccountProxy.ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort;
                        proxy.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                        proxy.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;

                        _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                        UpdateProxyList(proxy);
                        _proxyFileManager.EditProxy(proxy);
                        break;
                    }
                }

                #endregion

                #region To check proxy is Exist or not

                if (objDominatorAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                    && objDominatorAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                {
                    #region If other proxy with same ip/port not there
                    if (objDominatorAccountBaseModel.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                    {

                        if (objDominatorAccountBaseModel.AccountProxy.ProxyUsername != proxy.AccountProxy.ProxyUsername
                            || objDominatorAccountBaseModel.AccountProxy.ProxyPassword != proxy.AccountProxy.ProxyPassword)
                        {
                            proxy.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                            proxy.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;
                            _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                            UpdateProxyList(proxy);
                            //  ProxyFileManager.EditProxy(proxy);
                        }

                        var account = proxy.AccountsAssignedto.FirstOrDefault(x =>
                            x.UserName == objDominatorAccountBaseModel.UserName);
                        if (account == null)
                        {
                            #region Add account to AccountsAssignedto list if current proxy is not Assigned to current Account

                            proxy.AccountsAssignedto.Add(new AccountAssign
                            {
                                UserName = objDominatorAccountBaseModel.UserName,
                                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork
                            });

                            #endregion

                            _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                            isProxyUpdated = true;
                        }

                        break;
                    }

                    #endregion

                }

                #endregion

            }

            return isProxyUpdated;
        }


        private void UpdateProxyList(ProxyManagerModel proxy)
        {
            try
            {

                var proxyToupdate = _proxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x =>
                    x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                if (proxyToupdate != null)
                    proxyToupdate.AccountProxy = proxy.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void AddProxyIfNotExist(DominatorAccountBaseModel objAccount)
        {

            ProxyManagerModel ProxyManagerModel = new ProxyManagerModel
            {
                AccountProxy =
                    {
                        ProxyName = $"Proxy {objAccount.AccountProxy.ProxyIp.Replace(".","")}{objAccount.AccountProxy.ProxyPort}",
                        ProxyId = objAccount.AccountProxy.ProxyId,
                        ProxyIp = objAccount.AccountProxy.ProxyIp,
                        ProxyPort = objAccount.AccountProxy.ProxyPort,
                        ProxyUsername = objAccount.AccountProxy.ProxyUsername,
                        ProxyPassword = objAccount.AccountProxy.ProxyPassword
                    }
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                _proxyManagerViewModel.LstProxyManagerModel.ForEach(x =>
                {
                    if (x.AccountsAssignedto.Any(y => y.UserName == objAccount.UserName &&
                                                      y.AccountNetwork == objAccount.AccountNetwork))
                        x.AccountsAssignedto.Remove(x.AccountsAssignedto.FirstOrDefault(y =>
                            y.UserName == objAccount.UserName &&
                            y.AccountNetwork == objAccount.AccountNetwork));
                });
                _proxyManagerViewModel.LstProxyManagerModel.Add(ProxyManagerModel);
                ProxyManagerModel.Index = _proxyManagerViewModel.LstProxyManagerModel.IndexOf(ProxyManagerModel) + 1;
                _proxyManagerViewModel.AccountsAlreadyAssigned.Add(
                    new AccountAssign
                    {
                        UserName = objAccount.UserName,
                        AccountNetwork = objAccount.AccountNetwork
                    });
            }
            );

            ProxyManagerModel.AccountsAssignedto.Add(new AccountAssign
            {
                UserName = objAccount.UserName,
                AccountNetwork = objAccount.AccountNetwork
            });

            _proxyFileManager.SaveProxy(ProxyManagerModel);

            _proxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);



        }

        #endregion

        #region Delete Accounts


        public void DeleteAccountFromProxy(List<DominatorAccountModel> objAccountBaseModel)
        {
            var allProxy = _proxyFileManager.GetAllProxy();
            ThreadFactory.Instance.Start(() =>
            {
                allProxy?.ForEach(proxy =>
                {
                    try
                    {
                        objAccountBaseModel.ForEach(account =>
                        {
                            _proxyManagerViewModel.AccountsAlreadyAssigned.Remove(_proxyManagerViewModel.AccountsAlreadyAssigned.FirstOrDefault(x =>
                                x.UserName == account.UserName
                                && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x =>
                                x.UserName == account.UserName
                                && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    var proxyToupdate = _proxyManagerViewModel.LstProxyManagerModel
                                                               .FirstOrDefault(pr => pr.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                                    proxyToupdate?.AccountsAssignedto.Remove(proxyToupdate.AccountsAssignedto.FirstOrDefault(x =>
                                        x.UserName == account.UserName
                                        && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
                            });

                            // proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x => x.UserName == account.UserName));

                        });
                    }
                    catch (Exception ex)
                    {
                        ex.ErrorLog();
                    }

                    _proxyFileManager.EditProxy(proxy);
                });
            });

        }

        private void SingleAccountDeleteExecute(DominatorAccountModel selectedRow)
        {
            DeleteAccountByContextMenu(selectedRow);
        }
        List<DominatorAccountModel> GetSelectedAccount()
        {
            return LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected && (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social || x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork)).ToList();
        }
        private void DeleteAccountsExecute()
        {
            try
            {
                //collect the selected account
                var selectAccounts = GetSelectedAccount();

                if (selectAccounts.Count == 0)
                {
                    Dialog.ShowDialog("Alert", "Please select atleast one account !!");
                    return;
                }
                var dialogResult = Dialog.ShowCustomDialog("Confirmation", "If you delete it will delete all selected account permanently \nAre you sure ?", "Delete Anyways", "Don't delete");
                if (dialogResult != MessageDialogResult.Affirmative)
                    return;

                // ThreadFactory.Instance.Start(() => { DeleteAccounts(selectAccounts); });
                DeleteAccounts(selectAccounts);

            }
            catch (Exception ex)
            {
                /*INFO*/
                Console.WriteLine(ex.StackTrace);
            }
        }

        private void DeleteAccounts(IEnumerable<DominatorAccountModel> selectAccounts)
        {
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                var _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                selectAccounts.ToList().ForEach(item =>
                {
                    LstDominatorAccountModel.Remove(
                              LstDominatorAccountModel.FirstOrDefault(x => x.AccountId == item.AccountId));
                    Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            var network = item.AccountBaseModel.AccountNetwork.ToString();

                            _dbOperations.RemoveMatch<AccountDetails>(user => user.UserName == item.UserName && user.AccountNetwork == network);

                            item.NotifyCancelled();
                        }
                        catch { }
                    });
                });

            });

            // remove from file
            DeleteAccountFromCampaign(selectAccounts);

            DeleteAccountFromProxy(selectAccounts.ToList());

            //also delete the associated files
            _dataBaseHandler.DeleteDatabase(selectAccounts.Select(acct => acct.AccountId));

        }

        private void DeleteAccountFromCampaign(IEnumerable<DominatorAccountModel> selectAccounts)
        {
            Task.Factory.StartNew(() =>
            {
                foreach (var account in selectAccounts)
                {
                    try
                    {
                        var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                        var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                        var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                        foreach (var moduleConfiguration in jobActivityConfigurationManager[account.AccountId].ToList())
                        {
                            dominatorScheduler.StopActivity(account, moduleConfiguration.ActivityType.ToString(), moduleConfiguration.TemplateId, false);
                            if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                            {
                                campaignFileManager.DeleteSelectedAccount(moduleConfiguration.TemplateId, account.AccountBaseModel.UserName);
                                Application.Current.Dispatcher.Invoke(() =>
                                {
                                    var campToUpdate = Campaigns.GetCampaignsInstance(account.AccountBaseModel.AccountNetwork).CampaignViewModel.LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId);
                                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);
                                });

                            }
                        }
                        //Remove Account from Account bin file
                        _accountsFileManager.Delete(x => x.AccountId == account.AccountId);

                        GlobusLogHelper.log.Info(Log.Deleted, account.AccountBaseModel.AccountNetwork,
                            account.AccountBaseModel.UserName, "LangKeyAccounts".FromResourceDictionary());

                        Thread.Sleep(5);
                    }
                    catch (Exception ex)
                    {


                    }
                }
            });
        }
        private void DeleteAccountFromCampaign(DominatorAccountModel account)
        {
            // account = _accountsFileManager.GetAccountById(account.AccountId);
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            foreach (var moduleConfiguration in jobActivityConfigurationManager[account.AccountId])
            {
                dominatorScheduler.StopActivity(account, moduleConfiguration.ActivityType.ToString(), moduleConfiguration.TemplateId, false);
                if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    campaignFileManager.DeleteSelectedAccount(moduleConfiguration.TemplateId, account.AccountBaseModel.UserName);
                    var campToUpdate = Campaigns.GetCampaignsInstance(account.AccountBaseModel.AccountNetwork).CampaignViewModel.LstCampaignDetails.FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId);
                    campToUpdate?.SelectedAccountList.Remove(account.AccountBaseModel.UserName);
                }
            }
        }

        public void DeleteAccountByContextMenu(DominatorAccountModel selectedRow)
        {
            var selectedAccount = LstDominatorAccountModel.FirstOrDefault(x => selectedRow != null && x.AccountBaseModel.AccountId == selectedRow.AccountBaseModel.AccountId);

            if (selectedAccount == null)
                return;
            var dialogResult = DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Confirmation", "If you delete it will delete all selected account permanently \nAre you sure ?", MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Delete Anyways", "Don't delete"));
            if (dialogResult != MessageDialogResult.Affirmative)
                return;
            DeleteAccounts(new[] { selectedAccount });
        }

        #endregion

        #region Export Accounts

        private void ExportExecute()
        {
            var selectedAccounts = GetSelectedAccount();
            if (selectedAccounts.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select atleast one account !!");
                return;
            }

            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            const string header = "Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies,Alternate Email (For YouTube/Gplus),Banned";

            var filename = $"{exportPath}\\{SocinatorInitialize.ActiveSocialNetwork}_Accounts {ConstantVariable.DateasFileName}.csv";

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
                    var csvData =
                     account.AccountBaseModel.AccountGroup.Content + ","
                     + account.AccountBaseModel.AccountNetwork + ","
                     + account.AccountBaseModel.UserName + ","
                     + account.AccountBaseModel.Password + ","
                     + account.AccountBaseModel.AccountProxy.ProxyIp + ","
                     + account.AccountBaseModel.AccountProxy.ProxyPort + ","
                     + account.AccountBaseModel.AccountProxy.ProxyUsername + ","
                     + account.AccountBaseModel.AccountProxy.ProxyPassword + ","
                     + account.AccountBaseModel.Status + ","
                     + JsonConvert.SerializeObject(account.CookieHelperList).Replace(",", "<>") + ","
                     + account.AccountBaseModel.AlternateEmail + ","
                     + account.AccountBaseModel.Banned;

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
            Dialog.ShowDialog("Export Accounts", $"Accounts Successfully exported to [ {filename} ]");
        }

        #endregion

        #region Help Methods

        private void InfoCommandExecute() => IsOpenHelpControl = true;

        #endregion

        #region Select Accounts


        private void SelectAccountExecute(bool? sender)
        {
            SelectAllAccounts(sender ?? false);
        }

        private void SelectAccountByStatusExecute(string sender)
        {
            SelectAccount(sender);
        }

        public void SelectAllAccounts(bool select)
        {
            LstDominatorAccountModel
                .Where(x => SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social || x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(
                    x =>
                    {
                        x.IsAccountManagerAccountSelected = select;
                    });
        }

        public void SelectAccount(string menu)
        {
            SelectAllAccounts(false);

            switch (menu)
            {
                case "Working":
                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.Success).ForEach(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                        });
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x =>
                            x.AccountBaseModel.Status == AccountStatus.Success && x.AccountBaseModel.AccountNetwork ==
                            SocinatorInitialize.ActiveSocialNetwork).ForEach(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                        });
                    }
                    break;
                case "NotWorking":

                    if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
                    {
                        LstDominatorAccountModel.ForEach(x =>
                        {
                            switch (x.AccountBaseModel.Status)
                            {
                                case AccountStatus.Success:
                                case AccountStatus.NotChecked:
                                case AccountStatus.TryingToLogin:
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
                                 case AccountStatus.Success:
                                 case AccountStatus.NotChecked:
                                 case AccountStatus.TryingToLogin:
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
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked).ForEach(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                        });
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked && x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).ForEach(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                        });
                    }
                    break;
            }
        }

        private void SelectAccountByGroupExecute(ContentSelectGroup currentGroup)
        {
            if (currentGroup != null)
            {
                lock (_syncLoadAccounts)
                    LstDominatorAccountModel.Where(y => y.AccountBaseModel.AccountGroup.Content == currentGroup.Content).ForEach(
                        y =>
                        {
                            y.IsAccountManagerAccountSelected = currentGroup.IsContentSelected;
                        });
            }
        }

        #endregion

        #region Initialize AccountManager
        private AccessorStrategies strategyPack;

        public void InitialAccountDetails()
        {
            lock (_syncLoadAccounts)
            {
                try
                {
                    var accountList = _accountsFileManager.GetAll();

                    var availablenetworks = ServiceLocator.Current.GetAllInstances<ISocialNetworkModule>().Select(y => y.Network);

                    if (accountList == null || accountList.Count == 0)
                    {
                        var filePath = ConstantVariable.GetIndexAccountFile();
                        if (File.Exists(filePath))
                            File.Delete(filePath);

                        UpdateAccountFromDb(availablenetworks);
                    }
                    else
                    {
                        var savedAccounts = accountList.Where(x => availablenetworks.Contains(x.AccountBaseModel.AccountNetwork));

                        foreach (var account in savedAccounts)
                        {
                            if (SocinatorInitialize.AvailableNetworks.Contains(account.AccountBaseModel
                                .AccountNetwork))
                            {
                                if (LstDominatorAccountModel.Count >= SocinatorInitialize.MaximumAccountCount)
                                {
                                    GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                                    break;
                                }
                                if (!LstDominatorAccountModel.Any(x => x.AccountBaseModel.UserName == account.UserName &&
                                                    x.AccountBaseModel.AccountNetwork == account.AccountBaseModel.AccountNetwork))
                                    LstDominatorAccountModel.AddSync(account);
                            }
                        }
                    }
                    #region Start scheduling 

                    var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                    runningActivityManager.Initialize(LstDominatorAccountModel);

                    #endregion

                    var softwareSetting = ServiceLocator.Current.GetInstance<ISoftwareSettings>();

                    softwareSetting.ScheduleAutoUpdation();
                    if (SocinatorInitialize.GetSocialLibrary(SocialNetworks.Facebook) != null)
                        softwareSetting.ScheduleAdsScraping();
                }
                catch (Exception ex)
                {
                    /*DEBUG*/
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void UpdateAccountFromDb(IEnumerable<SocialNetworks> availablenetworks)
        {
            var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

            var accounts = globalDbOperation.Get<AccountDetails>();
            List<ProxyManagerModel> oldproxies = _proxyFileManager.GetAllProxy();
            foreach (var account in accounts)
            {
                var network = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), account.AccountNetwork);

                if (availablenetworks.Contains(network))
                {
                    if (!LstDominatorAccountModel.Any(x => x.AccountBaseModel.UserName == account.UserName &&
                                                     x.AccountBaseModel.AccountNetwork == network))
                    {
                        DominatorAccountModel dominatorAccountModel = new DominatorAccountModel()
                        {
                            AccountBaseModel = new DominatorAccountBaseModel
                            {
                                AccountNetwork = network,
                                AccountId = account.AccountId,
                                AccountGroup = new ContentSelectGroup { Content = account.AccountGroup },
                                UserName = account.UserName,
                                Password = account.Password,
                                UserFullName = account.UserFullName,
                                AccountProxy = new Proxy
                                {
                                    ProxyIp = account.ProxyIP,
                                    ProxyPort = account.ProxyPort,
                                    ProxyUsername = account.ProxyUserName,
                                    ProxyPassword = account.ProxyPassword,
                                }
                            },
                            AccountId = account.AccountId,
                            DisplayColumnValue1 = account.DisplayColumnValue1,
                            DisplayColumnValue2 = account.DisplayColumnValue2,
                            DisplayColumnValue3 = account.DisplayColumnValue3,
                            DisplayColumnValue4 = account.DisplayColumnValue4
                        };
                        if (!string.IsNullOrEmpty(account.Cookies))
                            try
                            {
                                dominatorAccountModel.CookieHelperList = JArray.Parse(account.Cookies.Replace("<>", ",")).ToObject<HashSet<CookieHelper>>();
                            }
                            catch (Exception ex)
                            {

                            }
                        if (!string.IsNullOrEmpty(account.Status))
                            dominatorAccountModel.AccountBaseModel.Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), account.Status);
                        if (!string.IsNullOrEmpty(account.ActivityManager))
                            dominatorAccountModel.ActivityManager = JsonConvert.DeserializeObject<JobActivityManager>(account.ActivityManager);
                        LstDominatorAccountModel.AddSync(dominatorAccountModel);

                        #region Update Proxies

                        if (!string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyIp) &&
                                            !string.IsNullOrEmpty(dominatorAccountModel.AccountBaseModel.AccountProxy.ProxyPort))
                        {
                            if (!IsDuplicatProxyAvailable(dominatorAccountModel.AccountBaseModel, oldproxies, null))
                                if (!UpdateProxy(dominatorAccountModel.AccountBaseModel))
                                    AddProxyIfNotExist(dominatorAccountModel.AccountBaseModel);
                        }

                        #endregion

                        _accountsFileManager.Add(dominatorAccountModel);

                    }
                }
            }
        }

        #endregion

        #region Update Account status & details

        private ImmutableQueue<Action> _checkPendingList = ImmutableQueue<Action>.Empty;

        public List<string> _updateAccountList { get; set; } = new List<string>();

        public object AccountUpdateLock { get; set; } = new object();

        private void UpdateAccountDetailsExecute(object sender)
        {
            var selectedAccount = LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList();

            if (selectedAccount.Count == 0)
            {
                Dialog.ShowDialog("Alert", "Please select account to update !!");
                return;
            }
            var updateMenuItem = sender as string;

            //if user clicked on Stop Process
            if (updateMenuItem == "StopProcess")
            {
                StopProcess(selectedAccount);
                return;
            }
            //if user clicked on Stop All Activity
            if (updateMenuItem == "StopAllActivity")
            {
                StopAllActivity(selectedAccount);
                return;
            }

            //if user clicked on Update all details

            #region Update all details
            try
            {
                Task.Factory.StartNew(() =>
                  {
                      selectedAccount.ForEach(account =>
                      {
                          var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                              .GetNetworkCoreFactory().AccountUpdateFactory;
                          try
                          {
                              if (!_updateAccountList.Contains(account.AccountBaseModel.UserName))
                                  MultipleUpdate(account, updateMenuItem, accountFactory);
                              else
                                  GlobusLogHelper.log.Info(Log.AlreadyUpdatingAccount,
                                      account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);
                              Task.Delay(5);
                          }
                          catch (Exception ex)
                          {
                              ex.DebugLog();
                          }
                      });
                  });
            }
            catch (AggregateException ex)
            {
                ex.DebugLog("At Account Updation");
            }
            catch (Exception ex)
            {
                ex.DebugLog("At Account Updation");
            }
            #endregion
        }

        public void MultipleUpdate(DominatorAccountModel account, string updateMenuItem, IAccountUpdateFactory accountFactory)
        {
            if (accountFactory is IAccountUpdateFactoryAsync)
            {
                // this account supports async modules
                var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;
                if (updateMenuItem == "UpdateAllDetail")
                {
                    if (account.Token.IsCancellationRequested)
                        account.CancellationSource = new CancellationTokenSource();

                    var updateAccount = new Task(async () =>
                    {
                        try
                        {
                            _updateAccountList.Add(account.UserName);

                            account.Token.ThrowIfCancellationRequested();

                            var checkResult = await asyncAccount.CheckStatusAsync(account, account.Token);

                            if (checkResult)
                            {
                                var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                                runningActivityManager.ScheduleIfAccountGotSucess(account);
                                account.Token.ThrowIfCancellationRequested();

                                await asyncAccount.UpdateDetailsAsync(account, account.Token);

                                new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                                    .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                                    .SaveToBinFile();

                                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                                var accounts = globalDbOperation.UpdateAccountDetails(account);

                                _updateAccountList.Remove(account.UserName);

                                try
                                {
                                    lock (AccountUpdateLock)
                                    {
                                        Monitor.Pulse(AccountUpdateLock);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    ex.DebugLog();
                                }
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
                    var checkAccount = new Task(async () =>
                    {
                        await asyncAccount.CheckStatusAsync(account, account.Token);

                        if (account.AccountBaseModel.Status == AccountStatus.Success)
                        {
                            var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                            runningActivityManager.ScheduleIfAccountGotSucess(account);
                            //To update proxy status
                            UpdateProxyStatus(account.AccountBaseModel);
                        }
                    }, account.Token);
                    checkAccount.Start();
                }
            }
        }

        private void StopAllActivity(List<DominatorAccountModel> selectedAccounts, bool isNeedToSchedule = false)
        {

            ThreadFactory.Instance.Start(() =>
            {
                selectedAccounts.ForEach(account =>
                {
                    var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                    var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                    jobActivityConfigurationManager[account.AccountId].ToList().ForEach(x =>
                    {
                        if (x.IsEnabled)
                        {
                            x.IsEnabled = false;
                            dominatorScheduler.StopActivity(account, x.ActivityType.ToString(), x.TemplateId, isNeedToSchedule);
                        }
                    });

                    // ReSharper disable once ConstantConditionalAccessQualifier
                    account?.NotifyCancelled();
                    GlobusLogHelper.log.Info(Log.StopAllActivitiesOfAccount,
                         account.AccountBaseModel.AccountNetwork,
                         account.AccountBaseModel.UserName);
                });

                var BinFileHelper = ServiceLocator.Current.GetInstance<IBinFileHelper>();
                BinFileHelper.UpdateAllAccounts(LstDominatorAccountModel.ToList());

            });
        }

        private void StopProcess(List<DominatorAccountModel> selectedAccounts)
        {
            ThreadFactory.Instance.Start(() =>
            {
                selectedAccounts.ForEach(accountSelected =>
                {
                    var accountFullDetails = LstDominatorAccountModel.FirstOrDefault(x => x.UserName == accountSelected.UserName);

                    accountFullDetails?.NotifyCancelled();

                    if (accountFullDetails != null)
                        GlobusLogHelper.log.Info(Log.StopUpdatingAccount, accountFullDetails.AccountBaseModel.AccountNetwork, accountFullDetails.AccountBaseModel.UserName);
                });
                _updateAccountList.Clear();
            });
        }

        private bool UpdateAccountDetailsCanExecute(object sender) => true;
        #endregion


        private bool ActivateBrowserAutomationCanExecute(object sender) => true;

        private void ActivateBrowserAutomationExecute(object sender)
        {
            if (LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().Count > 0)
            {
                var result = Dialog.ShowCustomDialog("Actvating Browser Automation",
                  "This will result in stopping all activity through HTTP and starting activity by Browser. \nDo you want to Continue?", "Continue", "Cancel");
                if (result == MessageDialogResult.Affirmative)
                {
                    LstDominatorAccountModel.ForEach(x =>
                    {
                        if (x.IsAccountManagerAccountSelected)
                        {
                            x.IsRunProcessThroughBrowser = true;
                            new SocinatorAccountBuilder(x.AccountBaseModel.AccountId)
                           .AddOrUpdateBrowserSettings(true)
                           .SaveToBinFile();
                        }

                    });

                    StopAllActivity(LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList(), true);

                    StopProcess(LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList());


                    Task.Factory.StartNew(() =>
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SelectedNetworkViewModel.Selected, "", "LangKeyAccountActivities".FromResourceDictionary(), $"Please wait for 10 secs!");

                        IsProgressActive = true;

                        Thread.Sleep(10000);

                        LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().ForEach(x =>
                        {
                            x.CancellationSource = new CancellationTokenSource();
                        });

                        IsProgressActive = false;


                    });
                }

                

            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "", "Browser Automation", "No account selecetd. Please select atleast one account!");
            }
        }

        private bool DeActivateBrowserAutomationCommandCanExecute(object sender) => true;

        private void DeActivateBrowserAutomationCommandExecute(object sender)
        {
            if (LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().Count > 0)
            {
                var result = Dialog.ShowCustomDialog("Deactvating Browser Automation",
                      "This will result in stopping all activity through Browser and starting activity by HTTP. \nDo you want to Continue?", "Continue", "Cancel");
                if (result == MessageDialogResult.Affirmative)
                {
                    LstDominatorAccountModel.ForEach(x =>
                    {
                        if (x.IsAccountManagerAccountSelected)
                        {
                            x.IsRunProcessThroughBrowser = false;
                            new SocinatorAccountBuilder(x.AccountBaseModel.AccountId)
                               .AddOrUpdateBrowserSettings(false)
                               .SaveToBinFile();
                        }

                    });

                    StopAllActivity(LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList(), true);

                    StopProcess(LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList());

                    Task.Factory.StartNew(() =>
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, SelectedNetworkViewModel.Selected, "", "LangKeyAccountActivities".FromResourceDictionary(), $"Please wait for 10 secs!");

                        IsProgressActive = true;

                        Thread.Sleep(10000);

                        LstDominatorAccountModel.Where(x => x.IsAccountManagerAccountSelected).ToList().ForEach(x =>
                        {
                            x.CancellationSource = new CancellationTokenSource();
                        });
                        
                        IsProgressActive = false;
                    });

                }
            }
            else
            {
                GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "", "Browser Automation", "No account selecetd. Please select atleast one account!");
            }


        }

        private void UpdateGroupDetailsExecute()
        {
            lock (_syncLoadAccounts)
            {
                Groups.Clear();
                Groups.AddRange(LstDominatorAccountModel
                    .Where(x => SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social ||
                                x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork)
                    .GroupBy(a => a.AccountBaseModel.AccountGroup).Select(a => a.Key).ToList());
            }
        }

        private void UpdateUserCradExecute(object sender)
        {
            var lstcred = FileUtilities.FileBrowseAndReader();
            if (lstcred.Count != 0)
            {
                ToasterNotification.ShowInfomation("Credentials imported successfully.\nStart updating...");
                var isAnyAccountUpdated = false;
                foreach (var cred in lstcred)
                {
                    var data = cred.Split('\t');
                    if (data.ToList().Any(x => string.IsNullOrEmpty(x)))
                        continue;
                    if (data.Length < 7)
                        continue;

                    var accountToUpdate = LstDominatorAccountModel.FirstOrDefault(x =>
                               x.AccountBaseModel.AccountNetwork.ToString() == data[0] && x.AccountBaseModel.UserName == data[1]);
                    if (accountToUpdate != null)
                    {
                        accountToUpdate.MailCredentials.Username = data[2];
                        accountToUpdate.MailCredentials.Password = data[3];
                        accountToUpdate.MailCredentials.Hostname = data[4];
                        accountToUpdate.MailCredentials.Port = int.Parse(data[5]);
                        accountToUpdate.IsUseSSL = bool.Parse(data[6]);
                        accountToUpdate.IsAutoVerifyByEmail = true;
                        isAnyAccountUpdated = true;
                    }

                }
                if (isAnyAccountUpdated)
                {
                    _accountsFileManager.UpdateAccounts(LstDominatorAccountModel);
                    ToasterNotification.ShowSuccess("Credentials successfully updated.");
                }
                else
                    ToasterNotification.ShowInfomation("No account found to update credentials or format is wrong.");
            }
        }

        public void AccountBrowserLogin(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var accountScopeFactory = ServiceLocator.Current.GetInstance<IAccountScopeFactory>();

                    var browserManager = accountScopeFactory[$"{dominatorAccountModel.AccountId}_BrowserLogin"].Resolve<IBrowserManager>(dominatorAccountModel.AccountBaseModel.AccountNetwork.ToString());

                    browserManager.BrowserLogin(dominatorAccountModel, LoginType.BrowserLogin);
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        public void AccountStatusChecker(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.CheckStatus(dominatorAccountModel);
                    if (dominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                        runningActivityManager.ScheduleIfAccountGotSucess(dominatorAccountModel);
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void AccountUpdate(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                ThreadFactory.Instance.Start(() =>
                {
                    var accountUpdateFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().AccountUpdateFactory;
                    accountUpdateFactory.UpdateDetails(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void EditProfile(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.EditProfile(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemovePhoneVerification(DominatorAccountModel dominatorAccountModel)
        {
            try
            {
                Task.Factory.StartNew(() =>
                {
                    var profileFactory = SocinatorInitialize
                        .GetSocialLibrary(dominatorAccountModel.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory().ProfileFactory;
                    profileFactory.RemovePhoneVerification(dominatorAccountModel);
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        public void UpdateProxyStatus(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {
            try
            {
                var proxyToBeUpdated = _proxyFileManager.GetProxyById(objDominatorAccountBaseModel.AccountProxy.ProxyId);
                proxyToBeUpdated.Status = "Working";
                _proxyFileManager.EditProxy(proxyToBeUpdated);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void ProfileDetails(DominatorAccountModel acount)
        {
            try
            {
                AccountManager.GetSingletonAccountManager(String.Empty, acount,
                      acount.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private void CopyAccountId(DominatorAccountModel account)
        {
            if (!string.IsNullOrEmpty(account.AccountId))
            {
                Clipboard.SetText(account.AccountId);
                ToasterNotification.ShowSuccess("AccountId copied");
            }

        }
        public void GotoTools(DominatorAccountModel account)
        {
            if (account == null || account.AccountBaseModel.Status != AccountStatus.Success)
                return;

            TabSwitcher.ChangeTabWithNetwork(3, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName);
        }
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

        private bool _headerVisible;

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
