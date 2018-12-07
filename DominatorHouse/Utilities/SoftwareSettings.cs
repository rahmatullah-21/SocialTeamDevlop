using System;
using System.IO;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Microsoft.Win32;

namespace DominatorHouse.Utilities
{
    public class SoftwareSettings
    {
        private AccessorStrategies _strategies;
        public void InitializeOnLoadConfigurations(AccessorStrategies strategies)
        {
            _strategies = strategies;
            CheckConfigurationFiles();
            ScheduleAccountUpdation();
            ActivityManagerInitializer();
            OtherInitializers();
        }

        private void OtherInitializers()
        {
            var settings = SoftwareSettingsFileManager.GetSoftwareSettings();

            if (settings == null)
                SoftwareSettingsFileManager.SaveSoftwareSettings(new SoftwareSettingsModel());
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
                    RunningActivityManager.Initialize(dominatorAccountViewModel.LstDominatorAccountModel);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            });
        }

        private void CheckConfigurationFiles()
        {
            CheckOrAddSoftwareSettingsFile();
            CheckSocinatorIcon();
        }

        private void CheckSocinatorIcon()
        {
            if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
            {
                DominatorHouseCore.Utility.Utilities.DownloadSocinatorIcon();
            }
        }

        private void CheckOrAddSoftwareSettingsFile()
        {
            if (!File.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                SoftwareSettingsModel SoftwareSettingsModel = new SoftwareSettingsModel();
                SoftwareSettingsFileManager.SaveSoftwareSettings(SoftwareSettingsModel);
            }
        }

        #region Account Update

        private void ScheduleAccountUpdation()
        {
            var socinatorSettings = SoftwareSettingsFileManager.GetSoftwareSettings();
            new ActionBlock<AccountDetailsUpdation>(
                            async job => await job.UpdateAccountAsync(),
                            new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAccountUpdateCount });


        }

        #endregion

    }
}
