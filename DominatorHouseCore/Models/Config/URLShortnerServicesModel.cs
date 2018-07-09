using DominatorHouseCore.Utility;
using ProtoBuf;

namespace DominatorHouseCore.Models.Config
{
    [ProtoContract]
    public class UrlShortnerServicesModel : BindableBase
    {
        private bool _isBitly;
        [ProtoMember(1)]
        public bool IsBitly
        {
            get
            {
                return _isBitly;
            }
            set
            {
                if(_isBitly == value)
                    return;
                SetProperty(ref _isBitly, value);
            }
        }

        private string _login;
        [ProtoMember(2)]
        public string Login
        {
            get
            {
                return _login;
            }
            set
            {
                if (value == _login)
                    return;
                SetProperty(ref _login, value);
            }
        }
        private string _apiKey;
        [ProtoMember(3)]
        public string ApiKey
        {
            get
            {
                return _apiKey;
            }
            set
            {
                if (value == _apiKey)
                    return;
                SetProperty(ref _apiKey, value);
            }
        }
    }
}
