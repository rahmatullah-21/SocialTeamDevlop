using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;

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
                jobConfigs = accountDetails
                    .SelectMany(acct => acct.ActivityManager.LstModuleConfiguration.Select(moduleConfig => Tuple.Create(acct, moduleConfig)));
            }
            else
            {
                // be picky - only one per account (choose wisely)
                jobConfigs = accountDetails
                    .Select(acct => Tuple.Create(acct, acct.ActivityManager.LstModuleConfiguration.Where(arg => arg.IsEnabled).OrderByDescending(PonderConfiguration).FirstOrDefault()))
                    .Where(job => job.Item2 != null);
            }

            jobConfigs.ForEach(jc =>
            {
                DominatorScheduler.ScheduleTodayJobs(jc.Item1, jc.Item1.AccountBaseModel.AccountNetwork, jc.Item2.ActivityType);
                DominatorScheduler.ScheduleForEachModule(jc.Item2.ActivityType, jc.Item1, jc.Item1.AccountBaseModel.AccountNetwork);
            });

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
    }
}
