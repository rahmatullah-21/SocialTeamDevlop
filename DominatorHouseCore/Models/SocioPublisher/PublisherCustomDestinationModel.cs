using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.SocioPublisher
{
    [ProtoContract]
    public class PublisherCustomDestinationModel : BindableBase
    {

        private string _destinationType=string.Empty;
        [ProtoMember(1)]
        public string DestinationType
        {
            get
            {
                return _destinationType;
            }
            set
            {              
                if (value == _destinationType)
                    return;
                SetProperty(ref _destinationType, value);
            }
        }

        private string _destinationValue = string.Empty;
        [ProtoMember(2)]
        public string DestinationValue
        {
            get
            {
                return _destinationValue;
            }
            set
            {               
                if (value == _destinationValue)
                    return;
                SetProperty(ref _destinationValue, value);
            }
        }
    }
}