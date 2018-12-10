using AutoMapper;
using CommonServiceLocator;
using DominatorHouse.AutoMapping;
using DominatorHouseCore;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using DominatorHouseCore.Utility;
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

        protected override Window CreateShell()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            InitializeAutoMapper();
            var shell = Container.Resolve<MainWindow>();

            return shell;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            if (IsAlreadyRunning())
            {
                MessageBox.Show("Socinator already running.", "Warnning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
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
                Mutex.OpenExisting("Socinator");
            }
            catch
            {
                _mutex = new Mutex(true, "Socinator");
                return false;
            }
            return true;
        }
    }
}
