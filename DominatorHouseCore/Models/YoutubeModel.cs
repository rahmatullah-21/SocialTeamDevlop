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
    }
}
