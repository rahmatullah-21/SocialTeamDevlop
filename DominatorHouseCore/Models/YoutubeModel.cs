using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    class YoutubeModel : BindableBase
    {
        private int _limitNumberOfSimultaneousWatchVideos;
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
