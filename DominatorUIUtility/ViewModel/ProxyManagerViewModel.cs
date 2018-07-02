using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
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
            BindingOperations.EnableCollectionSynchronization(lstProxyManagerModel, _lock);

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



        #endregion

        private static object _lock = new object();

        public void StartAddingItems()
        {
            Task.Factory.StartNew(() =>
            {

                try
                {
                    ProxyFileManager.GetAllProxy().ForEach(proxy =>
                    {
                        lstProxyManagerModel.Add(proxy);
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
            Task.Factory.StartNew(() =>
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
            AddOrUpdateProxyControl objAddProxyControl = new AddOrUpdateProxyControl();
            ProxyManagerModel = new ProxyManagerModel();
            try
            {
                objAddProxyControl.btnSave.Content = Application.Current.FindResource("LangKeySave").ToString();
                objAddProxyControl.MainGrid.DataContext = ProxyManagerModel;
                // ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {ProxyManagerModel.AccountProxy.ProxyIp.Replace(".", "")}";
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(objAddProxyControl, Application.Current.FindResource("LangKeyAddProxy").ToString());
                DialogParticipation.SetRegister(window, window);
                objAddProxyControl.btnSave.Click += (o, ex) =>
                {
                    try
                    {
                        var proxyById = ProxyFileManager.GetProxyById(ProxyManagerModel.AccountProxy.ProxyId);
                        if (proxyById != null && !string.IsNullOrEmpty(proxyById.AccountProxy.ProxyId))
                        {
                            DialogCoordinator.Instance.ShowModalMessageExternal(window, "Proxy Warning",
                                $"Proxy with name {ProxyManagerModel.AccountProxy.ProxyName} already exist.");
                            return;
                        }
                        foreach (var proxy in LstProxyManagerModel)
                        {
                            if (ProxyManagerModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                              && ProxyManagerModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                            {
                                DialogCoordinator.Instance.ShowModalMessageExternal(window, "Proxy Warning", "Proxy already exist !!!");
                                return;
                            }

                        }

                        //  UpdateAccountsToBeAssign(ProxyManagerModel);


                        if (IsAllProxySelected)
                            ProxyManagerModel.IsProxySelected = true;
                        ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {ProxyManagerModel.AccountProxy.ProxyIp.Replace(".", "")}{ProxyManagerModel.AccountProxy.ProxyPort}";
                        ProxyFileManager.SaveProxy(ProxyManagerModel);
                        LstProxyManagerModel.Add(ProxyManagerModel);

                        GlobusLogHelper.log.Info(Log.Added, SocialNetworks.Social, ProxyManagerModel.AccountProxy.ProxyIp + " : " +
                                                                                   ProxyManagerModel.AccountProxy.ProxyPort);
                        window.Close();
                    }
                    catch (Exception e)
                    {
                        e.DebugLog();
                    }

                };
                window.Show();
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

            
            Task.Factory.StartNew(() =>
            {
                foreach (var givenProxy in loadedProxylist)
                {
                    try
                    {
                        var proxy = givenProxy.Replace(",", ":");

                        var selectedProxy = Regex.Split(proxy, ":");
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
                            GlobusLogHelper.log.Info(Log.Exported, SocialNetworks.Social, proxy.AccountProxy.ProxyIp + " : " + proxy.AccountProxy.ProxyPort);
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
                if ((bool)sender)
                    ProxyManagerCollection.GroupDescriptions.Add(new PropertyGroupDescription("AccountProxy.ProxyGroup"));
                else
                    ProxyManagerCollection.GroupDescriptions.Clear();

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
                foreach (var proxy in ProxyFileManager.GetAllProxy())
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
            return true;
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
                                    $"{SelectedProxies.Count} proxies successfully Deleted.");
                                GlobusLogHelper.log.Info(Log.Deleted, SocialNetworks.Social, $"{SelectedProxies.Count} proxies");
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
                       $"{currentProxy.AccountProxy.ProxyIp}:{currentProxy.AccountProxy.ProxyPort} Successfully Deleted."));
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
                    Task.Factory.StartNew(async () =>
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
                            Task.Factory.StartNew(async () =>
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
                    LstProxyManagerModel[indexToUpdate].Status = currentProxyManager.Status;
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


    }

}
