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

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public partial class DominatorScheduler
    {
        static IJobProcessFactory _activeJobProcessFactory => DominatorHouseInitializer.ActiveLibrary.JobProcessFactory;

        public static object _runStopActivityLocker = new object();

        /// <summary>
        /// Runs activity for specific 'module' and social network. 
        /// </summary>
        /// <typeparam name="T">type of job like GramDominator.FollowProcess</typeparam>
        /// <param name="account"></param>
        /// <param name="templateId"></param>
        /// <param name="CurrentJobTimeRange"></param>
        /// <param name="module">Follow, Comment, etc.</param>
        public static void RunActivity(string account, string templateId, TimingRange CurrentJobTimeRange, string module)
        {
            var id = JobProcess.AsId(account, templateId);

            Schedule ScheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == id);
            if (ScheduledJob != null && ScheduledJob.Disabled)
                return;

            // jobProcess may be Follow, UnFollowProcess, Like, Comment, Repost, for any particular social network.
            // jobProcessFactory have to be registered for each library.
            var jobProcess = _activeJobProcessFactory.Create(account, templateId, CurrentJobTimeRange, module);
            
            jobProcess.StartProcessAsync();
        }


        public static void StopActivity(string accountName, string module, string templateId)
        {
            lock (_runStopActivityLocker)
            {                
                JobProcess.Stop(accountName, templateId);

                var id = JobProcess.AsId(accountName, templateId);
                JobManager.RemoveJob(id);
                Schedule ScheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == id);

                try
                {
                    ScheduledJob.Disable();

                    if (ScheduledJob.Disabled)
                    {
                        GlobusLogHelper.log.Info($"{module}-{templateId}" + " stopped");
                        return;
                    }
                }
                catch (Exception )
                {
                    
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
            if (JobProcess.IsStarted(dominatorAccount.UserName, moduleConfiguration.TemplateId))
            {
                GlobusLogHelper.log.Error($"Job {moduleConfiguration.TemplateId} already started for {dominatorAccount.UserName}");
                return;
            }

            try
            {
                // Check that at least one timing was set up before creating campaign
                if (dominatorAccount.ActivityManager.RunningTime == null ||
                    dominatorAccount.ActivityManager.RunningTime.All(rt => rt.Timings.Count == 0))
                    throw new InvalidOperationException($"Running time for activity {activityType} wasn't set");

                var today = DateTimeUtilities.GetDayOfWeek();

                // retrieve the account's todays scheduled modules.
                // TODO: check not only first but all running times                
                var timeScheduleModel = dominatorAccount.ActivityManager.RunningTime.First(x => x.DayOfWeek == today);

                if (!timeScheduleModel.IsEnabled)
                {
                    GlobusLogHelper.log.Debug($"Activity {activityType} is disabled");
                    return;
                }

                // get the hour and minute of current time
                var currentTimespan = DateTimeUtilities.GetTimeSpanCurrentHourMinute();

                // Schedule jobs of specific module for each time range
                foreach (var timing in timeScheduleModel.Timings)
                {
                    // get the template id for respective module
                    string templateId = GetTemplateId(timing, dominatorAccount);
                    var jobId = JobProcess.AsId(dominatorAccount.UserName, templateId);

                    // If start time not met before,it will schedule to start time
                    if (timing.StartTime.Hours >= currentTimespan.Hours && timing.StartTime.Minutes > currentTimespan.Minutes)
                    {
                        JobManager.AddJob(() =>
                        {
                            RunActivity(dominatorAccount.AccountBaseModel.UserName, templateId, timing, timing.Module);

                        }, s => s.WithName(jobId).ToRunOnceAt(timing.StartTime.Hours, timing.StartTime.Minutes));

                        JobManager.AddJob(() =>
                        {
                            StopActivity(dominatorAccount.AccountBaseModel.AccountId, timing.Module, templateId);

                        }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));
                    }

                    // If start time already crossed of the day and end time is not crossed, then it will start after 5 seconds
                    else if (timing.EndTime.Hours >= currentTimespan.Hours && timing.EndTime.Minutes > currentTimespan.Minutes)
                    {
                        JobManager.AddJob(() =>
                        {
                            RunActivity(dominatorAccount.AccountBaseModel.UserName, templateId, timing, timing.Module);

                        }, s => s.WithName(jobId).ToRunOnceAt(DateTime.Now.AddSeconds(5)));

                        JobManager.AddJob(() =>
                        {
                            StopActivity(dominatorAccount.AccountBaseModel.AccountId, timing.Module, templateId);

                        }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));
                    }
                };

            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Error(ex.Message);
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
                        if (moduleRunningTimes.Count() > 0)
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
            var activitySetting = string.Empty;
            var runningTime = new List<RunningTimes>();
            try
            {
                var moduleConfiguration = item.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == moduleType);
                activitySetting = BinFileHelper.GetTemplateDetails().FirstOrDefault(x => x.Id == moduleConfiguration.TemplateId).ActivitySettings;

                dynamic obj = JsonConvert.DeserializeObject(activitySetting);
                runningTime = obj.JobConfiguration.RunningTime;

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
    }
}