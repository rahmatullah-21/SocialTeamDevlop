using CommonServiceLocator;
using DominatorUIUtility.ViewModel.Startup;
using MahApps.Metro.Controls;
using Prism.Regions;
using System;
using System.Windows;

namespace DominatorUIUtility
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ModuleSetting : MetroWindow
    {
        IRegionManager _regionManager;
        public ModuleSetting(IRegionManager regionManager)
        {
            InitializeComponent();
            _regionManager = regionManager;
            this.SetValue(RegionManager.RegionManagerProperty, regionManager);

        }
        private static ModuleSetting _instance;
        public static ModuleSetting Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ModuleSetting(ServiceLocator.Current.GetInstance<IRegionManager>());
                var viewModel = ServiceLocator.Current.GetInstance<ISelectActivityViewModel>();
                _instance.Title = $"Socinator - {{ {viewModel.SelectedNetwork} }} ( {  viewModel.SelectAccount.UserName} )";
                return _instance;
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Instance.Visibility = Visibility.Collapsed;
            StartupBaseViewModel.selectedIndex = 0;
            StartupBaseViewModel.ViewModelToSave.Clear();
            _regionManager.Regions["StartupRegion"].RemoveAll();
            _regionManager.RequestNavigate("StartupRegion", "SelectActivity");
        }
    }
}
