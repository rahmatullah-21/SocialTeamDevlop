using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IPublisherJobProcessFactory
    {
        PublisherJobProcess Create(string campaignId);
    }
}