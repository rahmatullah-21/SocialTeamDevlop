using DominatorHouseCore.Models.SocioPublisher;
using System.Threading;

namespace DominatorHouseCore.Interfaces
{
    public interface IPublisherPostScraper
    {
        /// <summary>
        /// To hold the objects for scarping posts from facebook, pinterest, twitter
        /// </summary>
        /// <returns></returns>
        PostScraper GetPostScraperLibrary(string CampaignId
            , CancellationTokenSource campaignCancellationToken, PublisherPostFetchModel postFetchModel);
    }
}