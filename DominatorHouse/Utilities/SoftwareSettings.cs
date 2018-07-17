using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using DominatorUIUtility.ViewModel;
using FluentScheduler;
using Microsoft.Win32;

namespace DominatorHouse.Utilities
{
    public class SoftwareSettings
    {
        private DominatorAccountViewModel.AccessorStrategies _strategies;
        public void InitializeOnLoadConfigurations(DominatorAccountViewModel.AccessorStrategies strategies)
        {
            _strategies = strategies;
            CheckConfigurationFiles();
            ScheduleUpdation();
            ActivityManagerInitializer();
            OtherInitializers();
        }

        private void OtherInitializers()
        {
            var settings = SoftwareSettingsFileManager.GetSoftwareSettings();
            AddDHToStartup(settings);

        }

        private void AddDHToStartup(SoftwareSettingsModel settings)
        {

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (settings.IsRunDHAtStartUpChecked)
                rk.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            else
                rk.DeleteValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, false);


        }

        private void ActivityManagerInitializer()
        {
            var dominatorAccountViewModel = AccountCustomControl.GetAccountCustomControl(_strategies)
                .DominatorAccountViewModel;
            RunningActivityManager.Initialize(dominatorAccountViewModel.LstDominatorAccountModel);
        }

        private void CheckConfigurationFiles()
        {
            CheckOrAddSoftwareSettingsFile();
        }

        private void CheckOrAddSoftwareSettingsFile()
        {
            if (!File.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                SoftwareSettingsModel SoftwareSettingsModel = new SoftwareSettingsModel();
                SoftwareSettingsFileManager.SaveSoftwareSettings(SoftwareSettingsModel);
            }
        }

        private void ScheduleUpdation()
        {
            var dominatorAccountViewModel = AccountCustomControl.GetAccountCustomControl(_strategies)
                .DominatorAccountViewModel;
            var softwareSetting = SoftwareSettingsFileManager.GetSoftwareSettings();
            var AccountSynchronizationHours = softwareSetting.AccountSynchronizationHours;
            dominatorAccountViewModel.LstDominatorAccountModel.ForEach(account =>
            {
                if ((DateTimeUtilities.GetEpochTime() - account.LastUpdateTime) > AccountSynchronizationHours * 3600)
                {
                    try
                    {
                        var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                       .GetNetworkCoreFactory().AccountUpdateFactory;
                        UpdateAccountAsync(dominatorAccountViewModel, softwareSetting, account, accountFactory);
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }
                else
                {
                    var dateTime = DateTimeUtilities.EpochToDateTimeUtc(account.LastUpdateTime + (AccountSynchronizationHours * 3600));
                    JobManager.AddJob(() =>
                    {
                        try
                        {
                            var accountFactory = SocinatorInitialize
                                              .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                              .GetNetworkCoreFactory().AccountUpdateFactory;
                            UpdateAccountAsync(dominatorAccountViewModel, softwareSetting, account, accountFactory);
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                    }, s => s.ToRunOnceAt(dateTime).AndEvery(AccountSynchronizationHours).Hours());
                }
            });
        }

        private void UpdateAccountAsync(DominatorAccountViewModel dominatorAccountViewModel, SoftwareSettingsModel softwareSetting,
            DominatorAccountModel account, IAccountUpdateFactory accountFactory)
        {
            try
            {
                if (dominatorAccountViewModel._updateAccountList.Count >= softwareSetting.SimultaneousAccountUpdateCount)
                {
                    try
                    {
                        lock (dominatorAccountViewModel.AccountUpdateLock)
                        {
                            Monitor.Wait(dominatorAccountViewModel.AccountUpdateLock);
                        }
                    }
                    catch (Exception Ex)
                    {
                        GlobusLogHelper.log.Error(Ex.Message);
                    }
                }

                dominatorAccountViewModel.MultipleUpdate(account, "UpdateAllDetail", accountFactory);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}
