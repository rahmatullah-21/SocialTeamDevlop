using Dominator.Tests.Utils;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

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
        public void should_return_list_of_saved_destinations()
        {
            // arrange
            var destinations = new List<PublisherManageDestinationModel>
            {
                new PublisherManageDestinationModel {DestinationId = "1"},
                new PublisherManageDestinationModel {DestinationId = "2"},
            };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(destinations);
            
            // act
            var result = _sut.GetAll();

            // assert 
            result.Count.Should().Be(2);
            result.Should().BeEquivalentTo(destinations);
            _binFileHelper.Received(1).GetPublisherManageDestinationModels();
        }
        [TestMethod]
        public void should_delete_saved_destination()
        {
            // arrange
            var notDeleted = new PublisherManageDestinationModel { DestinationId = "1" };
            var shouldDeleted = new PublisherManageDestinationModel { DestinationId = "2" };
            var destinations = new List<PublisherManageDestinationModel>
            {
                notDeleted,
                shouldDeleted
            };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(destinations);

            // act
            _sut.Delete(d => d.DestinationId == shouldDeleted.DestinationId);

            // assert 
            _sut.GetAll().Count.Should().Be(1);
        }
        [TestMethod]
        public void should_save_destination_after_delete_selected()
        {
            // arrange  
            var destinations = new List<PublisherManageDestinationModel>
            {
                new PublisherManageDestinationModel {DestinationId = "1"},
                new PublisherManageDestinationModel {DestinationId = "2"},
                new PublisherManageDestinationModel {DestinationId = "3"}
             };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(destinations);

            // act 
            _sut.DeleteSelected(destinations);

            // assert 
            _sut.GetAll().Count.Should().Be(3);
        }

        [TestMethod]
        public void should_add_destination()
        {
            // arrange
            var newDestination = new PublisherManageDestinationModel { DestinationId = "3" };
            var destinations = new List<PublisherManageDestinationModel>
            {
                new PublisherManageDestinationModel {DestinationId = "1"},
                new PublisherManageDestinationModel {DestinationId = "2"},
            };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(destinations);

            // act
            _sut.Add(newDestination);

            // assert 
            _sut.GetAll().Count.Should().Be(3);
        }

        [TestMethod]
        public void should_update_destination()
        {
            // arrange
            var newDestination = new PublisherManageDestinationModel { DestinationId = "1", DestinationName = "Third" };
            var newDestinations = new ObservableCollection<PublisherManageDestinationModel>();
            newDestinations.Add(newDestination);

            var oldDestination = new PublisherManageDestinationModel { DestinationId = "1", DestinationName = "First" };
            var oldDestinationNotChanged = new PublisherManageDestinationModel { DestinationId = "2", DestinationName = "Second" };

            var destinations = new List<PublisherManageDestinationModel>
            {
                oldDestination,
                oldDestinationNotChanged,
            };
            _binFileHelper.GetPublisherManageDestinationModels().Returns(destinations);

            // act
            _sut.UpdateDestinations(newDestinations);

            // assert 
            _sut.GetAll().Count.Should().Be(2);
            _sut.GetAll().Single(a => a.DestinationId == newDestination.DestinationId).Should().Be(newDestination);
            _sut.GetAll().Single(a => a.DestinationId == oldDestinationNotChanged.DestinationId).Should().Be(oldDestinationNotChanged);

        }

    }
}
