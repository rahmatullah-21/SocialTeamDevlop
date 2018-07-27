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

        protected PublisherJobProcess(string campaignId, 
            string accountId, 
            SocialNetworks network,
            List<string> groupDestinationLists, 
            List<string> pageDestinationList, 
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken)
        {
            // assign campaign Id
            CampaignId = campaignId;

            // assign network 
            Network = network;

            //Get the general settings from bin files
            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                       (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            // Get the full account details from account Id
            AccountModel = AccountsFileManager.GetAccountById(accountId);

            PageDestinationList = pageDestinationList;

            GroupDestinationList = groupDestinationLists;

            CustomDestinationList = customDestinationModels;

            IsPublishOnOwnWall = isPublishOnOwnWall;

            // Get the campaigns full model
            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = campaignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            //Linked the job configuration's cancellation token source with campaign's cancellation token
            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token, CurrentJobCancellationToken.Token);

            // Get the fetcher details, its useful for getting campaigns Name
            var campaign = GenericFileManager.GetModuleDetails<PublisherPostFetchModel>(ConstantVariable
                .GetPublisherPostFetchFile).FirstOrDefault(x => x.CampaignId == CampaignId);

            CampaignName = campaign?.CampaignName;

        }


        protected PublisherJobProcess(string campaignId, string campaignName, string accountId,SocialNetworks network,
            IEnumerable<PublisherDestinationDetailsModel> destinationDetails,
            CancellationTokenSource campaignCancellationToken)
        {

            // assign campaign Id
            CampaignId = campaignId;

            // assign network 
            Network = network;

            //Get the general settings from bin files
            GeneralSettingsModel = GenericFileManager.GetModuleDetails<GeneralModel>
                                       (ConstantVariable.GetPublisherOtherConfigFile(SocialNetworks.Social))
                                       .FirstOrDefault(x => x.CampaignId == campaignId) ?? new GeneralModel();

            // Get the full account details from account Id
            AccountModel = AccountsFileManager.GetAccountById(accountId);

            PublisherDestinationDetailsModels = destinationDetails.ToList();

               // Get the campaigns full model
            var publisherCampaign =
                GenericFileManager.GetModuleDetails<PublisherCreateCampaignModel>(ConstantVariable
                    .GetPublisherCampaignFile()).FirstOrDefault(x => x.CampaignId == CampaignId);

            JobConfigurations = publisherCampaign?.JobConfigurations;

            OtherConfiguration = publisherCampaign?.OtherConfiguration;

            CampaignCancellationToken = campaignCancellationToken;

            CurrentJobCancellationToken = new CancellationTokenSource();

            //Linked the job configuration's cancellation token source with campaign's cancellation token
            CombinedCancellationToken = CancellationTokenSource.CreateLinkedTokenSource(CampaignCancellationToken.Token, CurrentJobCancellationToken.Token);

            CampaignName = campaignName;
        }

        #endregion

        #region Properties

        /// <summary>
        /// To specify the campaign Id
        /// </summary>
        public string CampaignId { get; set; }

        /// <summary>
        /// To Specify the campaign Name
        /// </summary>
        public string CampaignName { get; set; }

        /// <summary>
        /// To Specify the social network
        /// </summary>
        public SocialNetworks Network { get; set; }

        /// <summary>
        /// To Hold Job's configuration settings
        /// </summary>
        public JobConfigurationModel JobConfigurations { get; set; }

        /// <summary>
        /// To holds other configurations settings
        /// </summary>
        public OtherConfigurationModel OtherConfiguration { get; set; }

        /// <summary>
        /// To holds advanced general settings of the campaign
        /// </summary>
        public GeneralModel GeneralSettingsModel { get; set; }

        /// <summary>
        /// lock for job process
        /// </summary>
        private static readonly object SyncJobProcess = new object();

        /// <summary>
        /// Current account details
        /// </summary>
        public DominatorAccountModel AccountModel { get; set; }

        /// <summary>
        /// Groups destinations Collection
        /// </summary>
        public List<string> GroupDestinationList { get; set; }

        /// <summary>
        /// Pages destination collections
        /// </summary>
        public List<string> PageDestinationList { get; set; }


        public List<PublisherDestinationDetailsModel> PublisherDestinationDetailsModels { get; set; } 

        /// <summary>
        /// Custom destination collections
        /// </summary>
        public List<PublisherCustomDestinationModel> CustomDestinationList { get; set; }

        /// <summary>
        /// Is need to publish on own wall
        /// </summary>
        public bool IsPublishOnOwnWall { get; set; }

        /// <summary>
        /// Campaign's cancellation token
        /// </summary>
        public CancellationTokenSource CampaignCancellationToken { get; set; }

        /// <summary>
        /// Job's cancellation token
        /// </summary>
        public CancellationTokenSource CurrentJobCancellationToken { get; set; }

        /// <summary>
        /// Combined cancellation token source for campaigns and jobs
        /// </summary>
        public CancellationTokenSource CombinedCancellationToken { get; set; }

        /// <summary>
        /// The method for override to publishing to group for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="groupUrl">Group Url</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel"/></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        /// <returns></returns>
        public virtual bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;


        /// <summary>
        /// The method for override to publishing to page for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="pageUrl">Page Url</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel"/></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;


        /// <summary>
        /// The method for override to publishing to own profile for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel"/></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        /// <summary>
        /// The method for override to publishing to custom destination for an accounts
        /// </summary>
        /// <param name="accountId">Account Id</param>
        /// <param name="customDestinationModel">custom destination tyoe and Url <see cref="PublisherCustomDestinationModel"/></param>
        /// <param name="postDetails">Publishing post details<see cref="PublisherPostlistModel"/></param>
        /// <param name="isDelayNeed">Specify is delay needed after success/failed post</param>
        public virtual bool PublishOnCustomDestination(string accountId,
            PublisherCustomDestinationModel customDestinationModel, PublisherPostlistModel postDetails, bool isDelayNeed = true) => false;

        /// <summary>
        /// To specify already published count
        /// </summary>
        public int PublishedCount { get; set; }

        #endregion

        #region Methods

        /// <summary>
        /// To validate the networks settings for each network
        /// </summary>
        /// <param name="campaign">Campaign details</param>
        /// <returns></returns>
        protected abstract bool ValidateNetworksSettings(string campaign);

        /// <summary>
        /// To delete a published post after x hours
        /// </summary>
        /// <param name="postId">Published Post Id/Post Url</param>
        /// <returns></returns>
        public virtual bool DeletePost(string postId)
        {
            return true;
        }

        /// <summary>
        /// To Start publishing a post for an account
        /// </summary>
        /// <param name="maximumPostCount">Maximum posting count for a current campaign</param>
        /// <param name="isRunParallel">Specify whether need to run on parallely or not</param>
        public void StartPublishing(int maximumPostCount, bool isRunParallel)
        {
            lock (SyncJobProcess)
            {
                // check whether need to run parallel
                if (isRunParallel)
                {
                    // Call with task
                    ThreadFactory.Instance.Start(() =>
                    {
                        // start publishing with max post count
                        Publish(maximumPostCount);

                        // check any action waiting to perform
                        PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                        // after completion of publishing, decrease a publishing count
                        PublishScheduler.DecreasePublishingCount(CampaignId);

                        GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);

                    }, CampaignCancellationToken.Token);
                }
                else
                {
                    // start publishing with max post count
                    Publish(maximumPostCount);

                    // check any action waiting to perform
                    PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                    // after completion of publishing, decrease a publishing count
                    PublishScheduler.DecreasePublishingCount(CampaignId);

                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                }

            }
        }


        /// <summary>
        /// To Start publishing a post for an account
        /// </summary>      
        /// <param name="isRunParallel">Specify whether need to run on parallely or not</param>
        public void StartPublishingPosts( bool isRunParallel)
        {
            lock (SyncJobProcess)
            {
                // check whether need to run parallel
                if (isRunParallel)
                {
                    // Call with task
                    ThreadFactory.Instance.Start(() =>
                    {
                        // start publishing with max post count
                        StartPublish();

                        // check any action waiting to perform
                        PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                        // after completion of publishing, decrease a publishing count
                        PublishScheduler.DecreasePublishingCount(CampaignId);

                        GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);

                    }, CampaignCancellationToken.Token);
                }
                else
                {
                    // start publishing with max post count
                    StartPublish();

                    // check any action waiting to perform
                    PublishScheduler.RunAndRemovePublisherAction($"{CampaignId}-{AccountModel.AccountBaseModel.AccountId}");

                    // after completion of publishing, decrease a publishing count
                    PublishScheduler.DecreasePublishingCount(CampaignId);

                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                }

            }
        }

        /// <summary>
        /// Starting a post for a current accounts with selected destinations
        /// </summary>     
        private void StartPublish()
        {
            PublishedCount=0;

            try
            {
                // Getting the delay while running after a x posts completions
                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue, JobConfigurations.PostRange.StartValue);

                foreach (var destination in PublisherDestinationDetailsModels)
                {
                    // check whether cancellation token source already arised or not 
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    var destinationUrls = destination.DestinationType == ConstantVariable.OwnWall
                        ? AccountModel.AccountBaseModel.UserName
                        : destination.DestinationUrl;

                    GlobusLogHelper.log.Info(Log.StartPublishing,
                        AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, $"{destination.DestinationType} [{destinationUrls}]");

                    if (destination.DestinationType == ConstantVariable.Group)
                    {
                        // call networks to publishing on groups 
                        PublishOnGroups(AccountModel.AccountId, destination.DestinationUrl, destination.PublisherPostlistModel, destination != PublisherDestinationDetailsModels.Last());
                        
                    }
                    else if (destination.DestinationType == ConstantVariable.PageOrBoard)
                    {
                        // call networks to publishing on pages
                        PublishOnPages(AccountModel.AccountId, destination.DestinationUrl, destination.PublisherPostlistModel, destination != PublisherDestinationDetailsModels.Last());
                    }
                    else if (destination.DestinationType == ConstantVariable.OwnWall)
                    {
                        // call networks to publishing on own wall of an account
                        PublishOnOwnWall(AccountModel.AccountId, destination.PublisherPostlistModel, destination != PublisherDestinationDetailsModels.Last());
                    }
                    else
                    {
                        var customList = new PublisherCustomDestinationModel
                        {
                            DestinationType = destination.DestinationType,
                            DestinationValue = destination.DestinationUrl
                        };

                        // call networks to publishing on custom destinations 
                        PublishOnCustomDestination(AccountModel.AccountId, customList, destination.PublisherPostlistModel,
                            destination != PublisherDestinationDetailsModels.Last());
                    }

                    PublishedCount++;

                    // check whether multiple post delay reached or not
                    if (PublishedCount % multipostDelayCount == 0)
                        DelayBetweenMultiPublish();
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


        /// <summary>
        /// Starting a post for a current accounts with selected destinations
        /// </summary>
        /// <param name="maximumPostCount">maximum publishing count</param>
        private void Publish(int maximumPostCount)
        {
            PublishedCount = 0;
            var isReachedMaximumCount = false;

            try
            {

                // Get the already published post details
                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == CampaignId).ToList();

                // Filter used destination in previous campaign
                var usedDestination = publishedDetails.Select(x => x.DestinationUrl).ToList();

                // Getting the delay while running after a x posts completions
                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue, JobConfigurations.PostRange.StartValue);

                // Check whether shuffle the destination order or not  
                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    #region Publish on Groups

                    // Iterate the group destinations
                    foreach (var groupUrl in GroupDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(groupUrl))
                            continue;

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // Fetching the post models for publishing
                        var post = GetPostModel("Group", groupUrl);

                        // validate is post null or not
                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "group", groupUrl);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, $"group [{groupUrl}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on groups 
                        PublishOnGroups(AccountModel.AccountId, groupUrl, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion

                    #region Publish on Pages

                    if (isReachedMaximumCount)
                        return;

                    // Iterate the post destinations 
                    foreach (var pageUrl in PageDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(pageUrl))
                            continue;

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // Fetching the post models for publishing
                        var post = GetPostModel("Page", pageUrl);

                        // validate is post null or not
                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "page", pageUrl);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName, $"page [{pageUrl}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on pages
                        PublishOnPages(AccountModel.AccountId, pageUrl, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion

                    #region Custom Destination

                    if (isReachedMaximumCount)
                        return;
                    // Iterate the group destinations
                    foreach (var customList in CustomDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(customList.DestinationValue))
                            continue;

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName,
                            $"{customList.DestinationType} [{customList.DestinationValue}]");

                        // Fetching the post models for publishing
                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                        // validate is post null or not
                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, customList.DestinationType.ToLower(), customList.DestinationValue);
                            continue;
                        }

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on custom destinations 
                        PublishOnCustomDestination(AccountModel.AccountId, customList, post,
                            !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }
                // Otherwise, perform without shuffle
                else
                {
                    #region Shuffle the groups and pages

                    var allGroupsPages = new Dictionary<string, string>();

                    //shuffle destinations
                    GroupDestinationList.Shuffle();
                    PageDestinationList.Shuffle();

                    // Add with groups as a destination for group destinations
                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });

                    // Add with groups as a destination for page destinations
                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    // Iterate the groups pages destinations 
                    foreach (var destination in allGroupsPages)
                    {
                        // Validate whether cancellation token source arised or not
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(destination.Key))
                            continue;

                        // Check whether destinations is belongs to groups 
                        if (destination.Value == "Group")
                        {
                            // Fetching the post models for publishing
                            var post = GetPostModel("Group", destination.Key);

                            // validate is post null or not
                            if (post == null)
                            {
                                GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "group", destination.Key);

                                continue;
                            }

                            GlobusLogHelper.log.Info(Log.StartPublishing,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, $"group [{destination.Key}]");

                            // Increase the published count 
                            PublishedCount++;

                            // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            // call networks to publishing on groups 
                            PublishOnGroups(AccountModel.AccountId, destination.Key, post,
                                !isReachedMaximumCount);

                            // check whether multiple post delay reached or not
                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }
                        else
                        {
                            // Fetching the post models for publishing
                            var post = GetPostModel("Page", destination.Key);

                            // validate is post null or not
                            if (post == null)
                            {
                                GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, "page", destination.Key);
                                continue;
                            }

                            // If user needs to publish on a destination only once, then check its used already or not
                            if (GeneralSettingsModel.IsDeselectUsedDestination && usedDestination.Contains(destination.Key))
                                continue;

                            GlobusLogHelper.log.Info(Log.StartPublishing,
                                AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName, $"page [{destination.Key}]");

                            // Increase the published count
                            PublishedCount++;

                            // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            // call networks to publishing on pages
                            PublishOnPages(AccountModel.AccountId, destination.Key, post,
                                isReachedMaximumCount);

                            // check whether multiple post delay reached or not
                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }

                    }

                    #endregion

                    #region Shuffle custom destination

                    if (isReachedMaximumCount)
                        return;

                    // Shuffle custom destinations
                    CustomDestinationList.Shuffle();

                    // Iterate the custom destinations
                    foreach (var customList in CustomDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        // Validate whether cancellation token source arised or not
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // Fetching the post models for publishing
                        var post = GetPostModel(customList.DestinationType, customList.DestinationValue);

                        // validate is post null or not
                        if (post == null)
                        {
                            GlobusLogHelper.log.Info(Log.NoPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, customList.DestinationType.ToLower(), customList.DestinationValue);
                            continue;
                        }

                        GlobusLogHelper.log.Info(Log.StartPublishing,
                            AccountModel.AccountBaseModel.AccountNetwork,
                            AccountModel.AccountBaseModel.UserName,
                            $"{customList.DestinationType} [{customList.DestinationValue}]");

                        // Increase the published count
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks publishing to custom destinations 
                        PublishOnCustomDestination(AccountModel.AccountId, customList, post,
                            !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion
                }

                #region Own Wall

                if (IsPublishOnOwnWall)
                {

                    // check already maximum post has reached or not
                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    // If max count reached, stop the operations immediately
                    if (isReachedMaximumCount)
                        return;

                    // If user needs to publish on a destination only once, then check its used already or not
                    if (GeneralSettingsModel.IsDeselectUsedDestination &&
                        usedDestination.Contains(AccountModel.AccountBaseModel.UserName))
                        return;

                    // Fetching the post models for publishing
                    var ownWallpost = GetPostModel("OwnWall", AccountModel.AccountBaseModel.UserName);

                    // validate is post null or not
                    if (ownWallpost == null)
                        return;

                    GlobusLogHelper.log.Info(Log.StartPublishing,
                        AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Own wall");

                    // Increase the published count
                    PublishedCount++;

                    // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    // call networks to publishing on own wall of an account
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

        /// <summary>
        /// Start publishing the given post to destinations 
        /// </summary>
        /// <param name="post"><see cref="PublisherPostlistModel"/> Post list model</param>
        /// <param name="count">Publishing count</param>
        /// <param name="isStartParallel">Is need to publishing on parallely</param>
        public void StartPublishing(PublisherPostlistModel post, int count, bool isStartParallel)
        {
            lock (SyncJobProcess)
            {
                // check need to start parallely
                if (isStartParallel)
                {
                    ThreadFactory.Instance.Start(() =>
                     {
                         // Call to start publishing direct posts
                         PublishWithDirectPost(post, count);
                         GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                     }, CampaignCancellationToken.Token);
                }
                else
                {
                    // Call to start publishing direct posts
                    PublishWithDirectPost(post, count);
                    GlobusLogHelper.log.Info(Log.PublishingProcessCompleted, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, CampaignName);
                }
            }
        }

        /// <summary>
        /// To start given post list model to destinations
        /// </summary>
        /// <param name="post"><see cref="PublisherPostlistModel"/> Post list model</param>
        /// <param name="maximumPostCount">Publishing count</param>
        private void PublishWithDirectPost(PublisherPostlistModel post, int maximumPostCount)
        {
            PublishedCount = 0;
            var isReachedMaximumCount = false;

            try
            {
                // Get the already published post details
                var publishedDetails = GenericFileManager.GetModuleDetails<PublishedPostDetailsModel>(ConstantVariable.GetPublishedSuccessDetails).Where(x => x.CampaignId == CampaignId).ToList();

                // Filter used destination in previous campaign
                var usedDestination = publishedDetails.Select(x => x.DestinationUrl).ToList();

                // Getting the delay while running after a x posts completions
                var multipostDelayCount = RandomUtilties.GetRandomNumber(JobConfigurations.PostRange.EndValue, JobConfigurations.PostRange.StartValue);

                // Check whether shuffle the destination order or not  
                if (GeneralSettingsModel.IsStopRandomisingDestinationsOrder)
                {
                    #region Publish on Groups

                    // Iterate the group destinations
                    foreach (var groupUrl in GroupDestinationList)
                    {
                        // Update post status
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        // Check whether current post has already published with destinations 
                        if (!PostValidations(post, "Group", groupUrl, true))
                            continue;

                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(groupUrl))
                            continue;

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{groupUrl}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on groups 
                        PublishOnGroups(AccountModel.AccountId, groupUrl, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();

                    }

                    #endregion

                    #region Publish on Pages

                    if (isReachedMaximumCount)
                        return;

                    // Iterate the post destinations 
                    foreach (var pageUrl in PageDestinationList)
                    {
                        // Update post status
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        // Check whether current post has already published with destinations 
                        if (!PostValidations(post, "Page", pageUrl, true))
                            continue;

                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(pageUrl))
                            continue;

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{pageUrl}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on pages 
                        PublishOnPages(AccountModel.AccountId, pageUrl, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion

                    #region Publish on Custom destination


                    if (isReachedMaximumCount)
                        return;
                    // Iterate the custom destinations
                    foreach (var customList in CustomDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // Check whether current post has already published with destinations 
                        if (!PostValidations(post, customList.DestinationType, customList.DestinationValue, true))
                            continue;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        // Update post status
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType} [{customList.DestinationValue}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on custom destiantions
                        PublishOnCustomDestination(AccountModel.AccountId, customList, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }
                else
                {
                    #region Shuffle Groups and pages

                    var allGroupsPages = new Dictionary<string, string>();
                    // shuffle group destinations
                    GroupDestinationList.Shuffle();

                    // shuffle page destinations
                    PageDestinationList.Shuffle();

                    // Add with destination name as groups
                    GroupDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Group");
                    });

                    // Add with destination name as page
                    PageDestinationList.ForEach(x =>
                    {
                        allGroupsPages.Add(x, "Page");
                    });

                    // Iterate all shuffled groups
                    foreach (var x in allGroupsPages)
                    {
                        // Update post status
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(x.Key))
                            continue;

                        if (x.Value == "Group")
                        {
                            // Check whether current post has already published with destinations 
                            if (!PostValidations(post, "Group", x.Key, true))
                                continue;

                            // Increase the published count 
                            PublishedCount++;

                            // check already maximum post has reached or not
                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"group [{x.Key}]");

                            // call networks to publishing on groups 
                            PublishOnGroups(AccountModel.AccountId, x.Key, post, !isReachedMaximumCount);

                            // check whether multiple post delay reached or not
                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }
                        else
                        {
                            // Check whether current post has already published with destinations 
                            if (!PostValidations(post, "Page", x.Key, true))
                                continue;

                            // Increase the published count
                            PublishedCount++;

                            // check already maximum post has reached or not
                            if (PublishedCount >= maximumPostCount)
                                isReachedMaximumCount = true;

                            GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"page [{x.Key}]");

                            // call networks to publishing on pages 
                            PublishOnPages(AccountModel.AccountId, x.Key, post, !isReachedMaximumCount);

                            // check whether multiple post delay reached or not
                            if (PublishedCount % multipostDelayCount == 0)
                                DelayBetweenMultiPublish();
                        }

                    }

                    #endregion

                    #region Custom Destination

                    if (isReachedMaximumCount)
                        return;

                    // shuffle custom destinations
                    CustomDestinationList.Shuffle();

                    // Iterate the custom destinations
                    foreach (var customList in CustomDestinationList)
                    {
                        // check already maximum post has reached or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // If max count reached, stop the operations immediately
                        if (isReachedMaximumCount)
                            break;

                        // Check whether current post has already published with destinations 
                        if (!PostValidations(post, customList.DestinationType, customList.DestinationValue, true))
                            continue;

                        // If user needs to publish on a destination only once, then check its used already or not
                        if (GeneralSettingsModel.IsDeselectUsedDestination &&
                            usedDestination.Contains(customList.DestinationValue))
                            continue;

                        // Update post status
                        PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                        // check whether cancellation token source already arised or not 
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, $"{customList.DestinationType}  [{customList.DestinationValue}]");

                        // Increase the published count 
                        PublishedCount++;

                        // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                        if (PublishedCount >= maximumPostCount)
                            isReachedMaximumCount = true;

                        // call networks to publishing on custom destiantions
                        PublishOnCustomDestination(AccountModel.AccountId, customList, post, !isReachedMaximumCount);

                        // check whether multiple post delay reached or not
                        if (PublishedCount % multipostDelayCount == 0)
                            DelayBetweenMultiPublish();
                    }

                    #endregion
                }

                #region OwnWall

                if (IsPublishOnOwnWall)
                {
                    // check already maximum post has reached or not
                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    // If max count reached, stop the operations immediately
                    if (isReachedMaximumCount)
                        return;

                    // If user needs to publish on a destination only once, then check its used already or not
                    if (GeneralSettingsModel.IsDeselectUsedDestination &&
                        usedDestination.Contains(AccountModel.AccountBaseModel.UserName))
                        return;

                    // Update post status
                    PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                    // check whether cancellation token source already arised or not 
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    // Check whether current post has already published with destinations 
                    if (!PostValidations(post, "OwnWall", AccountModel.AccountBaseModel.UserName,
                        true))
                        return;

                    GlobusLogHelper.log.Info(Log.StartPublishing, AccountModel.AccountBaseModel.AccountNetwork,
                        AccountModel.AccountBaseModel.UserName, "Own wall");

                    // Increase the published count 
                    PublishedCount++;

                    // Finding the fetched post is last, for a current job or not, Based on this only we passing delay needed or not
                    if (PublishedCount >= maximumPostCount)
                        isReachedMaximumCount = true;

                    // call networks to publish on own wall
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

        /// <summary>
        /// Update Post with specified success status
        /// </summary>
        /// <param name="destinationUrl">>Destination Url</param>
        /// <param name="posts">Post model <see cref="PublisherPostlistModel"/></param>
        /// <param name="publishedUrl">Published Post Id/ Url</param>
        public void UpdatePostWithSuccessful(string destinationUrl, PublisherPostlistModel posts, string publishedUrl)
        {
            Thread.Sleep(1000);

            //Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl && y.AccountId == AccountModel.AccountId));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass the information about success
                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.Yes;
                post.LstPublishedPostDetailsModels[postIndex].Link = publishedUrl;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = ConstantVariable.NoError;

                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                // Add into success details
                GenericFileManager.AddModule(post.LstPublishedPostDetailsModels[postIndex], ConstantVariable.GetPublishedSuccessDetails);
            }
        }


        /// <summary>
        /// Update Post with specified failed status
        /// </summary>
        /// <param name="destinationUrl">Destination Url</param>
        /// <param name="posts">Post model <see cref="PublisherPostlistModel"/></param>
        /// <param name="errorMessage">Pass the error message</param>
        public void UpdatePostWithFailed(string destinationUrl, PublisherPostlistModel posts, string errorMessage)
        {
            //Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(posts.PostId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, posts.PostId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(
                    post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl && y.AccountId == AccountModel.AccountId));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass error message with current date time
                post.LstPublishedPostDetailsModels[postIndex].Successful = ConstantVariable.No;
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = errorMessage;
                post.LstPublishedPostDetailsModels[postIndex].PublishedDate = DateTime.Now;

                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
            }
        }

        /// <summary>
        /// Update Post for successful deletion after specified time
        /// </summary>
        /// <param name="destinationUrl">Destination url</param>
        /// <param name="postId">Post Id</param>
        public void UpdatePostWithDeletion(string destinationUrl, string postId)
        {
            Thread.Sleep(1000);

            // Get the locking objects
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(postId, _lock => new object());

            lock (updatelock)
            {
                // get the post details
                var post = PostlistFileManager.GetByPostId(CampaignId, postId);

                // get the post index where current destination present
                var postIndex = post.LstPublishedPostDetailsModels.IndexOf(post.LstPublishedPostDetailsModels.FirstOrDefault(y => y.DestinationUrl == destinationUrl));

                // if post index is not present,then return
                if (postIndex == -1)
                    return;

                // Pass the proper delete post text
                post.LstPublishedPostDetailsModels[postIndex].ErrorDetails = ConstantVariable.DeletedDateText();

                // Update the post details to bin file
                PostlistFileManager.UpdatePost(CampaignId, post);

                // Update the used post status
                PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);

                // Add into success details
                GenericFileManager.AddModule(post.LstPublishedPostDetailsModels[postIndex], ConstantVariable.GetPublishedSuccessDetails);

            }
        }

        /// <summary>
        /// To apply user selected general settings for campaigns
        /// </summary>
        /// <param name="givenPostModel">post list model <see cref="PublisherPostlistModel"/></param>
        /// <returns></returns>
        public PublisherPostlistModel PerformGeneralSettings(PublisherPostlistModel givenPostModel)
        {
            // Get the deep clone of the post list
            var postModelWithGeneralSettings = givenPostModel.DeepClone();

            #region Fetch Post media list

            // is user need to get a random single post or not
            if (GeneralSettingsModel.IsChooseSingleRandomImageChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                {
                    // get a random number
                    var randomNumber = RandomUtilties.GetRandomNumber(postModelWithGeneralSettings.MediaList.Count - 1);

                    // Fetch the media
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[randomNumber] };
                }
            }
            // If user need first image 
            else if (GeneralSettingsModel.IsChooseOnlyFirstImageChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                    //Get the first image
                    postModelWithGeneralSettings.MediaList = new ObservableCollection<string> { givenPostModel.MediaList[0] };
            }
            // If user needs select between random no of images
            else if (GeneralSettingsModel.IsChooseBetweenChecked)
            {
                // Validate media list atleast contains a single post
                if (givenPostModel.MediaList.Count > 0)
                {
                    // get the random no of counts for fetching medias
                    var randomNumber = RandomUtilties.GetRandomNumber(GeneralSettingsModel.ChooseBetween.EndValue, GeneralSettingsModel.ChooseBetween.StartValue);
                    if (randomNumber < givenPostModel.MediaList.Count)
                    {
                        // shuffle the image media lists
                        givenPostModel.MediaList.Shuffle();
                        postModelWithGeneralSettings.MediaList = new ObservableCollection<string>(givenPostModel.MediaList.Take(randomNumber));
                    }
                }
            }
            // In socinator we are appending "_SOCINATORIMAGE.jpg" text for video url, before publishing we need to remove those constant text
            var removeVideoExtension = new ObservableCollection<string>();

            // Removing extra added media lists
            foreach (var media in postModelWithGeneralSettings.MediaList)
            {
                removeVideoExtension.Add(media.Replace(ConstantVariable.VideoToImageConvertFileName, string.Empty));
            }

            postModelWithGeneralSettings.MediaList = removeVideoExtension;

            #endregion

            #region Macro Substitution

            // Substitute the macros for a post descriptions
            postModelWithGeneralSettings.PostDescription =
                MacrosHelper.SubstituteMacroValues(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Spin Text

            // check whether post description is null or not
            if (string.IsNullOrEmpty(postModelWithGeneralSettings.PostDescription))
                postModelWithGeneralSettings.PostDescription = string.Empty;

            // Substitute the spin text for post descriptions
            postModelWithGeneralSettings.PostDescription =
                SpinTexHelper.GetSpinText(postModelWithGeneralSettings.PostDescription);

            #endregion

            #region Remove link

            // Check user need to remove the links from post descriptions
            if (GeneralSettingsModel.IsRemoveLinkFromPostsChecked)
            {
                // Call to remove urls from post descriptions
                postModelWithGeneralSettings.PostDescription = Utilities.RemoveUrls(givenPostModel.PostDescription);
            }

            #endregion

            #region Shorten Url

            // Check user needs to make shorten url for long url
            if (OtherConfiguration.IsShortenURLsChecked)
            {
                // call to replace the long url to shorten rul by using Bitly 
                postModelWithGeneralSettings.PostDescription = Utilities.ReplaceWithShortenUrl(postModelWithGeneralSettings.PostDescription);
            }

            #endregion

            #region Adding Signature

            // Check user needs to append signature to post descriptions 
            if (OtherConfiguration.IsEnableSignatureChecked)
            {
                // Append the post signatures
                postModelWithGeneralSettings.PostDescription = postModelWithGeneralSettings.PostDescription + "\r\n" +
                                                  OtherConfiguration.SignatureText;
            }

            #endregion

            return postModelWithGeneralSettings;

        }

        /// <summary>
        /// To stop the current running jobs
        /// </summary>
        public void Stop()
        {
            try
            {
                //Cancell the cancellation token sources
                CurrentJobCancellationToken.Cancel();
            }
            catch (Exception ex)
            {
                ex.DebugLog();
            }
        }

        /// <summary>
        /// Apply the delay for next publish
        /// </summary>
        public void DelayBeforeNextPublish()
        {
            // Fetching delay seconds count
            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetweenPost.EndValue, JobConfigurations.DelayBetweenPost.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenPublishing, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, delay);

            // Apply the delay to current campaign specifically for current account to next post in seconds
            Thread.Sleep(delay * 1000);
        }

        /// <summary>
        /// Make a delay between every x to y posts for specified minutes
        /// </summary>
        public void DelayBetweenMultiPublish()
        {
            // Fetching delay minute count
            var delay = RandomUtilties.GetRandomNumber(JobConfigurations.DelayBetween.EndValue, JobConfigurations.DelayBetween.StartValue);

            GlobusLogHelper.log.Info(Log.DelayBetweenMultiPost, AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName, delay);

            // Apply the delay to current campaign specifically for current account to next post in minutes
            Thread.Sleep(delay * 1000 * 60);
        }

        /// <summary>
        /// To get a post for publishing, Its filtered based on settings
        /// </summary>
        /// <param name="destination">destination type</param>
        /// <param name="destinationUrl">destination url</param>
        /// <returns></returns>
        public PublisherPostlistModel GetPostModel(string destination, string destinationUrl)
        {
            Thread.Sleep(1000);

            // Add new lock object or getting already used objects
            var updatelock = PublishScheduler.GetPostsForPublishing.GetOrAdd(CampaignId, _lock => new object());

            lock (updatelock)
            {
                try
                {
                    // validate already token arised or not
                    CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                    // Getting all pending and published post lists
                    var pendingPostList = PostlistFileManager.GetAll(CampaignId)
                        .Where(x => x.PostQueuedStatus != PostQueuedStatus.Draft).ToList();

                    // Checking, If no more post available
                    if (!pendingPostList.Any())
                    {
                        GlobusLogHelper.log.Info($"No more post are available for campaign {CampaignName}!");
                        return null;
                    }

                    // Validate whether user needs a unique post for every time
                    if (GeneralSettingsModel.IsWhenPublishingSendOnePostChecked)
                        // Fetching only not used posts
                        pendingPostList = pendingPostList.Where(x => x.LstPublishedPostDetailsModels.Count == 0)
                            .ToList();
                    else
                        // Fetching not published posts for the current destinations 
                        pendingPostList = (from posts in pendingPostList
                                           let successfulDestinations = posts.LstPublishedPostDetailsModels
                                               .Where(x => x.Successful == ConstantVariable.Yes).Select(x => x.DestinationUrl)
                                           where !successfulDestinations.Contains(destinationUrl)
                                           select posts).ToList();

                    // Validate the toaster notifications is needed
                    if (ConstantVariable.IsToasterNotificationNeed)
                    {
                        // If user needs to notify when postlists going lesser than specified post, then trigger a notifications
                        if (pendingPostList.Count < GeneralSettingsModel.TriggerNotificationCount &&
                            GeneralSettingsModel.TriggerNotificationCount > 0)
                            ToasterNotification.ShowInfomation(
                                $"{AccountModel.AccountBaseModel.UserName} has {pendingPostList.Count} pending post for {destination}({destinationUrl}) in the {CampaignName} campaign!");
                    }

                    var iterationCount = 0;

                    // Iterate the posts untill finding post models
                    while (true)
                    {
                        // Validate whether cancellation token is arised
                        CampaignCancellationToken.Token.ThrowIfCancellationRequested();

                        // If not found and also reached end of available post, then break the finding operation
                        if (iterationCount >= pendingPostList.Count)
                            break;

                        // If user need to fetching post from shuffled lists, then get random post list otherwise get a first post list
                        var filterPostModel = GeneralSettingsModel.IsChooseRandomPostsChecked
                            ? pendingPostList[RandomUtilties.GetRandomNumber(pendingPostList.Count - 1, 0)]
                            : pendingPostList.FirstOrDefault(x => x.PostId != null);

                        // Validate current post is fit to publish or not
                        if (PostValidations(filterPostModel, destination, destinationUrl))
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
            }
            return null;
        }

        /// <summary>
        /// Validate the posts and Notify to default page view model for current post used in current account and campaign
        /// </summary>
        /// <param name="filterPostModel">Post model details <see cref="PublisherPostlistModel"/></param>
        /// <param name="destination">destination type</param>
        /// <param name="destinationUrl">destination url</param>
        /// <param name="isDirectPost">specify its direct posts or not</param>
        /// <returns></returns>
        public bool PostValidations(PublisherPostlistModel filterPostModel, string destination, string destinationUrl, bool isDirectPost = false)
        {
            Thread.Sleep(1000);

            // Get the readonly objects for validating 
            var updatelock = PublishScheduler.UpdatingLock.GetOrAdd(filterPostModel.PostId, _lock => new object());

            lock (updatelock)
            {

                try
                {
                    // get the current post details
                    var post = PostlistFileManager.GetByPostId(CampaignId, filterPostModel.PostId);

                    // fetching tried and successful post details 
                    var postTriedAndSuccessdestinations = post?.LstPublishedPostDetailsModels.ToList();

                    if (postTriedAndSuccessdestinations == null)
                        return false;

                    // Fetching the destination urls where this post is already published 
                    var successfulDestinations = postTriedAndSuccessdestinations
                        .Where(x => x.Successful == ConstantVariable.Yes).Select(x => x.DestinationUrl).ToList();

                    // If current destination has already published current post, specify its already used one
                    if (successfulDestinations.Contains(destinationUrl))
                    {
                        GlobusLogHelper.log.Info(destination == "OwnWall"
                            ? string.Format(Log.AlreadyPublishedOnOwnWall, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName)
                            : string.Format(Log.AlreadyPublishedOnDestination,
                                AccountModel.AccountBaseModel.AccountNetwork, AccountModel.AccountBaseModel.UserName,
                                destination, destinationUrl));
                        return false;
                    }

                    // get other destinatons for a current account
                    var allDestinations = postTriedAndSuccessdestinations.Where(x => x.AccountId == AccountModel.AccountId).Select(x => x.DestinationUrl);

                    // check current destination present in the fetched destination list
                    if (!allDestinations.Contains(destinationUrl))
                    {
                        // Append the post list details 
                        post.LstPublishedPostDetailsModels.Add(new PublishedPostDetailsModel
                        {
                            AccountName = AccountModel.AccountBaseModel.UserName,
                            Destination = destination,
                            DestinationUrl = destination == "OwnWall"
                                ? AccountModel.AccountBaseModel.UserName
                                : destinationUrl,
                            Description = post.PostDescription,
                            IsPublished = ConstantVariable.Yes,
                            Successful = ConstantVariable.No,
                            PublishedDate = DateTime.Now,
                            Link = ConstantVariable.NotPublished,
                            CampaignId = CampaignId,
                            CampaignName = CampaignName,
                            SocialNetworks = AccountModel.AccountBaseModel.AccountNetwork,
                            AccountId = AccountModel.AccountBaseModel.AccountId,
                            ErrorDetails = ConstantVariable.NotPublished,
                        });
                    }

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
                    PostlistFileManager.UpdatePost(CampaignId, post);

                    if (post.PostRunningStatus == PostRunningStatus.Completed)
                    {
                        if (isDirectPost)
                            GlobusLogHelper.log.Info(Log.PostExpired, AccountModel.AccountBaseModel.AccountNetwork,
                                AccountModel.AccountBaseModel.UserName);
                        return false;
                    }

                    // update stats to publisher default view
                    PublisherInitialize.GetInstance.UpdatePostStatus(CampaignId);
                }
                catch (Exception ex)
                {
                    ex.DebugLog();
                }
            }
            return true;
        }

        #endregion
    }
}