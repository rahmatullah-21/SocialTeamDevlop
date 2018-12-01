using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;

namespace DominatorHouseCore.UnitTests.Tests.Models
{
    [TestClass, Ignore("Ignored for a while")]
    public class JobActivityConfigurationManagerTests
    {
        private IJobActivityConfigurationManager _sut;
        private IAccountsCacheService _accountsCacheService;

        [TestInitialize]
        public void SetUp()
        {
            _accountsCacheService = Substitute.For<IAccountsCacheService>();
            _sut = new JobActivityConfigurationManager(_accountsCacheService);
        }

        [TestMethod]
        public void should_add_new_item()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;

            // act
            _sut.AddOrUpdate(accountId, activityType, config);

            // assert
            _sut[accountId, activityType].Should().Be(config);
        }

        [TestMethod]
        public void should_update()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var config1 = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            _sut.AddOrUpdate(accountId, activityType, config1);

            // assert
            _sut[accountId, activityType].Should().Be(config1);
        }

        [TestMethod]
        public void should_delete()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            _sut.Delete(accountId, activityType);

            // assert
            _sut[accountId, activityType].Should().BeNull();
        }

        [TestMethod]
        public void should_return_empty_collection_if_such_account()
        {
            // arrange
            var accountId = "123";

            // act
            var result = _sut[accountId];

            // assert
            result.Should().BeEmpty();
        }

        [TestMethod]
        public void should_return_collection__by_accountId()
        {
            // arrange
            var config = new ModuleConfiguration(); ;
            var accountId = "123";
            var activityType = ActivityType.Delete;
            _sut.AddOrUpdate(accountId, activityType, config);

            // act
            var result = _sut[accountId];

            // assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(config);
        }

        [TestMethod]
        public void should_return_enabled_accounts_initialized_LstRunningTimes()
        {
            // arrange
            var enabledConfig = new ModuleConfiguration
            {
                IsEnabled = true,
                LstRunningTimes = new List<RunningTimes>()

            };
            _sut.AddOrUpdate("1", ActivityType.Delete, enabledConfig);
            _sut.AddOrUpdate("2", ActivityType.AcceptConnectionRequest, new ModuleConfiguration { IsEnabled = true, LstRunningTimes = null });
            _sut.AddOrUpdate("3", ActivityType.AnswersScraper, new ModuleConfiguration
            {
                IsEnabled = false,
                LstRunningTimes = new List<RunningTimes>()
            });

            // act
            var result = _sut.AllEnabled();

            // assert
            result.Count.Should().Be(1);
            result.Single().Should().Be(enabledConfig);
        }
    }
}
