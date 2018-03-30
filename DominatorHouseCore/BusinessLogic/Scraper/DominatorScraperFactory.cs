using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IQueryScraperFactory
    {
        static DominatorScraperFactory _instance;

        public static DominatorScraperFactory Instance => _instance ?? (_instance = new DominatorScraperFactory());

        public QueryScraper Create(JobProcess jobProcess)
        {
            var scrapeProcess = SocinatorInitialize.GetSocialLibrary(jobProcess.SocialNetworks).GetNetworkCoreFactory()
                .QueryScraperFactory;
            return scrapeProcess.Create(jobProcess);               
        }
    }


}
