namespace DominatorHouseCore.Utility
{
    public static class Log
    {
        public static string StartingJob { get; set; } = "{0}\t {1}\t Started job to {2}.";
        public static string JobCompleted { get; set; } = "{0}\t {1}\t successfully complete job to {2}";
        public static string AccountLogin { get; set; } = "{0}\t {1}\t Attempt to login";
        public static string SuccessfulLogin { get; set; } = "{0}\t {1}\t Login successful.";
        public static string LoginFailed { get; set; } = "{0}\t {1}\t Login failed with error: {2}";
        public static string StartedActivity { get; set; } = "{0}\t {1}\t Trying to {2} {3}";
        public static string ActivitySuccessful { get; set; } = "{0}\t {1}\t successful to {2} {3}";
        public static string ActivityFailed { get; set; } = "{0}\t {1}\t failed to {2} {3} with error: {4}";
        public static string JobPaused { get; set; } = "{0}\t {1}\t stopped current job to {2} as {3}";
        public static string SavedCampaign { get; set; } = "{0}\t {1}\t successfully saved";
        public static string CampaignDeleted { get; set; } = "{0}\t {1}\t successfully deleted.";
        public static string CampaignUpdated { get; set; } = "{0}\t {1}\t successfully updated.";
        public static string CampaignPaused { get; set; } = "{0}\t {1}\t successfully paused.";
        public static string ActivatedCampaign { get; set; } = "{0}\t {1}\t actived successfully.";
        public static string UpdatingDetails { get; set; } = "{0}\t {1}\t Started {2} synchronization.";
        public static string DetailsUpdated { get; set; } = "{0}\t {1}\t synchronizing {2} Successful.";
        public static string UploadedAccount { get; set; } = "Successfully added {0} account to {1}";
        public static string DeletedAccounts { get; set; } = "Deleted {0} accounts from {1}";
        public static string AccountEdited { get; set; } = "{0}\t {1}\t details updated successfully.";
        public static string DelayBetweenActivity { get; set; } = "{0}\t {1}\t Next operation to {2} will perform in {3} seconds.";
        public static string NextScheduledJob { get; set; } = "{0}\t {1}\t Next job to {2} is scheduled to run by {3}";
        public static string JobLimitReached { get; set; } = "{0}\t {1}\t has reached per job limit of {2}";
        public static string DailyLimitReached { get; set; } = "{0}\t {1}\t has reached per day limit of {2}";
        public static string HourlyLimitReached { get; set; } = "{0}\t {1}\t has reached per hour limit of {2}";
        public static string WeeklyLimitReached { get; set; } = "{0}\t {1}\t has reached per week limit of {2}";
        public static string OtherConfigurationStarted { get; set; } = "{0}\t {1}\t other configuration for {2} is started";
        public static string OtherCongigurationCompleted { get; set; } = "{0}\t {1}\t other configuration for {2} is completed.";
        public static string FilterApplied { get; set; } = "{0}\t {1}\t applied filter to {2} search results";
        public static string DetailsScraped { get; set; } = "{0}\t {1}\t found {2} ScrapeType to activity.";
        public static string ManagedBlacklist { get; set; } = "{0}\t {1}\t removed {2} users belonging to blacklist to process {3}";
        public static string AddedToBlacklist { get; set; } = "{0}\t {1}\t successfully added {2} to blacklist.";

        public static string FoundXResults { get; set; } = "{0}\t {1}\t Found {2} results by Query type {3} and Query value {4} to {5}";

        public static string AccountNeedsVerification { get; set; } = "{0}\t {1}\t needs to verified to perform next activities.";

        public static string CustomMessage { get; set; } = "{0}\t {1}\t {2}";

    }
}