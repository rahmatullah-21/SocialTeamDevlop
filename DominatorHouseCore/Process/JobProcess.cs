using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.BusinessLogic.ActivitiesWorkflow;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.DatabaseHandler.CoreModels;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using Newtonsoft.Json;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json.Linq;

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
        public JobProcess(string account, string template, ActivityType activityType, TimingRange currentJobTimeRange, SocialNetworks network)
        {
            // Get the current account details 
            DominatorAccountModel = AccountsFileManager.GetAccount(account, network);

            SocialNetworks = network;

            CurrentJobTimeRange = currentJobTimeRange;

            // Get the Template Model from the given template id
          //  var model = TemplatesCacheService.GetTemplatesCacheService().GetTemplateModels().FirstOrDefault(x => x.Id == template);
            var model = TemplatesFileManager.GetTemplateById(template);

            if (model != null)
            {
                JObject jsonObject = JObject.Parse(model.ActivitySettings);
                //dynamic deserializedValue = JsonConvert.DeserializeObject(model.ActivitySettings); ----//Todo 
                JobConfiguration = jsonObject["JobConfiguration"]?.ToObject<JobConfiguration>();
                //  JsonConvert.DeserializeObject<JobConfiguration>(jsonObject["JobConfiguration"].ToString());
                try
                {
                    SavedQueries = jsonObject["SavedQueries"]?.ToObject<List<QueryInfo>>();
                    // JsonConvert.DeserializeObject<List<QueryInfo>>(jsonObject["SavedQueries"].ToString());
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
            InitializeDatabaseConnection();
        }

        private void InitializeDatabaseConnection()
        {
            var networkCoreFactory = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory();
            if (CampaignId != null)
            {
                try
                {
                    DataBaseConnectionCampaign = networkCoreFactory.CampaignDatabase;
                    networkCoreFactory.CampaignInteractionDetails.InitializeInteraction();
                    networkCoreFactory.GlobalInteractionDetails.InitializeInteraction();
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }

            DataBaseConnectionAccount = networkCoreFactory.AccountDatabase;

            //  DataBaseConnectionAccount = objDatabaseHandler.GetDataBaseConnection(DominatorAccountModel.AccountBaseModel.AccountId, SocialNetworks);            
        }

        protected void ScheduleNextJob(DateTime dateTime)
        {
            //Stop();
            if (SoftwareSettingsFileManager.GetSoftwareSettings()?.IsEnableParallelActivitiesChecked ?? false)
            {
                DominatorScheduler.ScheduleActivityForNextJob(DominatorAccountModel, DominatorAccountModel.AccountBaseModel.AccountNetwork, ActivityType);
            }
            else
            {
                if (RunningJobProcesses != null)
                {
                    foreach (var jobProcess in RunningJobProcesses.Values)
                    {
                        if (jobProcess.Id.Contains(AccountId + "-------"))
                        {
                            return;
                        }
                    }
                }
                RunningActivityManager.StartNextRound(DominatorAccountModel);
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
        protected abstract bool CheckJobProcessLimitsReached();

        protected void StopFollow()
        {
            Stop();

            var lstTemplateModel = TemplatesCacheService.GetTemplatesCacheService().GetTemplateModels().ToList();
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

        public void RunScrapper()
        {
            try
            {
                var scraperFactory = SocinatorInitialize.GetSocialLibrary(SocialNetworks).GetNetworkCoreFactory().QueryScraperFactory;

                var scraper = scraperFactory.Create(this);

                if (SavedQueries.Count == 0)
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

        protected IDatabaseConnection DataBaseConnectionCampaign { get; set; }

        protected IDatabaseConnection DataBaseConnectionAccount { get; set; }

        public string AccountName => DominatorAccountModel?.UserName;

        public string AccountId => DominatorAccountModel?.AccountId;

        /// <summary>
        /// To specify the given account is belongs to which networks
        /// </summary>
        public SocialNetworks SocialNetworks { get; set; }

        #endregion


        #region Job Process workflow routines


        // stores all running job processes. Key - TemplateId
        public static readonly Dictionary<string, JobProcess> RunningJobProcesses = new Dictionary<string, JobProcess>();

        private static readonly object SyncJobProcess = new object();

        public string Id => AsId(AccountId, TemplateId);

        public static string AsId(string account, string templateId)
        {
            return $"{account}-------{templateId}";
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

                    GlobusLogHelper.log.Info(Log.ProcessStarted, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

                    // Login and run scraper/poster from derived concrete classes
                    if (DominatorAccountModel.AccountBaseModel.Status == AccountStatus.Success)
                    {
                        if (Login())
                            RunScrapper();
                        else
                        {
                            GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "did not get processed as account failed to login");
                            DominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
                        }

                    }
                    else
                    {
                        GlobusLogHelper.log.Info(Log.CustomMessage, DominatorAccountModel.AccountBaseModel.AccountNetwork, DominatorAccountModel.AccountBaseModel.UserName, ActivityType, "Account was not logged in successfully last time, Please check Accoount Status first to get your activities processed");
                        DominatorScheduler.ScheduleNextActivity(DominatorAccountModel, ActivityType);
                    }


                }, JobCancellationTokenSource.Token);

                JobCancellationTokenSource.Token.Register(() =>
                {
                    Console.WriteLine("Cancellation requested!");
                });
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


                //DominatorAccountModel.NotifyCancelled();
                JobCancellationTokenSource.Cancel();

                GlobusLogHelper.log.Info(Log.ProcessStopped, DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    DominatorAccountModel.AccountBaseModel.UserName, ActivityType);

                RunningJobProcesses.Remove(Id);
                //JobCancellationTokenSource = null;
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
            if (NoOfActionPerformedCurrentJob >= MaxNoOfActionPerJob || NoOfActionPerformedCurrentWeek >= MaxNoOfActionPerWeek ||
                NoOfActionPerformedCurrentDay >= MaxNoOfActionPerDay || NoOfActionPerformedCurrentHour >= MaxNoOfActionPerHour)
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
    }
}