using AutoMapper;
using DominatorHouse.AutoMapping;
using DominatorHouseCore;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using Prism.Unity;
using System;
using System.Windows;
using Unity;
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
            IoC.Init(containerRegistry.GetContainer());
        }

        //protected override IModuleCatalog CreateModuleCatalog()
        //{
        //    return new DirectoryModuleCatalog() { ModulePath = @".\Modules" };
        //}

        private void InitializeAutoMapper()
        {
            var moduleProfiles = IoC.Container.ResolveAll<Profile>();
            AutoMapperConfiguration.Init(moduleProfiles);
        }
        protected override IModuleCatalog CreateModuleCatalog()
        {
            return new ConfigurationModuleCatalog();
        }

        protected override void ConfigureRegionAdapterMappings(RegionAdapterMappings regionAdapterMappings)
        {
            base.ConfigureRegionAdapterMappings(regionAdapterMappings);
            //regionAdapterMappings.RegisterMapping(typeof(StackPanel), Container.Resolve<StackPanelRegionAdapter>());
        }
    }
}
