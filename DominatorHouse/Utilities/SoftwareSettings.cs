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
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
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
            // StartScrapAds();
            // ScheduleUpdation();
            ActivityManagerInitializer();
            OtherInitializers();
        }

        private void StartScrapAds()
        {
            ActionBlock<ScrapAdsDetails> adScraperblock = null;

            var socinatorSettings = SoftwareSettingsFileManager.GetSoftwareSettings();

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

            adScraperblock = new ActionBlock<ScrapAdsDetails>(
                async job => await job.StartAdScarperAsync(adScraperblock),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAdsScreperThreadCount });


            ScrapAdsProducer(adScraperblock, accountSynchronizationHours);
        }

        private void ScrapAdsProducer(ActionBlock<ScrapAdsDetails> adsActionBuffer, int accountSynchronizationHours)
        {
            var dominatorAccountViewModel = AccountCustomControl
                .GetAccountCustomControl(_strategies)
                .DominatorAccountViewModel;

            var accounts = dominatorAccountViewModel.LstDominatorAccountModel.ToList();

            accounts = accounts.Where(x => x.AccountBaseModel.AccountNetwork == SocialNetworks.Facebook ||
              x.AccountBaseModel.AccountNetwork == SocialNetworks.Instagram || x.AccountBaseModel.AccountNetwork == SocialNetworks.Reddit).ToList();

            ListHelper.ShuffleAccountsForAds(accounts);

            accounts.ForEach(async account =>
            {
                await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));
            });

        }

        public async Task StartAdScraperProcessAsyc(DominatorAccountModel account, ActionBlock<ScrapAdsDetails> adsActionBuffer)
        {





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

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

            var block = new ActionBlock<AccountDetailsUpdation>(
                async job => await job.UpdateAccountAsync(),
                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAccountUpdateCount });


        }



        private void AccountUpdateProducer(ITargetBlock<AccountDetailsUpdation> accountActionBuffer, ITargetBlock<ScrapAdsDetails> adsActionBuffer,
            int accountSynchronizationHours)
        {
            var dominatorAccountViewModel = AccountCustomControl
                .GetAccountCustomControl(_strategies)
                .DominatorAccountViewModel;

            var accounts = dominatorAccountViewModel.LstDominatorAccountModel;

            var currentUpdateAccounts = accounts.Where(x =>
                DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

            #region Schedule jobs for account update

            var scheduleUpdateAccount = accounts.Except(currentUpdateAccounts);

            foreach (var account in scheduleUpdateAccount)
            {
                var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeLocal();

                JobManager.AddJob(async () =>
                {
                    try
                    {
                        await accountActionBuffer.SendAsync(new AccountDetailsUpdation(account));
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

            //scheduleUpdateAccount.ForEach(async account =>
            //{
            //    await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));
            //});

            //currentUpdateAccounts.ForEach(async account =>
            //{
            //    await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));
            //});

            currentUpdateAccounts.ForEach(async account =>
            {
                await accountActionBuffer.SendAsync(new AccountDetailsUpdation(account));
            });

        }

        #endregion


        // This solution not used 
        #region Producer Consumer Solution for Account Update


        private void ScheduleUpdation()
        {
            var socinatorSettings = SoftwareSettingsFileManager.GetSoftwareSettings();

            var accountUpdateCollection = new BlockingCollection<string>
                (socinatorSettings.SimultaneousAccountUpdateCount);

            var cancellationtokenSource = new CancellationTokenSource();

            var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

            ThreadFactory.Instance.Start(() =>
            {
                AccountUpdateProducer(accountUpdateCollection, cancellationtokenSource, accountSynchronizationHours);
            });

            ThreadFactory.Instance.Start(() =>
            {
                AccountUpdateConsumer(accountUpdateCollection, cancellationtokenSource);
            });
        }

        private void AccountUpdateProducer(BlockingCollection<string> accountUpdateCollection, CancellationTokenSource cancellationTokenSource, int accountSynchronizationHours)
        {

            var dominatorAccountViewModel = AccountCustomControl
                .GetAccountCustomControl(_strategies)
                .DominatorAccountViewModel;

            var accounts = dominatorAccountViewModel.LstDominatorAccountModel;

            var currentUpdateAccounts = accounts.Where(x =>
                DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

            #region Schedule jobs for account update

            var scheduleUpdateAccount = accounts.Except(currentUpdateAccounts);

            foreach (var account in scheduleUpdateAccount)
            {
                var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeUtc();

                JobManager.AddJob(() =>
                {
                    try
                    {
                        accountUpdateCollection.TryAdd(account.AccountBaseModel.AccountId, -1, cancellationTokenSource.Token);
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

            foreach (var account in currentUpdateAccounts)
            {
                try
                {
                    var status = accountUpdateCollection.TryAdd(account.AccountBaseModel.AccountId, -1, cancellationTokenSource.Token);
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

        private void AccountUpdateConsumer(BlockingCollection<string> accountUpdateCollection, CancellationTokenSource cancellationTokenSource)
        {
            while (!accountUpdateCollection.IsCompleted)
            {
                try
                {
                    string accountId;

                    if (accountUpdateCollection.TryTake(out accountId, -1, cancellationTokenSource.Token))
                    {
                        UpdateAccount(accountId, cancellationTokenSource);
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

        public void UpdateAccount(string accountId, CancellationTokenSource cancellationTokenSource)
        {
            var account = AccountsFileManager.GetAccountById(accountId);

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



        #endregion

        #region Old Update Methods


        private void ScheduleUpdationOld()
        {
            try
            {
                var dominatorAccountViewModel = AccountCustomControl.GetAccountCustomControl(_strategies)
                        .DominatorAccountViewModel;
                var softwareSetting = SoftwareSettingsFileManager.GetSoftwareSettings();
                var AccountSynchronizationHours = softwareSetting.AccountSynchronizationHours;
                dominatorAccountViewModel.LstDominatorAccountModel.ForEach(account =>
                {
                    try
                    {
                        //if ((DateTimeUtilities.GetEpochTime() - account.LastUpdateTime) > AccountSynchronizationHours * 3600)
                        //{
                        try
                        {
                            var accountFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                                           .GetNetworkCoreFactory().AccountUpdateFactory;
                            UpdateAccountAsync(dominatorAccountViewModel, softwareSetting, account, accountFactory);
                        }
                        catch (ArgumentException ex)
                        {
                            ex.DebugLog();
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }
                        //}
                        //else
                        //{
                        //    var dateTime = DateTimeUtilities.EpochToDateTimeUtc(account.LastUpdateTime + (AccountSynchronizationHours * 3600));
                        //    JobManager.AddJob(() =>
                        //    {
                        //        try
                        //        {
                        //            var accountFactory = SocinatorInitialize
                        //                              .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                        //                              .GetNetworkCoreFactory().AccountUpdateFactory;
                        //            UpdateAccountAsync(dominatorAccountViewModel, softwareSetting, account, accountFactory);
                        //        }
                        //        catch (ArgumentException ex)
                        //        {
                        //            ex.DebugLog();
                        //        }
                        //        catch (Exception ex)
                        //        {
                        //            ex.DebugLog();
                        //        }
                        //    }, s => s.ToRunOnceAt(dateTime).AndEvery(AccountSynchronizationHours).Hours());
                        //}
                    }
                    catch (ArgumentException ex)
                    {
                        ex.DebugLog();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
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
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                }

                dominatorAccountViewModel.MultipleUpdate(account, "UpdateAllDetail", accountFactory);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        #endregion

        //private void StartScrapAds()
        //{
        //    var socinatorSettings = SoftwareSettingsFileManager.GetSoftwareSettings();

        //    var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

        //    var adScraperblock = new ActionBlock<ScrapAdsDetails>(
        //        async job => await job.StartAdScarperAsync(),
        //        new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAdsScreperThreadCount });


        //    ScrapAdsProducer(adScraperblock, accountSynchronizationHours);
        //}

        //private void ScrapAdsProducer(ActionBlock<ScrapAdsDetails> adsActionBuffer, int accountSynchronizationHours)
        //{
        //    var dominatorAccountViewModel = AccountCustomControl
        //        .GetAccountCustomControl(_strategies)
        //        .DominatorAccountViewModel;

        //    var accounts = dominatorAccountViewModel.LstDominatorAccountModel;

        //    ListHelper.Shuffle(accounts);

        //    accounts.ForEach(async account =>
        //    {
        //        await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));

        //        var dateTime = DateTime.Now.AddMinutes(15);

        //        JobManager.AddJob(async () =>
        //        {
        //            try
        //            {
        //                await adsActionBuffer.SendAsync(new ScrapAdsDetails(account));
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
        //    });

        //}


    }
    //public class ScrapAdsDetails
    //{
    //    public string AccountId { get; set; }

    //    public DominatorAccountModel account { get; set; }

    //    public ScrapAdsDetails(DominatorAccountModel AccountModel)
    //    {
    //        account = AccountModel;
    //    }

    //    public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken { get; set; }
    //        = new ConcurrentDictionary<string, CancellationTokenSource>();


    //    public async Task StartAdScarperAsync()
    //    {

    //        try
    //        {
    //            var cancellationTokenSource = AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

    //            if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
    //                return;

    //            var accountFactory = SocinatorInitialize
    //                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
    //                .GetNetworkCoreFactory().AdScraperFactory;

    //            var asyncAccount = accountFactory as IAdScraperFactory;

    //            if (asyncAccount == null)
    //                return;

    //            try
    //            {
    //                cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //                var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

    //                if (!checkResult)
    //                    return;

    //                cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //                await asyncAccount.ScrapeAdsAsync(account, cancellationTokenSource.Token);

    //            }
    //            catch (OperationCanceledException ex)
    //            {
    //                ex.DebugLog("Cancellation Requested!");
    //            }
    //            catch (Exception ex)
    //            {
    //                ex.DebugLog();
    //            }
    //        }
    //        catch (Exception ex)
    //        {

    //        }
    //    }


    //}

    public class AccountDetailsUpdation
    {
        public string AccountId { get; set; }

        public AccountDetailsUpdation(string accountId)
        {
            AccountId = accountId;
        }
        public DominatorAccountModel account { get; set; }

        public AccountDetailsUpdation(DominatorAccountModel AccountModel)
        {
            account = AccountModel;
        }

        public static ConcurrentDictionary<string, CancellationTokenSource> AccountUpdatesCancellationToken { get; set; }
            = new ConcurrentDictionary<string, CancellationTokenSource>();



        public async Task UpdateAccountAsync()
        {

            var cancellationTokenSource = AccountUpdatesCancellationToken.GetOrAdd(account.AccountId, token => new CancellationTokenSource());

            if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
                return;

            var accountFactory = SocinatorInitialize
                .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
                .GetNetworkCoreFactory().AccountUpdateFactory;

            var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

            if (asyncAccount == null)
                return;

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
        }


        public static void StopUpdate(string accountId)
        {
            CancellationTokenSource cancellationToken;

            var status = AccountUpdatesCancellationToken.TryGetValue(accountId, out cancellationToken);

            if (status)
                cancellationToken.Cancel();
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
