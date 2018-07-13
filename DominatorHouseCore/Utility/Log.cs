using System.Net.Mime;
using System.Windows;

namespace DominatorHouseCore.Utility
{
    public static class Log
    {
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string StartingJob { get; set; } = "{0}\t {1}\t {2}\t" + "LangKeyStartedJobTo".FromResourceDictionary() + "{2}. \t" + CodeConstants.StartedJob;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string JobCompleted { get; set; } = "{0}\t {1}\t {2}\t" + "LangKeySuccessfullyCompleteJobTo".FromResourceDictionary() + " {2}. \t" + CodeConstants.CompletedJob;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AccountLogin { get; set; } = "{0}\t {1}\t Login \t " + "LangKeyAttemptToLogin".FromResourceDictionary() + "\t" + CodeConstants.LoginAttempt;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string SuccessfulLogin { get; set; } = "{0}\t {1}\t Login Success \t " + "LangKeyLoginSuccessful".FromResourceDictionary() + ".\t" + CodeConstants.LoginSuccessful;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string LoginFailed { get; set; } = "{0}\t {1}\t Login Failed \t " + "LangKeyLoginFailedWithError".FromResourceDictionary() + " {2}.\t" + CodeConstants.LoginFailed;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// </summary>
        public static string StartedActivity { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyTryingTo".FromResourceDictionary() + " {2} {3}\t" + CodeConstants.StartedActivity;



        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// </summary>
        public static string ActivitySuccessful { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfulTo".FromResourceDictionary() + " {2} {3}\t" + CodeConstants.ActivitySuccessful;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// 4 = SocialNetworkError
        /// </summary>
        public static string ActivityFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFailedTo".FromResourceDictionary() + " {2} " + "LangKeyWithError".FromResourceDictionary() + " {3}\t" + CodeConstants.ActivityFailed;

        public static string ImportFailed { get; set; } = "{0}\t {1}\t Import \t " + "LangKeyFailedBecauseOf".FromResourceDictionary() + " {3} " + "LangKeyIsAreNotCorrect".FromResourceDictionary() + "\t" + CodeConstants.ImportFailed;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ReasonToPause 
        /// </summary>
        public static string JobPaused { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedCurrentJobTo".FromResourceDictionary() + " {2} " + "LangKeyAs".FromResourceDictionary() + "\t" + CodeConstants.JobPaused;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string SavedCampaign { get; set; } = "{0}\t {1}\t Campaign \t " + "LangKeySuccessfullySaved".FromResourceDictionary() + "\t" + CodeConstants.SavedCampaign;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignDeleted { get; set; } = "{0}\t {1}\t Campaign\t " + "LangKeySuccessfullyDeleted".FromResourceDictionary() + "\t" + CodeConstants.CampaignDeleted;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignUpdated { get; set; } = "{0}\t {1}\t Campaign\t " + "LangKeySuccessfullyUpdated".FromResourceDictionary() + "\t" + CodeConstants.CampaignUpdated;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignPaused { get; set; } = "{0}\t {1}\t Campaign\t " + "LangKeySuccessfullyPaused".FromResourceDictionary() + "\t" + CodeConstants.CampaignPaused;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string ActivatedCampaign { get; set; } = "{0}\t {1}\t Campaign \t " + "LangKeyActivatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.ActivatedCampaign;


        public static string UpdatingDetails { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStarted".FromResourceDictionary() + " {2}" + "LangKeySynchronization".FromResourceDictionary() + "\t" + CodeConstants.UpdatingDetails;

        public static string DetailsUpdated { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySynchronization".FromResourceDictionary() + " {2} " + "LangKeySuccessful".FromResourceDictionary() + "\t" + CodeConstants.DetailsUpdated;

        public static string UploadedAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + " " + "LangKeyAccountTo".FromResourceDictionary() + " {2}.\t" + CodeConstants.UploadedAccount;
      
        public static string SelectedAccount { get; set; } = "{0}\t {1}\t " + "LangKeyAccountSelection".FromResourceDictionary() + "\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + " {2} " + "LangKeyAccountTo".FromResourceDictionary() + " {3}.\t" + CodeConstants.SelectedAccount;

        /// <summary>
        /// 0 = NumberOfAccounts
        /// 1 = PlatformName
        /// </summary>
        public static string DeletedAccounts { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyDeleted".FromResourceDictionary() + " " + "LangKeyAccountsFrom".FromResourceDictionary() + " {3}.\t" + CodeConstants.DeletedAccounts;


        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// </summary>
        public static string AccountEdited { get; set; } = "{0}\t {1}\t Account Info\t " + "LangKeyDetailsUpdatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.AccountEdited;


        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// 2 = ActivityType
        /// 3 = DelaySeconds
        /// </summary>
        public static string DelayBetweenActivity { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextOperationTo".FromResourceDictionary() + " {2} " + "LangKeyWillPerformIn".FromResourceDictionary() + " {3} " + "LangKeySeconds".FromResourceDictionary() + "\t" + CodeConstants.DelayBetweenActivity;

        public static string NextScheduledJob { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextJobTo".FromResourceDictionary() + " {2} " + "LangKeyIsScheduledToRunBy".FromResourceDictionary() + " {3}\t" + CodeConstants.NextScheduledJob;

        public static string NextJobExpectedToStartBy { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextJobTo".FromResourceDictionary() + " {2} " + "LangKeyIsExpectedToStartBy".FromResourceDictionary() + " {3}\t" + CodeConstants.NextJobExpectedToStartBy;

        public static string JobLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerJobLimitOf".FromResourceDictionary() + " {3}. \t" + CodeConstants.JobLimitReached;

        public static string DailyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerDayLimitOf".FromResourceDictionary() + " {3}. \t" + CodeConstants.DailyLimitReached;

        public static string HourlyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerHourLimitOf".FromResourceDictionary() + " {3}. \t" + CodeConstants.HourlyLimitReached;

        public static string WeeklyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerWeekLimitOf".FromResourceDictionary() + " {3}. \t" + CodeConstants.WeeklyLimitReached;

        public static string LimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReached".FromResourceDictionary() + " {2} " + "LangKeyLimitOf".FromResourceDictionary() + " {3}\t" + CodeConstants.LimitReached;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string OtherConfigurationStarted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyOtherConfigurationFor".FromResourceDictionary() + " {2} " + "LangKeyIsStarted".FromResourceDictionary() + "\t" + CodeConstants.OtherConfigurationStarted;

        public static string OtherCongigurationCompleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyOtherConfigurationFor".FromResourceDictionary() + " {2} " + "LangKeyIsCompleted".FromResourceDictionary() + "\t" + CodeConstants.OtherCongigurationCompleted;

        public static string FilterApplied { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAppliedFilterTo".FromResourceDictionary() + " {2} " + "LangKeySearchResults".FromResourceDictionary() + " {3}\t" + CodeConstants.FilterApplied;

        public static string DetailsScraped { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFound".FromResourceDictionary() + " {2} " + "LangKeyScrapetypeToActivity".FromResourceDictionary() + " {3}\t" + CodeConstants.DetailsScraped;
        public static string ManagedBlacklist { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyRemoved".FromResourceDictionary() + " {2} " + "LangKeyUsersBelongingToBlacklistToProcess".FromResourceDictionary() + " {3}\t" + CodeConstants.ManagedBlacklist;
        public static string AddedToBlacklist { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + " {2} " + "LangKeyToBlacklist".FromResourceDictionary() + " {3}\t" + CodeConstants.AddedToBlacklist;
        public static string NoMoreDataToPerform { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNoMoreDataAvailableToPerform".FromResourceDictionary() + "\t" + CodeConstants.NoMoreDataToPerform;
        public static string FoundXResults { get; set; } = "{0}\t {1}\t {5}\t" + "LangKeyFound".FromResourceDictionary() + " {2} " + "LangKeyResultsByQueryType".FromResourceDictionary() + " {3} " + "LangKeyAndQueryValue".FromResourceDictionary() + " {4} " + "LangKeyTo".FromResourceDictionary() + " {5}\t" + CodeConstants.FoundXResults;

        public static string AlreadyExistQuery { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyIndexIesAreAlreadyAddedIn".FromResourceDictionary() + " {2} " + "LangKeySearchQueryIes".FromResourceDictionary() + " {3}\t" + CodeConstants.AlreadyExistQuery;

        public static string AlreadyExistQueryCount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAreAlreadyAddedIn".FromResourceDictionary() + " {2}" + "LangKeySearchQueryIes".FromResourceDictionary() + " {3}\t" + CodeConstants.AlreadyExistQueryCount;

        public static string AccountNeedsVerification { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNeedsToVerifiedToPerformNextActivities".FromResourceDictionary() + "\t" + CodeConstants.AccountNeedsVerification;

        public static string CustomMessage { get; set; } = "{0}\t {1}\t {2}\t{3}\t" + CodeConstants.CustomMessage;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessCompleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyCompleteProcessTo".FromResourceDictionary() + " {2}\t" + CodeConstants.ProcessCompleted;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStarted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStartedProcessTo".FromResourceDictionary() + " {2}\t" + CodeConstants.ProcessStarted;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStopped { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedProcessTo".FromResourceDictionary() + " {2}\t" + CodeConstants.ProcessStopped;

        public static string CampaignNotSet { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedProcessTo".FromResourceDictionary() + " {2}\t" + CodeConstants.ProcessStopped;


        /// <summary>
        /// 0 = SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Deleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyDeleted".FromResourceDictionary() + "\t" + CodeConstants.Deleted;


        /// <summary>
        /// 0 = SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Added { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + "\t" + CodeConstants.Added;

        /// <summary>
        /// 0 =  SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Exported { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyExported".FromResourceDictionary() + "\t" + CodeConstants.Exported;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string NotAddedAccount { get; set; } = "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" + "LangKeyHavingIssuesToAdd".FromResourceDictionary() + "\t" + CodeConstants.NotAddedAccount;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyAddedAccount { get; set; } = "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" + "LangKeyAlreadyAdded".FromResourceDictionary() + "\t" + CodeConstants.AlreadyAddedAccount;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyStoppedUpdatingAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAlreadyStopped".FromResourceDictionary() + "\t" + CodeConstants.AlreadyStoppedUpdatingAccount;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyUpdatingAccount { get; set; } = "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" + "LangKeyAlreadyStarted".FromResourceDictionary() + "\t" + CodeConstants.AlreadyUpdatingAccount;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string StopUpdatingAccount { get; set; } = "{0}\t {1}\t " + "LangKeyAccounts".FromResourceDictionary() + "\t" + "LangKeyStoppedForFurtherFriendshipUpdate".FromResourceDictionary() + "\t" + CodeConstants.StopUpdatingAccount;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = campaign name    
        /// </summary>
        public static string StartPublishing { get; set; } = "{0}\t {1}\t" + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyPublishingStartedWith".FromResourceDictionary() + " {2}\t" + CodeConstants.StartPublishing;



        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = delay in seconds 
        /// </summary>
        public static string DelayBetweenPublishing { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyNextPostWillStartPublishingIn".FromResourceDictionary() + " {2} " + "LangKeySeconds".FromResourceDictionary() + "\t" + CodeConstants.DelayBetweenPublishing;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = delay in seconds 
        /// </summary>
        public static string DelayBetweenMultiPost { get; set; } = "{0}\t {1}\t"+ "LangKeyPublish".FromResourceDictionary() + "\t" + "Next post will start publishing in {2} minutes" + "\t" + CodeConstants.DelayBetweenMultiPost;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username          
        /// </summary>
        public static string AlreadyPublishedOnOwnWall { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyPostHasAlreadyPostedOnOwnWallProfile".FromResourceDictionary() + "\t" + CodeConstants.AlreadyPublishedOnOwnWall;

        //Post has already posted with destintion

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Destination Type
        /// 3 = Destination Url        
        /// </summary>
        public static string AlreadyPublishedOnDestination { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyPostHasAlreadyPostedWithDestintion".FromResourceDictionary() + "{2}-{3}\t" + CodeConstants.AlreadyPublishedOnDestination;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username         
        /// </summary>
        public static string PostExpired { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPostAlreadyExpired".FromResourceDictionary() + "\t" + CodeConstants.PostExpired;



        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string PublishingSuccessfully { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyPublishedSuccessfullyOn".FromResourceDictionary() + " {2}-[{3}]\t" + CodeConstants.PublishingSuccessfully;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string PublishingFailed { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyErrorWhilePublishingOn".FromResourceDictionary() + " {2}-[{3}]\t" + CodeConstants.PublishingFailed;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string SharedSuccessfully { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySharedSuccessfullyOn".FromResourceDictionary() + " {2}-[{3}]\t" + CodeConstants.SharedSuccessfully;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string ShareFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyErrorWhileSharingOn".FromResourceDictionary() + " {2}-[{3}]\t" + CodeConstants.ShareFailed;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMedia { get; set; } = "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() + "\t" + "LangKeyUploadingMediaFile".FromResourceDictionary() + " [{2}]\t" + CodeConstants.UploadingMedia;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaSuccessful { get; set; } = "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() + "\t" + "LangKeySuccessfullyUploadedMediaFile".FromResourceDictionary() + " [{2}]\t" + CodeConstants.UploadingMediaSuccessful;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaFailed { get; set; } = "{0}\t {1}\t " + "LangKeyMedia".FromResourceDictionary() + "\t" + "LangKeyErrorWhileUploadingMediaFile".FromResourceDictionary() + " [{2}]\t" + CodeConstants.UploadingMediaFailed;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Failed reason
        /// </summary>
        public static string UploadingMediaFailedReason { get; set; } = "{0}\t {1}\t" + "LangKeyMedia".FromResourceDictionary() + "\t" + "LangKeyFailedDueTo".FromResourceDictionary() + " [{2}]\t" + CodeConstants.UploadingMediaFailedReason;




        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string PublisherCampaignPaused { get; set; } = "Campaign : {0} successfully paused.";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = destination type
        /// 3 = destination Value
        /// </summary>
        public static string NoPost { get; set; } = "{0}\t {1}\t No more post are available for {2}[{3}].";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = CampaignName       
        /// </summary>
       
        public static string PublishingProcessCompleted { get; set; } = "{0}\t {1}\t " + "LangKeyPublish".FromResourceDictionary() + "\t" + "LangKeyPublishingProcessCompleted".FromResourceDictionary() + " -[{2}]\t" + CodeConstants.PublishingProcessCompleted;
        public static string ProxyVerificationStarted { get; set; } = "{0}\t {1}\t " + "LangKeyProxyVerification".FromResourceDictionary() + "\t" + "LangKeyProxyVerificationStarted".FromResourceDictionary() + "\t" + CodeConstants.ProxyVerificationStarted;
        public static string ProxyVerificationCompleted { get; set; } = "{0}\t {1}\t " + "LangKeyProxyVerification".FromResourceDictionary() + "\t" + "LangKeyProxyVerificationCompleted".FromResourceDictionary() + "\t" + CodeConstants.ProxyVerificationCompleted;

    }
}