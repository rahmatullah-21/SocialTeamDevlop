using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Windows;

namespace DominatorHouse.Utilities
{
    public class SoftwareSettings
    {
        private AccessorStrategies _strategies;
        public void InitializeOnLoadConfigurations(AccessorStrategies strategies)
        {
            _strategies = strategies;
            CheckSocinatorIcon();
            ScheduleAccountUpdation();
            ActivityManagerInitializer();
            OtherInitializers();
        }

        private void OtherInitializers()
        {
            var softwareSettings = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
            var settings = softwareSettings.Settings;
            AddDHToStartup(settings);

        }

        private void AddDHToStartup(SoftwareSettingsModel settings)
        {

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (settings.IsRunDHAtStartUpChecked)
                rk.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            else
                rk.DeleteValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, false);


        }

        private void ActivityManagerInitializer()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var dominatorAccountViewModel = AccountCustomControl.GetAccountCustomControl(_strategies)
                            .DominatorAccountViewModel;
                    var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                    runningActivityManager.Initialize(dominatorAccountViewModel.LstDominatorAccountModel);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }


        private void CheckSocinatorIcon()
        {
            if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
            {
                DominatorHouseCore.Utility.Utilities.DownloadSocinatorIcon();
            }
        }

        #region Account Update

        private void ScheduleAccountUpdation()
        {
            var softwareSettings = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
            var socinatorSettings = softwareSettings.Settings;
            new ActionBlock<AccountDetailsUpdation>(
                            async job => await job.UpdateAccountAsync(),
                            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAccountUpdateCount });


        }

        #endregion

    }

}
