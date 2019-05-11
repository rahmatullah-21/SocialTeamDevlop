using AutoMapper;
using CommonServiceLocator;
using DominatorHouse.AutoMapping;
using DominatorHouseCore;
using DominatorUIUtility.Behaviours;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Threading;
using System.Windows;
using Unity;
using Unity.Interception.ContainerIntegration;
using MessageBox = System.Windows.MessageBox;

namespace Socinator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    //base.OnStartup(e);
        //    var boostrapper = new Bootstrapper();
        //    boostrapper.Run();
        //}
        public void CheckAllforExpand(object sender, RoutedEventArgs e)
        {
            HeaderHelper.UpdateToggleButtonInCampaignMode?.Invoke();
            HeaderHelper.UpdateToggleButtonInAccountActivityMode?.Invoke();
        }
        protected override Window CreateShell()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeAutoMapper();
            var shell = Container.Resolve<MainWindow>();

            return shell;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ex.DebugLog();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (IsAlreadyRunning())
            {
                MessageBox.Show("Asteroid already running.", "Warnning", MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }
            var container = containerRegistry.GetContainer();
            container.AddNewExtension<Interception>();
            container.AddNewExtension<CoreUnityExtension>();
            container.LoadConfiguration();
        }

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        //}

        private void InitializeAutoMapper()
        {
            var moduleProfiles = ServiceLocator.Current.GetAllInstances<Profile>();
            AutoMapperConfiguration.Init(moduleProfiles);
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        private Mutex _mutex;
        bool IsAlreadyRunning()
        {
            try
            {
                Mutex.OpenExisting("Asteroid");
            }
            catch
            {
                _mutex = new Mutex(true, "Asteroid");
                return false;
            }
            return true;
        }
    }
}
