using DominatorHouseCore.Models.SocioPublisher;

namespace DominatorHouseCore.Interfaces
{
    public abstract class PublishingPost
    {
        public virtual bool PublishOnGroups(string accountId, string groupUrl, PublisherPostlistModel postDetails) => false;

        public virtual bool PublishOnPages(string accountId, string pageUrl, PublisherPostlistModel postDetails) => false;

        public virtual bool PublishOnOwnWall(string accountId, PublisherPostlistModel postDetails) => false;

    }
}