using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.Utility;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Process.ExecutionCounters;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Linq.Expressions;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.Process
{
    [TestClass]
    public class EntityCountersManagerTests : UnityInitializationTests

    {
        private IEntityCountersManager _sut;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            _sut = new EntityCountersManager();
        }

        [TestMethod]
        public void should_init_and_return_count()
        {
            // arrange
            var accountId = "accountId";
            var db = Substitute.For<IDbOperations>();
            db.SocialNetworks.Returns(SocialNetworks.Twitter);
            db.AccountId.Returns(accountId);
            db.Count<DummyEntity>(Arg.Any<Expression<Func<DummyEntity, bool>>>()).Returns(3);
            Container.RegisterInstance<IDbOperations>(db);

            // act
            var cnt = _sut.GetCounter<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);

            // assert
            cnt.NoOfActionPerformedCurrentDay.Should().Be(3);
            cnt.NoOfActionPerformedCurrentHour.Should().Be(3);
            cnt.NoOfActionPerformedCurrentWeek.Should().Be(3);
        }

        [TestMethod]
        public void shold_increment_and_return()
        {
            // arrange
            var accountId = "accountId";
            var db = Substitute.For<IDbOperations>();
            db.SocialNetworks.Returns(SocialNetworks.Twitter);
            db.AccountId.Returns(accountId);
            db.Count<DummyEntity>(Arg.Any<Expression<Func<DummyEntity, bool>>>()).Returns(3);
            Container.RegisterInstance<IDbOperations>(db);

            // act
            _sut.IncrementFor<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);
            var cnt = _sut.GetCounter<DummyEntity>(accountId, SocialNetworks.Twitter, ActivityType.AnswerOnQuestions);

            // assert
            cnt.NoOfActionPerformedCurrentDay.Should().Be(4);
            cnt.NoOfActionPerformedCurrentHour.Should().Be(4);
            cnt.NoOfActionPerformedCurrentWeek.Should().Be(4);
        }

        private class DummyEntity
        {
            public int Id { get; set; }
            public int InteractionDate { get; set; }
        }
    }
}
