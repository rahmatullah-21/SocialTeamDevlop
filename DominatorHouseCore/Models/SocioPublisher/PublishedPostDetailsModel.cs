using ProtoBuf;
using System;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [Serializable]
    [ProtoContract]
    public class PublishedPostDetailsModel
    {
        [ProtoMember(1)]
        public string AccountName { get; set; }
        [ProtoMember(2)]
        public string Destination { get; set; }
        [ProtoMember(3)]
        public string DestinationUrl { get; set; }
        [ProtoMember(4)]
        public string Description { get; set; }
        [ProtoMember(5)]
        public string IsPublished { get; set; }
        [ProtoMember(6)]
        public string Successful { get; set; }
        [ProtoMember(7)]
        public string PublishedDate { get; set; }
        [ProtoMember(8)]
        public string Link { get; set; }

    }
}