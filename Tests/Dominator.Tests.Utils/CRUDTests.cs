using DominatorHouseCore.DatabaseHandler.Common;
using DominatorHouseCore.DatabaseHandler.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleFixture;
using SQLite;
using System;
using System.IO;

namespace Dominator.Tests.Utils
{
    [TestClass]
    public abstract class CRUDTests<T> where T : class, IPrimaryKey, new()
    {
        private SQLiteConnection _connection;
        protected Fixture Fixture;
        [TestInitialize]
        public void SetUp()
        {
            var accountId = Guid.NewGuid().ToString();
            _connection = GetSqlConnection(accountId); ;
            Fixture = new Fixture();
        }

        protected abstract SQLiteConnection GetSqlConnection(string str);
        protected abstract T GenerateEntity(int id = 0);

        [TestCleanup]
        public void TearDown()
        {
            if (_connection != null)
            {
                var path = _connection.DatabasePath;
                _connection.Dispose();
                File.Delete(path);
            }
        }

        [TestMethod]
        public void should_create()
        {
            // Arrange 
            var data = GenerateEntity();
            var dbOperations = new DbOperations(_connection);


            // Act
            dbOperations.Add(data);

            // Assert
            data.Id.Should().BeGreaterThan(0);
            var saved = dbOperations.Get<T>(a => a.Id == data.Id);
            saved[0].Should().BeEquivalentTo(data);

        }

        [TestMethod]
        public void should_detele()
        {
            // Arrange 
            var data = GenerateEntity();
            var dbOperations = new DbOperations(_connection);
            dbOperations.Add(data);


            // Act
            dbOperations.Remove(data);

            // Assert
            var saved = dbOperations.Get<T>(a => a.Id == data.Id);
            saved.Should().BeEmpty();
        }

        [TestMethod]
        public void should_update()
        {
            // Arrange 
            var data = GenerateEntity();
            var dbOperations = new DbOperations(_connection);
            dbOperations.Add(data);


            // Act
            var data2 = GenerateEntity(data.Id);
            dbOperations.Update(data2);

            // Assert
            var saved = dbOperations.Get<T>(a => a.Id == data.Id);
            saved[0].Should().BeEquivalentTo(data2);
        }

    }
}
