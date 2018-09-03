using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DominatorHouseCore.BusinessLogic.Scraper
{
    public interface IScraperActionTables
    {
        // key will be for query type and action will be respective call back
        Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; }

        // key will be for module and action will be respective call back
        Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; }
    }

    public abstract class QueryScraper : IScraperActionTables
    {
        protected QueryScraper(JobProcess jobProcess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable)
        {
            _jobProcess = jobProcess;
            ScrapeWithQueriesActionTable = scrapeWithQueriesActionTable;
            ScrapeWithoutQueriesActionTable = scrapeWithoutQueriesActionTable;
        }

        private readonly JobProcess _jobProcess;

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; }

        public void ScrapeWithoutQueries(string module)
        {
            try
            {
                try
                {
                    _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    ScrapeWithoutQueriesActionTable[module]?.Invoke();
                }
                catch (OperationCanceledException)
                {
                    throw new OperationCanceledException(@"Cancellation Requested !");
                }
                catch (AggregateException ae)
                {
                    foreach (var e in ae.InnerExceptions)
                    {
                        if (e is TaskCanceledException || e is OperationCanceledException)
                            throw new OperationCanceledException(@"Cancellation Requested !");
                        else
                            e.DebugLog(e.StackTrace + e.Message);
                    }
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            catch (KeyNotFoundException ex)
            {
                ex.ErrorLog($"Unable to find key for given module - {module}. {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(@"Cancellation Requested !");
            }
        }

        public void ScrapeWithQueries()
        {
            Debug.Assert(_jobProcess.SavedQueries.Count > 0);
            int totalQueries = _jobProcess.SavedQueries.Count;
            int usedQueries = 0;
            _jobProcess.SavedQueries.Shuffle();

            try
            {
                foreach (var query in _jobProcess.SavedQueries)
                {
                    try
                    {
                        //  ScrapeWithQueriesActionTable[$"{_jobProcess.ActivityType}{query.SelectedQueryEnumId}"]?.Invoke(query);
                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        ScrapeWithQueriesActionTable[$"{_jobProcess.ActivityType}{query.QueryType}"]?.Invoke(query);
                    }
                    catch (KeyNotFoundException ex)
                    {
                        ex.ErrorLog($"Unable to find key for query type - {query.QueryType}. {ex.Message}");
                    }
                    catch (OperationCanceledException)
                    {
                        throw new OperationCanceledException(@"Cancellation Requested !");
                    }
                    catch (AggregateException ae)
                    {
                        foreach (var e in ae.InnerExceptions)
                        {
                            if (e is TaskCanceledException || e is OperationCanceledException)
                                throw new OperationCanceledException(@"Cancellation Requested !");
                            else
                                e.DebugLog(e.StackTrace + e.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog();
                    }

                    usedQueries++;
                }
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (totalQueries == usedQueries)
                    GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, _jobProcess.SocialNetworks, _jobProcess.DominatorAccountModel.AccountBaseModel.UserName, _jobProcess.ActivityType);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(@"Cancellation Requested !");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }
    }
}