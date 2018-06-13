using System;
using System.Linq;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public partial class DominatorScheduler
    {
        private static string GetTemplateId(TimingRange timing, DominatorAccountModel dominatorAccount)
        {
            var templateId = string.Empty;

            var gdModule = (ActivityType)Enum.Parse(typeof(ActivityType), timing.Module);

            // Returns TemplateId for particular module
            return dominatorAccount.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == gdModule).TemplateId;
        }
    }


}
