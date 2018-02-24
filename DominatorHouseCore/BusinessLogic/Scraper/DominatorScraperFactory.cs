using DominatorHouseCore.Process;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public class DominatorScraperFactory : IScraperFactory
    {
        static DominatorScraperFactory _instance;

        public static DominatorScraperFactory Instance => _instance ?? (_instance = new DominatorScraperFactory());


        public AbstractQueryScraper Create(JobProcess jobProcess)
        {
            return new NotImplementedQueryScraper();
        }
    }
}
