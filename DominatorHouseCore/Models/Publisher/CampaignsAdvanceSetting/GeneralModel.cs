using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
   public class GeneralModel : BindableBase
    {
        private int _triggerNotificationCount;
      
        [ProtoMember(1)]
        public int TriggerNotificationCount
        {
            get
            {
                return _triggerNotificationCount;
            }
            set
            {
                if (_triggerNotificationCount == value)
                    return;
                SetProperty(ref _triggerNotificationCount, value);
            }
        }
        private int _destinationTimeout;

        [ProtoMember(2)]
        public int DestinationTimeout
        {
            get
            {
                return _destinationTimeout;
            }
            set
            {
                if (_destinationTimeout == value)
                    return;
                SetProperty(ref _destinationTimeout, value);
            }
        }
        private int _maxPostCountToStore=300;

        [ProtoMember(3)]
        public int MaxPostCountToStore
        {
            get
            {
                return _maxPostCountToStore;
            }
            set
            {
                if (_maxPostCountToStore == value)
                    return;
                SetProperty(ref _maxPostCountToStore, value);
            }
        }
        private int _checkRSSFeedsminutes=30;

        [ProtoMember(4)]
        public int CheckRssFeedsminutes
        {
            get
            {
                return _checkRSSFeedsminutes;
            }
            set
            {
                if (_checkRSSFeedsminutes == value)
                    return;
                SetProperty(ref _checkRSSFeedsminutes, value);
            }
        }
        private int _checkMonitorFoldersminutes=30;

        [ProtoMember(5)]
        public int CheckMonitorFoldersminutes
        {
            get
            {
                return _checkMonitorFoldersminutes;
            }
            set
            {
                if (_checkMonitorFoldersminutes == value)
                    return;
                SetProperty(ref _checkMonitorFoldersminutes, value);
            }
        }
        private bool _isAutomaticallyRetryChecked;

        [ProtoMember(6)]
        public bool IsAutomaticallyRetryChecked
        {
            get
            {
                return _isAutomaticallyRetryChecked;
            }
            set
            {
                if (_isAutomaticallyRetryChecked == value)
                    return;
                SetProperty(ref _isAutomaticallyRetryChecked, value);
            }
        }
        private bool _isChooseSingleRandomImageChecked;

        [ProtoMember(7)]
        public bool IsChooseSingleRandomImageChecked
        {
            get
            {
                return _isChooseSingleRandomImageChecked;
            }
            set
            {
                if (_isChooseSingleRandomImageChecked == value)
                    return;
                SetProperty(ref _isChooseSingleRandomImageChecked, value);
            }
        }
        private bool _isChooseOnlyFirstImageChecked;

        [ProtoMember(8)]
        public bool IsChooseOnlyFirstImageChecked
        {
            get
            {
                return _isChooseOnlyFirstImageChecked;
            }
            set
            {
                if (_isChooseOnlyFirstImageChecked == value)
                    return;
                SetProperty(ref _isChooseOnlyFirstImageChecked, value);
            }
        }
        private bool _isChooseBetweenChecked;

        [ProtoMember(9)]
        public bool IsChooseBetweenChecked
        {
            get
            {
                return _isChooseBetweenChecked;
            }
            set
            {
                if (_isChooseBetweenChecked == value)
                    return;
                SetProperty(ref _isChooseBetweenChecked, value);
            }
        }
        private RangeUtilities _chooseBetween=new RangeUtilities();

        [ProtoMember(10)]
        public RangeUtilities ChooseBetween
        {
            get
            {
                return _chooseBetween;
            }
            set
            {
                if (_chooseBetween == value)
                    return;
                SetProperty(ref _chooseBetween, value);
            }
        }
        private bool _isWhenPublishingSendOnePostChecked;

        [ProtoMember(11)]
        public bool IsWhenPublishingSendOnePostChecked
        {
            get
            {
                return _isWhenPublishingSendOnePostChecked;
            }
            set
            {
                if (_isWhenPublishingSendOnePostChecked == value)
                    return;
                SetProperty(ref _isWhenPublishingSendOnePostChecked, value);
            }
        }
        private bool _isChooseRandomPostsChecked;

        [ProtoMember(12)]
        public bool IsChooseRandomPostsChecked
        {
            get
            {
                return _isChooseRandomPostsChecked;
            }
            set
            {
                if (_isChooseRandomPostsChecked == value)
                    return;
                SetProperty(ref _isChooseRandomPostsChecked, value);
            }
        }
        private bool _isInsertPostsAtRandomChecked;

        [ProtoMember(13)]
        public bool IsInsertPostsAtRandomChecked
        {
            get
            {
                return _isInsertPostsAtRandomChecked;
            }
            set
            {
                if (_isInsertPostsAtRandomChecked == value)
                    return;
                SetProperty(ref _isInsertPostsAtRandomChecked, value);
            }
        }
        private bool _isRunSingleAccountPerCampaign;

        [ProtoMember(14)]
        public bool IsRunSingleAccountPerCampaign
        {
            get
            {
                return _isRunSingleAccountPerCampaign;
            }
            set
            {
                if (_isRunSingleAccountPerCampaign == value)
                    return;
                SetProperty(ref _isRunSingleAccountPerCampaign, value);
            }
        }
        private bool _isRemoveLinkFromPostsChecked;

        [ProtoMember(15)]
        public bool IsRemoveLinkFromPostsChecked
        {
            get
            {
                return _isRemoveLinkFromPostsChecked;
            }
            set
            {
                if (_isRemoveLinkFromPostsChecked == value)
                    return;
                SetProperty(ref _isRemoveLinkFromPostsChecked, value);
            }
        }
        private bool _isDisableTemporarySleep;

        [ProtoMember(16)]
        public bool IsDisableTemporarySleep
        {
            get
            {
                return _isDisableTemporarySleep;
            }
            set
            {
                if (_isDisableTemporarySleep == value)
                    return;
                SetProperty(ref _isDisableTemporarySleep, value);
            }
        }
        private bool _isWaitToStartAction;

        [ProtoMember(17)]
        public bool IsWaitToStartAction
        {
            get
            {
                return _isWaitToStartAction;
            }
            set
            {
                if (_isWaitToStartAction == value)
                    return;
                SetProperty(ref _isWaitToStartAction, value);
            }
        }
        private int _jobProcessRunningCount=1;

        [ProtoMember(18)]
        public int JobProcessRunningCount
        {
            get
            {
                return _jobProcessRunningCount;
            }
            set
            {
                if (_jobProcessRunningCount == value)
                    return;
                SetProperty(ref _jobProcessRunningCount, value);
            }
        }
        private bool _isDeselectUsedDestination;

        [ProtoMember(19)]
        public bool IsDeselectUsedDestination
        {
            get
            {
                return _isDeselectUsedDestination;
            }
            set
            {
                if (_isDeselectUsedDestination == value)
                    return;
                SetProperty(ref _isDeselectUsedDestination, value);
            }
        }
        private bool _isKeepPostsInitialCreationDate;

        [ProtoMember(20)]
        public bool IsKeepPostsInitialCreationDate
        {
            get
            {
                return _isKeepPostsInitialCreationDate;
            }
            set
            {
                if (_isKeepPostsInitialCreationDate == value)
                    return;
                SetProperty(ref _isKeepPostsInitialCreationDate, value);
            }
        }
        private bool _isStopRandomisingDestinationsOrder;

        [ProtoMember(21)]
        public bool IsStopRandomisingDestinationsOrder
        {
            get
            {
                return _isStopRandomisingDestinationsOrder;
            }
            set
            {
                if (_isStopRandomisingDestinationsOrder == value)
                    return;
                SetProperty(ref _isStopRandomisingDestinationsOrder, value);
            }
        }
        private bool _isAutoTagFriends;

        [ProtoMember(22)]
        public bool IsAutoTagFriends
        {
            get
            {
                return _isAutoTagFriends;
            }
            set
            {
                if (_isAutoTagFriends == value)
                    return;
                SetProperty(ref _isAutoTagFriends, value);
            }
        }
        private RangeUtilities _usersForEachPost = new RangeUtilities();

        [ProtoMember(23)]
        public RangeUtilities UsersForEachPost
        {
            get
            {
                return _usersForEachPost;
            }
            set
            {
                if (_usersForEachPost == value)
                    return;
                SetProperty(ref _usersForEachPost, value);
            }
        }
        private int _percentageOfTaggingPost ;

        [ProtoMember(24)]
        public int PercentageOfTaggingPost
        {
            get
            {
                return _percentageOfTaggingPost;
            }
            set
            {
                if (_percentageOfTaggingPost == value)
                    return;
                SetProperty(ref _percentageOfTaggingPost, value);
            }
        }
        [ProtoMember(25)]
        public string CampaignId { get; set; }
        public GeneralModel Clone()
        {

            return (GeneralModel)MemberwiseClone();
        }
    }
}
