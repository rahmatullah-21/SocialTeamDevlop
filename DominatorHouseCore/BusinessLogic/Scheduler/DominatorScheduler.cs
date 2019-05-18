using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IDominatorScheduler
    {
        void RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange,
            string module);

        void StopActivity(DominatorAccountModel account, string module, string templateId, bool needRestart);
        bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime);
        bool ChangeAccountsRunningStatus(bool isStart, string accountId, ActivityType activityType);
        bool EnableDisableModules(ActivityType stopActivity, ActivityType startActivity, string accountId);
        void ScheduleEachActivity(DominatorAccountModel account);
        void ScheduleActivityForNextJob(DominatorAccountModel dominatorAccount, ActivityType activityType);
        void ScheduleNextActivity(DominatorAccountModel dominatorAccountModel, ActivityType activityType);
        void RescheduleifLimitReached(IJobProcess jobProcess, ReachedLimitInfo limitInfo, ReachedLimitType limitType);
    }

    public class DominatorScheduler : IDominatorScheduler
    {
        public object RunStopActivityLocker = new object();
        private readonly IRunningActivityManager _runningActivityManager;
        private readonly ISchedulerProxy _schedulerProxy;
        private readonly IJobLimitsHolder _jobLimitsHolder;
        private readonly IAccountsCacheService _accountsCacheService;
        private readonly IJobCountersManager _jobCountersManager;
        private readonly IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private readonly IRunningJobsHolder _runningJobsHolder;
        private readonly IJobProcessScopeFactory _jobProcessScopeFactory;

        public DominatorScheduler(IRunningActivityManager runningActivityManager, ISchedulerProxy schedulerProxy, IJobLimitsHolder jobLimitsHolder, IJobProcessScopeFactory jobProcessScopeFactory, IAccountsCacheService accountsCacheService, IJobCountersManager jobCountersManager, IJobActivityConfigurationManager jobActivityConfigurationManager, IRunningJobsHolder runningJobsHolder)
        {
            _runningActivityManager = runningActivityManager;
            _schedulerProxy = schedulerProxy;
            _jobLimitsHolder = jobLimitsHolder;
            _jobProcessScopeFactory = jobProcessScopeFactory;
            _accountsCacheService = accountsCacheService;
            _jobCountersManager = jobCountersManager;
            _jobActivityConfigurationManager = jobActivityConfigurationManager;
            _runningJobsHolder = runningJobsHolder;
        }

        /// <summary>
        /// To start the activity of template for the given account at specified time range
        /// </summary>
        /// <param name="account"></param>
        /// <param name="templateId"></param>
        /// <param name="currentJobTimeRange"></param>
        /// <param name="module"></param>
        public void RunActivity(DominatorAccountModel account, string templateId, TimingRange currentJobTimeRange, string module)
        {
            try
            {
                lock (RunStopActivityLocker)
                {
                    var id = JobProcess.AsId(account.AccountBaseModel.AccountId, templateId);

                    var scheduledJob = _schedulerProxy[id];

                    if (scheduledJob != null && scheduledJob.Disabled)
                        return;

                    var scope = _jobProcessScopeFactory.GetScope(account,
                                   (ActivityType)Enum.Parse(typeof(ActivityType), module), templateId, currentJobTimeRange,
                                   account.AccountBaseModel.AccountNetwork);
                    var activeJobProcessFactory =
                        scope.Resolve<IJobProcessFactory>(account.AccountBaseModel.AccountNetwork
                            .ToString());
                    var jobProcess = activeJobProcessFactory.Create(account.AccountBaseModel.UserName, templateId,
                        currentJobTimeRange, module, account.AccountBaseModel.AccountNetwork);
                    _jobLimitsHolder.Reset(jobProcess.Id, jobProcess.JobConfiguration);
                    var limitInfo = jobProcess.CheckLimit();
                    if (limitInfo.ReachedLimitType != ReachedLimitType.NoLimit)
                    {
                        RescheduleifLimitReached(jobProcess, limitInfo, limitInfo.ReachedLimitType);
                        return;
                    }

                    jobProcess.StartProcessAsync().ContinueWith(a => scope.Dispose());
                    jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void StopActivity(DominatorAccountModel account, string module, string templateId, bool needRestart)
        {
            lock (RunStopActivityLocker)
            {
                Stop(account.AccountId, templateId);
                try
                {
                    var moduleConfiguration =
                        _jobActivityConfigurationManager[account.AccountId].FirstOrDefault(x => x.TemplateId == templateId);
                    moduleConfiguration = moduleConfiguration ??
                        _jobActivityConfigurationManager[account.AccountId].FirstOrDefault(x => x.ActivityType.ToString() == module);
                    if (moduleConfiguration != null)
                    {
                        moduleConfiguration.IsEnabled = needRestart;
                        _jobActivityConfigurationManager.AddOrUpdate(account.AccountId, moduleConfiguration.ActivityType, moduleConfiguration);
                        _accountsCacheService.UpsertAccounts(account);
                    }
                }
                catch { }
                var id = JobProcess.AsId(account.AccountId, templateId);
                _schedulerProxy.RemoveJob(id);
                var scheduledJob = _schedulerProxy[id];

                try
                {
                    if (!needRestart)
                    {
                        //if activity of account is stopped then check if any other activity is enable with that account
                        //if enabled then start next round with enabled activity
                        _runningActivityManager.StartNextRound(account);
                        return;
                    }
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
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
        }

        public bool CompareRunningTime(List<RunningTimes> firstRunningTime, List<RunningTimes> secondRunningTime)
        {
            if ((firstRunningTime == null && secondRunningTime != null) || (secondRunningTime == null && firstRunningTime != null))
                return false;
            else if (firstRunningTime == null)
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
                    // ReSharper disable once RedundantAssignment
                    IsEqual = true;
                }

            }

            return IsEqual;
        }

        public bool ChangeAccountsRunningStatus(bool isStart, string accountId, ActivityType activityType)
        {
            var campaignFileManager = ServiceLocator.Current.GetInstance<ICampaignsFileManager>();
            var accountModel = _accountsCacheService[accountId];
            var moduleConfiguration = _jobActivityConfigurationManager[accountModel?.AccountId, activityType];
            if (moduleConfiguration == null)
                return false;
            try
            {
                var accountstemplateId = moduleConfiguration.TemplateId;
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
                                "This account belongs to campaign configuration, which is paused state. Please make the campaign active before changing activity status for this account.");
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }
                    moduleConfiguration.IsEnabled = true;
                    ScheduleNextActivity(accountModel, activityType);
                }
                else
                {
                    moduleConfiguration.IsEnabled = false;
                    StopActivity(accountModel, activityType.ToString(), accountstemplateId, false);
                }

                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
            finally
            {
                _jobActivityConfigurationManager.AddOrUpdate(accountModel.AccountBaseModel.AccountId, activityType, moduleConfiguration);
                _accountsCacheService.UpsertAccounts(accountModel);
            }
        }
        /// <summary>
        /// This method can be used in cases like Enable AutoFollow/Unfollow. You need to pass the Activity type which has to be disabled as first parameter and ActivityType which has to be enabled as second parameter, and accountID for which the activities has to be updated.
        /// </summary>
        /// <param name="stopActivity">ActivityType which has to be disabled</param>
        /// <param name="startActivity">ActivityType which has to be enabled</param>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public bool EnableDisableModules(ActivityType stopActivity, ActivityType startActivity, string accountId)
        {
            try
            {
                if (ChangeAccountsRunningStatus(false, accountId, stopActivity))
                    if (ChangeAccountsRunningStatus(true, accountId, startActivity))
                        return true;
                    else
                        throw new InvalidOperationException($"Error Code : 1001 Cant able start the activity {startActivity} with account {accountId}");
                else
                    throw new InvalidOperationException($"Error Code : 1002  Cant able stop the activity {stopActivity} with account {accountId}");
            }
            catch (Exception ex)
            {
                ex.DebugLog();
                return false;
            }
        }

        public void ScheduleEachActivity(DominatorAccountModel account)
        {
            try
            {
                foreach (var moduleConfiguration in _jobActivityConfigurationManager[account.AccountId])
                {
                    ScheduleNextActivity(account, moduleConfiguration.ActivityType);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void ScheduleActivityForNextJob(DominatorAccountModel dominatorAccount, ActivityType activityType)
        {
            var moduleConfiguration = _jobActivityConfigurationManager[dominatorAccount.AccountId, activityType];
            if (moduleConfiguration == null || !moduleConfiguration.IsEnabled)
                return;

            // Check if activity with the same id already running
            if (_runningJobsHolder.IsRunning(new JobKey(dominatorAccount.AccountId, moduleConfiguration.TemplateId)))
            {
                //GlobusLogHelper.log.Info(Log.CustomMessage, dominatorAccount.AccountBaseModel.AccountNetwork, dominatorAccount.UserName, activityType,$"User {dominatorAccount.UserName} is already running with {activityType} activity");
                return;
            }
            try
            {
                //Get the time when the activity has to be performed next and stopped.
                var timeToRunNext = DateTimeUtilities.GetNextStartTime(moduleConfiguration, ReachedLimitType.Job);
                if (timeToRunNext == DateTime.MinValue)
                {
                    GlobusLogHelper.log.Debug($"suspicious calculation {timeToRunNext} - {DateTime.Now.AddMinutes(-1)}");
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
                if (timeToRunNext < DateTime.Now)
                {
                    timeToRunNext = timeToRunNext.AddSeconds(25);
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

        public void ScheduleNextActivity(DominatorAccountModel dominatorAccountModel, ActivityType activityType)
        {
            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();

            if (softwareSettings?.IsEnableParallelActivitiesChecked ?? false)
            {
                ScheduleActivityForNextJob(dominatorAccountModel, activityType);
            }
            else
            {
                var moduleConfiguration = _jobActivityConfigurationManager[dominatorAccountModel.AccountId]
                    .Where(x => x.IsEnabled);

                foreach (var config in moduleConfiguration)
                {
                    var id = JobProcess.AsId(dominatorAccountModel.AccountId, config.TemplateId);
                    if (_runningJobsHolder.IsRunning(id))
                        return;
                }
                _runningActivityManager.StartNextRound(dominatorAccountModel);
            }

        }

        public bool Stop(string accountName, string templateId)
        {
            try
            {
                var id = new JobKey(accountName, templateId);
                if (!_runningJobsHolder.Stop(id))
                {
                    GlobusLogHelper.log.Trace($"Job process with Id - {id} not found");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ex.DebugLog(ex.StackTrace);
                return false;
            }
        }

        private void UpdatedScheduleJob(DominatorAccountModel dominatorAccount, TimingRange timing, string templateId,
            string jobId, DateTime timeToRunNext, DateTime stopTime)
        {

            _schedulerProxy.AddJob(() =>
            {
                RunActivity(dominatorAccount, templateId, timing, timing.Module);
            }, s => s.WithName(jobId).ToRunOnceAt(timeToRunNext));


            _schedulerProxy.AddJob(() =>
            {
                StopActivity(dominatorAccount, timing.Module, templateId, true);
            }, s => s.ToRunOnceAt(stopTime));


            #region Old
            //Task.Factory.StartNew(() =>
            //{
            //    _schedulerProxy.AddJob(() =>
            //        {
            //            RunActivity(dominatorAccount, templateId, timing, timing.Module);
            //        },
            //        s => s.WithName(jobId).ToRunOnceAt(timeToRunNext));


            //    _schedulerProxy.AddJob(() =>
            //        {
            //            StopActivity(dominatorAccount, timing.Module, templateId, true);
            //        },
            //        s => s.ToRunOnceAt(stopTime));
            //}); 
            #endregion

        }

        public void RescheduleifLimitReached(IJobProcess jobProcess, ReachedLimitInfo limitInfo, ReachedLimitType limitType)
        {
            //jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            GlobusLogHelper.log.Info(limitInfo.ReachedLimitType.ConvertToLogRecord(),
                jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                jobProcess.DominatorAccountModel.AccountBaseModel.UserName, jobProcess.ActivityType, limitInfo.LimitValue);

            Stop(jobProcess.DominatorAccountModel.AccountId, jobProcess.TemplateId);
            //here jobProcess.JobCancellationTokenSource.Token become true because campaign is stopped here

            var moduleConfiguration = _jobActivityConfigurationManager[jobProcess.DominatorAccountModel.AccountId, jobProcess.ActivityType];
            var nextStartTime = limitType == ReachedLimitType.Job
                ? DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType,
                    jobProcess.JobConfiguration.DelayBetweenJobs.GetRandom())
                : DateTimeUtilities.GetNextStartTime(moduleConfiguration, limitType);

            if (moduleConfiguration != null)
            {
                moduleConfiguration.NextRun = nextStartTime;
                moduleConfiguration.IsEnabled = true;
                _jobActivityConfigurationManager.AddOrUpdate(jobProcess.DominatorAccountModel.AccountBaseModel.AccountId, jobProcess.ActivityType,
                    moduleConfiguration);
                _accountsCacheService.UpsertAccounts(jobProcess.DominatorAccountModel);
            }

            StopActivity(jobProcess.DominatorAccountModel, jobProcess.ActivityType.ToString(), jobProcess.TemplateId, moduleConfiguration.IsEnabled);
            _jobCountersManager.Reset(jobProcess.Id);
            //jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

        }
    }
}