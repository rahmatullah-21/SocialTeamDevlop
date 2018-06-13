using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Enums;


namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class DominatorJobProcessFactory : IJobProcessFactory
    {   
        static DominatorJobProcessFactory _instance;

        public static DominatorJobProcessFactory Instance => _instance ?? (_instance = new DominatorJobProcessFactory());

        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module, SocialNetworks networks)
        {
            var jobProcessHandler =
                SocinatorInitialize.GetSocialLibrary(networks).GetNetworkCoreFactory().JobProcessFactory;

            return jobProcessHandler.Create(account, template, currentJobTimeRange, module,networks);
        }

        private DominatorJobProcessFactory() { }    // singleton
    }
}
