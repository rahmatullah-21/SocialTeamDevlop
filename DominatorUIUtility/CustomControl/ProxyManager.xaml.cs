using System;
using System.Collections.Generic;
using System.Collections.Immutable;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using DominatorHouseCore;
using DominatorHouseCore.Annotations;
using DominatorHouseCore.Request;
using DominatorUIUtility.ViewModel;
using DominatorHouseCore.Enums;

namespace DominatorUIUtility.CustomControl
{
    /// <summary>
    /// Interaction logic for ProxyManager.xaml
    /// </summary>
    public partial class ProxyManager : UserControl, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #region Properties

        private ProxyManagerViewModel _proxyManagerViewModel = new ProxyManagerViewModel();

        public ProxyManagerViewModel ProxyManagerViewModel
        {
            get
            {
                return _proxyManagerViewModel;
            }
            set
            {
                _proxyManagerViewModel = value;
                OnPropertyChanged(nameof(ProxyManagerViewModel));
            }
        }

        #endregion
        public ProxyManager(DominatorAccountViewModel.AccessorStrategies strategies)
        {

            InitializeComponent();
            ProxyManagerViewModel._strategies = strategies;
            MainGrid.DataContext = ProxyManagerViewModel;
            ProxyManagerViewModel.ProxyManagerCollection =
                CollectionViewSource.GetDefaultView(ProxyManagerViewModel.LstProxyManagerModel);
            ProxyManagerViewModel.StartAddingItems();



        }

        private static ProxyManager _proxyManagerInstance = null;
        public static ProxyManager GetProxyManagerControl(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            try
            {
                return _proxyManagerInstance ?? (_proxyManagerInstance = new ProxyManager(strategies));
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return _proxyManagerInstance;
            }
        }

    }
}
