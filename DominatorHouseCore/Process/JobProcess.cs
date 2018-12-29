using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobConfigurations;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DominatorHouseCore.Process
{
    public interface IJobProcess
    {
        Task StartProcessAsync();
        void Stop();
        JobKey Id { get; }
        ReachedLimitInfo CheckLimit();
        JobConfiguration JobConfiguration { get; }
    }

    /// <summary>
    ///     Base abstract class for other jobs: FollowProcess, LikeProcess
    ///     Contains: account, job configuration (schedule), activity type (follow, unfollow, etc),
    ///     other helper objects.
    ///     Derived class have to implement PostScrapeProcess
    /// </summary>
    public abstract class JobProcess : IJobProcess
    {
        private readonly IRunningJobsHolder _runningJobsHolder;
        private readonly IJobCountersManager _jobCountersManager;

        [Obsolete("only for test! DO NOT DELETE, DO NOT USE!", true)]
        public JobProcess()
        {

        }

        public bool IsNeedToSchedule { get; set; } = false;
        protected JobProcess(string account, string template, ActivityType activityType, TimingRange currentJobTimeRange, SocialNetworks network)
        {
            // Get the current account details 
            TemplateId = template;
            ActivityType = activityType;
            SocialNetworks = network;
            CurrentJobTimeRange = currentJobTimeRange;
            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            var accountsFileManager = ServiceLocator.Current.GetInstance<IAccountsFileManager>();
            var jobConfigurationProvider = ServiceLocator.Current.GetInstance<IJobConfigurationProvider>();
            _runningJobsHolder = ServiceLocator.Current.GetInstance<IRunningJobsHolder>();
            _jobCountersManager = ServiceLocator.Current.GetInstance<IJobCountersManager>();
            DominatorAccountModel = accountsFileManager.GetAccount(account, network);

            var commonConfiguration =
                jobConfigurationProvider.GetJobConfiguration(DominatorAccountModel.AccountId, activityType);

            IsNeedToSchedule = commonConfiguration.IsNeedToSchedule;
            JobConfiguration = commonConfiguration.JobConfiguration;
            SavedQueries = commonConfiguration.SavedQueries;


            CampaignId = campaignFileManager.FirstOrDefault(x => x.TemplateId == TemplateId)?.CampaignId;
        }

        protected void ScheduleNextJob(DateTime dateTime)
        {
            //Stop();
            var softwareSettings = ServiceLocator.Current.GetInstance<ISoftwareSettings>();
            var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            if (softwareSettings.Settings?.IsEnableParallelActivitiesChecked ?? false)
            {
                dominatorScheduler.ScheduleActivityForNextJob(DominatorAccountModel, ActivityType);
            }
            else
            {
                if (_runningJobsHolder.IsActivityRunningForAccount(AccountId))
                    return;

                var runningActivityManager = ServiceLocator.Current.GetInstance<IRunningActivityManager>();
                runningActivityManager.StartNextRound(DominatorAccountModel);
            }

        }


        /// <summary>
        ///     Will be called when JobProcess complete.
        ///     Starts actions that was selected by user from Other Configuration section.
        ///     Like Follow/Unfollow for GramDominator/Follow module.
        /// </summary>
        /// <param name="scrapeResult"></param>
        public virtual void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            // GlobusLogHelper.log.Info(Log.OtherConfigurationStarted, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
        }

        /// <summary>
        ///     Calls after scrapping result from social network (e.g. Instagram feed).
        ///     If process completed (time or activities limits reached) then starts other configuration stuff
        /// </summary>
        /// <param name="scrapedResult">Data that obtained from network's feed</param>
        /// <returns></returns>
        public virtual JobProcessResult FinalProcess(ScrapeResultNew scrapedResult)
        {
            JobProcessResult jobProcessResult =
                new JobProcessResult { IsProcessCompleted = CheckJobProcessLimitsReached() };
            if (!jobProcessResult.IsProcessCompleted)
            {
                jobProcessResult = PostScrapeProcess(scrapedResult);
                //  DelayBeforeNextActivity();
            }
            else
            {
                StartOtherConfiguration(scrapedResult);

                GlobusLogHelper.log.Info(Log.ProcessCompleted, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);
            }

            return jobProcessResult;

        }


        /// <summary>
        /// Implement the functionality for checking the Job Process count
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        //protected abstract bool CheckJobProcessLimitsReached();
        protected virtual bool CheckJobProcessLimitsReached()
        {
            var limitType = ReachedLimitType.NoLimit;
            try
            {
                var limitInfo = CheckLimit();
                limitType = limitInfo.ReachedLimitType;
                if (limitType != ReachedLimitType.NoLimit)
                {
                    GlobusLogHelper.log.Info(limitInfo.ReachedLimitType.ConvertToLogRecord(),
                        DominatorAccountModel.AccountBaseModel.AccountNetwork,
                        DominatorAccountModel.AccountBaseModel.UserName, ActivityType, limitInfo.LimitValue);
                    Stop(DominatorAccountModel.AccountId, TemplateId);
                    var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                    var moduleConfiguration = jobActivityConfigurationManager[DominatorAccountModel.AccountId, ActivityType];
                    var nextStartTime = limitType == ReachedLimitType.Job ? DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType, JobConfiguration.DelayBetweenJobs.GetRandom()) : DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType);
                    if (moduleConfiguration != null)
                    {
                        moduleConfiguration.NextRun = nextStartTime;
                        moduleConfiguration.IsEnabled = true;
                        var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
                        jobActivityConfigurationManager.AddOrUpdate(DominatorAccountModel.AccountBaseModel.AccountId, ActivityType, moduleConfiguration);
                        accountsCacheService.UpsertAccounts(DominatorAccountModel);
                    }

                    var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                    dominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
                }

            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return limitType != ReachedLimitType.NoLimit;
        }

        public abstract ReachedLimitInfo CheckLimit();


        protected void StopFollow()
        {
            Stop();

            var lstTemplateModel = ServiceLocator.Current.GetInstance<ITemplatesCacheService>().GetTemplateModels().ToList();
            foreach (var template in lstTemplateModel)
                if (template.Id == TemplateId)
                    JsonConvert.DeserializeObject<JobConfiguration>(template.ActivitySettings).RunningTime.Clear();

            var TemplatesFileManager = ServiceLocator.Current.GetInstance<ITemplatesFileManager>();
            TemplatesFileManager.Save(lstTemplateModel);
        }


        /// <summary>
        ///     1. Obtains Scraper factory for active library (GD, PD, TD etc.)
        ///     2. Creates Scraper
        ///     3. Executes scraping based on queries for certain social network and job process
        /// </summary>

        private void RunScrapper()
        {
            try
            {
                var scraperFactory = ServiceLocator.Current.GetInstance<IQueryScraperFactory>(SocialNetworks.ToString());
                var scraper = scraperFactory.Create(this);

                if (SavedQueries == null || SavedQueries?.Count == 0)
                    scraper.ScrapeWithoutQueries(ActivityType.ToString());
                else
                    scraper.ScrapeWithQueries();
            }
            catch (NullReferenceException ex)
            {
                ex.DebugLog("Cancellation requested before initialization!");
            }
        }

        #region Required Properties

        /// <summary>
        /// To get the template Id
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// To get the campaign Id
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// To specify from which account the neccessary actions takes place.
        /// </summary>
        public DominatorAccountModel DominatorAccountModel { get; }

        /// <summary>
        /// To get the job configurations from <see cref="DominatorHouseCore.Models.TemplateModel.ActivitySettings"/>
        /// </summary>
        public JobConfiguration JobConfiguration { get; set; }

        /// <summary>
        /// To get the activity type
        /// </summary>
        public ActivityType ActivityType { get; }

        /// <summary>
        /// To get the list of saved queries from <see cref="DominatorHouseCore.Models.TemplateModel.ActivitySettings"/>
        /// </summary>
        public List<QueryInfo> SavedQueries { get; set; }

        /// <summary>
        /// To specify running time range
        /// </summary>
        public TimingRange CurrentJobTimeRange { get; set; }

        public CancellationTokenSource JobCancellationTokenSource { get; set; }

        public string AccountName => DominatorAccountModel?.UserName;

        public string AccountId => DominatorAccountModel?.AccountId;

        /// <summary>
        /// To specify the given account is belongs to which networks
        /// </summary>
        public SocialNetworks SocialNetworks { get; set; }

        #endregion


        #region Job Process workflow routines

        private static readonly object SyncJobProcess = new object();

        public JobKey Id => AsId(AccountId, TemplateId);

        public static JobKey AsId(string account, string templateId)
        {
            return new JobKey(account, templateId);
        }

        /// <summary>
        ///     Main method to start process in thread
        /// </summary>
        /// <returns></returns>
        public Task StartProcessAsync()
        {
            lock (SyncJobProcess)
            {
                if (!_runningJobsHolder.StartIfNotRunning(Id, this)) return Task.CompletedTask;

                Debug.Assert(JobCancellationTokenSource == null);

                JobCancellationTokenSource = new CancellationTokenSource();

                var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                var task = ThreadFactory.Instance.Start(() =>
                  {

                      GlobusLogHelper.log.Info(Log.ProcessStarted, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

                      // Login and run scraper/poster from derived concrete classes
                      if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                      {
                          if (Login())
                              RunScrapper();
                          else
                          {
                              GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, $"did not get processed as account failed to login [{DominatorAccountModel.AccountBaseModel.Status}]");
                              dominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
                          }

                      }
                      else
                      {
                          GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Account was not logged in successfully last time, Please check Accoount Status first to get your activities processed");
                          dominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
                      }


                  }, JobCancellationTokenSource.Token);

                JobCancellationTokenSource.Token.Register(() =>
                {
                    Console.WriteLine("Cancellation requested!");
                });

                return task;
            }
        }

        public void Stop()
        {
            lock (SyncJobProcess)
            {
                JobCancellationTokenSource?.Cancel();
                GlobusLogHelper.log.Info(Log.ProcessStopped, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

            }
        }

        public static bool Stop(string accountName, string templateId)
        {
            try
            {
                var runningJobsHolder = ServiceLocator.Current.GetInstance<IRunningJobsHolder>();
                var id = new JobKey(accountName, templateId);
                if (!runningJobsHolder.Stop(id))
                {
                    GlobusLogHelper.log.Trace($"Job process with Id - {id} not found");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }


        /// <summary>
        ///     Starts process for certain social network. Must use JobProcess.StartProcess(ILoginProcess).
        ///     Use StartProcessAsync in consumer code to create task and start process.
        /// </summary>
        protected abstract bool Login();

        /// <summary>
        ///     Does a POST request for certain process after login. Like Follow, Like, Comment etc.
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>
        public abstract JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult);


        /// <summary>
        ///     Logs-in to social network and scrap data from its feed
        /// </summary>
        protected bool LoginBase(ILoginProcess logInProcess)
        {
            try
            {
                if (string.IsNullOrEmpty(CampaignId) && string.IsNullOrEmpty(TemplateId))
                {
                    GlobusLogHelper.log.Info($"Campign Id not set for {ActivityType} - {TemplateId}");
                    return false;
                }

                GlobusLogHelper.log.Info(Log.StartingJob, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);


                if (!DominatorAccountModel.IsUserLoggedIn || (DominatorAccountModel.HttpHelper.GetRequestParameter().Cookies == null))
                {
                    GlobusLogHelper.log.Info(Log.AccountLogin, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName);

                    logInProcess.LoginWithDataBaseCookies(DominatorAccountModel, true);
                }

                if (DominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info(Log.SuccessfulLogin, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

            return false;
        }


        public bool IsStopped()
        {
            lock (SyncJobProcess)
            {
                return JobCancellationTokenSource == null || JobCancellationTokenSource.IsCancellationRequested;
            }
        }

        #endregion      // task routines: start, stop, iscancelled


        #region Delay methods

        public void DelayBeforeNextActivity()
        {
            if (IsStopped()) return;
            var limitType = CheckLimit();
            if (limitType.ReachedLimitType != ReachedLimitType.NoLimit)
            {
                return;
            }

            var seconds = JobConfiguration.DelayBetweenActivity.GetRandom();

            GlobusLogHelper.log.Info(Log.DelayBetweenActivity, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, seconds);

            Thread.Sleep(seconds * 1000);
        }

        public void DelayBeforeNextJob()
        {
            if (IsStopped()) return;

            var minutes = JobConfiguration.DelayBetweenJobs.GetRandom();

            GlobusLogHelper.log.Info($"{minutes} minutes Delay before next job ({ActivityType})");

#if SKIP_DELAYS
            minutes = 0;
#endif

            Thread.Sleep(minutes * 60 * 1000);
        }

        #endregion

        protected void IncrementCounters()
        {
            _jobCountersManager.InitOrIncrement(Id);
        }
    }
}