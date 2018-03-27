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
        public static Func<string, string, TimingRange, string, JobProcess> TdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> PdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> FdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> QdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> LdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> YdAccountConfigScheduler { get; set; }
        public static Func<string, string, TimingRange, string, JobProcess> GplusAccountConfigScheduler { get; set; }

        static DominatorJobProcessFactory _instance;

        public static DominatorJobProcessFactory Instance => _instance ?? (_instance = new DominatorJobProcessFactory());

        public JobProcess Create(string account, string template, TimingRange currentJobTimeRange, string module, SocialNetworks networks)
        {
            ActivityType activity = (ActivityType)Enum.Parse(typeof(ActivityType), module);

            switch (networks)
            {
                case SocialNetworks.Instagram:
                    return GdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Twitter:
                    return TdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Pinterest:
                    return PdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Facebook:
                    return FdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Quora:
                    return QdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.LinkedIn:
                    return LdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Youtube:
                    return YdAccountConfigScheduler(account, template, currentJobTimeRange, module);
                case SocialNetworks.Gplus:
                    return GplusAccountConfigScheduler(account, template, currentJobTimeRange, module);
            }

            return null;
        }

        private DominatorJobProcessFactory() { }    // singleton
    }
}
