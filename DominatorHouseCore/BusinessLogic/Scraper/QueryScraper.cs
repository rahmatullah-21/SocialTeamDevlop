using System;
using System.Collections.Generic;
using System.Diagnostics;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public abstract class QueryScraper
    {
        private Dictionary<string, Action<QueryInfo>> QueryToActionTable { get; }

        private readonly JobProcess _jobProcess;

        protected QueryScraper(JobProcess jobProcess, Dictionary<string, Action<QueryInfo>> queryToActionTable)
        {
            _jobProcess = jobProcess;
            QueryToActionTable = queryToActionTable;
        }

        protected virtual void ScrapeWithoutQueries() { }

        public void ScrapeWithQueries()
        {        
            Debug.Assert(_jobProcess.SavedQueries.Count > 0);

            foreach (var query in _jobProcess.SavedQueries)
            {
                try
                {                  
                    QueryToActionTable[query.QueryType]?.Invoke(query);
                }
                catch (KeyNotFoundException ex)
                {
                    ex.ErrorLog($"Unable to find key for query type - {query.QueryType}. {ex.Message}");
                }
            }
        }

    }
}