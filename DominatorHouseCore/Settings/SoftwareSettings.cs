using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Config;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Microsoft.Win32;
using Registry = Microsoft.Win32.Registry;
using DominatorHouseCore.DatabaseHandler.Utility;

namespace DominatorHouseCore.Settings
{
    public interface ISoftwareSettings
    {
        void InitializeOnLoadConfigurations();
        void ActivityManagerInitializer();
        void ScheduleAutoUpdation();
        Task ScheduleAdsScraping();
        SoftwareSettingsModel Settings { get; set; }
        bool Save();
    }

    public class SoftwareSettings : BindableBase, ISoftwareSettings
    {
        public ISoftwareSettingsFileManager _softwareSettingsFileManager;
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
        private SoftwareSettingsModel _settings;

        public SoftwareSettingsModel Settings
        {
            get { return _settings; }
            set { SetProperty(ref _settings, value); }
        }

        public void InitializeOnLoadConfigurations()
        {
            CheckSocinatorIcon();

            if (CheckConfigurationFiles())
            {
                Settings = _softwareSettingsFileManager.GetSoftwareSettings();
            }
            //OtherInitializers();
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
            // AddDHToStartup(Settings);
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
                Utilities.DownloadSocinatorIcon();
        }

        #region Producer Consumer Solution for Account Update

        public void ScheduleAutoUpdation()
        {
            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var socinatorSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (!socinatorSettings.IsStopAutoSynchronizeAccount)
                return;

            var cancellationtokenSource = new CancellationTokenSource();

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;
            JobManager.AddJob(() =>
            {
                var registeredNetwork = SocinatorInitialize.GetRegisterNetwork();
                var accounts = _accountsFileManager.GetAll().Where(x =>
                    registeredNetwork.Contains(x.AccountBaseModel.AccountNetwork));

                var accountsToUpdate = accounts.Where(x =>
                    DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();
                if (accountsToUpdate.Count != 0)
                {
                    Task.Factory.StartNew(() =>
                        {
                            int count = 0;
                            accountsToUpdate.ForEach(account =>
                            {
                                UpdateAccount(account, cancellationtokenSource);
                                if (++count >= socinatorSettings.SimultaneousAccountUpdateCount)
                                {
                                    Thread.Sleep(20000);
                                    count = 0;
                                }
                                Thread.Sleep(2);
                            });

                        }, cancellationtokenSource.Token);
                }
            }, x => x.ToRunNow().AndEvery(accountSynchronizationHours).Hours().At(5));
        }
        #region Old AutoSchedule code

        //private void AccountUpdateProducer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource, int accountSynchronizationHours)
        //{
        //    var accounts = _accountsFileManager.GetAll();

        //    var accountsToUpdate = accounts.Where(x =>
        //        DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

        //    #region Schedule jobs for account update

        //    var scheduleUpdateAccount = accounts.Except(accountsToUpdate);

        //    foreach (var account in scheduleUpdateAccount)
        //    {
        //        var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeUtc();

        //        JobManager.AddJob(() =>
        //        {
        //            try
        //            {
        //                accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
        //                if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
        //                    Thread.Sleep(5000);
        //            }
        //            catch (ArgumentException ex)
        //            {
        //                ex.DebugLog();
        //            }
        //            catch (Exception ex)
        //            {
        //                ex.DebugLog();
        //            }
        //        }, s => s.ToRunOnceAt(dateTime).AndEvery(accountSynchronizationHours).Hours());
        //    }

        //    #endregion

        //    foreach (var account in accountsToUpdate)
        //    {
        //        try
        //        {
        //            var status = accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
        //            if (accountUpdateCollection.Count == accountUpdateCollection.BoundedCapacity)
        //                Thread.Sleep(15000);
        //        }
        //        catch (OperationCanceledException)
        //        {
        //            accountUpdateCollection.CompleteAdding();
        //            break;
        //        }
        //        catch (Exception ex)
        //        {
        //            ex.DebugLog();
        //        }
        //    }

        //    accountUpdateCollection.CompleteAdding();

        //}

        //private void AccountUpdateConsumer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource)
        //{
        //    while (!accountUpdateCollection.IsCompleted)
        //    {
        //        try
        //        {
        //            DominatorAccountModel dominatorAccountModel;

        //            if (accountUpdateCollection.TryTake(out dominatorAccountModel, -1, cancellationTokenSource.Token))
        //            {
        //                UpdateAccount(dominatorAccountModel, cancellationTokenSource);
        //            }

        //            Thread.SpinWait(500000);
        //        }
        //        catch (OperationCanceledException ex)
        //        {
        //            ex.DebugLog("Operation Cancelled!");
        //            break;
        //        }
        //    }
        //}

        #endregion
        public void UpdateAccount(DominatorAccountModel account, CancellationTokenSource cancellationTokenSource)
        {
            var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                                    .GetNetworkCoreFactory().AccountUpdateFactory;

            var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

            if (asyncAccount == null)
                return;

            Task.Factory.StartNew(async () =>
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
                    var globalDbOperation = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                    globalDbOperation.UpdateAccountDetails(account);
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

        }

        #endregion

        public async Task ScheduleAdsScraping()
        {
            var adScraperblock = new ActionBlock<ScrapAdsDetails>(
                async job =>
                {
                    await job.StartAdScarperAsync();
                },
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = 5 });

            await ScrapAdsProduceAsync(adScraperblock);
        }


        private async Task<bool> ScrapAdsProduceAsync(ActionBlock<ScrapAdsDetails> adsActionBuffer)
        {
            var accounts = _accountsFileManager.GetAll(SocialNetworks.Facebook);

            ListHelper.Shuffle(accounts);

            foreach (var account in accounts)
            {
                await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));
            }

            var jobId = Guid.NewGuid().ToString();

            JobManager.AddJob(async () => { await ScrapAdsProduceAsync(adsActionBuffer); },
                s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddHours(1)));

            return true;
        }
    }

    public class ScrapAdsDetails
    {
        public string AccountId { get; set; }

        public DominatorAccountModel account { get; set; }

        public ScrapAdsDetails(DominatorAccountModel AccountModel)
        {
            account = AccountModel;
        }

        public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken
        {
            get;
            set;
        }
            = new ConcurrentDictionary<string, CancellationTokenSource>();


        public async Task StartAdScarperAsync()
        {

            try
            {
                var cancellationTokenSource =
                    AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

                var asyncAccount =
                    ServiceLocator.Current.GetInstance<IAdScraperFactory>(SocialNetworks.Facebook.ToString());

                var runningAccounts = ServiceLocator.Current
                    .GetInstance<IPostScraperConstants>().LstRunningAccounts;

                if (runningAccounts.Contains(account.AccountId))
                    return;

                if (asyncAccount == null)
                    return;

                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

                    if (!checkResult)
                        return;

                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    await asyncAccount.ScrapeAdsAsync(account, cancellationTokenSource.Token);

                }
                catch (OperationCanceledException ex)
                {
                    ex.DebugLog("Cancellation Requested!");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
