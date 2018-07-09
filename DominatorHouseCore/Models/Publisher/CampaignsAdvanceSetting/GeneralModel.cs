using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Publisher.CampaignsAdvanceSetting
{
    [ProtoContract]
   public class GeneralModel : BindableBase
    {
        private int _triggerNotificationCount=50;
      
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
        private int _waitMaxOf;

        [ProtoMember(2)]
        public int WaitMaxOf
        {
            get
            {
                return _waitMaxOf;
            }
            set
            {
                if (_waitMaxOf == value)
                    return;
                SetProperty(ref _waitMaxOf, value);
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
        private int _checkRSSFeedsminutes;

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
        private int _checkMonitorFoldersminutes;

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
        private bool _isDoNotPublishPostsChecked;

        [ProtoMember(14)]
        public bool IsDoNotPublishPostsChecked
        {
            get
            {
                return _isDoNotPublishPostsChecked;
            }
            set
            {
                if (_isDoNotPublishPostsChecked == value)
                    return;
                SetProperty(ref _isDoNotPublishPostsChecked, value);
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
        private bool _isWaitToStartNewPost;

        [ProtoMember(17)]
        public bool IsWaitToStartNewPost
        {
            get
            {
                return _isWaitToStartNewPost;
            }
            set
            {
                if (_isWaitToStartNewPost == value)
                    return;
                SetProperty(ref _isWaitToStartNewPost, value);
            }
        }
        private int _waitToStartNewPost=1;

        [ProtoMember(18)]
        public int WaitToStartNewPost
        {
            get
            {
                return _waitToStartNewPost;
            }
            set
            {
                if (_waitToStartNewPost == value)
                    return;
                SetProperty(ref _waitToStartNewPost, value);
            }
        }
        private bool _isUnselectDestination;

        [ProtoMember(19)]
        public bool IsUnselectDestination
        {
            get
            {
                return _isUnselectDestination;
            }
            set
            {
                if (_isUnselectDestination == value)
                    return;
                SetProperty(ref _isUnselectDestination, value);
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
