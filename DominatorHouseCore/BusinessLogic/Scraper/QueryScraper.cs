using CommonServiceLocator;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Extensions;

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
        protected QueryScraper(IJobProcess jobProcess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable)
        {
            _jobProcess = jobProcess;
            ScrapeWithQueriesActionTable = scrapeWithQueriesActionTable;
            ScrapeWithoutQueriesActionTable = scrapeWithoutQueriesActionTable;
        }

        private readonly IJobProcess _jobProcess;

        public Dictionary<string, Action<QueryInfo>> ScrapeWithQueriesActionTable { get; }

        public Dictionary<string, Action> ScrapeWithoutQueriesActionTable { get; }

        public void ScrapeWithoutQueries(string module)
        {
            try
            {
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                ScrapeWithoutQueriesActionTable[module]?.Invoke();
                UpdateScheduleIfRequire();
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            }
            catch (KeyNotFoundException ex)
            {
                ex.ErrorLog($"Unable to find key for given module - {module}. {ex.Message}");
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine(@"Cancellation Requested !");
            }
            catch (AggregateException ae)
            {
                ae.HandleOperationCancellation();
            }
            catch (Exception ex)
            {
                ex.DebugLog(
                    $"{GetType().Name} : [Account: {_jobProcess?.DominatorAccountModel?.AccountBaseModel?.UserName}]   (Module => {_jobProcess?.ActivityType})");
            }

        }

        public void ScrapeWithQueries()
        {
            Debug.Assert(_jobProcess.SavedQueries.Count > 0);
            int totalQueries = _jobProcess.SavedQueries.Count;
            int usedQueries = 0;
            var listQueries = new List<QueryInfo>(_jobProcess.SavedQueries);

            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            if (softwareSettings.RunQueriesRandomly)
                listQueries.Shuffle();
            else if (softwareSettings.RunQueriesBottomToTop)
                listQueries.Reverse();

            try
            {
                foreach (var query in listQueries)
                {
                    try
                    {
                        _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        ScrapeWithQueriesActionTable[$"{_jobProcess.ActivityType}{query.QueryType}"]?.Invoke(query);
                        //ScrapeWithQueriesActionTable[$"{_jobProcess.ActivityType}{query.QueryTypeEnum}"]?.Invoke(query);
                        Thread.Sleep(5);
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
                        ae.HandleOperationCancellation();
                    }
                    catch (Exception ex)
                    {
                        ex.DebugLog(
                            $"{GetType().Name} : [Account: {_jobProcess?.DominatorAccountModel?.AccountBaseModel?.UserName}]   (Module => {_jobProcess?.ActivityType})");
                    }

                    usedQueries++;
                }
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
                if (totalQueries == usedQueries)
                {
                    UpdateScheduleIfRequire();
                }
            }
            catch (OperationCanceledException)
            {

            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (!(e is TaskCanceledException || e is OperationCanceledException))
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        private void UpdateScheduleIfRequire()
        {
            var softwareSettingsFileManager = ServiceLocator.Current.GetInstance<ISoftwareSettingsFileManager>();
            var softwareSettings = softwareSettingsFileManager.GetSoftwareSettings();
            
            if (_jobProcess.DominatorAccountModel.IsNeedToSchedule && !softwareSettings.StopIfNoMoreData)
                UpdateScheduleIfNoMoreData();
            else
            {
                GlobusLogHelper.log.Info(Log.ProcessCompleted, _jobProcess.DominatorAccountModel.AccountBaseModel.AccountNetwork,
                    _jobProcess.DominatorAccountModel.AccountBaseModel.UserName, _jobProcess.ActivityType.ToString());

                var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
                _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();

                dominatorScheduler.StopActivity(_jobProcess.DominatorAccountModel, _jobProcess.ActivityType.ToString(),
                    _jobProcess.TemplateId, false);
                _jobProcess.DominatorAccountModel.IsNeedToSchedule = true;
            }
        }

        private void UpdateScheduleIfNoMoreData()
        {
            var jobActivityConfigurationManager = ServiceLocator.Current.GetInstance<IJobActivityConfigurationManager>();
            var moduleConfiguration =
                jobActivityConfigurationManager[_jobProcess?.DominatorAccountModel?.AccountId, _jobProcess.ActivityType];

            var nextStartTime = DateTimeUtilities.GetStartTimeOfNextJob(moduleConfiguration,
                _jobProcess.JobConfiguration.DelayBetweenJobs.GetRandom());
            if (moduleConfiguration != null)
            {
                moduleConfiguration.NextRun = nextStartTime;
                var accountsCacheService = ServiceLocator.Current.GetInstance<IAccountsCacheService>();
                jobActivityConfigurationManager.AddOrUpdate(_jobProcess?.DominatorAccountModel?.AccountId,
                    _jobProcess.ActivityType, moduleConfiguration);
                accountsCacheService.UpsertAccounts(_jobProcess?.DominatorAccountModel);
            }

            GlobusLogHelper.log.Info(Log.NoMoreDataToPerform, _jobProcess.SocialNetworks,
                _jobProcess.DominatorAccountModel.AccountBaseModel.UserName, _jobProcess.ActivityType);
            var dominatorScheduler = ServiceLocator.Current.GetInstance<IDominatorScheduler>();
            _jobProcess.JobCancellationTokenSource.Token.ThrowIfCancellationRequested();
            dominatorScheduler.StopActivity(_jobProcess.DominatorAccountModel, _jobProcess.ActivityType.ToString(),
                _jobProcess.TemplateId, true);
        }
    }
}