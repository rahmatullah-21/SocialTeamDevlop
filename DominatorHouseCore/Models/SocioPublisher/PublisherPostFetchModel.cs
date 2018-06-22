using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string CampaignName { get; set; } = string.Empty;

        [ProtoMember(3)]
        public PostSource PostSource { get; set; } = PostSource.ScrapedPost;

        [ProtoMember(4)]
        public string PostDetailsWithFilters { get; set; } = string.Empty;

        [ProtoMember(5)]
        public DateTime ExpireDate { get; set; } = DateTime.Now.AddYears(1);

        [ProtoMember(6)]
        public int DelayForNext { get; set; } = 30;

        [ProtoMember(7)]
        public int MaximumPostLimitToStore { get; set; }

        [ProtoMember(8)]
        public ObservableCollection<string> SelectedDestinations { get; set; } = new ObservableCollection<string>();

    }
}