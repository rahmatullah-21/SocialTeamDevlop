using CommonServiceLocator;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IQueryScraperFactory
    {
        public QueryScraper Create(JobProcess jobProcess)
        {
            var scrapeProcess = ServiceLocator.Current.GetInstance<IQueryScraperFactory>(jobProcess.SocialNetworks.ToString());
            return scrapeProcess.Create(jobProcess);
        }
    }
}
