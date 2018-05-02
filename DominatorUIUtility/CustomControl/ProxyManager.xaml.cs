using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DominatorUIUtility.Behaviours;
using DominatorHouseCore.Models;
using DominatorHouseCore.FileManagers;
using MahApps.Metro.Controls.Dialogs;
using DominatorHouseCore.Utility;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using DominatorHouseCore.LogHelper;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using DominatorHouseCore.Request;
using DominatorUIUtility.ViewModel;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager : UserControl
    {
        ProxyManagerModel currentProxyManagerModel;
        private DominatorAccountViewModel.AccessorStrategies _strategies;

        ProxyManagerModel ProxyManagerModel { get; set; }

        ObservableCollection<ProxyManagerModel> ProxyDetail { get; set; } = new ObservableCollection<ProxyManagerModel>();

        public ProxyManager(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            _strategies = strategies;
            InitializeComponent();

        }

        private void SetDataContext()
        {
            ProxyManagerModel = new ProxyManagerModel();

            MainGrid.DataContext = ProxyManagerModel;

            ProxyDetail = new ObservableCollection<ProxyManagerModel>(ProxyFileManager.GetAllProxy());

            ProxyDetail.ForEach(proxy =>
            {
                if (ProxyManagerModel.Groups.Any(ProxyGroup => ProxyGroup == proxy.AccountProxy.ProxyGroup) == false)
                    ProxyManagerModel.Groups.Add(proxy.AccountProxy.ProxyGroup);

            }
            );

            ProxyManagerModel.ProxyManagerCollection = CollectionViewSource.GetDefaultView(ProxyDetail);
        }

        private void btnAddProxy_Click(object sender, RoutedEventArgs e)
        {
            AddOrUpdateProxyControl ObjAddProxyControl = new AddOrUpdateProxyControl();
            ObjAddProxyControl.btnSave.Content = FindResource("langSave").ToString();
            ObjAddProxyControl.MainGrid.DataContext = ProxyManagerModel;

            int ProxyID = ProxyDetail.Count > 0 ? ProxyDetail.Max(Proxy => Proxy.Id) + 1 : 1;
            ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {ProxyID}";
            Dialog dialog = new Dialog();
            Window window = dialog.GetMetroWindow(ObjAddProxyControl, FindResource("langAddProxy").ToString());
            DialogParticipation.SetRegister(window, window);
            ObjAddProxyControl.btnSave.Click += (o, ex) =>
            {
                var AvailableProxy = ProxyFileManager.GetProxyByName(ProxyManagerModel.AccountProxy.ProxyName);
                if (AvailableProxy != null && !string.IsNullOrEmpty(AvailableProxy.AccountProxy.ProxyName))
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(window, "Proxy Warning", $"Proxy with name {ProxyManagerModel.AccountProxy.ProxyName} already exist .");
                    return;
                }
                foreach (var proxy in ProxyDetail)
                {
                    if (ProxyManagerModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                      && ProxyManagerModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(window, "Proxy Warning", "Proxy already exist !!!");
                        return;
                    }

                }
                ProxyManagerModel.Id = ProxyID;
                ProxyFileManager.SaveProxy(ProxyManagerModel);
                ProxyDetail.Add(ProxyManagerModel);

                window.Close();

            };
            window.Show();

        }

        private void btnRemoveAccountFromProxy_Click(object sender, RoutedEventArgs e)
        {
            AccountAssign account = ((FrameworkElement)sender).DataContext as AccountAssign;
            var proxy = ProxyFileManager.GetProxyByName(currentProxyManagerModel.AccountProxy.ProxyName);
            var accountToDelete = proxy.AccountsAssignedto.FirstOrDefault(x => x.UserName == account.UserName);
            try
            {
                proxy.AccountsAssignedto.Remove(accountToDelete);

                ProxyFileManager.EditProxy(proxy);

                var item = ProxyDetail.FirstOrDefault(Proxy => Proxy.AccountProxy.ProxyName == proxy.AccountProxy.ProxyName);
                int indexToUpdate = ProxyDetail.IndexOf(item);
                ProxyDetail[indexToUpdate].AccountsAssignedto = proxy.AccountsAssignedto;
                ProxyDetail[indexToUpdate].AccountsToBeAssign.Add(accountToDelete);

                var AccountToDeleteProxy = AccountsFileManager.GetAccount(account.UserName);
                AccountToDeleteProxy.AccountBaseModel.AccountProxy = new Proxy();

                AccountsFileManager.Edit(AccountToDeleteProxy);
                UpdateAccountsProxy(AccountToDeleteProxy, _strategies);
                ProxyDetail.ForEach(oldProxy =>
                {
                    if (!oldProxy.AccountsToBeAssign.Any(x => x.UserName == accountToDelete.UserName && x.AccountNetwork == accountToDelete.AccountNetwork))
                        oldProxy.AccountsToBeAssign.Add(accountToDelete);
                    ProxyFileManager.EditProxy(oldProxy);
                });
                var v = ProxyFileManager.GetAllProxy();
            }
            catch (Exception ex)
            {

            }

        }

        private void btnDeletesProxy_Click(object sender, RoutedEventArgs e)
        {
            var currentProxy = ((FrameworkElement)sender).DataContext as ProxyManagerModel;

            ProxyFileManager.Delete(proxy => proxy.AccountProxy.ProxyName == currentProxy.AccountProxy.ProxyName);

            ProxyDetail.Remove(currentProxy);

        }

        private void dropdown_Click(object sender, RoutedEventArgs e)
        {
            currentProxyManagerModel = ((FrameworkElement)sender).DataContext as ProxyManagerModel;
        }

        private void chkShowUnassignedProxies_Checked(object sender, RoutedEventArgs e)
        {
            List<ProxyManagerModel> unassignedProxies = new List<ProxyManagerModel>();

            foreach (var proxy in ProxyDetail)
            {
                if (proxy.AccountsAssignedto.Count == 0)
                {
                    proxy.AccountsToBeAssign = proxy.AccountsToBeAssign;
                    unassignedProxies.Add(proxy);
                }

            }
            ProxyManagerModel.ProxyManagerCollection = CollectionViewSource.GetDefaultView(unassignedProxies);
        }

        private void chkShowUnassignedProxies_Unchecked(object sender, RoutedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection = CollectionViewSource.GetDefaultView(ProxyDetail);
        }

        private void ShowByGroup_Checked(object sender, RoutedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection.GroupDescriptions.Add(new PropertyGroupDescription("AccountProxy.ProxyGroup"));
        }

        private void ShowByGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ProxyManagerModel.ProxyManagerCollection.GroupDescriptions.Clear();
            }
            catch (Exception)
            {
            }
        }

        private void FilterByGroup_Unchecked(object sender, RoutedEventArgs e)
        {
            Group.SelectedIndex = -1;
        }

        private void Group_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection.Filter += FilterByGroups;
        }

        private void txtfilter_TextChanged(object sender, TextChangedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection.Filter += FilterByNameOrIP;
        }

        private void AllProxyChecked_Checked(object sender, RoutedEventArgs e)
        {
            try
            {
                ProxyDetail.Select(proxy => { proxy.IsProxySelected = true; return proxy; }).ToList();
            }
            catch (Exception)
            {
            }
        }

        private void AllProxyChecked_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                ProxyDetail.Select(proxy => { proxy.IsProxySelected = false; return proxy; }).ToList();
            }
            catch (Exception)
            {

            }
        }

        private void btnDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ProxyManagerModel> SelectedProxies = ProxyDetail.Where(proxy => proxy.IsProxySelected == true).ToList();
            SelectedProxies.ForEach(selectedProxy =>
            {
                ProxyFileManager.Delete(proxy => proxy.AccountProxy.ProxyName == selectedProxy.AccountProxy.ProxyName);
                ProxyDetail.Remove(selectedProxy);
            });
        }

        private void btnExportSelected_Click(object sender, RoutedEventArgs e)
        {
            List<ProxyManagerModel> SelectedProxies = ProxyDetail.Where(proxy => proxy.IsProxySelected == true).ToList();
            if (SelectedProxies.Count == 0)
                return;
            ExportProxies(SelectedProxies);

        }

        private void btnImportProxies_Click(object sender, RoutedEventArgs e)
        {
            var loadedProxylist = FileUtilities.FileBrowseAndReader();
            if (loadedProxylist == null)
                return;

            int proxyId = ProxyFileManager.GetAllProxy().Count + 1;

            foreach (var proxy in loadedProxylist)
            {
                var selectedProxy = Regex.Split(proxy, ":");
                if (selectedProxy.Length < 4)
                    continue;

                var alreadyExistProxy = ProxyFileManager.GetProxyByName(selectedProxy[1]);

                if (alreadyExistProxy != null && alreadyExistProxy.AccountProxy.ProxyIp == selectedProxy[2]
                    && alreadyExistProxy.AccountProxy.ProxyPort == selectedProxy[3])
                    continue;

                ProxyManagerModel = new ProxyManagerModel();
                try
                {
                    ProxyManagerModel.Id = proxyId;
                    ProxyManagerModel.AccountProxy.ProxyGroup = selectedProxy[0];
                    ProxyManagerModel.AccountProxy.ProxyName = selectedProxy[1];
                    ProxyManagerModel.AccountProxy.ProxyIp = selectedProxy[2];
                    ProxyManagerModel.AccountProxy.ProxyPort = selectedProxy[3];

                    if (selectedProxy.Length == 6)
                    {
                        ProxyManagerModel.AccountProxy.ProxyUsername = selectedProxy[4];
                        ProxyManagerModel.AccountProxy.ProxyPassword = selectedProxy[5];
                    }

                    ProxyFileManager.SaveProxy(ProxyManagerModel);

                    AccountsFileManager.GetAll().ForEach(user =>
                    {
                        ProxyManagerModel.AccountsToBeAssign.Add(new AccountAssign
                        {
                            UserName = user.UserName,
                            AccountNetwork = user.AccountBaseModel.AccountNetwork
                        });
                    });
                    ProxyDetail.Add(ProxyManagerModel);
                    proxyId++;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.StackTrace);
                }
            }
        }

        private void Account_Checked(object sender, RoutedEventArgs e)
        {
            AccountAssign account = ((FrameworkElement)sender).DataContext as AccountAssign;

            try
            {
                var accountToAdd = currentProxyManagerModel.AccountsToBeAssign.FirstOrDefault(x => x.UserName == account.UserName);
                ProxyDetail.ForEach(proxy =>
                {
                    proxy.AccountsToBeAssign.Remove(proxy.AccountsToBeAssign.FirstOrDefault(x => x.UserName == accountToAdd.UserName
                                                                                               && x.AccountNetwork == accountToAdd.AccountNetwork));
                });

                currentProxyManagerModel.AccountsAssignedto.Add(accountToAdd);
                ProxyFileManager.EditProxy(currentProxyManagerModel);
                var AccountToUpdateProxy = AccountsFileManager.GetAccount(account.UserName);
                AccountToUpdateProxy.AccountBaseModel.AccountProxy = currentProxyManagerModel.AccountProxy;
                AccountsFileManager.Edit(AccountToUpdateProxy);

                var item = ProxyDetail.FirstOrDefault(Proxy => Proxy.AccountProxy.ProxyName == currentProxyManagerModel.AccountProxy.ProxyName);
                int indexToUpdate = ProxyDetail.IndexOf(item);
                ProxyDetail[indexToUpdate].AccountsAssignedto = currentProxyManagerModel.AccountsAssignedto;
                ProxyDetail[indexToUpdate].AccountsToBeAssign = currentProxyManagerModel.AccountsToBeAssign;

                UpdateAccountsProxy(AccountToUpdateProxy, _strategies);
            }
            catch (Exception ex)
            {
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


        private void btnVerifyProxy_Click(object sender, RoutedEventArgs e)
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
            }
        }

        private async Task CheckProxyAsync(ProxyManagerModel currentProxyManager)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(new Uri(ProxyManagerModel.URLToUseToVerifyProxies));

                if (currentProxyManager != null)
                {
                    var webProxy = new WebProxy(currentProxyManager.AccountProxy.ProxyIp, int.Parse(currentProxyManager.AccountProxy.ProxyPort))
                    {
                        BypassProxyOnLocal = true
                    };
                    if (!string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyUsername)
                        && !string.IsNullOrEmpty(currentProxyManager.AccountProxy.ProxyPassword))
                    {
                        webProxy.Credentials = new NetworkCredential(currentProxyManager.AccountProxy.ProxyUsername, currentProxyManager.AccountProxy.ProxyPassword);
                    }

                    request.Proxy = webProxy;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    using (var response = (HttpWebResponse)await request.GetResponseAsync())
                    {
                        currentProxyManager.Status = response.StatusCode.ToString() == "OK" ? "Working" : "Not Working";
                    }

                    stopWatch.Stop();
                    var ts = stopWatch.Elapsed;
                    currentProxyManager.ResponseTime = $"{ts.Milliseconds} milli seconds";
                }
            }
            catch (Exception)
            {
                if (currentProxyManager != null)
                {
                    currentProxyManager.Status = "Fail";
                    currentProxyManager.Failures = 1;
                }
            }

            ProxyFileManager.EditProxy(currentProxyManager);

            var item = ProxyDetail.FirstOrDefault(proxy =>
                        proxy.AccountProxy.ProxyName == currentProxyManager?.AccountProxy.ProxyName);

            var indexToUpdate = ProxyDetail.IndexOf(item);

            if (currentProxyManager != null)
                ProxyDetail[indexToUpdate].Status = currentProxyManager.Status;
        }

        private void VerifyProxy(object sender)
        {
            var currentProxyManager = ((FrameworkElement)sender).DataContext as ProxyManagerModel;
            currentProxyManager.URLToUseToVerifyProxies = ProxyManagerModel.URLToUseToVerifyProxies;
            try
            {



                Uri url = new Uri(txtURLToUseToVerifyProxies.Text);
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

                request.Proxy = new WebProxy(currentProxyManager.AccountProxy.ProxyIp, int.Parse(currentProxyManager.AccountProxy.ProxyPort));

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                if (response.StatusCode.ToString() == "OK")
                    currentProxyManager.Status = "Working";
                else
                    currentProxyManager.Status = "Not Working";

            }
            catch (Exception)
            {
                currentProxyManager.Status = "Fail";
                currentProxyManager.Failures = 1;
            }
            ProxyFileManager.EditProxy(currentProxyManager);

            var item = ProxyDetail.FirstOrDefault(Proxy => Proxy.AccountProxy.ProxyName == currentProxyManager.AccountProxy.ProxyName);
            int indexToUpdate = ProxyDetail.IndexOf(item);
            ProxyDetail[indexToUpdate].Status = currentProxyManager.Status;
        }

        private void ShowProxiesWithError_Checked(object sender, RoutedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection.Filter += FilterByProxiesWithError;
        }

        private void ShowProxiesWithError_Unchecked(object sender, RoutedEventArgs e)
        {
            ProxyManagerModel.ProxyManagerCollection.Filter = (object ob) => { return true; };
        }

        private void btnExportProxies_Click(object sender, RoutedEventArgs e)
        {
            var allProxies = ProxyFileManager.GetAllProxy();
            ExportProxies(allProxies);
        }

        private bool FilterByGroups(object GroupName)
        {
            try
            {
                ProxyManagerModel ProxyGroup = GroupName as ProxyManagerModel;
                return ProxyGroup.AccountProxy.ProxyGroup.IndexOf(Group.SelectedValue.ToString(), StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception)
            {
            }
            return true;
        }

        private bool FilterByNameOrIP(object NameOrIP)
        {
            try
            {
                ProxyManagerModel ProxyGroup = NameOrIP as ProxyManagerModel;
                return ProxyGroup.AccountProxy.ProxyIp.IndexOf(txtfilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0
                    || ProxyGroup.AccountProxy.ProxyName.IndexOf(txtfilter.Text, StringComparison.InvariantCultureIgnoreCase) >= 0;
            }
            catch (Exception)
            {
            }
            return true;
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
                    }

                }
                catch (Exception)
                {

                    GlobusLogHelper.log.Error("Error in Export Proxies");
                }
            });
        }

        private void ProxyManager_OnLoaded(object sender, RoutedEventArgs e)
        {
            SetDataContext();
        }
    }
}
