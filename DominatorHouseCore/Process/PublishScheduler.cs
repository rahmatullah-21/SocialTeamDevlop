using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
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
        /// <summary>
        /// To specify the Campaign Cancellation Token with campaigns
        /// </summary>
        public static Dictionary<string, CancellationTokenSource> CampaignsCancellationTokens { get; set; }
        = new Dictionary<string, CancellationTokenSource>();

        /// <summary>
        /// To specify the campaign with their running count
        /// </summary>
        public static ConcurrentDictionary<string, int> AttachedActionCounts { get; set; } = new ConcurrentDictionary<string, int>();

        /// <summary>
        /// To block more running campaign for an actions
        /// </summary>
        public static ConcurrentDictionary<string, LinkedList<Action>> PublisherActionList { get; set; }
            = new ConcurrentDictionary<string, LinkedList<Action>>();

        /// <summary>
        /// To Specify the scheduled list id of campaigns
        /// </summary>
        public static SortedSet<string> PublisherScheduledList { get; set; } = new SortedSet<string>();

        /// <summary>
        /// To used in <see cref="PublisherJobProcess"/> for updating success or failed post details 
        /// </summary>
        public static ConcurrentDictionary<string, object> UpdatingLock { get; set; } = new ConcurrentDictionary<string, object>();

        /// <summary>
        /// To used in <see cref="PublisherJobProcess"/> getting post model
        /// </summary>
        public static ConcurrentDictionary<string, object> GetPostsForPublishing { get; set; } = new ConcurrentDictionary<string, object>();

        #endregion

        /// <summary>
        /// Increasing running count of the campaign
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void IncreasePublishingCount(string campaignId)
        {
            try
            {
                // Check whether campaign Id already present or not, If its not present add with zero 
                if (!AttachedActionCounts.ContainsKey(campaignId))
                {
                    AttachedActionCounts.GetOrAdd(campaignId, 0);
                }

                // Get the already saved running count
                var runningCount = AttachedActionCounts[campaignId];
                ++runningCount;

                // Update the action count with new value
                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    if (count < 0)
                        throw new ArgumentOutOfRangeException();
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

        /// <summary>
        /// Decrease the running count for a campaign
        /// </summary>
        /// <param name="campaignId"></param>
        public static void DecreasePublishingCount(string campaignId)
        {
            try
            {
                // Check whether campaign Id already present or not
                if (!AttachedActionCounts.ContainsKey(campaignId))
                    return;

                // If its present reduce by 1 
                var runningCount = AttachedActionCounts[campaignId];
                --runningCount;

                // And Update the recent value
                AttachedActionCounts.AddOrUpdate(campaignId, runningCount, (id, count) =>
                {
                    try
                    {
                        if (count <= 0)
                            count = 0;
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

        /// <summary>
        /// To added actions after specific no of actions are already running with a particular account
        /// </summary>
        /// <param name="campaignAndAccountId">Camapign Id and Account Id</param>
        /// <param name="action">Action which is going to take place</param>
        public static void AddPublisherAction(string campaignAndAccountId, Action action)
        {
            try
            {
                // Check given campaign Id or account Id combination present or not
                if (PublisherActionList.ContainsKey(campaignAndAccountId))
                {
                    // If its present and get action list
                    var actionCollection = PublisherActionList[campaignAndAccountId];

                    // And Append into last
                    actionCollection.AddLast(action);

                    // Update the action list
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
                    // If its not present , create a new action 
                    var list = new LinkedList<Action>();

                    // and as Head of Linked list
                    list.AddFirst(action);

                    // Add to concurrent list
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

        /// <summary>
        /// Execute the action for a campaign with specific account
        /// </summary>
        /// <param name="campaignAndAccountId">Campaign Id with account Id</param>
        public static void RunAndRemovePublisherAction(string campaignAndAccountId)
        {
            try
            {
                // Check whether action list contails campaign Id or account Id
                if (!PublisherActionList.ContainsKey(campaignAndAccountId))
                    return;

                // Get the actions collection for a given Index
                var actionCollection = PublisherActionList[campaignAndAccountId];

                // Fetching a first element from an action lsit
                var action = actionCollection.First();

                // After fetching an action remove from action collection
                actionCollection.RemoveFirst();

                // After remove update the action list
                PublisherActionList.AddOrUpdate(campaignAndAccountId, actionCollection, (id, actions) =>
                {
                    if (actions == null)
                        throw new ArgumentNullException(nameof(actions));
                    actions = actionCollection;
                    return actions;
                });

                // Invoking the actions
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





        /// <summary>
        /// Start publishing posts 
        /// </summary>
        /// <param name="campaignStatusModel">Campaign full status model</param>
        public static void StartPublishingPosts(PublisherCampaignStatusModel campaignStatusModel)
        {
            try
            {
                // create a new cancellation token source
                var currentCampaignsCancallationToken = new CancellationTokenSource();

                // If CampaignsCancellationTokens dictionary doesnt contains for current campaign, add to with proper campaign Id
                if (!CampaignsCancellationTokens.ContainsKey(campaignStatusModel.CampaignId))
                    CampaignsCancellationTokens.Add(campaignStatusModel.CampaignId, currentCampaignsCancallationToken);
                else
                    // If its already present fetch that cancellation token
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[campaignStatusModel.CampaignId];

                // Get he post fetcher details
                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId);

                // Get the success published details
                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == campaignStatusModel.CampaignId).ToList();

                // Filter the success published details with destination url
                var usedDestination = publishedDetails.Select(x => x.DestinationUrl);

                // Get the advanced settings for current campaign Id
                var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ??
                                       new GeneralModel();

                var runningCount = 0;

                // Attached accounts contains campaign Id, then get the already running count
                if (AttachedActionCounts.ContainsKey(campaignStatusModel.CampaignId))
                {
                    runningCount = AttachedActionCounts[campaignStatusModel.CampaignId];
                }

                // Increase the running count
                IncreasePublishingCount(campaignStatusModel.CampaignId);

                #region Random Destination

                // If Campaign start with random destination
                // if (campaignStatusModel.IsTakeRandomDestination)
                if (false)
                {
                    // Check whether total destination is zero 
                    if (campaignStatusModel.TotalRandomDestination == 0)
                    {
                        GlobusLogHelper.log.Info($"{campaignStatusModel.CampaignName} has zero as maximum publishing count!");
                        return;
                    }

                    var remainingDestinationCount = campaignStatusModel.TotalRandomDestination;

                    //var remainingDestinationCount = campaignStatusModel.TotalRandomDestination - publishedDetails.Count;

                    // shuffle the selected destination list
                    publisherPostFetchModel?.SelectedDestinations.Shuffle();

                    // If campaign saved without any account specific count
                    #region Perform random destinations without per account count

                    if (campaignStatusModel.MinRandomDestinationPerAccount == 0)
                    {
                        var deletedDestinationCount = 0;

                        var currentProcessCount = 0;

                        // Iterate all destinations
                        publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            try
                            {
                                // Fetch the destination details
                                var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                                // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
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

                                    // Check whether already reached neccessary count or not
                                    if (currentProcessCount >= remainingDestinationCount)
                                        return;

                                    var selectedGroupDestinations = new List<string>();
                                    var selectedPageOrBoardDestinations = new List<string>();
                                    var selectedCustomDestinations = new List<PublisherCustomDestinationModel>();

                                    #endregion

                                    #region Group Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        // Get the group destinations
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        // From reminding destination take neccessary count
                                        selectedGroupDestinations = getGroupDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        // Update current processing count
                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    #endregion

                                    #region Page Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        // Get the page destinations
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        // From reminding destination take neccessary count
                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        // Update current processing count
                                        currentProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    #endregion

                                    #region Custom Destinations

                                    if (currentProcessCount < remainingDestinationCount)
                                    {
                                        // Get the custom destinations
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getCustomDestinations.RemoveAll(x => usedDestination.Contains(x.DestinationValue));

                                        // From reminding destination take neccessary count
                                        selectedCustomDestinations = getCustomDestinations.Take(remainingDestinationCount - currentProcessCount).ToList();

                                        // Update current processing count
                                        currentProcessCount += selectedCustomDestinations.Count;
                                    }

                                    #endregion

                                    #region Own Wall 

                                    // Check whether own wall has selected or not
                                    var isPublishOnOwnWall = destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        // Get the account Name
                                        var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                            .AccountBaseModel.UserName;

                                        // If campaign saved remove already used destination, then make isPublishOnOwnWall to false
                                        if (advancedSettings.IsDeselectUsedDestination &&
                                            usedDestination.Contains(accountName)) isPublishOnOwnWall = false;

                                        // Update current processing count
                                        if (isPublishOnOwnWall)
                                            currentProcessCount++;
                                    }

                                    #endregion

                                    #region Publishing

                                    // Calculate the publishing count
                                    var publishingCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                                          selectedCustomDestinations.Count;

                                    if (isPublishOnOwnWall)
                                        publishingCount++;

                                    // If publishing count is zero means then there is no destination 
                                    if (publishingCount <= 0)
                                    {
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            GlobusLogHelper.log.Info("No more unique destinations are present!");
                                        return;
                                    }

                                    // Get the publisher Job process
                                    var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                        .GetPublisherCoreFactory()
                                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                                    #region Wait to start an action after previous one completes its task

                                    // Check whether wait to start action after x actions are running parallely
                                    if (advancedSettings.IsWaitToStartAction)
                                    {
                                        // If its running more than give count and add into linked list
                                        if (runningCount >= advancedSettings.JobProcessRunningCount)
                                        {
                                            AddPublisherAction($"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                                publisherJobProcess.StartPublishing(publishingCount,
                                                    !advancedSettings.IsRunSingleAccountPerCampaign));
                                        }
                                        else
                                        {
                                            // Otherwise start calling
                                            publisherJobProcess.StartPublishing(publishingCount,
                                                !advancedSettings.IsRunSingleAccountPerCampaign);
                                        }
                                    }

                                    #endregion

                                    else
                                    {
                                        // If there is no settings for wait to start, then call directly publishing methods
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

                        // Iterate all destinations
                        publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                        {
                            try
                            {
                                // Fetch the destination details
                                var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                                // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                                if (destinationDetails == null)
                                    deletedDestinationCount++;

                                destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                                {
                                    #region Properties and Initialization

                                    // Check whether current accounts network present or not
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
                                        // Get the group destinations
                                        var getGroupDestinations =
                                            destinationDetails.AccountGroupPair
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        // From reminding destination take neccessary count
                                        selectedGroupDestinations = getGroupDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        // Update current account processing count
                                        accountProcessCount += selectedGroupDestinations.Count;

                                        // Update current processing count
                                        currentProcessCount += selectedGroupDestinations.Count;
                                    }

                                    #endregion

                                    #region Page Destination

                                    if (currentProcessCount < remainingDestinationCount &&
                                                                   accountProcessCount < selectDestinationPerAccount)
                                    {
                                        // Get the page destinations
                                        var getPageOrBoardDestinations =
                                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                        // From reminding destination take neccessary count
                                        selectedPageOrBoardDestinations = getPageOrBoardDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        // Update current processing count
                                        currentProcessCount += selectedPageOrBoardDestinations.Count;

                                        // Update current account processing count
                                        accountProcessCount += selectedPageOrBoardDestinations.Count;
                                    }

                                    #endregion

                                    #region Custom Destination

                                    if (currentProcessCount < remainingDestinationCount &&
                                                                   accountProcessCount < selectDestinationPerAccount)
                                    {
                                        // Get the custom destinations
                                        var getCustomDestinations =
                                            destinationDetails.CustomDestinations
                                                .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value)
                                                .ToList();

                                        // If campaign saved remove already used destination, then remove used destinations
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            getCustomDestinations.RemoveAll(x => usedDestination.Contains(x.DestinationValue));

                                        // From reminding destination take neccessary count
                                        selectedCustomDestinations = getCustomDestinations.Take(selectDestinationPerAccount - accountProcessCount).ToList();

                                        // Update current processing count
                                        currentProcessCount += selectedCustomDestinations.Count;

                                        // Update current account processing count
                                        accountProcessCount += selectedCustomDestinations.Count;
                                    }

                                    #endregion

                                    #region Own Wall Destination

                                    // Check whether own wall has selected or not
                                    var isPublishOnOwnWall = destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                    if (isPublishOnOwnWall)
                                    {
                                        // Validate is already reached processing count or not
                                        isPublishOnOwnWall = currentProcessCount < remainingDestinationCount;

                                        if (isPublishOnOwnWall)
                                            isPublishOnOwnWall = accountProcessCount < selectDestinationPerAccount;

                                        if (isPublishOnOwnWall)
                                        {
                                            // Get the account Name
                                            var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                                .AccountBaseModel.UserName;

                                            // If campaign saved remove already used destination, then make isPublishOnOwnWall to false
                                            if (advancedSettings.IsDeselectUsedDestination &&
                                                usedDestination.Contains(accountName)) isPublishOnOwnWall = false;
                                        }
                                        // Update current processing count
                                        if (isPublishOnOwnWall)
                                            currentProcessCount += 1;
                                    }
                                    #endregion

                                    #region Scheduling

                                    // Calculate the publishing count
                                    var publishingCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                                                                    selectedCustomDestinations.Count;

                                    if (isPublishOnOwnWall)
                                        publishingCount++;

                                    // If publishing count is zero means then there is no destination 
                                    if (publishingCount <= 0)
                                    {
                                        if (advancedSettings.IsDeselectUsedDestination)
                                            GlobusLogHelper.log.Info("No more unique destinations are present!");
                                        return;
                                    }
                                    // Get the publisher Job process
                                    var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                                        .GetPublisherCoreFactory()
                                        .PublisherJobFactory.Create(campaignStatusModel.CampaignId, networkWithAccount.Value, selectedGroupDestinations, selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);


                                    #region Wait to start an action after previous one completes its task

                                    // Check whether wait to start action after x actions are running parallely
                                    if (advancedSettings.IsWaitToStartAction)
                                    {
                                        // If its running more than give count and add into linked list
                                        if (runningCount >= advancedSettings.JobProcessRunningCount)
                                        {
                                            AddPublisherAction($"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                                publisherJobProcess.StartPublishing(publishingCount,
                                                    !advancedSettings.IsRunSingleAccountPerCampaign));
                                        }
                                        else
                                        {
                                            // Otherwise start calling
                                            publisherJobProcess.StartPublishing(publishingCount,
                                                !advancedSettings.IsRunSingleAccountPerCampaign);
                                        }
                                    }

                                    #endregion
                                    // If there is no settings for wait to start, then call directly publishing methods
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

                #region New Random Destinations

                if (campaignStatusModel.IsTakeRandomDestination)
                {
                    // Check whether total destination is zero 
                    if (campaignStatusModel.TotalRandomDestination == 0)
                    {
                        GlobusLogHelper.log.Info(
                            $"{campaignStatusModel.CampaignName} has zero as maximum publishing count!");
                        return;
                    }

                    var postsMaximumDestinationCount = campaignStatusModel.TotalRandomDestination;

                    var postsAccountDestinationLimits = campaignStatusModel.MinRandomDestinationPerAccount;

                    var accountsWithDestinations = new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

                    //Get the general settings from bin files
                    var generalSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                                   (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                                   .FirstOrDefault(x => x.CampaignId == campaignStatusModel.CampaignId) ?? new GeneralModel();


                    var accountIds = new SortedSet<string>();

                    var allDestination = new Queue<PublisherDestinationDetailsModel>();

                    var accountsWithNetworks = new Dictionary<string, SocialNetworks>();

                    var totalDestinationCount = 0;

                    // To specify deleted destination, like suppose while making campaign with 10 destination then after some time 5 destination
                    var deletedDestinationCount = 0;

                    // Iterate all selected destinations
                    publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                    {
                        // Get destination details
                        var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                        // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                        if (destinationDetails == null)
                            deletedDestinationCount++;

                        if (!generalSettingsModel.IsStopRandomisingDestinationsOrder)
                            destinationDetails?.DestinationDetailsModels.Shuffle();

                        destinationDetails?.DestinationDetailsModels.ForEach(x =>
                        {
                            // If campaign saved remove already used destination, then remove used destinations
                            if (advancedSettings.IsDeselectUsedDestination && usedDestination.Contains(x.DestinationUrl))
                                return;

                            ++totalDestinationCount;

                            accountIds.Add(x.AccountId);

                            if (!accountsWithNetworks.ContainsKey(x.AccountId))
                                accountsWithNetworks.Add(x.AccountId, x.SocialNetworks);

                            if (postsAccountDestinationLimits > 0)
                            {
                                var currentAccountQueue = accountsWithDestinations.GetOrAdd(x.AccountId,
                                    queue => new Queue<PublisherDestinationDetailsModel>());
                                currentAccountQueue.Enqueue(x);
                            }
                            else
                                allDestination.Enqueue(x);
                        });
                    });

                    if (deletedDestinationCount > 0)
                        GlobusLogHelper.log.Info(
                            $"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {campaignStatusModel.CampaignName}");

                    var destinations = postsAccountDestinationLimits > 0 ?
                         AssignPostsToDestinationWithAccountLimit(accountsWithDestinations, accountIds, totalDestinationCount, campaignStatusModel.CampaignId, campaignStatusModel.CampaignName, postsMaximumDestinationCount, postsAccountDestinationLimits) :
                         AssignPostsToDestinationWithNoAccountLimit(allDestination, accountIds, totalDestinationCount, campaignStatusModel.CampaignId, campaignStatusModel.CampaignName, postsMaximumDestinationCount);

                    foreach (var destination in destinations)
                    {
                        var accountsNetwork = accountsWithNetworks[destination.Key];

                        // Check whether current accounts network present or not
                        if (!SocinatorInitialize.IsNetworkAvailable(accountsNetwork))
                        {
                            GlobusLogHelper.log.Info(
                                $"You don't have a permission to run with {accountsNetwork} network, please purchase !");
                            continue;
                        }

                        // Get the publisher Job process
                        var publisherJobProcess = PublisherInitialize
                            .GetPublisherLibrary(accountsNetwork)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(campaignStatusModel.CampaignId, campaignStatusModel.CampaignName,
                                destination.Key, accountsNetwork, destination.Value,
                                currentCampaignsCancallationToken);

                        #region Wait to start actions

                        // Check whether wait to start action after x actions are running parallely
                        if (advancedSettings.IsWaitToStartAction)
                        {
                            if (runningCount >= advancedSettings.JobProcessRunningCount)
                            {
                                AddPublisherAction(
                                    $"{campaignStatusModel.CampaignId}-{destination.Key}", () =>
                                        publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign));
                            }
                            else
                            {
                                // Otherwise start calling
                                publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign);
                            }
                        }         
                                                         
                        #endregion

                        else
                        {
                            // If there is no settings for wait to start, then call directly publishing methods
                            publisherJobProcess.StartPublishingPosts(!advancedSettings.IsRunSingleAccountPerCampaign);
                        }
                    }
                }

                #endregion

                #region All destination

                else
                {
                    #region publish on all destination with single post

                    // To specify deleted destination, like suppose while making campaign with 10 destination then after some time 5 destination
                    var deletedDestinationCount = 0;

                    // Iterate all selected destinations
                    publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                    {
                        try
                        {
                            // Get destination details
                            var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                            // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                            if (destinationDetails == null)
                                deletedDestinationCount++;

                            destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                            {

                                // Check whether current accounts network present or not
                                if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                                {
                                    GlobusLogHelper.log.Info(
                                        $"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                                    return;
                                }

                                // Get the all selected group destinations
                                var selectedGroupDestinations =
                                    destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value)
                                        .Select(x => x.Value).ToList();

                                // Get all pages Or Boards destinations
                                var selectedPageOrBoardDestinations =
                                    destinationDetails.AccountPagesBoardsPair
                                        .Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                                // Get all custom destinations
                                var selectedCustomDestinations =
                                    destinationDetails.CustomDestinations.Where(x => x.Key == networkWithAccount.Value)
                                        .Select(x => x.Value).ToList();

                                // Is validate Own wall selected or not
                                var isPublishOnOwnWall =
                                    destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                                // If campaign saved remove already used destination, then remove used destinations
                                if (advancedSettings.IsDeselectUsedDestination)
                                {
                                    // removed used group destinations
                                    selectedGroupDestinations.RemoveAll(x => usedDestination.Contains(x));

                                    // removed used page destinations
                                    selectedPageOrBoardDestinations.RemoveAll(x => usedDestination.Contains(x));

                                    // removed used custom destinations
                                    selectedCustomDestinations.RemoveAll(x =>
                                        usedDestination.Contains(x.DestinationValue));

                                    // Check own wall is selected or not
                                    if (isPublishOnOwnWall)
                                    {
                                        var accountName = AccountsFileManager.GetAccountById(networkWithAccount.Value)
                                            .AccountBaseModel.UserName;

                                        if (usedDestination.Contains(accountName))
                                            isPublishOnOwnWall = false;
                                    }
                                }

                                // Calculate the publishing count
                                var publishingCount =
                                    selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                    selectedCustomDestinations.Count;

                                if (isPublishOnOwnWall)
                                    publishingCount++;

                                // If publishing count is zero means then there is no destination 
                                if (publishingCount <= 0)
                                {
                                    if (advancedSettings.IsDeselectUsedDestination)
                                        GlobusLogHelper.log.Info("No more unique destinations are present!");
                                    return;
                                }

                                // Get the publisher Job process
                                var publisherJobProcess = PublisherInitialize
                                    .GetPublisherLibrary(networkWithAccount.Key)
                                    .GetPublisherCoreFactory()
                                    .PublisherJobFactory.Create(campaignStatusModel.CampaignId,
                                        networkWithAccount.Value, selectedGroupDestinations,
                                        selectedPageOrBoardDestinations, selectedCustomDestinations, isPublishOnOwnWall,
                                        currentCampaignsCancallationToken);

                                #region Wait to start actions

                                // Check whether wait to start action after x actions are running parallely
                                if (advancedSettings.IsWaitToStartAction)
                                {
                                    if (runningCount >= advancedSettings.JobProcessRunningCount)
                                    {
                                        AddPublisherAction(
                                            $"{campaignStatusModel.CampaignId}-{networkWithAccount.Value}", () =>
                                                publisherJobProcess.StartPublishing(publishingCount,
                                                    !advancedSettings.IsRunSingleAccountPerCampaign));
                                    }
                                    else
                                    {
                                        // Otherwise start calling
                                        publisherJobProcess.StartPublishing(publishingCount,
                                            !advancedSettings.IsRunSingleAccountPerCampaign);
                                    }
                                }

                                #endregion

                                else
                                {
                                    // If there is no settings for wait to start, then call directly publishing methods
                                    publisherJobProcess.StartPublishing(publishingCount,
                                        !advancedSettings.IsRunSingleAccountPerCampaign);
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
                        GlobusLogHelper.log.Info(
                            $"Error : Destination deleted {deletedDestinationCount} out of {publisherPostFetchModel?.SelectedDestinations.Count} from {campaignStatusModel.CampaignName}");

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



        public static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> AssignPostsToDestinationWithNoAccountLimit(
            Queue<PublisherDestinationDetailsModel> totalDestinations,
            SortedSet<string> accountId,
            int totalDestinationCount,
            string campaignId,
            string campaignName,
            int postsMaximumDestinationCount)
        {
            ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinationWithPosts;


            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {

                var givenDestinations = totalDestinations.ToList();

                var accounts = accountId.ToList();

                accounts.Shuffle();

                var postsDestinations = new List<List<string>>();

                #region Split the destinations for a post

                while (true)
                {
                    // Split the destination with maximum destinations
                    var currentPostsDestination = new List<string>();

                    for (var initial = 0; initial < postsMaximumDestinationCount; initial++)
                    {
                        if (totalDestinations.Count <= 0)
                            break;
                        // Get the destinations
                        var destination = totalDestinations.Dequeue();
                        currentPostsDestination.Add(destination.DestinationGuid);
                    }
                    if (currentPostsDestination.Count > 0)
                        // Add the splitted destinations
                        postsDestinations.Add(currentPostsDestination);
                    else
                        break;
                }

                #endregion

                destinationWithPosts = SubstitudePoststoDestinations(campaignId, campaignName, givenDestinations, postsDestinations);
            }

            return destinationWithPosts;
        }


        public static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> AssignPostsToDestinationWithAccountLimit
            (ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> totalDestinations,
            SortedSet<string> accountId,
            int totalDestinationCount,
            string campaignId,
            string campaignName,
            int postsMaximumDestinationCount,
            int postsAccountDestinationLimits)
        {
            ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> destinationWithPosts;

            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                var accountsDestinations = new List<PublisherDestinationDetailsModel>();

                var accounts = accountId.ToList();

                accounts.Shuffle();

                var postsDestinations = new List<List<string>>();

                #region Split the destinations for a post

                while (true)
                {
                    // Split the destination with maximum destinations
                    var currentPostsDestination = new List<string>();

                    foreach (var account in accounts)
                    {
                        // Check all destination already partcipate for splitting
                        if (accountsDestinations.Count >= totalDestinationCount)
                            break;

                        // current split already assigned reached accounts limits
                        if (currentPostsDestination.Count >= postsMaximumDestinationCount)
                            break;

                        // Get the destinations queue   
                        var currentAccountQueue = totalDestinations[account];

                        for (var accountLimit = 0; accountLimit < postsAccountDestinationLimits; accountLimit++)
                        {
                            // current split already assigned reached accounts limits
                            if (currentPostsDestination.Count >= postsMaximumDestinationCount)
                                break;

                            if (currentAccountQueue.Count == 0)
                                break;
                            var destination = currentAccountQueue.Dequeue();
                            accountsDestinations.Add(destination);
                            currentPostsDestination.Add(destination.DestinationGuid);
                        }
                    }
                    if (currentPostsDestination.Count > 0)
                        // Add the splitted destinations
                        postsDestinations.Add(currentPostsDestination);
                    else
                        break;
                }

                #endregion

                destinationWithPosts = SubstitudePoststoDestinations(campaignId, campaignName, accountsDestinations, postsDestinations);
            }

            return destinationWithPosts;
        }


        private static ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>> SubstitudePoststoDestinations(
            string campaignId,
            string campaignName,
            IReadOnlyCollection<PublisherDestinationDetailsModel> givenDestinations,
            IReadOnlyList<List<string>> postsDestinations)
        {
            if (givenDestinations.Count == 0)
            {
                GlobusLogHelper.log.Info("No more unique destinations are present!");
                return new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();
            }

            var destinationWithPosts = new ConcurrentDictionary<string, Queue<PublisherDestinationDetailsModel>>();

            try
            {
                // Getting all pending post lists
                var pendingPostList = PostlistFileManager.GetAll(campaignId)
                    .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

                // Checking, If no more post available
                if (!pendingPostList.Any())
                {
                    GlobusLogHelper.log.Info($"No more unique post are available for campaign {campaignName}!");
                    return null;
                }

                //Get the general settings from bin files
                var generalSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                               (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                               .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

                // Validate the toaster notifications is needed
                if (ConstantVariable.IsToasterNotificationNeed)
                {
                    // If user needs to notify when postlists going lesser than specified post, then trigger a notifications
                    if (pendingPostList.Count < generalSettingsModel.TriggerNotificationCount &&
                        generalSettingsModel.TriggerNotificationCount > 0)
                        ToasterNotification.ShowInfomation(
                            $"{campaignName} has {pendingPostList.Count} pending post!");
                }

                // Check whether needs to shuffle postlist order
                if (generalSettingsModel.IsChooseRandomPostsChecked)
                    pendingPostList.Shuffle();


                // Validate whether all destinations contains posts or not
                if (pendingPostList.Count < postsDestinations.Count)
                    GlobusLogHelper.log.Info("Pending postlist counts are lesser than required count!");

                #region Assigning the Posts to Destinations

                for (var count = 0; count < pendingPostList.Count; count++)
                {
                    // Get the posts
                    var post = pendingPostList[count];

                    // Check whether count exceeds destinations
                    if (count >= postsDestinations.Count)
                        break;

                    // Get the destination
                    var destinations = postsDestinations[count];

                    // Iterate and assign current post to selected destinations
                    destinations.ForEach(destinationId =>
                    {
                        // get the destination
                        var destinationDetails = givenDestinations.FirstOrDefault(x => x.DestinationGuid == destinationId);

                        // check null destinations
                        if (destinationDetails == null)
                            return;

                        // Assign the posts
                        destinationDetails.PublisherPostlistModel = post;

                        // get the accounts queue
                        var accountsQueue = destinationWithPosts.GetOrAdd(destinationDetails.AccountId,
                                queue => new Queue<PublisherDestinationDetailsModel>());

                        // Add to queue
                        accountsQueue.Enqueue(destinationDetails);

                        // Append the post list details 
                        post.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                        {
                            AccountName = destinationDetails.AccountName,
                            Destination = destinationDetails.DestinationType,
                            DestinationUrl = destinationDetails.DestinationType == ConstantVariable.OwnWall
                                    ? destinationDetails.AccountName
                                    : destinationDetails.DestinationUrl,
                            Description = post.PostDescription,
                            IsPublished = ConstantVariable.Yes,
                            Successful = ConstantVariable.No,
                            PublishedDate = DateTime.Now,
                            Link = ConstantVariable.NotPublished,
                            CampaignId = campaignId,
                            CampaignName = campaignName,
                            SocialNetworks = destinationDetails.SocialNetworks,
                            AccountId = destinationDetails.AccountId,
                            ErrorDetails = ConstantVariable.NotPublished,
                        });

                    });

                    // Mark as published one
                    post.PostQueuedStatus = PostQueuedStatus.Published;

                    // Calculate already tried count
                    var triedCount =
                        post.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                    // Calculate already success count
                    var successCount =
                        post.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                    // Update the stats
                    post.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                    // Checking post expire date time
                    if (post.ExpiredTime == null)
                        post.PostRunningStatus = PostRunningStatus.Active;
                    else
                    {
                        post.PostRunningStatus = DateTime.Now > post.ExpiredTime
                            ? PostRunningStatus.Completed
                            : PostRunningStatus.Active;
                    }

                    // Update to bin file
                    PostlistFileManager.UpdatePost(campaignId, post);
                }
                #endregion

                // update stats to publisher default view
                PublisherInitialize.GetInstance.UpdatePostStatus(campaignId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return destinationWithPosts;
        }

        public static List<PublisherDestinationDetailsModel> AddPostsToDestination(ConcurrentBag<PublisherDestinationDetailsModel> destinationDetails, List<string> accountIds, string campaignId, string campaignName, int postsMaximumDestinationCount, int postsAccountDestinationLimits)
        {
            var destinationWithPosts = new List<PublisherDestinationDetailsModel>();

            var updatelock = GetPostsForPublishing.GetOrAdd(campaignId, _lock => new object());

            lock (updatelock)
            {
                // Getting all pending post lists
                var pendingPostList = PostlistFileManager.GetAll(campaignId)
                    .Where(x => x.PostQueuedStatus == PostQueuedStatus.Pending).ToList();

                // Checking, If no more post available
                if (!pendingPostList.Any())
                {
                    GlobusLogHelper.log.Info($"No more unique post are available for campaign {campaignName}!");
                    return null;
                }

                //Get the general settings from bin files
                var generalSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                              (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                              .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

                // Validate the toaster notifications is needed
                if (ConstantVariable.IsToasterNotificationNeed)
                {
                    // If user needs to notify when postlists going lesser than specified post, then trigger a notifications
                    if (pendingPostList.Count < generalSettingsModel.TriggerNotificationCount &&
                        generalSettingsModel.TriggerNotificationCount > 0)
                        ToasterNotification.ShowInfomation(
                            $"{campaignName} has {pendingPostList.Count} pending post!");
                }

                // Check whether needs to shuffle postlist order
                if (generalSettingsModel.IsChooseRandomPostsChecked)
                    pendingPostList.Shuffle();

                // if(!generalSettingsModel.IsStopRandomisingDestinationsOrder)

                var isProcessCompleted = false;

                var usedPosts = new SortedSet<string>();




                foreach (var post in pendingPostList)
                {
                    #region Destination assign without accounts limit

                    // No Accounts limit
                    if (postsAccountDestinationLimits == 0)
                    {
                        for (var initial = 0; initial < postsMaximumDestinationCount; initial++)
                        {
                            usedPosts.Add(post.PostId);

                            PublisherDestinationDetailsModel destination;

                            var isRetrieved = destinationDetails.TryTake(out destination);

                            if (!isRetrieved)
                                continue;

                            destination.PublisherPostlistModel = post;

                            destinationWithPosts.Add(destination);

                            if (destinationDetails.Count == 0)
                            {
                                isProcessCompleted = true;
                                break;
                            }
                        }
                    }

                    #endregion

                    #region Destination assign with account limits

                    else
                    {
                        var addedCount = 0;

                        foreach (var accountId in accountIds)
                        {
                            var destinations = destinationDetails.Where(x => x.AccountId == accountId)
                                .Take(postsMaximumDestinationCount).ToList();

                            foreach (var destination in destinations)
                            {
                                usedPosts.Add(post.PostId);
                                destination.PublisherPostlistModel = post;
                                destinationWithPosts.Add(destination);
                            }

                            addedCount += destinations.Count;

                            var remainingDestinations = destinationDetails.ToList().Except(destinations);

                            destinationDetails = new ConcurrentBag<PublisherDestinationDetailsModel>(remainingDestinations);

                            if (addedCount > postsMaximumDestinationCount)
                                break;

                            if (destinationDetails.Count == 0)
                            {
                                isProcessCompleted = true;
                                break;
                            }
                        }
                    }

                    #endregion

                    var allpostlists = PostlistFileManager.GetAll(campaignId);
                    allpostlists.ForEach(x =>
                    {
                        if (usedPosts.Contains(x.PostId))
                        {
                            x.PostQueuedStatus = PostQueuedStatus.Published;
                        }
                    });

                    PostlistFileManager.UpdatePostlists(campaignId, allpostlists);

                    if (isProcessCompleted)
                        return destinationWithPosts;
                }
            }

            return destinationWithPosts;
        }


        public static void StartPublishingPosts(PublisherPostlistModel post, Action startAction)
        {
            try
            {
                // Get the campaign Details
                var campaignDetails =
                    PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

                //var campaignDetails =
                //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

                // Get the specific campaign Details
                var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == post.CampaignId);

                if (specificCampaign == null)
                {
                    GlobusLogHelper.log.Info("Current post isn't register with any campaign!");
                    return;
                }

                // Validate the campaign Time
                var isStart = ValidateCampaignsTime(specificCampaign);

                if (!isStart)
                {
                    GlobusLogHelper.log.Info("Current post's campaign expired!");
                    return;
                }


                GlobusLogHelper.log.Info(Log.StartPublishing, SocialNetworks.Social, string.Empty, specificCampaign.CampaignName);

                var currentCampaignsCancallationToken = new CancellationTokenSource();
                // Registering the cancellation token if its not present
                if (!CampaignsCancellationTokens.ContainsKey(post.CampaignId))
                    CampaignsCancellationTokens.Add(post.CampaignId, currentCampaignsCancallationToken);
                else
                    // Otherwise get the cancellation token source of the campaign
                    currentCampaignsCancallationToken = CampaignsCancellationTokens[post.CampaignId];

                // Get the post fetch details
                var publisherPostFetchModel =
                    GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                        .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == post.CampaignId);

                int publishCount;

                // Fetch the advanced settings
                var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == post.CampaignId) ??
                                       new GeneralModel();


                var deletedDestinationCount = 0;

                // Iterate selected destinations
                publisherPostFetchModel?.SelectedDestinations.ToList().ForEach(destinationId =>
                {
                    // Get destination details
                    var destinationDetails = BinFileHelper.GetSingleDestination(destinationId);

                    // If destination is aleady deleted, process will give null from above statement, if its null increase destination count
                    if (destinationDetails == null)
                        deletedDestinationCount++;

                    destinationDetails?.AccountsWithNetwork.ForEach(networkWithAccount =>
                    {
                        // Check whether current accounts network present or not
                        if (!SocinatorInitialize.IsNetworkAvailable(networkWithAccount.Key))
                        {
                            GlobusLogHelper.log.Info($"You don't have a permission to run with {networkWithAccount.Key} network, please purchase !");
                            return;
                        }

                        // Get the all selected group destinations
                        var selectedGroupDestinations =
                            destinationDetails.AccountGroupPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        // Get all pages Or Boards destinations
                        var selectedPageOrBoardDestinations =
                            destinationDetails.AccountPagesBoardsPair.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        // Get all custom destinations
                        var selectedCustomDestinations =
                            destinationDetails.CustomDestinations.Where(x => x.Key == networkWithAccount.Value).Select(x => x.Value).ToList();

                        // Is validate Own wall selected or not
                        var isPublishOnOwnWall =
                            destinationDetails.PublishOwnWallAccount.Any(x => x == networkWithAccount.Value);

                        // Calculate the publishing count
                        publishCount = selectedGroupDestinations.Count + selectedPageOrBoardDestinations.Count +
                                       selectedCustomDestinations.Count;

                        if (isPublishOnOwnWall)
                            publishCount++;

                        if (publishCount <= 0)
                            return;

                        // Start updating publisher Intialize count of manage post
                        startAction.Invoke();

                        // Get the job process factory object
                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(networkWithAccount.Key)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(post.CampaignId, networkWithAccount.Value,
                                selectedGroupDestinations, selectedPageOrBoardDestinations,
                                selectedCustomDestinations, isPublishOnOwnWall, currentCampaignsCancallationToken);

                        // Call start publishing
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

        /// <summary>
        /// Check whether start date is greater than or equal to today and check end date is already expire or not
        /// </summary>
        /// <param name="specificCampaign">Campaign Status model</param>
        /// <returns></returns>
        private static bool ValidateCampaignsTime(PublisherCampaignStatusModel specificCampaign)
        {
            var isStart = true;

            // Check start time is equal to null or not
            if (specificCampaign.StartDate != null)
            {
                // Compare with today
                if (!(DateTime.Now >= specificCampaign.StartDate))
                    isStart = false;
            }
            // Check end time is equal to null or not
            if (specificCampaign.EndDate != null)
            {
                // Compare with today
                if (!(DateTime.Now <= specificCampaign.EndDate))
                    isStart = false;
            }

            return isStart;
        }

        /// <summary>
        /// Stop publishing campaigns
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void StopPublishingPosts(string campaignId)
        {
            try
            {
                // Get the cancellation token from campaigns
                var cancellationToken = CampaignsCancellationTokens.FirstOrDefault(x => x.Key == campaignId);

                // Cancel the token
                cancellationToken.Value.Cancel();

                // After cancelling remove the token sources from collections
                if (CampaignsCancellationTokens.ContainsKey(campaignId))
                {
                    CampaignsCancellationTokens.Remove(campaignId);
                    var deletedList = new LinkedList<Action>();
                    PublisherActionList.TryRemove(campaignId, out deletedList);
                    DecreasePublishingCount(campaignId);
                }

                // Call to stop already scheduled Jobs
                StopScheduledPublisher(campaignId);
            }
            catch (Exception ex)
            {
                // Check whether campaign already started or nor
                var specificCampaign = PublisherInitialize.GetInstance.GetSavedCampaigns().ToList().FirstOrDefault(x => x.CampaignId == campaignId);
                if (specificCampaign != null)
                    ex.DebugLog($"Campaign : {specificCampaign.CampaignName} not started before!");
            }
        }

        /// <summary>
        /// Enable the delete option for published post
        /// </summary>
        /// <param name="postDeletionModel">Deletion post models</param>
        public static void EnableDeletePost(PostDeletionModel postDeletionModel)
        {
            // Add into bin files
            GenericFileManager.AddModule(postDeletionModel,
                ConstantVariable.GetDeletePublisherPostModel);

            // Schedule delete post itmes
            DeletePublishedPost(postDeletionModel);
        }

        /// <summary>
        /// Delete published post at specific time
        /// </summary>
        /// <param name="postDeletionModel"></param>
        public static void DeletePublishedPost(PostDeletionModel postDeletionModel)
        {
            // Check whether network present or not
            if (FeatureFlags.IsNetworkAvailable(postDeletionModel.Networks))
            {
                // Add into job process
                JobManager.AddJob(() =>
                    {
                        // Get the publisher Job Process factory
                        var publisherJobProcess = PublisherInitialize.GetPublisherLibrary(postDeletionModel.Networks)
                            .GetPublisherCoreFactory()
                            .PublisherJobFactory.Create(postDeletionModel.CampaignId, postDeletionModel.AccountId, null,
                                null, null, false, new CancellationTokenSource());

                        // Call is delete options
                        if (publisherJobProcess.DeletePost(postDeletionModel.PublishedIdOrUrl))
                        {
                            // If successfully deleted , update the details
                            publisherJobProcess.UpdatePostWithDeletion(postDeletionModel.DestinationUrl,
                                postDeletionModel.PostId);

                            // Make already deleted true
                            postDeletionModel.IsDeletedAlready = true;

                            // Get deletion model
                            var allDeletionList =
                                GenericFileManager.GetModuleDetails<PostDeletionModel>(ConstantVariable
                                    .GetDeletePublisherPostModel);

                            // Find the index of particular published Id
                            var index = allDeletionList.FindIndex(x =>
                                x.PublishedIdOrUrl == postDeletionModel.PublishedIdOrUrl);

                            // Update bin file objects
                            allDeletionList[index].IsDeletedAlready = true;

                            // save the updated details into bin files
                            GenericFileManager.UpdateModuleDetails(allDeletionList,
                                ConstantVariable.GetDeletePublisherPostModel);
                        }

                    },
                    s => s.WithName($"{postDeletionModel.CampaignId}- Delete Posts -{ConstantVariable.GetDate()}")
                        .ToRunOnceAt(postDeletionModel.DeletionTime));
            }
        }

        /// <summary>
        /// Publish now by campaign Id
        /// </summary>
        /// <param name="campaignId">Campaign Id</param>
        public static void SchedulePublishNowByCampaign(string campaignId)
        {
            // get the all campaigns which should be present in between 
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            // get the all campaigns
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();
            // Filter with current campaign
            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            // Validate the campaigns Times
            if (specificCampaign != null && ValidateCampaignsTime(specificCampaign))
            {
                // Is Rotate day has been selected
                if (specificCampaign.IsRotateDayChecked)
                    // Call to start publishing
                    StartPublishingPosts(specificCampaign);
                else
                {
                    // Check whether today is selected or not
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    // Call to start publishing
                    StartPublishingPosts(specificCampaign);
                }
            }
        }

        /// <summary>
        /// Todays publishing scheduler
        /// </summary>
        public static void ScheduleTodaysPublisher()
        {
            // get the all campaigns which should active 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => x.Status == PublisherCampaignStatus.Active).ToList();

            // Iterate campaigns 
            campaignDetails.ForEach(campaign =>
            {
                // Validate the start and end time of the campaign
                if (!ValidateCampaignsTime(campaign))
                    return;

                // Is Rotate day has been selected
                if (campaign.IsRotateDayChecked)
                    // Call to start publishing
                    SchedulePublisher(campaign);
                else
                {
                    // Check whether today is selected or not
                    var isCampaignSelected = campaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    // Call to start publishing
                    SchedulePublisher(campaign);
                }
            });
        }


        /// <summary>
        /// Today Publishing scheduler by Campaign Id
        /// </summary>
        /// <param name="campaignId"> Campaign Id</param>
        public static void ScheduleTodaysPublisherByCampaign(string campaignId)
        {
            //var campaignDetails =
            //    PublisherInitialize.GetInstance.GetSavedCampaigns().Where(x => DateTime.Now >= x.StartDate && DateTime.Now <= x.EndDate).ToList();

            // get the all campaigns 
            var campaignDetails =
                PublisherInitialize.GetInstance.GetSavedCampaigns().ToList();

            var specificCampaign = campaignDetails.FirstOrDefault(x => x.CampaignId == campaignId);

            // Validate the start and end time of the campaign
            if (specificCampaign != null && ValidateCampaignsTime(specificCampaign))
            {
                if (specificCampaign.IsRotateDayChecked)
                    // Call to start publishing
                    SchedulePublisher(specificCampaign);
                else
                {
                    // Check whether today is selected or not
                    var isCampaignSelected = specificCampaign.ScheduledWeekday.FirstOrDefault(x => x.Content == DateTime.Now.DayOfWeek.ToString() && x.IsContentSelected);
                    if (isCampaignSelected == null)
                        return;
                    // Call to start publishing
                    SchedulePublisher(specificCampaign);
                }
            }
        }

        /// <summary>
        /// Schedule publisher
        /// </summary>
        /// <param name="campaign"></param>
        private static void SchedulePublisher(PublisherCampaignStatusModel campaign)
        {
            #region Schedule

            // stop already running campaigns
            StopScheduledPublisher(campaign.CampaignId);

            // Get the specific running time of a campaign
            var timeRange = campaign.SpecificRunningTime;

            // Check whether random time for every day has selected
            if (campaign.IsRandomRunningTime)
            {
                if (campaign.UpdatedTime.Date != DateTime.Today)
                    // Otherwise fetch random intervals
                    timeRange = GenerateRandomIntervals(campaign.MaximumTime, campaign.TimeRange);
            }

            // Iterate running times 
            timeRange.ForEach(runningTime =>
                {
                    // Make start time
                    var startTime = DateTime.Today.Add(new TimeSpan(runningTime.Hours, runningTime.Minutes, runningTime.Seconds));

                    // If start time is greater than current time
                    if (startTime > DateTime.Now)
                    {
                        // Generate job name
                        var addJobName = $"{campaign.CampaignId}-{ConstantVariable.GetDate()}";

                        // Add into scheduled lsit
                        PublisherScheduledList.Add(addJobName);

                        // Add job manager
                        JobManager.AddJob(() =>
                        {
                            // Call the start publishing
                            StartPublishingPosts(campaign);
                        }, s => s.WithName(addJobName).ToRunOnceAt(startTime));

                        // Get the advanced settings details of an campaigns
                        var advancedSettings = GenericFileManager.GetModuleDetails<GeneralModel>(ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social)).FirstOrDefault(x => x.CampaignId == campaign.CampaignId);

                        // Check whether campaign destination time out options
                        if (advancedSettings?.DestinationTimeout > 0)
                        {
                            // Generate the job name for stopping campaigns
                            var stopJobName = $"{campaign.CampaignId}-StopRunningDueToTimeOut";

                            // Add into schedule list
                            PublisherScheduledList.Add(stopJobName);

                            // Calculate stopping time
                            var stopTime = DateTime.Now.AddMinutes(advancedSettings.DestinationTimeout);

                            // Add job process for stop publishing after some x minutes
                            JobManager.AddJob(() =>
                            {
                                // Call stop publishing
                                StopPublishingPosts(campaign.CampaignId);
                            }, s => s.WithName(stopJobName).ToRunOnceAt(stopTime));
                        }
                    }
                });

            #endregion
        }

        /// <summary>
        /// Generate random running times
        /// </summary>
        /// <param name="maxCount">max running time count</param>
        /// <param name="timeRange">Time range</param>
        /// <returns></returns>
        public static List<TimeSpan> GenerateRandomIntervals(int maxCount, TimeRange timeRange)
        {
            // Initialize time span
            var timer = new List<TimeSpan>();

            // Random objects
            var random = new Random();

            // start time
            var startTime = timeRange.StartTime;

            // End time
            var endTime = timeRange.EndTime;

            // Iterate untill getting required amount of time range
            for (int countIndex = 0; countIndex < maxCount; countIndex++)
            {
                timer.Add(DateTimeUtilities.GetRandomTime(startTime, endTime, random));
            }
            return timer;
        }

        /// <summary>
        /// Stop Publishing scheduler
        /// </summary>
        /// <param name="campaignId"></param>
        private static void StopScheduledPublisher(string campaignId)
        {
            // Get the current campaings schedule lists
            var currentCampaignsItems = PublisherScheduledList.Where(x => x.Contains(campaignId)).ToList();

            // Iterate one by one to remove 
            currentCampaignsItems.ForEach(name =>
            {
                // Remove schedule list
                JobManager.RemoveJob(name);
                PublisherScheduledList.Remove(name);
            });
        }

        /// <summary>
        /// Update new groups for a destinations
        /// </summary>
        public static void UpdateNewGroupList()
        {
            // Get all destinations details
            var destinations = ManageDestinationFileManager.GetAll();

            // Iterate destinations
            destinations.ForEach(x =>
            {
                if (x.IsAddNewGroups)
                {
                    // Update groups
                    PublisherInitialize.UpdateNewGroups(x.DestinationId);
                }
            });
        }

    }
}