using System;
using System.Collections.Generic;
using System.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.BusinessLogic.Scraper
{   
    public interface IScraperActionTables
    {
        // key will be for query type and action will be respective call back
        Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        // key will be for module and action will be respective call back
        Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }
    }

    public abstract class QueryScraper  : IScraperActionTables
    {
        protected QueryScraper(JobProcess jobProcess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable)
        {
            _jobProcess = jobProcess;
            ScrapeWithQueriesActionTable = scrapeWithQueriesActionTable;
            ScrapeWithoutQueriesActionTable = scrapeWithoutQueriesActionTable;
        }

        private readonly JobProcess _jobProcess;

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; set; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; set; }

        public void ScrapeWithoutQueries(string module)
        {
            try
            {
                ScrapeWithoutQueriesActionTable[module]?.Invoke();
            }
            catch (KeyNotFoundException ex)
            {
                ex.ErrorLog($"Unable to find key for given module - {module}. {ex.Message}");
            }
        }

        public void ScrapeWithQueries()
        {
            Debug.Assert(_jobProcess.SavedQueries.Count > 0);

            _jobProcess.SavedQueries.Shuffle();

            foreach (var query in _jobProcess.SavedQueries)
            {
                try
                {
                    ScrapeWithQueriesActionTable[$"{_jobProcess.ActivityType}{query.QueryType}"]?.Invoke(query);
                }
                catch (KeyNotFoundException ex)
                {
                    ex.ErrorLog($"Unable to find key for query type - {query.QueryType}. {ex.Message}");
                }
            }
        }
    }
}