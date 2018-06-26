using System.Collections.Generic;
using System.Threading;
using DominatorHouseCore.Models.SocioPublisher;
using DominatorHouseCore.Process;

namespace DominatorHouseCore.BusinessLogic.Scheduler
{
    public interface IPublisherJobProcessFactory
    {
        PublisherJobProcess Create(string campaignId,
            string accountId,
            List<string> groupLists,
            List<string> pageLists,
            List<PublisherCustomDestinationModel> customDestinationModels,
            bool isPublishOnOwnWall,
            CancellationTokenSource campaignCancellationToken);
    }
}