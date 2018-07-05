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
        public static string StartedActivity { get; set; } = "{0}\t {1}\t Trying to {2} {3}";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// </summary>
        public static string ActivitySuccessful { get; set; } = "{0}\t {1}\t successful to {2} {3}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ObjectId
        /// 4 = SocialNetworkError
        /// </summary>
        public static string ActivityFailed { get; set; } = "{0}\t {1}\t failed to {2} {3} with error: {4}";

        public static string ImportFailed { get; set; } = "{0}\t {1}\t failed because of {2} is/are not correct ";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// 3 = ReasonToPause 
        /// </summary>
        public static string JobPaused { get; set; } = "{0}\t {1}\t stopped current job to {2} as {3}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string SavedCampaign { get; set; } = "{0}\t {1}\t successfully saved";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignDeleted { get; set; } = "{0}\t {1}\t successfully deleted.";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignUpdated { get; set; } = "{0}\t {1}\t successfully updated.";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string CampaignPaused { get; set; } = "{0}\t {1}\t successfully paused.";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = CampaignName
        /// </summary>
        public static string ActivatedCampaign { get; set; } = "{0}\t {1}\t actived successfully.";


        public static string UpdatingDetails { get; set; } = "{0}\t {1}\t Started {2} synchronization.";
        public static string DetailsUpdated { get; set; } = "{0}\t {1}\t synchronizing {2} Successful.";
        public static string UploadedAccount { get; set; } = "Successfully added {0} account to {1}";
        public static string SelectedAccount { get; set; } = "{0}\t Successfully added {1} account to {2}";
        /// <summary>
        /// 0 = NumberOfAccounts
        /// 1 = PlatformName
        /// </summary>
        public static string DeletedAccounts { get; set; } = "Deleted {0} accounts from {1}";

        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// </summary>
        public static string AccountEdited { get; set; } = "{0}\t {1}\t details updated successfully.";

        /// <summary>
        /// 0 = account.SocialNetwork
        /// 1 = account.Username
        /// 2 = ActivityType
        /// 3 = DelaySeconds
        /// </summary>
        public static string DelayBetweenActivity { get; set; } = "{0}\t {1}\t Next operation to {2} will perform in {3} seconds.";
        public static string NextScheduledJob { get; set; } = "{0}\t {1}\t Next job to {2} is scheduled to run by {3}";
        public static string NextJobExpectedToStartBy { get; set; } = "{0}\t {1}\t Next job to {2} is expected to start by {3}";
        public static string JobLimitReached { get; set; } = "{0}\t {1}\t has reached per job limit of {2}";
        public static string DailyLimitReached { get; set; } = "{0}\t {1}\t has reached per day limit of {2}";
        public static string HourlyLimitReached { get; set; } = "{0}\t {1}\t has reached per hour limit of {2}";
        public static string WeeklyLimitReached { get; set; } = "{0}\t {1}\t has reached per week limit of {2}";
        public static string LimitReached { get; set; } = "{0}\t {1}\t has reached {2} limit of {3}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string OtherConfigurationStarted { get; set; } = "{0}\t {1}\t other configuration for {2} is started";
        public static string OtherCongigurationCompleted { get; set; } = "{0}\t {1}\t other configuration for {2} is completed.";
        public static string FilterApplied { get; set; } = "{0}\t {1}\t applied filter to {2} search results";
        public static string DetailsScraped { get; set; } = "{0}\t {1}\t found {2} ScrapeType to activity.";
        public static string ManagedBlacklist { get; set; } = "{0}\t {1}\t removed {2} users belonging to blacklist to process {3}";
        public static string AddedToBlacklist { get; set; } = "{0}\t {1}\t successfully added {2} to blacklist.";

        public static string NoMoreDataToPerform { get; set; } = "{0}\t {1}\t No more data available to perform {2}";

        public static string FoundXResults { get; set; } = "{0}\t {1}\t Found {2} results by Query type {3} and Query value {4} to {5}";
        public static string AlreadyExistQuery { get; set; } = "{0}\t {1} index(es) are already added in {2} search query(ies)";
        public static string AlreadyExistQueryCount { get; set; } = "{0}\t {1} are already added in {2} search query(ies)";

        public static string AccountNeedsVerification { get; set; } = "{0}\t {1}\t needs to verified to perform next activities.";

        public static string CustomMessage { get; set; } = "{0}\t {1}\t {2}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessCompleted { get; set; } = "{0}\t {1}\t successfully complete process to {2}";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStarted { get; set; } = "{0}\t {1}\t Started process to {2}.";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// 2 = ActivityType
        /// </summary>
        public static string ProcessStopped { get; set; } = "{0}\t {1}\t Stopped process to {2}.";

        public static string CampaignNotSet { get; set; } = "{0}\t {1}\t Stopped process to {2}.";


        /// <summary>
        /// 0 = SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Deleted { get; set; } = "{0}\t {1}\t successfully Deleted";

        /// <summary>
        /// 0 = SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Added { get; set; } = "{0}\t {1}\t successfully Added";
        /// <summary>
        /// 0 =  SocialNetwork
        /// 1 = Content
        /// </summary>
        public static string Exported { get; set; } = "{0}\t {1}\t successfully Exported";
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string NotAddedAccount { get; set; } = "{0}\t {1}\t Having issues to add!";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyAddedAccount { get; set; } = "{0}\t {1}\t Already added!";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyStoppedUpdatingAccount { get; set; } = "{0}\t {1}\t Already Stopped!";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string AlreadyUpdatingAccount { get; set; } = "{0}\t {1}\t Already started!";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username
        /// </summary>
        public static string StopUpdatingAccount { get; set; } = "{0}\t {1}\t Stopped for further friendship update!";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = campaign name    
        /// </summary>
        public static string StartPublishing { get; set; } = "{0}\t {1}\t Publishing Started with {2}";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = delay in seconds 
        /// </summary>
        public static string DelayBetweenPublishing { get; set; } = "{0}\t {1}\t Next post will start publishing in {2} seconds";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username          
        /// </summary>
        public static string AlreadyPublishedOnOwnWall { get; set; } = "{0}\t {1}\t Post has already posted on own wall/profile";

        //Post has already posted with destintion

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Destination Type
        /// 3 = Destination Url        
        /// </summary>
        public static string AlreadyPublishedOnDestination { get; set; } = "{0}\t {1}\t Post has already posted with destintion {2} - {3}";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username         
        /// </summary>
        public static string PostExpired { get; set; } = "{0}\t {1}\t Post already expired.";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string PublishingSuccessfully { get; set; } = "{0}\t {1}\t Published successfully on {2} [{3}]";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string PublishingFailed { get; set; } = "{0}\t {1}\t Error while publishing on {2} [{3}]";
        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string SharedSuccessfully { get; set; } = "{0}\t {1}\t Shared successfully on {2} [{3}]";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username   
        /// 2 = Destination type
        /// 3 = Destination Url    
        /// </summary>
        public static string ShareFailed { get; set; } = "{0}\t {1}\t Error while sharing on {2} [{3}]";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMedia { get; set; } = "{0}\t {1}\t Uploading media file [{2}]";

        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaSuccessful { get; set; } = "{0}\t {1}\t Successfully uploaded media file [{2}]";



        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Post Source
        /// </summary>
        public static string UploadingMediaFailed { get; set; } = "{0}\t {1}\t Error while uploading media file [{2}]";


        /// <summary>
        /// 0 = Account's SocialNetwork
        /// 1 = Account's Username  
        /// 2 = Failed reason
        /// </summary>
        public static string UploadingMediaFailedReason { get; set; } = "{0}\t {1}\t Failed due to {2}";

       
    }
}