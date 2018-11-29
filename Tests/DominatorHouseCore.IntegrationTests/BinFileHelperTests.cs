using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using System;
using System.Collections.Generic;
using Unity;

namespace DominatorHouseCore.IntegrationTests
{
    [TestClass]
    public class BinFileHelperTests : IntegrationTests<IBinFileHelper>
    {
        private ILockFileConfigProvider _lockFileConfigProvider;


        protected override void OverrideDependencies(IUnityContainer container)
        {
            _lockFileConfigProvider = Substitute.For<ILockFileConfigProvider>();
            container.RegisterInstance<ILockFileConfigProvider>(_lockFileConfigProvider);
        }


        [TestMethod]
        [DeploymentItem("TestData", "TestData")]
        public void should_deserialize_campaigns()
        {
            // arrange
            List<CampaignDetails> campaigns = null;

            _lockFileConfigProvider.WithFile<CampaignDetails, List<CampaignDetails>>(Arg.Do(
                (Func<string, List<CampaignDetails>> func) =>
                {
                    campaigns = func.Invoke(@"TestData\CampaignDetails.bin");
                }));

            // act
            Sut.GetCampaignDetail();

            // assert
            campaigns.Count.Should().Be(8);
        }
    }
}
