using Dominator.Tests.Utils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.BusinessLogic.Scraper;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using QuoraDominatorCore.QdLibrary.Processors;
using QuoraDominatorCore.Request;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using Unity;
using System;
using QuoraDominatorCore.QdLibrary;
using QuoraDominatorCore.QdLibrary.Processors.Message;
using QuoraDominatorCore.QdLibrary.DAL;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Process.ExecutionCounters;
using QuoraDominatorCore.Factories;
using QuoraDominatorCore.QdLibrary.Processors.Users;
using DominatorHouseCore.Process.JobConfigurations;
using QuoraDominatorCore.Models;
using DominatorHouseCore.Settings;
using Newtonsoft.Json;

namespace DominatorHouseCore.UnitTests.Tests.Scraper
{
    [TestClass]
    public class QueryScraperTest : UnityInitializationTests
    {
        #region Fields
        IDbAccountServiceScoped dbAccountService;
        IDbGlobalService globalService;
        IDbCampaignService campaignService;
        IQuoraFunctions objQuoraFunct;
        IQdHttpHelper httpHelper;
        private IScraperActionTables _scraperActionTables;
        private IQdJobProcess _qdJobProcess;
        QueryScraper _queryScraper;
        IProcessScopeModel processScopeModel;
        IQdQueryScraperFactory queryScraperFactory;
        IHttpHelper _httpHelper;
        IQdLogInProcess qdLogInProcess;
        IRunningJobsHolder _runningJobsHolder;
        IJobCountersManager _jobCountersManager;
        IDominatorScheduler _dominatorScheduler;
        ICampaignInteractionDetails CampaignInteractionDetails;
        IGlobalInteractionDetails GlobalInteractionDetails;
        JobProcessHelper jobProcess;
        ISoftwareSettings softwaresetting;
        #endregion

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();

            #region Initilization
            softwaresetting = Substitute.For<ISoftwareSettings>();
            Container.RegisterInstance(softwaresetting);

            CampaignInteractionDetails = Substitute.For<ICampaignInteractionDetails>();
            Container.RegisterInstance(CampaignInteractionDetails);

            GlobalInteractionDetails = Substitute.For<IGlobalInteractionDetails>();
            Container.RegisterInstance(GlobalInteractionDetails);

            _runningJobsHolder = Substitute.For<IRunningJobsHolder>();
            Container.RegisterInstance(_runningJobsHolder);

            _jobCountersManager = Substitute.For<IJobCountersManager>();
            Container.RegisterInstance(_jobCountersManager);

            _dominatorScheduler = Substitute.For<IDominatorScheduler>();
            Container.RegisterInstance(_dominatorScheduler);

            processScopeModel = Substitute.For<IProcessScopeModel>();
            Container.RegisterInstance(processScopeModel);

            queryScraperFactory = Substitute.For<IQdQueryScraperFactory>();
            Container.RegisterInstance(queryScraperFactory);

            _httpHelper = Substitute.For<IHttpHelper>();
            Container.RegisterInstance(_httpHelper);

            qdLogInProcess = Substitute.For<IQdLogInProcess>();
            Container.RegisterInstance(qdLogInProcess);

            dbAccountService = Substitute.For<IDbAccountServiceScoped>();
            Container.RegisterInstance(dbAccountService);

            globalService = Substitute.For<IDbGlobalService>();
            Container.RegisterInstance(globalService);

            campaignService = Substitute.For<IDbCampaignService>();
            Container.RegisterInstance(campaignService);

            objQuoraFunct = Substitute.For<IQuoraFunctions>();
            Container.RegisterInstance(objQuoraFunct);

            httpHelper = Substitute.For<IQdHttpHelper>();
            Container.RegisterInstance(httpHelper);

            _qdJobProcess = Substitute.For<IQdJobProcess>();
            Container.RegisterInstance(_qdJobProcess);
            #endregion

            Container.RegisterType<IQuoraScraperActionTables, QuoraScraperActionTables>();
            _scraperActionTables = Container.Resolve<QuoraScraperActionTables>();
        }
        [TestMethod]
        public void should_call_SendMessageToFollowerProcessor()
        {
            jobProcess = new JobProcessHelper(processScopeModel, dbAccountService, queryScraperFactory, httpHelper, qdLogInProcess);
            _queryScraper = new QueryScraperHelper(jobProcess, _scraperActionTables.ScrapeWithQueriesActionTable, _scraperActionTables.ScrapeWithoutQueriesActionTable);

            Container.RegisterType<BaseQuoraProcessor, SendMessageToFollowerProcessor>();

            _queryScraper.ScrapeWithoutQueries("SendMessageToFollower");
            globalService.Received(1).GetAllBlackListUsers();
            globalService.Received(1).GetAllWhiteListUsers();
            dbAccountService.Received(1).GetPrivateBlacklist();
        }
        [TestMethod]
        public void should_call_UserKeywordProcessor()
        {
            var moduleSetting = new FollowerModel();
            var activitySettings = JsonConvert.SerializeObject(moduleSetting);
            processScopeModel = new ProcessScopeModel(new DominatorAccountModel(), ActivityType.Follow, new TimingRange(new TimeSpan(), new TimeSpan()),
            "templateId", SocialNetworks.Quora, new CampaignDetails(), "campid",
            new CommonJobConfiguration(null, new List<QueryInfo>() { new QueryInfo() }, false), activitySettings);

            jobProcess = new JobProcessHelper(processScopeModel, dbAccountService, queryScraperFactory, httpHelper, qdLogInProcess);
            _queryScraper = new QueryScraperHelper(jobProcess, _scraperActionTables.ScrapeWithQueriesActionTable, _scraperActionTables.ScrapeWithoutQueriesActionTable);

            Container.RegisterType<BaseQuoraProcessor, UserKeywordProcessor>();
            _queryScraper.ScrapeWithQueries();
            processScopeModel.ActivityType.Should().Be(ActivityType.Follow);
            processScopeModel.Network.Should().Be(SocialNetworks.Quora);
        }
        [TestMethod]
        public void should_throw_NullReferenceException_if_SavedQuery_is_null()
        {
            var moduleSetting = new FollowerModel();
            var activitySettings = JsonConvert.SerializeObject(moduleSetting);
            processScopeModel = new ProcessScopeModel(new DominatorAccountModel(), ActivityType.Follow, new TimingRange(new TimeSpan(), new TimeSpan()),
            "templateId", SocialNetworks.Quora, new CampaignDetails(), "campid",
            new CommonJobConfiguration(null, null, false), activitySettings);

            jobProcess = new JobProcessHelper(processScopeModel, dbAccountService, queryScraperFactory, httpHelper, qdLogInProcess);
            _queryScraper = new QueryScraperHelper(jobProcess, _scraperActionTables.ScrapeWithQueriesActionTable, _scraperActionTables.ScrapeWithoutQueriesActionTable);

            Assert.ThrowsException<NullReferenceException>(() => _queryScraper.ScrapeWithQueries());
        }
     
    }
    class QueryScraperHelper : QueryScraper
    {
        public QueryScraperHelper(IJobProcess jobProcess, Dictionary<string, Action<QueryInfo>> scrapeWithQueriesActionTable, Dictionary<string, Action> scrapeWithoutQueriesActionTable) : base(jobProcess, scrapeWithQueriesActionTable, scrapeWithoutQueriesActionTable)
        {
        }
    }
    class JobProcessHelper : QuoraJobProcess
    {
        public JobProcessHelper(IProcessScopeModel processScopeModel, IDbAccountServiceScoped accountServiceScoped, IQdQueryScraperFactory queryScraperFactory, IQdHttpHelper httpHelper, IQdLogInProcess qdLogInProcess) : base(processScopeModel, accountServiceScoped, queryScraperFactory, httpHelper, qdLogInProcess)
        {
        }

        public override ReachedLimitInfo CheckLimit()
        {
            return new ReachedLimitInfo();
        }

        public override JobProcessResult PostScrapeProcess(ScrapeResultNew scrapeResult)
        {
            return new JobProcessResult();
        }

        protected override bool Login()
        {
            return true;
        }
    }
}
