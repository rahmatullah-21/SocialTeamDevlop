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

namespace DominatorHouseCore.BusinessLogic
{    
    public partial class DominatorScheduler
    {
        /// <summary>
        /// Loads all activities for particular social network and runs them
        /// </summary>
        public static void InitializeScheduler(IJobProcessFactory jobProcessFactory)
        {
            // TODO: load activities and create them through factory
        }


        /// <summary>
        /// Starts scheduler for specified 'module' . 
        /// </summary>
        /// <typeparam name="T">type of job like GramDominator.FollowProcess</typeparam>
        /// <param name="account"></param>
        /// <param name="template"></param>
        /// <param name="CurrentJobTimeRange"></param>
        /// <param name="module">Follow, Comment, etc.</param>
        public static void StartScheduler<T>(string account, string template, TimingRange CurrentJobTimeRange, string module)
            where T : JobProcess, new()
        {
            Schedule ScheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{module}-{template}");
            if (ScheduledJob != null && ScheduledJob.Disabled)
                return;

            var activity = (ActivityType)Enum.Parse(typeof(ActivityType), module);

            switch (activity)
            {
                // Call Follow Module
                case ActivityType.Follow:
                    
                    T followProcess = (T)(new T().Initialize(account, template, activity, CurrentJobTimeRange));
                    Task.Factory.StartNew(() =>
                    {
                        GlobusLogHelper.log.Info("process started with [ " + account + "]");
                        followProcess.StartProcess();
                       
                    }, followProcess.JobCancellationTokenSource.Token);
                    break;

                case ActivityType.Unfollow:
                    // Call Unfollow Module
                    break;

                case ActivityType.Like:
                    // Call Like Module
                    break;

                case ActivityType.Unlike:
                    // Call Unlike Module
                    break;

                case ActivityType.Comment:
                    // Call Comment Module
                    break;

                case ActivityType.DeleteComment:
                    // Call DeleteComment Module
                    break;
                case ActivityType.Post:
                    // Call Post Module
                    break;
                case ActivityType.Repost:
                    // Call Repost Module
                    break;
                case ActivityType.DeletePost:
                    // Call DeletePost Module
                    break;
                case ActivityType.Message:
                    // Call Message Module
                    break;
                case ActivityType.UserScraper:
                    // Call UserScraper Module
                    break;
                case ActivityType.DownloadScraper:
                    // Call DownloadScraper Module
                    break;

                default:
                    break;

            }
        }


        public static void StopScheduler(string accountId, string module, string tamplateId)
        {
            TaskAndThreadUtility.StopTask(accountId, tamplateId);

            JobManager.RemoveJob($"{module}-{tamplateId}");
            Schedule ScheduledJob = JobManager.RunningSchedules.FirstOrDefault(x => x.Name == $"{module}-{tamplateId}");

            try
            {
                ScheduledJob.Disable();

                if (ScheduledJob.Disabled)
                {
                    GlobusLogHelper.log.Info($"{module}-{tamplateId}" + " stopped");
                    return;
                }
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Info($"{module}-{tamplateId}" + " job not yet running");
            }

        }



        public static void ScheduleTodayJobs(DominatorAccountModel dominatorAccount , ActivityType? activityType = null)
        {
            var moduleConfiguration = dominatorAccount.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == activityType);
            if (moduleConfiguration!=null && !moduleConfiguration.IsEnabled)
                return;
            try
            {
                JobManager.RunningSchedules.ToList().ForEach(schedule =>
                {
                    if (schedule.Name == $"{activityType.ToString()}-{moduleConfiguration.TemplateId}")
                        return;
                });
            }
            catch (Exception ex)
            {
                GlobusLogHelper.log.Debug(ex, $"{nameof(ScheduleTodayJobs)} unexpected exception");
            }
            

            try
            {
                if (dominatorAccount.ActivityManager.RunningTime == null)
                    return;

                // get the current day
                var today = DateTimeUtilities.GetDayOfWeek();

                // retrieve the account's todays scheduled modules
           
                var timeScheduleModel = dominatorAccount.ActivityManager.RunningTime.First((x => x.DayOfWeek == today));

                if (!timeScheduleModel.IsEnabled)
                    return;

                // get the hour and minute of current time
                var currentTimespan = DateTimeUtilities.GetTimeSpanCurrentHourMinute();

                timeScheduleModel.Timings.ToList().ForEach(timing =>
                {
                    // get the template id for respective module
                    string templateId = GetTemplateId(timing, dominatorAccount);

                   
                    // If start time not met before,it will schedule to start time
                    if (timing.StartTime.Hours >= currentTimespan.Hours && timing.StartTime.Minutes > currentTimespan.Minutes)
                    {
                        JobManager.AddJob(() =>
                        {
                            var activityDeserialize = new ActivityDeserialize(timing.Module, templateId, dominatorAccount.AccountBaseModel.UserName, timing,
                                SocialNetworks.Instagram);

                        }, s => s.WithName($"{timing.Module}-{templateId}").ToRunOnceAt(timing.StartTime.Hours, timing.StartTime.Minutes));
                        JobManager.AddJob(() =>
                        {
                            StopScheduler(dominatorAccount.AccountBaseModel.AccountId,timing.Module,templateId);

                        }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));
                    }
                    // If start time already crossed of the day and end time is not crossed, then it will start after 5 seconds
                    else if(timing.EndTime.Hours >= currentTimespan.Hours && timing.EndTime.Minutes > currentTimespan.Minutes)
                    {
                        JobManager.AddJob(() =>
                        {
                           
                            var activityDeserialize = new ActivityDeserialize(timing.Module, templateId,  dominatorAccount.AccountBaseModel.UserName, timing,
                                SocialNetworks.Instagram);
                        }, s => s.WithName($"{timing.Module}-{templateId}").ToRunOnceAt(DateTime.Now.AddSeconds(5)));

                        JobManager.AddJob(() =>
                        {
                            
                            StopScheduler(dominatorAccount.AccountBaseModel.AccountId,timing.Module,templateId);

                        }, s => s.ToRunOnceAt(timing.EndTime.Hours, timing.EndTime.Minutes));
                    }
                       
                   
                });
             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
            //JobManager.AllSchedules.ToList().ForEach(x => JobList.Add(x.Name));
        }

        private static string GetTemplateId(TimingRange timing, DominatorAccountModel dominatorAccount)
        {
            var templateId = string.Empty;

            var gdModule = (ActivityType)Enum.Parse(typeof(ActivityType), timing.Module);

            // Returns TemplateId for particular module
            return dominatorAccount.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == gdModule).TemplateId;            
        }
    }    
}