using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.ProxyServerManagment;
using DominatorHouseCore.Utility;
using LegionUIUtility.ViewModel;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LegionUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for AddOrUpdateProxyControl.xaml
    /// </summary>
    public partial class AddOrUpdateProxyControl : UserControl
    {
        private readonly IProxyFileManager _proxyFileManager;
        public ProxyManagerModel ProxyManagerModel { get; set; }
        private ProxyManagerViewModel ProxyManagerViewModel { get; set; }
        public AddOrUpdateProxyControl(ProxyManagerViewModel ProxyManagerViewModel)
        {
            _proxyFileManager = ServiceLocator.Current.GetInstance<IProxyFileManager>();
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
                //IProxyValidationService _proxyValidationService = ServiceLocator.Current.GetInstance<IProxyValidationService>();
                //if (!_proxyValidationService.IsValidProxy(ProxyManagerModel.AccountProxy.ProxyIp, ProxyManagerModel.AccountProxy.ProxyPort))
                //{
                //    Dialog.ShowDialog("Proxy Warning", $"Invalid Proxy IP format :- \"{ProxyManagerModel.AccountProxy.ProxyIp}\". ");
                //    return;
                //}

                foreach (var proxy in ProxyManagerViewModel.LstProxyManagerModel)
                {
                    if (ProxyManagerModel.AccountProxy.ProxyName.Equals(proxy.AccountProxy.ProxyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        Dialog.ShowDialog("Proxy Warning", $"Proxy with name {ProxyManagerModel.AccountProxy.ProxyName} already exist.");
                        return;
                    }
                    if (ProxyManagerModel.AccountProxy.ProxyIp == proxy.AccountProxy.ProxyIp
                      && ProxyManagerModel.AccountProxy.ProxyPort == proxy.AccountProxy.ProxyPort)
                    {
                        Dialog.ShowDialog("Proxy Warning", "Proxy already exist !!!");
                        return;
                    }
                }

                if (ProxyManagerViewModel.IsAllProxySelected)
                    ProxyManagerModel.IsProxySelected = true;
                if (string.IsNullOrEmpty(ProxyManagerModel.AccountProxy.ProxyName))
                    ProxyManagerModel.AccountProxy.ProxyName = $"Proxy {ProxyManagerModel.AccountProxy.ProxyIp.Replace(".", "")}{ProxyManagerModel.AccountProxy.ProxyPort}";
                _proxyFileManager.SaveProxy(ProxyManagerModel);
                ProxyManagerViewModel.LstProxyManagerModel.Add(ProxyManagerModel);
                ProxyManagerViewModel.AddGroup(ProxyManagerModel);
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
