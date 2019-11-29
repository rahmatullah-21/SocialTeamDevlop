using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class InstagramUserModel : BindableBase
    {
        private bool _isEnableScrapeDiffrentUserChecked;
        [ProtoMember(1)]
        public bool IsEnableScrapeDiffrentUserChecked
        {
            get
            {
                return _isEnableScrapeDiffrentUserChecked;
            }
            set
            {
                if (value == _isEnableScrapeDiffrentUserChecked)
                    return;
                SetProperty(ref _isEnableScrapeDiffrentUserChecked, value);
            }
        }
    }

}