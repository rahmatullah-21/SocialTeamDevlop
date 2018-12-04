using System;
using System.Collections.Generic;
using System.Linq;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Settings
{

    [ProtoContract]
    public class RunningTimeSpanModel : BindableBase
    {
        private ObservableCollectionBase<TimingRange> _timings = new ObservableCollectionBase<TimingRange>();
        private bool _isEnabled;

        /// <summary>
        ///  DayWiseRunningTimeSpan property is initialize the running timespam for all days of the week
        /// </summary>
        public static List<RunningTimeSpanModel> DayWiseRunningTimeSpan
        {

            // Get the all enum values of DayOfWeek and cast them into DayOfWeek type, from that take one by one day
            // initailize the neccessary details such as the job is enabled(IsEnabled),
            // for the day how many time is going to run(Timings) 

            get
            {
                return
                    Enum.GetValues(typeof(DayOfWeek))
                    ?.
                    Cast<DayOfWeek>()
                    ?.
                    Select((Func<DayOfWeek, RunningTimeSpanModel>)
                    (day =>
                    {
                        var model = new RunningTimeSpanModel
                        {
                            Day = day
                        };
                        return model;
                    }))
                    ?.ToList();
            }

        }


        [ProtoMember(1)]
        // Day is used to specify the day of the week 
        public DayOfWeek Day { get; set; }
        [ProtoMember(4)]
        public DayOfWeek SunDay { get; } = DayOfWeek.Sunday;

        [ProtoMember(5)]
        public DayOfWeek Monday { get; } = DayOfWeek.Monday;

        [ProtoMember(6)]
        public DayOfWeek Tuesday { get; } = DayOfWeek.Tuesday;

        [ProtoMember(7)]
        public DayOfWeek Wednesday { get; } = DayOfWeek.Wednesday;

        [ProtoMember(8)]
        public DayOfWeek Thursday { get; } = DayOfWeek.Thursday;

        [ProtoMember(9)]
        public DayOfWeek Friday { get; } = DayOfWeek.Friday;

        [ProtoMember(10)]
        public DayOfWeek Saturday { get; } = DayOfWeek.Saturday;


        [ProtoMember(3)]
        // IsEnabled is whether job configuration are going to run or not
        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set {
                if (value == _isEnabled)
                    return;
                SetProperty(ref _isEnabled, value);
            }
        }


        [ProtoMember(2)]
        // Timings which include all the time spam when the job is going to start
        public ObservableCollectionBase<TimingRange> Timings
        {
            get
            {
                return _timings;
            }
            set
            {
                if (value == _timings)
                    return;
                SetProperty(ref _timings, value);
            }
        }


        // AddTimeRange method is used to add the time range of the job configurations
        public void AddTimeRange(TimingRange range) => Timings.Add(range);

        // Delete the time spam from job configurations
        //public void DeleteTimeRange(string id) => Timings.RemoveWhere((Predicate<TimingRange>)(x =>
        //    string.Equals(id, $"{(object)x.StartTime.Hours}:{(object)x.StartTime.Minutes}:{(object)x.EndTime.Hours}:{(object)x.EndTime.Minutes}", StringComparison.Ordinal)));


        /// <summary>
        /// TimingRange is used to specify the time range such as start time and end time
        /// </summary>
        [ProtoContract]
        public class TimingRange
        {
            // Constructor for initialize the start time and end time to local property
            public TimingRange(TimeSpan startTime, TimeSpan endTime)
            {
                StartTime = startTime;
                EndTime = endTime;
            }

            private TimingRange()
            {

            }

            [ProtoMember(1)]
            // Ending time
            public TimeSpan EndTime { get; }

            [ProtoMember(2)]
            // starting time
            public TimeSpan StartTime { get; }

        }

    }
}
