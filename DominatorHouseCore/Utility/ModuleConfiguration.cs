using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Models;
using ProtoBuf;

namespace DominatorHouseCore.Utility
{
    [ProtoContract]
    public class ModuleConfiguration
    {
        [ProtoMember(1)]
        public string TemplateId { get; set; }
        [ProtoMember(2)]
        public bool IsEnabled { get; set; }
        [ProtoMember(3)]
        public string Status { get; set; }
        [ProtoMember(4)]
        public int LastUpdatedDate { get; set; } = DateTimeUtilities.GetEpochTime();
        [ProtoMember(5)]
        public List<RunningTimes> LstRunningTimes { get; set; }

        [ProtoMember(6)]
        public ActivityType ActivityType { get; set; }
    }
}