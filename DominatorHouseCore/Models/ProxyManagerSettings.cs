#region

using DominatorHouseCore.Utility;
using ProtoBuf;

#endregion

namespace DominatorHouseCore.Models
{
    [ProtoContract]
    public class ProxyManagerSettings : BindableBase
    {
        private bool _isNumOfAccountPerProxy;

        [ProtoMember(1)]
        public bool IsNumOfAccountPerProxy
        {
            get => _isNumOfAccountPerProxy;
            set => SetProperty(ref _isNumOfAccountPerProxy, value);
        }

        private int _numOfAccountPerProxy = 1;

        [ProtoMember(2)]
        public int NumOfAccountPerProxy
        {
            get => _numOfAccountPerProxy;
            set => SetProperty(ref _numOfAccountPerProxy, value);
        }

        private bool _dontLogin;

        [ProtoMember(3)]
        public bool DontLogin
        {
            get => _dontLogin;
            set => SetProperty(ref _dontLogin, value);
        }
    }
}