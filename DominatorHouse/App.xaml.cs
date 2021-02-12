using AutoMapper;
using CommonServiceLocator;
using DominatorHouse.AutoMapping;
using DominatorHouse.Utilities.Facebook;
using DominatorHouseCore;
using DominatorHouseCore.Utility;
using DominatorUIUtility.Behaviours;
using DominatorUIUtility.Module;
using DominatorUIUtility.ViewModel.Startup;
using Microsoft.Practices.Unity.Configuration;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Unity;
using Unity.Interception;
using MessageBox = System.Windows.MessageBox;

// ReSharper disable once CheckNamespace
// ReSharper disable once IdentifierTypo
namespace Socinator
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public void CheckAllForExpand(object sender, RoutedEventArgs e)
        {
            HeaderHelper.UpdateToggleButtonInCampaignMode?.Invoke();
            HeaderHelper.UpdateToggleButtonInAccountActivityMode?.Invoke();
        }
        protected override Window CreateShell()
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.AssemblyResolve += Resolver;
            MahApps.Metro.ThemeManager.AddAccent("PrussianBlue", new Uri("pack://application:,,,/DominatorUIUtility;component/Themes/PrussianBlue.xaml"));
            InitializeAutoMapper();
            var shell = Container.Resolve<MainWindow>();

            return shell;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            ex.DebugLog();
        }


        // Will attempt to load missing assembly from either x86 or x64 subdir
        private static Assembly Resolver(object sender, ResolveEventArgs args)
        {
            if (args.Name.StartsWith("CefSharp"))
            {
                string assemblyName = args.Name.Split(new[] { ',' }, 2)[0] + ".dll";
                string archSpecificPath = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase,
                                                       Environment.Is64BitProcess ? "x64" : "x86",
                                                       assemblyName);

                return File.Exists(archSpecificPath)
                           ? Assembly.LoadFile(archSpecificPath)
                           : null;
            }

            return null;
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
            StartupBaseViewModel.GetFaceBookActivity = activityType => new FacebookActivity().GetActivity(activityType);
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

        bool IsAlreadyRunning()
        {
            return CheckByProcess();
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
