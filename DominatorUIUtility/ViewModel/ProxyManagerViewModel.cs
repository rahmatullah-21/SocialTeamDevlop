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
using DominatorHouseCore;
using DominatorHouseCore.Command;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.ViewModel
{
    public class ProxyManagerViewModel : BindableBase
    {
        public ProxyManagerViewModel()
        {

            AddProxyCommand = new BaseCommand<object>(AddProxyCanExecute, AddProxyExecute);
            ImportProxyCommand = new BaseCommand<object>(ImportProxyCanExecute, ImportProxyExecute);
            ShowByGroupCommand = new BaseCommand<object>(ShowByGroupCanExecute, ShowByGroupExecute);
            ExportProxyCommand = new BaseCommand<object>(ExportProxyCanExecute, ExportProxyExecute);
            HideUnhideSocialProfilesCommand = new BaseCommand<object>(HideUnhideSocialProfilesCanExecute, HideUnhideSocialProfilesExecute);
            HideUnhideUserPasswordCommand = new BaseCommand<object>(HideUnhideUserPasswordCanExecute, HideUnhideUserPasswordExecute);
            ShowProxiesWithErrorCommand = new BaseCommand<object>(ShowProxiesWithErrorCanExecute, ShowProxiesWithErrorExecute);
            ShowUnassignedProxiesCommand = new BaseCommand<object>(ShowUnassignedProxiesCanExecute, ShowUnassignedProxiesExecute);
            GroupsChangedCommand = new BaseCommand<object>(GroupsChangedCanExecute, GroupsChangedExecute);
            DeleteCommand = new BaseCommand<object>(DeleteCanExecute, DeleteExecute);
            SelectProxyCommand = new BaseCommand<object>(SelectProxyCanExecute, SelectProxyExecute);
            UpdateProxyCommand = new BaseCommand<object>(UpdateProxyCanExecute, UpdateProxyExecute);
            VerifyProxyCommand = new BaseCommand<object>(VerifyProxyCanExecute, VerifyProxyExecute);
            RemoveAccountFromProxyCommand = new BaseCommand<object>(RemoveAccountFromProxyCanExecute, RemoveAccountFromProxyExecute);
            AccountToAddToProxyCommand = new BaseCommand<object>(AccountToAddToProxyCanExecute, AccountToAddToProxyExecute);
            DropDownCommand = new BaseCommand<object>(DropDownCanExecute, DropDownExecute);
            AssignRandomProxyCommand = new BaseCommand<object>(AssignRandomProxyCanExecute, AssignRandomProxyExecute);
            BindingOperations.EnableCollectionSynchronization(LstProxyManagerModel, _lock);
           

        }


        #region Commands

        public ICommand UnLoadCommand { get; set; }
        public ICommand AddProxyCommand { get; set; }
        public ICommand ImportProxyCommand { get; set; }
        public ICommand ShowByGroupCommand { get; set; }
        public ICommand ExportProxyCommand { get; set; }
        public ICommand HideUnhideSocialProfilesCommand { get; set; }
        public ICommand HideUnhideUserPasswordCommand { get; set; }
        public ICommand ShowProxiesWithErrorCommand { get; set; }
        public ICommand ShowUnassignedProxiesCommand { get; set; }
        public ICommand GroupsChangedCommand { get; set; }
        public ICommand DeleteCommand { get; set; }
        public ICommand SelectProxyCommand { get; set; }
        public ICommand UpdateProxyCommand { get; set; }
        public ICommand VerifyProxyCommand { get; set; }
        public ICommand RemoveAccountFromProxyCommand { get; set; }
        public ICommand AccountToAddToProxyCommand { get; set; }
        public ICommand DropDownCommand { get; set; }
        public ICommand AssignRandomProxyCommand { get; set; }
        #endregion

        #region Properies
        public DominatorAccountViewModel.AccessorStrategies _strategies { get; set; }
        ProxyManagerModel CurrentProxyManagerModel { get; set; }
        private ICollectionView _proxyManagerCollection;
        public ICollectionView ProxyManagerCollection
        {
            get
            {
                return _proxyManagerCollection;
            }
            set
            {
                if (_proxyManagerCollection != null && _proxyManagerCollection == value)
                    return;
                SetProperty(ref _proxyManagerCollection, value);
            }
        }
        private ObservableCollection<ProxyManagerModel> lstProxyManagerModel = new ObservableCollection<ProxyManagerModel>();

        public ObservableCollection<ProxyManagerModel> LstProxyManagerModel
        {
            get
            {
                return lstProxyManagerModel;
            }
            set
            {

                if (lstProxyManagerModel == value)
                    return;
                SetProperty(ref lstProxyManagerModel, value);
            }
        }

        private ProxyManagerModel _proxyManagerModel = new ProxyManagerModel();
        public ProxyManagerModel ProxyManagerModel
        {
            get
            {
                return _proxyManagerModel;
            }
            set
            {
                if (_proxyManagerModel == value)
                    return;
                SetProperty(ref _proxyManagerModel, value);
            }
        }

        private string _filter;

        public ObservableCollection<AccountAssign> accountsAlreadyAssigned { get; set; } =
            new ObservableCollection<AccountAssign>();

        public string Filter
        {
            get
            {
                return _filter;
            }
            set
            {
                if (_filter == value)
                    return;
                SetProperty(ref _filter, value);
               if (string.IsNullOrEmpty(_filter))
                    ProxyManagerCollection.Filter = null;
                else
                    ProxyManagerCollection.Filter += FilterByNameOrIp;
            }
        }
        private bool _isAllProxySelected;

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
                _isUncheckedFromLstProxyManagerModel = false;
            }
        }
        private bool _isUncheckedFromLstProxyManagerModel { get; set; }
        private string _uRLToUseToVerifyProxies = "https://www.google.com";

        public string URLToUseToVerifyProxies
        {
            get
            {
                return _uRLToUseToVerifyProxies;
            }
            set
            {
                if (_uRLToUseToVerifyProxies == value)
                    return;
                SetProperty(ref _uRLToUseToVerifyProxies, value);
            }
        }

        private bool _isAddProxyEnabled = true;

        public bool IsAddProxyEnabled
        {
            get
            {
                return _isAddProxyEnabled;
            }
            set
            {
                if (_isAddProxyEnabled == value)
                    return;
                SetProperty(ref _isAddProxyEnabled, value);
            }
        }

        #endregion

        private static object _lock = new object();

        public void StartAddingItems()
        {
            ThreadFactory.Instance.Start(() =>
            {

                try
                {
                    ProxyFileManager.GetAllProxy().ForEach(proxy =>
                    {
                        Application.Current.Dispatcher.InvokeAsync(() => lstProxyManagerModel.Add(proxy));
                        Thread.Sleep(50);
                        LstProxyManagerModel.ForEach(pr => ProxyManagerModel.Groups.Add(pr.AccountProxy.ProxyGroup));
                        proxy.AccountsAssignedto.ForEach(x =>
                        {
                            accountsAlreadyAssigned.Add(new AccountAssign
                            {
                                UserName = x.UserName,
                                AccountNetwork = x.AccountNetwork
                            });
                        });

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
            if (_isUncheckedFromLstProxyManagerModel)
                return;
            ThreadFactory.Instance.Start(() =>
            {
                LstProxyManagerModel.Select(proxy =>
                {
                    proxy.IsProxySelected = isAllProxySelected;
                    return proxy;
                }).ToList();
            });
        }


        private void AddProxyExecute(object sender)
        {
            AddOrUpdateProxyControl objAddProxyControl = new AddOrUpdateProxyControl(this);
            ProxyManagerModel = new ProxyManagerModel();
            try
            {
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(objAddProxyControl, Application.Current.FindResource("LangKeyAddProxy").ToString());
                window.Show();
                window.Closing += (sr, ev) => IsAddProxyEnabled = true;
                IsAddProxyEnabled = false;
            }
            catch (Exception ex)
            {
                ex.DebugLog();

            }
        }

        private bool AddProxyCanExecute(object sender) => true;
        private void UpdateAccountsToBeAssign(ProxyManagerModel proxyManagerModel)
        {
            AccountsFileManager.GetAll()?.ForEach(account =>
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
        private void ImportProxyExecute(object sender)
        {
            var loadedProxylist = FileUtilities.FileBrowseAndReader();
            if (loadedProxylist == null)
                return;
            var allProxy = ProxyFileManager.GetAllProxy();
            int noOfExistingProxies = 0;
            int noOfProxyAdded = 0;
            int noOfInvalidProxies = 0;
            List<string> lstInvalidProxies = new List<string>();


            ThreadFactory.Instance.Start(() =>
            {
                foreach (var givenProxy in loadedProxylist)
                {
                    try
                    {
                       // var proxy = givenProxy.Replace(",", ":");
                        var proxy = givenProxy;

                        var selectedProxy = Regex.Split(proxy, "\t");
                        if (selectedProxy.Length < 2)
                            continue;

                        ProxyManagerModel = new ProxyManagerModel();

                        if (selectedProxy.Length == 2 || selectedProxy.Length == 4)
                        {
                            if (!Proxy.IsValidProxy(selectedProxy[0], selectedProxy[1]))
                            {
                                noOfInvalidProxies++;
                                lstInvalidProxies.Add(proxy);
                                continue;
                            }

                            ProxyManagerModel.AccountProxy.ProxyIp = selectedProxy[0];
                            ProxyManagerModel.AccountProxy.ProxyPort = selectedProxy[1];
                        }

                        if (selectedProxy.Length == 4)
                        {
                            ProxyManagerModel.AccountProxy.ProxyUsername = selectedProxy[2];
                            ProxyManagerModel.AccountProxy.ProxyPassword = selectedProxy[3];
                        }

                        if (selectedProxy.Length == 6)
                        {
                            ProxyManagerModel.AccountProxy.ProxyGroup = selectedProxy[0];
                            ProxyManagerModel.AccountProxy.ProxyName = selectedProxy[1];
                            ProxyManagerModel.AccountProxy.ProxyIp = selectedProxy[2];
                            ProxyManagerModel.AccountProxy.ProxyPort = selectedProxy[3];
                            ProxyManagerModel.AccountProxy.ProxyUsername = selectedProxy[4];
                            ProxyManagerModel.AccountProxy.ProxyPassword = selectedProxy[5];
                        }

                        if (!Proxy.IsValidProxy(ProxyManagerModel.AccountProxy.ProxyIp,
                            ProxyManagerModel.AccountProxy.ProxyPort))
                        {
                            noOfInvalidProxies++;
                            lstInvalidProxies.Add(proxy);
                            continue;
                        }

                        if (allProxy.Any(x => x.AccountProxy.ProxyIp == ProxyManagerModel.AccountProxy.ProxyIp
                                              && x.AccountProxy.ProxyPort == ProxyManagerModel.AccountProxy.ProxyPort))

                        {
                            noOfExistingProxies++;
                            continue;
                        }

                        if (string.IsNullOrEmpty(ProxyManagerModel.AccountProxy.ProxyGroup))
                            ProxyManagerModel.AccountProxy.ProxyGroup = ConstantVariable.UnGrouped;

                        if (string.IsNullOrEmpty(ProxyManagerModel.AccountProxy.ProxyName))
                            ProxyManagerModel.AccountProxy.ProxyName = "Proxy " + LstProxyManagerModel.Count + 1;

                        UpdateAccountsToBeAssign(ProxyManagerModel);
                        ProxyFileManager.SaveProxy(ProxyManagerModel);

                        LstProxyManagerModel.Add(ProxyManagerModel);
                        noOfProxyAdded++;
                        Thread.Sleep(50);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
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
                if (noOfInvalidProxies > 0)
                {
                    var Path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\Socinator";
                    DirectoryUtilities.CreateDirectory(Path);
                    var filename = $"{Path}\\invalidProxies {ConstantVariable.DateasFileName}.txt";
                    lstInvalidProxies.ForEach(p =>
                    {
                        using (var streamWriter = new StreamWriter(filename, true))
                        {
                            streamWriter.WriteLine(p);
                        }
                    });

                    GlobusLogHelper.log.Info(SocialNetworks.Social + $"\t Skipped {noOfInvalidProxies} proxie(s) as it does not match the import format. List of invalid proxies has been exported to {filename}");
                }
                #endregion
            });


        }

        private bool ImportProxyCanExecute(object sender) => true;
        private bool ExportProxyCanExecute(object sender) => true;

        private void ExportProxyExecute(object sender)
        {
            if (sender == null)
            {
                var allProxies = ProxyFileManager.GetAllProxy();
                ExportProxies(allProxies);
            }
            else
            {
                var SelectedProxies = GetSelectedProxies();

                if (SelectedProxies.Count != 0)
                {
                    ExportProxies(SelectedProxies);
                }
            }
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


            }


        }
        private void ShowByGroupExecute(object sender)
        {

            try
            {
                Application.Current.Dispatcher.BeginInvoke((Action)(() =>
                {
                    if ((bool)sender)
                        ProxyManagerCollection.GroupDescriptions.Add(
                            new PropertyGroupDescription("AccountProxy.ProxyGroup"));
                    else
                        ProxyManagerCollection.GroupDescriptions.Clear();
                }));

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        private bool ShowByGroupCanExecute(object sender) => true;
        private bool HideUnhideSocialProfilesCanExecute(object sender) => true;

        private void HideUnhideSocialProfilesExecute(object sender)
        {

            try
            {
                var senderList = sender as List<object>;
                var dataGrid = ((FrameworkElement)senderList[0]) as DataGrid;

                var columnToChangeVisiblity = dataGrid.Columns.Single(gridColumn => gridColumn.Header.ToString() ==
                                               Application.Current.FindResource("LangKeySocialProfiles").ToString());
                if ((bool)senderList[1])
                    columnToChangeVisiblity.Visibility = Visibility.Collapsed;
                else
                    columnToChangeVisiblity.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {

            }
        }

        private bool HideUnhideUserPasswordCanExecute(object sender) => true;

        private void HideUnhideUserPasswordExecute(object sender)
        {
            try
            {
                var senderList = sender as List<object>;
                var dataGrid = ((FrameworkElement)senderList[0]) as DataGrid;

                var proxyUserVisiblity = dataGrid.Columns.Single(gridColumn => gridColumn.Header.ToString() ==
                                                                                    Application.Current.FindResource("LangKeyProxyUsername").ToString());
                var proxyPasswordVisiblity = dataGrid.Columns.Single(gridColumn => gridColumn.Header.ToString() ==
                                                                                    Application.Current.FindResource("LangKeyProxyPassword").ToString());
                if ((bool)senderList[1])
                {
                    proxyUserVisiblity.Visibility = Visibility.Collapsed;
                    proxyPasswordVisiblity.Visibility = Visibility.Collapsed;
                }
                else
                {
                    proxyUserVisiblity.Visibility = Visibility.Visible;
                    proxyPasswordVisiblity.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool ShowProxiesWithErrorCanExecute(object sender) => true;

        private void ShowProxiesWithErrorExecute(object sender)
        {
            if ((bool)sender)
            {
                ProxyManagerCollection.Filter += FilterByProxiesWithError;
            }
            else
                ProxyManagerCollection.Filter = (object ob) => { return true; };
        }
        private bool FilterByProxiesWithError(object GroupName)
        {
            try
            {
                ProxyManagerModel ProxyGroup = GroupName as ProxyManagerModel;
                return ProxyGroup.Status.IndexOf("Fail", StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception)
            {
            }
            return true;
        }
        private bool ShowUnassignedProxiesCanExecute(object sender) => true;

        private void ShowUnassignedProxiesExecute(object sender)
        {
            ObservableCollection<ProxyManagerModel> unassignedProxies = new ObservableCollection<ProxyManagerModel>();

            if ((bool)sender)
            {
                foreach (var proxy in LstProxyManagerModel)
                {
                    if (proxy.AccountsAssignedto.Count == 0)
                    {
                        proxy.AccountsToBeAssign = proxy.AccountsToBeAssign;
                        unassignedProxies.Add(proxy);
                    }

                }
                ProxyManagerCollection = CollectionViewSource.GetDefaultView(unassignedProxies);
            }
            else
            {
                ProxyManagerCollection = CollectionViewSource.GetDefaultView(LstProxyManagerModel);
            }
        }
        private bool GroupsChangedCanExecute(object sender) => true;

        private void GroupsChangedExecute(object sender)
        {
            try
            {
                ProxyManagerCollection.Filter = proxy =>
                {
                    ProxyManagerModel proxyManagerModel = proxy as ProxyManagerModel;

                    return proxyManagerModel.AccountProxy.ProxyGroup.IndexOf(sender.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
                };

            }
            catch (Exception ex)
            {
                ProxyManagerCollection.Filter = null;
                ex.DebugLog();
            }
        }
        private bool FilterByNameOrIp(object nameOrIp)
        {
            try
            {
                ProxyManagerModel ProxyGroup = nameOrIp as ProxyManagerModel;
                return ProxyGroup.AccountProxy.ProxyIp.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0
                       || ProxyGroup.AccountProxy.ProxyName.IndexOf(Filter, StringComparison.InvariantCultureIgnoreCase) >= 0;


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return false;
        }

        private bool DeleteCanExecute(object sender) => true;

        private void DeleteExecute(object sender)
        {
            try
            {
                #region Delete Selected proxy

                if (sender == null)
                {

                    var SelectedProxies = GetSelectedProxies();

                    if (SelectedProxies.Count != 0)
                    {
                        if (ShowWarningMessage() == MessageDialogResult.Affirmative)
                        {
                            Application.Current.Dispatcher.InvokeAsync(() =>
                            {
                                SelectedProxies.ForEach(selectedProxy =>
                                {
                                    RemoveProxy(selectedProxy);
                                    Thread.Sleep(50);

                                });
                                DialogCoordinator.Instance.ShowModalMessageExternal(
                                    Application.Current.MainWindow, "Success",
                                    $"{SelectedProxies.Count} proxies successfully DeletedDateText.");
                                GlobusLogHelper.log.Info(Log.Deleted, SocialNetworks.Social, $"{SelectedProxies.Count} proxies", "LangKeyProxy".FromResourceDictionary());
                            });



                            IsAllProxySelected = false;
                        }
                    }
                }
                #endregion

                #region Delete current proxy

                else
                {
                    var currentProxy = ((FrameworkElement)sender).DataContext as ProxyManagerModel;

                    if (currentProxy != null && ShowWarningMessage() == MessageDialogResult.Affirmative)
                    {
                        RemoveProxy(currentProxy);
                        Application.Current.Dispatcher.Invoke(() => DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                       $"{currentProxy.AccountProxy.ProxyIp}:{currentProxy.AccountProxy.ProxyPort} Successfully DeletedDateText."));
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
                ProxyFileManager.Delete(proxy => proxy.AccountProxy.ProxyId == selectedProxy.AccountProxy.ProxyId);
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
                var accountCustomControl = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social, _strategies);

                accountCustomControl.DominatorAccountViewModel.LstDominatorAccountModel.Where(account =>
                    account.AccountBaseModel.AccountProxy.ProxyIp == selectedProxy.AccountProxy.ProxyIp
                    && account.AccountBaseModel.AccountProxy.ProxyPort == selectedProxy.AccountProxy.ProxyPort).ForEach(
                    account =>
                    {
                        accountsAlreadyAssigned.Remove(accountsAlreadyAssigned.FirstOrDefault(x =>
                            x.UserName == account.UserName &&
                            x.AccountNetwork == account.AccountBaseModel.AccountNetwork));


                        account.AccountBaseModel.AccountProxy = new Proxy();

                        var socinatorAccountBuilder = new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                            .AddOrUpdateDominatorAccountBase(account.AccountBaseModel)
                            .SaveToBinFile();
                        // AccountsFileManager.Edit(account);
                    });


            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool SelectProxyCanExecute(object sender) => true;

        private void SelectProxyExecute(object sender)
        {
            if (LstProxyManagerModel.All(x => x.IsProxySelected))
                IsAllProxySelected = true;
            else
            {
                if (IsAllProxySelected)
                    _isUncheckedFromLstProxyManagerModel = true;
                IsAllProxySelected = false;
            }
        }
        private bool UpdateProxyCanExecute(object sender) => true;

        private void UpdateProxyExecute(object sender)
        {
            var currentProxy = ((FrameworkElement)sender).DataContext as ProxyManagerModel;
            var oldProxy = ProxyFileManager.GetProxyById(currentProxy.AccountProxy.ProxyId);
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
                        return;
                    }

                }
            });
            if (isAvailable)
                return;
            //if (LstProxyManagerModel.Any(x =>
            //x.AccountProxy.ProxyIp == currentProxy.AccountProxy.ProxyIp &&
            //x.AccountProxy.ProxyPort == currentProxy.AccountProxy.ProxyPort))
            //{
            //    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Warning",
            //        $"Proxy with Ip { currentProxy.AccountProxy.ProxyIp} port { currentProxy.AccountProxy.ProxyPort} is already Exist.");
            //    return;
            //}

            var accountToUpdateProxy
                = AccountsFileManager.GetAll().Where(x => x.AccountBaseModel.AccountProxy.ProxyIp == oldProxy.AccountProxy.ProxyIp
                                                          && x.AccountBaseModel.AccountProxy.ProxyPort == oldProxy.AccountProxy.ProxyPort).ToList();

            oldProxy = currentProxy;

            ProxyFileManager.EditProxy(oldProxy);

            accountToUpdateProxy.ForEach(acc =>
            {

                acc.AccountBaseModel.AccountProxy = oldProxy.AccountProxy;
                // AccountsFileManager.Edit(acc);

                var socinatorAccountBuilder = new SocinatorAccountBuilder(acc.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(acc.AccountBaseModel)
                    .SaveToBinFile();

                var accountToUpdate = AccountsFileManager.GetAccount(acc.UserName, acc.AccountBaseModel.AccountNetwork);
                UpdateAccountsProxy(accountToUpdate, _strategies);
            });
            DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Success",
                $"{oldProxy.AccountProxy.ProxyIp}:{oldProxy.AccountProxy.ProxyPort} Successfully updated.");

        }
        private bool VerifyProxyCanExecute(object sender) => true;

        private void VerifyProxyExecute(object sender)
        {
            if (sender == null)
            {
                var currentProxyManager = ((FrameworkElement)sender).DataContext as ProxyManagerModel;
                try
                {
                    ThreadFactory.Instance.Start(async () =>
                    {
                        await CheckProxyAsync(currentProxyManager);
                    });
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            else
            {
                var SelectedProxies = GetSelectedProxies();

                if (SelectedProxies.Count != 0)
                {

                    SelectedProxies.ForEach(proxy =>
                    {
                        try
                        {
                            ThreadFactory.Instance.Start(async () =>
                            {
                                await CheckProxyAsync(proxy);
                            });
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    });

                }
            }





        }
        private async Task CheckProxyAsync(ProxyManagerModel currentProxyManager)
        {

            await ProxyFileManager.UpdateProxyStatusAsync(currentProxyManager, URLToUseToVerifyProxies);

            try
            {
                var item = LstProxyManagerModel.FirstOrDefault(proxy =>
                                proxy.AccountProxy.ProxyName == currentProxyManager?.AccountProxy.ProxyName);

                var indexToUpdate = LstProxyManagerModel.IndexOf(item);

                if (currentProxyManager != null)
                {
                    LstProxyManagerModel[indexToUpdate].Status = currentProxyManager.Status;
                    GlobusLogHelper.log.Info(Log.ProxyVerificationCompleted, SocialNetworks.Social, currentProxyManager.AccountProxy.ProxyIp + ":" + currentProxyManager.AccountProxy.ProxyPort);

                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private bool RemoveAccountFromProxyCanExecute(object sender) => true;

        private void RemoveAccountFromProxyExecute(object sender)
        {
            AccountAssign account = sender as AccountAssign;

            var proxy = ProxyFileManager.GetAllProxy().FirstOrDefault(x => x.AccountProxy.ProxyIp == CurrentProxyManagerModel.AccountProxy.ProxyIp
            && x.AccountProxy.ProxyPort == CurrentProxyManagerModel.AccountProxy.ProxyPort);


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

                var accountToDeleteProxy = AccountsFileManager.GetAccount(account.UserName, account.AccountNetwork);
                accountToDeleteProxy.AccountBaseModel.AccountProxy = new Proxy();

                // AccountsFileManager.Edit(accountToDeleteProxy);

                var socinatorAccountBuilder = new SocinatorAccountBuilder(accountToDeleteProxy.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(accountToDeleteProxy.AccountBaseModel)
                    .SaveToBinFile();

                UpdateAccountsProxy(accountToDeleteProxy, _strategies);
                LstProxyManagerModel.ForEach(oldProxy =>
                 accountsAlreadyAssigned.Remove(accountsAlreadyAssigned.FirstOrDefault(x => x.UserName == accountToDelete.UserName && x.AccountNetwork == accountToDelete.AccountNetwork))
               );
                ProxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
        private bool AccountToAddToProxyCanExecute(object sender) => true;
        private void AccountToAddToProxyExecute(object sender)
        {
            AccountAssign account = sender as AccountAssign;

            try
            {
                var accountToAdd = CurrentProxyManagerModel.AccountsToBeAssign.FirstOrDefault(x => x.UserName == account.UserName);
                LstProxyManagerModel.ForEach(proxy =>
                {
                    proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x => x.UserName == accountToAdd.UserName
                                                                                               && x.AccountNetwork == accountToAdd.AccountNetwork));
                });
                if (!CurrentProxyManagerModel.AccountsAssignedto.Any(x => x.UserName == account.UserName
                                                                       && x.AccountNetwork == account.AccountNetwork))
                {
                    CurrentProxyManagerModel.AccountsAssignedto.Add(accountToAdd);
                    accountsAlreadyAssigned.Add(accountToAdd);
                }

                ProxyFileManager.EditProxy(CurrentProxyManagerModel);
                var accountToUpdateProxy = AccountsFileManager.GetAccount(account.UserName, account.AccountNetwork);
                var proxyToAdd = new Proxy()
                {
                    ProxyId = CurrentProxyManagerModel.AccountProxy.ProxyId,
                    ProxyPort = CurrentProxyManagerModel.AccountProxy.ProxyPort,
                    ProxyIp = CurrentProxyManagerModel.AccountProxy.ProxyIp,
                    ProxyUsername = CurrentProxyManagerModel.AccountProxy.ProxyUsername,
                    ProxyPassword = CurrentProxyManagerModel.AccountProxy.ProxyPassword
                };
                accountToUpdateProxy.AccountBaseModel.AccountProxy = proxyToAdd;

                // AccountsFileManager.Edit(accountToUpdateProxy);


                var socinatorAccountBuilder = new SocinatorAccountBuilder(accountToUpdateProxy.AccountBaseModel.AccountId)
                    .AddOrUpdateDominatorAccountBase(accountToUpdateProxy.AccountBaseModel)
                    .SaveToBinFile();

                var item = LstProxyManagerModel.FirstOrDefault(Proxy => Proxy.AccountProxy.ProxyName == CurrentProxyManagerModel.AccountProxy.ProxyName);
                int indexToUpdate = LstProxyManagerModel.IndexOf(item);
                LstProxyManagerModel[indexToUpdate].AccountsAssignedto = CurrentProxyManagerModel.AccountsAssignedto;
                LstProxyManagerModel[indexToUpdate].AccountsToBeAssign = CurrentProxyManagerModel.AccountsToBeAssign;

                UpdateAccountsProxy(accountToUpdateProxy, _strategies);
                ProxyFileManager.EditAllProxy(LstProxyManagerModel.ToList());

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
        private static void UpdateAccountsProxy(DominatorAccountModel accountToUpdateProxy, DominatorAccountViewModel.AccessorStrategies strategies)
        {
            var objAccountCustomControl = AccountCustomControl.GetAccountCustomControl(strategies);

            try
            {
                var updateAccountsDetails = objAccountCustomControl.DominatorAccountViewModel.LstDominatorAccountModel.FirstOrDefault(x =>
                    x.AccountBaseModel.AccountId == accountToUpdateProxy.AccountBaseModel.AccountId);

                if (updateAccountsDetails != null)
                    updateAccountsDetails.AccountBaseModel.AccountProxy =
                        accountToUpdateProxy.AccountBaseModel.AccountProxy;
            }
            catch (Exception ex)
            {

            }
        }

        private bool DropDownCanExecute(object sender) => true;

        private void DropDownExecute(object sender)
        {
            CurrentProxyManagerModel = sender as ProxyManagerModel;
            AvailableAccountsToBeAssigned(CurrentProxyManagerModel);
        }

        private void AvailableAccountsToBeAssigned(ProxyManagerModel proxyManagerModel)
        {
            try
            {
                proxyManagerModel.AccountsToBeAssign.Clear();
                AccountsFileManager.GetAll()?.ForEach(account =>
                   {

                       if (!accountsAlreadyAssigned.Any(acc =>
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


            }

        }
        private bool AssignRandomProxyCanExecute(object sender) => true;
        private void AssignRandomProxyExecute(object sender)
        {
            try
            {
                var accounts = AccountCustomControl.GetAccountCustomControl(SocialNetworks.Social).DominatorAccountViewModel
                         .LstDominatorAccountModel.Where(x => string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyIp) && string.IsNullOrEmpty(x.AccountBaseModel.AccountProxy.ProxyPort)).ToList();

                if (LstProxyManagerModel.Count != 0)
                {
                    Random random = new Random();
                    accounts.ForEach(acc =>
                      {
                          int randomIndex = random.Next(LstProxyManagerModel.Count);
                          new SocinatorAccountBuilder(acc.AccountBaseModel.AccountId)
                              .AddOrUpdateProxy(LstProxyManagerModel[randomIndex].AccountProxy)
                              .SaveToBinFile();
                          var accountAssignTo = new AccountAssign
                          {
                              UserName = acc.UserName,
                              AccountNetwork = acc.AccountBaseModel.AccountNetwork
                          };
                          CurrentProxyManagerModel = LstProxyManagerModel[randomIndex];
                          CurrentProxyManagerModel.AccountsAssignedto.Add(accountAssignTo);
                          accountsAlreadyAssigned.Add(accountAssignTo);
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
        public void UpdateProxy(DominatorAccountBaseModel objAccountBaseModel, List<ProxyManagerModel> ProxyDetail, DominatorAccountViewModel.AccessorStrategies strategy)
        {
            try
            {
                var proxyManager = ProxyManager.GetProxyManagerControl(strategy);

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
                            proxyManager.ProxyManagerViewModel.accountsAlreadyAssigned.
                                Remove(proxyManager.ProxyManagerViewModel.accountsAlreadyAssigned.FirstOrDefault(x => x.UserName == objAccountBaseModel.UserName
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
       DominatorAccountBaseModel oldAccount, DominatorAccountViewModel.AccessorStrategies strategy)
        {
            bool isDuplicatProxyAvailable = false;
            ProxyManager proxyManager = ProxyManager.GetProxyManagerControl(strategy);
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
                                proxyManager.ProxyManagerViewModel.accountsAlreadyAssigned.Add(
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
            DominatorAccountBaseModel oldAccount, DominatorAccountViewModel.AccessorStrategies strategy)
        {
            bool isProxyUpdated = false;
            ProxyManager proxyManager = ProxyManager.GetProxyManagerControl(strategy);
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

                        await ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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
                            await ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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

                            await ProxyFileManager.UpdateProxyStatusAsync(proxy, ConstantVariable.GoogleLink);
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
        public void UpdateProxyList(ProxyManagerModel proxy, ProxyManager proxyManager)
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
        public async void AddProxyIfNotExist(DominatorAccountBaseModel objAccount, DominatorAccountViewModel.AccessorStrategies strategyPack)
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
                proxyManager.ProxyManagerViewModel.accountsAlreadyAssigned.Add(
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

            await ProxyFileManager.UpdateProxyStatusAsync(ProxyManagerModel, ConstantVariable.GoogleLink);



        }
        public bool UpdateProxy(DominatorAccountBaseModel objDominatorAccountBaseModel, DominatorAccountViewModel.AccessorStrategies strategy)
        {

            var oldproxies = ProxyFileManager.GetAllProxy();

            bool isProxyUpdated = false;
            try
            {
                var oldAccount = AccountsFileManager.GetAccount(objDominatorAccountBaseModel.UserName, objDominatorAccountBaseModel.AccountNetwork).AccountBaseModel;

                isProxyUpdated = IsProxyUpdated(objDominatorAccountBaseModel, oldproxies, oldAccount,strategy).Result;
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
