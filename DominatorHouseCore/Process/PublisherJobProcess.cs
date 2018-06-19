using DominatorHouseCore.Models.Publisher;

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

        public JobConfigurationModel JobConfigurations { get; set; } = new JobConfigurationModel();

        public OtherConfigurationModel OtherConfiguration { get; set; } = new OtherConfigurationModel();

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
}