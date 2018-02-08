using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataBaseConnection.CommonDatabaseConnection.Tables.Account;
using DominatorHouseCore.Enums;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;

namespace DominatorHouseCore.Process
{
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
        public object GdBinFileHelper { get; private set; }
        public TimingRange CurrentJobTimeRange { get; set; }
        public DominatorCancellationTokenSource JobCancellationTokenSource { get; set; }
        public static Dictionary<string, string> DictRunningJobs = new Dictionary<string, string>();
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionCampaign { get; set; }
        protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionAccount { get; set; } 
        #endregion

        public JobProcess(DominatorAccountModel dominatorAccountModel, JobConfiguration JobConfiguration, ActivityType activityType, TimingRange CurrentJobTimeRange)
        {
            this.DominatorAccountModel = dominatorAccountModel;
            this.JobConfiguration = JobConfiguration;
            this.ActivityType = activityType;
            this.CurrentJobTimeRange = CurrentJobTimeRange;
        }


        public JobProcess(string account, string template, ActivityType activityType, TimingRange CurrentJobTimeRange)
        {
            this.DominatorAccountModel = BinFileHelper.GetBinFileDetails<DominatorAccountModel>().FirstOrDefault(x => x.AccountBaseModel.UserName == account);
            this.CurrentJobTimeRange = CurrentJobTimeRange;
            TemplateModel model = BinFileHelper.GetBinFileDetails<TemplateModel>().FirstOrDefault(x => x.Id == template);
            this.JobConfiguration = Newtonsoft.Json.JsonConvert.DeserializeObject<JobConfiguration>(model.ActivitySettings);
            var cc = BinFileHelper.GetBinFileDetails<CampaignDetails>();
            this.TemplateId = template;
            this.campaignId = BinFileHelper.GetBinFileDetails<CampaignDetails>().FirstOrDefault(x => x.TemplateId == this.TemplateId)?.CampaignId;
            this.ActivityType = activityType;
            JobCancellationTokenSource = new DominatorCancellationTokenSource(account, template);
            InitializeActivityCount(account);
        }

        protected void InitializeActivityCount(string account)
        {
            MaxNoOfActionPerJob = this.JobConfiguration.JobsActivityCount.GetRandom();
            MaxNoOfActionPerHour = JobConfiguration.HoursActivityCount.GetRandom();
            MaxNoOfActionPerDay = JobConfiguration.DaysActivityCount.GetRandom();
            MaxNoOfActionPerWeek = JobConfiguration.WeeksActivityCount.GetRandom();
            InitializeDatabseConnection();
        }
        private void InitializeDatabseConnection()
        {
            DataBaseConnectionCampaign = DataBaseHandler.GetDataBaseConnectionInstance(campaignId, DatabaseType.CampaignType);
            DataBaseConnectionAccount = DataBaseHandler.GetDataBaseConnectionInstance(DominatorAccountModel.AccountBaseModel.UserName, DatabaseType.AccountType);
        }



        public JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult)
        {
            JobProcessResult jobProcessResult = PostScrapeProcess(ScrapedResult);
            jobProcessResult.IsProcessCompleted = checkJobProcessCompleted();
            if (jobProcessResult.IsProcessCompleted)
            {
                StartOtherConfiguration(ScrapedResult);
                GlobusLogHelper.log.Info("Process completed with account => " + DominatorAccountModel.AccountBaseModel.UserName + " module => " + ActivityType.ToString());
            }
            return jobProcessResult;
        }

        private bool checkJobProcessCompleted()
        {

            if (NoOfActionPerformedCurrentJob > MaxNoOfActionPerJob)
            {
                ScheduleNextJob(DateTime.Now.AddTicks(this.JobConfiguration.JobsDelay.GetRandom()));
                return true;
            }
            int currentTime = DateTimeUtilities.GetEpochTime();
            NoOfActionPerformedCurrentHour = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600).Count();
            if (NoOfActionPerformedCurrentHour > MaxNoOfActionPerHour)
            {

                ScheduleNextJob(DateTime.Now.AddMinutes(this.JobConfiguration.JobsDelay.GetRandom()));
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

        private void ScheduleNextJob(DateTime dateTime)
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
                        ActivityDeserialize.GdScheduler(DominatorAccountModel.AccountBaseModel.UserName, TemplateId, CurrentJobTimeRange,
                            ActivityType.Follow.ToString());
                    }, s => s.WithName($"{ActivityType.Follow.ToString()}-{this.TemplateId}").ToRunOnceAt(dateTime));
            }

        }


        public abstract JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult);


        public abstract void StartOtherConfiguration(ScrapeResultNew scrapeResult);

        public bool checkIfJobCompleted()
        {
            return false;
        }


        protected void StopFollow()
        {
            TaskAndThreadUtility.StopTask(this.DominatorAccountModel.AccountBaseModel.UserName, TemplateId);
            List<TemplateModel> lstTemplateModel = BinFileHelper.GetTemplateDetails();
            lstTemplateModel.ForEach(template =>
            {
                if (template.Id == TemplateId)
                {
                    JsonConvert.DeserializeObject<JobConfiguration>(template.ActivitySettings).RunningTime.Clear();
                }
            });
            BinFileHelper.UpdateBinFile(lstTemplateModel);

        }



    }
}
