using Dominator.Tests.Utils;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;
using DominatorHouseCore.Utility;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NSubstitute;
using Socinator.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class PublishSchedulerTest : UnityInitializationTests
    {
        string campaignId;
        IGenericFileManager _genericFileManager;
        IPublisherCollectionFactory _publisherCollectionFactory;
        [TestInitialize] 
        public void StartUp()
        {
            base.SetUp();
            _genericFileManager = Substitute.For<IGenericFileManager>();
            Container.RegisterInstance(_genericFileManager);

            campaignId = Guid.NewGuid().ToString();
        }
        [TestMethod]
        public void should_increase_running_count_by_one_if_campaignid_is_present()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 3;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.IncreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_add_campaignid_and_runningcount_should_be_1_if_campaignid_is_not_present()
        {
            var expectedRunningCount = 1;
            PublishScheduler.IncreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_decrease_running_count_by_one_if_campaignid_is_present()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 1;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.DecreasePublishingCount(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
        }
        [TestMethod]
        public void should_Run_And_Remove_Action_if_action_is_present_in_campaign_PublisherActionList()
        {
            var input = new LinkedList<Action>();
            input.AddFirst(() => PublishScheduler.DecreasePublishingCount(campaignId));
            PublishScheduler.PublisherActionList.GetOrAdd(campaignId, input);
            PublishScheduler.RunAndRemovePublisherAction(campaignId);
            PublishScheduler.PublisherActionList[campaignId].Should().BeEmpty();
        }

        [TestMethod]
        public void should_start_publishing_post()
        {
            var publisherPostFetchModel = new PublisherPostFetchModel();
            _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).ReturnsForAnyArgs(new List<PublisherPostFetchModel>() { publisherPostFetchModel });

            var publishedPostDetailsModel = new PublishedPostDetailsModel();
            _genericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails)
                .ReturnsForAnyArgs(new List<PublishedPostDetailsModel>() { publishedPostDetailsModel });

            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });

            var input = new PublisherCampaignStatusModel();
            PublishScheduler.StartPublishingPosts(input);
            _genericFileManager.Received(2).GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            _genericFileManager.Received(1).GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails);
            _genericFileManager.Received(1).GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile);
        }
       
    }
}
