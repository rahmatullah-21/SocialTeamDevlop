using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class RunningActivityManager
    {
        public static void Initialize(IEnumerable<DominatorAccountModel> accountDetails)
        {
            // decide activities to run
            IEnumerable<Tuple<DominatorAccountModel, Utility.ModuleConfiguration>> jobConfigs;
            if (SoftwareSettingsFileManager.GetSoftwareSettings()?.IsEnableParallelActivitiesChecked ?? false)
            {
                // everything is allowed

                foreach (var account in accountDetails)
                {
                    DominatorScheduler.ScheduleEachActivity(account);
                }
            }
            else
            {
                // be picky - only one per account (choose wisely)
                foreach (var account in accountDetails)
                {
                    StartNextRound(account);
                }
            }
        }

        public static void StartNextRound(DominatorAccountModel accountModel)
        {
            
            var moduleConfiguration = accountModel.ActivityManager.LstModuleConfiguration.Where(arg => arg.IsEnabled &&arg.LstRunningTimes !=null)
                .OrderByDescending(PickNextActivity).FirstOrDefault();
            if (moduleConfiguration==null)return;
            //Check if any job process is already scheduled before to run after this activity.
            var schedules = JobManager.AllSchedules;
            var enumerable = schedules as Schedule[] ?? schedules.ToArray();
            IEnumerable<Schedule> lstOfScheduledJobs = enumerable.Where(x => x.Name != null && x.Name.Contains($"{accountModel.AccountId}---"));
            var ofScheduledJobs = lstOfScheduledJobs as Schedule[] ?? lstOfScheduledJobs.ToArray();
            if (ofScheduledJobs.Any())
            {
                var latestScheduledJob = ofScheduledJobs.OrderBy(x => x.NextRun).FirstOrDefault();
                if (moduleConfiguration != null && (latestScheduledJob != null && latestScheduledJob.NextRun < moduleConfiguration.NextRun))
                {
                    return;
                }
                foreach (var scheduledJob in ofScheduledJobs)
                {
                    JobManager.RemoveJob(scheduledJob.Name);
                }
            }
            DominatorScheduler.ScheduleActivityForNextJob(accountModel, accountModel.AccountBaseModel.AccountNetwork, moduleConfiguration.ActivityType);
        }

        /// <summary>
        /// Scores a configuration
        /// </summary>
        /// <param name="arg">the configuration</param>
        /// <returns>an opaque number where greater is better candidate for running now</returns>
        private static int PonderConfiguration(ModuleConfiguration arg)
        {
            int score = 0; // start from zero
            score += 100 - (int)arg.ActivityType; // a lower type gets you more points
            if (arg.Status == "Active") score += 50; // prefer an active-status campaign
            var wd = (int)DateTime.Today.DayOfWeek;
            score += 50 * arg.LstRunningTimes?
                .Where(rt => rt.IsEnabled) // only if enabled
                .Select(rt => (7 + wd - (int)rt.DayOfWeek) % 7) // how many days ago
                .FirstOrDefault() ?? 0; // the closest (0 today or none enabled)

            return score;
        }

        private static int PickNextActivity(ModuleConfiguration arg)
        {
            int score = 0; //start from zero
            if (arg.IsEnabled) score += 50;
            TimeSpan differenceMinutes = DateTime.Now.Subtract(arg.NextRun);
            score += 1 * (int )differenceMinutes.TotalMinutes;
            return score;
        }
    }
}
