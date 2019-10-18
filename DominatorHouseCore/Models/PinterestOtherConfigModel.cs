using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class PinterestOtherConfigModel : BindableBase
    {
        private bool _isScrapDataBeforeSendToPerformActivity;

        [ProtoMember(1)]
        public bool IsScrapDataBeforeSendToPerformActivity
        {
            get
            {
                return _isScrapDataBeforeSendToPerformActivity;
            }
            set
            {
                if (value == _isScrapDataBeforeSendToPerformActivity)
                    return;
                SetProperty(ref _isScrapDataBeforeSendToPerformActivity, value);
            }
        }

        private int _paginationCount = 0;

        [ProtoMember(2)]
        public int PaginationCount
        {
            get
            {
                return _paginationCount;
            }
            set
            {
                if (value == _paginationCount)
                    return;
                SetProperty(ref _paginationCount, value);
            }
        }

        private bool _isCampainWiseUnique = true;
        [ProtoMember(3)]
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
        [ProtoMember(4)]
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
