using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DominatorHouseCore;
using DominatorHouseCore.Command;
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
        }

     
        #region Commands

        public ICommand AddProxyCommand { get; set; }
        public ICommand ImportProxyCommand { get; set; }
        public ICommand ShowByGroupCommand { get; set; }
        public ICommand ExportProxyCommand { get; set; }
        public ICommand HideUnhideSocialProfilesCommand { get; set; }
        public ICommand HideUnhideUserPasswordCommand { get; set; }
        public ICommand ShowProxiesWithErrorCommand { get; set; }
        public ICommand ShowUnassignedProxiesCommand { get; set; }
        #endregion

        #region Properies
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
        #endregion
        private void AddProxyExecute(object sender)
        {
            AddOrUpdateProxyControl objAddProxyControl = new AddOrUpdateProxyControl();
            ProxyManagerModel = new ProxyManagerModel();
            LstProxyManagerModel = new ObservableCollection<ProxyManagerModel>(ProxyFileManager.GetAllProxy());
            try
            {
                objAddProxyControl.btnSave.Content = Application.Current.FindResource("langSave").ToString();
                objAddProxyControl.MainGrid.DataContext = ProxyManagerModel;
                ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {LstProxyManagerModel.Count + 1}";
                Dialog dialog = new Dialog();
                Window window = dialog.GetMetroWindow(objAddProxyControl, Application.Current.FindResource("langAddProxy").ToString());
                DialogParticipation.SetRegister(window, window);
                objAddProxyControl.btnSave.Click += (o, ex) =>
                {
                    try
                    {
                        var AvailableProxy = ProxyFileManager.GetProxyById(ProxyManagerModel.AccountProxy.ProxyId);
                        if (AvailableProxy != null && !string.IsNullOrEmpty(AvailableProxy.AccountProxy.ProxyId))
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

                        UpdateAccountsToBeAssign(ProxyManagerModel);

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
            foreach (var proxy in loadedProxylist)
            {
                try
                {
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

                    if (!Proxy.IsValidProxy(ProxyManagerModel.AccountProxy.ProxyIp, ProxyManagerModel.AccountProxy.ProxyPort))
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
        }

        private bool ImportProxyCanExecute(object sender) => true;
        private bool ExportProxyCanExecute(object sender) => true;

        private void ExportProxyExecute(object sender)
        {
            var allProxies = ProxyFileManager.GetAllProxy();
            ExportProxies(allProxies);
        }
        private void ExportProxies(List<ProxyManagerModel> Proxies)
        {
            var exportPath = FileUtilities.GetExportPath();

            if (string.IsNullOrEmpty(exportPath))
                return;

            string header = "Proxy Group,Proxy name,Proxy IP,Proxy Port,Proxy Username,Proxy Password,Status";

            var filename = $"{exportPath}\\Proxies {ConstantVariable.DateasFileName}.csv";

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
                catch (Exception)
                {
                    GlobusLogHelper.log.Error("Error in Export Proxies");
                }
            });


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
                                               Application.Current.FindResource("langSocialProfiles").ToString());
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
                                                                                    Application.Current.FindResource("langProxyUsername").ToString());
                var proxyPasswordVisiblity = dataGrid.Columns.Single(gridColumn => gridColumn.Header.ToString() ==
                                                                                    Application.Current.FindResource("langProxyPassword").ToString());
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
            List<ProxyManagerModel> unassignedProxies = new List<ProxyManagerModel>();

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
                ProxyManagerModel.ProxyManagerCollection = CollectionViewSource.GetDefaultView(unassignedProxies); 
            }
            else
            {
                ProxyManagerModel.ProxyManagerCollection = CollectionViewSource.GetDefaultView(LstProxyManagerModel);
            }
        }





    }

}
