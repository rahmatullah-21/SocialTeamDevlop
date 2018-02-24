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
using DominatorHouseCore.BusinessLogic;
using DominatorHouseCore.DatabaseHandler;
using DominatorHouseCore.FileManagers;

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
        public object GdBinFileHelper { get; private set; }
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

            this.TemplateId = template;
            this.campaignId = CampaignsFileManager.Get().FirstOrDefault(x => x.TemplateId == this.TemplateId)?.CampaignId;
            this.ActivityType = activityType;
            JobCancellationTokenSource = new DominatorCancellationTokenSource(account, template);
            InitializeActivityCount(account);
        }

        public abstract JobProcess Initialize(string account, string template, ActivityType activity, TimingRange currentJobTimeRange);

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


        public abstract JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult);

        public abstract void StartProcess();

        public abstract void StartOtherConfiguration(ScrapeResultNew scrapeResult);

        public bool checkIfJobCompleted()
        {
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



    }
}
