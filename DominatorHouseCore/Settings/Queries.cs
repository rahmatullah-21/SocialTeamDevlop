using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Settings
{
    [ProtoContract]
    public class Queries : BindableBase
    {
        private string _queryType;

        [ProtoMember(1)]
        public string QueryType
        {
            get
            {
                return _queryType;
            }
            set
            {
                if (_queryType == null & _queryType == value)
                    return;
                SetProperty(ref _queryType, value);
            }
        }

        private string _query;
        [ProtoMember(2)]
        public string Query
        {
            get
            {
                return _query;
            }
            set
            {
                if (_query == null & _query == value)
                    return;
                SetProperty(ref _query, value);
            }
        }


        private bool _customFilter;
        [ProtoMember(3)]
        public bool CustomFilter
        {
            get { return _customFilter; }
            set
            {
                if (_customFilter == false & _customFilter == value)
                    return;
                SetProperty(ref _customFilter, value);
            }
        }
    }
}
