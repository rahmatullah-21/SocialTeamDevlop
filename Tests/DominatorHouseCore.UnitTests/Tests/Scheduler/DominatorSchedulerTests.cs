using Dominator.Tests.Utils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
using DominatorHouseCore.Process.ExecutionCounters;
using DominatorHouseCore.Process.JobLimits;
using DominatorHouseCore.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Scheduler
{
    [TestClass]
    public class DominatorSchedulerTests : UnityInitializationTests
    {
        private IDominatorScheduler _sut;
        private IRunningActivityManager _runningActivityManager;
        private ISchedulerProxy _schedulerProxy;
        private IJobProcessFactory _jobProcessFactory;
        private IJobLimitsHolder _jobLimitsHolder;
        private IAccountsCacheService _accountsCacheService;
        private IJobProcessScopeFactory _jobProcessScopeFactory;
        private IJobCountersManager _jobCountersManager;
        private IJobActivityConfigurationManager _jobActivityConfigurationManager;
        private IRunningJobsHolder _runningJobsHolder;


        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _runningActivityManager = Substitute.For<IRunningActivityManager>();
            _schedulerProxy = Substitute.For<ISchedulerProxy>();
            _jobLimitsHolder = Substitute.For<IJobLimitsHolder>();
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            _jobCountersManager = Substitute.For<IJobCountersManager>();
            _jobActivityConfigurationManager = Substitute.For<IJobActivityConfigurationManager>();
            _runningJobsHolder = Substitute.For<IRunningJobsHolder>();

            _jobProcessScopeFactory = Substitute.For<IJobProcessScopeFactory>();
            _sut = new DominatorScheduler(_runningActivityManager, _schedulerProxy, _jobLimitsHolder, _jobProcessScopeFactory, _accountsCacheService, _jobCountersManager, _jobActivityConfigurationManager, _runningJobsHolder);

            _jobProcessFactory = Substitute.For<IJobProcessFactory>();
            Container.RegisterInstance<IJobProcessFactory>(SocialNetworks.Twitter.ToString(), _jobProcessFactory);
            _jobProcessScopeFactory.GetScope(Arg.Any<DominatorAccountModel>(), Arg.Any<ActivityType>(),
                Arg.Any<string>(), Arg.Any<TimingRange>(), Arg.Any<SocialNetworks>())
                .Returns(Container);

        }

        [TestMethod]
        public void should_run_activity_if_its_not_run()
        {
            // arrange
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "UserName",
                    AccountNetwork = SocialNetworks.Twitter
                }
            };
            var template = "template";
            var timeRange = new TimingRange(TimeSpan.MinValue, TimeSpan.MaxValue);
            var module = ActivityType.Follow.ToString();
            var jp = Substitute.For<IJobProcess>();
            jp.CheckLimit().Returns(new ReachedLimitInfo(ReachedLimitType.NoLimit, 0));
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork).Returns(jp);

            // act
            _sut.RunActivity(account, template, timeRange, module);

            // assert
            _jobProcessFactory.Received(1).Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork);
            jp.Received(1).StartProcessAsync();
        }

        [DataTestMethod]
        [DataRow(ReachedLimitType.Daily)]
        [DataRow(ReachedLimitType.Hourly)]
        [DataRow(ReachedLimitType.Weekly)]
        [DataRow(ReachedLimitType.Job)]
        public void should_NOT_run_activity_if_reached_limit(ReachedLimitType limitType)
        {
            // arrange
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "UserName",
                    AccountNetwork = SocialNetworks.Twitter
                }
            };
            var template = "template";
            var timeRange = new TimingRange(TimeSpan.MinValue, TimeSpan.MaxValue);

            var module = ActivityType.Follow.ToString();

            var jp = Substitute.For<IJobProcess>();
            jp.CheckLimit().Returns(new ReachedLimitInfo(limitType, 0));
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork).Returns(jp);

            // act
            _sut.RunActivity(account, template, timeRange, module);

            // assert
            _jobProcessFactory.Received(1).Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork);
            jp.DidNotReceive().StartProcessAsync();
        }

        [TestMethod, Ignore("need to be understoond and probably re-engineered")]
        public void should_stop_running_activity()
        {
            // arrange
            var account = new DominatorAccountModel
            {
                AccountBaseModel = new DominatorAccountBaseModel
                {
                    UserName = "UserName",
                    AccountNetwork = SocialNetworks.Twitter
                }
            };
            var template = "template";
            var timeRange = new TimingRange(TimeSpan.MinValue, TimeSpan.MaxValue);
            var module = ActivityType.Follow.ToString();
            var jp = Substitute.For<IJobProcess>();
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork).Returns(jp);

            // act
            _sut.StopActivity(account, module, template, false);

            // assert

        }
    }
}
