using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.DatabaseHandler.AccountDB.Tables;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using System.Diagnostics;
using System.Threading;

namespace DominatorHouseCore.Process
{
    /// <summary>
    /// Base abstract class for other jobs: FollowProcess, LikeProcess
    /// Contains: account, job configuration (schedule), activity type (follow, unfollow, etc),
    /// other helper objects.
    /// 
    /// Derived class have to implement PostScrapeProcess
    /// </summary>
    public abstract class JobProcess
    {
        #region Required Properties
        protected int NoOfActionPerformedCurrentJob = 0;
        protected int MaxNoOfActionPerJob = 0;
        protected int NoOfActionPerformedCurrentHour = 0;
        protected int MaxNoOfActionPerHour = 0;
        protected int NoOfActionPerformedCurrentDay = 0;
        protected int MaxNoOfActionPerDay = 0;
        protected int MaxNoOfActionPerWeek = 0;
        protected int NoOfActionPerformedCurrentWeek = 0;
        
        public string TemplateId { get; set; }
        public string campaignId { get; set; }
        public DominatorAccountModel DominatorAccountModel { get; set; }
        public JobConfiguration JobConfiguration { get; set; }
        public ActivityType ActivityType { get; set; }
        public List<QueryInfo> SavedQueries { get; set; }

        public TimingRange CurrentJobTimeRange { get; set; }
        public CancellationTokenSource JobCancellationTokenSource { get; set; }
        
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionCampaign { get; set; }
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionAccount { get; set; }

        public string AccountName => DominatorAccountModel?.UserName;

        #endregion

        public JobProcess(string account, string template, ActivityType activityType, TimingRange CurrentJobTimeRange)
        {
            this.DominatorAccountModel = FileManagers.AccountsFileManager.GetAll().FirstOrDefault(x => x.AccountBaseModel.UserName == account);
            this.CurrentJobTimeRange = CurrentJobTimeRange;
            TemplateModel model = BinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == template);
            this.JobConfiguration = Newtonsoft.Json.JsonConvert.DeserializeObject<JobConfiguration>(model.ActivitySettings);

            dynamic deserializedValue = JsonConvert.DeserializeObject(model.ActivitySettings);

            try { this.SavedQueries = JsonConvert.DeserializeObject<List<QueryInfo>>(deserializedValue["SavedQueries"].ToString()); }
            catch { this.SavedQueries = new List<QueryInfo>(); }

            this.TemplateId = template;
            this.campaignId = CampaignsFileManager.Get().FirstOrDefault(x => x.TemplateId == this.TemplateId)?.CampaignId;
            this.ActivityType = activityType;
            
            InitializeActivityCount(account);
        }

        protected void InitializeActivityCount(string account)
        {
            MaxNoOfActionPerJob = this.JobConfiguration.ActivitiesPerJob.GetRandom();
            MaxNoOfActionPerHour = JobConfiguration.ActivitiesPerHour.GetRandom();
            MaxNoOfActionPerDay = JobConfiguration.ActivitiesPerDay.GetRandom();
            MaxNoOfActionPerWeek = JobConfiguration.ActivitiesPerWeek.GetRandom();
            InitializeDatabseConnection();
        }
        private void InitializeDatabseConnection()
        {
            DataBaseConnectionCampaign = DataBaseHandler.GetDataBaseConnectionInstance(campaignId, DatabaseType.CampaignType);
            DataBaseConnectionAccount = DataBaseHandler.GetDataBaseConnectionInstance(DominatorAccountModel.AccountBaseModel.UserName, DatabaseType.AccountType);
        }



        protected void ScheduleNextJob(DateTime dateTime)
        {
            Stop();

            List<RunningTimes> lstTimings = this.JobConfiguration.RunningTime;

            var today = DateTimeUtilities.GetDayOfWeek();
            var timeScheduleModel = this.JobConfiguration.RunningTime.First(x => x.DayOfWeek == today);

            if (!timeScheduleModel.IsEnabled)
                return;

            // get the hour and minute of current time
            var nextJobTimeSpan = DateTimeUtilities.GetTimeSpanForGivenTime(dateTime);//GetTimeSpanForGivenTime
            
            if (CurrentJobTimeRange.EndTime >= nextJobTimeSpan && nextJobTimeSpan > CurrentJobTimeRange.StartTime)
            {
                var TemplateId = DominatorAccountModel.ActivityManager.LstModuleConfiguration
                     .FirstOrDefault(x => x.ActivityType == ActivityType.Follow).TemplateId;
                JobManager.AddJob(
                    () =>
                    {                        
                        DominatorScheduler.RunActivity(DominatorAccountModel.AccountBaseModel.UserName, TemplateId, CurrentJobTimeRange,
                            ActivityType.Follow.ToString());

                    }, s => s.WithName(this.TemplateId).ToRunOnceAt(dateTime));
            }

        }


        #region Job Process workflow routines


        // stores all running job processes. Key - TemplateId
        private static Dictionary<string, JobProcess> _runningJobProcesses = new Dictionary<string, JobProcess>();
        static object _syncJobProcess = new object();

        private string Id => AsId(AccountName, TemplateId);
        public static string AsId(string account, string templateId) => $"{account}-{templateId}";
        

        /// <summary>
        /// Main method to start process in thread
        /// </summary>
        /// <returns></returns>
        public Task StartProcessAsync()
        {
            lock (_syncJobProcess)
            {
                Debug.Assert(JobCancellationTokenSource == null);

                JobCancellationTokenSource = new CancellationTokenSource();
                _runningJobProcesses.Add(Id, this);

                var task = ThreadFactory.Instance.Start(() =>
                {
                    GlobusLogHelper.log.Info($"{ActivityType} process started with {DominatorHouseInitializer.ActiveSocialNetwork} account [{AccountName}]");
                    StartProcess();

                }, JobCancellationTokenSource.Token);

                return task;
            }
        }


        public static bool IsStarted(string accountName, string templateId)
        {
            lock (_syncJobProcess)
            {
                return _runningJobProcesses.ContainsKey(AsId(accountName, templateId));
            }        
        }

        public void Stop()
        {            
            lock (_syncJobProcess)
            {
                if (JobCancellationTokenSource == null ||
                    !_runningJobProcesses.ContainsKey(Id))
                    return;                

                JobCancellationTokenSource.Cancel();
                GlobusLogHelper.log.Info($"{ActivityType} process stopped for {AccountName}");

                _runningJobProcesses.Remove(Id);
                JobCancellationTokenSource = null;
            }
        }
        

        public static bool Stop(string accountName, string templateId)
        {
            try
            {
                lock (_syncJobProcess)
                {
                    var id = AsId(accountName, templateId);
                    if (!_runningJobProcesses.ContainsKey(id))
                    {
                        GlobusLogHelper.log.Trace($"Job process with Id - {id} not found");
                        return false;
                    }

                    var jobProcess = _runningJobProcesses[id];
                    jobProcess.Stop();
                }

                return true;
            }
            catch (Exception Ex)
            {
                Ex.ErrorLog();
                return false;
            }
        }


        /// <summary>
        /// Starts process for certain social network. Must use JobProcess.StartProcess(ILoginProcess).
        /// Use StartProcessAsync in consumer code to create task and start process.
        /// </summary>
        protected abstract void StartProcess();

        /// <summary>
        /// Does a POST request for certain process after login. Like Follow, Like, Comment etc.
        /// </summary>
        /// <param name="scrapeResult"></param>
        /// <returns></returns>
        public abstract JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult);


        /// <summary>
        /// Logs-in to social network and scrap data from its feed
        /// </summary>
        protected void StartProcess(ILoginProcess logInProcess)
        {
            try
            {               
                if (string.IsNullOrEmpty(this.campaignId))
                {
                    GlobusLogHelper.log.Debug($"Campign Id not set for {ActivityType} - {TemplateId}");
                    return;
                }

                GlobusLogHelper.log.Info("Process started with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
                if (!this.DominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info("Logging in with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());

                    logInProcess.LoginWithDataBaseCookies(this.DominatorAccountModel, true);
                }

                if (this.DominatorAccountModel.IsUserLoggedIn)
                {
                    GlobusLogHelper.log.Info("Logged in successfully with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());

                    RunScraper();
                }                

            }
            catch (Exception Ex)
            {
                Ex.DebugLog();                
            }           
        }


        public bool IsStopped()
        {
            lock (_syncJobProcess)
            {
                return JobCancellationTokenSource == null || JobCancellationTokenSource.IsCancellationRequested;
            }
        }

        #endregion      // task routines: start, stop, iscancelled


        /// <summary>
        /// Will be called when JobProcess complete.
        /// Starts actions that was selected by user from Other Configuration section.
        /// Like Follow/Unfollow for GramDominator/Follow module.
        /// </summary>
        /// <param name="scrapeResult"></param>
        public virtual void StartOtherConfiguration(ScrapeResultNew scrapeResult)
        {
            GlobusLogHelper.log.Info($"Started other configuration with account => " +
                    $"{DominatorAccountModel.AccountBaseModel.UserName} module => {ActivityType}");
        }


        /// <summary>
        /// Calls after scrapping result from social network (e.g. Instagram feed).
        /// If process completed (time or activities limits reached) then starts other configuration stuff
        /// </summary>
        /// <param name="ScrapedResult">Data that obtained from network's feed</param>
        /// <returns></returns>
        public virtual JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult)
        {
            JobProcessResult jobProcessResult = PostScrapeProcess(ScrapedResult);
            jobProcessResult.IsProcessCompleted = CheckJobProcessLimitsReached();

            if (jobProcessResult.IsProcessCompleted)
            {
                StartOtherConfiguration(ScrapedResult);
                GlobusLogHelper.log.Info("Process completed with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
            }
            return jobProcessResult;
        }


        /// <summary>
        /// Checks wheter time limits(per hour/day/week) or activities count reached
        /// </summary>
        /// <returns>
        /// true if limits reached and caller needds to process with Other Configuration        
        /// </returns>
        protected virtual bool CheckJobProcessLimitsReached()
        {
            int currentTime = DateTimeUtilities.GetEpochTime();

            // Check weekly limit. If reached, Stop task and wait for next days.
            // TODO: implement schedule holder on a weekly basis.
            NoOfActionPerformedCurrentWeek = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24 * 7).Count();
            if (NoOfActionPerformedCurrentWeek > MaxNoOfActionPerWeek)
            {
                Stop();
                return true;
            }

            // Check daily limit
            // TODO: extend DominatorScheduler with holding days/weeks and obtain day of next job from there
            NoOfActionPerformedCurrentDay = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24).Count();
            if (NoOfActionPerformedCurrentDay > MaxNoOfActionPerDay)
            {
                Stop();
                return true;
            }

            // Check hourly limit. Wait a hour.
            // TODO: implement schedule holder on a weekly basis and extract next hours job from there.
            NoOfActionPerformedCurrentHour = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600).Count();
            if (NoOfActionPerformedCurrentHour > MaxNoOfActionPerHour)
            {
                // schedule next job on next hour
                ScheduleNextJob(DateTime.Now.AddHours(1));
                return true;
            }

           
            // Finally check max number of jobs limit
            if (NoOfActionPerformedCurrentJob > MaxNoOfActionPerJob)
            {
                GlobusLogHelper.log.Info($"Number of {ActivityType} per job limit reached. Scheduling next job.");

                // Next job have to be after X minutes, e.g 10-20 minutes.
                // TODO: implement via DominatorScheduler
                var nextJobTime = DateTime.Now.AddMinutes(this.JobConfiguration.DelayBetweenJobs.GetRandom());

                GlobusLogHelper.log.Info($"Next job scheduled to {nextJobTime.ToString("hh:mm")}");
                ScheduleNextJob(nextJobTime);
                return true;
            }

            return false;
        }


        #region Delay methods

        public void DelayBeforeNextActivity()
        {
            if (IsStopped()) return;

            int seconds = JobConfiguration.DelayBetweenActivity.GetRandom();

            GlobusLogHelper.log.Info($"{seconds} seconds Delay before next {ActivityType}");

#if SKIP_DELAYS
            seconds = 2;
#endif            
            Thread.Sleep(seconds * 1000);
        }

        public void DelayBeforeNextJob()
        {
            if (IsStopped()) return;

            int minutes = JobConfiguration.DelayBetweenJobs.GetRandom();

            GlobusLogHelper.log.Info($"{minutes} minutes Delay before next job ({ActivityType})");

#if SKIP_DELAYS
            minutes = 0;
#endif            

            Thread.Sleep(minutes * (60 * 1000));
        }

        #endregion        


        protected void StopFollow()
        {
            Stop();

            List<TemplateModel> lstTemplateModel = BinFileHelper.GetTemplateDetails().ToList();
            foreach (var template in lstTemplateModel)
                if (template.Id == TemplateId)
                    JsonConvert.DeserializeObject<JobConfiguration>(template.ActivitySettings).RunningTime.Clear();

            TemplatesFileManager.Save(lstTemplateModel);
        }


        /// <summary>
        /// 1. Obtains Scraper factory for active library (GD, PD, TD etc.)
        /// 2. Creates Scraper
        /// 3. Executes scraping based on queries for certain social network and job process
        /// </summary>
        /// <param name="jobProcess"></param>
        public void RunScraper()
        {
            IScraperFactory scraperFactory = DominatorHouseInitializer.ActiveLibrary.QueryScraperFactory;
            AbstractQueryScraper scraper = scraperFactory.Create(this);
            scraper.ScrapeWithQueries();
        }
    }
}
