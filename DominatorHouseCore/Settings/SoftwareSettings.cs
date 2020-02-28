using System;
using System.Collections.Concurrent;
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
using DominatorHouseCore.Enums.FdQuery;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using System.Net;
using System.Text;

namespace DominatorHouseCore.Settings
{
    public interface ISoftwareSettings
    {
        void InitializeOnLoadConfigurations();
        void ActivityManagerInitializer();
        void ScheduleAutoUpdation(List<string> lstAccountsForUpdate = null);
        Task ScheduleAdsScraping();

        Task DeleteUnnecessaryCampaigns();
        Task<bool> ScrapAdsProduceAsync(ActionBlock<ScrapAdsDetails> adsActionBuffer, DominatorAccountModel currentAccount = null
            , SocialNetworks currentNetwork = SocialNetworks.Facebook);
        SoftwareSettingsModel Settings { get; set; }
        bool Save();
        ObservableCollection<LocationModel> AssignLocationList();
    }

    public class SoftwareSettings : BindableBase, ISoftwareSettings

    {
        public ISoftwareSettingsFileManager _softwareSettingsFileManager;
        private readonly IFileSystemProvider _fileSystemProvider;
        private readonly IGenericFileManager _genericFileManager;

        private readonly IAccountsFileManager _accountsFileManager;

        public ICampaignsFileManager _campaignFileManager;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IAccountsCacheService _accountsCacheService;

        private IDominatorScheduler _dominatorScheduler;
        public SoftwareSettings(ISoftwareSettingsFileManager softwareSettingsFileManager,
            IFileSystemProvider fileSystemProvider, IGenericFileManager genericFileManager,
            IAccountsFileManager accountsFileManager, ICampaignsFileManager campaignFileManager,
            IJobActivityConfigurationManager jobActivityConfigurationManager, IAccountsCacheService accountsCacheService)
        {
            _softwareSettingsFileManager = softwareSettingsFileManager;
            _fileSystemProvider = fileSystemProvider;
            _genericFileManager = genericFileManager;
            _accountsFileManager = accountsFileManager;
            _campaignFileManager = campaignFileManager;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _accountsCacheService = accountsCacheService;
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
                if (!(Settings.RunQueriesRandomly || Settings.RunQueriesBottomToTop || Settings.RunQueriesTopToBottom))
                    Settings.RunQueriesRandomly = true;
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
                if (!(Settings.RunQueriesRandomly || Settings.RunQueriesBottomToTop || Settings.RunQueriesTopToBottom))
                    Settings.RunQueriesRandomly = true;

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
            {
                FileUtilities.Copy(ConstantVariable.MyAppFolderPath + @"\" + $"{"LangKeyLegion".FromResourceDictionary()}Icon.png", ConstantVariable.GetSocinatorIcon());
                //if (!File.Exists(ConstantVariable.GetSocinatorIcon()))
                //    Utilities.DownloadSocinatorIcon();
            }
        }

        #region Producer Consumer Solution for Account Update

        public void ScheduleAutoUpdation(List<string> lstAccountsForUpdate = null)
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

                    account.AccountBaseModel.Status = AccountStatus.TryingToLogin;
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
            try
            {
                IGlobalDatabaseConnection dataBaseConnectionGlb = SocinatorInitialize.GetGlobalDatabase();
                var dbGlobalContext = dataBaseConnectionGlb.GetSqlConnection();
                var _dbGlobalListOperations = new DbOperations(dbGlobalContext);

                
                await Task.Factory.StartNew( async() =>
                {
                    var ListCountry = _dbGlobalListOperations.Get<DominatorHouseCore.DatabaseHandler.DHTables.LocationList>();
                    var dt = new List<DominatorHouseCore.DatabaseHandler.DHTables.LocationList>();

                    foreach (var country in new NonStaticUtilities().CountriesList())
                    {
                        try
                        {
                            if (country == "India" || country == "China")
                                continue;

                            if (ListCountry.Any(x => x.CountryName.Equals(country)))
                                continue;

                            LogHelper.GlobusLogHelper.log.Info(Log.CustomMessage, SocialNetworks.Social, "", "Download Location", $"Downloading cities for country {country}");


                            var request = (HttpWebRequest)WebRequest.Create($"http://209.250.252.53/DownloadForSocinator/CityListByCountries/{country}.txt");
                            var response = await request.GetResponseAsync();
                            string cityResponse = string.Empty;
                            using (var responseStream = response.GetResponseStream())
                            {
                                if (responseStream != null)
                                {
                                    var reader = new StreamReader(responseStream, Encoding.UTF8);
                                    cityResponse = reader.ReadToEnd();
                                }
                            }
                            dt = new List<DatabaseHandler.DHTables.LocationList>();
                            List<string> cityList = System.Text.RegularExpressions.Regex.Split(cityResponse, "\r\n").ToList();
                            cityList.ForEach(x =>
                            {
                                var lst = new DominatorHouseCore.DatabaseHandler.DHTables.LocationList()
                                {
                                    CountryName = country,
                                    CityName = x,
                                    IsSelected = false
                                };
                                dt.Add(lst);
                            });
                        }
                        catch (Exception ex)
                        {
                            ex.DebugLog();
                        }

                        _dbGlobalListOperations.AddRange(dt);
                    }
                });

               
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }


        public async Task<bool> ScrapAdsProduceAsync(ActionBlock<ScrapAdsDetails> adsActionBuffer,
            DominatorAccountModel currentAccount = null, SocialNetworks currentNetwork = SocialNetworks.Facebook)
        {
            var accounts = _accountsFileManager.GetAll(currentNetwork);

            if (currentAccount != null)
                accounts = accounts.Where(x => x.AccountId == currentAccount.AccountId).ToList();

            ListHelper.Shuffle(accounts);

            foreach (var account in accounts)
            {
                await adsActionBuffer.SendAsync(new ScrapAdsDetails(account, adsActionBuffer));
            }

            return true;
        }

        public ObservableCollection<LocationModel> AssignLocationList()
        {
            var countrySet = new ObservableCollection<LocationModel>();
            try
            {
                new NonStaticUtilities().CountriesList().ForEach(x =>
                {
                    countrySet.Add(new LocationModel()
                    {
                        CountryName = x
                    });
                });
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return countrySet;
        }

        public async Task DeleteUnnecessaryCampaigns()
        {
            var _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
            var allAccounts = _accountsFileManager.GetAll(SocialNetworks.Facebook);
            var removedCapaigns = Enum.GetNames(typeof(RemovedActivityType));
            allAccounts.ForEach(x =>
            {
                var moduleConfig = _jobActivityConfigurationManager[x.AccountId];
                if (moduleConfig != null)
                {
                    // Remove task from list
                    foreach (var moduleConfiguration in _jobActivityConfigurationManager[x.AccountId].Where(mc => removedCapaigns.Any(y => y == mc.ActivityType.ToString())).ToList())
                    {
                        _jobActivityConfigurationManager.Delete(x.AccountId, moduleConfiguration.ActivityType);

                        //Update ActivityManager of account in Db
                        _dbOperations.UpdateAccountActivityManager(x);
                    }
                }
            });

            _accountsCacheService.UpsertAccounts(allAccounts.ToArray());

            var campaignsToRemove = _campaignFileManager.Where(x => removedCapaigns.Any(y => x.SubModule.ToString() == y));
            campaignsToRemove.ForEach(x =>
            {
                _campaignFileManager.Delete(x);
                var _dataBaseHandler = ServiceLocator.Current.GetInstance<IDataBaseHandler>();
                UpdateAccount(allAccounts, x, x.SelectedAccountList);
                _dataBaseHandler.DeleteDatabase(new List<string> { x.CampaignId }, DatabaseType.CampaignType);

            });
        }

        private void UpdateAccount(List<DominatorAccountModel> allAccounts, CampaignDetails camp, List<string> selectedAccount)
        {
            try
            {
                var _dbOperations = new DbOperations(SocinatorInitialize.GetGlobalDatabase().GetSqlConnection());
                _dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                // remove template from each account
                allAccounts.ForEach(x =>
                {
                    var moduleConfig = _jobActivityConfigurationManager[x.AccountId].FirstOrDefault(mc => mc.TemplateId == camp.TemplateId);
                    if (moduleConfig != null)
                    {
                        // Stop active task related to campaign
                        _dominatorScheduler.StopActivity(x, camp.SubModule, camp.TemplateId, false);

                        // Remove task from list
                        foreach (var moduleConfiguration in _jobActivityConfigurationManager[x.AccountId].Where(mc => mc.TemplateId == camp.TemplateId).ToList())
                        {
                            _jobActivityConfigurationManager.Delete(x.AccountId, moduleConfiguration.ActivityType);

                            //Update ActivityManager of account in Db
                            _dbOperations.UpdateAccountActivityManager(x);
                        }
                    }
                });

                _accountsCacheService.UpsertAccounts(allAccounts.ToArray());
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }

    public class ScrapAdsDetails
    {
        public string AccountId { get; set; }

        public DominatorAccountModel account { get; set; }

        public ActionBlock<ScrapAdsDetails> _adsActionBuffer { get; set; }

        public ScrapAdsDetails(DominatorAccountModel AccountModel, ActionBlock<ScrapAdsDetails> adsActionBuffer)
        {
            account = AccountModel;
            _adsActionBuffer = adsActionBuffer;
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

                //var postScraperConstants = ServiceLocator.Current
                //    .GetInstance<IPostScraperConstants>();

                //if ((DateTime.Now - postScraperConstants.LastLcsJobTime).TotalHours > 10000)
                //    postScraperConstants.LastLcsJobTime = DateTime.Now.Subtract(TimeSpan.FromHours(4));

                AdUpdationType currentUpdationType;

                currentUpdationType = AdUpdationType.FbAds;


                var asyncAdScraperFactory =
                    ServiceLocator.Current.GetInstance<IAdScraperFactory>(currentUpdationType.ToString());

                if (asyncAdScraperFactory == null)
                    return;

                try
                {
                    cancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var checkResult = await asyncAdScraperFactory.CheckStatusAsync(account, cancellationTokenSource.Token);

                    var jobId = Guid.NewGuid().ToString();

                    if (checkResult)
                    {
                        cancellationTokenSource.Token.ThrowIfCancellationRequested();

                        await asyncAdScraperFactory.ScrapeAdsAsync(account, cancellationTokenSource.Token);

                        JobManager.AddJob(async () => { await ServiceLocator.Current.GetInstance<ISoftwareSettings>().ScrapAdsProduceAsync(_adsActionBuffer, account, account.AccountBaseModel.AccountNetwork); },
                           s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddHours(1)));

                    }
                    else
                    {
                        JobManager.AddJob(async () => { await ServiceLocator.Current.GetInstance<ISoftwareSettings>().ScrapAdsProduceAsync(_adsActionBuffer, account, account.AccountBaseModel.AccountNetwork); },
                           s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddHours(2)));

                        return;
                    }
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
