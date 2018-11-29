using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System.Collections.Generic;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class CampaignsFileManagerTests
    {
        private ICampaignsFileManager _sut;
        private IBinFileHelper _binFileHelper;

        [TestInitialize]
        public void SetUp()
        {
            _binFileHelper = Substitute.For<IBinFileHelper>();
            _sut = new CampaignsFileManager(_binFileHelper);
        }

        [TestMethod]
        public void should_return_campaigns_by_network()
        {
            // arrange
            var campaigns = new List<CampaignDetails> { new CampaignDetails { SocialNetworks = SocialNetworks.Twitter } };

            _binFileHelper.GetCampaignDetail().Returns(campaigns);

            // act
            var result = _sut.GetCampaignByNetwork(SocialNetworks.Twitter);

            // assert
            result.Count.Should().Be(1);
        }
    }
}
