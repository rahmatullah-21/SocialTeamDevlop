namespace DominatorHouseCore.Process
{
    public abstract class PublisherJobProcess
    {

        #region Constructor

        public PublisherJobProcess(string campaignId)
        {
            CampaignId = campaignId;
        }

        #endregion

        #region Properties

        public string CampaignId { get; set; }

        #endregion

        #region Methods

        

        public void StopPublish()
        {
            // Todo : Stop publish with cancellation token
        }


        public void DelayBeforeNextPublish()
        {
            // Todo : Delays
        }

        protected abstract bool CheckLimitsReached();

        #endregion
    }

    public class PublishScheduler
    {
        public void StartPublish()
        {
            // Todo : Start publish
        }

        public static void ScheduleTodaysJob(string campaignId)
        {
            
        }
    }
}