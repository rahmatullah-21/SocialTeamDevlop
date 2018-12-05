using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.FileManagers;
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

            RegistryKey rk = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
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


    public class ScrapAdsDetails
    {
        public string AccountId { get; set; }

        public DominatorAccountModel account { get; set; }

        public ScrapAdsDetails(DominatorAccountModel AccountModel)
        {
            account = AccountModel;
        }

        public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken { get; set; }
            = new ConcurrentDictionary<string, CancellationTokenSource>();


        public async Task StartAdScarperAsync(ActionBlock<ScrapAdsDetails> adsActionBlock)
        {

            //try
            //{
            //    var cancellationTokenSource = AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

            //    if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
            //        return;

            //    var accountFactory = SocinatorInitialize
            //        .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
            //        .GetNetworkCoreFactory().AdScraperFactory;

            //    var asyncAccount = accountFactory as IAdScraperFactory;

            //    if (asyncAccount == null)
            //        return;

            //    try
            //    {
            //        cancellationTokenSource.Token.ThrowIfCancellationRequested();

            //        var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

            //        if (!checkResult)
            //            return;

            //        cancellationTokenSource.Token.ThrowIfCancellationRequested();

            //        await asyncAccount.ScrapeAdsAsync(account, cancellationTokenSource.Token);

            //        var dateTime = DateTime.Now;

            //        if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook ||
            //        account.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit)
            //            dateTime = DateTime.Now.AddMinutes(15);

            //        else if (account.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram)
            //            dateTime = DateTime.Now.AddMinutes(5);

            //        JobManager.AddJob(async () =>
            //        {
            //            try
            //            {
            //                await adsActionBlock.SendAsync(new ScrapAdsDetails(account));
            //            }
            //            catch (ArgumentException ex)
            //            {
            //                ex.DebugLog();
            //            }
            //            catch (Exception ex)
            //            {
            //                ex.DebugLog();
            //            }
            //        }, s => s.ToRunOnceAt(dateTime));

            //    }
            //    catch (OperationCanceledException ex)
            //    {
            //        ex.DebugLog("Cancellation Requested!");
            //    }
            //    catch (Exception ex)
            //    {
            //        ex.DebugLog();
            //    }
            //}
            //catch (Exception ex)
            //{

            //}
        }

    }
}
