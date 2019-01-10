using CommonServiceLocator;
using DominatorHouseCore;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using DominatorUIUtility.CustomControl;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorUIUtility.ViewModel;
using FluentScheduler;
using Registry = Microsoft.Win32.Registry;

namespace DominatorHouse.Utilities
{
    public class SoftwareSettings
    {
        private AccessorStrategies _strategies;
        public void InitializeOnLoadConfigurations(AccessorStrategies strategies)
        {
            _strategies = strategies;
            Parallel.Invoke(CheckSocinatorIcon,
                            ScheduleAccountUpdation,
                            ActivityManagerInitializer,
                            OtherInitializers,
                            ScheduleUpdation);
            //CheckSocinatorIcon();
            //ScheduleAccountUpdation();
            //ActivityManagerInitializer();
            //OtherInitializers();
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



        private void ScheduleAccountUpdation()
        {
            try
            {
                var softwareSettings = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
                var socinatorSettings = softwareSettings.Settings;
                new ActionBlock<AccountDetailsUpdation>(
                                async job => await job.UpdateAccountAsync(),
                                new ExecutionDataflowBlockOptions { MaxDegreeOfParallelism = socinatorSettings.SimultaneousAccountUpdateCount });

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

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

        // This solution not used 
        #region Producer Consumer Solution for Account Update


        private void ScheduleUpdation()
        {

            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var socinatorSettings = softwareSettingsFileManager.GetSoftwareSettings();
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
            var account = ServiceLocator.Current.GetInstance<IAccountsFileManager>().GetAccountById(accountId);

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


    }







}
