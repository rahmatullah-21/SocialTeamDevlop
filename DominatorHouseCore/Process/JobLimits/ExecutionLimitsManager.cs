using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Process.JobLimits
{
    public interface IExecutionLimitsManager
    {
        ReachedLimitInfo CheckIfLimitreached<T>(JobKey key, SocialNetworks networks, ActivityType activityType) where T : class, new();
    }
    public class ExecutionLimitsManager : IExecutionLimitsManager
    {
        private readonly IEntityCountersManager _entityCountersManager;
        private readonly IJobLimitsHolder _jobLimitsHolder;
        private readonly IJobCountersManager _jobCountersManager;

        public ExecutionLimitsManager(IEntityCountersManager entityCountersManager, IJobLimitsHolder jobLimitsHolder, IJobCountersManager jobCountersManager)
        {
            _entityCountersManager = entityCountersManager;
            _jobLimitsHolder = jobLimitsHolder;
            _jobCountersManager = jobCountersManager;
        }

        public ReachedLimitInfo CheckIfLimitreached<T>(JobKey key, SocialNetworks networks, ActivityType activityType) where T : class, new()
        {
            var noOfActionPerformedCurrentJob = _jobCountersManager[key];
            var counters = _entityCountersManager.GetCounter<T>(key.AccountId, networks, activityType);
            var limits = _jobLimitsHolder[key];

            if (counters.NoOfActionPerformedCurrentWeek >= limits.MaxNoOfActionPerWeek)
            {
                return new ReachedLimitInfo(ReachedLimitType.Weekly, limits.MaxNoOfActionPerWeek);
            }

            if (counters.NoOfActionPerformedCurrentDay >= limits.MaxNoOfActionPerDay)
            {
                return new ReachedLimitInfo(ReachedLimitType.Daily, limits.MaxNoOfActionPerDay);
            }

            if (counters.NoOfActionPerformedCurrentHour >= limits.MaxNoOfActionPerHour)
            {
                return new ReachedLimitInfo(ReachedLimitType.Hourly, limits.MaxNoOfActionPerHour);
            }
            if (noOfActionPerformedCurrentJob >= limits.MaxNoOfActionPerJob)
            {
                return new ReachedLimitInfo(ReachedLimitType.Job, limits.MaxNoOfActionPerJob);
            }

            return new ReachedLimitInfo(ReachedLimitType.NoLimit, 0); ;
        }
    }
}
