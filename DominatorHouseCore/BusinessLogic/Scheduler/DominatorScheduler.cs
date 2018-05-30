using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Navigation;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Process;
using DominatorHouseCore.Interfaces;
using Newtonsoft.Json;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.FileManagers;
using MahApps.Metro.Controls.Dialogs;

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


        public static void StopActivity(string accountId, string module, string templateId)
        {
            lock (RunStopActivityLocker)
            {
                JobProcess.Stop(accountId, templateId);

                var id = JobProcess.AsId(accountId, templateId);
                JobManager.RemoveJob(id);
                var scheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == id);

                try
                {
                    if (scheduledJob == null) return;
                    scheduledJob.Disable();

                    if (!scheduledJob.Disabled) return;
                    GlobusLogHelper.log.Info($"{module}-{templateId}" + " stopped");
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
        public static void ScheduleTodayJobs(DominatorAccountModel dominatorAccount, SocialNetworks netowork, ActivityType activityType)
        {
            var moduleConfiguration = dominatorAccount.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);
            if (moduleConfiguration != null && !moduleConfiguration.IsEnabled)
                return;

            // Check if activity with the same id already running
            if (JobProcess.IsStarted(dominatorAccount.AccountId, moduleConfiguration.TemplateId))
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

                    if (TimeBetween(now, timing.StartTime, timing.EndTime))
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
                GlobusLogHelper.log.Error(ex.Message);
            }
        }

        private static bool TimeBetween(TimeSpan now, TimeSpan start, TimeSpan end)
        {
            if (start < end)
                if (now <= end || start > now)
                    return true;
            return false;
        }

        private static void ScheduleJob(DominatorAccountModel dominatorAccount, TimingRange timing, string templateId, string jobId, bool isDelayed)
        {
            if (isDelayed)
            {
                JobManager.AddJob(() =>
                {
                    RunActivity(dominatorAccount, templateId, timing, timing.Module);
                }, s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddSeconds(5)));

                JobManager.AddJob(() =>
                {
                    StopActivity(dominatorAccount.AccountBaseModel.AccountId, timing.Module, templateId);
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
                    StopActivity(dominatorAccount.AccountBaseModel.AccountId, timing.Module, templateId);

                }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));

            }
        }


        /// <summary>
        /// ScheduleForEachModule take two argument first is Module type  and second is an object of AccountModel
        /// it will schedule job for all module having running time except given moduleType if moduleType is 
        /// null it will schedule for all module having running time
        /// </summary>
        /// <param name="moduleToIgnore"></param>
        /// <param name="account"></param>
        public static void ScheduleForEachModule(ActivityType? moduleToIgnore, DominatorAccountModel account, SocialNetworks network)
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
                GlobusLogHelper.log.Error(ex.ToString());
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
                var moduleConfiguration = item.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == moduleType);
                if (moduleConfiguration != null)
                {
                    var activitySetting = BinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == moduleConfiguration.TemplateId)?.ActivitySettings;

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

                var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);

                if (moduleConfiguration == null)
                    return false;

                var accountstemplateId = moduleConfiguration.TemplateId;
                if (accountstemplateId == null || moduleConfiguration.LstRunningTimes==null)
                {
                    return false;
                }
                if (isStart)
                {
                    moduleConfiguration.IsEnabled = true;
                    ScheduleTodayJobs(accountModel, accountModel.AccountBaseModel.AccountNetwork, activityType);
                }
                else
                {
                    moduleConfiguration.IsEnabled = false;
                    StopActivity(accountModel.AccountBaseModel.AccountId,
                        activityType.ToString(), accountstemplateId);                 
                }

                AccountsFileManager.Edit(accountModel);

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

    }
}