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
    }
}
