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
    //public abstract class JobProcess
    //{
    //    protected int NoOfActionPerformedCurrentJob = 0;
    //    protected int MaxNoOfActionPerJob = 0;
    //    protected int NoOfActionPerformedCurrentHour = 0;
    //    protected int MaxNoOfActionPerHour = 0;
    //    protected int NoOfActionPerformedCurrentDay = 0;
    //    protected int MaxNoOfActionPerDay = 0;
    //    protected int MaxNoOfActionPerWeek = 0;
    //    protected int NoOfActionPerformedCurrentWeek = 0;
    //    public DominatorCancellationTokenSource JobCancellationTokenSource { get; set; }

    //    public string TemplateId { get; set; }
    //    public string campaignId { get; set; }

    //    public TimingRange CurrentJobTimeRange { get; set; }

    //    public static Dictionary<string, string> DictRunningJobs = new Dictionary<string, string>();


    //    protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionCampaign { get; set; }
    //    protected DataBaseConnectionCodeFirst.DataBaseConnection DataBaseConnectionAccount { get; set; }

    //    public AccountModel AccountModel { get; set; }
    //    public ModuleSetting ModuleSetting { get; set; }
    //    public ActivityType ActivityType { get; set; }
    //    public object GdBinFileHelper { get; private set; }

    //    public JobProcess(AccountModel AccountModel, ModuleSetting ModuleSetting, ActivityType activityType, TimingRange CurrentJobTimeRange)
    //    {
    //        this.AccountModel = AccountModel;
    //        this.ModuleSetting = ModuleSetting;
    //        this.ActivityType = activityType;
    //        this.CurrentJobTimeRange = CurrentJobTimeRange;
    //    }


    //    public JobProcess(string account, string template, ActivityType activityType, TimingRange CurrentJobTimeRange)
    //    {
    //        this.AccountModel = AccountManagerViewModel.GetAccountManagerViewModel().LstAccountModel.FirstOrDefault(x => x.UserName == account);
    //        this.CurrentJobTimeRange = CurrentJobTimeRange;
    //        TemplateModel model = GdBinFileHelper.GetBinFileDetails<TemplateModel>().FirstOrDefault(x => x.Id == template);
    //        this.ModuleSetting = Newtonsoft.Json.JsonConvert.DeserializeObject<FollowerModel>(model.ActivitySettings);
    //        var cc = GdBinFileHelper.GetBinFileDetails<CampaignDetails>();
    //        this.TemplateId = template;
    //        this.campaignId = GdBinFileHelper.GetBinFileDetails<CampaignDetails>().FirstOrDefault(x => x.TemplateId == this.TemplateId)?.CampaignId;
    //        this.ActivityType = activityType;
    //        JobCancellationTokenSource = new DominatorCancellationTokenSource(account, template);
    //        InitializeActivityCount(account);
    //    }

    //    private void InitializeActivityCount(string account)
    //    {
    //        MaxNoOfActionPerJob = ModuleSetting.JobConfiguration.JobsActivityCount.GetRandom();
    //        MaxNoOfActionPerHour = ModuleSetting.JobConfiguration.HoursActivityCount.GetRandom();
    //        MaxNoOfActionPerDay = ModuleSetting.JobConfiguration.DaysActivityCount.GetRandom();
    //        MaxNoOfActionPerWeek = ModuleSetting.JobConfiguration.WeeksActivityCount.GetRandom();
    //        InitializeDatabseConnection();
    //    }
    //    private void InitializeDatabseConnection()
    //    {
    //        DataBaseConnectionCampaign = DataBaseHandler.GetDataBaseConnectionInstance(campaignId, DatabaseType.CampaignType);
    //        DataBaseConnectionAccount = DataBaseHandler.GetDataBaseConnectionInstance(AccountModel.UserName, DatabaseType.AccountType);
    //    }

    //    public void StartProcess()
    //    {

    //        try
    //        {
    //            DictRunningJobs.Add(this.TemplateId, "");
    //            if (string.IsNullOrEmpty(this.campaignId))
    //                return;

    //            GlobusLogHelper.log.Info("Process started with account => " + AccountModel.UserName + " module => " + ActivityType.ToString());
    //            if (!this.AccountModel.IsUserLoggedIn)
    //            {
    //                GlobusLogHelper.log.Info("Logging in with account => " + AccountModel.UserName + " module => " + ActivityType.ToString());
    //                LogInProcess logInProcess = new LogInProcess();
    //                logInProcess.LoginWithDataBaseCookies(this.AccountModel, true);
    //            }

    //            if (this.AccountModel.IsUserLoggedIn)
    //            {
    //                GlobusLogHelper.log.Info("Logged in successfully with account => " + AccountModel.UserName + " module => " + ActivityType.ToString());
    //                InstagramScraper instragramscraper = new InstagramScraper(this);
    //                instragramscraper.ScrapeWithQuery();
    //            }
    //            try
    //            {
    //                DictRunningJobs.Remove(this.TemplateId);
    //            }
    //            catch (Exception)
    //            {
    //            }
    //        }
    //        catch (Exception Ex)
    //        {
    //        }
    //        finally
    //        {
    //        }
    //    }


    //    public JobProcessResult FinalProcess(ScrapeResultNew ScrapedResult)
    //    {
    //        JobProcessResult jobProcessResult = PostScrapeProcess(ScrapedResult);
    //        jobProcessResult.IsProcessCompleted = checkJobProcessCompleted();
    //        if (jobProcessResult.IsProcessCompleted)
    //        {
    //            StartOtherConfiguration(ScrapedResult);
    //            GlobusLogHelper.log.Info("Process completed with account => " + AccountModel.UserName + " module => " + ActivityType.ToString());
    //        }
    //        return jobProcessResult;
    //    }

    //    private bool checkJobProcessCompleted()
    //    {

    //        if (NoOfActionPerformedCurrentJob > MaxNoOfActionPerJob)
    //        {
    //            ScheduleNextJob(DateTime.Now.AddTicks(ModuleSetting.JobConfiguration.JobsDelay.GetRandom()));
    //            return true;
    //        }
    //        int currentTime = DateTimeUtilities.GetEpochTime();
    //        NoOfActionPerformedCurrentHour = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600).Count();
    //        if (NoOfActionPerformedCurrentHour > MaxNoOfActionPerHour)
    //        {

    //            ScheduleNextJob(DateTime.Now.AddMinutes(ModuleSetting.JobConfiguration.JobsDelay.GetRandom()));
    //            return true;
    //        }

    //        NoOfActionPerformedCurrentDay = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24).Count();
    //        if (NoOfActionPerformedCurrentDay > MaxNoOfActionPerDay)
    //        {
    //            TaskAndThreadUtility.StopTask(this.AccountModel.UserName, this.TemplateId);
    //            return true;
    //        }

    //        NoOfActionPerformedCurrentWeek = DataBaseConnectionCampaign.Get<InteractedUsers>(x => (currentTime - x.Date) <= 3600 * 24 * 7).Count();
    //        if (NoOfActionPerformedCurrentWeek > MaxNoOfActionPerWeek)
    //        {
    //            TaskAndThreadUtility.StopTask(this.AccountModel.UserName, this.TemplateId);
    //            return true;
    //        }

    //        return false;
    //    }

    //    private void ScheduleNextJob(DateTime dateTime)
    //    {
    //        TaskAndThreadUtility.StopTask(this.AccountModel.UserName, this.TemplateId);

    //        List<RunningTimes> lstTimings = ModuleSetting.JobConfiguration.RunningTime;

    //        var today = DateTimeUtilities.GetDayOfWeek();
    //        var timeScheduleModel = ModuleSetting.JobConfiguration.RunningTime.First((x => x.DayOfWeek == today));

    //        if (!timeScheduleModel.IsEnabled)
    //            return;

    //        // get the hour and minute of current time
    //        var nextJobTimeSpan = DateTimeUtilities.GetTimeSpanForGivenTime(dateTime);//GetTimeSpanForGivenTime


    //        JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{ActivityType.Follow.ToString()}-{this.TemplateId}");

    //        if (CurrentJobTimeRange.EndTime >= nextJobTimeSpan && nextJobTimeSpan > CurrentJobTimeRange.StartTime)
    //        {
    //            var TemplateId = AccountModel.ActivityManager.LstModuleConfiguration
    //                 .FirstOrDefault(x => x.ActivityType == ActivityType.Follow).TemplateId;
    //            JobManager.AddJob(
    //                () =>
    //                {
    //                    ActivityDeserialize.GdScheduler(AccountModel.UserName, TemplateId, CurrentJobTimeRange,
    //                        ActivityType.Follow.ToString());
    //                }, s => s.WithName($"{ActivityType.Follow.ToString()}-{this.TemplateId}").ToRunOnceAt(dateTime));
    //        }

    //    }


    //    public abstract JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult);


    //    public abstract void StartOtherConfiguration(ScrapeResultNew scrapeResult);

    //    public bool checkIfJobCompleted()
    //    {
    //        return false;
    //    }


    //    protected void StopFollow()
    //    {
    //        TaskAndThreadUtility.StopTask(this.AccountModel.UserName, TemplateId);
    //        List<TemplateModel> lstTemplateModel = GdBinFileHelper.GetTemplateDetails();
    //        lstTemplateModel.ForEach(template =>
    //        {
    //            if (template.Id == TemplateId)
    //            {
    //                JsonConvert.DeserializeObject<FollowerModel>(template.ActivitySettings).JobConfiguration.RunningTime.Clear();
    //            }
    //        });
    //        GdBinFileHelper.UpdateBinFile(lstTemplateModel);

    //    }



    //}
}
