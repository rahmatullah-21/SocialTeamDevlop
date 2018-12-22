using Dominator.Tests.Utils;
using DominatorHouseCore.BusinessLogic.Scheduler;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using DominatorHouseCore.Process;
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

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _runningActivityManager = Substitute.For<IRunningActivityManager>();
            _schedulerProxy = Substitute.For<ISchedulerProxy>();
            _sut = new DominatorScheduler(_runningActivityManager, _schedulerProxy);
            _jobProcessFactory = Substitute.For<IJobProcessFactory>();
            Container.RegisterInstance<IJobProcessFactory>(SocialNetworks.Twitter.ToString(), _jobProcessFactory);
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
            var module = SocialNetworks.Twitter.ToString();
            var jp = Substitute.For<IJobProcess>();
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork).Returns(jp);

            // act
            _sut.RunActivity(account, template, timeRange, module);

            // assert
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork);
            jp.Received(1).StartProcessAsync();
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
            var module = SocialNetworks.Twitter.ToString();
            var jp = Substitute.For<IJobProcess>();
            _jobProcessFactory.Create(account.AccountBaseModel.UserName, template, timeRange, module,
                account.AccountBaseModel.AccountNetwork).Returns(jp);

            // act
            _sut.StopActivity(account, module, template, false);

            // assert

        }
    }
}
