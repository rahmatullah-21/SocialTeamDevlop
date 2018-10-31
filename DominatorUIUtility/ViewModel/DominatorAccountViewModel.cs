using CommonServiceLocator;
using DominatorHouseCore;
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
using LiveCharts;
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
using System.Windows.Data;
using System.Windows.Input;

namespace DominatorUIUtility.ViewModel
{
    public interface IDominatorAccountViewModel
    {

    }

    [ProtoContract]
    public class DominatorAccountViewModel : BindableBase, IDominatorAccountViewModel
    {
        private DbOperations _dbOperations { get; }

        public ObservableCollection<DominatorAccountModel> LstDominatorAccountModel { get; }

        public ObservableCollection<ContentSelectGroup> Groups { get; }
        public ISelectedNetworkViewModel SelectedNetworkViewModel { get; }

        private ImmutableQueue<Action> _pendingActions = ImmutableQueue<Action>.Empty;

        private bool _allAccountsQueued;

        private readonly object _syncLoadAccounts = new object();

        #region Property

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


        public string[] Labels { get; set; }
        public Func<int, string> YFormatter { get; set; }

        public SeriesCollection SeriesCollection
        {
            get
            {
                return _seriesCollection;
            }
            set
            {
                SetProperty(ref _seriesCollection, value);
            }
        }

        public string GrowthChartAccountNumber
        {
            get
            {
                return _growthChartAccountNumber;
            }
            set
            {
                SetProperty(ref _growthChartAccountNumber, value);
            }
        }


        public SocialNetworks GrowthChartAccountNetwork
        {
            get
            {
                return _growthChartAccountNetwork;
            }
            set
            {
                SetProperty(ref _growthChartAccountNetwork, value);
            }
        }

        public string GrowthChartPeriod { get; set; }
        public string GrowthChartProperty { get; set; }
        public string GrowthChartType { get; set; }

        public List<GrowthProperty> GrowthProperties
        {
            get
            {
                return _growthProperties;
            }
            set
            {
                SetProperty(ref _growthProperties, value);
            }
        }


        public List<string> GrowthChartProperties { get; set; }
        public List<string> GrowthChartPeriods { get; set; }
        public List<string> GrowthChartTypes { get; set; }

        public int? YMaxValue { get; set; }

        public List<DailyStatisticsViewModel> GrowthList { get; set; }

        #endregion

        #region Command 

        public ICommand AddSingleAccountCommand { get; }
        public ICommand InfoCommand { get; }
        public ICommand LoadMultipleAccountsCommand { get; }
        public ICommand DeleteAccountsCommand { get; }
        public ICommand ExportCommand { get; }
        public ICommand SelectAccountCommand { get; }
        public ICommand SelectAccountByStatusCommand { get; }
        public ICommand SelectAccountByGroupCommand { get; }
        public ICommand SingleAccountEditCommand { get; }
        public ICommand SingleAccountDeleteCommand { get; }
        public ICommand UpdateAccountDetailsCommand { get; }
        public ICommand UpdateGroupCommand { get; }
        public ICommand UpdateUserCradCommand { get; }
        #endregion


        public DominatorAccountViewModel(IMainViewModel mainViewModel, ISelectedNetworkViewModel selectedNetworkViewModel)
        {
            SelectedNetworkViewModel = selectedNetworkViewModel;
            this.strategyPack = mainViewModel.Strategies;
            Groups = new ObservableCollection<ContentSelectGroup>();
            BindingOperations.EnableCollectionSynchronization(Groups, _syncLoadAccounts);
            LstDominatorAccountModel = new ObservableCollection<DominatorAccountModel>();
            BindingOperations.EnableCollectionSynchronization(LstDominatorAccountModel, _syncLoadAccounts);

            InitialAccountDetails();

            _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());


            #region Command Initialization

            AddSingleAccountCommand = new BaseCommand<object>(AddSingleAccountCanExecute, (o) => AddSingleAccountExecute(o, this.strategyPack._determine_available, this.strategyPack._inform_warnings));

            LoadMultipleAccountsCommand = new BaseCommand<object>(LoadMultipleAccountsCanExecute, (o) => LoadMultipleAccountsExecute(o, this.strategyPack._determine_available, this.strategyPack._inform_warnings));

            InfoCommand = new BaseCommand<object>(InfoCommandCanExecute, InfoCommandExecute);

            ExportCommand = new BaseCommand<object>(ExportCanExecute, ExportExecute);

            DeleteAccountsCommand = new BaseCommand<object>(DeleteAccountsCanExecute, DeleteAccountsExecute);

            SelectAccountCommand = new DelegateCommand<bool?>(SelectAccountExecute);

            SelectAccountByStatusCommand = new DelegateCommand<string>(SelectAccountByStatusExecute);

            SelectAccountByGroupCommand = new DelegateCommand<ContentSelectGroup>(SelectAccountByGroupExecute);

            SingleAccountDeleteCommand = new BaseCommand<object>(SingleAccountDeleteCanExecute, SingleAccountDeleteExecute);

            UpdateAccountDetailsCommand = new BaseCommand<object>(UpdateAccountDetailsCanExecute, UpdateAccountDetailsExecute);

            UpdateGroupCommand = new DelegateCommand(UpdateGroupDetailsExecute);

            UpdateUserCradCommand = new BaseCommand<object>((sender) => true, UpdateUserCradExecute);

            #endregion
        }

        #region Add accounts

        private bool AddSingleAccountCanExecute(object sender) => true;

        private void AddSingleAccountExecute(object sender, Func<SocialNetworks, bool> isNetworkAvailable, Action<string> warn)
        {
            var objDominatorAccountBaseModel = new DominatorAccountBaseModel();

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "LangKeyAddAccount".FromResourceDictionary(), "LangKeySave".FromResourceDictionary(), false, SocinatorInitialize.ActiveSocialNetwork.ToString());

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
                        // var finalAccount = singleAccount.Replace(",", ":").Replace("<NA>", "");
                        var finalAccount = singleAccount.Replace("<NA>", "\t");
                        var splitAccount = Regex.Split(finalAccount.TrimEnd(), "\t");
                        //var splitAccount = Regex.Split(finalAccount, ":");
                        if (splitAccount.Length <= 1) continue;

                        //assign the username, password and groupname
                        var groupname = splitAccount[0];

                        var socialNetwork = splitAccount[1];
                        if (socialNetwork == "AccountNetwork") continue;
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
                            case 9:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                break;
                            case 10:
                                proxyaddress = splitAccount[4];
                                proxyport = splitAccount[5];
                                proxyusername = splitAccount[6];
                                proxypassword = splitAccount[7];
                                status = splitAccount[8];
                                cookies = splitAccount[9].Replace("<>", ",");
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
                            AccountNetwork = (SocialNetworks)Enum.Parse(typeof(SocialNetworks), socialNetwork),
                            Status = (AccountStatus)Enum.Parse(typeof(AccountStatus), status),

                        };

                        if (isNetworkAvailable(objDominatorAccountBaseModel.AccountNetwork))
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

            if (LstDominatorAccountModel.Count > SocinatorInitialize.MaximumAccountCount)
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
                AccountId = objDominatorAccountBaseModel.AccountId
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1,
                AccountId = dominatorAccountBaseModel.AccountId
            };
            if (!string.IsNullOrEmpty(cookies))
                try
                {
                    dominatorAccountModel.CookieHelperList = JArray.Parse(cookies).ToObject<HashSet<CookieHelper>>();
                }
                catch (Exception ex)
                {

                }
            List<ProxyManagerModel> oldproxies = ProxyFileManager.GetAllProxy();

            //var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            //dominatorAccountModel.Token.Register(cancel);
            if (!String.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyIp) &&
                !String.IsNullOrEmpty(dominatorAccountBaseModel.AccountProxy.ProxyPort))
            {
                if (!IsDuplicatProxyAvailable(objDominatorAccountBaseModel, oldproxies, null))
                {
                    if (!UpdateProxy(dominatorAccountBaseModel))
                        AddProxyIfNotExist(dominatorAccountBaseModel, strategyPack);
                }
            }

            var cancel = secondaryTaskStrategyReturningCancellation(() => UpdateProxy(objDominatorAccountBaseModel));
            dominatorAccountModel.Token.Register(cancel);


            //serialize the given account, if its success then add to account model list
            if (AccountsFileManager.Add(dominatorAccountModel))
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
            var accountDetails = AccountsFileManager.GetAll();

            var databaseCreation = secondaryTaskStrategyReturningCancellation(() =>
            {
                var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());

                //DataBaseHandler.DbInitialCounters[objDominatorAccountBaseModel.AccountNetwork](dbOperations);

                // DataBaseHandler.CreateDataBase(objDominatorAccountBaseModel.AccountId, objDominatorAccountBaseModel.AccountNetwork, DatabaseType.AccountType);

                #region Saving Account detail to AccountDetails database

                globalDbOperation.Add<AccountDetails>(new AccountDetails
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
                    AddedDate = DateTime.Now
                });

                #endregion
            });
            dominatorAccountModel.Token.Register(databaseCreation);

            #endregion
            //if (dominatorAccountBaseModel.Status != AccountStatus.Success)
            //{
            if (!(bool)SoftwareSettings.Settings?.IsDoNotAutoLoginAccountsWhileAddingToSoftware)
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
            //}
        }
        public bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {

            var oldproxies = ProxyFileManager.GetAllProxy();

            bool isProxyUpdated = false;
            try
            {
                var oldAccount = AccountsFileManager.GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork).AccountBaseModel;

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
            ProxyManager proxyManager = null;
            Application.Current.Dispatcher.Invoke(
                () => proxyManager = ProxyManager.GetProxyManagerControl(strategyPack));
            //  ProxyManager proxyManager = ProxyManager.GetProxyManagerControl(strategyPack);
            foreach (var proxy in oldProxies)
            {
                #region To check for duplicate proxy 
                try
                {

                    if (objAccountBaseModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                        && objAccountBaseModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                    {
                        #region If other proxy with same ip/port is present

                        //if (objAccountBaseModel.AccountProxy.ProxyId != proxy.AccountProxy.ProxyId)
                        //{
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
                                                ProxyFileManager.EditProxy(pr);
                                            }

                                        });
                                        var proxyToUpdate = proxyManager.ProxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == oldAccount.AccountProxy.ProxyIp
                                                                                                                                        && x.AccountProxy.ProxyPort == oldAccount.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Remove(proxyToUpdate?.AccountsAssignedto.FirstOrDefault(x => x.UserName == oldAccount.UserName && x.AccountNetwork == oldAccount.AccountNetwork));

                                        proxyToUpdate = proxyManager.ProxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
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
                                        var proxyToUpdate = proxyManager.ProxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
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

                                ProxyFileManager.EditProxy(proxy);
                                proxyManager?.ProxyManagerViewModel.AccountsAlreadyAssigned.Add(
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
            ProxyManager proxyManager = ProxyManager.GetProxyManagerControl(strategyPack);
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

                        ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                        UpdateProxyList(proxy, proxyManager);
                        ProxyFileManager.EditProxy(proxy);
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
                            ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
                            UpdateProxyList(proxy, proxyManager);
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

                            ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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


        private void UpdateProxyList(ProxyManagerModel proxy, ProxyManager proxyManager)
        {
            try
            {

                var proxyToupdate = proxyManager.ProxyManagerViewModel.LstProxyManagerModel.FirstOrDefault(x =>
                    x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                if (proxyToupdate != null)
                    proxyToupdate.AccountProxy = proxy.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private static void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, AccessorStrategies strategyPack)
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


            #region remove account from AccountsAssignedto if any proxy having account

            ProxyFileManager.GetAllProxy().ForEach(proxy =>
            {
                AccountsFileManager.GetAll().ForEach(acc =>
                {
                    if (proxy.AccountsAssignedto.Any(x =>
                        x.UserName == acc.UserName && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork))
                    {
                        proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName ==
                            acc.UserName
                                && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork));

                    }
                });

                ProxyFileManager.EditProxy(proxy);
            });

            #endregion

            Application.Current.Dispatcher.Invoke(() =>
            {
                var proxyManager = ProxyManager.GetProxyManagerControl(strategyPack);
                proxyManager.ProxyManagerViewModel.LstProxyManagerModel.ForEach(x =>
                {
                    if (x.AccountsAssignedto.Any(y => y.UserName == objAccount.UserName &&
                                                      y.AccountNetwork == objAccount.AccountNetwork))
                        x.AccountsAssignedto.Remove(x.AccountsAssignedto.FirstOrDefault(y =>
                            y.UserName == objAccount.UserName &&
                            y.AccountNetwork == objAccount.AccountNetwork));
                });
                proxyManager.ProxyManagerViewModel.LstProxyManagerModel.Add(ProxyManagerModel);
                proxyManager.ProxyManagerViewModel.AccountsAlreadyAssigned.Add(
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

            ProxyFileManager.SaveProxy(ProxyManagerModel);

            ProxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);



        }
        private static bool UpdateProxyIfNull(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail, AccessorStrategies strategy)
        {
            try
            {
                var proxyManager = ProxyManager.GetProxyManagerControl(strategy);
                if (string.IsNullOrEmpty(objAccountBaseModel.AccountProxy.ProxyIp) &&
                    string.IsNullOrEmpty(objAccountBaseModel.AccountProxy.ProxyPort))
                {
                    #region if proxy | port empty then that account will remove from proxy AccountsAssignedto list and that account will add to all proxies

                    foreach (var proxy in ProxyDetail)
                    {

                        try
                        {
                            var account = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == objAccountBaseModel.UserName && x.AccountNetwork == objAccountBaseModel.AccountNetwork);

                            if (account != null)
                            {
                                proxy.AccountsAssignedto.Remove(account);
                                ProxyFileManager.EditProxy(proxy);
                                proxyManager = ProxyManager.GetProxyManagerControl(strategy);
                                proxyManager.ProxyManagerViewModel.LstProxyManagerModel
                                    .FirstOrDefault(x => x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                                    .AccountsAssignedto = proxy.AccountsAssignedto;
                                proxyManager.ProxyManagerViewModel.AccountsAlreadyAssigned.
                                    Remove(proxyManager.ProxyManagerViewModel.AccountsAlreadyAssigned.FirstOrDefault(x => x.UserName == objAccountBaseModel.UserName
                                                                                                                          && x.AccountNetwork == objAccountBaseModel.AccountNetwork));
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }

                    #endregion

                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;

        }




        #endregion

        #region Delete Accounts


        public void DeleteAccountFromProxy(List<DominatorAccountModel> objAccountBaseModel)
        {
            var proxyManager = ProxyManager.GetProxyManagerControl(strategyPack);
            var allProxy = ProxyFileManager.GetAllProxy();
            ThreadFactory.Instance.Start(() =>
            {
                allProxy?.ForEach(proxy =>
                {
                    try
                    {
                        objAccountBaseModel.ForEach(account =>
                        {
                            proxyManager.ProxyManagerViewModel.AccountsAlreadyAssigned.Remove(proxyManager.ProxyManagerViewModel.AccountsAlreadyAssigned.FirstOrDefault(x =>
                                x.UserName == account.UserName
                                && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x =>
                                x.UserName == account.UserName
                                && x.AccountNetwork == account.AccountBaseModel.AccountNetwork));

                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                try
                                {
                                    var proxyToupdate = proxyManager.ProxyManagerViewModel.LstProxyManagerModel
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
                    }

                    ProxyFileManager.EditProxy(proxy);
                });
            });

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

                ThreadFactory.Instance.Start(() => { DeleteAccounts(selectAccounts); });

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
                var network = item.AccountBaseModel.AccountNetwork.ToString();

                _dbOperations.Remove<AccountDetails>(user => user.AccountNetwork == network && user.UserName == item.UserName);

                GlobusLogHelper.log.Info(Log.Deleted, item.AccountBaseModel.AccountNetwork, item.AccountBaseModel.UserName, "LangKeyAccounts".FromResourceDictionary());
                DeleteAccountFromCampaign(item);
                item.NotifyCancelled();
            });


            // remove from file
            AccountsFileManager.Delete(x => selectAccounts.FirstOrDefault(a => a.AccountId == x.AccountId) != null);
            DeleteAccountFromProxy(selectAccounts.ToList());

            //also delete the associated files
            DataBaseHandler.DeleteDatabase(selectAccounts.Select(acct => acct.AccountId));

        }

        private void DeleteAccountFromCampaign(DominatorAccountModel account)
        {
            account = AccountsFileManager.GetAccount(account.UserName, account.AccountBaseModel.AccountNetwork);
            var moduleConfigurations = account.ActivityManager.LstModuleConfiguration;
            foreach (var moduleConfiguration in moduleConfigurations)
            {
                DominatorScheduler.StopActivity(account, moduleConfiguration.ActivityType.ToString(), moduleConfiguration.TemplateId, false);
                if (moduleConfiguration.IsTemplateMadeByCampaignMode)
                {
                    CampaignsFileManager.DeleteSelectedAccount(moduleConfiguration.TemplateId, account.AccountBaseModel.UserName);
                }
            }
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

            const string header = "Account Group,AccountNetwork,Username,Password,Proxy Address,Proxy Port,Proxy Username,Proxy Password,Status,Cookies";

            var filename = $"{exportPath}\\Accounts {ConstantVariable.DateasFileName}.csv";

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
                     + JsonConvert.SerializeObject(account.CookieHelperList).Replace(",", "<>");

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

        private bool InfoCommandCanExecute(object sender) => true;

        private void InfoCommandExecute(object sender) => IsOpenHelpControl = true;

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
                var accountList = AccountsFileManager.GetAll();

                var availablenetworks = ServiceLocator.Current.GetAllInstances<ISocialNetworkModule>().Select(y => y.Network);

                var savedAccounts = accountList.Where(x => availablenetworks.Contains(x.AccountBaseModel.AccountNetwork));
                //var savedAccounts = accountList.ToList();

                try
                {
                    LstDominatorAccountModel.Clear();
                    savedAccounts.ForEach(account =>
                    {
                        if (SocinatorInitialize.AvailableNetworks.Contains(account.AccountBaseModel.AccountNetwork))
                        {
                            if (LstDominatorAccountModel.Count > SocinatorInitialize.MaximumAccountCount)
                            {
                                GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                                return;
                            }
                            LstDominatorAccountModel.Add(account);
                        }
                    });
                }
                catch (Exception ex)
                {
                    /*DEBUG*/
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        #endregion

        #region Update Account status & details

        ImmutableQueue<Action> _checkPendingList = ImmutableQueue<Action>.Empty;

        bool _allSelectedAccountsQueued;
        private List<GrowthProperty> _growthProperties;
        private string _growthChartAccountNumber;
        private SocialNetworks _growthChartAccountNetwork;
        private SeriesCollection _seriesCollection;

        public List<string> _updateAccountList { get; set; } = new List<string>();

        public object AccountUpdateLock { get; set; } = new object();

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
            if (updateMenuItem == "StopAllActivity")
            {
                StopAllActivity(selectedAccount);
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

        public void MultipleUpdate(DominatorAccountModel account, string updateMenuItem, IAccountUpdateFactory accountFactory)
        {
            if (typeof(IAccountUpdateFactoryAsync).IsAssignableFrom(accountFactory.GetType()))
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

                                account.Token.ThrowIfCancellationRequested();

                                await asyncAccount.UpdateDetailsAsync(account, account.Token);

                                new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                                    .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                                    .SaveToBinFile();

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
                            //To update proxy status
                            UpdateProxyStatus(account.AccountBaseModel);
                        }
                    }, account.Token);
                    checkAccount.Start();
                }

                else if (updateMenuItem == "StopProcess")
                {
                    StopProcess();
                }

            }
        }

        private void StopAllActivity(List<DominatorAccountModel> selectedAccounts)
        {

            ThreadFactory.Instance.Start(() =>
            {
                selectedAccounts.ForEach(account =>
                {
                    var accountToUpdate = AccountsFileManager.GetAccountById(account.AccountId);
                    accountToUpdate.ActivityManager.LstModuleConfiguration.ForEach(x =>
                    {
                        x.IsEnabled = false;
                        DominatorScheduler.StopActivity(account, x.ActivityType.ToString(), x.TemplateId, false);
                    });
                    account.ActivityManager.LstModuleConfiguration =
                        accountToUpdate.ActivityManager.LstModuleConfiguration;
                    account?.NotifyCancelled();
                    GlobusLogHelper.log.Info(Log.StopAllActivitiesOfAccount,
                         account.AccountBaseModel.AccountNetwork,
                         account.AccountBaseModel.UserName);
                });

                BinFileHelper.UpdateAllAccounts(LstDominatorAccountModel.ToList());

            });
        }

        private void StopProcess()
        {
            ThreadFactory.Instance.Start(() =>
            {
                _updateAccountList.ForEach(accountSelected =>
                {
                    var accountFullDetails = LstDominatorAccountModel.FirstOrDefault(x => x.UserName == accountSelected);

                    accountFullDetails?.NotifyCancelled();

                    if (accountFullDetails != null)
                        GlobusLogHelper.log.Info(Log.StopUpdatingAccount, accountFullDetails.AccountBaseModel.AccountNetwork, accountFullDetails.AccountBaseModel.UserName);
                });
                _updateAccountList.Clear();
            });
        }

        private bool UpdateAccountDetailsCanExecute(object sender) => true;
        #endregion

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
            ToasterNotification.ShowInfomation("Credentials imported successfully.\nStart updating...");
            foreach (var cred in lstcred)
            {
                var data = cred.Split('\t');

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
                }

            }
            AccountsFileManager.UpdateAccounts(LstDominatorAccountModel);
            ToasterNotification.ShowSuccess("Credentials successfully updated.");
        }



        public void AccountBrowserLogin(DominatorAccountModel model)
            => strategyPack.AccountBrowserLogin(model);

        public void ActionCheckAccount(DominatorAccountModel model)
            => strategyPack.ActionCheckAccount(model);

        public void ActionUpdateAccount(DominatorAccountModel model)
            => strategyPack.action_UpdateFollower(model);
        public void EditProfile(DominatorAccountModel model)
            => strategyPack.EditProfile(model);
        public void RemovePhoneVerification(DominatorAccountModel model)
            => strategyPack.RemovePhoneVerification(model);
        public void UpdateProxyStatus(DominatorAccountBaseModel objDominatorAccountBaseModel)
        {
            try
            {
                var ProxyToBeUpdated = ProxyFileManager.GetProxyById(objDominatorAccountBaseModel.AccountProxy.ProxyId);
                ProxyToBeUpdated.Status = "Working";
                ProxyFileManager.EditProxy(ProxyToBeUpdated);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DeleteAccount(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null)
                DeleteAccountByContextMenu(sender);
            // AccountListView.ItemsSource = AccountCollectionView;

        }

        public void GotoTools(object sender, RoutedEventArgs e)
        {
            var dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dominatorAccountModel == null)
                return;
            TabSwitcher.ChangeTabWithNetwork(3, dominatorAccountModel.AccountBaseModel.AccountNetwork, dominatorAccountModel.AccountBaseModel.UserName);
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
