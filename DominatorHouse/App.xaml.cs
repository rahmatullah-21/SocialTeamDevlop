using AutoMapper;
using CommonServiceLocator;
using DominatorHouse.AutoMapping;
using DominatorHouseCore;
using DominatorUIUtility.Behaviours;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Threading;
using System.Windows;
using Microsoft.Practices.Unity.Configuration;
using Unity;
using Unity.Interception;
using MessageBox = System.Windows.MessageBox;
using DominatorUIUtility.Module;
using DominatorUIUtility.ViewModel.Startup;
using DominatorHouse.Utilities.Facebook;
using DominatorHouseCore.Utility;

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
                MessageBox.Show("LangKeySocinatorAlreadyRunning".FromResourceDictionary(), "LangKeyWarning".FromResourceDictionary(), MessageBoxButton.OK, MessageBoxImage.Warning);
                Environment.Exit(0);
            }
            var container = containerRegistry.GetContainer();
            container.AddNewExtension<Interception>();
            container.AddNewExtension<CoreUnityExtension>();
            container.LoadConfiguration();
            StartupBaseViewModel.GetFaceBookActivity = (activityType) => new FacebookActivity().GetActivity(activityType);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            base.ConfigureModuleCatalog(moduleCatalog);
            moduleCatalog.AddModule<UiModule>();
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
            return CheckByProcess();
            //try   // commented this code temporarily as it was not working properly
            //{
            //    Mutex.OpenExisting("Socinator");
            //}
            //catch
            //{
            //    _mutex = new Mutex(true, "Socinator");
            //    return false;
            //}
            //return true;
        }

        bool CheckByProcess()
        {
            try
            {
                var existed = false;
                var itemCount = 0;

                foreach (var item in System.Diagnostics.Process.GetProcesses())
                {
                    try
                    {
                        if (item.ProcessName != "Socinator")
                            continue;
                        itemCount++;
                        if (itemCount <= 1) continue;
                        existed = true;
                        break;
                    }
                    catch
                    { /* ignored*/ }
                }
                
                return existed;
            }
            catch
            { return false; }
        }
    }
}
