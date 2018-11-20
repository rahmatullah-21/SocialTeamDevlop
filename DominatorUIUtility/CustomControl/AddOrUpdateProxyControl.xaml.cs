using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddOrUpdateProxyControl.xaml
    /// </summary>
    public partial class AddOrUpdateProxyControl : UserControl
    {
        public ProxyManagerModel ProxyManagerModel { get; set; }
        private ProxyManagerViewModel ProxyManagerViewModel { get; set; }
        public AddOrUpdateProxyControl(ProxyManagerViewModel ProxyManagerViewModel)
        {
            InitializeComponent();
            this.ProxyManagerViewModel = ProxyManagerViewModel;
            ProxyManagerModel = new ProxyManagerModel();
            ProxyManagerModel.AccountProxy.ProxyName = $"Proxy - {DateTimeUtilities.GetEpochTime()}";

            MainGrid.DataContext = ProxyManagerModel;
        }
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSave.IsDefault = true;
            }
        }

        private void BtnCancel_OnClick(object sender, RoutedEventArgs e)
        {
            Dialog.CloseDialog(sender);
        }

        private void BtnSave_OnClick(object sender, RoutedEventArgs e)
        {

            try
            {
                var proxyById = ProxyFileManager.GetProxyById(ProxyManagerModel.AccountProxy.ProxyId);
                if (proxyById != null && !string.IsNullOrEmpty(proxyById.AccountProxy.ProxyId))
                {
                    DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Proxy Warning",
                        $"Proxy with name {ProxyManagerModel.AccountProxy.ProxyName} already exist.");
                    return;
                }
                foreach (var proxy in ProxyManagerViewModel.LstProxyManagerModel)
                {
                    if (ProxyManagerModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                      && ProxyManagerModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                    {
                        DialogCoordinator.Instance.ShowModalMessageExternal(Application.Current.MainWindow, "Proxy Warning", "Proxy already exist !!!");
                        return;
                    }

                }
                
                if (ProxyManagerViewModel.IsAllProxySelected)
                    ProxyManagerModel.IsProxySelected = true;
                if (string.IsNullOrEmpty(ProxyManagerModel.AccountProxy.ProxyName))
                    ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {ProxyManagerModel.AccountProxy.ProxyIp.Replace(".", "")}{ProxyManagerModel.AccountProxy.ProxyPort}";
                ProxyFileManager.SaveProxy(ProxyManagerModel);
                ProxyManagerViewModel.LstProxyManagerModel.Add(ProxyManagerModel);
                ProxyManagerModel.Index = ProxyManagerViewModel.LstProxyManagerModel.IndexOf(ProxyManagerModel) + 1;
                GlobusLogHelper.log.Info(Log.Added, SocialNetworks.Social, ProxyManagerModel.AccountProxy.ProxyIp + " : " +
                                                                           ProxyManagerModel.AccountProxy.ProxyPort, "LangKeyProxy".FromResourceDictionary());
                Dialog.CloseDialog(sender);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }
    }
}
