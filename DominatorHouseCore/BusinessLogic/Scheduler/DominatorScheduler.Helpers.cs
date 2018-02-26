using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using Newtonsoft.Json;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Utility;
using DominatorHouseCore.Interfaces;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public partial class DominatorScheduler
    {
        public static Action<int, int> ChangeTabIndex { get; set; }

        private static string GetTemplateId(TimingRange timing, DominatorAccountModel dominatorAccount)
        {
            var templateId = string.Empty;

            var gdModule = (ActivityType)Enum.Parse(typeof(ActivityType), timing.Module);

            // Returns TemplateId for particular module
            return dominatorAccount.ActivityManager.LstModuleConfiguration.FirstOrDefault(x => x.ActivityType == gdModule).TemplateId;
        }
    }


}
