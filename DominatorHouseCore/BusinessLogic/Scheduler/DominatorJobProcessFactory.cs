using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;


namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class DominatorJobProcessFactory : IJobProcessFactory
    {
        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module, SocialNetworks networks)
        {
            var jobProcessHandler = ServiceLocator.Current.GetInstance<IJobProcessFactory>(networks.ToString());
            return jobProcessHandler.Create(account, template, currentJobTimeRange, module, networks);
        }
    }
}
