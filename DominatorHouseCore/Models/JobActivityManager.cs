using DominatorHouseCore.Utility;
using ProtoBuf;
using System.Collections.Generic;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class JobActivityManager : BindableBase
    {
        [ProtoMember(2)]
        public List<RunningTimes> RunningTime { get; set; } = new List<RunningTimes>();
    }
}