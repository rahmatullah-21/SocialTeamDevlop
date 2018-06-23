using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;

namespace DominatorHouseCore.Process
{
    public class PublishScheduler
    {

        #region Properties


        public static Dictionary<string, CancellationTokenSource> CampaignsCancellationTokens { get; set; }
        = new Dictionary<string, CancellationTokenSource>();

        #endregion

        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                if (CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                {
                    GlobusLogHelper.log.Info($"Campaign : {campaignStatusModel.CampaignName} is already runnning!");
                    return;
                }

                var currentCampaignsCancallationToken = new CancellationTokenSource();

                CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);

                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                    destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                    {
                        var selectedGroupDestinations =
                            destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var selectedPageOrBoardDestinations =
                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var isPublishOnOwnWall =
                            destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                        if (campaignStatusModel.IsRunSingleAccountPerCampaign)
                        {
                            publisherJobProcess.StartPublishing(true);
                            //publisherJobProcess.StartPublish with synchronously
                        }
                        else
                        {
                            publisherJobProcess.StartPublishing(false);
                            //publisherJobProcess.StartPublish with Asynchronously
                        }
                    });
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
           
        }

        public static void StartPublishingPosts(PublisherPostlistModel post)
        {
            try
            {
                var campaignDetails =
                    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

                var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == post.CampaignId);

                if (specificCampaign == null)
                {
                    GlobusLogHelper.log.Info("Current post is register with any campaign!");
                    return;
                }

                if (CampaignsCancellationTokens.ContainsKey(post.CampaignId))
                {
                    GlobusLogHelper.log.Info($"Campaign : {specificCampaign.CampaignName} is already runnning!");
                    return;
                }

                var currentCampaignsCancallationToken = new CancellationTokenSource();

                CampaignsCancellationTokens.Add(post.CampaignId, currentCampaignsCancallationToken);

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == post.CampaignId);

                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                    destinationDetails.AccountsWithNetwork.ForEach(networkWithAccount =>
                    {
                        var selectedGroupDestinations =
                            destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var selectedPageOrBoardDestinations =
                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var isPublishOnOwnWall =
                            destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(post.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                        if (specificCampaign.IsRunSingleAccountPerCampaign)
                        {
                            publisherJobProcess.StartPublishing(true, post);
                            //publisherJobProcess.StartPublish with synchronously
                        }
                        else
                        {
                            publisherJobProcess.StartPublishing(false, post);
                            //publisherJobProcess.StartPublish with Asynchronously
                        }
                    });
                });
            }
            catch (OperationCanceledException ex)
            {
                ex.DebugLog("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        e.DebugLog("Cancellation requested before task completion!");
                    else
                        e.DebugLog(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }

        }

        public static void StopPublishingPosts(string campaignId)
        {        
            try
            {
                var cancellationToken =
                      CampaignsCancellationTokens.FirstOrDefault(x => x.Key == campaignId);
                cancellationToken.Value.Cancel();
            }
            catch (Exception ex)
            {
                var specificCampaign = PublisherInitialize.GetInstance.GetSavedCampaigns().ToList().FirstOrDefault(x => x.CampaignId == campaignId);
                if (specificCampaign != null)               
                    ex.DebugLog($"Campaign : {specificCampaign.CampaignName} not started before!");
            }
        }

        public static void EnableDeletePost(PostDeletionModel postDeletionModel)
        {
            GenericFileManager.AddModule(postDeletionModel,
                ConstantVariable.GetDeletePublisherPostModel);

            JobManager.AddJob(() =>
            {
                var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(postDeletionModel.Networks)
                    .GetPublisherCoreFactory()
                    .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null, null, false, new CancellationTokenSource());
                publisherJobProcess.DeletePost(postDeletionModel.PublishedIdOrUrl);
            }, s => s.WithName($"{postDeletionModel.CampaignId}- Delete Posts -{ConstantVariable.GetDate()}").ToRunOnceAt(postDeletionModel.DeletionTime));
        }

        public static void ScheduleTodaysPublisherByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            if (specificCampaign != null)
            {
                if (specificCampaign.IsRotateDayChecked)
                    SchedulePublisher(specificCampaign);
                else
                {
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    SchedulePublisher(specificCampaign);
                }
            }
        }

        public static void SchedulePublishNowByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            if (specificCampaign != null)
            {
                if (specificCampaign.IsRotateDayChecked)
                    StartPublishingPosts(specificCampaign);
                else
                {
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    StartPublishingPosts(specificCampaign);
                }
            }
        }

        public static void ScheduleTodaysPublisher()
        {
            // get the all campaigns which should be present in between 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            campaignDetails.ForEach(campaign =>
            {
                if (campaign.IsRotateDayChecked)
                    SchedulePublisher(campaign);
                else
                {
                    var isCampaignSelected = campaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    SchedulePublisher(campaign);
                }
            });
        }

        private static void SchedulePublisher(PublisherCampaignStatusModel campaign)
        {

            #region Schedule

            campaign.SpecificRunningTime.ForEach(runningTime =>
            {
                var startTime = DateTime.Today.Add(new TimeSpan(runningTime.Hours, runningTime.Minutes, runningTime.Seconds));
                if (startTime > DateTime.Now)
                {
                    JobManager.AddJob(() =>
                    {
                        StartPublishingPosts(campaign);
                    }, s => s.WithName($"{campaign.CampaignId}-{ConstantVariable.GetDate()}").ToRunOnceAt(startTime));
                }
            });

            #endregion
        }
    }
}