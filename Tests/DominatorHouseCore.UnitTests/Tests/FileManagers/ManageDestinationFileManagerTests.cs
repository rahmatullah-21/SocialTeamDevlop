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
        public void should_return()
        {
            // arrange
            _binFileHelper.GetCampaignInteractedDetails(network).Returns(new List<CampaignInteractionViewModel>());

            // act
            var result = _sut.GetAll();

            // assert
             
        }

        //List<PublisherManageDestinationModel> GetAll();
        //void UpdateDestinations(IList<PublisherManageDestinationModel> libraryDestinations);
        //void DeleteSelected(List<PublisherManageDestinationModel> accs);
        //void Delete(Predicate<PublisherManageDestinationModel> match);
    }
}
