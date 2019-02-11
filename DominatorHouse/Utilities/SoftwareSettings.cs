namespace DominatorHouse.Utilities
{

    //public class SoftwareSettings
    //{
    //    private readonly IAccountsFileManager _accountsFileManager;
    //    public SoftwareSettings()
    //    {
    //        _accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
    //    }
    //    public void InitializeOnLoadConfigurations(AccessorStrategies strategies)
    //    {
    //        CheckSocinatorIcon();
    //        ActivityManagerInitializer();
    //        OtherInitializers();
    //        ScheduleAutoUpdation();
    //    }

    //    private void OtherInitializers()
    //    {
    //        var softwareSettings = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
    //        var settings = softwareSettings.Settings ?? new SoftwareSettingsModel();
    //        AddDHToStartup(settings);
    //    }

    //    private void AddDHToStartup(SoftwareSettingsModel settings)
    //    {

    //        RegistryKey rk = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
    //        if (settings.IsRunDHAtStartUpChecked)
    //            rk.SetValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
    //        else
    //            rk.DeleteValue(System.Diagnostics.Process.GetCurrentProcess().ProcessName, false);


    //    }

    //private void ActivityManagerInitializer()
    //{
    //    Application.Current.Dispatcher.Invoke(() =>
    //    {
    //        try
    //        {
    //            var accounts = ServiceLocator.Current.GetInstance<IAccountCollectionViewModel>();
    //            var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
    //            runningActivityManager.Initialize(accounts.GetCopySync());
    //        }
    //        catch (Exception ex)
    //        {
    //            ex.DebugLog();
    //        }
    //    });
    //}
    //    private void ActivityManagerInitializer()
    //    {
    //        Application.Current.Dispatcher.Invoke(() =>
    //        {
    //            try
    //            {
    //                var lstDominatorAccountModel = _accountsFileManager.GetAll();
    //                var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
    //                runningActivityManager.Initialize(lstDominatorAccountModel);
    //            }
    //            catch (Exception ex)
    //            {
    //                ex.DebugLog();
    //            }
    //        });
    //    }


    //    private void CheckSocinatorIcon()
    //    {
    //        if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
    //        {
    //            DominatorHouseCore.Utility.Utilities.DownloadSocinatorIcon();
    //        }
    //    }

    //    #region Producer Consumer Solution for Account Update

    //    private void ScheduleAutoUpdation()
    //    {
    //        var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
    //        var socinatorSettings = softwareSettingsFileManager.GetSoftwareSettings();
    //        if (!socinatorSettings.IsStopAutoSynchronizeAccount)
    //            return;

    //        var accountUpdateCollection = new BlockingCollection<DominatorAccountModel>
    //                (socinatorSettings.SimultaneousAccountUpdateCount);

    //        var cancellationtokenSource = new CancellationTokenSource();

    //        var accountSynchronizationHours = socinatorSettings.AccountSynchronizationHours;

    //        ThreadFactory.Instance.Start(() =>
    //        {
    //            AccountUpdateProducer(accountUpdateCollection, cancellationtokenSource, accountSynchronizationHours);
    //        });

    //        ThreadFactory.Instance.Start(() =>
    //        {
    //            AccountUpdateConsumer(accountUpdateCollection, cancellationtokenSource);
    //        });
    //    }

    //    private void AccountUpdateProducer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource, int accountSynchronizationHours)
    //    {
    //        var accounts = _accountsFileManager.GetAll();

    //        var currentUpdateAccounts = accounts.Where(x =>
    //            DateTimeUtilities.GetEpochTime() - x.LastUpdateTime > accountSynchronizationHours * 3600).ToList();

    //        #region Schedule jobs for account update

    //        var scheduleUpdateAccount = accounts.Except(currentUpdateAccounts);

    //        foreach (var account in scheduleUpdateAccount)
    //        {
    //            var dateTime = (account.LastUpdateTime + accountSynchronizationHours * 3600).EpochToDateTimeUtc();

    //            JobManager.AddJob(() =>
    //            {
    //                try
    //                {
    //                    accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
    //                }
    //                catch (ArgumentException ex)
    //                {
    //                    ex.DebugLog();
    //                }
    //                catch (Exception ex)
    //                {
    //                    ex.DebugLog();
    //                }
    //            }, s => s.ToRunOnceAt(dateTime).AndEvery(accountSynchronizationHours).Hours());
    //        }

    //        #endregion

    //        foreach (var account in currentUpdateAccounts)
    //        {
    //            try
    //            {
    //                var status = accountUpdateCollection.TryAdd(account, -1, cancellationTokenSource.Token);
    //            }
    //            catch (OperationCanceledException)
    //            {
    //                accountUpdateCollection.CompleteAdding();
    //                break;
    //            }
    //            catch (Exception ex)
    //            {
    //                AccountUpdateConsumer(accountUpdateCollection, cancellationTokenSource);
    //                ex.DebugLog();
    //            }
    //        }

    //        accountUpdateCollection.CompleteAdding();

    //    }

    //    private void AccountUpdateConsumer(BlockingCollection<DominatorAccountModel> accountUpdateCollection, CancellationTokenSource cancellationTokenSource)
    //    {
    //        while (!accountUpdateCollection.IsCompleted)
    //        {
    //            try
    //            {
    //                DominatorAccountModel dominatorAccountModel;

    //                if (accountUpdateCollection.TryTake(out dominatorAccountModel, -1, cancellationTokenSource.Token))
    //                {
    //                    UpdateAccount(dominatorAccountModel, cancellationTokenSource);
    //                }

    //                Thread.SpinWait(500000);
    //            }
    //            catch (OperationCanceledException ex)
    //            {
    //                ex.DebugLog("Operation Cancelled!");
    //                break;
    //            }
    //        }
    //    }

    //    public void UpdateAccount(DominatorAccountModel account, CancellationTokenSource cancellationTokenSource)
    //    {
    //        if (!SocinatorInitialize.IsNetworkAvailable(account.AccountBaseModel.AccountNetwork))
    //            return;

    //        var accountFactory = SocinatorInitialize
    //            .GetSocialLibrary(account.AccountBaseModel.AccountNetwork)
    //            .GetNetworkCoreFactory().AccountUpdateFactory;

    //        var asyncAccount = accountFactory as IAccountUpdateFactoryAsync;

    //        if (asyncAccount == null)
    //            return;

    //        var updateAccount = new Task(async () =>
    //        {
    //            try
    //            {
    //                cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //                var checkResult = await asyncAccount.CheckStatusAsync(account, cancellationTokenSource.Token);

    //                if (!checkResult)
    //                    return;

    //                cancellationTokenSource.Token.ThrowIfCancellationRequested();

    //                await asyncAccount.UpdateDetailsAsync(account, cancellationTokenSource.Token);

    //                new SocinatorAccountBuilder(account.AccountBaseModel.AccountId)
    //                    .UpdateLastUpdateTime(DateTimeUtilities.GetEpochTime())
    //                    .SaveToBinFile();
    //            }
    //            catch (OperationCanceledException ex)
    //            {
    //                ex.DebugLog("Cancellation Requested!");
    //            }
    //            catch (Exception ex)
    //            {
    //                ex.DebugLog();
    //            }
    //        }, account.Token);

    //        updateAccount.Start();
    //    }

    //    #endregion
    //}

}
