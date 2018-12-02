using System.Collections.Generic;

namespace DominatorHouseCore.Process
{
    public interface IRunningJobsHolder
    {
        bool IsRunning(JobKey id);
        bool Stop(JobKey id);
        bool StartIfNotRunning(JobKey id, JobProcess jobProcess);
        bool IsActivityRunningForAccount(string accountId);
    }

    /// <summary>
    /// Stores all running job processes. Key - TemplateId
    /// </summary>
    public class RunningJobsHolder : IRunningJobsHolder
    {
        private readonly Dictionary<JobKey, JobProcess> _runningJobProcesses = new Dictionary<JobKey, JobProcess>();
        private readonly HashSet<string> _runningAccounts = new HashSet<string>();
        private readonly object _syncJobProcess = new object();

        public bool IsRunning(JobKey id)
        {
            lock (_syncJobProcess)
            {
                return _runningJobProcesses.ContainsKey(id);
            }
        }

        public bool Stop(JobKey id)
        {
            lock (_syncJobProcess)
                if (IsRunning(id))
                {
                    var jobProcess = _runningJobProcesses[id];
                    _runningJobProcesses.Remove(id);
                    _runningAccounts.Remove(id.AccountId);
                    jobProcess.Stop();
                    return true;
                }

            return false;
        }

        public bool StartIfNotRunning(JobKey id, JobProcess jobProcess)
        {
            lock (_syncJobProcess)
            {
                if (IsRunning(id))
                    return false;
                _runningAccounts.Add(id.AccountId);
                _runningJobProcesses.Add(id, jobProcess);
                return true;
            }
        }

        public bool IsActivityRunningForAccount(string accountId)
        {
            lock (_syncJobProcess)
            {
                return _runningAccounts.Contains(accountId);
            }
        }
    }
}
