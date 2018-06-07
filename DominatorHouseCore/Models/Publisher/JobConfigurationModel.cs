using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Models;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher
{
    [ProtoContract]
    public class JobConfigurationModel : BindableBase
    {
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
        private TimeSpan _startTime = new TimeSpan();
        [ProtoMember(2)]
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
        private TimeSpan _endTime = new TimeSpan();
        [ProtoMember(3)]
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
        private RangeUtilities _sendingBetween=new RangeUtilities();
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
        private int _waitAround;
        [ProtoMember(16)]
        public int WaitAround
        {
            get
            {
                return _waitAround;
            }
            set
            {
                if (value == _waitAround)
                    return;
                SetProperty(ref _waitAround, value);

            }
        }
        private bool _isSundayChecked;
        [ProtoMember(17)]
        public bool IsSundayChecked
        {
            get
            {
                return _isSundayChecked;
            }
            set
            {
                if (value == _isSundayChecked)
                    return;
                SetProperty(ref _isSundayChecked, value);
            }
        }
        private bool _isMondayChecked;
        [ProtoMember(18)]
        public bool IsMondayChecked
        {
            get
            {
                return _isMondayChecked;
            }
            set
            {
                if (value == _isMondayChecked)
                    return;
                SetProperty(ref _isMondayChecked, value);
            }
        }
        private bool _isTuesdayChecked;
        [ProtoMember(19)]
        public bool IsTuesdayChecked
        {
            get
            {
                return _isTuesdayChecked;
            }
            set
            {
                if (value == _isTuesdayChecked)
                    return;
                SetProperty(ref _isTuesdayChecked, value);
            }
        }
        private bool _isWednesdayChecked;
        [ProtoMember(20)]
        public bool IsWednesdayChecked
        {
            get
            {
                return _isWednesdayChecked;
            }
            set
            {
                if (value == _isWednesdayChecked)
                    return;
                SetProperty(ref _isWednesdayChecked, value);
            }
        }
        private bool _isThursdayChecked;
        [ProtoMember(21)]
        public bool IsThursdayChecked
        {
            get
            {
                return _isThursdayChecked;
            }
            set
            {
                if (value == _isThursdayChecked)
                    return;
                SetProperty(ref _isThursdayChecked, value);
            }
        }
        private bool _isFridayChecked;
        [ProtoMember(22)]
        public bool IsFridayChecked
        {
            get
            {
                return _isFridayChecked;
            }
            set
            {
                if (value == _isFridayChecked)
                    return;
                SetProperty(ref _isFridayChecked, value);
            }
        }
        private bool _isSaturdayChecked;
        [ProtoMember(23)]
        public bool IsSaturdayChecked
        {
            get
            {
                return _isSaturdayChecked;
            }
            set
            {
                if (value == _isSaturdayChecked)
                    return;
                SetProperty(ref _isSaturdayChecked, value);
            }
        }
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
       
        private List<ContentSelectGroup> _weekday=new List<ContentSelectGroup>();
        [ProtoMember(25)]
        public List<ContentSelectGroup> Weekday
        {
            get { return _weekday; }
            set
            {
                if (value == _weekday)
                    return;
                SetProperty(ref _weekday, value);
            }
        }
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
        private int _publishOn;
        [ProtoMember(27)]
        public int PublishOn
        {
            get
            {
                return _publishOn;
            }
            set
            {
                if (value == _publishOn)
                    return;
                SetProperty(ref _publishOn, value);
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
        private RangeUtilities _delayBetween=new RangeUtilities();
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
        private RangeUtilities _postRange = new RangeUtilities();
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
        private RangeUtilities _delayBetweenPost = new RangeUtilities();
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
    }
}
