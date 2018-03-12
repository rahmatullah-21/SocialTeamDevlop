using DominatorHouseCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Enums;


namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public class DominatorJobProcessFactory : IJobProcessFactory
    {
        public static Func<string, string, TimingRange, string, JobProcess> GdAccountConfigScheduler { get; set; }


        static DominatorJobProcessFactory _instance;

        public static DominatorJobProcessFactory Instance => _instance ?? (_instance = new DominatorJobProcessFactory());

        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module, SocialNetworks networks)
        {
            ActivityType activity = (ActivityType)Enum.Parse(typeof(ActivityType), module);
        
            switch (networks)
            {
                case SocialNetworks.Instagram:
                   return GdAccountConfigScheduler(account, template, currentJobTimeRange, module);                  
            }

            return null;
        }

        private DominatorJobProcessFactory() { }    // singleton
    }
}
