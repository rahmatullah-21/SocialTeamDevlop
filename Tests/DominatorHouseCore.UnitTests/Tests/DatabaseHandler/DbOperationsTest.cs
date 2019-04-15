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
      
    }

}
