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
                if (value == _limitNumberOfSimultaneousWatchVideos)
                    return;
                SetProperty(ref _limitNumberOfSimultaneousWatchVideos, value);
            }
        }
    }
}
