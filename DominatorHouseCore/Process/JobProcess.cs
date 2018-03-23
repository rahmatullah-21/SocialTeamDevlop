using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;

namespace DominatorHouseCore.Process
{
    /// <summary>
    ///     Base abstract class for other jobs: FollowProcess, LikeProcess
    ///     Contains: account, job configuration (schedule), activity type (follow, unfollow, etc),
    ///     other helper objects.
    ///     Derived class have to implement PostScrapeProcess
    /// </summary>
    public abstract class JobProcess
    {
        
        public JobProcess
            (string account, string template, ActivityType activityType, TimingRange currentJobTimeRange,
            SocialNetworks network)
        {
            // Get the current account details 
            DominatorAccountModel =  AccountsFileManager.GetAccount(account);

            SocialNetworks = network;

            CurrentJobTimeRange = currentJobTimeRange;

            // Get the Template Model from the given template id
            var model = BinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == template);

            if (model != null)
            {
                dynamic deserializedValue = JsonConvert.DeserializeObject(model.ActivitySettings);

                JobConfiguration =
                    JsonConvert.DeserializeObject<JobConfiguration>(deserializedValue["JobConfiguration"].ToString());

                try
                {
                    SavedQueries =
                        JsonConvert.DeserializeObject<List<QueryInfo>>(deserializedValue["SavedQueries"].ToString());
                }
                catch
                {
                    SavedQueries = new List<QueryInfo>();
                }
            }

            TemplateId = template;
            CampaignId = CampaignsFileManager.Get().FirstOrDefault(x => x.TemplateId == TemplateId)?.CampaignId;
            ActivityType = activityType;

            InitializeActivityCount(account);
        }

        protected void InitializeActivityCount(string account)
        {
            MaxNoOfActionPerJob = JobConfiguration.ActivitiesPerJob.GetRandom();
            MaxNoOfActionPerHour = JobConfiguration.ActivitiesPerHour.GetRandom();
            MaxNoOfActionPerDay = JobConfiguration.ActivitiesPerDay.GetRandom();
            MaxNoOfActionPerWeek = JobConfiguration.ActivitiesPerWeek.GetRandom();
            InitializeDatabseConnection();
        }

        private void InitializeDatabseConnection()
        {
            DataBaseConnectionCampaign =
                DataBaseHandler.GetDataBaseConnectionInstance(CampaignId, SocialNetworks, DatabaseType.CampaignType);
            DataBaseConnectionAccount = DataBaseHandler.GetDataBaseConnectionInstance(
                DominatorAccountModel.AccountBaseModel.UserName, SocialNetworks, DatabaseType.AccountType);
        }


        protected void ScheduleNextJob(DateTime dateTime)
        {
            Stop();

            var today = DateTimeUtilities.GetDayOfWeek();

            var timeScheduleModel = JobConfiguration.RunningTime.First(x => x.DayOfWeek == today);

            if (!timeScheduleModel.IsEnabled)
                return;

            // get the hour and minute of current time
            var nextJobTimeSpan = DateTimeUtilities.GetTimeSpanForGivenTime(dateTime); //GetTimeSpanForGivenTime

            if (CurrentJobTimeRange.EndTime >= nextJobTimeSpan && nextJobTimeSpan > CurrentJobTimeRange.StartTime)
            {
                var templateId = DominatorAccountModel.ActivityManager.LstModuleConfiguration
                    .FirstOrDefault(x => x.ActivityType == ActivityType)
                    ?.TemplateId;

                JobManager.AddJob(
                    () =>
                    {
                        DominatorScheduler.RunActivity(DominatorAccountModel, templateId, CurrentJobTimeRange, ActivityType.ToString());
                    }, s => s.WithName(this.TemplateId).ToRunOnceAt(dateTime));
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
            GlobusLogHelper.log.Info($"Started other configuration with account => " +
                                     $"{DominatorAccountModel.AccountBaseModel.UserName} module => {ActivityType}");
        }


        /// <summary>
        ///     Calls after scrapping result from social network (e.g. Instagram feed).
        ///     If process completed (time or activities limits reached) then starts other configuration stuff
        /// </summary>
        /// <param name="scrapedResult">Data that obtained from network's feed</param>
        /// <returns></returns>
        public virtual JobProcessResult FinalProcess(ScrapeResultNew scrapedResult)
        {
            var jobProcessResult = PostScrapeProcess(scrapedResult);
            jobProcessResult.IsProcessCompleted = CheckJobProcessLimitsReached();

            if (jobProcessResult.IsProcessCompleted)
            {
                StartOtherConfiguration(scrapedResult);
                GlobusLogHelper.log.Info("Process completed with account => " +
                                         DominatorAccountModel.AccountBaseModel.UserName + " module => " +
                                         ActivityType);
            }
            return jobProcessResult;
        }


        /// <summary>
        ///     Checks wheter time limits(per hour/day/week) or activities count reached
        /// </summary>
        /// <returns>
        ///     true if limits reached and caller needds to process with Other Configuration
        /// </returns>
        protected virtual bool CheckJobProcessLimitsReached()
        {
            var currentTime = DateTimeUtilities.GetEpochTime();

            // Check weekly limit. If reached, Stop task and wait for next days.
            // TODO: implement schedule holder on a weekly basis.
            NoOfActionPerformedCurrentWeek = DataBaseConnectionCampaign
                .Get<InteractedUsers>(x => currentTime - x.Date <= 3600 * 24 * 7).Count();

            if (NoOfActionPerformedCurrentWeek > MaxNoOfActionPerWeek)
            {
                Stop();
                return true;
            }

            // Check daily limit
            // TODO: extend DominatorScheduler with holding days/weeks and obtain day of next job from there
            NoOfActionPerformedCurrentDay = DataBaseConnectionCampaign
                .Get<InteractedUsers>(x => currentTime - x.Date <= 3600 * 24).Count();
            if (NoOfActionPerformedCurrentDay > MaxNoOfActionPerDay)
            {
                Stop();
                return true;
            }

            // Check hourly limit. Wait a hour.
            // TODO: implement schedule holder on a weekly basis and extract next hours job from there.
            NoOfActionPerformedCurrentHour = DataBaseConnectionCampaign.Get<InteractedUsers>(x => currentTime - x.Date <= 3600).Count;
            if (NoOfActionPerformedCurrentHour > MaxNoOfActionPerHour)
            {
                // schedule next job on next hour
                ScheduleNextJob(DateTime.Now.AddHours(1));
                return true;
            }


            // Finally check max number of jobs limit
            if (NoOfActionPerformedCurrentJob >= MaxNoOfActionPerJob)
            {
                GlobusLogHelper.log.Info($"Number of {ActivityType} per job limit reached. Scheduling next job.");

                // Next job have to be after X minutes, e.g 10-20 minutes.
                // TODO: implement via DominatorScheduler
                var nextJobTime = DateTime.Now.AddMinutes(JobConfiguration.DelayBetweenJobs.GetRandom());

                GlobusLogHelper.log.Info($"Next job scheduled to {nextJobTime.ToString("hh:mm")}");
                ScheduleNextJob(nextJobTime);
                return true;
            }

            return false;
        }


        protected void StopFollow()
        {
            Stop();

            var lstTemplateModel = BinFileHelper.GetTemplateDetails().ToList();
            foreach (var template in lstTemplateModel)
                if (template.Id == TemplateId)
                    JsonConvert.DeserializeObject<JobConfiguration>(template.ActivitySettings).RunningTime.Clear();

            TemplatesFileManager.Save(lstTemplateModel);
        }


        /// <summary>
        ///     1. Obtains Scraper factory for active library (GD, PD, TD etc.)
        ///     2. Creates Scraper
        ///     3. Executes scraping based on queries for certain social network and job process
        /// </summary>
        /// <param name="jobProcess"></param>
        public void RunScrapper()
        {
            //var scraperFactory1 = DominatorHouseInitializer.ActiveLibrary.QueryScraperFactory;
            var scraperFactory = DominatorHouseInitializer.GetSocialLibrary(SocialNetworks).QueryScraperFactory; 

            var scraper = scraperFactory.Create(this);

            if (SavedQueries.Count == 0)
                scraper.ScrapeNoQueries();
            else
                scraper.ScrapeWithQueries();
        }

        #region Required Properties

        /// <summary>
        ///     To specific the action count for a current job
        /// </summary>
        protected int NoOfActionPerformedCurrentJob = 0;

        /// <summary>
        ///     To specify the maximum action count for a job
        /// </summary>
        protected int MaxNoOfActionPerJob;

        /// <summary>
        ///     To specific the action count for a current hour
        /// </summary>
        protected int NoOfActionPerformedCurrentHour;

        /// <summary>
        ///     To specify the maximum action count for a hour
        /// </summary>
        protected int MaxNoOfActionPerHour;

        /// <summary>
        ///     To specific the action count for a current day
        /// </summary>
        protected int NoOfActionPerformedCurrentDay;

        /// <summary>
        ///     To specify the maximum action count for a day
        /// </summary>
        protected int MaxNoOfActionPerDay;

        /// <summary>
        ///     To specific the action count for a current week
        /// </summary>
        protected int MaxNoOfActionPerWeek;

        /// <summary>
        ///     To specify the maximum action count for a week
        /// </summary>
        protected int NoOfActionPerformedCurrentWeek;

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
        public DominatorAccountModel DominatorAccountModel { get; set; }

        /// <summary>
        /// To get the job configurations from <see cref="DominatorHouseCore.Models.TemplateModel.ActivitySettings"/>
        /// </summary>
        public JobConfiguration JobConfiguration { get; set; }

        /// <summary>
        /// To get the activity type
        /// </summary>
        public ActivityType ActivityType { get; set; }

        /// <summary>
        /// To get the list of saved queries from <see cref="DominatorHouseCore.Models.TemplateModel.ActivitySettings"/>
        /// </summary>
        public List<QueryInfo> SavedQueries { get; set; }

        /// <summary>
        /// To specify running time range
        /// </summary>
        public TimingRange CurrentJobTimeRange { get; set; }

        public CancellationTokenSource JobCancellationTokenSource { get; set; }

        protected DataBaseConnection DataBaseConnectionCampaign { get; set; }

        protected DataBaseConnection DataBaseConnectionAccount { get; set; }

        public string AccountName => DominatorAccountModel?.UserName;

        /// <summary>
        /// To specify the given account is belongs to which networks
        /// </summary>
        public SocialNetworks SocialNetworks { get; set; }

        #endregion


        #region Job Process workflow routines


        // stores all running job processes. Key - TemplateId
        private static readonly Dictionary<string, JobProcess> RunningJobProcesses =
            new Dictionary<string, JobProcess>();


        private static readonly object SyncJobProcess = new object();


        private string Id => AsId(AccountName, TemplateId);


        public static string AsId(string account, string templateId)
        {
            return $"{account}-{templateId}";
        }


        /// <summary>
        ///     Main method to start process in thread
        /// </summary>
        /// <returns></returns>
        public void StartProcessAsync()
        {
            lock (SyncJobProcess)
            {
                if (RunningJobProcesses.ContainsKey(Id)) return;

                Debug.Assert(JobCancellationTokenSource == null);

                JobCancellationTokenSource = new CancellationTokenSource();
                RunningJobProcesses.Add(Id, this);

                var task = ThreadFactory.Instance.Start(() =>
                {
                    GlobusLogHelper.log.Info(
                        $"{ActivityType} process started with {DominatorHouseInitializer.ActiveSocialNetwork} account [{AccountName}]");
                    // Login and run scraper/poster from derived concrete classes
                    if (Login())
                        RunScrapper();
                }, JobCancellationTokenSource.Token);
            }
        }


        public static bool IsStarted(string accountName, string templateId)
        {
            lock (SyncJobProcess)
            {
                return RunningJobProcesses.ContainsKey(AsId(accountName, templateId));
            }
        }

        public void Stop()
        {
            lock (SyncJobProcess)
            {
                if (JobCancellationTokenSource == null ||
                    !RunningJobProcesses.ContainsKey(Id))
                    return;

                JobCancellationTokenSource.Cancel();
                GlobusLogHelper.log.Info($"{ActivityType} process stopped for {AccountName}");

                RunningJobProcesses.Remove(Id);
                JobCancellationTokenSource = null;
            }
        }

        public static bool Stop(string accountName, string templateId)
        {
            try
            {
                lock (SyncJobProcess)
                {
                    var id = AsId(accountName, templateId);
                    if (!RunningJobProcesses.ContainsKey(id))
                    {
                        GlobusLogHelper.log.Trace($"Job process with Id - {id} not found");
                        return false;
                    }

                    var jobProcess = RunningJobProcesses[id];
                    jobProcess.Stop();
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.ErrorLog();
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

                GlobusLogHelper.log.Info("Process started with account => " +
                                         DominatorAccountModel.AccountBaseModel.UserName + " module => " +
                                         ActivityType);

                if (!DominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info("Logging in with account => " +
                                             DominatorAccountModel.AccountBaseModel.UserName + " module => " +
                                             ActivityType);

                    logInProcess.LoginWithDataBaseCookies(DominatorAccountModel, true);
                }

                if (DominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info("Logged in successfully with account => " +
                                             DominatorAccountModel.AccountBaseModel.UserName + " module => " +
                                             ActivityType);

                    return true;
                }
            }
            catch (Exception Ex)
            {
                Ex.DebugLog();
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

            var seconds = JobConfiguration.DelayBetweenActivity.GetRandom();

            GlobusLogHelper.log.Info($"{seconds} seconds Delay before next {ActivityType}");

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
    }
}