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
        public static string StartingJob { get; set; } = "{0}\t {1}\t {2}\t LangKeyStartedJobTo {2}. \t"+ CodeConstants.StartedJob;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string JobCompleted { get; set; } = "{0}\t {1}\t {2}\t LangKeySuccessfullyCompleteJobTo {2}. \t" + CodeConstants.CompletedJob;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AccountLogin { get; set; } = "{0}\t {1}\t Login \t " + "LangKeyAttemptToLogin".FromResourceDictionary() + " {2}. \t" + CodeConstants.LoginAttempt;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string SuccessfulLogin { get; set; } = "{0}\t {1}\t Login Success \t " + "LangKeyLoginSuccessful".FromResourceDictionary() + " {2}. \t" + CodeConstants.LoginSuccessful;

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string LoginFailed { get; set; } = "{0}\t {1}\t Login Failed \t " + "LangKeyLoginFailedWithError".FromResourceDictionary() + " {2}. \t" + CodeConstants.LoginFailed;

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
        public static string ActivitySuccessful { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfulTo".FromResourceDictionary() + " {2} {3} \t" + CodeConstants.ActivitySuccessful;
          

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// 4 = SocialNetworkError
        /// </summary>
        public static string ActivityFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFailedTo".FromResourceDictionary() + " {2}" + "LangKeyWithError".FromResourceDictionary() + "{3}\t" + CodeConstants.ActivityFailed;


        public static string ImportFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFailedBecauseOf".FromResourceDictionary() + " {2}.\t" + "LangKeyIs/AreNotCorrect".FromResourceDictionary() + "\t" + CodeConstants.ImportFailed;
           
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ReasonToPause 
        /// </summary>
        public static string JobPaused { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedCurrentJobTo".FromResourceDictionary() + " {2}.\t" + "LangKeyAs".FromResourceDictionary() + "\t" + CodeConstants.JobPaused;
        

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string SavedCampaign { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullySaved".FromResourceDictionary() + "\t" + CodeConstants.SavedCampaign;
           

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignDeleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyDeleted".FromResourceDictionary() + "\t" + CodeConstants.CampaignDeleted;
        

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignUpdated { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyUpdated".FromResourceDictionary() + "\t" + CodeConstants.CampaignUpdated;
         

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignPaused { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyPaused".FromResourceDictionary() + "\t" + CodeConstants.CampaignPaused;
           

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string ActivatedCampaign { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyActivatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.ActivatedCampaign;
    


        public static string UpdatingDetails { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStarted".FromResourceDictionary() + " {2}.\t" + "LangKeySynchronization".FromResourceDictionary() + "\t" + CodeConstants.UpdatingDetails;
           
        public static string DetailsUpdated { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySynchronization".FromResourceDictionary() + " {2}.\t" + "LangKeySuccessful".FromResourceDictionary() + "\t" + CodeConstants.DetailsUpdated;

        public static string UploadedAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + "\t" + "LangKeyAccountTo".FromResourceDictionary() + "\t {2}.\t" + CodeConstants.UploadedAccount;
            
        public static string SelectedAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + "\t" + "LangKeyAccountTo".FromResourceDictionary() + "\t {2}.\t" + CodeConstants.SelectedAccount;
        
        /// <summary>
        /// 0 = NumberOfAccounts
        /// 1 = PlatformName
        /// </summary>
        public static string DeletedAccounts { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyDeleted".FromResourceDictionary() + "\t" + "LangKeyAccountsFrom".FromResourceDictionary() + "\t {2}.\t" + CodeConstants.DeletedAccounts;
         

        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// </summary>
        public static string AccountEdited { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyDetailsUpdatedSuccessfully".FromResourceDictionary() + "\t" + CodeConstants.AccountEdited;
       

        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// 2 = ActivityType
        /// 3 = DelaySeconds
        /// </summary>
        public static string DelayBetweenActivity { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextOperationTo".FromResourceDictionary() + " {2}.\t" + "LangKeyWillPerformIn".FromResourceDictionary() + "\t{2}" + "LangKeySeconds".FromResourceDictionary() + "\t" + CodeConstants.DelayBetweenActivity;


        public static string NextScheduledJob { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextJobTo".FromResourceDictionary() + " {2}.\t" + "LangKeyIsScheduledToRunBy".FromResourceDictionary() + "\t{2}\t"  + CodeConstants.NextScheduledJob;
  
        public static string NextJobExpectedToStartBy { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNextJobTo".FromResourceDictionary() + " {2}.\t" + "LangKeyIsExpectedToStartBy".FromResourceDictionary() + "\t{2}\t" + CodeConstants.NextJobExpectedToStartBy;

        public static string JobLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerJobLimitOf".FromResourceDictionary() + " {2}. \t" + CodeConstants.JobLimitReached;
           
        public static string DailyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerDayLimitOf".FromResourceDictionary() + " {2}. \t" + CodeConstants.DailyLimitReached;
          
        public static string HourlyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerHourLimitOf".FromResourceDictionary() + " {2}. \t" + CodeConstants.HourlyLimitReached;
         
        public static string WeeklyLimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReachedPerWeekLimitOf".FromResourceDictionary() + " {2}. \t" + CodeConstants.WeeklyLimitReached;
         
        public static string LimitReached { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHasReached".FromResourceDictionary() + " {2}.\t" + "LangKeyLimitOf".FromResourceDictionary() + "\t{2}\t" + CodeConstants.LimitReached;


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string OtherConfigurationStarted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyOtherConfigurationFor".FromResourceDictionary() + " {2}.\t" + "LangKeyIsStarted".FromResourceDictionary() + "\t{2}\t" + CodeConstants.OtherConfigurationStarted;
       
        public static string OtherCongigurationCompleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyOtherConfigurationFor".FromResourceDictionary() + " {2}.\t" + "LangKeyIsCompleted".FromResourceDictionary() + "\t{2}\t" + CodeConstants.OtherCongigurationCompleted;

        public static string FilterApplied { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAppliedFilterTo".FromResourceDictionary() + " {2}.\t" + "LangKeySearchResults".FromResourceDictionary() + "\t{2}\t" + CodeConstants.FilterApplied;

        public static string DetailsScraped { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFound".FromResourceDictionary() + " {2}.\t" + "LangKeyScrapetypeToActivity".FromResourceDictionary() + "\t{2}\t" + CodeConstants.DetailsScraped;
        public static string ManagedBlacklist { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyRemoved".FromResourceDictionary() + " {2}.\t" + "LangKeyUsersBelongingToBlacklistToProcess".FromResourceDictionary() + "\t{2}\t" + CodeConstants.ManagedBlacklist;
        public static string AddedToBlacklist { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyAdded".FromResourceDictionary() + " {2}.\t" + "LangKeyToBlacklist".FromResourceDictionary() + "\t{2}\t" + CodeConstants.AddedToBlacklist;
        public static string NoMoreDataToPerform { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNoMoreDataAvailableToPerform".FromResourceDictionary() + " {2}.\t"  + CodeConstants.NoMoreDataToPerform;
        public static string FoundXResults { get; set; } =   "{0}\t {1}\t Found {2} results by Query type {3} and Query value {4} to {5}";
        public static string AlreadyExistQuery { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyIndexIesAreAlreadyAddedIn".FromResourceDictionary() + " {2}.\t" + "LangKeySearchQueryIes".FromResourceDictionary() + "\t{2}\t" + CodeConstants.AlreadyExistQuery;
          
        public static string AlreadyExistQueryCount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAreAlreadyAddedIn".FromResourceDictionary() + " {2}.\t" + "LangKeySearchQueryIes".FromResourceDictionary() + "\t{2}\t" + CodeConstants.AlreadyExistQueryCount;
           
        public static string AccountNeedsVerification { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyNeedsToVerifiedToPerformNextActivities".FromResourceDictionary() + "\t" + CodeConstants.AccountNeedsVerification;
    
        public static string CustomMessage { get; set; } = "{0}\t {1}\t {2}\t {3}\t {4}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessCompleted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyCompleteProcessTo".FromResourceDictionary() + "{2}\t" + CodeConstants.ProcessCompleted;
         
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStarted { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStartedProcessTo".FromResourceDictionary() + "{2}\t" + CodeConstants.ProcessStarted;
           

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStopped { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedProcessTo".FromResourceDictionary() + "{2}\t" + CodeConstants.ProcessStopped;
        
        public static string CampaignNotSet { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedProcessTo".FromResourceDictionary() + "{2}\t" + CodeConstants.ProcessStopped;
            

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
        public static string NotAddedAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyHavingIssuesToAdd".FromResourceDictionary() + "\t" + CodeConstants.NotAddedAccount;
           
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyAddedAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAlreadyAdded".FromResourceDictionary() + "\t" + CodeConstants.AlreadyAddedAccount;
           

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyStoppedUpdatingAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAlreadyStopped".FromResourceDictionary() + "\t" + CodeConstants.AlreadyStoppedUpdatingAccount;
        
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyUpdatingAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyAlreadyStarted".FromResourceDictionary() + "\t" + CodeConstants.AlreadyUpdatingAccount;
           

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string StopUpdatingAccount { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyStoppedForFurtherFriendshipUpdate".FromResourceDictionary() + "\t" + CodeConstants.StopUpdatingAccount;
        

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = campaign name    
        /// </summary>
        public static string StartPublishing { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPublishingStartedWith".FromResourceDictionary() + "{2}\t" + CodeConstants.StartPublishing;



        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = delay in seconds 
        /// </summary>
        public static string DelayBetweenPublishing { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPublishingStartedWith".FromResourceDictionary() + "{2} " + "LangKeySeconds".FromResourceDictionary()+"\t" + CodeConstants.DelayBetweenPublishing;
     

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username          
        /// </summary>
        public static string AlreadyPublishedOnOwnWall { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPostHasAlreadyPostedOnOwnWallProfile".FromResourceDictionary() + "\t" + CodeConstants.AlreadyPublishedOnOwnWall;

        //Post has already posted with destintion

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Destination Type
        /// 3 = Destination Url        
        /// </summary>
        public static string AlreadyPublishedOnDestination { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPostHasAlreadyPostedWithDestintion".FromResourceDictionary() + "{2}-{2}\t" + CodeConstants.AlreadyPublishedOnDestination;
    

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
        public static string PublishingSuccessfully { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyPublishedSuccessfullyOn".FromResourceDictionary() + "{2}-[{2}]\t" + CodeConstants.PublishingSuccessfully;
         
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string PublishingFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyErrorWhilePublishingOn".FromResourceDictionary() + "{2}-[{2}]\t" + CodeConstants.PublishingFailed;
           
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string SharedSuccessfully { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySharedSuccessfullyOn".FromResourceDictionary() + "{2}-[{2}]\t" + CodeConstants.SharedSuccessfully;
          
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string ShareFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyErrorWhileSharingOn".FromResourceDictionary() + "{2}-[{2}]\t" + CodeConstants.ShareFailed;
            

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMedia { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyUploadingMediaFile".FromResourceDictionary() + "[{2}]\t" + CodeConstants.UploadingMedia;
           
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaSuccessful { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeySuccessfullyUploadedMediaFile".FromResourceDictionary() + "[{2}]\t" + CodeConstants.UploadingMediaSuccessful;
    

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaFailed { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyErrorWhileUploadingMediaFile".FromResourceDictionary() + "[{2}]\t" + CodeConstants.UploadingMediaFailed;
    

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Failed reason
        /// </summary>
        public static string UploadingMediaFailedReason { get; set; } = "{0}\t {1}\t {2}\t " + "LangKeyFailedDueTo".FromResourceDictionary() + "[{2}]\t" + CodeConstants.UploadingMediaFailedReason;
          

       
    }
}