using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public interface IScraperFactory
    {
        AbstractQueryScraper Create(JobProcess jobProcess);      
    }

    public interface IQueryScraperFactory
    {
        QueryScraper Create(JobProcess jobProcess);
    }
}
