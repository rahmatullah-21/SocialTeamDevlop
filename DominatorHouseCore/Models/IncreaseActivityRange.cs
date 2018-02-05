using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models
{
    public class IncreaseActivityRange : BindableBase
    {
        public IncreaseActivityRange()
        {

        }

        public IncreaseActivityRange(int increaseActivityCount, int untilMaxCountReached, bool isIncreaseCount) : this()
        {
            this.IncreaseActivityCount = increaseActivityCount;
            this.UntilMaxCountReached = untilMaxCountReached;
            this.IsIncreaseCountChecked = isIncreaseCount;
        }

        private int _increaseActivityCount;

        [ProtoMember(1)]
        public int IncreaseActivityCount
        {
            get
            {
                return _increaseActivityCount;
            }
            set
            {
                if (value == _increaseActivityCount)
                    return;
                SetProperty(ref _increaseActivityCount, value);
            }
        }

        private int _untilMaxCountReached;

        [ProtoMember(2)]
        public int UntilMaxCountReached
        {
            get
            {
                return _untilMaxCountReached;
            }
            set
            {
                if (value == _untilMaxCountReached)
                    return;
                SetProperty(ref _untilMaxCountReached, value);
            }
        }


        private bool _isIncreaseCountChecked;
        [ProtoMember(3)]
        public bool IsIncreaseCountChecked
        {
            get
            {
                return _isIncreaseCountChecked;
            }
            set
            {
                if (value == _isIncreaseCountChecked)
                    return;
                SetProperty(ref _isIncreaseCountChecked, value);
            }
        }

    }
}