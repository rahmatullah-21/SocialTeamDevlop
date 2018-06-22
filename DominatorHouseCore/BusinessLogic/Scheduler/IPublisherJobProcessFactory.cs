using System.Collections.Generic;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IPublisherJobProcessFactory
    {
        PublisherJobProcess Create(string campaignId,string accountId, List<string> groupLists, List<string> pageLists,bool isPublishOnOwnWall);
    }
}