using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class YoutubeModel : BindableBase
    {
        private int _limitNumberOfSimultaneousWatchVideos = 5;
        [ProtoMember(1)]
        public int LimitNumberOfSimultaneousWatchVideos
        {
            get
            {
                return _limitNumberOfSimultaneousWatchVideos;
            }
            set
            {
                SetProperty(ref _limitNumberOfSimultaneousWatchVideos, value);
            }
        }

        private bool _isCampainWiseUnique = true;
        [ProtoMember(2)]
        public bool IsCampainWiseUnique
        {
            get
            {
                return _isCampainWiseUnique;
            }
            set
            {
                if (value)
                    IsAccountWiseUnique = false;
                SetProperty(ref _isCampainWiseUnique, value);
            }
        }

        private bool _isAccountWiseUnique = true;
        [ProtoMember(3)]
        public bool IsAccountWiseUnique
        {
            get
            {
                return _isAccountWiseUnique;
            }
            set
            {
                if (value)
                    IsCampainWiseUnique = false;
                SetProperty(ref _isAccountWiseUnique, value);
            }
        }

        private int _StopRunQueryIfNActivityFailed = 20;
        [ProtoMember(4)]
        public int StopRunQueryIfNActivityFailed
        {
            get
            {
                return _StopRunQueryIfNActivityFailed;
            }
            set
            {
                SetProperty(ref _StopRunQueryIfNActivityFailed, value);
            }
        }

        private int _timeoutToSolveCaptchaManually = 25;
        [ProtoMember(5)]
        public int TimeoutToSolveCaptchaManually
        {
            get
            {
                return _timeoutToSolveCaptchaManually;
            }
            set
            {
                SetProperty(ref _timeoutToSolveCaptchaManually, value);
            }
        }

        private bool _isCheckActivitiesOnNPost;
        [ProtoMember(6)]
        public bool IsCheckActivitiesOnNPost
        {
            get { return _isCheckActivitiesOnNPost; }
            set { SetProperty(ref _isCheckActivitiesOnNPost, value); }
        }
        private RangeUtilities _activitiesOnNPost = new RangeUtilities(5, 10);
        [ProtoMember(7)]
        public RangeUtilities ActivitiesOnNPost
        {
            get { return _activitiesOnNPost; }
            set { SetProperty(ref _activitiesOnNPost, value); }
        }
    }
}
