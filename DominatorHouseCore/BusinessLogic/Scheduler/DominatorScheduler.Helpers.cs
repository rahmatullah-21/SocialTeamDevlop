using CommonServiceLocator;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using System;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public partial class DominatorScheduler
    {
        private static string GetTemplateId(TimingRange timing, DominatorAccountModel dominatorAccount)
        {
            var gdModule = (ActivityType)Enum.Parse(typeof(ActivityType), timing.Module);
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration = jobActivityConfigurationManager[dominatorAccount.AccountId, gdModule];

            // Returns TemplateId for particular module
            return moduleConfiguration?.TemplateId;
        }
    }


}
