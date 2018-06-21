using System;
using DominatorHouseCore.Enums.SocioPublisher;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherPostFetchModel
    {
        [ProtoMember(1)]
        public string CampaignId { get; set; } = string.Empty;

        [ProtoMember(2)]
        public PostSource PostSource { get; set; } = PostSource.ScrapedPost;

        [ProtoMember(3)]
        public string PostDetailsWithFilters { get; set; } = string.Empty;

        [ProtoMember(4)]
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(1);

        [ProtoMember(5)]
        public int DelayForNext { get; set; } = 30;
    }
}