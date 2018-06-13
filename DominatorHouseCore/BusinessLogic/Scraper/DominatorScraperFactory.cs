using DominatorHouseCore.Process;
using DominatorHouseCore.Diagnostics;

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
