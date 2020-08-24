#region

using CommonServiceLocator;
using DominatorHouseCore.Process;

#endregion

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IQueryScraperFactory
    {
        public QueryScraper Create(JobProcess jobProcess)
        {
            var scrapeProcess =
                ServiceLocator.Current.GetInstance<IQueryScraperFactory>(jobProcess.SocialNetworks.ToString());
            return scrapeProcess.Create(jobProcess);
        }
    }
}