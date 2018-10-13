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
using DominatorUIUtility.Views;
using LiveCharts;
using MahApps.Metro.Controls.Dialogs;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace DominatorUIUtility.ViewModel
{
    public interface IDominatorAccountViewModel
    {

    }

    [ProtoContract]
    public class DominatorAccountViewModel : BindableBase, IDominatorAccountViewModel
    {
        private DbOperations dbOperations { get; }

        public DominatorAccountViewModel(IMainViewModel mainViewModel)
        {
            this.strategyPack = mainViewModel.Strategies;

            InitialAccountDetails();

            dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());


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

            UpdateUserCradCommand = new BaseCommand<object>((sender) => true, UpdateUserCradExecute);

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
                if (_seriesCollection == value)
                    return;
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
                if (_growthChartAccountNumber == value)
                    return;
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
                if (_growthChartAccountNetwork == value)
                    return;
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
                if (_growthProperties == value)
                    return;
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
        public ICommand UpdateUserCradCommand { get; set; }
        #endregion

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

                    dialogWindow.Close();

                    if (LstDominatorAccountModel.Count + 1 >=
                        SocinatorInitialize.MaximumAccountCount)
                    {
                        GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                    }

                    ThreadFactory.Instance.Start(() =>
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


                if (loadedAccountlist.Count + LstDominatorAccountModel.Count >
                    SocinatorInitialize.MaximumAccountCount)
                {
                    GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                }

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
                        // var finalAccount = singleAccount.Replace(",", ":").Replace("<NA>", "");
                        var finalAccount = singleAccount.Replace("<NA>", "\t");
                        var splitAccount = Regex.Split(finalAccount, "\t");
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

                allAccountsQueued = true;

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public void AddAccount(DominatorAccountBaseModel objDominatorAccountBaseModel,
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
                Status = AccountStatus.NotChecked,
                AccountNetwork = objDominatorAccountBaseModel.AccountNetwork,
                AccountId = objDominatorAccountBaseModel.AccountId
            };

            var dominatorAccountModel = new DominatorAccountModel
            {
                AccountBaseModel = dominatorAccountBaseModel,
                RowNo = LstDominatorAccountModel.Count + 1,
                AccountId = dominatorAccountBaseModel.AccountId
            };

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
                    Application.Current.Dispatcher.Invoke(new Action(delegate
                    {
                        LstDominatorAccountModel.Add(dominatorAccountModel);
                    }));
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

                DataBaseHandler.DbInitialCounters[objDominatorAccountBaseModel.AccountNetwork](dbOperations);

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
            if (!SoftwareSettings.Settings.IsDoNotAutoLoginAccountsWhileAddingToSoftware)
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
                dbOperations.Remove<AccountDetails>(user =>
                    user.AccountNetwork == item.AccountBaseModel.AccountNetwork.ToString() &&
                    user.UserName == item.UserName);
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
            //CampaignsFileManager.Get().ForEach(camp =>
            //{
            //    if (camp.SelectedAccountList.Any(acc => acc == account.UserName) && camp.SocialNetworks == account.AccountBaseModel.AccountNetwork)
            //    {
            //        var moduleSettings = account.ActivityManager.LstModuleConfiguration.FirstOrDefault(module => module.ActivityType.ToString() == camp.SubModule);

            //        CampaignsFileManager.DeleteSelectedAccount(moduleSettings.TemplateId, account.AccountBaseModel.UserName);

            //        DominatorScheduler.StopActivity(account, camp.SubModule, moduleSettings.TemplateId,true);
            //    }
            //});
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
                ex.DebugLog();
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

            const string header = "Account Group,AccountNetwork,Username,Password,Status,Proxy Address,Proxy Port,Proxy Username,Proxy Password";

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
                    var csvData =
                     account.AccountBaseModel.AccountGroup.Content + ","
                     + account.AccountBaseModel.AccountNetwork + ","
                     + account.AccountBaseModel.UserName + ","
                     + account.AccountBaseModel.Password + ","
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
                AccountNetwork = selectedAccount.AccountBaseModel.AccountNetwork,
                AccountId = selectedAccount.AccountId
            };

            var objAddUpdateAccountControl = new AddUpdateAccountControl(objDominatorAccountBaseModel, "LangKeyUpdateAccount".FromResourceDictionary(), "LangKeyUpdate".FromResourceDictionary(), !string.IsNullOrEmpty(selectedAccount.AccountBaseModel.AccountProxy.ProxyIp), objDominatorAccountBaseModel.AccountNetwork.ToString());

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
                if (selectedAccount.AccountBaseModel.UserName != objDominatorAccountBaseModel.UserName || selectedAccount.AccountBaseModel.Password != objDominatorAccountBaseModel.Password)
                {
                    selectedAccount.Cookies = new CookieCollection();
                }

                selectedAccount.AccountBaseModel.UserName = objDominatorAccountBaseModel.UserName;
                selectedAccount.AccountBaseModel.Password = objDominatorAccountBaseModel.Password;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyIp = objDominatorAccountBaseModel.AccountProxy.ProxyIp;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPort = objDominatorAccountBaseModel.AccountProxy.ProxyPort;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyUsername = objDominatorAccountBaseModel.AccountProxy.ProxyUsername;
                selectedAccount.AccountBaseModel.AccountProxy.ProxyPassword = objDominatorAccountBaseModel.AccountProxy.ProxyPassword;
                selectedAccount.AccountBaseModel.AccountNetwork = objDominatorAccountBaseModel.AccountNetwork;


                List<ProxyManagerModel> oldproxies = ProxyFileManager.GetAllProxy();

                if (!UpdateProxyIfNull(objDominatorAccountBaseModel, oldproxies, strategyPack))
                {
                    var oldAccount = AccountsFileManager.GetAccountById(selectedAccount.AccountId).AccountBaseModel;

                    if (!IsDuplicatProxyAvailable(objDominatorAccountBaseModel, oldproxies, oldAccount))
                    {
                        if (!UpdateProxy(selectedAccount.AccountBaseModel))
                            AddProxyIfNotExist(selectedAccount.AccountBaseModel, strategyPack);
                    }
                    else
                        selectedAccount.AccountBaseModel.AccountProxy = objDominatorAccountBaseModel.AccountProxy;

                    try
                    {
                        selectedAccount.Token.ThrowIfCancellationRequested();

                        var socinatorAccountBuilder = new SocinatorAccountBuilder(selectedAccount.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(selectedAccount.AccountBaseModel)
                            .SaveToBinFile();

                        // AccountsFileManager.Edit(selectedAccount);
                    }
                    catch (OperationCanceledException)
                    {
                        throw new System.OperationCanceledException();
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

                #region Checking status


                ThreadFactory.Instance.Start(async () =>
                 {
                     try
                     {
                         selectedAccount.CookieHelperList.Clear();
                         selectedAccount.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                         var accountFactory = SocinatorInitialize.GetSocialLibrary(objDominatorAccountBaseModel.AccountNetwork)
                             .GetNetworkCoreFactory().AccountUpdateFactory;
                         var asyncAccount = (IAccountUpdateFactoryAsync)accountFactory;

                         await asyncAccount.CheckStatusAsync(selectedAccount, selectedAccount.Token);
                         if (selectedAccount.AccountBaseModel.Status == AccountStatus.Success)
                         {
                             //DominatorHouseCore.BusinessLogic.Scheduler.RunningActivityManager.InitializeSingleAccount(selectedAccount);
                             UpdateProxyStatus(selectedAccount.AccountBaseModel);
                             await asyncAccount.UpdateDetailsAsync(selectedAccount, selectedAccount.Token);

                         }

                     }
                     catch (Exception ex)
                     {
                         ex.DebugLog();
                     }
                 });

                #endregion

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
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.Success).Select(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                            return x;
                        }).ToList();
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.Success && x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
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
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked).Select(x =>
                        {
                            x.IsAccountManagerAccountSelected = true;
                            return x;
                        }).ToList();
                    }
                    else
                    {
                        LstDominatorAccountModel.Where(x => x.AccountBaseModel.Status == AccountStatus.NotChecked && x.AccountBaseModel.AccountNetwork == SocinatorInitialize.ActiveSocialNetwork).Select(x =>
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
                        if (SocinatorInitialize.AvailableNetworks.Contains(account.AccountBaseModel.AccountNetwork))
                        {
                            if (LstDominatorAccountModel.Count > SocinatorInitialize.MaximumAccountCount)
                            {
                                GlobusLogHelper.log.Info("You have already added maximum account as per your plan");
                                return;
                            }
                            LstDominatorAccountModel.Add(account);
                            AccountCollectionView = CollectionViewSource.GetDefaultView(LstDominatorAccountModel);
                        }
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
                    var accountToStop = LstDominatorAccountModel.FirstOrDefault(x => x.AccountId == account.AccountId);
                    accountToStop.ActivityManager.LstModuleConfiguration.Select(x =>
                    {
                        x.IsEnabled = false;
                        return x;
                    });
                    GlobusLogHelper.log.Info(Log.StopAllActivitiesOfAccount,
                        account.AccountBaseModel.AccountNetwork,
                        account.AccountBaseModel.UserName);
                });
               
                BinFileHelper.UpdateAllAccounts(LstDominatorAccountModel.ToList());
                _updateAccountList.Clear();
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

        public IEnumerable<MenuItem> GetContextMenuItems(string socialNetwork, DominatorAccountModel dominatorAccountModel)
        {
            var menuOptions = new List<MenuItem>();

            #region Details Menu

            var image = Application.Current.FindResource("appbar_book_open_hardcover");
            var convasImage = GetConvasImage(image);

            var deatilProfileMenu = new MenuItem { Header = "Details", Icon = convasImage };
            deatilProfileMenu.Click += ProfileDetails;
            deatilProfileMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(deatilProfileMenu);

            #endregion

            //#region Edit Profile Menu

            //image = Application.Current.FindResource("appbar_edit_box");
            //convasImage = GetConvasImage(image);

            //var editProfileMenu = new MenuItem { Header = "Edit Profile", Icon = convasImage };
            //editProfileMenu.Click += EditProfile;
            //editProfileMenu.DataContext = dominatorAccountModel;
            //menuOptions.Add(editProfileMenu);

            //#endregion

            #region Delete Profile Menu

            image = Application.Current.FindResource("appbar_delete");
            convasImage = GetConvasImage(image);

            var deleteProfileMenu = new MenuItem { Header = "Delete Profile", Icon = convasImage };
            deleteProfileMenu.Click += DeleteAccount;
            deleteProfileMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(deleteProfileMenu);

            #endregion

            #region Browser Login Menu

            image = Application.Current.FindResource("appbar_browser");
            convasImage = GetConvasImage(image);
            var browserLoginMenu = new MenuItem { Header = "Browser Login", Icon = convasImage };
            browserLoginMenu.Click += BrowserLogin;
            browserLoginMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(browserLoginMenu);

            #endregion

            #region Go to Tools Menu

            if (SocinatorInitialize.ActiveSocialNetwork == SocialNetworks.Social)
            {
                image = Application.Current.FindResource("appbar_tools");
                convasImage = GetConvasImage(image);

                var goToToolsMenu = new MenuItem { Header = "Go to Tools", Icon = convasImage };
                goToToolsMenu.Click += GotoTools;
                goToToolsMenu.DataContext = dominatorAccountModel;
                menuOptions.Add(goToToolsMenu);
            }

            #endregion

            #region Check Account Status Menu

            image = Application.Current.FindResource("appbar_page_search");
            convasImage = GetConvasImage(image);

            var loginStatusMenu = new MenuItem { Header = "Check Account Status", Icon = convasImage };
            loginStatusMenu.Click += CheckinStatus;
            loginStatusMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(loginStatusMenu);

            #endregion

            #region Update Friendship Menu

            image = Application.Current.FindResource("appbar_group");
            convasImage = GetConvasImage(image);

            var updateMenu = new MenuItem { Header = "Update Friendship", Icon = convasImage };
            updateMenu.Click += UpdateFriendshipCount;
            updateMenu.DataContext = dominatorAccountModel;
            menuOptions.Add(updateMenu);

            #endregion

            switch (socialNetwork)
            {
                case "Facebook":

                    #region Remove Phone Verification Menu

                    //image = Application.Current.FindResource("appbar_iphone");
                    //convasImage = GetConvasImage(image);

                    //var removePhoneVerificationMenu = new MenuItem { Header = "Remove Phone Verification", Icon = convasImage };
                    //removePhoneVerificationMenu.Click += FacebookRemovePhoneVerification;
                    //removePhoneVerificationMenu.DataContext = dominatorAccountModel;
                    //menuOptions.Add(removePhoneVerificationMenu);

                    #endregion

                    break;

                case "Instagram":

                    #region Edit Insta Profile Menu

                    image = Application.Current.FindResource("appbar_page_edit");
                    convasImage = GetConvasImage(image);

                    var editInstaProfileMenu = new MenuItem { Header = "Edit Insta Profile", Icon = convasImage };
                    editInstaProfileMenu.Click += EditNetworkProfile;
                    editInstaProfileMenu.DataContext = dominatorAccountModel;
                    menuOptions.Add(editInstaProfileMenu);

                    #endregion

                    //#region Phone Verification Menu

                    //image = Application.Current.FindResource("appbar_iphone");
                    //convasImage = GetConvasImage(image);
                    //var phoneVerificationMenu = new MenuItem { Header = "Phone Verification", Icon = convasImage };
                    //phoneVerificationMenu.Click += InstaPhoneVerification;
                    //phoneVerificationMenu.DataContext = dominatorAccountModel;
                    //menuOptions.Add(phoneVerificationMenu);

                    //#endregion

                    break;
                case "Twitter":

                    #region Edit Twitter Profile Menu

                    image = Application.Current.FindResource("appbar_page_edit");
                    convasImage = GetConvasImage(image);

                    var editTwtProfileMenu = new MenuItem { Header = "Edit Twitter Profile", Icon = convasImage };
                    editTwtProfileMenu.Click += EditNetworkProfile;
                    editTwtProfileMenu.DataContext = dominatorAccountModel;
                    menuOptions.Add(editTwtProfileMenu);

                    #endregion
                    break;
            }
            #region Edit Twitter Profile Menu

            image = Application.Current.FindResource("appbar_page_duplicate");
            convasImage = GetConvasImage(image);

            var copyAccountId = new MenuItem { Header = "Copy Account Id", Icon = convasImage };
            copyAccountId.Click += CopyAccountId;
            copyAccountId.DataContext = dominatorAccountModel;
            menuOptions.Add(copyAccountId);

            #endregion
            return menuOptions;
        }

        private void CopyAccountId(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
            if (!string.IsNullOrEmpty(dataContext.AccountId))
            {
                Clipboard.SetText(dataContext.AccountId);
                ToasterNotification.ShowSuccess("AccountId copied");
            }

        }

        private static Rectangle GetConvasImage(object image)
        {
            Rectangle rectangle = new Rectangle();
            rectangle.Width = 18;
            rectangle.Height = 20;
            rectangle.Fill = Brushes.Black;
            VisualBrush visualBrush = new VisualBrush();
            visualBrush.Visual = image as Visual;
            visualBrush.Stretch = Stretch.Fill;
            rectangle.OpacityMask = visualBrush;
            return rectangle;
        }

        private void ProfileDetails(object sender, RoutedEventArgs e)
        {
            try
            {
                var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                AccountManager.GetSingletonAccountManager(String.Empty, dataContext, dataContext.AccountBaseModel.AccountNetwork);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void EditProfile(object sender, RoutedEventArgs e)
        {
            var dataContext = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

            if (dataContext != null) EditAccount(sender);
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

        public void BrowserLogin(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                AccountBrowserLogin(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                exception.DebugLog();
                //MessageBox.Show(exception.Message);
                Console.WriteLine(exception);
            }
        }

        public void CheckinStatus(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                ActionCheckAccount(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void UpdateFriendshipCount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                ActionUpdateAccount(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void EditNetworkProfile(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;

                EditProfile(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        public void InstaPhoneVerification(object sender, RoutedEventArgs e)
        {

        }

        public void InstaCheckAccount(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                DominatorAccountModel objDominatorAccountModel =
                    ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                ActionCheckAccount(dominatorAccountModel);

            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }

        public void FacebookRemovePhoneVerification(object sender, RoutedEventArgs e)
        {
            try
            {
                DominatorAccountModel dominatorAccountModel = ((FrameworkElement)sender).DataContext as DominatorAccountModel;
                RemovePhoneVerification(dominatorAccountModel);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
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
