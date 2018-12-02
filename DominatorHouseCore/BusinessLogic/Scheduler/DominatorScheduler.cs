using CommonServiceLocator;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FluentScheduler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public partial class DominatorScheduler
    {

        private static IJobProcessFactory _activeJobProcessFactory;

        public static object RunStopActivityLocker = new object();

        /// <summary>
        /// To start the activity of template for the given account at specified time range
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateId"></param>
        /// <param name="currentJobTimeRange"></param>
        /// <param name="module"></param>
        public static void RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange, string module)
        {
            try
            {
                _activeJobProcessFactory = SocinatorInitialize.GetSocialLibrary(account.AccountBaseModel.AccountNetwork).GetNetworkCoreFactory().JobProcessFactory;

                var id = JobProcess.AsId(account.AccountBaseModel.AccountId, templateId);

                var scheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == id);

                if (scheduledJob != null && scheduledJob.Disabled)
                    return;

                var jobProcess = _activeJobProcessFactory.Create(account.AccountBaseModel.UserName, templateId, currentJobTimeRange, module, account.AccountBaseModel.AccountNetwork);

                jobProcess.StartProcessAsync();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        public static void StopActivity(DominatorAccountModel account, string module, string templateId, bool needRestart)
        {
            lock (RunStopActivityLocker)
            {
                JobProcess.Stop(account.AccountId, templateId);

                // GlobusLogHelper.log.Info(Log.ProcessStopped, account.AccountBaseModel.AccountNetwork, account.AccountBaseModel.UserName, module, $"{module}-{templateId}" + " stopped");

                var id = JobProcess.AsId(account.AccountId, templateId);
                JobManager.RemoveJob(id);
                var scheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == id);

                try
                {
                    if (scheduledJob == null)
                    {
                        if (needRestart)
                            ScheduleNextActivity(account, (ActivityType)Enum.Parse(typeof(ActivityType), module));
                        return;
                    }
                    scheduledJob.Disable();

                    if (!scheduledJob.Disabled)
                    {
                        if (needRestart)
                            ScheduleNextActivity(account, (ActivityType)Enum.Parse(typeof(ActivityType), module));
                        return;
                    }


                    // GlobusLogHelper.log.Info($"{module}-{templateId}" + " stopped");
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }

            }
        }


        /// <summary>

        /// Schedules for today or run at once specific activity for certain social network
        /// </summary>
        /// <param name="dominatorAccount"></param>
        /// <param name="netowork"></param>
        /// <param name="activityType"></param>
        internal static void ScheduleTodayJobs(DominatorAccountModel dominatorAccount, SocialNetworks netowork, ActivityType activityType)
        {
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var runningJobsHolder = ServiceLocator.Current.GetInstance<IRunningJobsHolder>();
            var moduleConfiguration = jobActivityConfigurationManager[dominatorAccount.AccountId, activityType];
            if (moduleConfiguration != null && !moduleConfiguration.IsEnabled)
                return;

            // Check if activity with the same id already running
            if (runningJobsHolder.IsRunning(new JobKey(dominatorAccount.AccountId, moduleConfiguration.TemplateId)))
            {
                GlobusLogHelper.log.Debug($"Job {moduleConfiguration.TemplateId} already started for {dominatorAccount.UserName}");
                return;
            }

            try
            {
                // Check that at least one timing was set up before creating campaign
                if (moduleConfiguration.LstRunningTimes == null ||
                    moduleConfiguration.LstRunningTimes.All(rt => rt.Timings.Count == 0))
                {
                    throw new InvalidOperationException($"Running time for activity {activityType} wasn't set");
                }


                var today = DateTimeUtilities.GetDayOfWeek();

                // retrieve the account's todays scheduled modules.
                // TODO: check not only first but all running times                
                var timeScheduleModel = moduleConfiguration.LstRunningTimes.First(x => x.DayOfWeek == today);

                if (!timeScheduleModel.IsEnabled)
                {
                    GlobusLogHelper.log.Debug($"Activity {activityType} is disabled");
                    return;
                }


                // Schedule jobs of specific module for each time range
                foreach (var timing in timeScheduleModel.Timings)
                {
                    // get the template id for respective module
                    var templateId = GetTemplateId(timing, dominatorAccount);

                    var jobId = JobProcess.AsId(dominatorAccount.AccountId, templateId);

                    var now = DateTime.Now.TimeOfDay;

                    if (DateTimeUtilities.TimeBetween(now, timing.StartTime, timing.EndTime))
                    {
                        ScheduleJob(dominatorAccount, timing, templateId, jobId, isDelayed: now > timing.StartTime);
                    }
                    //// If start time not met before,it will schedule to start time
                    //if (timing.StartTime.Hours >= currentTimespan.Hours && timing.StartTime.Minutes > currentTimespan.Minutes)
                    //{
                    //    ScheduleJob(dominatorAccount, timing, templateId, jobId, isDelayed: false);
                    //}

                    //// If start time already crossed of the day and end time is not crossed, then it will start after 5 seconds
                    //else if (timing.EndTime.Hours >= currentTimespan.Hours && timing.EndTime.Minutes > currentTimespan.Minutes)
                    //{
                    //    ScheduleJob(dominatorAccount, timing, templateId, jobId, isDelayed: true);
                    //}
                }
                ;
            }
            catch (InvalidOperationException)
            {
                ChangeAccountsRunningStatus(false, dominatorAccount.AccountId, activityType);
                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                    dominatorAccount.UserName,
                    $"Error:- {activityType} activity is not configured properly for this account. Please make sure you have added enough queries and updated time when activity has to be performed and clicked on save button.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private static void ScheduleJob(DominatorAccountModel dominatorAccount, TimingRange timing, string templateId, string jobId, bool isDelayed)
        {
            Task.Factory.StartNew(() =>
            {
                if (isDelayed)
                {
                    JobManager.AddJob(() =>
                    {
                        RunActivity(dominatorAccount, templateId, timing, timing.Module);
                    }, s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddSeconds(5)));

                    JobManager.AddJob(() =>
                    {
                        StopActivity(dominatorAccount, timing.Module, templateId, true);
                    }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));
                }
                else
                {
                    JobManager.AddJob(() =>
                    {
                        RunActivity(dominatorAccount, templateId, timing, timing.Module);

                    }, s => s.WithName(jobId).ToRunOnceAt(timing.StartTime.Hours, timing.StartTime.Minutes));

                    JobManager.AddJob(() =>
                    {
                        StopActivity(dominatorAccount, timing.Module, templateId, true);

                    }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));

                }
            });
        }

        /// <summary>
        /// ScheduleForEachModule take two argument first is Module type  and second is an object of AccountModel
        /// it will schedule job for all module having running time except given moduleType if moduleType is 
        /// null it will schedule for all module having running time
        /// </summary>
        /// <param name="moduleToIgnore"></param>
        /// <param name="account"></param>
        internal static void ScheduleForEachModule(ActivityType? moduleToIgnore, DominatorAccountModel account, SocialNetworks network)
        {
            try
            {
                foreach (ActivityType activity in Enum.GetValues(typeof(ActivityType)))
                {
                    if (activity != moduleToIgnore)
                    {
                        var moduleRunningTimes = GetRunningTimes(account, activity);
                        if (moduleRunningTimes.Count > 0)
                        {
                            account.ActivityManager.RunningTime = moduleRunningTimes;
                            foreach (var timing in account.ActivityManager.RunningTime)
                                foreach (var timingRange in timing.Timings)
                                    timingRange.Module = activity.ToString();

                            DominatorScheduler.ScheduleTodayJobs(account, network, activity);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }


        /// <summary>
        /// GetRunningTimes take two argument first is an object of AccountModel and second is Module type
        /// it will return running time list according to Module type
        /// </summary>
        /// <param name="item"></param>
        /// <param name="moduleType"></param>
        /// <returns></returns>
        public static List<RunningTimes> GetRunningTimes(DominatorAccountModel item, ActivityType moduleType)
        {
            var runningTime = new List<RunningTimes>();
            try
            {
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var moduleConfiguration = jobActivityConfigurationManager[item.AccountId, moduleType];
                if (moduleConfiguration != null)
                {
                    var activitySetting = ServiceLocator.Current.GetInstance<ITemplatesCacheService>().GetTemplateModels()
                        .FirstOrDefault(x => x.Id == moduleConfiguration.TemplateId)?.ActivitySettings;

                    dynamic obj = JsonConvert.DeserializeObject(activitySetting);
                    runningTime = obj.JobConfiguration.RunningTime;
                }


                #region Commented
                //switch (moduleType)
                //{
                //    case ActivityType.Follow:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.FollowModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<FollowerModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Unfollow:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.UnfollowModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<UnfollowerModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Like:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.LikeModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<LikeModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Comment:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.CommentModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<CommentModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.Repost:
                //        activitySetting = GdBinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == item.ActivityManager.RepostModule.TemplateId).ActivitySettings;
                //        runningTime = JsonConvert.DeserializeObject<RePosterModel>(activitySetting).JobConfiguration.RunningTime;
                //        break;
                //    case ActivityType.DownloadScraper:
                //        break;
                //    case ActivityType.UserScraper:
                //        break;
                //} 
                #endregion
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug(ex);
            }

            return runningTime;
        }

        public static bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime)
        {

            if ((firstRunningTime == null && secondRunningTime != null) || (secondRunningTime == null && firstRunningTime != null))
                return false;
            else if (firstRunningTime == null && secondRunningTime == null)
                return true;

            if (firstRunningTime.Count != secondRunningTime.Count)
                return false;

            bool IsEqual = true;
            foreach (RunningTimes item in firstRunningTime)
            {
                RunningTimes oldRunningTime = item;
                RunningTimes newRunningTime = secondRunningTime.ElementAt(firstRunningTime.IndexOf(item));


                if ((oldRunningTime == null && newRunningTime != null) || (newRunningTime == null && oldRunningTime != null))
                {
                    IsEqual = false;
                    break;
                }
                if (oldRunningTime.Timings.Count != newRunningTime.Timings.Count)
                    return false;
                foreach (var oldtime in oldRunningTime.Timings)
                {
                    if (!newRunningTime.Timings.Contains(oldtime))
                        return false;
                    IsEqual = true;
                }

            }

            return IsEqual;
        }

        public static bool ChangeAccountsRunningStatus(bool isStart, string accountId, ActivityType activityType)
        {
            try
            {

                var accountModel = AccountsFileManager.GetAccountById(accountId);

                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
                var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId, activityType];

                var accountstemplateId = moduleConfiguration?.TemplateId;
                if (accountstemplateId == null || moduleConfiguration.LstRunningTimes == null)
                {
                    return false;
                }
                if (isStart)
                {
                    try
                    {
                        var campaignStatus = campaignFileManager
                            .FirstOrDefault(x => x.TemplateId == moduleConfiguration.TemplateId)
                            ?.Status;
                        if (campaignStatus == "Paused" && moduleConfiguration.IsEnabled)
                        {
                            Dialog.ShowDialog("Error",
                                $"This account belongs to campaign configuration, which is paused state. Please make the campaign active before changing activity status for this account.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    moduleConfiguration.IsEnabled = true;
                    DominatorScheduler.ScheduleNextActivity(accountModel, activityType);
                }
                else
                {
                    moduleConfiguration.IsEnabled = false;
                    StopActivity(accountModel,
                        activityType.ToString(), accountstemplateId, false);
                    //DominatorScheduler.ScheduleNextActivity(accountModel, activityType);
                }

                var socinatorAccountBuilder = new SocinatorAccountBuilder(accountModel.AccountBaseModel.AccountId)
                                                .AddOrUpdateModuleSettings(activityType, moduleConfiguration)
                                                .SaveToBinFile();

                //AccountsFileManager.Edit(accountModel);

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }
        /// <summary>
        /// This method can be used in cases like Enable AutoFollow/Unfollow. You need to pass the Activity type which has to be disabled as first parameter and ActivityType which has to be enabled as second parameter, and accountID for which the activities has to be updated.
        /// </summary>
        /// <param name="stopActivity">ActivityType which has to be disabled</param>
        /// <param name="startActivity">ActivityType which has to be enabled</param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public static bool EnableDisableModules(ActivityType stopActivity, ActivityType startActivity, string accountId)
        {
            try
            {
                if (ChangeAccountsRunningStatus(false, accountId, stopActivity))
                    if (ChangeAccountsRunningStatus(true, accountId, startActivity))
                        return true;
                    else
                    {

                        throw new InvalidOperationException($"Error Code : 1001 Cant able start the activity {startActivity} with account {accountId}");
                    }

                else
                    throw new InvalidOperationException($"Error Code : 1002  Cant able stop the activity {stopActivity} with account {accountId}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public static void ScheduleEachActivity(DominatorAccountModel account)
        {
            try
            {
                var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
                foreach (var moduleConfiguration in jobActivityConfigurationManager[account.AccountId])
                {
                    ScheduleNextActivity(account, moduleConfiguration.ActivityType);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void ScheduleActivityForNextJob(DominatorAccountModel dominatorAccount, SocialNetworks netowork,
            ActivityType activityType)
        {
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var runningJobsHolder = ServiceLocator.Current.GetInstance<IRunningJobsHolder>();
            var moduleConfiguration = jobActivityConfigurationManager[dominatorAccount.AccountId, activityType];
            if (moduleConfiguration == null || !moduleConfiguration.IsEnabled)
                return;

            // Check if activity with the same id already running
            if (runningJobsHolder.IsRunning(new JobKey(dominatorAccount.AccountId, moduleConfiguration.TemplateId)))
            {
                GlobusLogHelper.log.Info($"Job {moduleConfiguration.TemplateId} already started for {dominatorAccount.UserName}");
                return;
            }
            try
            {
                //Get the time when the activity has to be performed next and stopped.
                var timeToRunNext = DateTimeUtilities.GetNextStartTime(moduleConfiguration, ReachedLimitType.Job);
                if (timeToRunNext == DateTime.MinValue)
                {
                    GlobusLogHelper.log.Info($"suspicious calculation {timeToRunNext} - {DateTime.Now.AddMinutes(-1)}");
                    return;
                }
                if (timeToRunNext < moduleConfiguration.NextRun)
                {
                    timeToRunNext = moduleConfiguration.NextRun;
                }
                var stopTime = timeToRunNext.Date;
                var templateId = moduleConfiguration.TemplateId;
                var jobId = JobProcess.AsId(dominatorAccount.AccountId, templateId);
                var runningTimes = moduleConfiguration.LstRunningTimes[(int)timeToRunNext.DayOfWeek];
                var timings = runningTimes.Timings.ToList();
                timings.Sort(new RunningTimeComparer());
                var time = new TimingRange(TimeSpan.MinValue, TimeSpan.MaxValue);
                time.Module = activityType.ToString();

                foreach (var timing in timings)
                {
                    if (timing.StartTime <= timeToRunNext.TimeOfDay && timeToRunNext.TimeOfDay < timing.EndTime)
                    {
                        stopTime = stopTime.Date.Add(timing.EndTime);
                        time = timing;
                        break;
                    }
                }

                UpdatedScheduleJob(dominatorAccount, time, templateId, jobId, timeToRunNext, stopTime);
                GlobusLogHelper.log.Info(Log.NextJobExpectedToStartBy,
                                     dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.AccountBaseModel.UserName,
                                     activityType, timeToRunNext);

            }
            catch (InvalidOperationException)
            {
                ChangeAccountsRunningStatus(false, dominatorAccount.AccountId, activityType);
                GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork,
                    dominatorAccount.UserName,
                    $"Error:- {activityType} activity is not configured properly for this account. Please make sure you have added enough queries and updated time when activity has to be performed and clicked on save button.");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void ScheduleNextActivity(DominatorAccountModel dominatorAccountModel, ActivityType activityType)
        {
            if (SoftwareSettingsFileManager.GetSoftwareSettings()?.IsEnableParallelActivitiesChecked ?? false)
            {
                ScheduleActivityForNextJob(dominatorAccountModel, dominatorAccountModel.AccountBaseModel.AccountNetwork, activityType);
            }
            else
            {
                var runningJobsHolder = ServiceLocator.Current.GetInstance<IRunningJobsHolder>();
                if (runningJobsHolder.IsActivityRunningForAccount(dominatorAccountModel.AccountId))
                    return;

                RunningActivityManager.StartNextRound(dominatorAccountModel);
            }

        }

        private static void UpdatedScheduleJob(DominatorAccountModel dominatorAccount, TimingRange timing, string templateId, string jobId, DateTime timeToRunNext, DateTime stopTime)
        {
            if (timeToRunNext < DateTime.Now)
            {
                timeToRunNext.AddSeconds(25);
            }
            Task.Factory.StartNew(() =>
            {
                JobManager.AddJob(() =>
                {
                    RunActivity(dominatorAccount, templateId, timing, timing.Module);
                }, s => s.WithName(jobId).ToRunOnceAt(timeToRunNext));
                JobManager.AddJob(() =>
                {
                    StopActivity(dominatorAccount, timing.Module, templateId, true);
                }, s => s.ToRunOnceAt(stopTime));
            });
        }
    }
}