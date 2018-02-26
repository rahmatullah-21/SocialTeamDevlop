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
        public DominatorCancellationTokenSource JobCancellationTokenSource { get; set; }
        public static Dictionary<string, string> DictRunningJobs = new Dictionary<string, string>();
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionCampaign { get; set; }
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionAccount { get; set; }
        
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
            JobCancellationTokenSource = new DominatorCancellationTokenSource(account, template);
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
            TaskAndThreadUtility.StopTask(this.DominatorAccountModel.AccountBaseModel.UserName, this.TemplateId);

            List<RunningTimes> lstTimings = this.JobConfiguration.RunningTime;

            var today = DateTimeUtilities.GetDayOfWeek();
            var timeScheduleModel = this.JobConfiguration.RunningTime.First((x => x.DayOfWeek == today));

            if (!timeScheduleModel.IsEnabled)
                return;

            // get the hour and minute of current time
            var nextJobTimeSpan = DateTimeUtilities.GetTimeSpanForGivenTime(dateTime);//GetTimeSpanForGivenTime


            JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{ActivityType.Follow.ToString()}-{this.TemplateId}");

            if (CurrentJobTimeRange.EndTime >= nextJobTimeSpan && nextJobTimeSpan > CurrentJobTimeRange.StartTime)
            {
                var TemplateId = DominatorAccountModel.ActivityManager.LstModuleConfiguration
                     .FirstOrDefault(x => x.ActivityType == ActivityType.Follow).TemplateId;
                JobManager.AddJob(
                    () =>
                    {
                        // use registered Factories
                        DominatorScheduler.RunActivity(DominatorAccountModel.AccountBaseModel.UserName, TemplateId, CurrentJobTimeRange,
                            ActivityType.Follow.ToString(), SocialNetworks.Facebook);

                    }, s => s.WithName($"{ActivityType.Follow.ToString()}-{this.TemplateId}").ToRunOnceAt(dateTime));
            }

        }

        /// <summary>
        /// Starts process for certain social network. Must use JobProcess.StartProcess(ILoginProcess)
        /// </summary>
        public abstract void StartProcess();

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
            lock (DictRunningJobs)
            {
                try
                {
                    if (DictRunningJobs.ContainsKey(TemplateId)) return;        // job already running

                    DictRunningJobs.Add(this.TemplateId, "");
                    Debug.Assert(!string.IsNullOrEmpty(this.campaignId));
                    
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
                finally
                {
                    try
                    {
                        DictRunningJobs.Remove(this.TemplateId);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }



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
            if (NoOfActionPerformedCurrentJob > MaxNoOfActionPerJob)
            {
                ScheduleNextJob(DateTime.Now.AddTicks(this.JobConfiguration.DelayBetweenJobs.GetRandom()));
                return true;
            }

            int currentTime = DateTimeUtilities.GetEpochTime();
            NoOfActionPerformedCurrentHour = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600).Count();
            if (NoOfActionPerformedCurrentHour > MaxNoOfActionPerHour)
            {
                ScheduleNextJob(DateTime.Now.AddMinutes(this.JobConfiguration.DelayBetweenJobs.GetRandom()));
                return true;
            }

            NoOfActionPerformedCurrentDay = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24).Count();
            if (NoOfActionPerformedCurrentDay > MaxNoOfActionPerDay)
            {
                TaskAndThreadUtility.StopTask(this.DominatorAccountModel.AccountBaseModel.UserName, this.TemplateId);
                return true;
            }

            NoOfActionPerformedCurrentWeek = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24 * 7).Count();
            if (NoOfActionPerformedCurrentWeek > MaxNoOfActionPerWeek)
            {
                TaskAndThreadUtility.StopTask(this.DominatorAccountModel.AccountBaseModel.UserName, this.TemplateId);
                return true;
            }

            return false;
        }
        


        protected void StopFollow()
        {
            TaskAndThreadUtility.StopTask(this.DominatorAccountModel.AccountBaseModel.UserName, TemplateId);
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
