using System.Collections.Generic;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
 
    [ProtoContract]
    public class JobConfiguration : IJobConfiguration
    {

        public JobConfiguration()
        {
            ActivitiesDelay = new RangeUtilities(10, 20);
            JobsDelay = new RangeUtilities(10, 20);
            JobsActivityCount = new RangeUtilities(10, 20);
            HoursActivityCount = new RangeUtilities(10, 20);
            DaysActivityCount = new RangeUtilities(10, 20);
            WeeksActivityCount = new RangeUtilities(10, 20);
            IncreaseActivity = new IncreaseActivityRange(10, 100, false);
         
        }


        #region IJobConfiguration

        [ProtoMember(1)]
        public RangeUtilities ActivitiesDelay { get; set; }

        [ProtoMember(2)]
        public RangeUtilities JobsDelay { get; set; }

        [ProtoMember(3)]
        public RangeUtilities JobsActivityCount { get; set; }

        [ProtoMember(4)]
        public RangeUtilities HoursActivityCount { get; set; }

        [ProtoMember(5)]
        public RangeUtilities DaysActivityCount { get; set; }

        [ProtoMember(6)]
        public RangeUtilities WeeksActivityCount { get; set; }

        [ProtoMember(7)]
        public IncreaseActivityRange IncreaseActivity { get; set; }

        [ProtoMember(8)]
        public List<RunningTimes> RunningTime { get; set; }

        [ProtoMember(9)]
        public string JobsActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(10)]
        public string HoursActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(11)]
        public string DaysActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(12)]
        public string WeeksActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(13)]
        public string IncreaseActivityDisplayName { get; set; } = string.Empty;

        #endregion

    }
}