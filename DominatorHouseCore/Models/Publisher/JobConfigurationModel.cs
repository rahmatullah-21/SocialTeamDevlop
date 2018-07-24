using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher
{
    [ProtoContract]
    public class JobConfigurationModel : BindableBase
    {
        /// <summary>
        /// To Specify the maximum post time
        /// </summary>
        private int _maxPost;
        [ProtoMember(1)]
        public int MaxPost
        {
            get
            {
                return _maxPost;
            }
            set
            {
                if (value == _maxPost)
                    return;
                SetProperty(ref _maxPost, value);
            }
        }


        /// <summary>
        /// To specify the time range when to start the publishing and when to should end the publishing
        /// </summary>
        private TimeRange _timeRange;
        [ProtoMember(2)]
        public TimeRange TimeRange
        {
            get
            {
                return _timeRange;
            }
            set
            {
                if (value == _timeRange)
                    return;
                SetProperty(ref _timeRange, value);
            }
        }

        /// <summary>
        /// To specify whether posting time in specific interval
        /// </summary>
        private bool _isSpecifyPostingIntervalChecked;
        [ProtoMember(4)]
        public bool IsSpecifyPostingIntervalChecked
        {
            get
            {
                return _isSpecifyPostingIntervalChecked;
            }
            set
            {               
                if (value == _isSpecifyPostingIntervalChecked)
                    return;
                SetProperty(ref _isSpecifyPostingIntervalChecked, value);
            }
        }

        /// <summary>
        /// To specify the posting time is in randomize every day 
        /// </summary>
        private bool _isRandomizePublishingTimerChecked;
        [ProtoMember(5)]
        public bool IsRandomizePublishingTimerChecked
        {
            get
            {
                return _isRandomizePublishingTimerChecked;
            }
            set
            {             
                if (value == _isRandomizePublishingTimerChecked)
                    return;
                SetProperty(ref _isRandomizePublishingTimerChecked, value);

            }
        }

        /// <summary>
        /// To specify the publishing count for an specific accounts
        /// </summary>
        private RangeUtilities _postBetween = new RangeUtilities();
        [ProtoMember(7)]
        public RangeUtilities PostBetween
        {
            get
            {
                return _postBetween;
            }
            set
            {
                if (value == _postBetween)
                    return;
                SetProperty(ref _postBetween, value);
            }
        }


        /// <summary>
        /// To Specify the publisher to all postlists
        /// </summary>
        private bool _isPublishPostOnDestinationsChecked;
        [ProtoMember(9)]
        public bool IsPublishPostOnDestinationsChecked
        {
            get
            {
                return _isPublishPostOnDestinationsChecked;
            }
            set
            {
                if (value == _isPublishPostOnDestinationsChecked)
                    return;
                SetProperty(ref _isPublishPostOnDestinationsChecked, value);
            }
        }

        #region Not Used


        private bool _isRandomizeNumberOfPostsChecked;
        [ProtoMember(6)]
        public bool IsRandomizeNumberOfPostsChecked
        {
            get
            {
                return _isRandomizeNumberOfPostsChecked;
            }
            set
            {
                if (value == _isRandomizeNumberOfPostsChecked)
                    return;
                SetProperty(ref _isRandomizeNumberOfPostsChecked, value);
            }
        }

        private RangeUtilities _increaseEachDay = new RangeUtilities();
        [ProtoMember(8)]
        public RangeUtilities IncreaseEachDay
        {
            get
            {
                return _increaseEachDay;
            }
            set
            {
                if (value == _increaseEachDay)
                    return;
                SetProperty(ref _increaseEachDay, value);
            }
        }

        private bool _isAddRandomSleepTimeWhilePublishingChecked;
        [ProtoMember(10)]
        public bool IsAddRandomSleepTimeWhilePublishingChecked
        {
            get
            {
                return _isAddRandomSleepTimeWhilePublishingChecked;
            }
            set
            {
                if (value == _isAddRandomSleepTimeWhilePublishingChecked)
                    return;
                SetProperty(ref _isAddRandomSleepTimeWhilePublishingChecked, value);
            }
        }


        private bool _isSleepBetweenChecked;
        [ProtoMember(11)]
        public bool IsSleepBetweenChecked
        {
            get
            {
                return _isSleepBetweenChecked;
            }
            set
            {
                if (value == _isSleepBetweenChecked)
                    return;
                SetProperty(ref _isSleepBetweenChecked, value);
            }
        }


        private RangeUtilities _sleepBetween = new RangeUtilities();
        [ProtoMember(12)]
        public RangeUtilities SleepBetween
        {
            get
            {
                return _sleepBetween;
            }
            set
            {
                if (value == _sleepBetween)
                    return;
                SetProperty(ref _sleepBetween, value);
            }
        }


        private RangeUtilities _sendingBetween = new RangeUtilities();
        [ProtoMember(13)]
        public RangeUtilities SendingBetween
        {
            get
            {
                return _sendingBetween;
            }
            set
            {
                if (value == _sendingBetween)
                    return;
                SetProperty(ref _sendingBetween, value);
            }
        }


        private int _maxDestination;
        [ProtoMember(28)]
        public int MaxDestination
        {
            get
            {
                return _maxDestination;
            }
            set
            {
                if (value == _maxDestination)
                    return;
                SetProperty(ref _maxDestination, value);
            }
        }


        private bool _isAddDelayBetweenPublishingPost;
        [ProtoMember(29)]
        public bool IsAddDelayBetweenPublishingPost
        {
            get
            {
                return _isAddDelayBetweenPublishingPost;
            }
            set
            {
                if (value == _isAddDelayBetweenPublishingPost)
                    return;
                SetProperty(ref _isAddDelayBetweenPublishingPost, value);
            }
        }

        #endregion

        /// <summary>
        /// To specify whether campaign Contains start date
        /// </summary>
        private bool _isCampaignHasStartDateChecked;
        [ProtoMember(14)]
        public bool IsCampaignHasStartDateChecked
        {
            get
            {
                return _isCampaignHasStartDateChecked;
            }
            set
            {
                if (value == _isCampaignHasStartDateChecked)
                    return;
                SetProperty(ref _isCampaignHasStartDateChecked, value);
            }
        }

        /// <summary>
        /// To specify whether campaign contains end date
        /// </summary>
        private bool _isCampaignHasEndDateChecked;
        [ProtoMember(15)]
        public bool IsCampaignHasEndDateChecked
        {
            get
            {
                return _isCampaignHasEndDateChecked;
            }
            set
            {
                if (value == _isCampaignHasEndDateChecked)
                    return;
                SetProperty(ref _isCampaignHasEndDateChecked, value);
            }
        }

        /// <summary>
        /// Is need to rotate the campaigns running day
        /// </summary>
        private bool _isRotateDayChecked;
        [ProtoMember(24)]
        public bool IsRotateDayChecked
        {
            get
            {
                return _isRotateDayChecked;
            }
            set
            {
                if (value == _isRotateDayChecked)
                    return;
                SetProperty(ref _isRotateDayChecked, value);
            }
        }

        /// <summary>
        /// To specify the selected days
        /// </summary>
        private List<ContentSelectGroup> _weekday = new List<ContentSelectGroup>();
        [ProtoMember(25)]
        public List<ContentSelectGroup> Weekday
        {
            get { return _weekday; }
            set
            {                
                SetProperty(ref _weekday, value);
            }
        }

        /// <summary>
        /// To specify the running time
        /// </summary>
        private ObservableCollection<TimeSpanHelper> _lstTimer = new ObservableCollection<TimeSpanHelper>();
        [ProtoMember(26)]
        public ObservableCollection<TimeSpanHelper> LstTimer
        {
            get
            {
                return _lstTimer;
            }
            set
            {
                if (value == _lstTimer)
                    return;
                SetProperty(ref _lstTimer, value);
            }
        }

        /// <summary>
        /// To specify the random destinations count 
        /// </summary>
        private int _randomDestinationCount = 2;
        [ProtoMember(27)]
        public int RandomDestinationCount
        {
            get
            {
                return _randomDestinationCount;
            }
            set
            {
                if (value == _randomDestinationCount)
                    return;
                SetProperty(ref _randomDestinationCount, value);
            }
        }


      

        /// <summary>
        /// To specify the delay between each post
        /// </summary>
        private RangeUtilities _delayBetween = new RangeUtilities(10,30);
        [ProtoMember(30)]
        public RangeUtilities DelayBetween
        {
            get
            {
                return _delayBetween;
            }
            set
            {
                if (value == _delayBetween)
                    return;
                SetProperty(ref _delayBetween, value);
            }
        }

        /// <summary>
        /// To specify the post range for campaign
        /// </summary>
        private RangeUtilities _postRange = new RangeUtilities(2,4);
        [ProtoMember(31)]
        public RangeUtilities PostRange
        {
            get
            {
                return _postRange;
            }
            set
            {
                if (value == _postRange)
                    return;
                SetProperty(ref _postRange, value);
            }
        }

        /// <summary>
        /// To specify the delay in minutes for multiple posts
        /// </summary>
        private RangeUtilities _delayBetweenPost = new RangeUtilities(10,30);
        [ProtoMember(32)]
        public RangeUtilities DelayBetweenPost
        {
            get
            {
                return _delayBetweenPost;
            }
            set
            {
                if (value == _delayBetweenPost)
                    return;
                SetProperty(ref _delayBetweenPost, value);
            }
        }

        /// <summary>
        /// To specify to start date of the campaign
        /// </summary>
        private DateTime? _campaignStartDate;
        [ProtoMember(33)]
        public DateTime? CampaignStartDate
        {
            get
            {
                return _campaignStartDate;
            }
            set
            {
                if (value == _campaignStartDate)
                    return;
                SetProperty(ref _campaignStartDate, value);
            }
        }


        /// <summary>
        /// To specify the campaign end date
        /// </summary>
        private DateTime? _campaignEndDate;
        [ProtoMember(34)]
        public DateTime? CampaignEndDate
        {
            get
            {
                return _campaignEndDate;
            }
            set
            {
                if (value == _campaignEndDate)
                    return;
                SetProperty(ref _campaignEndDate, value);
            }
        }


        public void InitializeDefaultJobConfiguration()
        {
            Weekday.Clear();

            foreach (var day in Enum.GetValues(typeof(DayOfWeek)))
            {
                Weekday.Add(new ContentSelectGroup
                {
                    Content = day.ToString(),
                    IsContentSelected = true
                });
            }

            TimeRange = new TimeRange(new TimeSpan(0, 0, 0), new TimeSpan(23, 59, 59));
            IsSpecifyPostingIntervalChecked = true;
        }
    }

    [Serializable]
    [ProtoContract]
    public class TimeRange : BindableBase
    {
        public TimeRange()
        {
            
        }

        // Constructor for initialize the start time and end time to local property
        public TimeRange(TimeSpan startTime, TimeSpan endTime)
        {
            this.StartTime = startTime;
            this.EndTime = endTime;

        }
        /// <summary>
        /// To specify the start time
        /// </summary>
        private TimeSpan _startTime;
        [ProtoMember(1)]
        public TimeSpan StartTime
        {
            get
            {
                return _startTime;
            }
            set
            {
                if (value == _startTime)
                    return;
                SetProperty(ref _startTime, value);
            }
        }

        /// <summary>
        /// To specify to end time
        /// </summary>
        private TimeSpan _endTime;
        [ProtoMember(2)]
        public TimeSpan EndTime
        {
            get
            {
                return _endTime;
            }
            set
            {
                if (value == _endTime)
                    return;
                SetProperty(ref _endTime, value);
            }
        }



    }
}
