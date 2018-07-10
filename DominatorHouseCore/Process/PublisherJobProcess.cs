using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DominatorHouseCore.Diagnostics;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Enums.SocioPublisher;
using DominatorHouseCore.FileManagers;
using DominatorHouseCore.LogHelper;
using DominatorHouseCore.Models;
using DominatorHouseCore.Models.Publisher;
using DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Patterns;
using DominatorHouseCore.Utility;

namespace DominatorHouseCore.Process
{
    public abstract class PublisherJobProcess
    {
        #region Constructor

        public PublisherJobProcess(string campaignId,
            string accountId,
            SocialNetworks network,
            List<string> groupDestinationLists,
            List<string> pageDestinationList,
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource camapignCancellationToken)
        {
            CampaignId = campaignId;

            Network = network;

            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                       (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            AccountModel = AccountsFileManager.GetAccountById(accountId);

            PageDestinationList = pageDestinationList;

            GroupDestinationList = groupDestinationLists;

            CustomDestinationList = customDestinationModels;

            IsPublishOnOwnWall = isPublishOnOwnWall;

            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = camapignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token, CurrentJobCancellationToken.Token);

            var campaign = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == CampaignId);

            CampaignName = campaign?.CampaignName;

        }

        #endregion

        #region Properties

        public string CampaignId { get; set; }

        public string CampaignName { get; set; }

        public SocialNetworks Network { get; set; }

        public JobConfigurationModel JobConfigurations { get; set; }

        public OtherConfigurationModel OtherConfiguration { get; set; }

        public GeneralModel GeneralSettingsModel { get; set; }

        private static readonly object SyncJobProcess = new object();

        public DominatorAccountModel AccountModel { get; set; }

        public List<string> GroupDestinationList { get; set; }

        public List<string> PageDestinationList { get; set; }

        public List<PublisherCustomDestinationModel> CustomDestinationList { get; set; }

        public bool IsPublishOnOwnWall { get; set; }

        public CancellationTokenSource CampaignCancellationToken { get; set; }

        public CancellationTokenSource CurrentJobCancellationToken { get; set; }

        public CancellationTokenSource CombinedCancellationToken { get; set; }

        public virtual bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        public virtual bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        public virtual bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        public virtual bool PublishOnCustomDestination(string accountId,
            PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        public int PublishedCount { get; set; }

        #endregion

        #region Methods

        protected abstract bool ValidateNetworksSettings(string campaign);

        public virtual bool DeletePost(string postId) => true;

        public void StartPublishing(int maximumPostCount, bool isRunParallel)
        {
            lock (SyncJobProcess)
            {
                if (isRunParallel)
                {
                    ThreadFactory.Instance.Start(() =>
                    {
                        Publish(maximumPostCount);                       
                        PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");
                        PublishScheduler.DecreasePublishingCount(CampaignId);
                        GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,CampaignName);                        
                    }, CampaignCancellationToken.Token);
                }
                else
                {
                    Publish(maximumPostCount);                    
                    PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");
                    PublishScheduler.DecreasePublishingCount(CampaignId);
                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                }
                    
            }
        }

        private void Publish(int maximumPostCount)
        {
            PublishedCount = 0;
            var isReachedMaximumCount = false;
            
            try
            {
                GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork,
                    AccountModel.AccountBaseModel.UserName, CampaignName);

                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == CampaignId).ToList();

                var usedDestination = publishedDetails.Select(x => x.DestinationUrl).ToList();

                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue, JobConfigurations.PostRange.StartValue);

                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    #region Publish on Groups

                  
                    foreach (var groupUrl in GroupDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(groupUrl))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var post = GetPostModel("Group", groupUrl);

                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost,AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,"group", groupUrl);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, $"group [{groupUrl}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnGroups(AccountModel.AccountId, groupUrl, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion

                    #region Publish on Pages

                    if (isReachedMaximumCount)
                        return;

                    foreach (var pageUrl in PageDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(pageUrl))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var post = GetPostModel("Page", pageUrl);

                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "page", pageUrl);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, $"page [{pageUrl}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnPages(AccountModel.AccountId, pageUrl, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion

                    #region Custom Destination

                    if (isReachedMaximumCount )
                        return;

                    foreach (var customList in CustomDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(customList.DestinationValue))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName,
                            $"{customList.DestinationType} [{customList.DestinationValue}]");

                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, customList.DestinationType.ToLower(), customList.DestinationValue);
                            continue;
                        }

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnCustomDestination(AccountModel.AccountId, customList, post,
                            !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }
                else
                {
                    #region Shuffle the groups and pages

                    var allGroupsPages = new Dictionary<string, string>();

                    GroupDestinationList.Shuffle();
                    PageDestinationList.Shuffle();

                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });

                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    foreach (var destination in allGroupsPages)
                    {
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(destination.Key))
                            continue;

                        if (destination.Value == "Group")
                        {
                            var post = GetPostModel("Group", destination.Key);

                            if (post == null)
                            {
                                GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "group", destination.Key);

                                continue;
                            }

                            GlobusLogHelper.log.Info(Log.StartPublishing,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, $"group [{destination.Key}]");

                            PublishedCount++;

                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            PublishOnGroups(AccountModel.AccountId, destination.Key, post,
                                !isReachedMaximumCount);

                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }
                        else
                        {
                            var post = GetPostModel("Page", destination.Key);

                            if (post == null)
                            {
                                GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "page", destination.Key);
                                continue;
                            }

                            if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(destination.Key))
                                continue;

                            GlobusLogHelper.log.Info(Log.StartPublishing,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, $"page [{destination.Key}]");

                            PublishedCount++;

                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            PublishOnPages(AccountModel.AccountId, destination.Key, post,
                                isReachedMaximumCount);

                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }

                    }

                    #endregion

                    #region Shuffle custom destination

                    if (isReachedMaximumCount)
                        return;

                    CustomDestinationList.Shuffle();

                    foreach (var customList in CustomDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, customList.DestinationType.ToLower(), customList.DestinationValue);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName,
                            $"{customList.DestinationType} [{customList.DestinationValue}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnCustomDestination(AccountModel.AccountId, customList, post,
                            !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion
                }

                #region Own Wall

                if (IsPublishOnOwnWall)
                {

                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    if (isReachedMaximumCount)
                        return;

                    if (GeneralSettingsModel.IsDeselectUsedDestination &&
                        usedDestination.Contains(AccountModel.AccountBaseModel.UserName))
                        return;

                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountBaseModel.UserName);

                    if (ownWallpost == null)
                        return;

                    GlobusLogHelper.log.Info(Log.StartPublishing,
                        AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Own wall");

                    PublishedCount++;

                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    PublishOnOwnWall(AccountModel.AccountId, ownWallpost, !isReachedMaximumCount);


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


        public void StartPublishing(PublisherPostlistModel post, int count, bool isStartParallel)
        {
            lock (SyncJobProcess)
            {
                if (isStartParallel)
                {
                    ThreadFactory.Instance.Start(() =>
                     {
                         PublishWithDirectPost(post, count);
                         GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                     }, CampaignCancellationToken.Token);
                }
                else
                {
                    PublishWithDirectPost(post, count);
                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                }
            }
        }

        private void PublishWithDirectPost(PublisherPostlistModel post, int maximumPostCount)
        {
            PublishedCount = 0;
            var isReachedMaximumCount = false;

            try
            {
                GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);

                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == CampaignId).ToList();

                var usedDestination = publishedDetails.Select(x => x.DestinationUrl).ToList();

                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue, JobConfigurations.PostRange.StartValue);

                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    #region Publish on Groups

                    foreach (var groupUrl in GroupDestinationList)
                    {
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        if (!ValidateNetworkAdvancedSettings(post, "Group", groupUrl, true))
                            continue;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(groupUrl))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{groupUrl}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnGroups(AccountModel.AccountId, groupUrl, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion

                    #region Publish on Pages

                    if (isReachedMaximumCount)
                        return;

                    foreach (var pageUrl in PageDestinationList)
                    {
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        if (!ValidateNetworkAdvancedSettings(post, "Page", pageUrl, true))
                            continue;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(pageUrl))
                            continue;

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{pageUrl}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnPages(AccountModel.AccountId, pageUrl, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion

                    #region Publish on Custom destination


                    if (isReachedMaximumCount)
                        return;

                    foreach (var customList in CustomDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (!ValidateNetworkAdvancedSettings(post, customList.DestinationType, customList.DestinationValue, true))
                            continue;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType} [{customList.DestinationValue}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnCustomDestination(AccountModel.AccountId, customList, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }
                else
                {
                    #region Shuffle Groups and pages

                    var allGroupsPages = new Dictionary<string, string>();

                    GroupDestinationList.Shuffle();
                    PageDestinationList.Shuffle();

                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });

                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    foreach (var x in allGroupsPages)
                    {
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(x.Key))
                            continue;

                        if (x.Value == "Group")
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Group", x.Key, true))
                                continue;

                            PublishedCount++;

                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{x.Key}]");

                            PublishOnGroups(AccountModel.AccountId, x.Key, post, !isReachedMaximumCount);

                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }
                        else
                        {
                            if (!ValidateNetworkAdvancedSettings(post, "Page", x.Key, true))
                                continue;

                            PublishedCount++;

                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{x.Key}]");

                            PublishOnPages(AccountModel.AccountId, x.Key, post, !isReachedMaximumCount);

                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }

                    }

                    #endregion

                    #region Custom Destination

                    if (isReachedMaximumCount)
                        return;

                    CustomDestinationList.Shuffle();

                    foreach (var customList in CustomDestinationList)
                    {
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        if (isReachedMaximumCount)
                            break;

                        if (!ValidateNetworkAdvancedSettings(post, customList.DestinationType, customList.DestinationValue, true))
                            continue;

                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType}  [{customList.DestinationValue}]");

                        PublishedCount++;

                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        PublishOnCustomDestination(AccountModel.AccountId, customList, post, !isReachedMaximumCount);

                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }

                #region OwnWall

                if (IsPublishOnOwnWall)
                {
                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    if (isReachedMaximumCount)
                        return;

                    if (GeneralSettingsModel.IsDeselectUsedDestination &&
                        usedDestination.Contains(AccountModel.AccountBaseModel.UserName))
                        return;

                    PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    if (!ValidateNetworkAdvancedSettings(post, "OwnWall", AccountModel.AccountBaseModel.UserName,
                        true))
                        return;

                    GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Own wall");

                    PublishedCount++;

                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    PublishOnOwnWall(AccountModel.AccountId, post, false);

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

        public void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel posts, string publishedUrl)
        {
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));

                if (postIndex == -1)
                    return;

                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
                post.LstPublishedPostDetailsModels[postIndex].Link = publishedUrl;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = ConstantVariable.NoError;
                PostlistFileManager.UpdatePost(CampaignId, post);
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                GenericFileManager.AddModule(post.LstPublishedPostDetailsModels[postIndex], ConstantVariable.GetPublishedSuccessDetails);

            }
        }

        public void UpdatePostWithFailed(string destinationUrl, PublisherPostlistModel posts, string errorMessage)
        {
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(
                    post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));

                if (postIndex == -1)
                    return;

                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.No;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = errorMessage;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;

                PostlistFileManager.UpdatePost(CampaignId, post);
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
            }
        }


        public PublisherPostlistModel PerformGeneralSettings(PublisherPostlistModel givenPostModel)
        {

            var postModelWithGeneralSettings = givenPostModel.DeepClone();

            #region Fetch Post media list

            if (GeneralSettingsModel.IsChooseSingleRandomImageChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                {
                    var randomNumber = RandomUtilties.GetRandomNumber(postModelWithGeneralSettings.MediaList.Count, 0);
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[randomNumber] };
                }
            }
            else if (GeneralSettingsModel.IsChooseOnlyFirstImageChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[0] };
            }
            else if (GeneralSettingsModel.IsChooseBetweenChecked)
            {
                if (givenPostModel.MediaList.Count > 0)
                {
                    var randomNumber = RandomUtilties.GetRandomNumber(GeneralSettingsModel.ChooseBetween.EndValue, GeneralSettingsModel.ChooseBetween.StartValue);
                    if (randomNumber < givenPostModel.MediaList.Count)
                    {
                        givenPostModel.MediaList.Shuffle();
                        postModelWithGeneralSettings.MediaList = new ObservableCollection<string>(givenPostModel.MediaList.Take(randomNumber));
                    }
                }
            }

            var removeVideoExtension = new ObservableCollection<string>();

            foreach (var media in postModelWithGeneralSettings.MediaList)
            {
                removeVideoExtension.Add(media.Replace(ConstantVariable.VideoToImageConvertFileName, string.Empty));
            }

            postModelWithGeneralSettings.MediaList = removeVideoExtension;

            #endregion

            #region Spin Text

            postModelWithGeneralSettings.PostDescription =
                SpinTexHelper.GetSpinText(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Macro Substitution

            postModelWithGeneralSettings.PostDescription =
                MacrosHelper.SubstituteMacroValues(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Remove link

            if (GeneralSettingsModel.IsRemoveLinkFromPostsChecked)
            {
                postModelWithGeneralSettings.PostDescription = Utilities.RemoveUrls(givenPostModel.PostDescription);
            }

            #endregion

            #region Shorten Url

            if (OtherConfiguration.IsShortenURLsChecked)
            {
                postModelWithGeneralSettings.PostDescription = Utilities.ReplaceWithShortenUrl(postModelWithGeneralSettings.PostDescription);
            }

            #endregion

            #region Adding Signature

            if (OtherConfiguration.IsEnableSignatureChecked)
            {
                postModelWithGeneralSettings.PostDescription = postModelWithGeneralSettings.PostDescription + "\r\n" +
                                                  OtherConfiguration.SignatureText;
            }

            #endregion
          
            return postModelWithGeneralSettings;

        }

        public void Stop()
        {
            try
            {
                CurrentJobCancellationToken.Cancel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        public void DelayBeforeNextPublish()
        {

            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenPost.EndValue, JobConfigurations.DelayBetweenPost.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, delay);

            Thread.Sleep(delay * 1000);
        }

        public void DelayBetweenMultiPublish()
        {

            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetween.EndValue, JobConfigurations.DelayBetween.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenMultiPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, delay);

            Thread.Sleep(delay * 1000 * 60);
        }

        public PublisherPostlistModel GetPostModel(string destination, string destinationUrl)
        {
            try
            {
                CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                var pendingPostList = PostlistFileManager.GetAll(CampaignId).Where(x => x.PostQueuedStatus != PostQueuedStatus.Draft).ToList();

                if (!pendingPostList.Any())
                {
                    GlobusLogHelper.log.Info($"No more post are available for campaign {CampaignName}!");
                    return null;
                }

                if (GeneralSettingsModel.IsWhenPublishingSendOnePostChecked)
                    pendingPostList = pendingPostList.Where(x => x.LstPublishedPostDetailsModels.Count == 0).ToList();
                else
                    pendingPostList = (from posts in pendingPostList
                                       let successfulDestinations = posts.LstPublishedPostDetailsModels.Where(x => x.Successful == ConstantVariable.Yes).Select(x => x.DestinationUrl)
                                       where !successfulDestinations.Contains(destinationUrl)
                                       select posts).ToList();


                var iterationCount = 0;

                while (true)
                {
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    if (iterationCount >= pendingPostList.Count)
                        break;

                    var filterPostModel = GeneralSettingsModel.IsChooseRandomPostsChecked ?
                        pendingPostList[RandomUtilties.GetRandomNumber(pendingPostList.Count - 1, 0)] :
                        pendingPostList.FirstOrDefault(x => x.PostId != null);

                    if (ValidateNetworkAdvancedSettings(filterPostModel, destination, destinationUrl))
                    {
                        return filterPostModel;
                    }

                    iterationCount++;
                }
            }
            catch (OperationCanceledException)
            {
                throw new OperationCanceledException("Cancellation Requested!");
            }
            catch (AggregateException ae)
            {
                foreach (var e in ae.InnerExceptions)
                {
                    if (e is TaskCanceledException || e is OperationCanceledException)
                        throw new AggregateException("Cancellation requested before task completion!");
                    else
                        throw new AggregateException(e.StackTrace + e.Message);
                }
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return null;
        }

        public bool ValidateNetworkAdvancedSettings(PublisherPostlistModel filterPostModel, string destination, string destinationUrl, bool isDirectPost = false)
        {
            try
            {
                var post = PostlistFileManager.GetByPostId(CampaignId, filterPostModel.PostId);

                var postTriedAndSuccessdestinations = post?.LstPublishedPostDetailsModels.ToList();

                if (postTriedAndSuccessdestinations == null)
                    return false;

                var successfulDestinations = postTriedAndSuccessdestinations.Where(x => x.Successful == ConstantVariable.Yes).Select(x => x.DestinationUrl).ToList();

                if (successfulDestinations.Contains(destinationUrl))
                {
                    GlobusLogHelper.log.Info(destination == "OwnWall"
                        ? string.Format(Log.AlreadyPublishedOnOwnWall, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName)
                        : string.Format(Log.AlreadyPublishedOnDestination, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, destination, destinationUrl));
                    return false;
                }

                var allDestinations = postTriedAndSuccessdestinations.Select(x => x.DestinationUrl);

                if (!allDestinations.Contains(destinationUrl))
                {
                    post.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                    {
                        AccountName = AccountModel.AccountBaseModel.UserName,
                        Destination = destination,
                        DestinationUrl = destination == "OwnWall" ? AccountModel.AccountBaseModel.UserName : destinationUrl,
                        Description = post.PostDescription,
                        IsPublished = ConstantVariable.Yes,
                        Successful = ConstantVariable.No,
                        PublishedDate = DateTime.Now,
                        Link = ConstantVariable.NotPublished,
                        CampaignId = CampaignId,
                        CampaignName = CampaignName,
                        AccountId = AccountModel.AccountBaseModel.AccountId,
                        ErrorDetails = ConstantVariable.NotPublished,
                    });
                }

                post.PostQueuedStatus = PostQueuedStatus.Published;

                var triedCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.IsPublished == ConstantVariable.Yes);

                var successCount =
                    post.LstPublishedPostDetailsModels.Count(x => x.Successful == ConstantVariable.Yes);

                post.PublishedTriedAndSuccessStatus = $"{triedCount}/{successCount}";

                post.PostRunningStatus = DateTime.Now > post.ExpiredTime
                    ? PostRunningStatus.Completed
                    : PostRunningStatus.Active;

                PostlistFileManager.UpdatePost(CampaignId, post);

                if (post.PostRunningStatus == PostRunningStatus.Completed)
                {
                    if (isDirectPost)
                        GlobusLogHelper.log.Info(Log.PostExpired, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName);
                    return false;
                }

                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
            return true;
        }

        #endregion
    }
}