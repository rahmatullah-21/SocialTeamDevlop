using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.SessionState;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Utility;
using FluentScheduler;

namespace DominatorHouseCore.Process
{
    public static class PublishScheduler
    {

        #region Properties

        public static Dictionary<string, CancellationTokenSource> CampaignsCancellationTokens { get; set; }
        = new Dictionary<string, CancellationTokenSource>();

        public static ConcurrentDictionary<string, int> AttachedActionCounts { get; set; } = new ConcurrentDictionary<string, int>();

        public static ConcurrentDictionary<string, LinkedList<Action>> PublisherActionList { get; set; }
            = new ConcurrentDictionary<string, LinkedList<Action>>();

        public static SortedSet<string> PublisherScheduledList { get; set; } = new SortedSet<string>();

        #endregion

        public static void IncreasePublishingCount(string campaignId)
        {
            try
            {
                if (!AttachedActionCounts.ContainsKey(campaignId))
                {
                    AttachedActionCounts.GetOrAdd(campaignId, 0);
                }
                var runningCount = AttachedActionCounts[campaignId];
                ++runningCount;
                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    if (count < 0)
                        throw new ArgumentOutOfRangeException(nameof(count));
                    count = runningCount;
                    return count;
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void DecreasePublishingCount(string campaignId)
        {
            try
            {
                if (!AttachedActionCounts.ContainsKey(campaignId))
                    return;

                var runningCount = AttachedActionCounts[campaignId];
                --runningCount;

                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    try
                    {
                        if (count <= 0)
                            throw new ArgumentOutOfRangeException(nameof(count));
                        count = runningCount;
                        return count;
                    }
                    catch (ArgumentOutOfRangeException ex)
                    {
                        ex.DebugLog();
                    }
                    return count;
                });
            }
            catch (ArgumentOutOfRangeException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void AddPublisherAction(string campaignAndAccountId, Action action)
        {
            try
            {
                if (PublisherActionList.ContainsKey(campaignAndAccountId))
                {
                    var actionCollection = PublisherActionList[campaignAndAccountId];

                    actionCollection.AddLast(action);

                    PublisherActionList.AddOrUpdate(campaignAndAccountId, actionCollection, (id, actions) =>
                        {
                            try
                            {
                                if (actions == null)
                                    throw new ArgumentNullException(nameof(actions));
                                actions = actionCollection;
                                return actions;
                            }
                            catch (ArgumentNullException ex)
                            {
                                ex.DebugLog();
                            }
                            return actions;
                        });
                }
                else
                {
                    var list = new LinkedList<Action>();

                    list.AddFirst(action);

                    PublisherActionList.GetOrAdd(campaignAndAccountId, list);
                }
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void RunAndRemovePublisherAction(string campaignAndAccountId)
        {
            try
            {
                if (!PublisherActionList.ContainsKey(campaignAndAccountId))
                    return;

                var actionCollection = PublisherActionList[campaignAndAccountId];

                var action = actionCollection.First();

                actionCollection.RemoveFirst();

                PublisherActionList.AddOrUpdate(campaignAndAccountId, actionCollection, (id, actions) =>
                {
                    if (actions == null)
                        throw new ArgumentNullException(nameof(actions));
                    actions = actionCollection;
                    return actions;
                });

                action.Invoke();
            }
            catch (ArgumentNullException ex)
            {
                ex.DebugLog();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                var currentCampaignsCancallationToken = new CancellationTokenSource();

                if (!CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                    CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);
                else
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[campaignStatusModel.CampaignId];

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);

                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == campaignStatusModel.CampaignId).ToList();

                var usedDestination = publishedDetails.Select(x => x.DestinationUrl);

                var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                                       new GeneralModel();

                var runningCount = 0;

                if (AttachedActionCounts.ContainsKey(campaignStatusModel.CampaignId))
                {
                    runningCount = AttachedActionCounts[campaignStatusModel.CampaignId];
                }

                IncreasePublishingCount(campaignStatusModel.CampaignId);

                #region Random Destination
               
                 if (campaignStatusModel.IsTakeRandomDestination)
                {
                    if (campaignStatusModel.TotalRandomDestination == 0)
                    {
                        GlobusLogHelper.log.Info($"{campaignStatusModel.CampaignName} has zero as maximum publishing count!");
                        return;
                    }

                    var remainingDestinationCount = campaignStatusModel.TotalRandomDestination;

                    //var remainingDestinationCount = campaignStatusModel.TotalRandomDestination - publishedDetails.Count;

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

                                    #region Properties and Initialize

                                    if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                    {
                                        GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                        return;
                                    }

                                    if (currentProcessCount >= remainingDestinationCount)
                                        return;

                                    var selectedGroupDestinations = new List<string>();
                                    var selectedPageOrBoardDestinations = new List<string>();
                                    var selectedCustomDestinations = new List<PublisherCustomDestinationModel>();

                                    #endregion

                                    #region Group Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedGroupDestinations = getGroupDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    #endregion

                                    #region Page Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    #endregion

                                    #region Custom Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getCustomDestinations.RemoveAll(x => usedDestination.Contains(x.DestinationValue));

                                        selectedCustomDestinations = getCustomDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        currentProcessCount += selectedCustomDestinations.Count;
                                    }

                                    #endregion

                                    #region Own Wall 

                                    var isPublishOnOwnWall =
                                                                  destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                            .AccountBaseModel.UserName;

                                        if (advancedSettings.IsDeselectUsedDestination &&
                                            usedDestination.Contains(accountName)) isPublishOnOwnWall = false;

                                        if (isPublishOnOwnWall)
                                            currentProcessCount++;
                                    }

                                    #endregion

                                    #region Publishing

                                    var publishingCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                                          selectedCustomDestinations.Count;

                                    if (isPublishOnOwnWall)
                                        publishingCount++;

                                    if (publishingCount <= 0)
                                    {
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            GlobusLogHelper.log.Info("No more unique destinations are present!");
                                        return;
                                    }

                                    var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                        .GetPublisherCoreFactory()
                                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                    #region Wait to start an action after previous one completes its task

                                    if (advancedSettings.IsWaitToStartAction)
                                    {
                                        if (runningCount >= advancedSettings.JobProcessRunningCount)
                                        {
                                            AddPublisherAction($"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                                publisherJobProcess.StartPublishing(publishingCount,
                                                    !advancedSettings.IsRunSingleAccountPerCampaign));
                                        }
                                        else
                                        {
                                            publisherJobProcess.StartPublishing(publishingCount,
                                                !advancedSettings.IsRunSingleAccountPerCampaign);
                                        }
                                    }

                                    #endregion

                                    else
                                    {
                                        publisherJobProcess.StartPublishing(publishingCount, !advancedSettings.IsRunSingleAccountPerCampaign);
                                    }
                                    #endregion

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
                                    #region Properties and Initialization

                                    if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                    {
                                        GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                        return;
                                    }

                                    if (currentProcessCount >= remainingDestinationCount)
                                        return;

                                    var selectedGroupDestinations = new List<string>();
                                    var selectedPageOrBoardDestinations = new List<string>();
                                    var selectedCustomDestinations = new List<PublisherCustomDestinationModel>();

                                    var accountProcessCount = 0;

                                    var selectDestinationPerAccount =
                                        campaignStatusModel.MinRandomDestinationPerAccount;

                                    #endregion

                                    #region Group Destinations

                                    if (currentProcessCount < remainingDestinationCount
                                                                && accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedGroupDestinations = getGroupDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        accountProcessCount += selectedGroupDestinations.Count;

                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    #endregion

                                    #region Page Destination

                                    if (currentProcessCount < remainingDestinationCount &&
                                                                   accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        currentProcessCount += selectedPageOrBoardDestinations.Count;

                                        accountProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    #endregion

                                    #region Custom Destination

                                    if (currentProcessCount < remainingDestinationCount &&
                                                                   accountProcessCount < selectDestinationPerAccount)
                                    {
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getCustomDestinations.RemoveAll(x => usedDestination.Contains(x.DestinationValue));

                                        selectedCustomDestinations = getCustomDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        currentProcessCount += selectedCustomDestinations.Count;

                                        accountProcessCount += selectedCustomDestinations.Count;
                                    }

                                    #endregion

                                    #region Own Wall Destination

                                    var isPublishOnOwnWall = destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        isPublishOnOwnWall = currentProcessCount < remainingDestinationCount;

                                        if (isPublishOnOwnWall)
                                            isPublishOnOwnWall = accountProcessCount < selectDestinationPerAccount;

                                        if (isPublishOnOwnWall)
                                        {
                                            var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                                .AccountBaseModel.UserName;

                                            if (advancedSettings.IsDeselectUsedDestination &&
                                                usedDestination.Contains(accountName)) isPublishOnOwnWall = false;
                                        }

                                        if (isPublishOnOwnWall)
                                            currentProcessCount += 1;
                                    }
                                    #endregion

                                    #region Scheduling

                                    var publishingCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                                                                    selectedCustomDestinations.Count;

                                    if (isPublishOnOwnWall)
                                        publishingCount++;

                                    if (publishingCount <= 0)
                                    {
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            GlobusLogHelper.log.Info("No more unique destinations are present!");
                                        return;
                                    }

                                    var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                        .GetPublisherCoreFactory()
                                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);


                                    #region Wait to start an action after previous one completes its task

                                    if (advancedSettings.IsWaitToStartAction)
                                    {
                                        if (runningCount >= advancedSettings.JobProcessRunningCount)
                                        {
                                            AddPublisherAction($"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                                publisherJobProcess.StartPublishing(publishingCount,
                                                    !advancedSettings.IsRunSingleAccountPerCampaign));
                                        }
                                        else
                                        {
                                            publisherJobProcess.StartPublishing(publishingCount,
                                                !advancedSettings.IsRunSingleAccountPerCampaign);
                                        }
                                    }

                                    #endregion

                                    else
                                        publisherJobProcess.StartPublishing(publishingCount, !advancedSettings.IsRunSingleAccountPerCampaign);

                                    #endregion
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

                #endregion

                #region All destination

                else
                {
                    #region publish on all destination with single post

                    // To specify deleted destination, like suppose while making campaign with 10 destination then after some time 5 destination
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


                                if (advancedSettings.IsDeselectUsedDestination)
                                {
                                    selectedGroupDestinations.RemoveAll(x => usedDestination.Contains(x));
                                    selectedPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));
                                    selectedCustomDestinations.RemoveAll(x => usedDestination.Contains(x.DestinationValue));
                                    if (isPublishOnOwnWall)
                                    {
                                        var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                            .AccountBaseModel.UserName;

                                        if (usedDestination.Contains(accountName))
                                            isPublishOnOwnWall = false;
                                    }
                                }

                                var publishingCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                                selectedCustomDestinations.Count;

                                if (isPublishOnOwnWall)
                                    publishingCount++;

                                if (publishingCount <= 0)
                                {
                                    if (advancedSettings.IsDeselectUsedDestination)
                                        GlobusLogHelper.log.Info("No more unique destinations are present!");
                                    return;
                                }

                                var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                    .GetPublisherCoreFactory()
                                    .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                #region Under Development

                                if (advancedSettings.IsWaitToStartAction)
                                {
                                    if (runningCount >= advancedSettings.JobProcessRunningCount)
                                    {
                                        AddPublisherAction($"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                            publisherJobProcess.StartPublishing(publishingCount,
                                                !advancedSettings.IsRunSingleAccountPerCampaign));
                                    }
                                    else
                                    {
                                        publisherJobProcess.StartPublishing(publishingCount,
                                            !advancedSettings.IsRunSingleAccountPerCampaign);
                                    }
                                }

                                #endregion
                                else
                                {
                                    publisherJobProcess.StartPublishing(publishingCount, !advancedSettings.IsRunSingleAccountPerCampaign);
                                }

                                //publisherJobProcess.StartPublishing(publishingCount, !campaignStatusModel.IsRunSingleAccountPerCampaign);
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

                #endregion

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
                    PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

                //var campaignDetails =
                //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

                var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == post.CampaignId);

                if (specificCampaign == null)
                {
                    GlobusLogHelper.log.Info("Current post isn't register with any campaign!");
                    return;
                }

                var isStart = ValidateCampaignsTime(specificCampaign);

                if (!isStart)
                {
                    GlobusLogHelper.log.Info("Current post's campaign expired!");
                    return;
                }


                GlobusLogHelper.log.Info(Log.StartPublishing, SocialNetworks.Social, string.Empty, specificCampaign.CampaignName);

                var currentCampaignsCancallationToken = new CancellationTokenSource();

                if (!CampaignsCancellationTokens.ContainsKey(post.CampaignId))
                    CampaignsCancellationTokens.Add(post.CampaignId, currentCampaignsCancallationToken);
                else
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[post.CampaignId];

                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == post.CampaignId);

                int publishCount;

                var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == post.CampaignId) ??
                                       new GeneralModel();


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

                        publishCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                       selectedCustomDestinations.Count;

                        if (isPublishOnOwnWall)
                            publishCount++;

                        if (publishCount <= 0)
                            return;

                        startAction.Invoke();

                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(post.CampaignId, networkWithAccount.Value,
                                selectedGroupDestinations, selectedPageOrBoardDestinations,
                                selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                        publisherJobProcess.StartPublishing(post, publishCount, !advancedSettings.IsRunSingleAccountPerCampaign);

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

        private static bool ValidateCampaignsTime(PublisherCampaignStatusModel specificCampaign)
        {
            var isStart = true;

            if (specificCampaign.StartDate != null)
            {
                if (!(DateTime.Now >= specificCampaign.StartDate))
                    isStart = false;
            }
            if (specificCampaign.EndDate != null)
            {
                if (!(DateTime.Now <= specificCampaign.EndDate))
                    isStart = false;
            }

            return isStart;
        }

        public static void StopPublishingPosts(string campaignId)
        {
            try
            {
                var cancellationToken = CampaignsCancellationTokens.FirstOrDefault(x => x.Key == campaignId);
                cancellationToken.Value.Cancel();

                if (CampaignsCancellationTokens.ContainsKey(campaignId))
                    CampaignsCancellationTokens.Remove(campaignId);

                StopScheduledPublisher(campaignId);
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

            DeletePublishedPost(postDeletionModel);
        }

        public static void DeletePublishedPost(PostDeletionModel postDeletionModel)
        {
            JobManager.AddJob(() =>
            {
                var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(postDeletionModel.Networks)
                    .GetPublisherCoreFactory()
                    .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null, null, null, false, new CancellationTokenSource());

                if (publisherJobProcess.DeletePost(postDeletionModel.PublishedIdOrUrl))
                {
                    publisherJobProcess.UpdatePostWithDeletion(postDeletionModel.DestinationUrl,
                        postDeletionModel.PostId);

                    postDeletionModel.IsDeletedAlready = true;

                    var allDeletionList =
                        GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable.GetDeletePublisherPostModel);
                    var index = allDeletionList.FindIndex(x =>
                        x.PublishedIdOrUrl == postDeletionModel.PublishedIdOrUrl);
                    allDeletionList[index].IsDeletedAlready = true;

                    GenericFileManager.UpdateModuleDetails(allDeletionList, ConstantVariable.GetDeletePublisherPostModel);
                }

            }, s => s.WithName($"{postDeletionModel.CampaignId}- Delete Posts -{ConstantVariable.GetDate()}").ToRunOnceAt(postDeletionModel.DeletionTime));
        }

        public static void SchedulePublishNowByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            if (specificCampaign != null && ValidateCampaignsTime(specificCampaign))
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
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => x.Status == PublisherCampaignStatus.Active).ToList();

            campaignDetails.ForEach(campaign =>
            {
                if (!ValidateCampaignsTime(campaign))
                    return;

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
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            if (specificCampaign != null && ValidateCampaignsTime(specificCampaign))
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

            StopScheduledPublisher(campaign.CampaignId);

            var timeRange = campaign.SpecificRunningTime;

            if (campaign.IsRandomRunningTime)
            {
                if (campaign.UpdatedTime.Date != DateTime.Today)
                    timeRange = GenerateRandomIntervals(campaign.MaximumTime, campaign.TimeRange);
            }

            timeRange.ForEach(runningTime =>
                {
                    var startTime = DateTime.Today.Add(new TimeSpan(runningTime.Hours, runningTime.Minutes, runningTime.Seconds));
                    if (startTime > DateTime.Now)
                    {
                        var addJobName = $"{campaign.CampaignId}-{ConstantVariable.GetDate()}";
                        PublisherScheduledList.Add(addJobName);

                        JobManager.AddJob(() =>
                        {
                            StartPublishingPosts(campaign);
                        }, s => s.WithName(addJobName).ToRunOnceAt(startTime));

                        var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

                        if (advancedSettings?.DestinationTimeout > 0)
                        {
                            var stopJobName = $"{campaign.CampaignId}-StopRunningDueToTimeOut";
                            PublisherScheduledList.Add(stopJobName);
                            var stopTime = DateTime.Now.AddMinutes(advancedSettings.DestinationTimeout);
                            JobManager.AddJob(() =>
                            {
                                StopPublishingPosts(campaign.CampaignId);
                            }, s => s.WithName(stopJobName).ToRunOnceAt(stopTime));
                        }
                    }
                });

            #endregion
        }

        public static List<TimeSpan> GenerateRandomIntervals(int maxCount, TimeRange timeRange)
        {
            var timer = new List<TimeSpan>();
            var random = new Random();
            var startTime = timeRange.StartTime;
            var endTime = timeRange.EndTime;
            for (int countIndex = 0; countIndex < maxCount; countIndex++)
            {
                timer.Add(DateTimeUtilities.GetRandomTime(startTime, endTime, random));
            }
            return timer;
        }

        private static void StopScheduledPublisher(string campaignId)
        {
            var currentCampaignsItems = PublisherScheduledList.Where(x => x.Contains(campaignId)).ToList();
            currentCampaignsItems.ForEach(name =>
            {
                JobManager.RemoveJob(name);
                PublisherScheduledList.Remove(name);
            });
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

        public static ConcurrentDictionary<string, object> UpdatingLock { get; set; } = new ConcurrentDictionary<string, object>();

    }
}