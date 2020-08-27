#region

using System;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    /// <summary>
    ///     TimingRange is used to specify the time range such as start time and end time
    /// </summary>
    [ProtoContract]
    public class TimingRange
    {
        public TimingRange()
        {

        }

        // Constructor for initialize the start time and end time to local property
        public TimingRange(TimeSpan startTime, TimeSpan endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
            TimeId = Guid.NewGuid().ToString();
            Module = string.Empty;
        }


        [ProtoMember(1)]
        // Ending time
        public TimeSpan EndTime { get; }

        [ProtoMember(2)]
        // starting time
        public TimeSpan StartTime { get; }

        public string TimeId { get; }


        [ProtoMember(4)]
        // Module specify the module name which is going to run on at particular time
        public string Module { get; set; }
    }
}