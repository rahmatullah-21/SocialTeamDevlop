using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Microsoft.Win32;
using Registry = Microsoft.Win32.Registry;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;

namespace DominatorHouseCore.Settings
{
    public interface ISoftwareSettings
    {
        void InitializeOnLoadConfigurations();
        void ActivityManagerInitializer();
        void ScheduleAutoUpdation();
        void ScheduleAdsScraping();
        SoftwareSettingsModel Settings { get; set; }
        bool Save();
    }

    public class SoftwareSettings : ISoftwareSettings
    {
        private readonly ISoftwareSettingsFileManager _softwareSettingsFileManager;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IGenericFileManager _genericFileManager;

        private readonly IAccountsFileManager _accountsFileManager;
        public SoftwareSettings(ISoftwareSettingsFileManager softwareSettingsFileManager, IFileSystemProvider fileSystemProvider, IGenericFileManager genericFileManager, IAccountsFileManager accountsFileManager)
        {
            _softwareSettingsFileManager = softwareSettingsFileManager;
            _fileSystemProvider = fileSystemProvider;
            _genericFileManager = genericFileManager;
            _accountsFileManager = accountsFileManager;
        }

        public SoftwareSettingsModel Settings { get; set; }

        public void InitializeOnLoadConfigurations()
        {
            CheckSocinatorIcon();

            if (CheckConfigurationFiles())
            {
                Settings = _softwareSettingsFileManager.GetSoftwareSettings();
            }
            OtherInitializers();
            if (_fileSystemProvider.Exists(ConstantVariable.GetURLShortnerServicesFile()))
            {
                var shortnerServices =
                    _genericFileManager.GetModel<UrlShortnerServicesModel>(ConstantVariable.GetURLShortnerServicesFile());
                ConstantVariable.BitlyLogin = shortnerServices.Login;
                ConstantVariable.BitlyApiKey = shortnerServices.ApiKey;
            }
        }

        private bool CheckConfigurationFiles()
        {
            if (!_fileSystemProvider.Exists(ConstantVariable.GetOtherSoftwareSettingsFile()))
            {
                Settings = new SoftwareSettingsModel
                {
                    IsEnableAdvancedUserMode = true,
                    IsStopAutoSynchronizeAccount = true
                };

                _softwareSettingsFileManager.SaveSoftwareSettings(Settings);

                return false;
            }

            return true;
        }

        public bool Save()
        {
            return _softwareSettingsFileManager.SaveSoftwareSettings(Settings);
        }

        private void OtherInitializers()
        {
            AddDHToStartup(Settings);
        }

        private void AddDHToStartup(SoftwareSettingsModel settings)
        {

            RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
            if (settings.IsRunDHAtStartUpChecked)
                rk.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            else
                rk.DeleteValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, false);


        }

        public void ActivityManagerInitializer()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                try
                {
                    var lstDominatorAccountModel = _accountsFileManager.GetAll();
                    var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                    runningActivityManager.Initialize(lstDominatorAccountModel);
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
                Utilities.DownloadSocinatorIcon();
            }
        }

        #region Producer Consumer Solution for Account Update

        public void ScheduleAutoUpdation()
        {
            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var socinatorSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!socinatorSettings.IsStopAutoSynchronizeAccount)
                return;

            var accountUpdateCollection = new BlockingCollection<DominatorAccountModel>
                    (socinatorSettings.SimultaneousAccountUpdateCount);

            var cancellationtokenSource = new CancellationTokenSource();

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

            var accounts = _accountsFileManager.GetAll();

            var accountsToUpdate = accounts.Where(x =>
                DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

            int count = 0;

            accountsToUpdate.ForEach(account =>
            {
                UpdateAccount(account, cancellationtokenSource);
                if (++count >= socinatorSettings.SimultaneousAccountUpdateCount)
                {
                    Thread.Sleep(20000);
                    count = 0;
                }
            });



            //ThreadFactory.Instance.Start(() =>
            //{
            //    AccountUpdateProducer(accountUpdateCollection, cancellationtokenSource, accountSynchronizationHours);
            //});

            //ThreadFactory.Instance.Start(() =>
            //{
            //    AccountUpdateConsumer(accountUpdateCollection, cancellationtokenSource);
            //});
        }


        public void ScheduleAdsScraping()
        {
            var accounts = _accountsFileManager.GetAll();

            var cancellationtokenSource = new CancellationTokenSource();

            accounts.ForEach(account =>
            {
                UpdateAds(account, cancellationtokenSource);
                
            });
        }

        private void AccountUpdateProducer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource, int accountSynchronizationHours)
        {
            var accounts = _accountsFileManager.GetAll();

            var accountsToUpdate = accounts.Where(x =>
                DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

            #region Schedule jobs for account update

            var scheduleUpdateAccount = accounts.Except(accountsToUpdate);

            foreach (var account in scheduleUpdateAccount)
            {
                var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeUtc();

                JobManager.AddJob(() =>
                {
                    try
                    {
                        accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
                        if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
                            Thread.Sleep(5000);
                    }
                    catch (ArgumentException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }, s => s.ToRunOnceAt(dateTime).AndEvery(accountSynchronizationHours).Hours());
            }

            #endregion

            foreach (var account in accountsToUpdate)
            {
                try
                {
                    var status = accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
                    if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
                        Thread.Sleep(15000);
                }
                catch (OperationCanceledException)
                {
                    accountUpdateCollection.CompleteAdding();
                    break;
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            accountUpdateCollection.CompleteAdding();

        }

        private void AccountUpdateConsumer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource)
        {
            while (!accountUpdateCollection.IsCompleted)
            {
                try
                {
                    DominatorAccountModel dominatorAccountModel;

                    if (accountUpdateCollection.TryTake(out dominatorAccountModel, -1, cancellationTokenSource.Token))
                    {
                        UpdateAccount(dominatorAccountModel, cancellationTokenSource);
                    }

                    Thread.SpinWait(500000);
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Operation Cancelled!");
                    break;
                }
            }
        }

        public void UpdateAccount(DominatorAccountModel account, CancellationTokenSource cancellationTokenSource)
        {
            if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
                return;

            var accountFactory = SocinatorInitialize
                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory().AccountUpdateFactory;

            var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

            if (asyncAccount == null)
                return;

            var updateAccount = new Task(async () =>
            {
                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

                    if (!checkResult)
                        return;

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    await asyncAccount.UpdateDetailsAsync(account, cancellationTokenSource.Token);

                    new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
                        .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
                        .SaveToBinFile();
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }, account.Token);

            updateAccount.Start();
        }


        public void UpdateAds(DominatorAccountModel account, CancellationTokenSource cancellationTokenSource, string jobId="")
        {
            if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork)
                || account.AccountBaseModel.AccountNetwork!=SocialNetworks.Facebook )
                return;

            GlobusLogHelper.log.Info(Log.CustomMessage, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, "", $"Started Ads scraping process with account {account.AccountBaseModel.UserName}");
           

            var adsFactory = SocinatorInitialize
                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory().AdScraperFactory;

            var asyncAccount = adsFactory as IAdScraperFactory;

            if (asyncAccount == null)
                return;

            var updateAd = new Task(async () =>
            {
                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

                    if (!checkResult)
                        return;

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    await asyncAccount.ScrapeAdsAsync(account, cancellationTokenSource.Token, jobId);

                    jobId = Guid.NewGuid().ToString();

                    JobManager.AddJob(() => { UpdateAds(account,cancellationTokenSource, jobId);},
                        s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddMinutes(30)));
                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }, account.Token);

            updateAd.Start();
        }

       
        #endregion
    }
}
