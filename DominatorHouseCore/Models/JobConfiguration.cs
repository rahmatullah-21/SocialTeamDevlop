using System;
using System.Collections.Generic;
using DominatorHouseCore.Enums;
using DominatorHouseCore.Interfaces;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    /// <summary>
    /// Stores schedule of JobProcess and its activities: delays, timing range, limits per job/hour/day/week
    /// </summary>
    [ProtoContract]
    public class JobConfiguration : BindableBase, IJobConfiguration
    {
        private RangeUtilities _activitiesPerDay;
        private RangeUtilities _delayBetweenActivity;
        private RangeUtilities _delayBetweenJobs;
        private RangeUtilities _activitiesPerJob;
        private RangeUtilities _activitiesPerHour;
        private RangeUtilities _activitiesPerWeek;
        private IncreaseActivityRange _increaseActivitiesEachDay;
        private List<RunningTimes> _runningTime;
        private string _selectedItem = string.Empty;
        private bool _isAdvanceSetting;

        public JobConfiguration()
        {
            //  Delay between each operations (seconds)
            DelayBetweenActivity = new RangeUtilities(0, 0);

            //  Delay between jobs (minutes)
            DelayBetweenJobs = new RangeUtilities(0, 0);

            // Number of <activities> per Job (users)
            ActivitiesPerJob = new RangeUtilities(0, 0);

            // Number of <activities> per Hour (users)
            ActivitiesPerHour = new RangeUtilities(0, 0);

            // Number of <activities> per Day (users)
            ActivitiesPerDay = new RangeUtilities(0, 0);

            // Number of <activities> per Week (users)
            ActivitiesPerWeek = new RangeUtilities(0, 0);

            // Increase each day with 10 until it reaches 100 max <activity> per day
            IncreaseActivitiesEachDay = new IncreaseActivityRange(0, 0, false);

        }


        #region IJobConfiguration

        [ProtoMember(1)]
        public RangeUtilities DelayBetweenActivity
        {
            get { return _delayBetweenActivity; }
            set
            {
                if (_delayBetweenActivity == value)
                    return;
                SetProperty(ref _delayBetweenActivity, value);
            }
        }

        [ProtoMember(2)]
        public RangeUtilities DelayBetweenJobs
        {
            get { return _delayBetweenJobs; }
            set
            {
                if (_delayBetweenJobs == value)
                    return;
                SetProperty(ref _delayBetweenJobs, value);
            }
        }

        [ProtoMember(3)]
        public RangeUtilities ActivitiesPerJob
        {
            get
            {
                return _activitiesPerJob;
            }
            set
            {
                if (_activitiesPerJob == value)
                    return;
                SetProperty(ref _activitiesPerJob, value);
            }
        }

        [ProtoMember(4)]
        public RangeUtilities ActivitiesPerHour
        {
            get { return _activitiesPerHour; }
            set
            {
                if (_activitiesPerHour == value)
                    return;
                SetProperty(ref _activitiesPerHour, value);
            }
        }

        [ProtoMember(5)]
        public RangeUtilities ActivitiesPerDay
        {
            get { return _activitiesPerDay; }
            set
            {
                if (_activitiesPerDay == value)
                    return;
                SetProperty(ref _activitiesPerDay, value);
            }
        }

        [ProtoMember(6)]
        public RangeUtilities ActivitiesPerWeek
        {
            get { return _activitiesPerWeek; }
            set
            {
                if (_activitiesPerWeek == value)
                    return;
                SetProperty(ref _activitiesPerWeek, value);
            }
        }

        [ProtoMember(7)]
        public IncreaseActivityRange IncreaseActivitiesEachDay
        {
            get { return _increaseActivitiesEachDay; }
            set
            {
                if (_increaseActivitiesEachDay == value)
                    return;
                SetProperty(ref _increaseActivitiesEachDay, value);
            }
        }

        // Day of Week and Time interval when Activity will be active
        [ProtoMember(8)]
        public List<RunningTimes> RunningTime
        {
            get
            {
                return _runningTime;
            }
            set
            {

                if (_runningTime == value)
                    return;
                SetProperty(ref _runningTime, value);
            }
        }

        [ProtoMember(9)]
        public string ActivitiesPerJobDisplayName { get; set; } = string.Empty;

        [ProtoMember(10)]
        public string ActivitiesPerHourDisplayName { get; set; } = string.Empty;

        [ProtoMember(11)]
        public string ActivitiesPerDayDisplayName { get; set; } = string.Empty;

        [ProtoMember(12)]
        public string ActivitiesPerWeekDisplayName { get; set; } = string.Empty;

        [ProtoMember(13)]
        public string IncreaseActivityDisplayName { get; set; } = string.Empty;

        [ProtoMember(14)]
        public bool IsAdvanceSetting
        {
            get { return _isAdvanceSetting; }
            set
            {
                if (_isAdvanceSetting == value)
                    return;
                SetProperty(ref _isAdvanceSetting, value);
            }
        }

        [ProtoMember(15)]
        public string SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (_selectedItem == value)
                    return;
                SetProperty(ref _selectedItem, value);
            }
        }

        public List<string> Speeds { get; set; } = new List<string>();

        #endregion
    }
}