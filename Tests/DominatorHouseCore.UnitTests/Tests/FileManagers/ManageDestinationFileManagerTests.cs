using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class ManageDestinationFileManagerTests : UnityInitializationTests
    {
        private IBinFileHelper _binFileHelper;
        private IManageDestinationFileManager _sut;

        [TestInitialize]
        public void SetUp()
        {
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new ManageDestinationFileManager(_binFileHelper);
        }

        [TestMethod]
        public void should_return_list_of_ManageDestinationModels()
        {
            // arrange
            var helperList = _binFileHelper.GetPublisherManageDestinationModels();

            // act
            var result = _sut.GetAll();

            // assert 
            result.Should().BeEquivalentTo(helperList);
        }
         
    }
}
