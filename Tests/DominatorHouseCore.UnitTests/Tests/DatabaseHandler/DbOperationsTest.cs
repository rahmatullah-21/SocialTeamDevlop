using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Dominator.Tests.Utils;
using DominatorHouseCore.DatabaseHandler.Utility;
using QuoraDominatorCore.Factories;
using SQLite;
using QuoraDominatorCore.DbMigrations;
using DominatorHouseCore.DatabaseHandler.QdTables.Accounts;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.DatabaseHandler
{
    [TestClass]
    public class DbOperationsTest : UnityInitializationTests
    {
        IDbOperations _dbOperation;
        SQLiteConnection _connection;

        [TestInitialize]
        public override void SetUp()
        {
            base.SetUp();
            var accountId = Guid.NewGuid().ToString();
            _connection = new QdAccountDbConnection(new QdAccountDbMigrations()).GetSqlConnection(accountId);
            _dbOperation = new DbOperations(_connection);

        }
        [TestMethod]
        public void Should_Add_method_return_true_if_data_is_successfully_added()
        {
            var _interactedUsers = new InteractedUsers();
            var result = _dbOperation.Add(_interactedUsers);
            result.Should().BeTrue();
        }
        [TestMethod]
        public void Should_Add_method_return_false_if_table_is_not_exists()
        {
            var _interactedUsers = new object();
            var result = _dbOperation.Add(_interactedUsers);
            result.Should().BeFalse();
        }
        [TestMethod]
        public void Should_AddRange_method_return_true_if_data_is_successfully_added()
        {
            var _interactedUsers = new List<InteractedUsers> { new InteractedUsers() };
            var result = _dbOperation.AddRange(_interactedUsers);
            result.Should().BeTrue();
        }
        [TestMethod]
        public void Should_AddRange_method_return_false_if_data_is_empty_list()
        {
            var _interactedUsers = new List<InteractedUsers>();
            var result = _dbOperation.AddRange(_interactedUsers);
            result.Should().BeFalse();
        }
        [TestMethod]
        public void Should_Update_method_return_true_if_data_is_successfully_updated()
        {
            var _interactedUsers = new InteractedUsers();
            var addResult = _dbOperation.Add(_interactedUsers);
            var addeddata = _dbOperation.Get<InteractedUsers>();
            addeddata.Count.Should().Be(1);
            addeddata[0].InteractedUsername.Should().BeNull();
            _interactedUsers.InteractedUsername = "Kumar";
            var result = _dbOperation.Update(_interactedUsers);
            addResult.Should().BeTrue();
            result.Should().BeTrue();
            var Updateddata = _dbOperation.Get<InteractedUsers>();
            Updateddata[0].InteractedUsername.Should().Be("Kumar");
        }
        [TestMethod]
        public void Should_Count_method_return_count_of_data_of_given_db()
        {
            var dbName = "108";
            _connection = new QdAccountDbConnection(new QdAccountDbMigrations()).GetSqlConnection(dbName);
            _dbOperation = new DbOperations(_connection);
            var result = _dbOperation.Count<InteractedUsers>();
            result.Should().Be(1);
        }
    }

}
