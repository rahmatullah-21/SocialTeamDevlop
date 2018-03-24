using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IScraperFactory
    {

        public static Func<JobProcess, AbstractQueryScraper> GdAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> TdAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> PdAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> YdAccountConfigScraper { get; set; }


        public static Func<JobProcess, AbstractQueryScraper> FdAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> LdAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> GplusAccountConfigScraper { get; set; }

        public static Func<JobProcess, AbstractQueryScraper> QdAccountConfigScraper { get; set; }


        static DominatorScraperFactory _instance;

        public static DominatorScraperFactory Instance => _instance ?? (_instance = new DominatorScraperFactory());


        public AbstractQueryScraper Create(JobProcess jobProcess)
        {

            switch (jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Instagram:
                    return GdAccountConfigScraper(jobProcess);
                case SocialNetworks.Twitter:
                    return TdAccountConfigScraper(jobProcess);
                case SocialNetworks.Pinterest:
                    return PdAccountConfigScraper(jobProcess);
                case SocialNetworks.Facebook:
                    return FdAccountConfigScraper(jobProcess);
                case SocialNetworks.LinkedIn:
                    return LdAccountConfigScraper(jobProcess);
                case SocialNetworks.Quora:
                    return QdAccountConfigScraper(jobProcess);
                case SocialNetworks.Youtube:
                    return YdAccountConfigScraper(jobProcess);
                case SocialNetworks.Gplus:
                    return GplusAccountConfigScraper(jobProcess);
            }

            return new NotImplementedQueryScraper(jobProcess);
        }
    }


}
