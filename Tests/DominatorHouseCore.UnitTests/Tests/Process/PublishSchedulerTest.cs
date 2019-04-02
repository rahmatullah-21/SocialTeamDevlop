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
using QuoraDominatorCore.ViewModel.Publisher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity;

namespace DominatorHouseCore.UnitTests.Tests.FileManagers
{
    [TestClass]
    public class PublishSchedulerTest : UnityInitializationTests
    {
        string campaignId;
        IGenericFileManager _genericFileManager;
        IAccountsFileManager _accountFileManager;
        IAccountScopeFactory _accountScopeFactory;
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
        [TestMethod]
        public void should_Schedule_PublishNow_ByCampaign()
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

            var publisherCampaignStatusModel = new PublisherCampaignStatusModel
            {
                CampaignId = campaignId,
                ScheduledWeekday = new List<ContentSelectGroup>
                    {
                        new ContentSelectGroup
                        {
                            Content= DateTime.Now.DayOfWeek.ToString(),
                            IsContentSelected=true
                        }
                    }
            };
            PublisherInitialize.GetInstance.ListPublisherCampaignStatusModels.Add(publisherCampaignStatusModel);
            PublisherInitialize.GetInstance.GetSavedCampaigns().FirstOrDefault(x => x.CampaignId == campaignId).Should().Be(publisherCampaignStatusModel);
            var specificCampaign = new PublisherCampaignStatusModel { CampaignId = campaignId };

            PublishScheduler.ValidateCampaignsTime(specificCampaign).Should().Be(true);

            PublishScheduler.SchedulePublishNowByCampaign(campaignId);

            _genericFileManager.Received(2).GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social));
            _genericFileManager.Received(1).GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails);
            _genericFileManager.Received(1).GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile);

        }


        [TestMethod]
        public void should_stop_Publishing_Campaign()
        {
            var actualRunningCount = 2;
            var expectedRunningCount = 1;
            PublishScheduler.AttachedActionCounts.GetOrAdd(campaignId, actualRunningCount);
            PublishScheduler.PublisherScheduledList.Add(campaignId);
            PublishScheduler.CampaignsCancellationTokens.Add(campaignId, new System.Threading.CancellationTokenSource());
            PublishScheduler.StopPublishingPosts(campaignId);
            PublishScheduler.AttachedActionCounts[campaignId].Should().Be(expectedRunningCount);
            PublishScheduler.CampaignsCancellationTokens.Should().BeEmpty();
        }
        [TestMethod]
        public void should_enable_delete_post()
        {
            _accountFileManager = Substitute.For<IAccountsFileManager>();
            Container.RegisterInstance(_accountFileManager);
            _accountScopeFactory = Substitute.For<IAccountScopeFactory>();
            Container.RegisterInstance(_accountFileManager);
            var postDeletionModel = new PostDeletionModel()
            {
                Networks = SocialNetworks.Quora,
                CampaignId = campaignId,
                AccountId = Guid.NewGuid().ToString(),
                DeletionTime = DateTime.Now
            };
            _genericFileManager.AddModule(postDeletionModel,
              ConstantVariable.GetDeletePublisherPostModel).ReturnsForAnyArgs(true);
            var generalmodel = new GeneralModel();
            _genericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
            .ReturnsForAnyArgs(new List<GeneralModel> { generalmodel });

            var publisherCreateCampaignModel = new PublisherCreateCampaignModel();
            _genericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable.GetPublisherCampaignFile())
            .ReturnsForAnyArgs(new List<PublisherCreateCampaignModel> { publisherCreateCampaignModel });

            var publisherPostFetchModel = new PublisherPostFetchModel();

            _genericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).ReturnsForAnyArgs(new List<PublisherPostFetchModel> { publisherPostFetchModel });

            FeatureFlags.Instance = new FeatureFlags { { SocialNetworks.Quora.ToString(), true } };
            var qdPublisherJobProcess = new QdPublisherJobProcess(postDeletionModel.CampaignId, postDeletionModel.AccountId, SocialNetworks.Quora,
                                null, null, null, false, new CancellationTokenSource());
            PublisherInitialize.GetPublisherLibrary(postDeletionModel.Networks)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null,
                                null, null, false, new CancellationTokenSource()).Should().Be(qdPublisherJobProcess);
            PublishScheduler.EnableDeletePost(postDeletionModel);
            _genericFileManager.Received(1).AddModule(postDeletionModel,
              ConstantVariable.GetDeletePublisherPostModel);
        }
    }
}
