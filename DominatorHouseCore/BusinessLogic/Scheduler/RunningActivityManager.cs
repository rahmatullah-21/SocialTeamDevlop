using CommonServiceLocator;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Settings;
using DominatorHouseCore.Utility;
using FluentScheduler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IRunningActivityManager
    {
        void Initialize(IEnumerable<DominatorAccountModel> accountDetails);
        void StartNextRound(DominatorAccountModel accountModel);
        void ScheduleIfAccountGotSucess(DominatorAccountModel account);

    }
    public class RunningActivityManager : IRunningActivityManager
    {
        public void Initialize(IEnumerable<DominatorAccountModel> accountDetails)
        {

            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            var enabledAccount = accountDetails.Where(x => x.ActivityManager.LstModuleConfiguration.Any(y => y.IsEnabled));
            if (enabledAccount.Count() > 0)
                if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
                {
                    Task.Factory.StartNew(() =>
                     {
                         var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                         // everything is allowed
                         foreach (var account in enabledAccount)
                         {
                             dominatorScheduler?.ScheduleEachActivity(account);
                             Task.Delay(2);
                         }
                     });
                }
                else
                {
                    Task.Factory.StartNew(() =>
                    {
                        // be picky - only one per account (choose wisely)
                        foreach (var account in enabledAccount)
                        {
                            StartNextRound(account);
                            Task.Delay(2);
                        }
                    });
                }

        }

        public void StartNextRound(DominatorAccountModel accountModel)
        {
            var jobActivityConfigurationManager =
                ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            var moduleConfiguration = jobActivityConfigurationManager[accountModel.AccountId].Where(x => x.IsEnabled)
                .OrderByDescending(PickNextActivity)
                .FirstOrDefault();
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

            dominatorScheduler.ScheduleActivityForNextJob(accountModel, moduleConfiguration.ActivityType);
        }
        public void ScheduleIfAccountGotSucess(DominatorAccountModel account)
        {
            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();

            if (account.ActivityManager.LstModuleConfiguration.Any(y => y.IsEnabled))
                if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
                {
                    var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                    dominatorScheduler?.ScheduleEachActivity(account);
                }
                else
                    StartNextRound(account);

        }
        private int PickNextActivity(ModuleConfiguration arg)
        {
            int score = 0; //start from zero
            if (arg.IsEnabled) score += 50;
            TimeSpan differenceMinutes = DateTime.Now.Subtract(arg.NextRun);
            score += 1 * (int)differenceMinutes.TotalMinutes;
            return score;
        }
    }
}
