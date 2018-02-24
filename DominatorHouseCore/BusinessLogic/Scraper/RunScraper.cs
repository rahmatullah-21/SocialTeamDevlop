using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public static class ScraperRunner
    {
        public static void Run<T>(JobProcess jobProcess) where T : AbstractQueryScraper
        {
            IScraperFactory scraperFactory = DominatorHouseInitializer.ActiveLibrary.QueryScraperFactory;
            AbstractQueryScraper scraper = scraperFactory.Create(jobProcess);
            scraper.ScrapeWithQueries();
        }
    }
}
