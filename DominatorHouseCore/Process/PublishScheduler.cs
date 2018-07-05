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
                var currentCampaignsCancallationToken = new CancellationTokenSource();

                var publishCount = 1;

                if (!CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                    CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);
                else
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[campaignStatusModel.CampaignId];

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);


                if (campaignStatusModel.IsTakeRandomDestination)
                {
                    if (campaignStatusModel.TotalRandomDestination == 0)
                    {
                        GlobusLogHelper.log.Info($"{campaignStatusModel.CampaignName} has zero as maximum publishing count!");
                        return;
                    }

                    var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == campaignStatusModel.CampaignId).ToList();

                    if (publishedDetails.Count == campaignStatusModel.TotalRandomDestination)
                    {
                        GlobusLogHelper.log.Info($"{campaignStatusModel.CampaignName} already published on {campaignStatusModel.TotalRandomDestination} random destination");
                        return;
                    }

                    var usedDestination = publishedDetails.Select(x => x.DestinationUrl);

                    var remainingDestinationCount = campaignStatusModel.TotalRandomDestination - publishedDetails.Count;

                    publisherPostFetchModel?.SelectedDestinations.Shuffle();

                    #region Perform random destinations without per account count

                    if (campaignStatusModel.MinRandomDestinationPerAccount == 0)
                    {
                        var deletedDestinationCount = 0;

                        var currentProcessCount = 0;

                        publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            try
                            {
                                var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                                if (destinationDetails == null)
                                    deletedDestinationCount++;

                                destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                                {
                                    if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                    {
                                        GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                        return;
                                    }

                                    var selectedGroupDestinations = new List<string>();
                                    var selectedPageOrBoardDestinations = new List<string>();
                                    var selectedCustomDestinations = new List<PublisherCustomDestinationModel>();

                                    if (currentProcessCount > remainingDestinationCount)
                                        return;

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedGroupDestinations = getGroupDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        getCustomDestinations.RemoveAll(x =>
                                            usedDestination.Contains(x.DestinationValue));

                                        selectedCustomDestinations = getCustomDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedCustomDestinations.Count;
                                    }

                                    var isPublishOnOwnWall =
                                        destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                            .AccountBaseModel.UserName;

                                        if (usedDestination.Contains(accountName))
                                            isPublishOnOwnWall = false;
                                    }

                                    if (selectedCustomDestinations.Count > 0 ||
                                        selectedPageOrBoardDestinations.Count > 0 ||
                                        selectedGroupDestinations.Count > 0 || isPublishOnOwnWall)
                                    {
                                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                            .GetPublisherCoreFactory()
                                            .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                        publisherJobProcess.StartPublishing(publishCount, !campaignStatusModel.IsRunSingleAccountPerCampaign);
                                    }

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
                        });

                        if (deletedDestinationCount > 0)
                            GlobusLogHelper.log.Info($"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {campaignStatusModel.CampaignName}");

                    }

                    #endregion

                    #region Perform random destinations with per account count

                    else
                    {
                        var accountWithDestinations = publishedDetails.Select(x => new KeyValuePair<string, string>(x.AccountId, x.DestinationUrl)).ToList();

                        var deletedDestinationCount = 0;

                        var currentProcessCount = 0;

                        publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            try
                            {
                                var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                                if (destinationDetails == null)
                                    deletedDestinationCount++;

                                destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                                {
                                    if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                    {
                                        GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                        return;
                                    }

                                    var selectedGroupDestinations = new List<string>();
                                    var selectedPageOrBoardDestinations = new List<string>();
                                    var selectedCustomDestinations = new List<PublisherCustomDestinationModel>();

                                    var accountProcessCount = 0;

                                    var accountCount =
                                        accountWithDestinations.Count(x => x.Key == networkWithAccount.Value);

                                    if (accountCount >= campaignStatusModel.MinRandomDestinationPerAccount)
                                        return;

                                    var selectDestinationPerAccount =
                                        campaignStatusModel.MinRandomDestinationPerAccount - accountCount;

                                    if (currentProcessCount >= remainingDestinationCount)
                                        return;

                                    if (currentProcessCount < remainingDestinationCount
                                    && accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedGroupDestinations = getGroupDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        accountProcessCount += selectedGroupDestinations.Count;

                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    if (currentProcessCount < remainingDestinationCount &&
                                        accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        currentProcessCount += selectedPageOrBoardDestinations.Count;

                                        accountProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    if (currentProcessCount < remainingDestinationCount &&
                                        accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        getCustomDestinations.RemoveAll(x =>
                                            usedDestination.Contains(x.DestinationValue));

                                        selectedCustomDestinations = getCustomDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        currentProcessCount += selectedCustomDestinations.Count;

                                        accountProcessCount += selectedCustomDestinations.Count;
                                    }

                                    var isPublishOnOwnWall =
                                        destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        isPublishOnOwnWall = accountProcessCount < selectDestinationPerAccount;

                                        if (isPublishOnOwnWall)
                                        {
                                            var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                                .AccountBaseModel.UserName;

                                            if (usedDestination.Contains(accountName))
                                                isPublishOnOwnWall = false;
                                        }

                                        if (isPublishOnOwnWall)
                                            currentProcessCount += 1;
                                    }


                                    if (selectedCustomDestinations.Count > 0 ||
                                        selectedPageOrBoardDestinations.Count > 0 ||
                                        selectedGroupDestinations.Count > 0 || isPublishOnOwnWall)
                                    {
                                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                            .GetPublisherCoreFactory()
                                            .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                        publisherJobProcess.StartPublishing(publishCount, !campaignStatusModel.IsRunSingleAccountPerCampaign);
                                    }

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
                        });

                        if (deletedDestinationCount > 0)
                            GlobusLogHelper.log.Info($"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {campaignStatusModel.CampaignName}");

                    }

                    #endregion
                }
                else
                {

                    #region publish on all destination with single post

                    var deletedDestinationCount = 0;

                    publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                               {
                                   try
                                   {
                                       var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                                       if (destinationDetails == null)
                                           deletedDestinationCount++;

                                       destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                                       {
                                           if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                           {
                                               GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                               return;
                                           }

                                           var selectedGroupDestinations =
                                               destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                           var selectedPageOrBoardDestinations =
                                               destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                           var selectedCustomDestinations =
                                               destinationDetails.CustomDestinations.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                           var isPublishOnOwnWall =
                                               destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                           if (selectedCustomDestinations.Count > 0 ||
                                               selectedPageOrBoardDestinations.Count > 0 ||
                                               selectedGroupDestinations.Count > 0 || isPublishOnOwnWall)
                                           {
                                               var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                                   .GetPublisherCoreFactory()
                                                   .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                               publisherJobProcess.StartPublishing(publishCount, !campaignStatusModel.IsRunSingleAccountPerCampaign);
                                           }

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
                               });

                    if (deletedDestinationCount > 0)
                        GlobusLogHelper.log.Info($"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {campaignStatusModel.CampaignName}");

                    #endregion
                }

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

        public static void StartPublishingPosts(PublisherPostlistModel post, Action startAction)
        {
            try
            {
                var campaignDetails =
                    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

                var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == post.CampaignId);

                if (specificCampaign == null)
                {
                    GlobusLogHelper.log.Info("Current post isn't register with any campaign!");
                    return;
                }

                var currentCampaignsCancallationToken = new CancellationTokenSource();

                if (!CampaignsCancellationTokens.ContainsKey(post.CampaignId))
                    CampaignsCancellationTokens.Add(post.CampaignId, currentCampaignsCancallationToken);
                else
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[post.CampaignId];

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == post.CampaignId);

                var publishCount = 1;

                var deletedDestinationCount = 0;

                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                    if (destinationDetails == null)
                        deletedDestinationCount++;

                    destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                    {
                        if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                        {
                            GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                            return;
                        }
                           
                        var selectedGroupDestinations =
                            destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var selectedPageOrBoardDestinations =
                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var selectedCustomDestinations =
                            destinationDetails.CustomDestinations.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        var isPublishOnOwnWall =
                            destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                        if (selectedCustomDestinations.Count > 0 ||
                            selectedPageOrBoardDestinations.Count > 0 ||
                            selectedGroupDestinations.Count > 0 || isPublishOnOwnWall)
                        {
                            startAction.Invoke();

                            var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                .GetPublisherCoreFactory()
                                .PublisherJobFactory.Create(post.CampaignId, networkWithAccount.Value,
                                    selectedGroupDestinations, selectedPageOrBoardDestinations,
                                    selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                            publisherJobProcess.StartPublishing(post, publishCount,
                                !specificCampaign.IsRunSingleAccountPerCampaign);
                        }
                    });
                });
                if (deletedDestinationCount > 0)
                    GlobusLogHelper.log.Info($"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {specificCampaign.CampaignName}");
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
                var cancellationToken = CampaignsCancellationTokens.FirstOrDefault(x => x.Key == campaignId);
                cancellationToken.Value.Cancel();

                if (CampaignsCancellationTokens.ContainsKey(campaignId))
                    CampaignsCancellationTokens.Remove(campaignId);
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
                    .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null, null, null, false, new CancellationTokenSource());
                publisherJobProcess.DeletePost(postDeletionModel.PublishedIdOrUrl);
            }, s => s.WithName($"{postDeletionModel.CampaignId}- Delete Posts -{ConstantVariable.GetDate()}").ToRunOnceAt(postDeletionModel.DeletionTime));
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
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate && x.Status == PublisherCampaignStatus.Active).ToList();

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

        public static void ScheduleTodaysPublisherByCampaign(string campaignId)
        {

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

                    if (campaign.DestinationTimeout > 0)
                    {
                        var stopTime = DateTime.Now.AddMinutes(campaign.DestinationTimeout);
                        JobManager.AddJob(() =>
                        {
                            StopPublishingPosts(campaign.CampaignId);
                        }, s => s.WithName($"{campaign.CampaignId}-StopRunningDueToTimeOut").ToRunOnceAt(stopTime));
                    }
                }
            });

            #endregion
        }

        public static void UpdateNewGroupList()
        {
            var destinations = ManageDestinationFileManager.GetAll();

            destinations.ForEach(x =>
            {
                if (x.IsAddNewGroups)
                {
                    PublisherInitialize.UpdateNewGroups(x.DestinationId);
                }
            });
        }

    }
}