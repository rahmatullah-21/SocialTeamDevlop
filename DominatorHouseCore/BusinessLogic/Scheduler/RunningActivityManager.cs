using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class RunningActivityManager
    {
        public static void Initialize(IEnumerable<DominatorAccountModel> accountDetails)
        {
            // decide activities to run
            //IEnumerable<Tuple<DominatorAccountModel, Utility.ModuleConfiguration>> jobConfigs;
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
            var jobActivityConfigurationManager =
                ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId].Where(x => x.IsEnabled)
                .OrderByDescending(PickNextActivity)
                .FirstOrDefault();
            //var moduleConfiguration = jobActivityConfigurationManager
            //    .AllEnabled()
            //    .OrderByDescending(PickNextActivity)
            //    .FirstOrDefault();
            if (moduleConfiguration == null) return;
            //Check if any job process is already scheduled before to run after this activity.
            var schedules = JobManager.AllSchedules;
            var enumerable = schedules as Schedule[] ?? schedules.ToArray();
            IEnumerable<Schedule> lstOfScheduledJobs = enumerable.Where(x => x.Name != null && x.Name.Contains($"{accountModel.AccountId}---"));
            var ofScheduledJobs = lstOfScheduledJobs as Schedule[] ?? lstOfScheduledJobs.ToArray();
            if (ofScheduledJobs.Any())
            {
                var latestScheduledJob = ofScheduledJobs.OrderBy(x => x.NextRun).FirstOrDefault();
                if ((latestScheduledJob != null && latestScheduledJob.NextRun < moduleConfiguration.NextRun))
                {
                    return;
                }
                foreach (var scheduledJob in ofScheduledJobs)
                {
                    JobManager.RemoveJob(scheduledJob.Name);
                }
            }
            DominatorScheduler.ScheduleActivityForNextJob(accountModel, moduleConfiguration.ActivityType);
        }

        private static int PickNextActivity(ModuleConfiguration arg)
        {
            int score = 0; //start from zero
            if (arg.IsEnabled) score += 50;
            TimeSpan differenceMinutes = DateTime.Now.Subtract(arg.NextRun);
            score += 1 * (int)differenceMinutes.TotalMinutes;
            return score;
        }
    }
}
