using DominatorHouseCore.Models;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.Interfaces
{
    /// <summary>
    /// Each library have to implement its own job process factory to create particular
    /// activities via scheduler
    /// </summary>
    public interface IJobProcessFactory
    {
        JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module);
    }
}
