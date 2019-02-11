using ProtoBuf;

namespace DominatorHouseCore.Utility
{
    [ProtoContract]
    public class RangeUtilities:BindableBase
    {
        public RangeUtilities()
        {

        }

        public RangeUtilities(int start, int end) : this()
        {
            StartValue = start;
            EndValue = end;
        }
        private int _startValue;

         [ProtoMember(1)]
        public int StartValue
        {
            get
            {
                if (_endValue < _startValue)
                    EndValue = _startValue;
                return _startValue;
            }
            set
            {
                if (value == _startValue)
                    return;
                SetProperty(ref _startValue, value);
            }
        }

        private int _endValue;

        [ProtoMember(2)]
        public int EndValue
        {
            get
            {
                return _endValue;
            }
            set
            {
                if (value < _startValue)
                    value = _startValue;
                if (value == _endValue)
                    return;
                SetProperty(ref _endValue, value);
            }
        }

        /// <summary>
        /// IsValidRange Property is return true if endrange is greater that begin range
        /// </summary>
        public bool IsValidRange => EndValue >= StartValue;


        /// <summary>
        /// GetRandom is used to get the random numbers between the Begin and EndValue
        /// </summary>
        /// <returns>Returns a integer value which lies between those two ranges</returns>
        public int GetRandom() => RandomUtilties.GetRandomNumber(EndValue, StartValue);

        /// <summary>
        /// InRange is used to check whether the given numbers in between the StartValue and EndValue
        /// </summary>
        /// <param name="number">number which is used to check in between the ranges</param>
        /// <returns></returns>
        public bool InRange(int number) => number >= StartValue && number <= EndValue;

    }
}
