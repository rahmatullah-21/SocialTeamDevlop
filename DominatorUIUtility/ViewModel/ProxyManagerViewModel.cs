using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.Utility;
using DominatorHouseCore.ViewModel;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;
using Prism.Commands;
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

namespace DominatorUIUtility.ViewModel
{
    public interface IProxyManagerViewModel : ITabViewModel
    {
        ObservableCollection<ProxyManagerModel> LstProxyManagerModel { get; }
        ObservableCollection<AccountAssign> AccountsAlreadyAssigned { get; }

        bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel, AccessorStrategies strategy);

        void UpdateProxy(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail,
            AccessorStrategies strategy);

        bool IsProxyAvailable(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount, AccessorStrategies strategy);

        void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, AccessorStrategies strategyPack);
    }

    public class ProxyManagerViewModel : BaseTabViewModel, IProxyManagerViewModel
    {
        private readonly IMainViewModel _mainViewModel;
        private readonly IProxyServerParserService _proxyServerParserService;
        private readonly IAccountsFileManager _accountsFileManager;
        private readonly IAccountCollectionViewModel _accountCollectionViewModel;
        private readonly IProxyFileManager _proxyFileManager;
        private static readonly object _lock = new object();
        private bool _isAddProxyEnabled = true;
        private ProxyManagerModel _proxyManagerModel = new ProxyManagerModel();
        private bool _isAllProxySelected;

        public bool IsAddProxyEnabled
        {
            get { return _isAddProxyEnabled; }
            set { SetProperty(ref _isAddProxyEnabled, value); }
        }

        public IVerifyProxiesViewModel VerifyProxiesViewModel { get; }
        public ObservableCollection<ProxyManagerModel> LstProxyManagerModel { get; }
        public ObservableCollection<string> Groups { get; }

        public ObservableCollection<AccountAssign> AccountsAlreadyAssigned { get; }

        public ProxyManagerModel ProxyManagerModel
        {
            get { return _proxyManagerModel; }
            set { SetProperty(ref _proxyManagerModel, value); }
        }

        private bool _isUnCheckedFromList;
        public bool IsAllProxySelected
        {
            get
            {
                return _isAllProxySelected;
            }
            set
            {
                if (_isAllProxySelected == value)
                    return;
                SetProperty(ref _isAllProxySelected, value);

                SelectAllProxies(_isAllProxySelected);
                _isUnCheckedFromList = false;

            }
        }
        private int _numOfAccountPerProxy = 1;

        public int NumOfAccountPerProxy
        {
            get { return _numOfAccountPerProxy; }
            set { SetProperty(ref _numOfAccountPerProxy, value); }
        }
        private bool _isNumOfAccountPerProxy;
        public bool IsNumOfAccountPerProxy
        {
            get { return _isNumOfAccountPerProxy; }
            set { SetProperty(ref _isNumOfAccountPerProxy, value); }
        }

        private bool _isRandomSelected;

        public bool IsRandomSelected
        {
            get { return _isRandomSelected; }
            set { SetProperty(ref _isRandomSelected, value); }
        }

        #region Commands

        public ICommand AddProxyCommand { get; }
        public ICommand ImportProxyCommand { get; }
        public ICommand ShowByGroupCommand { get; }
        public ICommand ExportProxyCommand { get; }
        public ICommand DeleteCommand { get; }
        public ICommand SelectProxyCommand { get; }
        public ICommand UpdateProxyCommand { get; }
        public ICommand VerifyProxyCommand { get; }
        public ICommand RemoveAccountFromProxyCommand { get; }
        public ICommand AccountToAddToProxyCommand { get; }
        public ICommand DropDownCommand { get; }
        public ICommand AssignRandomProxyCommand { get; }
        public ICommand AssignXAccountPerProxyCommand { get; }
        #endregion

        public ProxyManagerViewModel(IMainViewModel mainViewModel, IVerifyProxiesViewModel verifyProxiesViewModel, IProxyServerParserService proxyServerParserService, IAccountsFileManager accountsFileManager, IAccountCollectionViewModel accountCollectionViewModel, IProxyFileManager proxyFileManager) : base("LangKeyProxyManager", "ProxyManagerControlTemplate")
        {
            _mainViewModel = mainViewModel;
            VerifyProxiesViewModel = verifyProxiesViewModel;
            _proxyServerParserService = proxyServerParserService;
            _accountsFileManager = accountsFileManager;
            _accountCollectionViewModel = accountCollectionViewModel;
            _proxyFileManager = proxyFileManager;

            AddProxyCommand = new DelegateCommand(AddProxyExecute);
            ImportProxyCommand = new DelegateCommand(ImportProxyExecute);
            ShowByGroupCommand = new DelegateCommand<bool?>(ShowByGroupExecute);
            ExportProxyCommand = new DelegateCommand(ExportProxyExecute);
            DeleteCommand = new DelegateCommand<ProxyManagerModel>(DeleteExecute);
            SelectProxyCommand = new DelegateCommand(SelectProxyExecute);
            UpdateProxyCommand = new DelegateCommand<ProxyManagerModel>(UpdateProxyExecute);
            VerifyProxyCommand = new DelegateCommand<ProxyManagerModel>(VerifyProxyExecute);
            RemoveAccountFromProxyCommand = new DelegateCommand<AccountAssign>(RemoveAccountFromProxyExecute);
            AccountToAddToProxyCommand = new DelegateCommand<object>(AccountToAddToProxyExecute);
            DropDownCommand = new BaseCommand<object>(DropDownCanExecute, DropDownExecute);
            AssignRandomProxyCommand = new DelegateCommand<object>(AssignRandomProxyExecute);
            AssignXAccountPerProxyCommand = new DelegateCommand<object>(AssignRandomProxyExecute);
            LstProxyManagerModel = new ObservableCollection<ProxyManagerModel>();
            AccountsAlreadyAssigned = new ObservableCollection<AccountAssign>();
            Groups = new ObservableCollection<string>();
            BindingOperations.EnableCollectionSynchronization(LstProxyManagerModel, _lock);
            BindingOperations.EnableCollectionSynchronization(Groups, _lock);
            StartAddingItems();
        }


        private void StartAddingItems()
        {
            ThreadFactory.Instance.Start(() =>
            {

                try
                {
                    _proxyFileManager.GetAllProxy().ForEach(proxy =>
                    {
                        lock (_lock)
                        {
                            Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                LstProxyManagerModel.Add(proxy);
                                proxy.Index = LstProxyManagerModel.IndexOf(proxy) + 1;
                            });

                            LstProxyManagerModel.ForEach(pr => ProxyManagerModel.Groups.Add(pr.AccountProxy.ProxyGroup));
                            proxy.AccountsAssignedto.ForEach(x =>
                            {
                                AccountsAlreadyAssigned.Add(new AccountAssign
                                {
                                    UserName = x.UserName,
                                    AccountNetwork = x.AccountNetwork
                                });
                            });
                        }

                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            });
        }

        private void SelectAllProxies(bool isAllProxySelected)
        {
            if (_isUnCheckedFromList)
                return;
            ThreadFactory.Instance.Start(() =>
             {
                 LstProxyManagerModel.ForEach(proxy =>
                 {
                     proxy.IsProxySelected = isAllProxySelected;
                 });
             });
        }


        private void AddProxyExecute()
        {
            AddOrUpdateProxyControl objAddProxyControl = new AddOrUpdateProxyControl(this);
            ProxyManagerModel = new ProxyManagerModel();
            try
            {
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(objAddProxyControl, Application.Current.FindResource("LangKeyAddProxy").ToString());
                window.ShowDialog();

            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }
        }

        private void UpdateAccountsToBeAssign(ProxyManagerModel proxyManagerModel)
        {
            _accountsFileManager.GetAll()?.ForEach(account =>
            {
                if (!proxyManagerModel.AccountsToBeAssign.Any(acc =>
                    acc.AccountNetwork == account.AccountBaseModel.AccountNetwork && acc.UserName == account.UserName))
                {
                    proxyManagerModel.AccountsToBeAssign.Add(new AccountAssign
                    {
                        UserName = account.UserName,
                        AccountNetwork = account.AccountBaseModel.AccountNetwork
                    });
                }
            });
        }

        private void ImportProxyExecute()
        {
            var loadedProxylist = FileUtilities.FileBrowseAndReader();
            if (loadedProxylist == null)
                return;
            var allProxy = _proxyFileManager.GetAllProxy();
            int noOfExistingProxies = 0;
            int noOfProxyAdded = 0;


            ThreadFactory.Instance.Start(() =>
            {
                var parsingResult = _proxyServerParserService.ParseProxies(loadedProxylist);
                foreach (var givenProxy in parsingResult.Proxies)
                {
                    try
                    {
                        if (allProxy.Any(x => x.AccountProxy.ProxyIp == givenProxy.AccountProxy.ProxyIp
                                              && x.AccountProxy.ProxyPort == givenProxy.AccountProxy.ProxyPort))

                        {
                            noOfExistingProxies++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(givenProxy.AccountProxy.ProxyGroup))
                            givenProxy.AccountProxy.ProxyGroup = ConstantVariable.UnGrouped;

                        if (string.IsNullOrEmpty(givenProxy.AccountProxy.ProxyName))
                            givenProxy.AccountProxy.ProxyName = "Proxy " + LstProxyManagerModel.Count + 1;

                        UpdateAccountsToBeAssign(givenProxy);
                        _proxyFileManager.SaveProxy(givenProxy);

                        LstProxyManagerModel.Add(givenProxy);
                        givenProxy.Index = LstProxyManagerModel.IndexOf(givenProxy) + 1;
                        noOfProxyAdded++;
                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error(ex);
                    }
                }

                #region Update Import Proxies Status in Logger
                if (noOfExistingProxies > 0)
                {
                    GlobusLogHelper.log.Info(SocialNetworks.Social + $"\t Skipped {noOfExistingProxies} already existing proxie(s)");
                }
                if (noOfProxyAdded > 0)
                {
                    GlobusLogHelper.log.Info(SocialNetworks.Social + $"\t Added {noOfProxyAdded} proxie(s).");
                }
                if (parsingResult.InvalidProxies.Any())
                {
                    var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Socinator";
                    DirectoryUtilities.CreateDirectory(path);
                    var filename = $"{path}\\invalidProxies {ConstantVariable.DateasFileName}.txt";
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        parsingResult.InvalidProxies.ForEach(p =>
                        {
                            streamWriter.WriteLine(p);
                        });
                    }

                    GlobusLogHelper.log.Info(SocialNetworks.Social + $"\t Skipped {parsingResult.InvalidProxies.Count} proxie(s) as it does not match the import format. List of invalid proxies has been exported to {filename}");
                }
                #endregion
            });


        }

        private void ExportProxyExecute()
        {
            var proxiesToExport = LstProxyManagerModel.Where(proxy => proxy.IsProxySelected).ToList();
            if (!proxiesToExport.Any())
            {
                proxiesToExport = _proxyFileManager.GetAllProxy();
            }

            ExportProxies(proxiesToExport);
        }

        private List<ProxyManagerModel> GetSelectedProxies()
        {
            List<ProxyManagerModel> SelectedProxies = LstProxyManagerModel.Where(proxy => proxy.IsProxySelected).ToList();
            if (SelectedProxies.Count == 0)
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Alert",
                    "Please select atleast one Proxy.");
                return SelectedProxies;
            }

            return SelectedProxies;
        }

        private void ExportProxies(List<ProxyManagerModel> Proxies)
        {
            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            string header = "Proxy Group,Proxy name,Proxy IP,Proxy Port,Proxy Username,Proxy Password,Status";

            var filename = $"{exportPath}\\Proxies {ConstantVariable.DateasFileName}.csv";

            try
            {
                if (!File.Exists(filename))
                {
                    using (var streamWriter = new StreamWriter(filename, true))
                    {
                        streamWriter.WriteLine(header);
                    }
                }
                Proxies.ForEach(proxy =>
                {
                    try
                    {
                        var csvData = proxy.AccountProxy.ProxyGroup + ","
                                                                    + proxy.AccountProxy.ProxyName + ","
                                                                    + proxy.AccountProxy.ProxyIp + ","
                                                                    + proxy.AccountProxy.ProxyPort + ","
                                                                    + proxy.AccountProxy.ProxyUsername + ","
                                                                    + proxy.AccountProxy.ProxyPassword + ","
                                                                    + proxy.Status;

                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(csvData);
                            GlobusLogHelper.log.Info(Log.Exported, SocialNetworks.Social, proxy.AccountProxy.ProxyIp + " : " + proxy.AccountProxy.ProxyPort, "LangKeyProxy".FromResourceDictionary());
                        }

                    }
                    catch (Exception ex)
                    {
                        GlobusLogHelper.log.Error("Error in Export Proxies");
                    }
                });
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                    "Success", $"Successfully exported to {filename}");
            }
            catch (Exception ex)
            {

                ex.DebugLog();
            }


        }
        private async void ShowByGroupExecute(bool? isChecked)
        {
            try
            {
                lock (_lock)
                {
                    if (isChecked ?? false)
                        Groups.Add("AccountProxy.ProxyGroup");
                    else
                        Groups.Clear();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        private void DeleteExecute(ProxyManagerModel sender)
        {
            try
            {
                #region Delete Selected proxy

                if (sender == null)
                {

                    var selectedProxies = GetSelectedProxies();

                    if (selectedProxies.Count != 0)
                    {
                        if (ShowWarningMessage() == MessageDialogResult.Affirmative)
                        {
                            Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                selectedProxies.ForEach(selectedProxy =>
                                {
                                    RemoveProxy(selectedProxy);
                                    Thread.Sleep(50);

                                });
                                Dialog.ShowDialog("Success", $"{selectedProxies.Count} proxies successfully Deleted.");
                                GlobusLogHelper.log.Info(Log.Deleted, SocialNetworks.Social, $"{selectedProxies.Count} proxies", "LangKeyProxy".FromResourceDictionary());
                            });



                            IsAllProxySelected = false;
                        }
                    }
                }
                #endregion

                #region Delete current proxy

                else
                {
                    if (ShowWarningMessage() == MessageDialogResult.Affirmative)
                    {
                        RemoveProxy(sender);
                        Application.Current.Dispatcher.Invoke(() => DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                       $"{sender.AccountProxy.ProxyIp}:{sender.AccountProxy.ProxyPort} Successfully DeletedDateText."));
                    }
                }

                #endregion
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private MessageDialogResult ShowWarningMessage()
        {
            return DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow,
                "Warning", "Proxy(ies) will remove from all account associated with this proxy(ies)\nAre you sure ?"
                , MessageDialogStyle.AffirmativeAndNegative, Dialog.SetMetroDialogButton("Yes", "No"));
        }

        private void RemoveProxy(ProxyManagerModel selectedProxy)
        {
            try
            {
                _proxyFileManager.Delete(proxy => proxy.AccountProxy.ProxyId == selectedProxy.AccountProxy.ProxyId);
                RemoveProxyFromAccount(selectedProxy);

                LstProxyManagerModel.Remove(selectedProxy);

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void RemoveProxyFromAccount(ProxyManagerModel selectedProxy)
        {
            try
            {
                _accountCollectionViewModel.GetCopySync().Where(account =>
                    account.AccountBaseModel.AccountProxy.ProxyIp == selectedProxy.AccountProxy.ProxyIp
                    && account.AccountBaseModel.AccountProxy.ProxyPort == selectedProxy.AccountProxy.ProxyPort).ForEach(
                    account =>
                    {
                        AccountsAlreadyAssigned.Remove(AccountsAlreadyAssigned.FirstOrDefault(x =>
                            x.UserName == account.UserName &&
                            x.AccountNetwork == account.AccountBaseModel.AccountNetwork));


                        account.AccountBaseModel.AccountProxy = new Proxy();

                        new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                             .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                             .SaveToBinFile();
                    });


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void SelectProxyExecute()
        {
            if (LstProxyManagerModel.All(x => x.IsProxySelected))
                IsAllProxySelected = true;
            else
            {
                if (IsAllProxySelected)
                    _isUnCheckedFromList = true;
                IsAllProxySelected = false;
            }

        }

        private void UpdateProxyExecute(ProxyManagerModel currentProxy)
        {
            var oldProxy = _proxyFileManager.GetProxyById(currentProxy.AccountProxy.ProxyId);
            if (!Proxy.IsValidProxy(currentProxy.AccountProxy.ProxyIp, currentProxy.AccountProxy.ProxyPort))
            {
                DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                    "Please enter valid proxy.");
                return;
            }

            bool isAvailable = false;
            LstProxyManagerModel.ForEach(proxy =>
            {
                if (proxy.AccountProxy.ProxyId != currentProxy.AccountProxy.ProxyId)
                {
                    if (proxy.AccountProxy.ProxyName == currentProxy.AccountProxy.ProxyName)
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                            $"Proxy with name {currentProxy.AccountProxy.ProxyName} is already Exist.");
                        currentProxy.AccountProxy.ProxyName = oldProxy.AccountProxy.ProxyName;
                        isAvailable = true;
                    }

                    else if (proxy.AccountProxy.ProxyIp == currentProxy.AccountProxy.ProxyIp &&
                        proxy.AccountProxy.ProxyPort == currentProxy.AccountProxy.ProxyPort)
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
                            $"Proxy with Ip {currentProxy.AccountProxy.ProxyIp} port {currentProxy.AccountProxy.ProxyPort} is already Exist.");
                        currentProxy.AccountProxy = oldProxy.AccountProxy;
                        isAvailable = true;
                        // ReSharper disable once RedundantJumpStatement
                        return;
                    }

                }
            });
            if (isAvailable)
                return;

            var accountToUpdateProxy
                = _accountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountProxy.ProxyIp == oldProxy.AccountProxy.ProxyIp
                                                          && x.AccountBaseModel.AccountProxy.ProxyPort == oldProxy.AccountProxy.ProxyPort).ToList();

            oldProxy = currentProxy;

            _proxyFileManager.EditProxy(oldProxy);

            accountToUpdateProxy.ForEach(acc =>
            {
                acc.AccountBaseModel.AccountProxy = oldProxy.AccountProxy;

                new SocinatorAccountBuilder(acc.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(acc.AccountBaseModel)
                    .SaveToBinFile();

                var accountToUpdate = _accountsFileManager.GetAccount(acc.UserName, acc.AccountBaseModel.AccountNetwork);
                UpdateAccountsProxy(accountToUpdate, _mainViewModel.Strategies);
            });
            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                $"{oldProxy.AccountProxy.ProxyIp}:{oldProxy.AccountProxy.ProxyPort} Successfully updated.");

        }

        private async void VerifyProxyExecute(ProxyManagerModel currentProxyManager)
        {
            if (currentProxyManager != null)
            {
                await VerifyProxiesViewModel.Verify(currentProxyManager);
                return;
            }

            await VerifyProxiesViewModel.Verify(LstProxyManagerModel.Where(a => a.IsProxySelected).ToArray());
        }


        private void RemoveAccountFromProxyExecute(AccountAssign account)
        {
            var proxy = _proxyFileManager.GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyIp == ProxyManagerModel.AccountProxy.ProxyIp
            && x.AccountProxy.ProxyPort == ProxyManagerModel.AccountProxy.ProxyPort);


            //var proxy = ProxyFileManager.GetProxyById(CurrentProxyManagerModel.AccountProxy.ProxyId);
            var accountToDelete = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName
                                                                               && x.AccountNetwork == account.AccountNetwork);
            try
            {
                proxy.AccountsAssignedto.Remove(accountToDelete);

                var item = LstProxyManagerModel.FirstOrDefault(Proxy => Proxy.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp &&
                                                                        Proxy.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort);
                int indexToUpdate = LstProxyManagerModel.IndexOf(item);
                LstProxyManagerModel[indexToUpdate].AccountsAssignedto = proxy.AccountsAssignedto;
                // LstProxyManagerModel[indexToUpdate].AccountsToBeAssign.Add(accountToDelete);

                var accountToDeleteProxy = _accountsFileManager.GetAccount(account.UserName, account.AccountNetwork);
                accountToDeleteProxy.AccountBaseModel.AccountProxy = new Proxy();

                // AccountsFileManager.Edit(accountToDeleteProxy);

                var socinatorAccountBuilder = new SocinatorAccountBuilder(accountToDeleteProxy.AccountBaseModel.AccountId);
                socinatorAccountBuilder.AddOrUpdateDominatorAccountBase(accountToDeleteProxy.AccountBaseModel)
                    .SaveToBinFile();

                UpdateAccountsProxy(accountToDeleteProxy, _mainViewModel.Strategies);
                StartLogin(accountToDeleteProxy, socinatorAccountBuilder);
                LstProxyManagerModel.ForEach(oldProxy =>
                 AccountsAlreadyAssigned.Remove(AccountsAlreadyAssigned.FirstOrDefault(x => x.UserName == accountToDelete.UserName && x.AccountNetwork == accountToDelete.AccountNetwork))
               );
                _proxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        private void AccountToAddToProxyExecute(object sender)
        {
            AccountAssign account = sender as AccountAssign;

            try
            {
                var accountToAdd = ProxyManagerModel.AccountsToBeAssign.FirstOrDefault(x => x.UserName == account.UserName);
                LstProxyManagerModel.ForEach(proxy =>
                {
                    proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x => x.UserName == accountToAdd.UserName
                                                                                               && x.AccountNetwork == accountToAdd.AccountNetwork));
                });
                if (!ProxyManagerModel.AccountsAssignedto.Any(x => x.UserName == account.UserName
                                                                       && x.AccountNetwork == account.AccountNetwork))
                {
                    ProxyManagerModel.AccountsAssignedto.Add(accountToAdd);
                    AccountsAlreadyAssigned.Add(accountToAdd);
                }

                _proxyFileManager.EditProxy(ProxyManagerModel);
                var accountToUpdateProxy = _accountsFileManager.GetAccount(account.UserName, account.AccountNetwork);
                var proxyToAdd = new Proxy()
                {
                    ProxyId = ProxyManagerModel.AccountProxy.ProxyId,
                    ProxyPort = ProxyManagerModel.AccountProxy.ProxyPort,
                    ProxyIp = ProxyManagerModel.AccountProxy.ProxyIp,
                    ProxyUsername = ProxyManagerModel.AccountProxy.ProxyUsername,
                    ProxyPassword = ProxyManagerModel.AccountProxy.ProxyPassword
                };
                accountToUpdateProxy.AccountBaseModel.AccountProxy = proxyToAdd;

                var socinatorAccountBuilder = new SocinatorAccountBuilder(accountToUpdateProxy.AccountBaseModel.AccountId);
                socinatorAccountBuilder.AddOrUpdateDominatorAccountBase(accountToUpdateProxy.AccountBaseModel)
                    .SaveToBinFile();
                UpdateAccountsProxy(accountToUpdateProxy, _mainViewModel.Strategies);
                StartLogin(accountToUpdateProxy, socinatorAccountBuilder);

                var item = LstProxyManagerModel.FirstOrDefault(proxy => proxy.AccountProxy.ProxyName == ProxyManagerModel.AccountProxy.ProxyName);
                int indexToUpdate = LstProxyManagerModel.IndexOf(item);
                LstProxyManagerModel[indexToUpdate].AccountsAssignedto = ProxyManagerModel.AccountsAssignedto;
                LstProxyManagerModel[indexToUpdate].AccountsToBeAssign = ProxyManagerModel.AccountsToBeAssign;


                _proxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void StartLogin(DominatorAccountModel accountToUpdateProxy, SocinatorAccountBuilder socinatorAccountBuilder)
        {
            Task.Factory.StartNew(async () =>
            {
                try
                {
                    var networkCoreFactory = SocinatorInitialize
                        .GetSocialLibrary(accountToUpdateProxy.AccountBaseModel.AccountNetwork)
                        .GetNetworkCoreFactory();

                    var asyncAccount = (IAccountUpdateFactoryAsync)networkCoreFactory.AccountUpdateFactory;

                    accountToUpdateProxy.AccountBaseModel.Status = AccountStatus.TryingToLogin;
                    UpdateUserInfoCountToAccountManagerUi(accountToUpdateProxy);
                    await asyncAccount.CheckStatusAsync(accountToUpdateProxy, accountToUpdateProxy.Token);
                    if (accountToUpdateProxy.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        await asyncAccount.UpdateDetailsAsync(accountToUpdateProxy, accountToUpdateProxy.Token);
                        socinatorAccountBuilder.UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                            .SaveToBinFile();
                    }
                    else
                    {
                        UpdateUserInfoCountToAccountManagerUi(accountToUpdateProxy);

                        socinatorAccountBuilder
                            .AddOrUpdateDisplayColumn1(accountToUpdateProxy.DisplayColumnValue1)
                            .AddOrUpdateDisplayColumn2(accountToUpdateProxy.DisplayColumnValue2)
                            .AddOrUpdateDisplayColumn3(accountToUpdateProxy.DisplayColumnValue3)
                            .AddOrUpdateDisplayColumn4(accountToUpdateProxy.DisplayColumnValue4)
                            .AddOrUpdateProxy(accountToUpdateProxy.AccountBaseModel.AccountProxy)
                            .AddOrUpdateMailCredentials(accountToUpdateProxy.MailCredentials)
                            .AddOrUpdateIsAutoVerifyByEmail(accountToUpdateProxy.IsAutoVerifyByEmail)
                            .SaveToBinFile();
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private static void UpdateUserInfoCountToAccountManagerUi(DominatorAccountModel account)
        {
            account.DisplayColumnValue1 = 0;
            account.DisplayColumnValue2 = 0;
            account.DisplayColumnValue3 = 0;
            account.DisplayColumnValue4 = 0;
        }

        private void UpdateAccountsProxy(DominatorAccountModel accountToUpdateProxy, AccessorStrategies strategies)
        {
            try
            {
                var updateAccountsDetails = _accountCollectionViewModel.GetCopySync().FirstOrDefault(x =>
                    x.AccountBaseModel.AccountId == accountToUpdateProxy.AccountBaseModel.AccountId);

                if (updateAccountsDetails != null)
                    updateAccountsDetails.AccountBaseModel.AccountProxy =
                        accountToUpdateProxy.AccountBaseModel.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool DropDownCanExecute(object sender) => true;

        private void DropDownExecute(object sender)
        {
            ProxyManagerModel = sender as ProxyManagerModel;
            AvailableAccountsToBeAssigned(ProxyManagerModel);
        }

        private void AvailableAccountsToBeAssigned(ProxyManagerModel proxyManagerModel)
        {
            try
            {
                proxyManagerModel.AccountsToBeAssign.Clear();
                _accountsFileManager.GetAll()?.ForEach(account =>
                   {

                       if (!AccountsAlreadyAssigned.Any(acc =>
                           acc.AccountNetwork == account.AccountBaseModel.AccountNetwork && acc.UserName == account.UserName))
                       {
                           if (!proxyManagerModel.AccountsToBeAssign.Any(x => x.UserName == account.UserName && x.AccountNetwork == account.AccountBaseModel.AccountNetwork))
                           {
                               proxyManagerModel.AccountsToBeAssign.Add(new AccountAssign
                               {
                                   UserName = account.UserName,
                                   AccountNetwork = account.AccountBaseModel.AccountNetwork
                               });
                           }
                       }
                   });
            }
            catch (Exception ex)
            {

                ex.ErrorLog();
            }

        }
        private void AssignRandomProxyExecute(object sender)
        {
            try
            {
                var accounts = _accountCollectionViewModel.GetCopySync().Where(x => string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyIp) && string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyPort)).ToList();

                if (LstProxyManagerModel.Count != 0)
                {
                    Random random = new Random();
                    int randomIndex = 0;
                    accounts.ForEach(acc =>
                      {
                          if (IsNumOfAccountPerProxy && !IsRandomSelected)
                              randomIndex = LstProxyManagerModel.IndexOf(LstProxyManagerModel.FirstOrDefault(x => x.AccountsAssignedto.Count < NumOfAccountPerProxy));
                          else
                              randomIndex = random.Next(LstProxyManagerModel.Count);
                          new SocinatorAccountBuilder(acc.AccountBaseModel.AccountId)
                              .AddOrUpdateProxy(LstProxyManagerModel[randomIndex].AccountProxy)
                              .SaveToBinFile();
                          var accountAssignTo = new AccountAssign
                          {
                              UserName = acc.UserName,
                              AccountNetwork = acc.AccountBaseModel.AccountNetwork
                          };

                          ProxyManagerModel = LstProxyManagerModel[randomIndex];
                          ProxyManagerModel.AccountsAssignedto.Add(accountAssignTo);
                          AccountsAlreadyAssigned.Add(accountAssignTo);
                          AccountToAddToProxyExecute(accountAssignTo);


                      });

                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
            }

        }

        #region Methods for account proxy updation
        public void UpdateProxy(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail, AccessorStrategies strategy)
        {
            try
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
                            _proxyFileManager.EditProxy(proxy);
                            LstProxyManagerModel
                                .FirstOrDefault(x => x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId)
                                .AccountsAssignedto = proxy.AccountsAssignedto;
                            AccountsAlreadyAssigned.
                                Remove(AccountsAlreadyAssigned.FirstOrDefault(x => x.UserName == objAccountBaseModel.UserName
                                                                                                                      && x.AccountNetwork == objAccountBaseModel.AccountNetwork));
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                #endregion

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public bool IsProxyAvailable(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> oldProxies,
       DominatorAccountBaseModel oldAccount, AccessorStrategies strategy)
        {
            bool isDuplicatProxyAvailable = false;
            foreach (var proxy in oldProxies)
            {
                #region To check for available proxy 
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
                                        var proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == oldAccount.AccountProxy.ProxyIp
                                                                                                                                        && x.AccountProxy.ProxyPort == oldAccount.AccountProxy.ProxyPort);
                                        proxyToUpdate?.AccountsAssignedto.Remove(proxyToUpdate.AccountsAssignedto.FirstOrDefault(x => x.UserName == oldAccount.UserName && x.AccountNetwork == oldAccount.AccountNetwork));

                                        proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
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
                                        var proxyToUpdate = LstProxyManagerModel.FirstOrDefault(x => x.AccountProxy.ProxyIp == objAccountBaseModel.AccountProxy.ProxyIp
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
                                AccountsAlreadyAssigned.Add(
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
        public async Task<bool> IsProxyUpdated(DominatorAccountBaseModel objDominatorAccountBaseModel, List<ProxyManagerModel> oldProxies,
            DominatorAccountBaseModel oldAccount, AccessorStrategies strategy)
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

                        await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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
                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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

                            await _proxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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
        public void UpdateProxyList(ProxyManagerModel proxy)
        {
            try
            {

                var proxyToupdate = LstProxyManagerModel.FirstOrDefault(x =>
                    x.AccountProxy.ProxyId == proxy.AccountProxy.ProxyId);

                if (proxyToupdate != null)
                    proxyToupdate.AccountProxy = proxy.AccountProxy;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public async void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, AccessorStrategies strategyPack)
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

            _proxyFileManager.GetAllProxy().ForEach(proxy =>
            {
                _accountsFileManager.GetAll().ForEach(acc =>
                {
                    if (proxy.AccountsAssignedto.Any(x =>
                        x.UserName == acc.UserName && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork))
                    {
                        proxy.AccountsAssignedto.Remove(proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName ==
                            acc.UserName
                                && x.AccountNetwork == acc.AccountBaseModel.AccountNetwork));

                    }
                });

                _proxyFileManager.EditProxy(proxy);
            });

            #endregion

            Application.Current.Dispatcher.Invoke(() =>
            {
                LstProxyManagerModel.ForEach(x =>
                {
                    if (x.AccountsAssignedto.Any(y => y.UserName == objAccount.UserName &&
                                                      y.AccountNetwork == objAccount.AccountNetwork))
                        x.AccountsAssignedto.Remove(x.AccountsAssignedto.FirstOrDefault(y =>
                            y.UserName == objAccount.UserName &&
                            y.AccountNetwork == objAccount.AccountNetwork));
                });
                LstProxyManagerModel.Add(ProxyManagerModel);
                ProxyManagerModel.Index = LstProxyManagerModel.IndexOf(ProxyManagerModel) + 1;
                AccountsAlreadyAssigned.Add(
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

            await _proxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);



        }
        public bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel, AccessorStrategies strategy)
        {

            var oldproxies = _proxyFileManager.GetAllProxy();

            bool isProxyUpdated = false;
            try
            {
                var oldAccount = _accountsFileManager.GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork)?.AccountBaseModel;

                isProxyUpdated = IsProxyUpdated(objDominatorAccountBaseModel, oldproxies, oldAccount, strategy).Result;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return isProxyUpdated;
        }

        #endregion
    }

}
