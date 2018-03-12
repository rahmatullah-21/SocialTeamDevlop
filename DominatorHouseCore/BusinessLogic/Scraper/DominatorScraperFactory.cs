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

        static DominatorScraperFactory _instance;

        public static DominatorScraperFactory Instance => _instance ?? (_instance = new DominatorScraperFactory());


        public AbstractQueryScraper Create(JobProcess jobProcess)
        {

            switch (jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork)
            {
                case SocialNetworks.Instagram:
                    return GdAccountConfigScraper(jobProcess);
            }

            return new NotImplementedQueryScraper(jobProcess);
        }
    }


}
